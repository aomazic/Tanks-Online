using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public abstract class TurretControllerBase<T> : MonoBehaviour where T : TurretEffects
{
    [Header("Configuration")]
    [SerializeField] protected BaseTurretConfig config;
    [SerializeField] protected Transform firePoint;
    
    [Header("Input")]
    [SerializeField] protected PlayerTankInput tankInput;
    
    [Header("References")]
    [SerializeField] protected MainCrosshair crosshair;
    [SerializeField] protected PlayerMpController playerMpController;
    
    protected Vector2 aimDirection;

    protected T TowerEffects;

    private float currentRotationSpeed;
    private bool shouldRotate;

    protected virtual void Awake()
    {
        tankInput.OnFire += HandleFireInput;
        tankInput.OnFireCanceled += HandleFireCanceled;
        TowerEffects = GetComponentInChildren<T>();
    }
    
    protected virtual void Update()
    {
        HandleAimInput();
        HandleRotation();
    }
    
    protected virtual void HandleFireInput()
    {
    
    }
    
    protected virtual void HandleFireCanceled()
    {
  
    }
    
    private void HandleAimInput()
    {
        aimDirection = (crosshair.currentWorldPosition - (Vector2)transform.position).normalized;
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
        
        var absAngleDifference = Mathf.Abs(angleDifference);
        
        shouldRotate = absAngleDifference > config.aimThreshold;
        
        // Update rotation audio
        crosshair.SetCrosshairRotation(currentRotation, absAngleDifference);
        TowerEffects.UpdateRotationAudio(absAngleDifference, config.rotationSpeed, Mathf.Abs(currentRotationSpeed), shouldRotate);
    }
}