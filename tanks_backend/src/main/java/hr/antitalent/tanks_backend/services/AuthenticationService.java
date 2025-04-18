package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.dto.auth.AuthResponse;
import hr.antitalent.tanks_backend.enums.UserRole;
import hr.antitalent.tanks_backend.enums.UserStatus;
import hr.antitalent.tanks_backend.domain.User;
import hr.antitalent.tanks_backend.repositories.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.UUID;

@Service
@RequiredArgsConstructor
public class AuthenticationService {
    private final UserRepository userRepository;
    private final JwtService jwtService;
    private final PasswordEncoder passwordEncoder;
    private final AuthenticationManager authenticationManager;

    @Transactional
    public AuthResponse register(String username, String password, String email) {
        try {
            User user = User.builder()
                    .username(username)
                    .passwordHash(passwordEncoder.encode(password))
                    .email(email)
                    .role(UserRole.PLAYER)
                    .status(UserStatus.ONLINE)
                    .build();
            User savedUser = userRepository.save(user);
            String token = jwtService.generateToken(savedUser);
            return new AuthResponse(savedUser.getId(), savedUser.getUsername(), token);
        } catch (DataIntegrityViolationException e) {
            throw new IllegalArgumentException("Username or email already exists");
        }
    }

    public AuthResponse authenticate(String username, String password) {
        authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(username, password)
        );
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));
        String token = jwtService.generateToken(user);
        return new AuthResponse(user.getId(), user.getUsername(), token);
    }

    public AuthResponse guestRegister() {
        String guestUsername = String.format("guest_%s", UUID.randomUUID().toString().substring(0, 8));
        User guestUser = User.builder()
                .username(guestUsername)
                .email(String.format("%s@guest.com", guestUsername))
                .passwordHash(passwordEncoder.encode(UUID.randomUUID().toString()))
                .role(UserRole.GUEST)
                .status(UserStatus.ONLINE)
                .build();

        User savedUser = userRepository.save(guestUser);
        String token = jwtService.generateToken(savedUser);
        return new AuthResponse(savedUser.getId(), savedUser.getUsername(), token);
    }

    public boolean checkUsername(String username) {
        return !userRepository.existsByUsername(username);
    }
}