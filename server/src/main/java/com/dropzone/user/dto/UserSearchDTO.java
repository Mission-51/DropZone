package com.dropzone.user.dto;

import com.dropzone.user.entity.UserEntity;
import lombok.*;

import java.time.LocalDateTime;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserSearchDTO {
    private int userId;                 // 회원 ID
    private String userEmail;           // 이메일
    private String userPassword;        // 비밀번호
    private String userNickname;        // 닉네임
    private LocalDateTime userCreatedAt; // 가입일
    private LocalDateTime userDeletedAt; // 탈퇴일
    private long userGameMoney;         // 게임 머니
    private int userLevel;              // 레벨
    private LocalDateTime userAttendance;     // 출석 여부
    private LocalDateTime userLastLogin; // 마지막 로그인 날짜
    private String userStatus;          // 유저 상태 (ON: 온라인, OFF: 오프라인)
    private String userProfileImage;    // 프로필 이미지
    private String userChosenCharacter; // 선택된 캐릭터
    private boolean userIsOnline;       // 접속 여부

    public static UserSearchDTO toUserSearchDTO(UserEntity userEntity){
        UserSearchDTO userSearchDTO = new UserSearchDTO();
        userSearchDTO.setUserId(userEntity.getUserId());
        userSearchDTO.setUserEmail(userEntity.getUserEmail());
        userSearchDTO.setUserPassword(userEntity.getUserPassword());
        userSearchDTO.setUserNickname(userEntity.getUserNickname());
        userSearchDTO.setUserCreatedAt(userEntity.getUserCreatedAt());
        userSearchDTO.setUserDeletedAt(userEntity.getUserDeletedAt());
        userSearchDTO.setUserGameMoney(userEntity.getUserGameMoney());
        userSearchDTO.setUserLevel(userEntity.getUserLevel());
        userSearchDTO.setUserAttendance(userEntity.getUserAttendance());
        userSearchDTO.setUserLastLogin(userEntity.getUserLastLogin());
        userSearchDTO.setUserStatus(userEntity.getUserStatus());
        userSearchDTO.setUserProfileImage(userEntity.getUserProfileImage());
        userSearchDTO.setUserChosenCharacter(userEntity.getUserChosenCharacter());
        userSearchDTO.setUserIsOnline(userEntity.isUserIsOnline());

        return userSearchDTO;
    }
}
