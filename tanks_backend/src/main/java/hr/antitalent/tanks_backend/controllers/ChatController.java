package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.models.ChatMessage;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.Payload;
import org.springframework.messaging.simp.SimpMessageHeaderAccessor;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.stereotype.Controller;

@Controller
@RequiredArgsConstructor
public class ChatController {

    private final SimpMessagingTemplate messagingTemplate;

    @MessageMapping("/chat.sendMessage/{sessionId}")
    public void sendMessage(
            @DestinationVariable String sessionId,
            @Payload ChatMessage chatMessage,
            SimpMessageHeaderAccessor headerAccessor) {

        if (headerAccessor.getSessionAttributes() != null) {
            String username = (String) headerAccessor.getSessionAttributes().get("username");
            if (username != null) {
                chatMessage.setSender(username);
            }
        }

        messagingTemplate.convertAndSend(String.format("/topic/chat/%s", sessionId), chatMessage);
    }

    @MessageMapping("/chat.join/{sessionId}")
    public void addUser(
            @DestinationVariable String sessionId,
            @Payload ChatMessage chatMessage,
            SimpMessageHeaderAccessor headerAccessor) {
        
        if (headerAccessor.getSessionAttributes() != null) {
            headerAccessor.getSessionAttributes().put("username", chatMessage.getSender());
            headerAccessor.getSessionAttributes().put("sessionId", sessionId);
        }

        messagingTemplate.convertAndSend(String.format("/topic/chat/%s", sessionId), chatMessage);
    }
}