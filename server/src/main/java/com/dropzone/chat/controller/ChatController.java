package com.dropzone.chat.controller;

import com.dropzone.chat.dto.ChatMessageDto;
import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.service.ChatService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.simp.SimpMessageSendingOperations;
import org.springframework.web.bind.annotation.*;
import java.util.List;


@RequiredArgsConstructor
@Tag(name = "채팅 API", description = "채팅 메시지 저장 / 특정 채팅방 이전 기록 조회")
@RestController
public class ChatController {

    private final ChatService chatService;
    private final SimpMessageSendingOperations messagingTemplate;

    // 메시지 전송 처리 - 로비 채팅방에 입장할 때에는 메시지에 oo유저가 입장하였습니다. 라고 메시지를 담아서 보내면 됨.
    @MessageMapping("/chat/message")
    public void sendPrivateMessage(ChatMessageDto chatMessageDto) {
        String roomId = chatMessageDto.getRoomId();
        messagingTemplate.convertAndSend("/sub/room/" + roomId, chatMessageDto);
    }

    // 채팅 메시지 저장
    @Operation(summary = "채팅 메시지 저장 API", description = "채팅 메시지 저장")
    @PostMapping("/api/chat/message")
    public ChatMessage saveMessage(
            @Parameter(description = "roomId", required = true)
            @RequestParam("roomId") String roomId,
            @Parameter(description = "sender", required = true)
            @RequestParam("sender") String sender,
            @Parameter(description = "message", required = true)
            @RequestParam("message") String message
              ) {
        return chatService.saveMessage(roomId, sender, message);
    }

    // 개인 채팅방의 채팅 기록 조회
    @Operation(summary = "개인 채팅방 이전 메시지 조회 API", description = "개인 채팅방에 저장된 이전 채팅 메시지들을 조회")
    @GetMapping("/api/chat/messages")
    public List<ChatMessage> getMessages(
            @Parameter(description = "roomId", required = true)
            @RequestParam("roomId") String roomId) {

        return chatService.getChatMessage(roomId);
    }
}
