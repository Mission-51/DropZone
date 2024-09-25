//package com.dropzone.chat.config;
//
//import org.springframework.context.annotation.Bean;
//import org.springframework.context.annotation.Configuration;
//import org.springframework.web.servlet.config.annotation.CorsRegistry;
//import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
//
//@Configuration
//public class WebConfig {
//
//    @Bean
//    public WebMvcConfigurer corsConfigurer() {
//        return new WebMvcConfigurer() {
//            @Override
//            public void addCorsMappings(CorsRegistry registry) {
//                registry.addMapping("http://localhost:5173") // 모든 경로에 대해 CORS 허용
//                        .allowedOrigins("http://localhost:5173") // 허용할 클라이언트 도메인
//                        .allowedOriginPatterns("http://localhost:5173")
//                        .allowedMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
//                        .exposedHeaders("Authorization")
//                        .allowCredentials(true); // 자격 증명 허용 (withCredentials 사용 시 필요)
//            }
//        };
//    }
//}