using System.Collections;
using UnityEngine;

public class LaserTurretController : TurretControllerBase<LaserTowerEffects>
{
    [Header("Configuration")]
    [SerializeField] private LaserTurretConfig laserTurretConfig;
    [SerializeField] private Laser laserPrefab;
    
    private float currentEnergy;
    private bool isInCooldown;
    private Coroutine firingCoroutine;
    
    private void Start()
    {
        currentEnergy = laserTurretConfig.energyCapacity;
        laserPrefab.SetLength(laserTurretConfig.maxRange);
        laserPrefab.TurnOffLaser();
    }
    
    protected override void Update()
    {
        base.Update();
        RegenerateEnergy();
    }
    
    protected override void HandleFireInput()
    {
        if (!isInCooldown && currentEnergy > 0)
            StartContinuousFiring();
    }
    
    protected override void HandleFireCanceled()
    {
        StopFiring();
    }
    
    private void StartContinuousFiring()
    {
        if (firingCoroutine == null)
            firingCoroutine = StartCoroutine(FireContinuously());
    }
    
    private IEnumerator FireContinuously()
    {
        laserPrefab.TurnOnLaser(laserTurretConfig.laserDamage);
        TowerEffects.StartFiring();
        while (currentEnergy > 0)
        {
            currentEnergy -= Time.deltaTime * laserTurretConfig.energyDepletionRate;
            yield return null;
        }
        StopFiring();
    }
    
    private void StopFiring()
    {
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
        }
        laserPrefab.TurnOffLaser();
        TowerEffects.StopFiring();
        isInCooldown = currentEnergy <= 0;
    }
    
    private void RegenerateEnergy()
    {
        if (currentEnergy < laserTurretConfig.energyCapacity)
        {
            currentEnergy += laserTurretConfig.energyRegenRate * Time.deltaTime;
        }
        else if (isInCooldown)
        {
            isInCooldown = false;
        }
    }
}
