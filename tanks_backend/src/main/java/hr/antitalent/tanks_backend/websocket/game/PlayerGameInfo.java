package hr.antitalent.tanks_backend.websocket.game;

import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class PlayerGameInfo {
    // Player info
    private String playerId;
    
    // Position and movement
    private double x;
    private double y;
    private double rotation; 
    private double turretRotation;
    private double velocity;
    private boolean isMoving;

    // Tank state
    private int health;
    private int score;

    // Game state
    private boolean alive;
    private boolean connected;
}