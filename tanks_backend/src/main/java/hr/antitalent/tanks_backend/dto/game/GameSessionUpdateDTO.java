package hr.antitalent.tanks_backend.dto.game;

public record GameSessionUpdateDTO(
        String winningTeam,
        String summary
) {
}
