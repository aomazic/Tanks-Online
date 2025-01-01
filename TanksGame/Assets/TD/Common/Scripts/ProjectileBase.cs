using System;
using System.Collections;
using UnityEngine;

public class ProjectileBase : MonoBehaviour, IProjectile
{
    [Header("Projectile Attributes")]
    [SerializeField]
    private float damage;
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private float explosionForce;
    [SerializeField]
    private float penetrationRating;
    [SerializeField]
    private float maxRange;

    [Header("References")]
    [SerializeField]
    private GameObject projectilePrefab;

    
    private ParticleSystem burstParticleEmitter;
    private Vector2 startPosition;
    private const float CheckInterval = 0.1f; 
    private const float ParticleSpeed = 100;
    

    public float Damage
    {
        get => damage;
        set => damage = value;  
    }
    public float ExplosionRadius
    {
        get => explosionRadius;
        set => explosionRadius = value;
    }
    public float ExplosionForce
    {
        get => explosionForce;
        set => explosionForce = value;
    }
    public float PenetrationRating
    {
        get => penetrationRating;
        set => penetrationRating = value;
    }
    public float MaxRange
    {
        get => maxRange;
        set => maxRange = value;
    }
    public GameObject ProjectilePrefab
    {
        get => projectilePrefab;
        set => projectilePrefab = value;
    }

    private void Start()
    {
        burstParticleEmitter = GetComponentInChildren<ParticleSystem>();
        startPosition = transform.position;
        StartCoroutine(CheckMaxRangeCoroutine());
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (penetrationRating > 0)
        {
            Penetrate();
        }
        else
        {
            Explode();
        }
    }

    private IEnumerator CheckMaxRangeCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(CheckInterval);
            CheckMaxRange();
        }
    }

    private void CheckMaxRange()
    {
        if (Vector2.Distance(startPosition, transform.position) > maxRange)
        {
            Explode();
        }
    }
    

    public void Explode()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var col in colliders)
        {
            var target = col.GetComponent<IDamagable>();
            if (target != null)
            {
                ApplyDamageAndForceToEnemy(target);
            }
            else
            {
                ApplyExplosionForce(col.attachedRigidbody, col);
            }
        }
        
        if (burstParticleEmitter)
        {
            burstParticleEmitter.transform.SetParent(null);
            var mainModule = burstParticleEmitter.main; 
            mainModule.startSpeed = ParticleSpeed;
            burstParticleEmitter.Play();
        }
        
        DestroyProjectile();
    }

    private void ApplyDamageAndForceToEnemy(IDamagable target)
    {
        var distance = Vector2.Distance(transform.position, target.Transform.position);
        var damageMultiplier = 1 - distance / explosionRadius;
        var finalDamage = damage * damageMultiplier;

        target.TakeDamage(finalDamage);
        var rb = target.Rigidbody;
        ApplyExplosionForce(rb, target.Collider);
    }

    private void ApplyExplosionForce(Rigidbody2D rb, Collider2D col)
    {
        if (rb)
        {
            Vector2 direction = col.transform.position - transform.position;
            rb.AddForce(direction.normalized * explosionForce, ForceMode2D.Impulse);
        }
    }

    public void Penetrate()
    {
        // Implement penetration logic here
        // For now, just call DestroyProjectile
        DestroyProjectile();
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
