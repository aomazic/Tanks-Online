package hr.antitalent.tanks_backend.dto.game;

public record GameSessionCreateDTO(
        String name,
        Long hostId,
        String password
) {
}
