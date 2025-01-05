package hr.antitalent.tanks_backend.dto;

public record GameSessionUpdateDTO(
        String winningTeam,
        String summary
) {
}
