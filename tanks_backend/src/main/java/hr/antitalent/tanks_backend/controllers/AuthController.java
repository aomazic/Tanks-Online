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
     * @param user The user details for registration.
     * @return ResponseEntity with registration status message.
     */
    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody User user) {
        String registrationResult = authenticationService.register(user);
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
}
