package hr.antitalent.tanks_backend.configurations;

import org.springframework.boot.autoconfigure.AutoConfiguration;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.Message;
import org.springframework.security.authorization.AuthorizationManager;
import org.springframework.security.config.annotation.web.socket.EnableWebSocketSecurity;
import org.springframework.security.messaging.access.intercept.MessageMatcherDelegatingAuthorizationManager;

@AutoConfiguration
@EnableWebSocketSecurity
public class WebSocketSecurityConfig {
    @Bean
    AuthorizationManager<Message<?>> authorizationManager(MessageMatcherDelegatingAuthorizationManager.Builder messages) {
        messages.simpDestMatchers("/user/queue/errors").permitAll()
                .simpSubscribeDestMatchers("/topic/ws-game").authenticated()
                .simpSubscribeDestMatchers("/topic/chat/**").authenticated() // Allow all chat topics
                .simpDestMatchers("/app/chat.**/**").authenticated() // Allow all chat endpoints
                .simpDestMatchers("/admin/**").hasRole("ADMIN")
                .anyMessage().denyAll();
        return messages.build();
    }
}
