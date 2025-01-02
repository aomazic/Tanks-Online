package hr.antitalent.tanks_backend.model;

import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import jakarta.persistence.*;
import lombok.*;

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
    private Integer id;

    @Column(unique = true, nullable = false)
    private String name;

    private String password;

    @Enumerated(EnumType.STRING)
    private GameSessionStatus status = GameSessionStatus.WAITING;

    private LocalDateTime startTime;
    private LocalDateTime endTime;

    @ManyToOne
    @JoinColumn(name = "winner_team_id", referencedColumnName = "id")
    private Team winnerTeam;

    @Column(columnDefinition = "jsonb")
    private String gameSettings;

    @Column(columnDefinition = "jsonb")
    private String summary;

    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;

    @OneToMany(mappedBy = "gameSession", cascade = CascadeType.ALL, fetch = FetchType.EAGER)
    private List<GameSessionPlayer> players;

    @OneToMany(mappedBy = "gameSession", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    private List<GameEvent> events;
}