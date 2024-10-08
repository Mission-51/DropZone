package com.dropzone.friend.dto;

import com.dropzone.user.entity.UserEntity;
import lombok.*;

@Getter
@Builder
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class FriendListDto {
    private int fiendShipId;
    private int friendId;
    private String friendEmail;
    private String friendNickName;
}
