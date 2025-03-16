package hr.antitalent.tanks_backend.domain;

import com.fasterxml.jackson.annotation.JsonBackReference;
import hr.antitalent.tanks_backend.enums.GameEventType;
import io.hypersistence.utils.hibernate.type.json.JsonBinaryType;
import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.CreationTimestamp;
import org.hibernate.annotations.Type;

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

    @ManyToOne
    @JoinColumn(name = "game_session_id", referencedColumnName = "id")
    @JsonBackReference
    private GameSession gameSession;

    @Enumerated(EnumType.STRING)
    private GameEventType eventType;

    @CreationTimestamp
    @Column(updatable = false)
    private LocalDateTime eventTime;

    @Type(JsonBinaryType.class)
    @Column(columnDefinition = "jsonb")
    private String summary;
}