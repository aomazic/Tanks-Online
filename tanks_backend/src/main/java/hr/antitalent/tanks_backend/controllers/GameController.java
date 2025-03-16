package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.GameEvent;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;

@Controller
@RequiredArgsConstructor
@Slf4j
public class GameController {
    private final SimpMessagingTemplate messagingTemplate;

    private static final String GAME_JOIN_TOPIC_TEMPLATE = "/topic/game/%s";
    private static final String GAME_EVENT_TOPIC_TEMPLATE = "/topic/game/event/%s";

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
                String.format(GAME_JOIN_TOPIC_TEMPLATE, sessionId),
                gameEvent
        );
    }

    @MessageMapping("/game.event/{sessionId}")
    public void handleGameEvent(
            @DestinationVariable String sessionId,
            @Payload GameEvent gameEvent) {

        // Process game event
        log.info("Game event received for session {}: {}", sessionId, gameEvent.getEventType());

        // Broadcast event to all players in the session
        messagingTemplate.convertAndSend(
                String.format(GAME_EVENT_TOPIC_TEMPLATE, sessionId),
                gameEvent
        );
    }
}