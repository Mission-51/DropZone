package com.dropzone.auth.jwt;

import com.dropzone.auth.service.CustomUserDetailsService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Lazy;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.web.authentication.WebAuthenticationDetailsSource;
import org.springframework.stereotype.Component;
import org.springframework.web.filter.OncePerRequestFilter;

import jakarta.servlet.FilterChain;
import jakarta.servlet.ServletException;
import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import java.io.IOException;

@Component
public class JwtAuthenticationFilter extends OncePerRequestFilter { // 이 클래스는 요청당 한 번 실행되는 필터로, 매 요청마다 JWT를 검증하고, 사용자 인증을 처리

    @Autowired
    private JwtTokenProvider jwtTokenProvider; // JWT 토큰 생성 및 검증 로직을 처리하는 클래스

    @Autowired
    @Lazy
    private CustomUserDetailsService customUserDetailsService; // 사용자 정보를 데이터베이스에서 조회하는 서비스

    @Override
    protected void doFilterInternal(HttpServletRequest request, HttpServletResponse response, FilterChain filterChain)
            throws ServletException, IOException {
        // HTTP 요청에서 토큰을 가져옴
        String token = getTokenFromRequest(request);

        // 토큰이 존재하고 유효하다면 아래의 작업을 수행
        if (token != null && jwtTokenProvider.validateToken(token)) {

            // 토큰에서 이메일 정보를 추출
            String email = jwtTokenProvider.getEmailFromToken(token);

            // 해당 이메일로 사용자 정보를 조회
            UserDetails userDetails = customUserDetailsService.loadUserByUsername(email);

            // 사용자의 인증 정보 생성 (비밀번호는 null로 설정)
            UsernamePasswordAuthenticationToken authentication = new UsernamePasswordAuthenticationToken(
                    userDetails, null, userDetails.getAuthorities());

            // 추가적인 요청 정보를 설정 (예: IP 주소)
            authentication.setDetails(new WebAuthenticationDetailsSource().buildDetails(request));

            // SecurityContextHolder에 인증 정보 설정
            SecurityContextHolder.getContext().setAuthentication(authentication);
        }

        // 다음 필터로 요청을 전달
        filterChain.doFilter(request, response);
    }

    // HTTP 요청에서 "Authorization" 헤더에서 Bearer 토큰을 추출하는 메서드
    private String getTokenFromRequest(HttpServletRequest request) {
        String bearerToken = request.getHeader("Authorization");

        // Bearer로 시작하는 토큰이 있으면 실제 토큰 값만 추출
        if (bearerToken != null && bearerToken.startsWith("Bearer ")) {
            return bearerToken.substring(7); // Bearer 다음의 실제 토큰 부분을 리턴
        }
        return null;
    }
}
