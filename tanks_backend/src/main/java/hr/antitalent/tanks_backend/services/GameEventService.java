package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.domain.GameEvent;
import hr.antitalent.tanks_backend.repositories.GameEventRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class GameEventService {
    private final GameEventRepository gameEventRepository;

    public GameEvent saveGameEvent(GameEvent gameEvent, Long sessionId) {
        gameEvent.setGameSessionId(sessionId);
        return gameEventRepository.save(gameEvent);
    }

    public List<GameEvent> findEventsBySessionId(Long sessionId) {
        return gameEventRepository.findByGameSessionIdOrderByEventTimeDesc(sessionId);
    }
}
