package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.dto.GameSessionCreateDTO;
import hr.antitalent.tanks_backend.dto.GameSessionUpdateDTO;
import hr.antitalent.tanks_backend.enums.GameSessionStatus;
import hr.antitalent.tanks_backend.models.GameSession;
import hr.antitalent.tanks_backend.models.Team;
import hr.antitalent.tanks_backend.repositories.GameSessionRepository;
import hr.antitalent.tanks_backend.repositories.TeamRepository;
import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.cache.CacheManager;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;

import static hr.antitalent.tanks_backend.configurations.CacheConfig.FIFTEEN_MINUTE_CACHE;

@Service
@RequiredArgsConstructor
public class GameSessionService {
    public static final String GAME_SESSION_NOT_FOUND = "Game session not found";

    private static final Logger logger = LoggerFactory.getLogger(GameSessionService.class);
    private final GameSessionRepository gameSessionRepository;
    private final TeamRepository teamRepository;
    private final CacheManager fifteenMinutesCacheManager;

    @Transactional
    public GameSession createGameSession(GameSessionCreateDTO dto) {
        if (dto.name() == null || dto.name().isBlank()) {
            throw new IllegalArgumentException("Game session name cannot be empty");
        }

        GameSession gameSession = GameSession.builder()
                .name(dto.name())
                .password(dto.password())
                .gameSettings(dto.gameSettings())
                .status(GameSessionStatus.WAITING)
                .build();

        logger.info("Creating game session with name: {}", dto.name());
        return gameSessionRepository.save(gameSession);
    }

    @Transactional
    public GameSession startGameSession(Long gameSessionId) {
        GameSession gameSession = gameSessionRepository.findById(gameSessionId)
                .orElseThrow(() -> new IllegalArgumentException(GAME_SESSION_NOT_FOUND));
        gameSession.setStatus(GameSessionStatus.IN_PROGRESS);
        return gameSessionRepository.save(gameSession);
    }

    @Transactional
    public GameSession endGameSession(Long gameSessionId, GameSessionUpdateDTO dto) {
        GameSession gameSession = gameSessionRepository.findById(gameSessionId)
                .orElseThrow(() -> new IllegalArgumentException(GAME_SESSION_NOT_FOUND));

        gameSession.setStatus(GameSessionStatus.FINISHED);
        gameSession.setEndTime(LocalDateTime.now());

        if (dto.winningTeamId() != null) {
            Team winningTeam = teamRepository.findById(dto.winningTeamId())
                    .orElseThrow(() -> new IllegalArgumentException("Winner team not found"));
            gameSession.setWinningTeam(winningTeam);
        }

        if (dto.summary() != null && !dto.summary().isBlank()) {
            gameSession.setSummary(dto.summary());
        }

        logger.info("Ending game session with ID: {}. Winning Team ID: {}, Summary: {}",
                gameSessionId, dto.winningTeamId(), dto.summary());

        return gameSessionRepository.save(gameSession);
    }

    @Transactional
    public GameSession updateGameSessionSettings(Long gameSessionId, String newGameSettings) {
        GameSession gameSession = gameSessionRepository.findById(gameSessionId)
                .orElseThrow(() -> new IllegalArgumentException(GAME_SESSION_NOT_FOUND));
        gameSession.setGameSettings(newGameSettings);
        return gameSessionRepository.save(gameSession);
    }

    @Cacheable(cacheNames = FIFTEEN_MINUTE_CACHE, cacheManager = "fifteenMinutesCacheManager", key = "#gameSessionName")
    public GameSession findGameSessionByName(String gameSessionName) {
        return gameSessionRepository.findByName(gameSessionName)
                .orElseThrow(() -> new IllegalArgumentException(GAME_SESSION_NOT_FOUND));
    }

    public Page<GameSession> findGameSessionsByStatus(GameSessionStatus status, Pageable pageable) {
        return gameSessionRepository.findByStatus(status, pageable);
    }

    public Page<GameSession> findAllGameSessions(Pageable pageable) {
        if (pageable == null) {
            pageable = Pageable.ofSize(10);
        }
        return gameSessionRepository.findAll(pageable);
    }

    @Transactional
    public void deleteGameSession(Long gameSessionId) {
        logger.info("Deleting game session with id: {}", gameSessionId);
        gameSessionRepository.deleteById(gameSessionId);
    }
}