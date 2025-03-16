package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.User;
import hr.antitalent.tanks_backend.enums.UserStatus;
import hr.antitalent.tanks_backend.services.UserService;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/user")
@RequiredArgsConstructor
@Slf4j
public class UserController {
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
        log.info("Request to update user status: user='{}', new status={}", userName, newStatus);

        try {
            User updatedUser = userService.updateUserStatus(userName, newStatus);
            log.info("User status updated successfully: user='{}', id={}, new status={}",
                    updatedUser.getUsername(), updatedUser.getId(), updatedUser.getStatus());
            return ResponseEntity.ok(updatedUser);
        } catch (IllegalArgumentException e) {
            log.error("Failed to update status for user '{}' to {}: {}", userName, newStatus, e.getMessage());
            return ResponseEntity.notFound().build();
        } catch (Exception e) {
            log.error("Unexpected error updating status for user '{}' to {}: {}",
                    userName, newStatus, e.getMessage(), e);
            return ResponseEntity.badRequest().build();
        }
    }
}
