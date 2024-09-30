package com.dropzone.chat.entity;

import lombok.*;
import org.springframework.data.mongodb.core.mapping.Document;
import java.time.LocalDateTime;

@Getter
@Builder
@AllArgsConstructor
@NoArgsConstructor
@Document(collection = "chatMessage")
public class ChatMessage {

    private String chatRoomId;
    private String sender;
    private String message;
    private LocalDateTime timeStamp;

}
