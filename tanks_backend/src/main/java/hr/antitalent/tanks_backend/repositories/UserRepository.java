package hr.antitalent.tanks_backend.repositories;

import hr.antitalent.tanks_backend.models.User;
import org.springframework.data.jpa.repository.JpaRepository;

public interface UserRepository extends JpaRepository<User, Integer> {

}
