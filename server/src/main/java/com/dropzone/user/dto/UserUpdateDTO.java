package com.dropzone.user.dto;

import com.dropzone.user.entity.UserEntity;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserUpdateDTO {

    @Schema(description = "사용자의 비밀번호", example = "password123")
    private String userPassword;

    @Schema(description = "사용자의 닉네임", example = "newNickname")
    private String userNickname;

    @Schema(description = "사용자의 프로필 이미지 URL", example = "https://example.com/profile.jpg")
    private String userProfileImage;

}
