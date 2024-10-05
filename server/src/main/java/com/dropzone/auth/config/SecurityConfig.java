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
import org.springframework.web.cors.CorsConfiguration;

import java.util.List;

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
                .cors(cors -> cors.configurationSource(request -> {
                    // CORS 설정
                            CorsConfiguration config = new CorsConfiguration();
                            config.setAllowedOriginPatterns(List.of("*")); // 모든 출처 허용
                            config.setAllowedMethods(List.of("GET", "POST", "PUT", "DELETE", "OPTIONS"));  // 허용할 HTTP 메서드
                            config.setAllowedHeaders(List.of("*"));  // 모든 헤더 허용
                            config.setAllowCredentials(true);
                            return config;
                        }))
                .sessionManagement(sessionManagement ->
                        sessionManagement
                                .sessionCreationPolicy(SessionCreationPolicy.IF_REQUIRED)
                                .maximumSessions(1) // 한 사용자당 하나의 세션만 허용
                                .maxSessionsPreventsLogin(true)
                )
                // 세션을 사용하지 않도록 설정 (JWT로 인증을 처리하므로 STATELESS 모드 사용)
                .authorizeHttpRequests(authorizeHttpRequests ->
                                authorizeHttpRequests
                                        .requestMatchers("/**").permitAll()
                                        .anyRequest().authenticated());
        http
                .addFilterBefore(jwtAuthenticationFilter, UsernamePasswordAuthenticationFilter.class);
        // JWT 인증 필터를 UsernamePasswordAuthenticationFilter 전에 추가
        return http.build();
    }
}
