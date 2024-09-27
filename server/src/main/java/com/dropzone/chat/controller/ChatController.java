package com.dropzone.chat.controller;

import com.dropzone.chat.dto.ChatMessageDto;
import com.dropzone.chat.dto.MessageType;
import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.service.ChatService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;

import java.time.LocalDateTime;
import java.util.Set;


@Controller
@RequiredArgsConstructor
@Tag(name = "채팅 API", description = "개인 채팅 / 로비 채팅 / 개인 채팅 저장 / 개인 채팅 조회")
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
    @PostMapping("/api/chat/message")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "개인 채팅 메시지 저장", description = "채팅을 전송하자마자 레디스에 저장")
    public void saveMessage(ChatMessageDto chatMessageDto) {
        String chatRoomId = chatMessageDto.getRoomId();
        MessageType messageType = chatMessageDto.getType();
        String sender = chatMessageDto.getSender();
        String message = chatMessageDto.getMessage();

        ChatMessage chatMessage = new ChatMessage(chatRoomId, sender, messageType, message, LocalDateTime.now());
        chatService.saveMessage(chatMessage);
    }

    // 특정 채팅방의 채팅 기록 조회 (시간 순서대로)
    @GetMapping("/api/chat/messages")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "채팅방 기록 조회", description = "특정 채팅방의 채팅 기록을 조회")
    public Set<ChatMessage> getMessages(
            @RequestParam("roomId") String roomId) {
        return chatService.getChatMessage(roomId);
    }

}
