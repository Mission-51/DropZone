package com.dropzone.chat.manager;

import com.dropzone.chat.domain.ChatRoom;
import jakarta.annotation.PostConstruct;
import lombok.Getter;
import org.springframework.stereotype.Component;

import java.util.HashMap;
import java.util.Map;

@Getter
@Component
public class ChatRoomManager {
    private final Map<String, ChatRoom> chatRooms = new HashMap<>();

    // 서버 시작 시 기본 채팅방을 생성
    @PostConstruct
    public void init() {
        createRoom("default-room");
    }


    public void createRoom(String roomId) {
        ChatRoom chatRoom = new ChatRoom(roomId);
        chatRooms.put(roomId, chatRoom);
    }

    public ChatRoom getRoom(String roomId) {
        return chatRooms.get(roomId);
    }
}
