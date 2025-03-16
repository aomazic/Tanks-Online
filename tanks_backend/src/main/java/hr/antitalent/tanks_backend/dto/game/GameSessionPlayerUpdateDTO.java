package hr.antitalent.tanks_backend.dto.game;

public record GameSessionPlayerUpdateDTO(
        String team,
        Integer kills,
        Integer deaths
) {
}
