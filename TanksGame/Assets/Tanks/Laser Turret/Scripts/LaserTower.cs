using System.Collections;
using UnityEngine;

public class LaserTower : TowerBase
{
    [Header("Laser Tower Properties")]
    [SerializeField]
    private float energyDepletionRate;
    [SerializeField]
    private float energyRegenRate;
    [SerializeField]
    private float energyCapacity;
    [SerializeField]
    private float laserDamage;

    [Header("Laser Tower References")]
    [SerializeField]
    private Laser laserPrefab;

    private float currentEnergy;
    private bool isInCooldown;
    private bool isFiring;
    private bool isRotating;
    private Vector3 lastTargetPosition;
    private Coroutine firingCoroutine;
    private LaserTowerEffects laserEffects;

    private void Start()
    {
        setRanges();
        isFiring = false;
        laserPrefab.SetLength(MaximumRange);
        isInCooldown = false;
        laserPrefab.TurnOffLaser();
        currentEnergy = energyCapacity;
        laserEffects = GetComponentInChildren<LaserTowerEffects>();
    }

    private void Update()
    {
        if (Target == null)
        {
            laserEffects.StopRotation();
            FindNewTarget();
        }
        else
        {
            RotateTurretTowardsTarget();
            HandleFiring();
        }
        RegenerateEnergy();
    }

    private void HandleFiring()
    {
        if (isInCooldown)
        {
            return;
        }

        if (currentEnergy > 0 && IsTurretPointedAtTarget())
        {
            if (!isFiring)
            {
                isFiring = true;
                laserEffects.StartFiring();
            }
            else
            {
                currentEnergy -= Time.deltaTime * energyDepletionRate;
            }
        }
        else
        {
            if (firingCoroutine == null)
            {
                return;
            }
            StopCoroutine(firingCoroutine);
            firingCoroutine = null;
            isFiring = false;

            laserPrefab.TurnOffLaser();
            laserEffects.StopFiring();

            if (currentEnergy <= 0)
            {
                isInCooldown = true;
            }
        }
    }
    
    public void StartContinuousFiring()
    {
        firingCoroutine ??= StartCoroutine(FireContinuously());
    }
    
    private IEnumerator FireContinuously()
    {
        laserPrefab.laser.enabled = true;
        while (currentEnergy > 0)
        {
            Fire();
            yield return null;
        }
    }

    private void RegenerateEnergy()
    {
        if (currentEnergy < energyCapacity)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
        }
        else if (isInCooldown)
        {
            isInCooldown = false;
        }
    }

    public override void Fire()
    {
        laserPrefab.TurnOnLaser(laserDamage);
    }
}