package hr.antitalent.tanks_backend.services;

import hr.antitalent.tanks_backend.domain.User;
import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.io.Decoders;
import io.jsonwebtoken.security.Keys;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.cache.annotation.Cacheable;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;

import javax.crypto.SecretKey;
import java.util.Date;
import java.util.HashMap;
import java.util.Map;
import java.util.function.Function;

import static hr.antitalent.tanks_backend.configurations.CacheConfig.AUTH_CACHE;

@Service
public class JwtService {

    @Value("${security.jwt.secret-key}")
    private String secretKey;

    @Value("${security.jwt.expiration-time}")
    private long jwtExpiration;

    /**
     * Extract the username (subject) from the token.
     */
    @Cacheable(cacheNames = AUTH_CACHE, cacheManager = "authCacheManager", key = "#token")
    public String extractUsername(String token) {
        return extractClaimFromToken(token, Claims::getSubject);
    }

    /**
     * Extract a specific claim from the token.
     */
    public <T> T extractClaimFromToken(String token, Function<Claims, T> claimsResolver) {
        final Claims claims = extractAllClaimsFromToken(token);
        return claimsResolver.apply(claims);
    }

    /**
     * Generate a token with user details.
     */
    public String generateToken(User user) {
        return generateToken(new HashMap<>(), user);
    }

    /**
     * Generate a token with additional claims.
     */
    public String generateToken(Map<String, Object> extraClaims, User user) {
        return buildToken(extraClaims, user, jwtExpiration);
    }

    /**
     * Build the JWT token with claims, expiration, and signature.
     */
    private String buildToken(
            Map<String, Object> extraClaims,
            User user,
            long expiration
    ) {
        return Jwts.builder()
                .claims(extraClaims)
                .subject(user.getUsername())
                .issuedAt(new Date(System.currentTimeMillis()))
                .expiration(new Date(System.currentTimeMillis() + expiration))
                .signWith(getSignInKey())
                .compact();
    }

    /**
     * Validate if the token matches the user's username and is not expired.
     */
    @Cacheable(cacheNames = AUTH_CACHE, cacheManager = "authCacheManager", key = "#token + #user.username")
    public boolean isTokenValid(String token, UserDetails user) {
        final String username = extractUsername(token);
        return (username.equals(user.getUsername())) && !isTokenExpired(token);
    }

    /**
     * Check if the token is expired.
     */
    private boolean isTokenExpired(String token) {
        return extractExpiration(token).before(new Date());
    }

    /**
     * Extract expiration date from the token.
     */
    private Date extractExpiration(String token) {
        return extractClaimFromToken(token, Claims::getExpiration);
    }

    /**
     * Extract all claims from the token.
     */
    private Claims extractAllClaimsFromToken(String token) {
        return Jwts.parser()
                .verifyWith(getSignInKey()) // Correct use for HS256
                .build()
                .parseSignedClaims(token)
                .getPayload();
    }

    /**
     * Decode the secret key and return it as a SecretKey object.
     */
    private SecretKey getSignInKey() {
        byte[] keyBytes = Decoders.BASE64.decode(secretKey);
        return Keys.hmacShaKeyFor(keyBytes);
    }
}
