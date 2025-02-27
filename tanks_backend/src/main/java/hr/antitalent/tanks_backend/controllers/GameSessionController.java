package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.GameSessionCreateDTO;
import hr.antitalent.tanks_backend.dto.GameSessionUpdateDTO;
import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import hr.antitalent.tanks_backend.models.GameSession;
import hr.antitalent.tanks_backend.services.GameSessionService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/game-sessions")
@RequiredArgsConstructor
public class GameSessionController {
    private final GameSessionService gameSessionService;

    private static final Logger logger = LoggerFactory.getLogger(GameSessionController.class);

    /**
     * Create a new game session.
     *
     * @param dto Game session details.
     * @return ResponseEntity with created GameSession.
     */
    @PostMapping
    public ResponseEntity<GameSession> createGameSession(@Valid @RequestBody GameSessionCreateDTO dto) {
        try {
            GameSession gameSession = gameSessionService.createGameSession(dto);
            return new ResponseEntity<>(gameSession, HttpStatus.CREATED);
        } catch (IllegalArgumentException e) {
            logger.error("Error creating game session: {}", e.getMessage());
            return ResponseEntity.badRequest().build();
        }
    }

    /**
     * Start a game session by its ID.
     *
     * @param gameSessionId The ID of the game session.
     * @return ResponseEntity with the updated GameSession.
     */
    @PutMapping("/{gameSessionId}/start")
    public ResponseEntity<GameSession> startGameSession(@PathVariable Long gameSessionId) {
        try {
            GameSession gameSession = gameSessionService.startGameSession(gameSessionId);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            logger.error("Error starting game session: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }

    /**
     * End a game session by its ID and provide winning team and summary.
     *
     * @param gameSessionId The ID of the game session.
     * @param dto           Winning team and summary.
     * @return ResponseEntity with the updated GameSession.
     */
    @PutMapping("/{gameSessionId}/end")
    public ResponseEntity<GameSession> endGameSession(
            @PathVariable Long gameSessionId,
            @Valid @RequestBody GameSessionUpdateDTO dto
    ) {
        try {
            GameSession gameSession = gameSessionService.endGameSession(gameSessionId, dto);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            logger.error("Error ending game session: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }

    /**
     * Update game session settings.
     *
     * @param gameSessionId   The ID of the game session.
     * @param newGameSettings New settings for the game.
     * @return ResponseEntity with updated GameSession.
     */
    @PutMapping("/{gameSessionId}/settings")
    public ResponseEntity<GameSession> updateGameSessionSettings(
            @PathVariable Long gameSessionId,
            @RequestParam String newGameSettings
    ) {
        try {
            GameSession gameSession = gameSessionService.updateGameSessionSettings(gameSessionId, newGameSettings);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            logger.error("Error updating game session settings: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }

    /**
     * Get a game session by name.
     *
     * @param gameSessionName The name of the game session.
     * @return ResponseEntity with the GameSession.
     */
    @GetMapping("/{gameSessionName}")
    public ResponseEntity<GameSession> getGameSessionByName(@PathVariable String gameSessionName) {
        try {
            GameSession gameSession = gameSessionService.findGameSessionByName(gameSessionName);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            logger.error("Game session not found: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }

    /**
     * Get all game sessions by status.
     *
     * @param status   The status of the game sessions.
     * @param pageable Pagination information.
     * @return Paged list of GameSessions.
     */
    @GetMapping("/status/{status}")
    public ResponseEntity<Page<GameSession>> getGameSessionsByStatus(
            @PathVariable GameSessionStatus status,
            Pageable pageable
    ) {
        Page<GameSession> gameSessions = gameSessionService.findGameSessionsByStatus(status, pageable);
        return ResponseEntity.ok(gameSessions);
    }

    /**
     * Get all game sessions.
     *
     * @param pageable Pagination information.
     * @return Paged list of all GameSessions.
     */
    @GetMapping
    public ResponseEntity<Page<GameSession>> getAllGameSessions(Pageable pageable) {
        Page<GameSession> gameSessions = gameSessionService.findAllGameSessions(pageable);
        return ResponseEntity.ok(gameSessions);
    }

    /**
     * Delete a game session by its ID.
     *
     * @param gameSessionId The ID of the game session to be deleted.
     * @return ResponseEntity indicating success or failure.
     */
    @DeleteMapping("/{gameSessionId}")
    public ResponseEntity<Void> deleteGameSession(@PathVariable Long gameSessionId) {
        try {
            gameSessionService.deleteGameSession(gameSessionId);
            return ResponseEntity.noContent().build();
        } catch (IllegalArgumentException e) {
            logger.error("Error deleting game session: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }
}
