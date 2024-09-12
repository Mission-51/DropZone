package com.dropzone.user.dto;

import lombok.*;
import org.springframework.web.multipart.MultipartFile;

import java.time.LocalDateTime;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserRequestDTO {
    private int userId;                 // 회원 ID
    private String userEmail;           // 이메일
    private String userPassword;        // 비밀번호
    private String userNickname;        // 닉네임
    private MultipartFile userProfileImage;  // 프로필 이미지 (파일 업로드)
    private LocalDateTime userCreatedAt;     // 가입일
    private LocalDateTime userDeletedAt;     // 탈퇴일 (nullable)
    private long userGameMoney;         // 게임 머니
    private int userLevel;              // 레벨
    private boolean userAttendance;     // 출석일
    private LocalDateTime userLastLogin;    // 마지막 로그인 날짜
    private String userStatus;          // 유저 상태 (ON: 온라인, OFF: 오프라인)
    private String userChosenCharacter; // 선택된 캐릭터
    private boolean userIsOnline;       // 접속 여부
}
