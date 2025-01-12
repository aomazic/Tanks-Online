package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.models.User;
import hr.antitalent.tanks_backend.services.AuthenticationService;
import lombok.RequiredArgsConstructor;
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
     * @param username The username of the user.
     * @param password The password of the user.
     * @param email The email of the user.                     
     * @return ResponseEntity with registration status message.
     */
    @PostMapping("/register")
    public ResponseEntity<String> register(
            @RequestParam String username,
            @RequestParam String password,
            @RequestParam String email
    ) {
        String registrationResult = authenticationService.register(username, password, email);
        return ResponseEntity.ok(registrationResult);
    }

    /**
     * Authenticates a user and logs them in.
     *
     * @param username The username of the user.
     * @param password The password of the user.
     * @return ResponseEntity with the authentication token or error message.
     */
    @PostMapping("/login")
    public ResponseEntity<String> login(
            @RequestParam String username,
            @RequestParam String password) {
        String authToken = authenticationService.authenticate(username, password);
        return ResponseEntity.ok(authToken);
    }

    /**
     * Registers a guest user.
     *
     * @return ResponseEntity with guest registration status message.
     */
    @PostMapping("/guest-register")
    public ResponseEntity<String> guestRegister() {
        String guestToken = authenticationService.guestRegister();
        return ResponseEntity.ok(guestToken);
    }

    /**
     * Checks if username is already taken.
     *
     * @param username The username to check.
     * @return ResponseEntity with username availability status message.
     */
    @GetMapping("/check-username/{username}")
    public ResponseEntity<Boolean> checkUsername(@PathVariable String username) {
        return ResponseEntity.ok(authenticationService.checkUsername(username));
    }
}
