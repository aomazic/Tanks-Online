package hr.antitalent.tanks_backend.model;

import jakarta.persistence.*;
import lombok.*;

import java.time.LocalDateTime;

@Entity
@Table(name = "leaderboard", schema = "game")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Leaderboard {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @OneToOne
    @JoinColumn(name = "user_id", referencedColumnName = "id")
    private User user;

    private Integer wins = 0;
    private Integer losses = 0;
    private Integer kills = 0;
    private Integer deaths = 0;
    private Integer matchesPlayed = 0;

    private LocalDateTime createdAt;
    private LocalDateTime updatedAt;
}