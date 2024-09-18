package com.dropzone.user.dto;

import com.dropzone.user.entity.UserEntity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import lombok.*;

import java.time.LocalDateTime;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserDTO {
    private int userId;
    private String userEmail;
    private String userPassword;
    private String userNickname;
    private LocalDateTime userCreatedAt;
    private LocalDateTime userDeletedAt;
    private long userGameMoney;
    private int userLevel;
    private LocalDateTime userAttendance; // 출석일
    private LocalDateTime userLastLogin;
    private String userStatus;
    private String userProfileImage;
    private String userChosenCharacter;  // 선택된 캐릭터
    private boolean userIsOnline;

    public static UserDTO toUserDTO(UserEntity userEntity){
        UserDTO userDTO = new UserDTO();
        userDTO.setUserId(userEntity.getUserId());
        userDTO.setUserEmail(userEntity.getUserEmail());
        userDTO.setUserPassword(userEntity.getUserPassword());
        userDTO.setUserNickname(userEntity.getUserNickname());
        userDTO.setUserCreatedAt(userEntity.getUserCreatedAt());
        userDTO.setUserDeletedAt(userEntity.getUserDeletedAt());
        userDTO.setUserGameMoney(userEntity.getUserGameMoney());
        userDTO.setUserLevel(userEntity.getUserLevel());
        userDTO.setUserAttendance(userEntity.getUserAttendance()); // 타입 맞춤
        userDTO.setUserLastLogin(userEntity.getUserLastLogin());
        userDTO.setUserStatus(userEntity.getUserStatus());
        userDTO.setUserProfileImage(userEntity.getUserProfileImage());
        userDTO.setUserChosenCharacter(userEntity.getUserChosenCharacter()); // 이름 맞춤
        userDTO.setUserIsOnline(userEntity.isUserIsOnline());
        return userDTO;
    }
}
