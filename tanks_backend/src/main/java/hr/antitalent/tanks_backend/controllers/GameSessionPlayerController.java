package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.game.GameSessionPlayerUpdateDTO;
import hr.antitalent.tanks_backend.domain.GameSessionPlayer;
import hr.antitalent.tanks_backend.services.GameSessionPlayerService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.ArraySchema;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/api/game-sessions/{sessionId}/players")
@RequiredArgsConstructor
@Tag(name = "Game Session Players", description = "Endpoints for managing players within game sessions")
public class GameSessionPlayerController {

    private static final Logger logger = LoggerFactory.getLogger(GameSessionPlayerController.class);

    private final GameSessionPlayerService gameSessionPlayerService;

    /**
     * Add a player to a game session.
     */
    @Operation(summary = "Add player to game session", description = "Adds a player to an existing game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Player added to session successfully",
                content = @Content(schema = @Schema(implementation = GameSessionPlayer.class))),
        @ApiResponse(responseCode = "404", description = "Game session not found"),
        @ApiResponse(responseCode = "400", description = "Player could not be added to session")
    })
    @PostMapping("/{userId}")
    public ResponseEntity<GameSessionPlayer> createGameSessionPlayer(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "ID of the user to add as player", required = true) @PathVariable Long userId) {
        logger.info("Request to add player to session: {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.addPlayerToSession(sessionId, userId));
    }

    /**
     * Get details of a specific player.
     */
    @Operation(summary = "Get player details", description = "Retrieves details of a specific player in a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Player found",
                content = @Content(schema = @Schema(implementation = GameSessionPlayer.class))),
        @ApiResponse(responseCode = "404", description = "Player not found in session")
    })
    @GetMapping("/{playerId}")
    public ResponseEntity<GameSessionPlayer> getPlayerById(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "ID of the player to retrieve", required = true) @PathVariable Long playerId) {
        logger.info("Fetching player {} in session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getPlayerById(playerId));
    }

    /**
     * Update player details in a session.
     */
    @Operation(summary = "Update player in session", description = "Updates a player's details in a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Player updated successfully",
                content = @Content(schema = @Schema(implementation = GameSessionPlayer.class))),
        @ApiResponse(responseCode = "404", description = "Player not found in session"),
        @ApiResponse(responseCode = "400", description = "Invalid update data")
    })
    @PutMapping("/{playerId}")
    public ResponseEntity<GameSessionPlayer> updatePlayerInSession(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "ID of the player to update", required = true) @PathVariable Long playerId,
            @Parameter(description = "Updated player information", required = true) @RequestBody GameSessionPlayerUpdateDTO dto) {
        logger.info("Updating player {} in session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.updatePlayerGameInfo(playerId, dto));
    }

    /**
     * Player leaves a session.
     */
    @Operation(summary = "Player leaves session", description = "Removes a player from a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Player removed from session successfully",
                content = @Content(schema = @Schema(implementation = String.class))),
        @ApiResponse(responseCode = "404", description = "Player not found in session")
    })
    @PutMapping("/{playerId}/leave")
    public ResponseEntity<String> removePlayerFromSession(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "ID of the player to remove", required = true) @PathVariable Long playerId) {
        logger.info("Player {} is leaving game session {}", playerId, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.leaveGameSession(playerId));
    }

    /**
     * List all players in a session.
     */
    @Operation(summary = "Get all players in session", description = "Retrieves a list of all players in a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Players retrieved successfully",
                content = @Content(array = @ArraySchema(schema = @Schema(implementation = GameSessionPlayer.class))))
    })
    @GetMapping
    public ResponseEntity<List<GameSessionPlayer>> getAllPlayersInSession(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId) {
        logger.info("Fetching all players in session {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getPlayersBySessionId(sessionId));
    }

    /**
     * Get top players by score in a session.
     */
    @Operation(summary = "Get top players by score", description = "Retrieves the top-scoring players in a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Top players retrieved successfully",
                content = @Content(array = @ArraySchema(schema = @Schema(implementation = GameSessionPlayer.class))))
    })
    @GetMapping("/top")
    public ResponseEntity<List<GameSessionPlayer>> getTopPlayersInSession(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "Number of top players to retrieve") @RequestParam(defaultValue = "5") int limit) {
        logger.info("Fetching top {} players in session {}", limit, sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.getTopPlayersInSession(sessionId, limit));
    }

    /**
     * Remove all players from a game session.
     */
    @Operation(summary = "Remove all players from session", description = "Removes all players from a game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "All players removed from session successfully",
                content = @Content(schema = @Schema(implementation = String.class))),
        @ApiResponse(responseCode = "404", description = "Game session not found")
    })
    @DeleteMapping
    public ResponseEntity<String> removeAllPlayersFromSession(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId) {
        logger.info("Removing all players from session {}", sessionId);
        return ResponseEntity.ok(gameSessionPlayerService.removeAllPlayersFromSession(sessionId));
    }
}

