using System;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamagable
{
    
    [Header("Dummy Settings")]
    [SerializeField] private float health = 100f;
    
    public Transform Transform => transform;
    public Collider2D Collider { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    
 
    public float Health
    {
        get => health;
        set => health = value;
    }
    
    public event Action<IDamagable> OnDamaged;
    public event Action<IDamagable> OnDestroyed;

    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        OnDamaged?.Invoke(this);
        if (Health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }
}