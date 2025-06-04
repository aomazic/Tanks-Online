package hr.antitalent.tanks_backend.configurations;    // Domain methods that encapsulate business logic

import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Contact;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.info.License;
import io.swagger.v3.oas.models.servers.Server;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import java.util.List;

@Configuration
public class OpenApiConfig {

    @Bean
    public OpenAPI tanksOnlineOpenAPI() {
        return new OpenAPI()
                .info(new Info()
                        .title("Tanks Online API")
                        .description("API documentation for ATA systems multiplayer game")
                        .version("1.0.0")
                        .contact(new Contact()
                                .name("Antitalent Team")
                                .email("omazicantonio7@gmail.com"))
                        .license(new License()
                                .name("Apache License 2.0")
                                .url("https://www.apache.org/licenses/LICENSE-2.0")))
                .servers(List.of(
                        new Server()
                                .url("http://localhost:8080")
                                .description("Local development server"))
                );
    }
}
