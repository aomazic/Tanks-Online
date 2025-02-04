package hr.antitalent.tanks_backend.configurations;


import hr.antitalent.tanks_backend.filters.JwtHandshakeInterceptor;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.messaging.Message;
import org.springframework.security.authorization.AuthorizationManager;
import org.springframework.security.config.annotation.web.socket.EnableWebSocketSecurity;
import org.springframework.security.messaging.access.intercept.MessageMatcherDelegatingAuthorizationManager;

@Configuration
@EnableWebSocketSecurity
public class WebSocketSecurityConfig {
    
    @Bean
    AuthorizationManager<Message<?>> authorizationManager(MessageMatcherDelegatingAuthorizationManager.Builder messages) {
        messages.simpDestMatchers("/user/queue/errors").permitAll()
                .simpSubscribeDestMatchers("/topic/ws-game").authenticated()
                .simpDestMatchers("/admin/**").hasRole("ADMIN")
                .anyMessage().denyAll();
        return messages.build();
    }
}
