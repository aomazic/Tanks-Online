using System;
using UnityEngine;

public class TestDummy : MonoBehaviour, IDamagable
{
    
    [Header("Dummy Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private TankConfig tankConfig;
    
    public Transform Transform => transform;
    public Collider2D Collider { get; private set; }
    public Rigidbody2D Rigidbody { get; private set; }
    
    private TankAudio tankAudio;
    
 
    public float Health
    {
        get => health;
        set => health = value;
    }
    
    public event Action<IDamagable> OnDamaged;
    public event Action<IDamagable> OnDestroyed;

    private void Start()
    {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        
        tankAudio = GetComponent<TankAudio>();
        
        tankAudio.Initialize(tankConfig);
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
    
    private void Update()
    {
        HandleAudio();
    }
    
    private void HandleAudio()
    {
        // Update audio
        tankAudio.UpdateEngineAudio(false, 0);
        tankAudio.UpdateTrackAudio(false, 0);
    }
    
    public void Die()
    {
        OnDestroyed?.Invoke(this);
        tankAudio.PlayDestructionAudio();
        Destroy(gameObject);
    }
}