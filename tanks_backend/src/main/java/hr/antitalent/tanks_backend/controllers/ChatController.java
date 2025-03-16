package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.services.ChatMessageService;
import hr.antitalent.tanks_backend.websocket.chat.ChatMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;

import java.util.List;

@Controller
@RequiredArgsConstructor
@Slf4j
public class ChatController {

    private final SimpMessagingTemplate messagingTemplate;
    private final ChatMessageService chatMessageService;

    private static final String CHAT_TOPIC_TEMPLATE = "/topic/chat/%s";
    private static final String USERNAME_ATTR = "username";
    private static final String SESSION_ID_ATTR = "sessionId";

    @MessageMapping("/chat.sendMessage/{sessionId}")
    public void sendMessage(
            @DestinationVariable String sessionId,
            @Payload ChatMessage chatMessage,
            SimpMessageHeaderAccessor headerAccessor) {

        String sender = "unknown";
        if (headerAccessor.getSessionAttributes() != null) {
            sender = (String) headerAccessor.getSessionAttributes().get(USERNAME_ATTR);
            chatMessage.setSender(sender);
        }

        log.debug("Message received in session {}: sender='{}', content='{}'",
                sessionId, sender, chatMessage.getContent());

        // Store message in memory
        try {
            chatMessageService.addMessage(sessionId, chatMessage);
            log.trace("Message stored in session {}: sender='{}', timestamp={}",
                    sessionId, sender, chatMessage.getTimestamp());
        } catch (Exception e) {
            log.error("Failed to store message in session {}: {}", sessionId, e.getMessage(), e);
        }

        // Broadcast to all subscribers
        try {
            messagingTemplate.convertAndSend(String.format(CHAT_TOPIC_TEMPLATE, sessionId), chatMessage);
            log.debug("Message broadcasted to session {}: sender='{}'", sessionId, sender);
        } catch (Exception e) {
            log.error("Failed to broadcast message to session {}: {}", sessionId, e.getMessage(), e);
        }
    }

    @MessageMapping("/chat.join/{sessionId}")
    public void addUser(
            @DestinationVariable String sessionId,
            @Payload ChatMessage chatMessage,
            SimpMessageHeaderAccessor headerAccessor) {

        String username = chatMessage.getSender();

        if (headerAccessor.getSessionAttributes() != null) {
            headerAccessor.getSessionAttributes().put(USERNAME_ATTR, username);
            headerAccessor.getSessionAttributes().put(SESSION_ID_ATTR, sessionId);
            log.info("User '{}' joined chat session: {} (WebSocket session: {})",
                    username, sessionId, headerAccessor.getSessionId());
        } else {
            log.warn("Session attributes null when user '{}' joined session {}", username, sessionId);
        }

        // Store join message
        try {
            chatMessageService.addMessage(sessionId, chatMessage);
            log.debug("Join message stored for user '{}' in session {}", username, sessionId);
        } catch (Exception e) {
            log.error("Failed to store join message for user '{}' in session {}: {}",
                    username, sessionId, e.getMessage(), e);
        }

        // Send join notification
        try {
            messagingTemplate.convertAndSend(String.format(CHAT_TOPIC_TEMPLATE, sessionId), chatMessage);
            log.debug("Join notification sent for user '{}' in session {}", username, sessionId);
        } catch (Exception e) {
            log.error("Failed to send join notification for user '{}' in session {}: {}",
                    username, sessionId, e.getMessage(), e);
        }
    }

    @GetMapping("/api/chat/{sessionId}")
    public ResponseEntity<List<ChatMessage>> getChatHistory(@PathVariable String sessionId) {
        log.info("REST request: Retrieving chat history for session: {}", sessionId);
        try {
            List<ChatMessage> messages = chatMessageService.getSessionMessages(sessionId);
            log.info("Retrieved {} chat messages for session {}", messages.size(), sessionId);
            return ResponseEntity.ok(messages);
        } catch (Exception e) {
            log.error("Failed to retrieve chat history for session {}: {}", sessionId, e.getMessage(), e);
            return ResponseEntity.badRequest().build();
        }
    }
}