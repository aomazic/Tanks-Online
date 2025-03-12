package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.models.ChatMessage;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

@Service
@RequiredArgsConstructor
public class ChatMessageService {
    private static final int MAX_MESSAGES_PER_SESSION = 250;
    
    private final Map<String, List<ChatMessage>> sessionMessages = new ConcurrentHashMap<>();

    /**
     * Add a message to the session and return all messages
     */
    public void addMessage(String sessionId, ChatMessage message) {
        sessionMessages.computeIfAbsent(sessionId, k -> new ArrayList<>());

        List<ChatMessage> messages = sessionMessages.get(sessionId);
        synchronized (messages) {
            messages.add(message);
            if (messages.size() > MAX_MESSAGES_PER_SESSION) {
                messages.removeFirst();
            }
        }
    }

    /**
     * Get all messages for a session
     */
    public List<ChatMessage> getSessionMessages(String sessionId) {
        return sessionMessages.getOrDefault(sessionId, new ArrayList<>());
    }

    /**
     * Clear messages for a session (e.g., when the game ends)
     */
    public void clearSessionMessages(String sessionId) {
        sessionMessages.remove(sessionId);
    }
}
