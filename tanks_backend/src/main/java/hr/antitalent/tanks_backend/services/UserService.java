package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.enums.UserStatus;
import hr.antitalent.tanks_backend.domain.User;
import hr.antitalent.tanks_backend.repositories.UserRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

@Service
public class UserService {
    private static final Logger logger = LoggerFactory.getLogger(UserService.class);
    
    private final UserRepository userRepository;


    public UserService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Transactional
    public String deleteUser(Long userId) {
        userRepository.deleteById(userId);
        logger.info("User {} deleted", userId);
        return "User deleted";
    }

    @Transactional
    public User updateUserStatus(String userName, UserStatus newStatus) {
        User user = userRepository.findByUsername(userName)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));

        logger.info("Updating user {} status to {}", user.getUsername(), newStatus);
        user.setStatus(newStatus);
        return userRepository.save(user);
    }
}
