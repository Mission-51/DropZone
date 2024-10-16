package com.dropzone.user.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.*;

@Getter
@Setter
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class UserUpdateNickNameDTO {
    @Schema(description = "사용자의 닉네임", example = "newNickname")
    private String userNickname;

}
