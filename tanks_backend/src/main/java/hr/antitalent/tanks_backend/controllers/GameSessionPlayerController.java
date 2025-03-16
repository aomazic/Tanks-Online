package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.game.GameSessionPlayerUpdateDTO;
import hr.antitalent.tanks_backend.domain.GameSessionPlayer;
import hr.antitalent.tanks_backend.services.GameSessionPlayerService;
import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/game-sessions/{sessionId}/players")
@RequiredArgsConstructor
public class GameSessionPlayerController {

    private static final Logger logger = LoggerFactory.getLogger(GameSessionPlayerController.class);

    private final GameSessionPlayerService gameSessionPlayerService;

    /**
     * Add a player to a game session.
     */
    @PostMapping("/{userId}")
    public ResponseEntity<GameSessionPlayer> createGameSessionPlayer(
            @PathVariable Long sessionId,
            @PathVariable Long userId) {
        logger.info("Request to add player to session: {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.addPlayerToSession(sessionId, userId));
    }

    /**
     * Get details of a specific player.
     */
    @GetMapping("/{playerId}")
    public ResponseEntity<GameSessionPlayer> getPlayerById(
            @PathVariable Long sessionId,
            @PathVariable Long playerId) {
        logger.info("Fetching player {} in session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getPlayerById(playerId));
    }

    /**
     * Update player details in a session.
     */
    @PutMapping("/{playerId}")
    public ResponseEntity<GameSessionPlayer> updatePlayerInSession(
            @PathVariable Long sessionId,
            @PathVariable Long playerId,
            @RequestBody GameSessionPlayerUpdateDTO dto) {
        logger.info("Updating player {} in session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.updatePlayerGameInfo(playerId, dto));
    }

    /**
     * Player leaves a session.
     */
    @PutMapping("/{playerId}/leave")
    public ResponseEntity<String> removePlayerFromSession(
            @PathVariable Long sessionId,
            @PathVariable Long playerId) {
        logger.info("Player {} is leaving game session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.leaveGameSession(playerId));
    }

    /**
     * List all players in a session.
     */
    @GetMapping
    public ResponseEntity<List<GameSessionPlayer>> getAllPlayersInSession(@PathVariable Long sessionId) {
        logger.info("Fetching all players in session {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getPlayersBySessionId(sessionId));
    }

    /**
     * Get top players by score in a session.
     */
    @GetMapping("/top")
    public ResponseEntity<List<GameSessionPlayer>> getTopPlayersInSession(
            @PathVariable Long sessionId,
            @RequestParam(defaultValue = "5") int limit) {
        logger.info("Fetching top {} players in session {}", limit, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getTopPlayersInSession(sessionId, limit));
    }

    /**
     * Remove all players from a game session.
     */
    @DeleteMapping
    public ResponseEntity<String> removeAllPlayersFromSession(@PathVariable Long sessionId) {
        logger.info("Removing all players from session {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.removeAllPlayersFromSession(sessionId));
    }
}