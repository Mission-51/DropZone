package com.dropzone.auth.controller;

import com.dropzone.auth.jwt.JwtTokenProvider;
import com.dropzone.auth.service.AuthService;
import com.dropzone.user.dto.UserDTO;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.HashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/auth")
@CrossOrigin("*")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "로그인 API", description = "로그인 / 로그아웃 / AccessToken 갱신 / AccessToken 만료 확인")
public class AuthController {

    // 인증 관련 비즈니스 로직을 담당하는 AuthService를 주입받음
    private final AuthService authService;
    // JWT 토큰 관련 처리를 담당하는 JwtTokenProvider를 주입받음
    private final JwtTokenProvider jwtTokenProvider;

    // 로그인
    @Operation(summary = "로그인 API", description = "email, password을 입력하여 로그인")
    @PostMapping("/login")
    public ResponseEntity<Map<String, Object>> loginByEmail(
            // 로그인 요청 시 필요한 필드를 설명하는 Swagger 어노테이션
            @io.swagger.v3.oas.annotations.parameters.RequestBody(
                    description = "userEmail, userPassword",
                    required = true,
                    content = @Content(
                            mediaType = "application/json",
                            schema = @Schema(
                                    example = "{\"userEmail\": \"yg9618@naver.com\", \"userPassword\": \"qwer1234!\"}"
                            )
                    )
            )
            // 로그인 요청 시 email, password, device 정보를 JSON으로 받아옴
            @RequestBody Map<String, String> credentials) {

        Map<String, Object> response = new HashMap<>();
        String userEmail = credentials.get("userEmail");
        String userPassword = credentials.get("userPassword");

        try {
            // AuthService에서 로그인 처리
            UserDTO userDTO = authService.loginByEmail(userEmail, userPassword);
            log.info(userEmail,userPassword);

            if (userDTO != null) {
                // 성공적으로 로그인하면 AccessToken과 RefreshToken을 발급
                String accessToken = jwtTokenProvider.generateAccessToken(userEmail);
                String refreshToken = jwtTokenProvider.generateRefreshToken(userEmail);
                int userId = userDTO.getUserId();

                // 응답에 필요한 정보 추가
                response.put("message", "로그인 성공");
                response.put("accessToken", accessToken);
                response.put("refreshToken", refreshToken);
                response.put("id", userId);

                log.info("로그인 성공: email={}, id = {}", userEmail, userId);

                return ResponseEntity.ok(response);
            } else {
                // 로그인 실패 시 처리
                log.error("로그인 실패: 잘못된 이메일 또는 비밀번호: email={}", userEmail);
                response.put("message", "로그인 실패: 잘못된 이메일 또는 비밀번호");
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            log.error("로그인 중 오류 발생 : {}", e.getMessage(), e);
            response.put("message", "로그인 실패");
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    // 로그아웃
    @Operation(summary = "로그아웃 API", description = "로그아웃 - accessToken을 blackList에 추가")
    @PostMapping("/logout")
    public ResponseEntity<Map<String, Object>> logout(
            // 로그아웃 요청 시 필요한 accessToken을 설명하는 Swagger 어노테이션
            @io.swagger.v3.oas.annotations.parameters.RequestBody(
                    description = "액세스 토큰",
                    required = true,
                    content = @Content(
                            mediaType = "application/json",
                            schema = @Schema(
                                    example = "{\"accessToken\": \"your_access_token_here\"}"
                            )
                    )
            )
            // 로그아웃 요청 시 accessToken 정보를 JSON으로 받아옴
            @RequestBody Map<String, String> body) {

        Map<String, Object> response = new HashMap<>();
        String accessToken = body.get("accessToken");

        try {
            if (jwtTokenProvider.validateToken(accessToken)) {
                // 토큰이 유효하다면 블랙리스트에 추가하여 로그아웃 처리
//                String email = JwtTokenProvider.getEmailFromToken(accessToken);
//                 authService.logout(email); // 서비스 레이어에서 로그아웃 처리 및 is_online 업데이트
                jwtTokenProvider.invalidateToken(accessToken);
                response.put("message", "로그아웃 성공");
                log.info("로그아웃 성공: accessToken={}", accessToken);

                return ResponseEntity.ok(response);
            } else {
                // 유효하지 않은 토큰일 때 처리
                response.put("message", "유효하지 않은 액세스 토큰입니다.");

                log.error("로그아웃 실패: 유효하지 않은 엑세스 토큰 - accessToken={}", accessToken);
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            log.error("로그아웃 중 오류 발생 : {}", e.getMessage(), e);
            response.put("message", "로그아웃 서버 에러");
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    // 토큰 갱신
    @Operation(summary = "AccessToken 갱신 API", description = "RefreshToken으로 accessToken 생성")
    @PostMapping("/refresh")
    public ResponseEntity<Map<String, Object>> refreshAccessToken(
            // RefreshToken을 설명하는 Swagger 어노테이션
            @io.swagger.v3.oas.annotations.parameters.RequestBody(
                    description = "리프레시 토큰",
                    required = true,
                    content = @Content(
                            mediaType = "application/json",
                            schema = @Schema(
                                    example = "{\"refreshToken\": \"your_refresh_token_here\"}"
                            )
                    )
            )
            // 요청으로 전달받은 refreshToken을 JSON으로 받아옴
            @RequestBody Map<String, String> body) {

        Map<String, Object> response = new HashMap<>();
        String refreshToken = body.get("refreshToken");

        try {
            if (jwtTokenProvider.validateToken(refreshToken)) {
                // RefreshToken이 유효할 때 AccessToken을 재발급
                String userEmail = JwtTokenProvider.getEmailFromToken(refreshToken);
                String newAccessToken = jwtTokenProvider.generateAccessToken(userEmail);
                response.put("message", "액세스 토큰 갱신 성공");
                response.put("accessToken", newAccessToken);

                log.info("액세스 토큰 갱신 성공: email={}", userEmail);
                return ResponseEntity.ok(response);
            } else {
                // 유효하지 않은 RefreshToken 처리
                log.error("액세스 토큰 갱신 실패: 유효하지 않은 리프레시 토큰 - refreshToken={}", refreshToken);
                response.put("message", "리프레시 토큰이 유효하지 않습니다.");
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(response);
            }
        } catch (Exception e) {
            log.error("AccessToken 갱신 중 오류 발생 : {}", e.getMessage(), e);
            response.put("message", "액세스 토큰 갱신 서버 에러");
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }

    @Operation(summary = "AccessToken 만료 여부 확인 API", description = "AccessToken 만료 여부 확인")
    @GetMapping("/checkAccessTokenExpired/{accessToken}")
    public ResponseEntity<Map<String, Object>> checkAccessTokenExpired(
            // 만료 여부를 확인할 accessToken을 설명하는 Swagger 어노테이션
            @Parameter(description = "accessToken", required = true) @PathVariable("accessToken") String accessToken) {

        Map<String, Object> response = new HashMap<>();

        try {
            boolean isExpired = jwtTokenProvider.validateToken(accessToken);
            response.put("isExpired", isExpired);
            if (isExpired) {
                response.put("message", "AccessToken이 만료되었습니다.");
            } else {
                response.put("message", "AccessToken이 유효합니다.");
            }
            return ResponseEntity.ok(response);
        } catch (Exception e) {
            log.error("AccessToken 만료 여부 확인 중 오류 발생 : {}", e.getMessage(), e);
            response.put("message", "서버 오류로 인해 AccessToken 만료 여부를 확인할 수 없습니다.");
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(response);
        }
    }
}
