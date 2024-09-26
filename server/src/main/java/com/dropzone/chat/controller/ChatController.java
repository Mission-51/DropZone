package com.dropzone.chat.controller;

import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.service.ChatService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import java.time.LocalDateTime;
import java.util.Set;


@Controller
@RequiredArgsConstructor
public class ChatController {

    @Autowired
    private ChatService chatService;

    private final SimpMessageSendingOperations messagingTemplate;

    // 채팅방 입장 처리
    @MessageMapping("/chat/enter")
    public void enterRoom() {
        // 로비에 접속한 모든 유저를 기본 채팅방으로 입장시킴
        String message = "유저가 로비에 입장했습니다.";
        messagingTemplate.convertAndSend("/sub/room/default-room", message);
    }

    // 로비에 채팅 메시지 전송 처리
    @MessageMapping("/chat/message")
    @SendTo("/sub/room/default-room")
    public String sendDefaultMessage(String message) {
        return message;
    }


    // 개인 채팅 메시지 전송 처리
    @MessageMapping()
    @SendTo()
    public void errr() {

    }


    // 채팅 메시지 저장
    @PostMapping("/api/chat/{roomId}/message")
    public void saveMessage(@PathVariable String roomId, @RequestParam String sender, @RequestParam String message) {
        ChatMessage chatMessage = new ChatMessage(roomId, sender, message, LocalDateTime.now());
        chatService.saveMessage(chatMessage);
    }

    // 특정 채팅방의 채팅 기록 조회 (시간 순서대로)
    @GetMapping("/api/chat/{roomId}/messages")
    public Set<ChatMessage> getMessages(@PathVariable String roomId) {
        return chatService.getChatMessage(roomId);
    }

}
