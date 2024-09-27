package com.dropzone.user.controller;

import com.dropzone.auth.jwt.JwtTokenProvider;
import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.dto.UserUpdateDTO;
import com.dropzone.user.service.EmailService;
import com.dropzone.user.service.UserService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

@RestController
@RequestMapping("/api/users")
@CrossOrigin("*")
@RequiredArgsConstructor
@Slf4j
@Tag(name = "회원 API", description = "중복 체크 / 이메일 인증 / 회원 가입 CRUD")
public class UserController {

    private final UserService userService;
    private final EmailService emailService;

    // 이메일 중복 체크
    @Operation(summary = "email 중복 체크 API", description = "회원 가입 시 email이 중복인지 확인")
    @GetMapping("/checkDuplicated/email/{email}")
    public ResponseEntity<String> checkDuplicatedEmail(
            @Parameter(description = "Email", required = true, example = "yg9618@naver.com")
            @PathVariable("email") String userEmail) {
        log.info("이메일 중복 체크 요청: {}", userEmail);
        try {
            boolean isDuplicated = userService.checkDuplicatedEmail(userEmail);

            if (!isDuplicated) {
                log.info("사용 가능한 이메일입니다: {}", userEmail);
                return ResponseEntity.ok("사용 가능한 email");
            } else {
                log.warn("중복된 이메일입니다: {}", userEmail);
                return ResponseEntity.status(HttpStatus.CONFLICT).body("중복된 email");
            }
        } catch (Exception e) {
            log.error("이메일 중복 체크 중 오류 발생: {}", userEmail, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // nickname 중복 체크
    @Operation(summary = "nickname 중복 체크 API", description = "회원 가입 시 nickname이 중복인지 확인")
    @GetMapping("/checkDuplicated/user_nickname/{user_nickname}")
    public ResponseEntity<String> checkDuplicatedNickname(
            @Parameter(description = "Nickname", required = true, example = "bts")
            @PathVariable("user_nickname") String user_nickname) {
        log.info("닉네임 중복 체크 요청: {}", user_nickname);
        try {
            boolean isDuplicated = userService.checkDuplicatedNickname(user_nickname);

            if (!isDuplicated) {
                log.info("사용 가능한 닉네임입니다: {}", user_nickname);
                return ResponseEntity.ok("사용 가능한 nickname 입니다");
            } else {
                log.warn("중복된 닉네임입니다: {}", user_nickname);
                return ResponseEntity.status(HttpStatus.CONFLICT).body("중복된 nickname 입니다");
            }
        } catch (Exception e) {
            log.error("닉네임 중복 체크 중 오류 발생: {}", user_nickname, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // 이메일 발송
    @Operation(summary = "이메일 발송 API", description = "email을 입력하여 인증코드를 발송")
    @PostMapping("/sendEmail")
    public ResponseEntity<?> sendEmail(
            @Parameter(description = "이메일 주소", required = true, example = "yg9618@gmail.com")
            @RequestParam("email") String userEmail) {
        log.info("이메일 발송 요청: {}", userEmail);
        try {
            if (userService.checkDuplicatedEmail(userEmail)) {
                log.warn("이미 존재하는 이메일입니다: {}", userEmail);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("이미 존재하는 email 입니다");
            } else {
                emailService.sendAuthenticationCodeEmail(userEmail);
                log.info("이메일 발송 성공: {}", userEmail);
                return ResponseEntity.accepted().body("이메일 발송에 성공했습니다");
            }
        } catch (Exception e) {
            log.error("이메일 발송 중 오류 발생: {}", userEmail, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // 이메일 인증
    @Operation(summary = "이메일 인증 API", description = "email, authenticationCode를 입력하여 이메일 인증")
    @GetMapping("/authenticateEmail")
    public ResponseEntity<?> authenticateEmail(
            @Parameter(description = "이메일 주소", required = true, example = "yg9618@naver.com")
            @RequestParam("user_email") String userEmail,
            @Parameter(description = "인증 코드", required = true, example = "")
            @RequestParam("authenticationCode") String authenticationCode) {
        log.info("이메일 인증 요청: {} / 인증 코드: {}", userEmail, authenticationCode);
        try {
            if (emailService.authenticateEmail(userEmail, authenticationCode)) {
                log.info("이메일 인증 성공: {}", userEmail);
                return ResponseEntity.ok("이메일 인증에 성공했습니다");
            } else {
                log.warn("이메일 인증 실패: {} / 인증 코드 불일치", userEmail);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("인증번호가 일치하지 않습니다");
            }
        } catch (Exception e) {
            log.error("이메일 인증 중 오류 발생: {}", userEmail, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // 회원가입
    @Operation(summary = "회원가입 API", description = "회원가입 페이지를 통해 입력한 정보로 회원가입 (이메일 인증 필수)")
    @PostMapping("/signup")
    public ResponseEntity<?> signUp(
            @io.swagger.v3.oas.annotations.parameters.RequestBody(
                    description = "이메일, 패스워드, 닉네임",
                    required = true,
                    content = @Content(
                            mediaType = "application/json",
                            schema = @Schema(
                                    example = "{\"userEmail\": \"yg9618@naver.com\", \"userPassword\": \"qwer1234!\", \"userNickname\": \"BTS\"}"
                            )
                    )
            )
            @RequestBody UserDTO user) {
        log.info("회원가입 요청: {}", user.getUserEmail());
        try {
            // 이메일 중복 체크
            if (userService.checkDuplicatedEmail(user.getUserEmail())) {
                log.warn("이미 존재하는 이메일로 회원가입 시도: {}", user.getUserEmail());
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("중복된 email 입니다");
            }
            log.info(user.toString());
            // 회원가입 처리
            userService.signUp(user);

            log.info("회원가입 성공: {}", user.getUserEmail());
            return ResponseEntity.accepted().body("회원 가입에 성공했습니다");
        } catch (Exception e) {
            log.error("회원가입 중 오류 발생: {}", user.getUserEmail(), e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }


    @Operation(summary = "전체 회원 검색 API", description = "가입된 전체 회원을 검색")
    @GetMapping("/search/all")
    public ResponseEntity<?> searchAllUser() {
        log.info("전체 회원 검색 요청");
        try {
            return new ResponseEntity<>(userService.searchAllUser(), HttpStatus.OK);
        } catch (Exception e) {
            log.error("전체 회원 검색 중 오류 발생: {}", e.getMessage(), e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    @Operation(summary = "특정 회원 검색 API", description = "Id를 입력하여 특정 회원 1명 조회")
    @GetMapping("/search/user_id/{user_id}")
    public ResponseEntity<?> searchById(
            @Parameter(description = "Id", required = true, example = "5")
            @PathVariable("user_id") int userId) {
        log.info("특정 회원 검색 요청: id={}", userId);
        try {
            UserSearchDTO user = userService.searchById(userId);

            if (user != null) {
                log.info("회원 검색 성공: id={}", userId);
                return ResponseEntity.accepted().body(user);
            } else {
                log.warn("회원 검색 실패: 없는 id={}", userId);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("없는 회원입니다");
            }
        } catch (Exception e) {
            log.error("회원 검색 중 오류 발생: id={}", userId, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    @Operation(summary = "Email 회원 검색 API", description = "email을 입력하여 특정 회원 1명을 조회")
    @GetMapping("/search/user_email/{user_email}")
    public ResponseEntity<?> searchByEmailForClient(
            @Parameter(description = "이메일 주소", required = true, example = "eoblue23@gmail.com")
            @PathVariable("email") String userEmail) {
        log.info("이메일로 회원 검색 요청: {}", userEmail);
        try {
            UserSearchDTO user = userService.searchByEmailForClient(userEmail);

            if (user != null) {
                log.info("이메일로 회원 검색 성공: {}", userEmail);
                return ResponseEntity.accepted().body(user);
            } else {
                log.warn("회원 검색 실패: 없는 이메일={}", userEmail);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("없는 회원입니다");
            }
        } catch (Exception e) {
            log.error("이메일로 회원 검색 중 오류 발생: {}", userEmail, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // nickname 으로 회원 조회
    @Operation(summary = "nickname 회원 검색 API", description = "nickname을 입력하여 특정 회원 1명을 조회")
    @ApiResponse(responseCode = "1000", description = "요청에 성공하였습니다.", content = @Content(mediaType = "application/json"))
    @GetMapping("/search/user_nickname/{user_nickname}")
    public ResponseEntity<?> searchByNickname(
            @Parameter(description = "Nickname", required = true, example = "bluebird")
            @PathVariable("nickname") String userNickname) {
        log.info("닉네임으로 회원 검색 요청: {}", userNickname);
        try {
            UserSearchDTO user = userService.searchByNickname(userNickname);

            if (user != null) {
                log.info("닉네임으로 회원 검색 성공: {}", userNickname);
                return ResponseEntity.accepted().body(user);
            } else {
                log.warn("회원 검색 실패: 없는 닉네임={}", userNickname);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("없는 회원입니다");
            }
        } catch (Exception e) {
            log.error("닉네임으로 회원 검색 중 오류 발생: {}", userNickname, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 에러 입니다");
        }
    }

    // 회원 정보 수정
    @Operation(summary = "회원 정보 수정 API", description = "회원 정보 수정")
    @PatchMapping(value = "/update")
    public ResponseEntity<?> updateUser(
            @RequestHeader("Authorization") String token,
            @RequestBody UserUpdateDTO userUpdateDTO
            ) {
        log.info("회원 정보 수정 요청: email={}");
        try {
            // JWT에서 이메일 추출
            String jwtToken = token.replace("Bearer ", "");
            String userEmail = JwtTokenProvider.getEmailFromToken(jwtToken);

            // 이메일로 기존 사용자 조회
            UserDTO existingUserDTO = userService.searchByEmail(userEmail);
            if (existingUserDTO == null) {
                log.warn("회원 정보 수정 실패: 인증되지 않은 사용자");
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("로그인이 필요합니다");
            }

            // 수정 가능한 필드만 업데이트
            if (userUpdateDTO.getUserPassword() != null && !userUpdateDTO.getUserPassword().isEmpty()) {
                existingUserDTO.setUserPassword(userUpdateDTO.getUserPassword());
            }
            if (userUpdateDTO.getUserNickname() != null && !userUpdateDTO.getUserNickname().isEmpty()) {
                existingUserDTO.setUserNickname(userUpdateDTO.getUserNickname());
            }
            if (userUpdateDTO.getUserProfileImage() != null && !userUpdateDTO.getUserProfileImage().isEmpty()) {
                existingUserDTO.setUserProfileImage(userUpdateDTO.getUserProfileImage());
            }

            // 수정된 사용자 정보 저장
            userService.updateUser(existingUserDTO.getUserId(), existingUserDTO);
            log.info("회원 정보 수정 성공: email={}", userEmail);
            return ResponseEntity.ok("회원 정보 수정에 성공했습니다");

        } catch (Exception e) {
            log.error("회원 정보 수정 중 오류 발생: email={}", e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류 입니다");
        }
    }

    // 회원 탈퇴
    @Operation(summary = "회원 탈퇴 API", description = "회원 탈퇴")
    @DeleteMapping("/delete")
    public ResponseEntity<?> deleteUser(
            @RequestHeader("Authorization") String token) {
        log.info("회원 탈퇴 요청");
        try {
            String jwtToken = token.replace("Bearer ", "");
            String userEmail = JwtTokenProvider.getEmailFromToken(jwtToken);

            UserDTO existingUser = userService.searchByEmail(userEmail);
            if (existingUser == null) {
                log.warn("회원 탈퇴 실패: 인증되지 않은 사용자");
                return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("로그인이 필요합니다");
            }

            userService.deleteUser(existingUser.getUserId());
            log.info("회원 탈퇴 성공: email={}", userEmail);
            return ResponseEntity.ok("회원 탈퇴에 성공했습니다");

        } catch (Exception e) {
            log.error("회원 탈퇴 중 오류 발생: email={}", JwtTokenProvider.getEmailFromToken(token.replace("Bearer ", "")), e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("서버 오류 입니다");
        }
    }

    @Operation(summary = "Email을 ID로 전환하는 API", description = "email을 입력하여 해당 id를 조회")
    @ApiResponse(responseCode = "1000", description = "요청에 성공하였습니다.", content = @Content(mediaType = "application/json"))
    @GetMapping("/changeEmailToId/{email}")
    public ResponseEntity<?> changeEmailToId(
            @Parameter(description = "Email", required = true, example = "yg9618@gmail.com")
            @PathVariable("email") String userEmail) {
        log.info("이메일을 ID로 전환 요청: {}", userEmail);
        try {
            int userId = userService.changeEmailToId(userEmail);

            if (userId != 0) {
                log.info("이메일을 ID로 전환 성공: email={} / id={}", userEmail, userId);
                return ResponseEntity.accepted().body(userId);
            } else {
                log.warn("이메일을 ID로 전환 실패: 없는 이메일={}", userEmail);
                return ResponseEntity.status(HttpStatus.BAD_REQUEST).body("없는 회원");
            }
        } catch (Exception e) {
            log.error("이메일을 ID로 전환 중 오류 발생: {}", userEmail, e);
            return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body("회원 검색 실패");
        }
    }
}
