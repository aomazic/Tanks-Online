package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.GameEvent;
import hr.antitalent.tanks_backend.services.GameEventService;
import hr.antitalent.tanks_backend.websocket.game.PlayerGameInfo;
import hr.antitalent.tanks_backend.websocket.game.ProjectileEvent;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.ArraySchema;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import java.util.List;

@Controller
@RequiredArgsConstructor
@Slf4j
@Tag(name = "Game", description = "Endpoints for real-time game interactions and event management")
public class GameController {
    private final SimpMessagingTemplate messagingTemplate;
    private final GameEventService gameEventService;

    private static final String GAME_TOPIC_TEMPLATE = "/topic/game/%s";
    private static final String PLAYER_UPDATE_TOPIC_TEMPLATE = "/topic/game/players/%s";
    private static final String PROJECTILE_TOPIC_TEMPLATE = "/topic/game/projectiles/%s";

    @MessageMapping("/game.join/{sessionId}")
    public void playerJoinGame(
            @DestinationVariable String sessionId,
            @Payload GameEvent gameEvent,
            SimpMessageHeaderAccessor headerAccessor) {

        if (headerAccessor.getSessionAttributes() != null) {
            headerAccessor.getSessionAttributes().put("gameSessionId", sessionId);
        }

        log.info("Player joined game session: {}", sessionId);

        messagingTemplate.convertAndSend(
                String.format(GAME_TOPIC_TEMPLATE, sessionId),
                gameEvent
        );
    }

    @MessageMapping("/game.update/{sessionId}")
    public void updatePlayerState(
            @DestinationVariable String sessionId,
            @Payload PlayerGameInfo playerInfo) {

        log.debug("Player update in session {}: {} at position ({}, {})",
                sessionId, playerInfo.getPlayerId(), playerInfo.getX(), playerInfo.getY());

        messagingTemplate.convertAndSend(
                String.format(PLAYER_UPDATE_TOPIC_TEMPLATE, sessionId),
                playerInfo
        );
    }

    @MessageMapping("/game.fire/{sessionId}")
    public void handleProjectile(
            @DestinationVariable String sessionId,
            @Payload ProjectileEvent projectileEvent) {

        log.debug("Projectile fired in session {}: player {} at ({}, {})",
                sessionId, projectileEvent.getPlayerId(),
                projectileEvent.getOriginX(), projectileEvent.getOriginY());

        messagingTemplate.convertAndSend(
                String.format(PROJECTILE_TOPIC_TEMPLATE, sessionId),
                projectileEvent
        );
    }

    @Operation(summary = "Create game event", description = "Records a new game event for a specified session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "201", description = "Event created successfully",
                content = @Content(schema = @Schema(implementation = GameEvent.class))),
        @ApiResponse(responseCode = "400", description = "Invalid event data")
    })
    @PostMapping("/api/game-sessions/{sessionId}/events")
    public ResponseEntity<GameEvent> createGameEvent(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId,
            @Parameter(description = "Event details", required = true) @RequestBody GameEvent gameEvent) {
        log.info("Creating game event for session: {}", sessionId);
        try {
            GameEvent savedEvent = gameEventService.saveGameEvent(gameEvent, sessionId);
            return new ResponseEntity<>(savedEvent, HttpStatus.CREATED);
        } catch (Exception e) {
            log.error("Error creating game event: {}", e.getMessage(), e);
            return ResponseEntity.badRequest().build();
        }
    }

    @Operation(summary = "Get game events", description = "Retrieves all events for a specific game session")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Events retrieved successfully",
                content = @Content(array = @ArraySchema(schema = @Schema(implementation = GameEvent.class)))),
        @ApiResponse(responseCode = "404", description = "Game session not found")
    })
    @GetMapping("/api/game-sessions/{sessionId}/events")
    public ResponseEntity<List<GameEvent>> getGameEvents(
            @Parameter(description = "ID of the game session", required = true) @PathVariable Long sessionId) {
        log.info("Fetching game events for session: {}", sessionId);
        try {
            List<GameEvent> events = gameEventService.findEventsBySessionId(sessionId);
            return ResponseEntity.ok(events);
        } catch (Exception e) {
            log.error("Error retrieving game events: {}", e.getMessage(), e);
            return ResponseEntity.notFound().build();
        }
    }
}

