package com.dropzone.user.entity;

import com.dropzone.friend.entity.FriendShipEntity;
import com.dropzone.user.dto.UserDTO;
import jakarta.persistence.*;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import lombok.experimental.SuperBuilder;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

@Entity
@Getter
@Setter
@Table(name = "user")
@NoArgsConstructor
@AllArgsConstructor
@SuperBuilder
public class UserEntity extends BaseEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "user_id")
    private int userId;  // 회원 ID (기본 키, 자동 생성)

    @Column(name = "user_email", nullable = false, length = 50)
    private String userEmail;  // 사용자의 이메일 정보

    @Column(name = "user_password", nullable = false, length = 64)
    private String userPassword;  // 사용자의 비밀번호 정보

    @Column(name = "user_nickname", nullable = false, length = 50)
    private String userNickname;  // 회원의 닉네임 정보

    @Column(name = "user_deleted_at")
    private LocalDateTime userDeletedAt;  // 회원의 탈퇴일 정보 (기본값: NULL)

    @Column(name = "user_game_money", nullable = false, columnDefinition = "int default 0")
    private int userGameMoney;  // 회원의 게임머니 정보 (DB 기본값 0)

    @Column(name = "user_level", nullable = false, columnDefinition = "int default 1")
    private int userLevel;  // 회원의 레벨 정보 (DB 기본값 1)

    @Column(name = "user_attendance")
    private LocalDateTime userAttendance;  // 회원의 출석일 정보 (LocalDateTime으로 수정)

    @Column(name = "user_last_login")
    private LocalDateTime userLastLogin;  // 회원의 마지막 로그인 날짜 정보

    @Column(name = "user_status", length = 20, columnDefinition = "varchar(20) default 'ON'")
    private String userStatus;  // 회원의 유저 상태 (기본값: "ON", 가능 값: "ON", "OFF", "BAN")

    @Column(name = "user_profile_image", length = 500)
    private String userProfileImage;  // 프로필 이미지 (선택적)

    @Column(name = "user_chosen_character", length = 50, columnDefinition = "varchar(50) default 'defaultCharacter'")
    private String userChosenCharacter;  // 선택된 캐릭터 (기본값: "defaultCharacter")

    @Column(name = "user_is_online", nullable = false, columnDefinition = "boolean default false")
    private boolean userIsOnline;  // 회원의 접속 여부 (기본값: false)

    @OneToMany(mappedBy = "user", cascade = CascadeType.ALL)
    private List<FriendShipEntity> friendshipList = new ArrayList<>();

    // DTO -> Entity 변환 메소드
    public static UserEntity toSaveEntity(UserDTO userDTO) {
        UserEntity userEntity = new UserEntity();
        userEntity.setUserPassword(userDTO.getUserPassword());  // 비밀번호 설정
        userEntity.setUserEmail(userDTO.getUserEmail());  // 이메일 설정
        userEntity.setUserNickname(userDTO.getUserNickname());  // 닉네임 설정
        userEntity.setUserProfileImage(userDTO.getUserProfileImage());  // 프로필 이미지 설정 (선택적)

        // 나머지 필드들은 DB 기본값에 의해 자동으로 설정되므로 따로 설정하지 않음
        return userEntity;
    }
}

