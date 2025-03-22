[System.Serializable]
public class PlayerGameInfo
{
    // Player info
    public long playerId;
    
    // Position and movement
    public float x;
    public float y;
    public float rotation;
    public float turretRotation;
    public float velocity;
    public bool isMoving;
    
    // Tank state
    public float health;

    public PlayerGameInfo() { }

    public PlayerGameInfo(
        long playerId, 
        float x, float y, float rotation, float turretRotation, float velocity, 
        float health, 
        bool isMoving)
    {
        this.playerId = playerId;
        this.x = x; this.y = y;
        this.rotation = rotation; 
        this.turretRotation = turretRotation; 
        this.velocity = velocity; 
        this.isMoving = isMoving;
        this.health = health;
    }
}