using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ProjectileTurretController : TurretControllerBase<ProjectileCannonEffects>
{
    [Header("Configuration")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private ProjectileTurretConfig projectileTurretConfig;
    
    [Header("References")]
    public TankController tankHullController;
    
    private bool canFire = true;
    private Dictionary<int, (GameObject projectile, Rigidbody2D rb)> projectiles = new Dictionary<int, (GameObject, Rigidbody2D)>();
    
    private void Start()
    {
        TowerEffects = GetComponentInChildren<ProjectileCannonEffects>();
        tankHullController = GetComponentInParent<TankController>();
        
        
        for (var i = 0; i < projectileTurretConfig.totalAmmo; i++)
        {
            var projectile = Instantiate(projectilePrefab);
            var rb = projectile.GetComponent<Rigidbody2D>();
            projectile.SetActive(false);
            projectiles.Add(i, (projectile, rb));
        }
        
        crosshair.SetAmmoText(projectiles.Keys.Count, projectiles.Keys.Count);
    }
    
    protected override void HandleFireInput()
    {
        if (!canFire || projectiles.Count <= 0)
            return;
        
        StartCoroutine(FireCooldown());
        FireProjectile();
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
        
        _ = SendFireMessageAsync(Mathf.Atan2(transform.up.y, transform.up.x));
        
        TowerEffects.Fire();
        
        crosshair.SetAmmoText(projectiles.Keys.Count, projectileTurretConfig.totalAmmo);
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        var elapsedTime = 0f;
        
        while (elapsedTime < projectileTurretConfig.fireRate)
        {
            elapsedTime += Time.deltaTime;
            var progress = elapsedTime / projectileTurretConfig.fireRate;
            crosshair.UpdateCrosshairProgress(progress);
            yield return null;
        }
    
        crosshair.UpdateCrosshairProgress(1f);
        canFire = true;
    }
    
    public void RefillProjectile()
    {
        if (projectiles.Count >= projectileTurretConfig.totalAmmo)
        {
            return;
        }
        
        var projectile = Instantiate(projectilePrefab);
        var rb = projectile.GetComponent<Rigidbody2D>();
        projectile.SetActive(false);
        projectiles.Add(projectiles.Count, (projectile, rb));
        crosshair.SetAmmoText(projectiles.Keys.Count, projectileTurretConfig.totalAmmo);
    }
    
    private async Task SendFireMessageAsync(float angle)
    {
        try
        {
            await playerMpController.SendFireProjectileMessage(
                angle, 
                projectileTurretConfig.projectileSpeed,
                projectileTurretConfig.projectileDamage,
                "standard" // projectile type
            );
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to send fire message: {ex.Message}");
        }
    }
}