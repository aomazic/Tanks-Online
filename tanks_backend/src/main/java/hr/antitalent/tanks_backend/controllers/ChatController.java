package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.websocket.chat.ChatMessage;
import hr.antitalent.tanks_backend.services.ChatMessageService;
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

        if (headerAccessor.getSessionAttributes() != null) {
            chatMessage.setSender((String) headerAccessor.getSessionAttributes().get(USERNAME_ATTR));
        }
        
        // Store message in memory
        chatMessageService.addMessage(sessionId, chatMessage);

        // Broadcast to all subscribers
        messagingTemplate.convertAndSend(String.format(CHAT_TOPIC_TEMPLATE, sessionId), chatMessage);
    }

    @MessageMapping("/chat.join/{sessionId}")
    public void addUser(
            @DestinationVariable String sessionId,
            @Payload ChatMessage chatMessage,
            SimpMessageHeaderAccessor headerAccessor) {

        if (headerAccessor.getSessionAttributes() != null) {
            headerAccessor.getSessionAttributes().put(USERNAME_ATTR, chatMessage.getSender());
            headerAccessor.getSessionAttributes().put(SESSION_ID_ATTR, sessionId);
        }

        log.info("User joined chat session: {}", sessionId);
        
        // Store join message
        chatMessageService.addMessage(sessionId, chatMessage);

        // Send join notification
        messagingTemplate.convertAndSend(String.format(CHAT_TOPIC_TEMPLATE, sessionId), chatMessage);
    }

    @GetMapping("/api/chat/{sessionId}")
    public ResponseEntity<List<ChatMessage>> getChatHistory(@PathVariable String sessionId) {
        return ResponseEntity.ok(chatMessageService.getSessionMessages(sessionId));
    }
}