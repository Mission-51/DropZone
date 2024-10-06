package com.dropzone.config;

import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Configuration // Spring 설정 클래스로 인식되도록 함
public class WebMvcConfiguration implements WebMvcConfigurer {

    // CORS 설정을 위한 메서드를 정의함
    public WebMvcConfigurer corsConfigurer() {
        return new WebMvcConfigurer() {

            // CORS 정책을 추가하는 메서드
            @Override
            public void addCorsMappings(CorsRegistry registry) {
                // 모든 경로에 대해 CORS 설정을 적용
                registry.addMapping("/**")
                        // 특정 도메인에서의 요청을 허용
                        .allowedOriginPatterns("*")
                        // 모든 요청 헤더를 허용
                        .allowedHeaders("*")
                        // 허용할 HTTP 메서드를 지정 (GET, POST, PUT, DELETE, PATCH 등)
                        .allowedMethods("GET", "POST", "PUT", "DELETE", "HEAD", "OPTIONS" , "PATCH")
                        // 응답 헤더에서 Authorization과 RefreshToken을 노출시킴 (프론트엔드에서 해당 헤더 접근 가능)
                        .exposedHeaders("Authorization", "RefreshToken")
                        // 자격 증명(쿠키나 인증 정보 등)을 포함한 요청을 허용
                        .allowCredentials(true);
            }
        };
    }
}
