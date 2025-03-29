using UnityEngine;

public class nonHostController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform tankTransform;
    [SerializeField] private Transform turretTransform;

    [Header("Settings")]
    [SerializeField] private float positionLerpSpeed = 10f;
    [SerializeField] private float rotationLerpSpeed = 15f;
    
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
        // TODO: ADD fire projectile to the scene
        if (projectileEvent.playerId != playerId)
            return;
        
        Debug.Log($"Remote player {playerId} fired projectile at angle {projectileEvent.angle}");
    }
    
    void Update()
    {
        if (!isInitialized)
            return;
            
        // Smoothly move tank to target position
        tankTransform.position = Vector2.Lerp(
            tankTransform.position, 
            targetPosition, 
            positionLerpSpeed * Time.deltaTime
        );
        
        // Smoothly rotate tank hull
        Vector3 currentRotation = tankTransform.eulerAngles;
        float newZ = Mathf.LerpAngle(
            currentRotation.z, 
            targetTankRotation, 
            rotationLerpSpeed * Time.deltaTime
        );
        tankTransform.eulerAngles = new Vector3(currentRotation.x, currentRotation.y, newZ);
        
        // Smoothly rotate turret
        Vector3 currentTurretRotation = turretTransform.eulerAngles;
        float newTurretZ = Mathf.LerpAngle(
            currentTurretRotation.z, 
            targetTurretRotation, 
            rotationLerpSpeed * Time.deltaTime
        );
        turretTransform.eulerAngles = new Vector3(
            currentTurretRotation.x, 
            currentTurretRotation.y, 
            newTurretZ
        );
        
        // Update physics velocity if needed
        if (tankRigidbody != null && lastPlayerInfo != null)
        {
            // Set velocity to create movement illusion
            Vector2 velocity = lastPlayerInfo.isMoving ? 
                (targetPosition - (Vector2)tankTransform.position).normalized * lastPlayerInfo.velocity : 
                Vector2.zero;
                
            tankRigidbody.linearVelocity = velocity;
        }
    }
}