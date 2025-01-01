using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    public LineRenderer laser;
    
    private readonly Dictionary<Collider2D, IDamagable> recentHits = new Dictionary<Collider2D, IDamagable>();
    private readonly Queue<Collider2D> hitOrder = new Queue<Collider2D>();
    private const int MaxRecentHits = 3;
    private float maxLength;
    private float damage = 1;

    private void Start()
    {
        laser = GetComponent<LineRenderer>();
    }

    public void SetLength(float length)
    {
        maxLength = length * 2f;
    }

    private void Update()
    {
        DamageFirstTargetAlongLaser();
    }

    public void TurnOnLaser(float damage)
    {
        this.damage = damage;
        gameObject.SetActive(true);
    }
    
    public void TurnOffLaser()
    {
        laser.SetPosition(0, Vector2.zero);
        laser.SetPosition(1, Vector2.zero);
        gameObject.SetActive(false);
    }

    private void DamageFirstTargetAlongLaser()
    {
        Vector2 laserDirection = transform.up;
        var layerMask = ~LayerMask.GetMask("Ignore Raycast");
        var hit = Physics2D.Raycast(transform.position, laserDirection.normalized, maxLength, layerMask);

        if (hit.collider)
        {
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, hit.point);
            
            if (recentHits.TryGetValue(hit.collider, out var damageable))
            {
                damageable.TakeDamage(damage * Time.deltaTime);
            }
            else
            {
                var target = hit.collider.GetComponent<IDamagable>();
                if (target == null) return;

                if (recentHits.Count >= MaxRecentHits)
                {
                    var oldestHit = hitOrder.Dequeue();
                    recentHits.Remove(oldestHit);
                }
                
                recentHits.Add(hit.collider, target);
                hitOrder.Enqueue(hit.collider);
                target.TakeDamage(damage * Time.deltaTime);
            }
        }
        else
        {
            laser.SetPosition(0, transform.position);
            laser.SetPosition(1, (Vector2)transform.position + laserDirection * maxLength);
        }
    }
}