package hr.antitalent.tanks_backend.enums;

import org.springframework.security.core.GrantedAuthority;

public enum UserRole implements GrantedAuthority {
    GUEST, PLAYER, ADMIN;

    @Override
    public String getAuthority() {
        return name();
    }
}
