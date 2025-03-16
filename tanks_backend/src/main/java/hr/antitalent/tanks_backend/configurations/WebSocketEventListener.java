package hr.antitalent.tanks_backend.configurations;

import hr.antitalent.tanks_backend.websocket.chat.ChatMessage;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.context.event.EventListener;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.messaging.SessionDisconnectEvent;

@Component
@Slf4j
@RequiredArgsConstructor
public class WebSocketEventListener {

    private final SimpMessageSendingOperations messagingTemplate;

    @EventListener
    public void handleWebSocketDisconnectListener(SessionDisconnectEvent event) {
        StompHeaderAccessor headerAccessor = StompHeaderAccessor.wrap(event.getMessage());
        if (headerAccessor.getSessionAttributes() != null) {
            String username = (String) headerAccessor.getSessionAttributes().get("username");
            String sessionId = (String) headerAccessor.getSessionAttributes().get("sessionId");

            if (username != null && sessionId != null) {
                log.info("User Disconnected: {} from session: {}", username, sessionId);
                
                ChatMessage chatMessage = new ChatMessage();
                chatMessage.setContent(String.format("%s has left the chat", username));
                chatMessage.setSender("System");
                
                messagingTemplate.convertAndSend(String.format("/topic/chat/%s", sessionId), chatMessage);
            }
        }
    }
    
}
