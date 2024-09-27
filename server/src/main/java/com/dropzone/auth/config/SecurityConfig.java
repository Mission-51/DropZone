package com.dropzone.auth.config;

import com.dropzone.auth.jwt.JwtAuthenticationFilter;
import com.dropzone.auth.service.CustomUserDetailsService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configurers.AbstractHttpConfigurer;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.authentication.UsernamePasswordAuthenticationFilter;

@Configuration  // 이 클래스가 설정 클래스임을 나타냄
@EnableWebSecurity  // Spring Security를 활성화
@EnableMethodSecurity  // 개별 메소드에 대한 권한 검증을 활성화

public class SecurityConfig {

    @Autowired
    private JwtAuthenticationFilter jwtAuthenticationFilter;  // JWT 인증 필터를 주입받음

    @Bean
    public UserDetailsService userDetailsService() {
        // 사용자 정보를 로드하는 서비스 정의
        return new CustomUserDetailsService();
    }

    @Bean
    public PasswordEncoder passwordEncoder() {
        // 비밀번호 암호화를 위한 Bean 생성 (BCrypt 사용)
        return new BCryptPasswordEncoder();
    }

    @Bean
    public AuthenticationManager authenticationManager(AuthenticationConfiguration authenticationConfiguration) throws Exception {
        // AuthenticationManager를 생성하여 Spring Security에서 인증 관리를 처리함
        return authenticationConfiguration.getAuthenticationManager();
    }

    @Bean
    public SecurityFilterChain securityFilterChain(HttpSecurity http) throws Exception {
        // Spring Security의 필터 체인 설정

        http
                .csrf(AbstractHttpConfigurer::disable)  // CSRF 보안을 비활성화 (JWT 사용 시 일반적으로 CSRF는 필요 없음)
                .sessionManagement(sessionManagement ->
                        sessionManagement.sessionCreationPolicy(SessionCreationPolicy.STATELESS))
                // 세션을 사용하지 않도록 설정 (JWT로 인증을 처리하므로 STATELESS 모드 사용)
                .authorizeHttpRequests(authorizeHttpRequests -> authorizeHttpRequests
                                .requestMatchers("/**").permitAll()
                                // 모든 요청에 대해 접근을 허용
                                .anyRequest().authenticated()
                        // 그 외의 요청은 인증을 요구
                )
                .cors(cors -> cors.configurationSource(request -> {
    var corsConfiguration = new org.springframework.web.cors.CorsConfiguration();
    
    // 허용할 도메인 목록 (Swagger UI와 Unity 클라이언트)
    corsConfiguration.setAllowedOrigins(List.of(
        "http://localhost:3000",   // Swagger UI (예: React와 연동된 경우)
        "http://localhost"
    ));
    
    corsConfiguration.setAllowedMethods(List.of("GET", "POST", "PUT", "DELETE", "OPTIONS"));
    corsConfiguration.setAllowedHeaders(List.of("*"));  // 모든 헤더 허용
    corsConfiguration.setAllowCredentials(true);  // 인증 정보 허용 (필요시)
    
    return corsConfiguration;
}));

        http.addFilterBefore(jwtAuthenticationFilter, UsernamePasswordAuthenticationFilter.class);
        // JWT 인증 필터를 UsernamePasswordAuthenticationFilter 전에 추가
        return http.build();
    }
}
