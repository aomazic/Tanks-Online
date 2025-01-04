package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.models.User;
import hr.antitalent.tanks_backend.repositories.UserRepository;
import org.springframework.stereotype.Service;

import java.util.logging.Logger;

@Service
public class UserService {
    private static final Logger logger = Logger.getLogger(UserService.class.getName());

    private final UserRepository userRepository;

    public UserService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    public String deleteUser(String username) {
        User user = userRepository.findByUsername(username)
                .orElseThrow(() -> new IllegalArgumentException("User not found"));
        userRepository.delete(user);
        return "User deleted";
    }
}
