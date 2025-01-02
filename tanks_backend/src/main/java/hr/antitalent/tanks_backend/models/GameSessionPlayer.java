package hr.antitalent.tanks_backend.models;

import jakarta.persistence.*;
import lombok.*;

import java.time.LocalDateTime;

@Entity
@Table(name = "game_session_player", schema = "game")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class GameSessionPlayer {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "player_id", referencedColumnName = "id")
    private User player;

    @ManyToOne
    @JoinColumn(name = "game_session_id", referencedColumnName = "id")
    private GameSession gameSession;

    @ManyToOne
    @JoinColumn(name = "team_id", referencedColumnName = "id")
    private Team team;

    private Integer kills = 0;
    private Integer deaths = 0;
    private LocalDateTime joinedAt;
    private LocalDateTime leftAt;
}