package com.dropzone.friend.dto;

import com.dropzone.friend.entity.FriendShipStatus;
import lombok.*;

@Getter
@Builder
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class WaitingFriendListDto {
    private int friendShipId;
    private String friendEmail;
    private String friendNickName;
    private FriendShipStatus status;
}
