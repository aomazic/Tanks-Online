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

    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody User user) {
        return ResponseEntity.ok(authenticationService.register(user));
    }

    @PostMapping("/login")
    public ResponseEntity<String> login(@RequestParam String username, @RequestParam String password) {
        return ResponseEntity.ok(authenticationService.authenticate(username, password));
    }

    @PostMapping("/guest-register")
    public ResponseEntity<String> guestLogin() {
        return ResponseEntity.ok(authenticationService.guestRegister());
    }
}
