package hr.antitalent.tanks_backend.repositories;

import hr.antitalent.tanks_backend.domain.GameEvent;
import hr.antitalent.tanks_backend.domain.GameSession;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface GameEventRepository extends JpaRepository<GameEvent, Long> {

    List<GameEvent> findByGameSessionOrderByEventTimestampDesc(GameSession gameSession);
}
