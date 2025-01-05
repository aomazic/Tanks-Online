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
     * @param userId    User ID.
     * @param newStatus New status.
     * @return ResponseEntity with updated User.
     */
    @PutMapping("/{userId}/status")
    public ResponseEntity<User> updateUserStatus(
            @PathVariable Long userId,
            @RequestParam UserStatus newStatus) {
        try {
            User updatedUser = userService.updateUserStatus(userId, newStatus);
            return ResponseEntity.ok(updatedUser);
        } catch (IllegalArgumentException e) {
            logger.error("Error updating user status: {}", e.getMessage());
            return ResponseEntity.notFound().build();
        }
    }
}
