using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class RecyclableScrollView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float itemSpacing = 10f;
    [SerializeField] private float buffer = 100f;

    [Header("References")]
    [SerializeField] private ScrollItem itemPrefab;
    [SerializeField] private RectTransform content;
    
    private List<GameSession> data = new List<GameSession>();
    private float itemHeight;
    private float viewportHeight;
    private int visibleItemsCount;
    private int currentTopIndex;
    
    private Pool<ScrollItem> itemPool;
    private readonly List<ScrollItem> activeItems = new List<ScrollItem>();

    public event System.Action<int> OnItemClicked;

    private void Start()
    {
        GetComponent<ScrollRect>().onValueChanged.AddListener(OnScroll);
        InitializePool();
    }

    private void InitializePool()
    {
        itemHeight = itemPrefab.GetComponent<RectTransform>().rect.height + itemSpacing;
        viewportHeight = GetComponent<RectTransform>().rect.height;
        visibleItemsCount = Mathf.CeilToInt((viewportHeight + buffer) / itemHeight);
        
        itemPool = new Pool<ScrollItem>(() => 
        {
            var item = Instantiate(itemPrefab, content);
            item.OnClick += HandleItemClick;
            return item;
        }, visibleItemsCount);
    }

    public void FillData(List<GameSession> data)
    {
        this.data = data;
        UpdateContentSize();
        RecycleItems(true);
    }

    private void UpdateContentSize()
    {
        content.sizeDelta = new Vector2(
            content.sizeDelta.x,
            Mathf.Max(viewportHeight, data.Count * itemHeight)
        );
    }

    private void OnScroll(Vector2 scrollPos)
    {
        RecycleItems();
    }

    private void RecycleItems(bool forceUpdate = false)
    {
        if (data.Count == 0) return;

        var contentY = content.anchoredPosition.y;
        var newTopIndex = Mathf.FloorToInt(contentY / itemHeight);

        if (newTopIndex == currentTopIndex && !forceUpdate) return;
        currentTopIndex = newTopIndex;

        var startIndex = Mathf.Max(0, currentTopIndex - 1);
        var endIndex = Mathf.Min(data.Count - 1, startIndex + visibleItemsCount + 2);

        // Return out-of-view items
        for (var i = activeItems.Count - 1; i >= 0; i--)
        {
            if (activeItems[i].Index < startIndex || activeItems[i].Index > endIndex)
            {
                itemPool.Return(activeItems[i]);
                activeItems.RemoveAt(i);
            }
        }

        // Create new items
        for (var i = startIndex; i <= endIndex; i++)
        {
            if (!HasActiveItem(i))
            {
                var item = itemPool.Get();
                item.transform.SetSiblingIndex(i);
                item.SetData(i, data[i]);
                item.RectTransform.anchoredPosition = new Vector2(0, -i * itemHeight);
                activeItems.Add(item);
            }
        }
    }

    private bool HasActiveItem(int index)
    {
        foreach (var item in activeItems)
        {
            if (item.Index == index) return true;
        }
        return false;
    }

    private void HandleItemClick(int index)
    {
        OnItemClicked?.Invoke(index);
    }
}