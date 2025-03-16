package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.auth.AuthRequest;
import hr.antitalent.tanks_backend.dto.auth.AuthResponse;
import hr.antitalent.tanks_backend.dto.auth.RegistrationRequest;
import hr.antitalent.tanks_backend.services.AuthenticationService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/auth")
@RequiredArgsConstructor
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
        AuthResponse response = authenticationService.register(
                request.getUsername(),
                request.getPassword(),
                request.getEmail());
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }

    /**
     * Authenticates a user and logs them in.
     *
     * @param request Authentication data containing username and password
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @PostMapping("/login")
    public ResponseEntity<AuthResponse> login(@Valid @RequestBody AuthRequest request) {
        AuthResponse response = authenticationService.authenticate(
                request.getUsername(),
                request.getPassword());
        return ResponseEntity.ok(response);
    }

    /**
     * Registers a guest user.
     *
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @PostMapping("/guest")
    public ResponseEntity<AuthResponse> registerGuest() {
        AuthResponse response = authenticationService.guestRegister();
        return ResponseEntity.status(HttpStatus.CREATED).body(response);
    }

    /**
     * Checks if username is available.
     *
     * @param username The username to check
     * @return ResponseEntity with boolean value indicating if username is available
     */
    @GetMapping("/{username}")
    public ResponseEntity<Boolean> isUsernameAvailable(@PathVariable String username) {
        return ResponseEntity.ok(authenticationService.checkUsername(username));
    }
}