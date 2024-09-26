package com.dropzone.chat.dto;

import lombok.Getter;
import lombok.Setter;


@Getter
@Setter
public class ChatMessageDto {
    private String roomId;
    private MessageType type;
    private String sender;
    private String message;
}
