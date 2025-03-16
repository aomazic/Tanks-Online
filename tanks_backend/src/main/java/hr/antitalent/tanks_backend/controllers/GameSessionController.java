package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.GameSession;
import hr.antitalent.tanks_backend.dto.game.GameSessionCreateDTO;
import hr.antitalent.tanks_backend.dto.game.GameSessionUpdateDTO;
import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import hr.antitalent.tanks_backend.services.GameSessionService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.time.Duration;

@RestController
@RequestMapping("/api/game-sessions")
@RequiredArgsConstructor
@Slf4j
public class GameSessionController {
    private final GameSessionService gameSessionService;

    /**
     * Create a new game session.
     *
     * @param dto Game session details.
     * @return ResponseEntity with created GameSession.
     */
    @PostMapping
    public ResponseEntity<GameSession> createGameSession(@Valid @RequestBody GameSessionCreateDTO dto) {
        log.info("Creating game session. Name: '{}', Creator ID: {}",
                dto.name(), dto.hostId());

        try {
            GameSession gameSession = gameSessionService.createGameSession(dto);
            log.info("Game session created successfully. ID: {}, Name: '{}', Status: {}",
                    gameSession.getId(), gameSession.getName(), gameSession.getStatus());
            return new ResponseEntity<>(gameSession, HttpStatus.CREATED);
        } catch (IllegalArgumentException e) {
            log.error("Failed to create game session '{}': {}", dto.name(), e.getMessage());
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
        log.info("Starting game session. ID: {}", gameSessionId);

        try {
            GameSession gameSession = gameSessionService.startGameSession(gameSessionId);
            log.info("Game session started. ID: {}, Name: '{}', Players: {}",
                    gameSession.getId(), gameSession.getName(),
                    gameSession.getPlayers() != null ? gameSession.getPlayers().size() : 0);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            log.error("Failed to start game session {}: {}", gameSessionId, e.getMessage());
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
        log.info("Ending game session. ID: {}", gameSessionId);

        try {
            GameSession gameSession = gameSessionService.endGameSession(gameSessionId, dto);
            log.info("Game session ended. ID: {}, Name: '{}', Duration: {} seconds",
                    gameSession.getId(), gameSession.getName(),
                    Duration.between(gameSession.getStartTime(), gameSession.getEndTime()).getSeconds());
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            log.error("Failed to end game session {}: {}", gameSessionId, e.getMessage());
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
        log.debug("Fetching game session by name: '{}'", gameSessionName);

        try {
            GameSession gameSession = gameSessionService.findGameSessionByName(gameSessionName);
            log.debug("Found game session. Name: '{}', Status: {}, Players: {}",
                    gameSession.getName(), gameSession.getStatus(),
                    gameSession.getPlayers() != null ? gameSession.getPlayers().size() : 0);
            return ResponseEntity.ok(gameSession);
        } catch (IllegalArgumentException e) {
            log.warn("Game session not found: '{}'", gameSessionName);
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
        log.debug("Fetching game sessions by status: {}, Page: {}, Size: {}",
                status, pageable.getPageNumber(), pageable.getPageSize());

        Page<GameSession> gameSessions = gameSessionService.findGameSessionsByStatus(status, pageable);
        log.debug("Found {} game sessions with status: {}",
                gameSessions.getTotalElements(), status);
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
        log.debug("Fetching all game sessions. Page: {}, Size: {}",
                pageable.getPageNumber(), pageable.getPageSize());

        Page<GameSession> gameSessions = gameSessionService.findAllGameSessions(pageable);
        log.debug("Found {} total game sessions", gameSessions.getTotalElements());
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
        log.info("Deleting game session. ID: {}", gameSessionId);

        try {
            gameSessionService.deleteGameSession(gameSessionId);
            log.info("Game session deleted successfully. ID: {}", gameSessionId);
            return ResponseEntity.noContent().build();
        } catch (IllegalArgumentException e) {
            log.error("Failed to delete game session {}: {}", gameSessionId, e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }
}