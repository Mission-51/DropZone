package com.dropzone.chat.entity;

import lombok.Getter;
import lombok.Setter;
import org.springframework.data.mongodb.core.mapping.Document;
import java.time.LocalDateTime;

@Getter
@Setter
@Document(collection = "chatMessage")
public class ChatMessage {

    private String chatRoomId;
    private String sender;
    private String message;
    private LocalDateTime timeStamp;

}
