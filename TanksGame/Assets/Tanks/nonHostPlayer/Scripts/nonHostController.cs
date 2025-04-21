using UnityEngine;

public class nonHostController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform tankTransform;
    [SerializeField] private Transform turretTransform;
    [SerializeField] private Transform firePoint;

    [Header("Settings")]
    [SerializeField] private float positionLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 15f;
    
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    
    private ProjectileCannonEffects turretEffects;
    private bool isInitialized = false;
    private Vector2 targetPosition;
    private float targetTankRotation;
    private float targetTurretRotation;
    private float lastUpdateTime;
    private Rigidbody2D tankRigidbody;
    private long playerId;
    private PlayerGameInfo lastPlayerInfo;
    
    public void Initialize(long remotePlayerId)
    {
        tankRigidbody = tankTransform.GetComponent<Rigidbody2D>();
        turretEffects = GetComponentInChildren<ProjectileCannonEffects>();
        
        if (isInitialized)
            return;
            
        playerId = remotePlayerId;
        targetPosition = tankTransform.position;
        targetTankRotation = tankTransform.eulerAngles.z;
        targetTurretRotation = turretTransform.eulerAngles.z;
        
        WebSocketController.Instance.OnPlayerGameInfoReceived += HandlePlayerGameInfo;
        WebSocketController.Instance.OnProjectileEventReceived += HandleProjectileEvent;
        
        isInitialized = true;
        Debug.Log($"Non-host player initialized for player ID: {playerId}");
    }
    
    private void OnDestroy()
    {
        if (WebSocketController.Instance != null)
        {
            WebSocketController.Instance.OnPlayerGameInfoReceived -= HandlePlayerGameInfo;
            WebSocketController.Instance.OnProjectileEventReceived -= HandleProjectileEvent;
        }
    }
    
    private void HandlePlayerGameInfo(PlayerGameInfo playerInfo)
    {
        // Only process updates for this specific player
        if (playerInfo.playerId != playerId)
            return;
            
        targetPosition = new Vector2(playerInfo.x, playerInfo.y);
        targetTankRotation = playerInfo.rotation;
        targetTurretRotation = playerInfo.turretRotation;
        lastUpdateTime = Time.time;
        lastPlayerInfo = playerInfo;
    }
    
    private void HandleProjectileEvent(ProjectileEvent projectileEvent)
    {
        // Only process events for this specific player
        if (projectileEvent.playerId != playerId)
            return;
    
        // Create projectile
        var projectile = Instantiate(projectilePrefab);
        var rb = projectile.GetComponent<Rigidbody2D>();
        
        projectile.transform.position = firePoint.position;
        projectile.SetActive(true);
    
        // Calculate direction from angle
        var angle = projectileEvent.angle;
        var fireDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    
        // Set rotation and velocity
        projectile.transform.rotation = Quaternion.LookRotation(Vector3.forward, fireDirection);
        rb.linearVelocity = fireDirection * (projectileEvent.speed > 0 ? projectileEvent.speed : projectileSpeed);
    
        // Play turret effects
        if (turretEffects != null)
        {
            turretEffects.Fire();
        }
        else
        {
            Debug.LogWarning("Turret effects component not assigned to non-host controller");
        }
    
        Debug.Log($"Remote player {playerId} fired projectile at angle {projectileEvent.angle}");
    }
    
    void Update()
    {
        if (!isInitialized)
            return;
        
        UpdatePosition();
        UpdateTankRotation();
        UpdateTurretRotation();
        UpdatePhysicsVelocity();
    }

    private void UpdatePosition()
    {
        // Smoothly move tank to target position
        tankTransform.position = Vector2.Lerp(
            tankTransform.position, 
            targetPosition, 
            positionLerpSpeed * Time.deltaTime
        );
    }

    private void UpdateTankRotation()
    {
        // Smoothly rotate tank hull
        var currentRotation = tankTransform.eulerAngles;
        var newZ = Mathf.LerpAngle(
            currentRotation.z, 
            targetTankRotation, 
            rotationLerpSpeed * Time.deltaTime
        );
        tankTransform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, newZ);
    }

    private void UpdateTurretRotation()
    {
        // Smoothly rotate turret
        var currentTurretRotation = turretTransform.eulerAngles;
        var newTurretZ = Mathf.LerpAngle(
            currentTurretRotation.z, 
            targetTurretRotation, 
            rotationLerpSpeed * Time.deltaTime
        );
        turretTransform.eulerAngles = new Vector3(
            currentTurretRotation.x, 
            currentTurretRotation.y, 
            newTurretZ
        );
    }

    private void UpdatePhysicsVelocity()
    {
        // Update physics velocity if needed
        if (!tankRigidbody || lastPlayerInfo == null)
            return;

        // Set velocity to create movement illusion
        var velocity = lastPlayerInfo.isMoving ? 
            (targetPosition - (Vector2)tankTransform.position).normalized * lastPlayerInfo.velocity : 
            Vector2.zero;

        tankRigidbody.linearVelocity = velocity;
    }}