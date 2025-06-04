package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.GameSession;
import hr.antitalent.tanks_backend.dto.game.GameSessionCreateDTO;
import hr.antitalent.tanks_backend.dto.game.GameSessionUpdateDTO;
import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import hr.antitalent.tanks_backend.services.GameSessionService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
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
@Tag(name = "Game Sessions", description = "Endpoints for managing game sessions")
public class GameSessionController {
    private final GameSessionService gameSessionService;

    /**
     * Create a new game session.
     *
     * @param dto Game session details.
     * @return ResponseEntity with created GameSession.
     */
    @Operation(summary = "Create game session", description = "Creates a new game session with the provided details")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "201", description = "Game session created successfully",
                content = @Content(schema = @Schema(implementation = GameSession.class))),
        @ApiResponse(responseCode = "400", description = "Invalid game session data")
    })
    @PostMapping
    public ResponseEntity<GameSession> createGameSession(
            @Parameter(description = "Game session creation details", required = true)
            @Valid @RequestBody GameSessionCreateDTO dto) {
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
    @Operation(summary = "Start game session", description = "Starts an existing game session by its ID")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Game session started successfully",
                content = @Content(schema = @Schema(implementation = GameSession.class))),
        @ApiResponse(responseCode = "404", description = "Game session not found"),
        @ApiResponse(responseCode = "400", description = "Game session cannot be started")
    })
    @PutMapping("/{gameSessionId}/start")
    public ResponseEntity<GameSession> startGameSession(
            @Parameter(description = "ID of the game session to start", required = true)
            @PathVariable Long gameSessionId) {
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
    @Operation(summary = "End game session", description = "Ends a running game session and records the results")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Game session ended successfully",
                content = @Content(schema = @Schema(implementation = GameSession.class))),
        @ApiResponse(responseCode = "404", description = "Game session not found"),
        @ApiResponse(responseCode = "400", description = "Game session cannot be ended")
    })
    @PutMapping("/{gameSessionId}/end")
    public ResponseEntity<GameSession> endGameSession(
            @Parameter(description = "ID of the game session to end", required = true)
            @PathVariable Long gameSessionId,
            @Parameter(description = "Game session update details", required = true)
            @Valid @RequestBody GameSessionUpdateDTO dto) {
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
    @Operation(summary = "Get game session by name", description = "Retrieves a game session by its unique name")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Game session found",
                content = @Content(schema = @Schema(implementation = GameSession.class))),
        @ApiResponse(responseCode = "404", description = "Game session not found")
    })
    @GetMapping("/{gameSessionName}")
    public ResponseEntity<GameSession> getGameSessionByName(
            @Parameter(description = "Name of the game session to retrieve", required = true)
            @PathVariable String gameSessionName) {
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
    @Operation(summary = "Get game sessions by status", description = "Retrieves a paginated list of game sessions with the specified status")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Game sessions retrieved successfully",
                content = @Content(schema = @Schema(implementation = Page.class)))
    })
    @GetMapping("/status/{status}")
    public ResponseEntity<Page<GameSession>> getGameSessionsByStatus(
            @Parameter(description = "Status of the game sessions to retrieve", required = true)
            @PathVariable GameSessionStatus status,
            @Parameter(description = "Pagination parameters")
            Pageable pageable) {
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
    @Operation(summary = "Get all game sessions", description = "Retrieves a paginated list of all game sessions")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Game sessions retrieved successfully",
                content = @Content(schema = @Schema(implementation = Page.class)))
    })
    @GetMapping
    public ResponseEntity<Page<GameSession>> getAllGameSessions(
            @Parameter(description = "Pagination parameters")
            Pageable pageable) {
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
    @Operation(summary = "Delete game session", description = "Deletes a game session by its ID")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "204", description = "Game session deleted successfully"),
        @ApiResponse(responseCode = "404", description = "Game session not found")
    })
    @DeleteMapping("/{gameSessionId}")
    public ResponseEntity<Void> deleteGameSession(
            @Parameter(description = "ID of the game session to delete", required = true)
            @PathVariable Long gameSessionId) {
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

