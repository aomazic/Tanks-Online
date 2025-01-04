package hr.antitalent.tanks_backend;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cache.annotation.EnableCaching;

@SpringBootApplication
@EnableCaching
public class TanksBackendApplication {
    public static void main(String[] args) {
        SpringApplication.run(TanksBackendApplication.class, args);
    }

}
