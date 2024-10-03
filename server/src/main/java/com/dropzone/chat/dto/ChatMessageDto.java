package com.dropzone.chat.dto;

import lombok.Builder;
import lombok.Getter;


@Getter
@Builder
public class ChatMessageDto {
    private String roomId;
    private MessageType type;
    private String sender;
    private String message;
}
