package hr.antitalent.tanks_backend.model;

import hr.antitalent.tanks_backend.enums.GameEventType;
import jakarta.persistence.*;
import lombok.*;

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
    private Integer id;

    @ManyToOne
    @JoinColumn(name = "game_session_id", referencedColumnName = "id")
    private GameSession gameSession;

    @Enumerated(EnumType.STRING)
    private GameEventType eventType;

    private LocalDateTime eventTime;

    @Column(columnDefinition = "jsonb")
    private String summary;
}