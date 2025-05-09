package hr.antitalent.tanks_backend.domain;

import com.fasterxml.jackson.annotation.JsonBackReference;
import jakarta.persistence.*;
import lombok.*;
import org.hibernate.annotations.CreationTimestamp;

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
    @JsonBackReference
    private User player;

    @ManyToOne
    @JoinColumn(name = "game_session_id", referencedColumnName = "id")
    @JsonBackReference
    private GameSession gameSession;

    private Integer kills = 0;
    private Integer deaths = 0;

    @CreationTimestamp
    @Column(updatable = false)
    private LocalDateTime joinedAt;

    @Column(updatable = false)
    private LocalDateTime leftAt;
}