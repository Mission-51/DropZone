package com.dropzone.auth.interceptor;

import io.github.bucket4j.Bandwidth;
import io.github.bucket4j.Bucket;
import io.github.bucket4j.Bucket4j;
import io.github.bucket4j.Refill;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import org.springframework.stereotype.Component;
import org.springframework.web.servlet.HandlerInterceptor;

import java.time.Duration;

@Component
public class RateLimitInterceptor implements HandlerInterceptor {

    private final Bucket bucket;

    public RateLimitInterceptor() {
        // 1초에 1개의 요청만 허용하도록 설정
        Bandwidth limit = Bandwidth.classic(1, Refill.intervally(1, Duration.ofSeconds(1)));
        this.bucket = Bucket4j.builder().addLimit(limit).build();
    }

    @Override
    public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler) throws Exception {
        // 1초에 1개씩 토큰을 소비할 수 있도록 제한
        if (bucket.tryConsume(1)) {
            return true; // 요청 허용
        } else {
            response.setStatus(429);
            response.getWriter().write("1초에 한 번만 로그인할 수 있습니다.");
            return false; // 요청 거부
        }
    }
}
