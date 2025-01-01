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
    
    public IDamagable DamagableComponent => this;
    
    public event System.Action<IEnemy> OnDeath;
    public Transform Transform => transform;
    public Collider2D Collider => GetComponent<Collider2D>();
    public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();
    
    
    public void TakeDamage(float amount)
    {
        Health -= amount;
        if (Health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        OnDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
