package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.auth.AuthRequest;
import hr.antitalent.tanks_backend.dto.auth.AuthResponse;
import hr.antitalent.tanks_backend.dto.auth.RegistrationRequest;
import hr.antitalent.tanks_backend.services.AuthenticationService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
@Slf4j
public class AuthController {
    private final AuthenticationService authenticationService;

    /**
     * Registers a new user.
     *
     * @param request Registration data containing username, password, and email
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @PostMapping("/register")
    public ResponseEntity<AuthResponse> register(@Valid @RequestBody RegistrationRequest request) {
        log.info("Processing registration request for username: {}", request.getUsername());
        try {
            AuthResponse response = authenticationService.register(
                    request.getUsername(),
                    request.getPassword(),
                    request.getEmail());

            log.info("User successfully registered: {} (ID: {})",
                    response.getUsername(), response.getId());
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        } catch (Exception e) {
            log.error("Registration failed for username '{}': {}",
                    request.getUsername(), e.getMessage());
            return ResponseEntity.badRequest().build();
        }
    }

    /**
     * Authenticates a user and logs them in.
     *
     * @param request Authentication data containing username and password
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @PostMapping("/login")
    public ResponseEntity<AuthResponse> login(@Valid @RequestBody AuthRequest request) {
        log.info("Login attempt for username: {}", request.getUsername());

        try {
            AuthResponse response = authenticationService.authenticate(
                    request.getUsername(),
                    request.getPassword());

            log.info("Successful login for user: {} (ID: {})",
                    response.getUsername(), response.getId());
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            log.warn("Failed login attempt for username '{}': {}",
                    request.getUsername(), e.getMessage());
            return ResponseEntity.badRequest().build();
        }
    }

    /**
     * Registers a guest user.
     *
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @PostMapping("/guest")
    public ResponseEntity<AuthResponse> registerGuest() {
        log.info("Processing guest registration request");

        try {
            AuthResponse response = authenticationService.guestRegister();
            log.info("Guest user successfully registered: {} (ID: {})",
                    response.getUsername(), response.getId());
            return ResponseEntity.status(HttpStatus.CREATED).body(response);
        } catch (Exception e) {
            log.error("Guest registration failed: {}", e.getMessage());
            return ResponseEntity.badRequest().build();
        }
    }

    /**
     * Checks if username is available.
     *
     * @param username The username to check
     * @return ResponseEntity with boolean value indicating if username is available
     */
    @GetMapping("/{username}")
    public ResponseEntity<Boolean> isUsernameAvailable(@PathVariable String username) {
        log.debug("Checking username availability: {}", username);

        boolean isAvailable = authenticationService.checkUsername(username);
        log.debug("Username '{}' is {}", username, isAvailable ? "available" : "already taken");

        return ResponseEntity.ok(isAvailable);
    }
}