package hr.antitalent.tanks_backend.domain;

import hr.antitalent.tanks_backend.enums.GameEventType;
import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.CreationTimestamp;

import java.time.LocalDateTime;

@Entity
@Table(name = "game_events", schema = "game")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class GameEvent {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private Long gameSessionId;

    @Enumerated(EnumType.STRING)
    private GameEventType eventType;

    @CreationTimestamp
    @Column(updatable = false)
    private LocalDateTime eventTime;
    
    @Column
    private String event_summary;
}