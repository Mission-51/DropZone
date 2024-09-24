package com.dropzone.friend.dto;

import lombok.*;

@Getter
@Builder
@ToString
@NoArgsConstructor
@AllArgsConstructor
public class FriendListDto {
    private Long friendShipId;
    private String friendEmail;
    private String friendNickname;
}
