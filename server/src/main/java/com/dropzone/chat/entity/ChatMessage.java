package com.dropzone.chat.entity;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

import java.io.Serializable;
import java.time.LocalDateTime;

@Getter
@RequiredArgsConstructor
public class ChatMessage implements Serializable {

    private final String chatRoomId;
    private final String sender;
    private final String message;
    private final LocalDateTime timeStamp;

}
