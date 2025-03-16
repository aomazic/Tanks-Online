package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.domain.GameEvent;
import hr.antitalent.tanks_backend.domain.GameSession;
import hr.antitalent.tanks_backend.repositories.GameEventRepository;
import hr.antitalent.tanks_backend.repositories.GameSessionRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
@RequiredArgsConstructor
public class GameEventService {
    private final GameEventRepository gameEventRepository;
    private final GameSessionRepository gameSessionRepository;

    public GameEvent saveGameEvent(GameEvent gameEvent, String sessionId) {
        GameSession gameSession = gameSessionRepository.findByName(sessionId)
                .orElseThrow(() -> new IllegalArgumentException(String.format("Game session not found: %s", sessionId)));

        gameEvent.setGameSession(gameSession);
        return gameEventRepository.save(gameEvent);
    }

    public List<GameEvent> findEventsBySessionId(String sessionId) {
        GameSession gameSession = gameSessionRepository.findByName(sessionId)
                .orElseThrow(() -> new IllegalArgumentException(String.format("Game session not found: %s", sessionId)));

        return gameEventRepository.findByGameSessionOrderByEventTimestampDesc(gameSession);
    }
}
