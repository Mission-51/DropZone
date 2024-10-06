package com.dropzone.user.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserUpdatePasswordDTO {
    @Schema(description = "사용자의 비밀번호", example = "newPassword")
    private String userPassword;
}
