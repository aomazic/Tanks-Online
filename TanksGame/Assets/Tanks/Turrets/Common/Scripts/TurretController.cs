using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretController : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private TurretConfig config;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Input")]
    [SerializeField] private PlayerTankInput tankInput;
    
    private Camera mainCamera;
    private float currentRotationSpeed;
    private bool isRotating;
    private bool canFire = true;
    private Vector2 aimDirection;
    private AutoCannonEffects towerEffects;
    
    private Dictionary<int, (GameObject projectile, Rigidbody2D rb)> projectiles = new Dictionary<int, (GameObject, Rigidbody2D)>();
    
    private void Awake()
    {
        mainCamera = Camera.main;
        tankInput.OnFire += HandleFireInput;
    }

    private void Start()
    {
        towerEffects = GetComponentInChildren<AutoCannonEffects>();
        
        
        for (var i = 0; i < config.totalAmmo; i++)
        {
            var projectile = Instantiate(projectilePrefab);
            var rb = projectile.GetComponent<Rigidbody2D>();
            projectile.SetActive(false);
            projectiles.Add(i, (projectile, rb));
        }
    }

    private void Update()
    {
        HandleAimInput();
        HandleRotation();
    }

    private void HandleAimInput()
    {
        var mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        aimDirection = (mousePosition - transform.position).normalized;
    }

    private void HandleRotation()
    {
        var targetRotation = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        var currentRotation = transform.rotation.eulerAngles.z;
        var angleDifference = Mathf.DeltaAngle(currentRotation, targetRotation);
        
        // Smooth rotation acceleration
        currentRotationSpeed = Mathf.Lerp(
            currentRotationSpeed, 
            Mathf.Clamp(angleDifference, -config.rotationSpeed, config.rotationSpeed),
            config.rotationAcceleration * Time.deltaTime
        );

        // Apply rotation
        transform.Rotate(Vector3.forward, currentRotationSpeed * Time.deltaTime);
        
        var shouldRotate = angleDifference > config.aimThreshold;
        // Update rotation audio
        towerEffects.UpdateRotationAudio(Mathf.Abs(angleDifference), config.rotationSpeed, currentRotationSpeed, shouldRotate);
    }
    

    private void HandleFireInput()
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
        rb.linearVelocity = fireDirection * config.projectileSpeed;

        towerEffects.Fire();
    }

    private IEnumerator FireCooldown()
    {
        canFire = false;
        yield return new WaitForSeconds(config.fireRate);
        canFire = true;
    }
}