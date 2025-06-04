package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.domain.User;
import hr.antitalent.tanks_backend.enums.UserStatus;
import hr.antitalent.tanks_backend.services.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@RestController
@RequestMapping("/api/user")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "User Management", description = "Endpoints for managing user accounts and statuses")
public class UserController {
    private final UserService userService;

    /**
     * Update status of a user.
     *
     * @param userName  Username.
     * @param newStatus New status.
     * @return ResponseEntity with updated User.
     */
    @Operation(summary = "Update user status", description = "Updates a user's online status by username")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "Status updated successfully",
                    content = @Content(schema = @Schema(implementation = User.class))),
        @ApiResponse(responseCode = "404", description = "User not found"),
        @ApiResponse(responseCode = "400", description = "Invalid status update request")
    })
    @PostMapping("/{userName}/status")
    public ResponseEntity<User> updateUserStatus(
            @Parameter(description = "Username of the user to update") @PathVariable String userName,
            @Parameter(description = "New status to set") @RequestParam UserStatus newStatus) {
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
