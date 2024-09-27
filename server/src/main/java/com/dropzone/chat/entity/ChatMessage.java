package com.dropzone.chat.entity;

import com.dropzone.chat.dto.MessageType;
import com.dropzone.chat.service.ChatService;
import com.fasterxml.jackson.annotation.JsonCreator;
import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.databind.annotation.JsonDeserialize;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;
import com.fasterxml.jackson.datatype.jsr310.deser.LocalDateDeserializer;
import com.fasterxml.jackson.datatype.jsr310.deser.LocalDateTimeDeserializer;
import com.fasterxml.jackson.datatype.jsr310.ser.LocalDateSerializer;
import com.fasterxml.jackson.datatype.jsr310.ser.LocalDateTimeSerializer;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;

import java.io.Serializable;
import java.time.LocalDateTime;

@Getter
@Setter
public class ChatMessage implements Serializable {

    private String chatRoomId;
    private String sender;
    private MessageType messageType;
    private String message;
    @JsonDeserialize(using = LocalDateTimeDeserializer.class)
    @JsonSerialize(using = LocalDateTimeSerializer.class)
    private final LocalDateTime timeStamp;

    @JsonCreator
    public ChatMessage(@JsonProperty("chatRoomId") String chatRoomId,
                       @JsonProperty("sender") String sender,
                       @JsonProperty("messageType") MessageType messageType,
                       @JsonProperty("message") String message,
                       @JsonProperty("timeStamp") LocalDateTime timeStamp) {
            this.chatRoomId = chatRoomId;
            this.sender = sender;
            this.messageType = messageType;
            this.message = message;
            this.timeStamp = timeStamp;
    }

}
