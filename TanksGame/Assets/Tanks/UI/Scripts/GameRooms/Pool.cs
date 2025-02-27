using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : Component
{
    private Queue<T> available = new Queue<T>();
    private List<T> all = new List<T>();
    private System.Func<T> createFunc;

    public Pool(System.Func<T> createFunc, int initialSize)
    {
        this.createFunc = createFunc;
        for (int i = 0; i < initialSize; i++)
        {
            var obj = this.createFunc();
            obj.gameObject.SetActive(false);
            available.Enqueue(obj);
            all.Add(obj);
        }
    }

    public T Get()
    {
        if (available.Count == 0)
        {
            var obj = createFunc();
            all.Add(obj);
            available.Enqueue(obj);
        }
            
        var item = available.Dequeue();
        item.gameObject.SetActive(true);
        return item;
    }

    public void Return(T item)
    {
        item.gameObject.SetActive(false);
        available.Enqueue(item);
    }
}