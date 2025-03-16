package hr.antitalent.tanks_backend.domain;

import com.fasterxml.jackson.annotation.JsonManagedReference;
import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import io.hypersistence.utils.hibernate.type.json.JsonBinaryType;
import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.CreationTimestamp;
import org.hibernate.annotations.Type;
import org.hibernate.annotations.UpdateTimestamp;

import java.time.LocalDateTime;
import java.util.List;

@Entity
@Table(name = "game_session", schema = "game")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class GameSession {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @Column(nullable = false)
    private Long hostId;

    @Column(unique = true, nullable = false)
    private String name;

    private String password;
    
    private int maxPlayers;

    @Enumerated(EnumType.STRING)
    private GameSessionStatus status = GameSessionStatus.WAITING;

    private LocalDateTime startTime;
    
    private LocalDateTime endTime;

    @Type(JsonBinaryType.class)
    @Column(columnDefinition = "jsonb")
    private String gameSettings;

    @Type(JsonBinaryType.class)
    @Column(columnDefinition = "jsonb")
    private String summary;

    @CreationTimestamp
    @Column(updatable = false)
    private LocalDateTime createdAt;

    @UpdateTimestamp
    private LocalDateTime updatedAt;

    @OneToMany(mappedBy = "gameSession", cascade = CascadeType.ALL, fetch = FetchType.EAGER)
    @JsonManagedReference
    private List<GameSessionPlayer> players;

    @OneToMany(mappedBy = "gameSession", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JsonManagedReference
    private List<GameEvent> events;
}