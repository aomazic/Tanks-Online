using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AutoCannonTower : TowerBase
{
    [Header("Projectile references")]
    [SerializeField]
    private GameObject projectilePrefab;
    
    [FormerlySerializedAs("totalProjectiles")]
    [Header("Auto Cannon Properties")]
    [SerializeField]
    private int totalAmmo = 10;
    [SerializeField]
    // Shots per minute
    private int rpm = 60;
    [SerializeField]
    private float accuracyFallOffRate = 0.1f;
    [SerializeField]
    private float accuracyRegenRate = 0.2f;
    [SerializeField]
    private float launchForce = 10f;

    private Dictionary<int, (GameObject projectile, Rigidbody2D rb)> projectiles = new Dictionary<int, (GameObject, Rigidbody2D)>();
    private float currentAccuracy;
    private float timeBetweenShots;
    private float lastShotTime;
    
    private ProjectileCannonEffects projectileCannonEffects;
    
    private void Start()
    {
        projectileCannonEffects = GetComponentInChildren<ProjectileCannonEffects>();
        setRanges();
        currentAccuracy = Accuracy;
        timeBetweenShots = 60f / rpm;
        
        for (var i = 0; i < totalAmmo; i++)
        {
            var projectile = Instantiate(projectilePrefab);
            var rb = projectile.GetComponent<Rigidbody2D>();
            projectile.SetActive(false);
            projectiles.Add(i, (projectile, rb));
        }
    }

    private void Update()
    {
        if (Target == null)
        {
            FindNewTarget();
        }
        else
        {
            RotateTurretTowardsTarget();
            if (!IsTurretPointedAtTarget())
            {
                return;
            }
            HandleFiring();
        }
        RegenerateAccuracy();
    }

    private void HandleFiring()
    {
        if (Target == null || projectiles.Count <= 0)
        {
            return;
        }

        if (Time.time - lastShotTime < timeBetweenShots)
        {
            return;
        }

        lastShotTime = Time.time;
        Fire();
    } 

    private (GameObject projectile, Rigidbody2D rb) GetNextProjectile()
    {
        var projectileIndex = projectiles.Keys.Count - 1;
        if (projectiles.ContainsKey(projectileIndex))
        {
            var projectileData = projectiles[projectileIndex];
            projectiles.Remove(projectileIndex);
            return projectileData;
        }
        return (null, null);
    }
    
    public override void Fire()
    {
        var (projectile, rb) = GetNextProjectile();
        if (!projectile) return;

        projectileCannonEffects.Fire();
        LaunchProjectile(projectile, rb);

        currentAccuracy -= accuracyFallOffRate;
        if (currentAccuracy < 0)
        {
            currentAccuracy = 0;
        }
    }
    
    private void LaunchProjectile(GameObject projectile, Rigidbody2D rb)
    {
        projectile.transform.position = firePoint.position;
        projectile.SetActive(true);

        Vector2 direction = (Target.Transform.position - firePoint.position).normalized;
        var sway = Random.Range(-5f, 5f) * (1f - currentAccuracy / 100f);
        direction = Quaternion.Euler(0, 0, sway) * direction;
        
        projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        rb.linearVelocity = direction * launchForce;
    }
    
    private void RegenerateAccuracy()
    {
        currentAccuracy += accuracyRegenRate * Time.deltaTime;
        if (currentAccuracy > Accuracy)
        {
            currentAccuracy = Accuracy;
        }
    }
}