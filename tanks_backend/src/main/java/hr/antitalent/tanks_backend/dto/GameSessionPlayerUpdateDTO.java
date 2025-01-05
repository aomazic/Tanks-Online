package hr.antitalent.tanks_backend.dto;

public record GameSessionPlayerUpdateDTO(
        String team,
        Integer kills,
        Integer deaths
) {
}
