[System.Serializable]
public class ProjectileEvent
{
    // Player info
    public long playerId;

    // Starting position and trajectory
    public float originX;
    public float originY;
    public float angle;

    // Projectile properties
    public float speed;
    public float damage;
    public string projectileType;
    
    public long timestamp;
}
