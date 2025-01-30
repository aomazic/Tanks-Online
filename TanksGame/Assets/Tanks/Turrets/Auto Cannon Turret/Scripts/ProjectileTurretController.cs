using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileTurretController : TurretControllerBase<AutoCannonEffects>
{
    [Header("Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ProjectileTurretConfig projectileTurretConfig;
    
    private bool canFire = true;
    private Dictionary<int, (GameObject projectile, Rigidbody2D rb)> projectiles = new Dictionary<int, (GameObject, Rigidbody2D)>();
    
    private void Start()
    {
        TowerEffects = GetComponentInChildren<AutoCannonEffects>();
        
        
        for (var i = 0; i < projectileTurretConfig.totalAmmo; i++)
        {
            var projectile = Instantiate(projectilePrefab);
            var rb = projectile.GetComponent<Rigidbody2D>();
            projectile.SetActive(false);
            projectiles.Add(i, (projectile, rb));
        }
    }
    protected override void HandleFireInput()
    {
        if(canFire)
        {
            StartCoroutine(FireCooldown());
            FireProjectile();
        }
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
    
    private void FireProjectile()
    {
        var (projectile, rb) = GetNextProjectile();
        if (!projectile) return;
        
        projectile.transform.position = firePoint.position;
        projectile.SetActive(true);

        Vector2 fireDirection = transform.up; 
        projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, fireDirection);
        rb.linearVelocity = fireDirection * projectileTurretConfig.projectileSpeed;

        TowerEffects.Fire();
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(projectileTurretConfig.fireRate);
        canFire = true;
    }
}