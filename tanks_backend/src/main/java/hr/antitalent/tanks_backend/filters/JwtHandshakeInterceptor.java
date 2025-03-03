package hr.antitalent.tanks_backend.filters;

import hr.antitalent.tanks_backend.services.JwtService;
import lombok.RequiredArgsConstructor;
import org.springframework.http.HttpStatus;
import org.springframework.http.server.ServerHttpRequest;
import org.springframework.http.server.ServerHttpResponse;
import org.springframework.http.server.ServletServerHttpRequest;
import org.springframework.lang.NonNull;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.stereotype.Component;
import org.springframework.web.socket.WebSocketHandler;
import org.springframework.web.socket.server.HandshakeInterceptor;

import java.util.List;
import java.util.Map;

@Component
@RequiredArgsConstructor
public class JwtHandshakeInterceptor implements HandshakeInterceptor {

    private final JwtService jwtService;
    private final UserDetailsService userDetailsService;

    @Override
    public boolean beforeHandshake(@NonNull ServerHttpRequest request,
                                   @NonNull ServerHttpResponse response,
                                   @NonNull WebSocketHandler wsHandler,
                                   @NonNull Map<String, Object> attributes) {
        String jwt = extractToken(request);

        if (jwt == null) {
            response.setStatusCode(HttpStatus.UNAUTHORIZED);
            return false;
        }
        
        String username = jwtService.extractUsername(jwt);
        
        if (username == null) {
            response.setStatusCode(HttpStatus.NOT_FOUND);
            return false;
        }
        
        UserDetails userDetails = this.userDetailsService.loadUserByUsername(username);
        
        if (jwtService.isTokenValid(jwt, userDetails)) {
            attributes.put("user", userDetails);
            return true;
        }

        response.setStatusCode(HttpStatus.UNAUTHORIZED);
        return false;
    }

    private String extractToken(ServerHttpRequest request) {
        List<String> headers = request.getHeaders().get("Authorization");
        if (headers != null && !headers.isEmpty()) {
            String header = headers.getFirst();
            if (header.startsWith("Bearer ")) {
                return header.substring(7);
            }
        }
        return null;
    }

    @Override
    public void afterHandshake(@NonNull ServerHttpRequest request,
                               @NonNull ServerHttpResponse response,
                               @NonNull WebSocketHandler wsHandler,
                               @NonNull Exception exception) {
        
    }
}