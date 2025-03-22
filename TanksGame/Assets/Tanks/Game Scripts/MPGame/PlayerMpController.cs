using System.Threading.Tasks;
using UnityEngine;

public class PlayerMpController : MonoBehaviour
{
    [SerializeField] private TankController tankController;
    [SerializeField] private Transform turretTransform;

    private WebSocketController webSocketController;
    private PlayerGameInfo playerInfo;
    private float updateInterval = 0.1f; // 10 updates per second
    private float timeSinceLastUpdate = 0f;
    private bool isInitialized = false;

    void Awake()
    {
        webSocketController = WebSocketController.Instance;
        
        // TODO: move this to another place later
        _ = webSocketController.ConnectToServer();
        
        // Create player info object
        playerInfo = new PlayerGameInfo
        {
            playerId = UserInfoController.GetUserId(),
            health = tankController.Health,
            isMoving = false
        };

        // Subscribe to tank events
        tankController.OnDamaged += HandleTankDamaged;
        tankController.OnDestroyed += HandleTankDestroyed;
    }

    void Start()
    {
        if (isInitialized || webSocketController == null)
        {
            return;
        }

        isInitialized = true;
        Debug.Log("PlayerMpController initialized");
    }

    void OnDestroy()
    {
        if (tankController == null)
        {
            return;
        }

        tankController.OnDamaged -= HandleTankDamaged;
        tankController.OnDestroyed -= HandleTankDestroyed;
    }

    void Update()
    {
        if (!isInitialized || tankController == null)
            return;

        timeSinceLastUpdate += Time.deltaTime;

        if (timeSinceLastUpdate < updateInterval)
        {
            return;
        }

        UpdatePlayerState();
        timeSinceLastUpdate = 0f;
    }

    private void UpdatePlayerState()
    {
        // Update player position and rotation
        Vector2 position = tankController.Transform.position;
        
        playerInfo.x = position.x;
        playerInfo.y = position.y;
        playerInfo.rotation = tankController.Transform.eulerAngles.z;
        playerInfo.turretRotation = turretTransform.eulerAngles.z;
        playerInfo.velocity = tankController.Rigidbody.linearVelocity.magnitude;
        playerInfo.isMoving = playerInfo.velocity > 0.1f;
        playerInfo.health = tankController.Health;

        // Send update to server
        webSocketController.UpdatePlayerState(playerInfo);
    }

    public async Task SendFireProjectileMessage(float angle, float speed, float damage, string projectileType)
    {
        Vector2 origin = turretTransform.position;

        var projectileEvent = new ProjectileEvent
        {
            playerId = UserInfoController.GetUserId(),
            originX = origin.x,
            originY = origin.y,
            angle = angle,
            speed = speed,
            damage = damage,
            projectileType = projectileType,
            timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await webSocketController.FireProjectile(projectileEvent);
    }

    private void HandleTankDamaged(IDamagable damagable)
    {
        // Update health when tank is damaged
        playerInfo.health = (int)tankController.Health;
        UpdatePlayerState();
    }

    private void HandleTankDestroyed(IDamagable damagable)
    {
        // Send final update when tank is destroyed
        playerInfo.health = 0;
        UpdatePlayerState();
    }
}
