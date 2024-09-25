package com.dropzone.chat.controller;

import com.dropzone.chat.manager.ChatRoomManager;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.stereotype.Controller;

@RequiredArgsConstructor
@Controller
public class ChatController {

    private final ChatRoomManager chatRoomManager;
    private final SimpMessageSendingOperations messagingTemplate;

    // 채팅방 입장 처리
    @MessageMapping("/enter")
    public void enterRoom(String roomId) {
        // 로비에 접속한 모든 유저를 기본 채팅방으로 입장시킴
        String message = "유저가 채팅방 [" + roomId + "]에 입장했습니다.";
        messagingTemplate.convertAndSend("/sub/room/" + roomId, message);
    }

    // 채팅 메시지 전송 처리
    @MessageMapping("/message")
    @SendTo("/sub/room/default-room")
    public String sendMessage(String message) {
        return message;
    }
}
