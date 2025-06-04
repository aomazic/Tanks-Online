package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.dto.auth.AuthRequest;
import hr.antitalent.tanks_backend.dto.auth.AuthResponse;
import hr.antitalent.tanks_backend.dto.auth.RegistrationRequest;
import hr.antitalent.tanks_backend.services.AuthenticationService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
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
@Tag(name = "Authentication", description = "Endpoints for user registration, login, and authentication")
public class AuthController {
    private final AuthenticationService authenticationService;

    /**
     * Registers a new user.
     *
     * @param request Registration data containing username, password, and email
     * @return ResponseEntity with authentication response containing id, username and token
     */
    @Operation(summary = "Register a new user", description = "Creates a new user account with the provided credentials")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "201", description = "User successfully registered",
                content = @Content(schema = @Schema(implementation = AuthResponse.class))),
        @ApiResponse(responseCode = "400", description = "Invalid registration data or username already exists")
    })
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
    @Operation(summary = "Login", description = "Authenticates a user and returns a JWT token")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Login successful",
                content = @Content(schema = @Schema(implementation = AuthResponse.class))),
        @ApiResponse(responseCode = "400", description = "Invalid credentials")
    })
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
    @Operation(summary = "Register a guest user", description = "Creates a temporary guest account for anonymous users")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "201", description = "Guest user successfully registered",
                    content = @Content(schema = @Schema(implementation = AuthResponse.class))),
            @ApiResponse(responseCode = "400", description = "Failed to register guest user")
    })
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
    @Operation(summary = "Check username availability", description = "Verifies if a username is available for registration")
    @ApiResponses(value = {
            @ApiResponse(responseCode = "200", description = "Username availability checked",
                    content = @Content(schema = @Schema(implementation = Boolean.class))),
            @ApiResponse(responseCode = "400", description = "Invalid username format")
    })
    @GetMapping("/{username}")
    public ResponseEntity<Boolean> isUsernameAvailable(
            @Parameter(description = "Username to check for availability", required = true)
            @PathVariable String username) {
        log.debug("Checking username availability: {}", username);

        boolean isAvailable = authenticationService.checkUsername(username);
        log.debug("Username '{}' is {}", username, isAvailable ? "available" : "already taken");

        return ResponseEntity.ok(isAvailable);
    }
}