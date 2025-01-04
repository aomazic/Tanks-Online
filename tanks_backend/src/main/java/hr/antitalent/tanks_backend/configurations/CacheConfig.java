package hr.antitalent.tanks_backend.configurations;


import com.github.benmanes.caffeine.cache.Caffeine;
import org.springframework.cache.CacheManager;
import org.springframework.cache.annotation.EnableCaching;
import org.springframework.cache.caffeine.CaffeineCacheManager;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Primary;

import java.util.List;
import java.util.concurrent.TimeUnit;

@EnableCaching
@Configuration
public class CacheConfig {
    public static final String AUTH_CACHE = "authCache";
    public static final String FIFTEEN_MINUTE_CACHE = "fifteenMinutesCache";
    
    @Bean
    public Caffeine<Object, Object> authCache() {
        return Caffeine.newBuilder()
                .maximumSize(500)
                .expireAfterWrite(1, TimeUnit.DAYS);
    }

    @Bean
    public Caffeine<Object, Object> fifteenMinutesCache() {
        return Caffeine.newBuilder()
                .maximumSize(500)
                .expireAfterWrite(15, TimeUnit.MINUTES);
    }

    @Bean
    public CacheManager authCacheManager() {
        CaffeineCacheManager cacheManager = new CaffeineCacheManager();
        cacheManager.setCaffeine(authCache());
        cacheManager.setCacheNames(List.of(AUTH_CACHE));
        return cacheManager;
    }

    @Bean
    @Primary
    public CacheManager fifteenMinutesCacheManager() {
        CaffeineCacheManager cacheManager = new CaffeineCacheManager();
        cacheManager.setCaffeine(fifteenMinutesCache());
        cacheManager.setCacheNames(List.of(FIFTEEN_MINUTE_CACHE));
        return cacheManager;
    }
}
