package com.dropzone.chat.manager;

import com.dropzone.chat.domain.ChatRoom;
import lombok.Getter;
import org.springframework.stereotype.Component;

import java.util.HashMap;
import java.util.Map;

@Getter
@Component
public class ChatRoomManager {
    private final Map<String, ChatRoom> chatRooms = new HashMap<>();

    public ChatRoomManager() {
        // 서버 시작 시 하나의 채팅방을 미리 생성
        createRoom("default-room");
    }

    public ChatRoom createRoom(String roomId) {
        ChatRoom chatRoom = new ChatRoom(roomId);
        chatRooms.put(roomId, chatRoom);
        return chatRoom;
    }

    public ChatRoom getRoom(String roomId) {
        return chatRooms.get(roomId);
    }
}
