package hr.antitalent.tanks_backend.dto;

public record GameSessionUpdateDTO(
        Long winningTeamId,
        String summary
) {
}
