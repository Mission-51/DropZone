package com.dropzone.friend.dto;

import com.dropzone.friend.entity.FriendShipStatus;
import lombok.*;

@Getter
@Builder
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class WaitingFriendListDto {
    private Long friendShipId;
    private String friendEmail;
    private String friendNickname;
    private FriendShipStatus status;
}
