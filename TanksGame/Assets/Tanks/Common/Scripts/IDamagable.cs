using System;
using UnityEngine;

public interface IDamagable
{
    Transform Transform { get; }
    Collider2D Collider { get; }
    Rigidbody2D Rigidbody { get; }
    float Health { get; set; }
    void TakeDamage(float damage);
    void Die();
    event Action<IDamagable>  OnDamaged;
    event Action<IDamagable> OnDestroyed;
}
