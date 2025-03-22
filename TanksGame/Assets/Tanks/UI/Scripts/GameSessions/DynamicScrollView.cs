using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class DynamicScrollView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameRoomItem itemPrefab;
    [SerializeField] private RectTransform content;
    
    private List<GameRoomItem> roomItems = new List<GameRoomItem>();
    
    public event System.Action<int> OnRoomSelected;
    
    private void ClearItems()
    {
        foreach (var item in roomItems)
        {
            if (!item)
            {
                return;
            }
            item.OnClick -= HandleRoomItemClick;
            Destroy(item.gameObject);
        }
        roomItems.Clear();
    }

    public void FillData(List<GameSession> data)
    {
        ClearItems();

        if (data.Count == 0) return;

        for (int i = 0; i < data.Count; i++)
        {
            var item = Instantiate(itemPrefab, content);
            item.SetData(data[i], i);
            item.OnClick += HandleRoomItemClick;
            roomItems.Add(item);
        }
    }
    
    private void HandleRoomItemClick(int index)
    {
        OnRoomSelected?.Invoke(index);
    }
    
    private void OnDestroy()
    {
        ClearItems();
    }
}