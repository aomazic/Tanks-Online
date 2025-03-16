package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.domain.GameSession;
import hr.antitalent.tanks_backend.domain.GameSessionPlayer;
import hr.antitalent.tanks_backend.dto.game.GameSessionPlayerUpdateDTO;
import hr.antitalent.tanks_backend.repositories.GameSessionPlayerRepository;
import hr.antitalent.tanks_backend.repositories.GameSessionRepository;
import hr.antitalent.tanks_backend.repositories.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.Instant;
import java.time.LocalDateTime;
import java.time.ZoneId;
import java.util.List;

import static hr.antitalent.tanks_backend.exceptions.GlobalExceptionHandler.USER_NOT_FOUND;

@Service
@RequiredArgsConstructor
public class GameSessionPlayerService {
    private final GameSessionPlayerRepository gameSessionPlayerRepository;
    private final GameSessionRepository gameSessionRepository;
    private final UserRepository userRepository;


    /**
     * Add a player to a game session.
     */
    @Transactional
    public GameSessionPlayer addPlayerToSession(Long sessionId, Long userId) {
        GameSession gameSession = gameSessionRepository.findById(sessionId)
                .orElseThrow(() -> new IllegalArgumentException("Game session not found"));

        GameSessionPlayer player = GameSessionPlayer.builder()
                .gameSession(gameSession)
                .player(userRepository.findById(userId)
                        .orElseThrow(() -> new IllegalArgumentException(USER_NOT_FOUND)))
                .kills(0)
                .deaths(0)
                .build();

        return gameSessionPlayerRepository.save(player);
    }

    /**
     * Get a player by their ID.
     */
    public GameSessionPlayer getPlayerById(Long playerId) {
        return gameSessionPlayerRepository.findById(playerId)
                .orElseThrow(() -> new IllegalArgumentException(USER_NOT_FOUND));
    }

    /**
     * Update player details.
     */
    @Transactional
    public GameSessionPlayer updatePlayerGameInfo(Long playerId, GameSessionPlayerUpdateDTO dto) {
        GameSessionPlayer player = gameSessionPlayerRepository.findById(playerId)
                .orElseThrow(() -> new IllegalArgumentException(USER_NOT_FOUND));

        player.setKills(dto.kills());
        player.setDeaths(dto.deaths());

        return gameSessionPlayerRepository.save(player);
    }

    /**
     * List all players in a specific game session.
     */
    public List<GameSessionPlayer> getPlayersBySessionId(Long sessionId) {
        return gameSessionPlayerRepository.findByGameSessionId(sessionId);
    }

    /**
     * Remove all players from a game session.
     */
    @Transactional
    public String removeAllPlayersFromSession(Long sessionId) {
        gameSessionPlayerRepository.deleteByGameSessionId(sessionId);
        return "All players removed";
    }

    /**
     * Get the top players by score in a session.
     */
    public List<GameSessionPlayer> getTopPlayersInSession(Long sessionId, int limit) {
        return gameSessionPlayerRepository.findTopPlayersBySessionId(sessionId, limit);
    }

    /**
     * Remove a player from a game session.
     */
    @Transactional
    public String leaveGameSession(Long playerId) {
        GameSessionPlayer player = gameSessionPlayerRepository.findById(playerId)
                .orElseThrow(() -> new IllegalArgumentException(USER_NOT_FOUND));

        player.setLeftAt(LocalDateTime.ofInstant(Instant.now(), ZoneId.systemDefault()));

        gameSessionPlayerRepository.save(player);
        return "Player left the game session";
    }
    

}
