package hr.antitalent.tanks_backend.controllers;

import hr.antitalent.tanks_backend.services.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.DeleteMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/api/admin")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "Admin Operations", description = "Administrative endpoints for user management")
public class AdminController {
    private final UserService userService;

    @Operation(summary = "Delete user", description = "Deletes a user account by ID (admin only)")
    @ApiResponses(value = {
        @ApiResponse(responseCode = "200", description = "User successfully deleted"),
        @ApiResponse(responseCode = "400", description = "Invalid user ID or deletion failed"),
        @ApiResponse(responseCode = "403", description = "Not authorized to perform this action")
    })
    @DeleteMapping("/delete")
    public ResponseEntity<String> deleteUser(
            @Parameter(description = "ID of the user to delete") @RequestParam Long userId) {
        log.info("Deleting user with id: {}", userId);
        return ResponseEntity.ok(userService.deleteUser(userId));
    }
}
