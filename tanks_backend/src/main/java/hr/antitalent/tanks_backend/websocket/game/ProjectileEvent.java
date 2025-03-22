package hr.antitalent.tanks_backend.websocket.game;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class ProjectileEvent {
    // Player info
    private String playerId;

    // Starting position and trajectory
    private double originX; 
    private double originY;
    private double angle;

    // Projectile properties
    private float speed;
    private float damage;
    private String projectileType;
    
    private long timestamp;
}
