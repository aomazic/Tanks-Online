using System;
using UnityEngine;

public class Slug : MonoBehaviour, IEnemy
{
    [Header("Slug Properties")]
    [SerializeField]
    private float health;

    public float Health
    {
        get => health;
        set => health = value;
    }
    
    public event Action<IDamagable> OnDestroyed;
    public event Action<IDamagable> OnDamaged; 
    public Transform Transform => transform;
    public Collider2D Collider => GetComponent<Collider2D>();
    public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();
    
    
    public void TakeDamage(float amount)
    {
        OnDamaged?.Invoke(this);
        Health -= amount;
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
