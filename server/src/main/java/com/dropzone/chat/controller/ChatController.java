package com.dropzone.chat.controller;

import com.dropzone.chat.dto.ChatMessageDto;
import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.service.ChatService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.security.Principal;
import java.time.LocalDateTime;
import java.util.Set;


@Controller
@RequiredArgsConstructor
public class ChatController {

    @Autowired
    private ChatService chatService;

    private final SimpMessageSendingOperations messagingTemplate;

    // 로비에 채팅 메시지 전송 처리 - 로비 채팅방에 입장할 때에는 메시지에 oo유저가 입장하였습니다. 라고 메시지를 담아서 보내면 됨.
    @MessageMapping("/chat/lobby-message")
    public void sendDefaultMessage(ChatMessageDto chatMessageDto) {
        String defaultRoom = "/sub/room/default-room";
        messagingTemplate.convertAndSend(defaultRoom, chatMessageDto);
    }

    // 개인 채팅 메시지 전송 처리
    @MessageMapping("/chat/private-message")
    public void sendPrivateMessage(ChatMessageDto chatMessageDto) {
        String roomId = chatMessageDto.getRoomId();
        messagingTemplate.convertAndSend("/sub/room/" + roomId, chatMessageDto);
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
