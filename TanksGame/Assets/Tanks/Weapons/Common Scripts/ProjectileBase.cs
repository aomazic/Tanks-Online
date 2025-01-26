using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ProjectileBase : MonoBehaviour, IProjectile
{
    [Header("Projectile Settings")]
    [SerializeField] private ProjectileStats stats;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ParticleSystem burstParticleEmitter;
    [SerializeField] private LayerMask collisionMask;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private float currentPenetration;
    private bool isQuitting;

    public ProjectileStats Stats => stats;
    public GameObject ProjectilePrefab => projectilePrefab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPenetration = stats.PenetrationRating;
    }

    private void Start()
    {
        startPosition = transform.position;
        StartCoroutine(CheckMaxRangeCoroutine());
    }

    private void OnApplicationQuit() => isQuitting = true;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (((1 << other.gameObject.layer) & collisionMask) == 0) return;

        if (currentPenetration > 0)
        {
            Penetrate();
            currentPenetration--;
        }
        else
        {
            Explode();
        }
    }

    private IEnumerator CheckMaxRangeCoroutine()
    {
        var wait = new WaitForSeconds(0.1f);
        while (enabled)
        {
            if (Vector2.Distance(startPosition, transform.position) > stats.MaxRange)
            {
                Explode();
                yield break;
            }
            yield return wait;
        }
    }

    public void Explode()
    {
        if (isQuitting) return;

        var position = transform.position;
        var colliders = Physics2D.OverlapCircleAll(position, stats.ExplosionRadius);

        foreach (var col in colliders)
        {
            if (col.gameObject == gameObject) continue;

            ApplyDamage(col.transform.position);
            ApplyForce(col);
        }

        PlayParticleEffect();
        DestroyProjectile();
    }

    private void ApplyDamage(Vector3 targetPosition)
    {
        var damagable = GetComponentInParent<IDamagable>();
        if (damagable == null) return;

        var distance = Vector2.Distance(transform.position, targetPosition);
        var damageMultiplier = Mathf.Clamp01(1 - distance / stats.ExplosionRadius);
        damagable.TakeDamage(stats.Damage * damageMultiplier);
    }

    private void ApplyForce(Collider2D col)
    {
        if (!col.attachedRigidbody) return;

        var direction = (Vector2)(col.transform.position - transform.position);
        col.attachedRigidbody.AddForce(direction.normalized * stats.ExplosionForce, ForceMode2D.Impulse);
    }

    private void PlayParticleEffect()
    {
        if (!burstParticleEmitter) return;

        burstParticleEmitter.transform.SetParent(null);
        burstParticleEmitter.Play();
        Destroy(burstParticleEmitter.gameObject, burstParticleEmitter.main.duration);
    }

    public void Penetrate()
    {
        // Add penetration logic here
        // Example: Continue moving with reduced damage
        // For now just continue without destroying
    }

    public void DestroyProjectile()
    {
        if (isQuitting) return;
        Destroy(gameObject);
    }
}