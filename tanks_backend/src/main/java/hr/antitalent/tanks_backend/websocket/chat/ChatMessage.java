package hr.antitalent.tanks_backend.websocket.chat;

import lombok.Data;

import java.sql.Timestamp;

@Data
public class ChatMessage {
    private String content;
    private String sender;
    private Timestamp timestamp;
    private String sessionId;
}
