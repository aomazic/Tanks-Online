using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class DynamicScrollView : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float itemSpacing = 10f;
    [SerializeField] private float buffer = 100f;

    [Header("References")]
    [SerializeField] private ScrollItem itemPrefab;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;
    
    private float itemHeight;
    private float viewportHeight;
    private int visibleItemsCount;
    private int currentTopIndex;
    
    private Pool<ScrollItem> itemPool;
    
    public void FillData(List<GameSession> data)
    {
        if (data.Count == 0) return;

        foreach (var dataPoint in data)
        {
            var item = Instantiate(itemPrefab, content);
            item.SetData(dataPoint);
        }
    }
}