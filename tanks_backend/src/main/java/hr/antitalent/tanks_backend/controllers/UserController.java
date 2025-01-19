package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.enums.UserStatus;
import hr.antitalent.tanks_backend.models.User;
import hr.antitalent.tanks_backend.services.UserService;
import lombok.RequiredArgsConstructor;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/user")
@RequiredArgsConstructor
public class UserController {
    private static final Logger logger = LoggerFactory.getLogger(UserController.class);

    private final UserService userService;

    /**
     * Update status of a user.
     *
     * @param userName  Username.
     * @param newStatus New status.
     * @return ResponseEntity with updated User.
     */
    @PostMapping("/{userName}/status")
    public ResponseEntity<User> updateUserStatus(
            @PathVariable String userName,
            @RequestParam UserStatus newStatus) {
        try {
            User updatedUser = userService.updateUserStatus(userName, newStatus);
            return ResponseEntity.ok(updatedUser);
        } catch (IllegalArgumentException e) {
            logger.error("Error updating user status: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }
}
