using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class TowerBase : MonoBehaviour, ITower
{
    [Header("Tower Properties")]
    [SerializeField]
    private float maxRange;
    [SerializeField]
    private float minRange;
    [SerializeField]
    private float accuracy;
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float health;
    
    [Header("Tower References")]
    [SerializeField]
    public Transform firePoint;
    [SerializeField]
    private Transform turret;
    
    
    public float MaximumRange
    {
        get => maxRange;
        set => maxRange = value;
    }
    
    public float MinimumRange
    {
        get => minRange;
        set => minRange = value;
    }
    
    public float Accuracy
    {
        get => accuracy;
        set => accuracy = value; 
    }
    
    public float TurnSpeed
    {
        get => turnSpeed;
        set => turnSpeed = value;
    }
    
    public Transform FirePoint
    {
        get => firePoint;
        set => firePoint = value;
    }
    
    public float Health
    {
        get => health;
        set => health = value;
    }
    
    public Transform Transform => transform;
    public Collider2D Collider => GetComponent<Collider2D>();
    public Rigidbody2D Rigidbody => GetComponent<Rigidbody2D>();
    public ITowerEffects TowerEffects  => GetComponentInChildren<ITowerEffects>();
    public event Action<IDamagable> OnDestroyed;
    public event Action<IDamagable> OnDamaged; 
    
    public IEnemy Target { get; set; }
    public HashSet<IEnemy> enemiesInMaxRange { get; set; } = new HashSet<IEnemy>();
    
    public virtual void setRanges()
    {
        var colliders = GetComponentsInChildren<CircleCollider2D>();
        if (colliders.Length >= 2)
        {
            colliders[0].radius = maxRange;
            colliders[1].radius = minRange;
        }
        else
        {
            Debug.LogError("Not enough CircleCollider2D components found.");
        }
    }
    
    public virtual void NewEnemyDetected(IEnemy enemy)
    {
        enemiesInMaxRange.Add(enemy);
        if (Target == null)
        {
            SetTarget(enemy);
        }
    }

    public virtual void EnemyOutOfRange(IEnemy enemy)
    {
        enemiesInMaxRange.Remove(enemy);
        if (Target == enemy)
        {
            Target = null;
        }
    }

    public virtual void FindNewTarget()
    {
        if (enemiesInMaxRange.Count == 0)
        {
            return;
        }

        SetTarget(enemiesInMaxRange.FirstOrDefault());
    }
    
    public virtual void SetTarget(IEnemy target)
    {
        Target = target;

        // Subscribe to the new target's OnDeath event
        if (Target != null)
        {
            Target.OnDestroyed += HandleTargetDeath;
        }
    }
    
    public virtual void RotateTurretTowardsTarget()
    {
        var direction = (Target.Transform.position - firePoint.position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; 
        var targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
    
        if (turret.rotation != targetRotation)
        {
            turret.rotation = Quaternion.RotateTowards(turret.rotation, targetRotation, TurnSpeed * Time.deltaTime);
        }
    }
    public virtual bool IsTurretPointedAtTarget()
    {
        if (Target == null)
        {
            return false;
        }

        var directionToTarget = (Target.Transform.position - firePoint.position).normalized;
        var angleToTarget = Vector2.Angle(firePoint.up, directionToTarget);
        
        const float thresholdAngle = 7f;

        if (angleToTarget <= thresholdAngle)
        {
            return true;
        }

        return false;
    }
    
    public virtual void HandleTargetDeath(IDamagable enemy)
    {
        if (Target != enemy)
        {
            return;
        }
        // Unsubscribe from the dead target's OnDeath event
        Target.OnDestroyed -= HandleTargetDeath;
        Target = null;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
    
    public void Die()
    {
        Destroy(gameObject);
    }
    public abstract void Fire();
}