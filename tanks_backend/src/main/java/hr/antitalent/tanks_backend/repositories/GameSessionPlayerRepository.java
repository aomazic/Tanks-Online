package hr.antitalent.tanks_backend.repositories;

import hr.antitalent.tanks_backend.models.GameSessionPlayer;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface GameSessionPlayerRepository extends JpaRepository<GameSessionPlayer, Long> {
    List<GameSessionPlayer> findByGameSessionId(Long gameSessionId);

    void deleteByGameSessionId(Long gameSessionId);

    @Query("SELECT p FROM GameSessionPlayer p WHERE p.gameSession.id = :sessionId ORDER BY p.kills DESC LIMIT :limit")
    List<GameSessionPlayer> findTopPlayersBySessionId(Long sessionId, int limit);
}
