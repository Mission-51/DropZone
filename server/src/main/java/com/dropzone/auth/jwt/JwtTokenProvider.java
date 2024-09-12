package com.dropzone.auth.jwt;

import io.jsonwebtoken.Claims;
import io.jsonwebtoken.Jwts;
import io.jsonwebtoken.SignatureAlgorithm;
import org.springframework.stereotype.Component;

import java.util.Date;
import java.util.HashSet;
import java.util.Set;

@Component // Spring Bean으로 등록, 의존성 주입이 가능하게 설정
public class JwtTokenProvider {

    // 비밀 키. JWT를 서명하고 검증하는 데 사용됨
    private static final String SECRET_KEY = "5ad900a5079ed71ab8bf24752f8cf3025a164ba352a8d85d23ff42973ee4cc53911d5e2d2b4543848265668243df7098d6076380a9bc50ce6d02d8687c3da881";

    // 액세스 토큰의 만료 시간: 1시간 (3600000밀리초)
    private final long ACCESS_TOKEN_EXPIRATION_TIME = 3600000;

    // 리프레시 토큰의 만료 시간: 2주 (1209600000밀리초)
    private final long REFRESH_TOKEN_EXPIRATION_TIME = 1209600000;

    // 블랙리스트로 추가된 토큰을 저장할 Set. 이 안에 들어간 토큰은 유효하지 않다고 간주됨
    private Set<String> tokenBlacklist = new HashSet<>();

    // 이메일을 기반으로 액세스 토큰 생성
    public String generateAccessToken(String email) {
        // 클레임(Claims): 토큰에 담길 정보. 여기서는 사용자의 이메일 정보를 주제로 설정
        Claims claims = Jwts.claims().setSubject(email);

        // 현재 시간과 만료 시간을 설정
        Date now = new Date();
        Date expiryDate = new Date(now.getTime() + ACCESS_TOKEN_EXPIRATION_TIME);

        // JWT 빌더를 사용해 토큰을 생성
        return Jwts.builder()
                .setClaims(claims) // 클레임 설정
                .setIssuedAt(now) // 발행 시간 설정
                .setExpiration(expiryDate) // 만료 시간 설정
                .signWith(SignatureAlgorithm.HS512, SECRET_KEY) // 서명 알고리즘과 비밀 키를 사용해 서명
                .compact(); // 최종적으로 토큰을 문자열 형태로 변환
    }

    // 이메일을 기반으로 리프레시 토큰 생성
    public String generateRefreshToken(String email) {
        Claims claims = Jwts.claims().setSubject(email); // 클레임에 이메일 추가
        Date now = new Date();
        Date expiryDate = new Date(now.getTime() + REFRESH_TOKEN_EXPIRATION_TIME); // 만료 시간 설정

        // JWT 리프레시 토큰 생성
        return Jwts.builder()
                .setClaims(claims)
                .setIssuedAt(now)
                .setExpiration(expiryDate)
                .signWith(SignatureAlgorithm.HS512, SECRET_KEY)
                .compact();
    }

    // 토큰에서 이메일 정보(Subject)를 추출하는 메서드
    public static String getEmailFromToken(String token) {
        // 토큰을 파싱하여 클레임에서 이메일(Subject) 반환
        Claims claims = Jwts.parser()
                .setSigningKey(SECRET_KEY) // 비밀 키를 사용해 토큰을 파싱
                .parseClaimsJws(token) // 토큰에서 서명을 검증하고, 클레임을 추출
                .getBody();

        return claims.getSubject(); // Subject (이메일) 반환
    }

    // 토큰이 유효한지 검사하는 메서드
    public boolean validateToken(String token) {
        try {
            // 토큰을 파싱하고, 서명이 유효한지 확인
            Jwts.parser().setSigningKey(SECRET_KEY).parseClaimsJws(token);
            return true; // 토큰이 유효하다면 true 반환
        } catch (Exception e) {
            return false; // 예외가 발생하면 토큰이 유효하지 않음
        }
    }

    // 토큰을 블랙리스트에 추가하여 무효화하는 메서드 (로그아웃 시 사용)
    public void invalidateToken(String token) {
        tokenBlacklist.add(token); // 블랙리스트에 토큰을 추가
    }
}
