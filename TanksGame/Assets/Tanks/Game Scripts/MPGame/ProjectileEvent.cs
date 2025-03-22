[System.Serializable]
public class ProjectileEvent
{
    // Player info
    public long playerId;

    // Starting position and trajectory
    public double originX;
    public double originY;
    public double angle;

    // Projectile properties
    public float speed;
    public float damage;
    public string projectileType;
    
    public long timestamp;
}
