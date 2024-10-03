package com.dropzone.chat.service;


import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.repository.ChatRepository;
import lombok.Getter;
import lombok.RequiredArgsConstructor;
import lombok.Setter;
import org.springframework.stereotype.Service;

import java.time.LocalDateTime;
import java.util.List;

@Getter
@Setter
@RequiredArgsConstructor
@Service
public class ChatService {

    private final ChatRepository chatRepository;

    // 채팅 메시지 저장
    public ChatMessage saveMessage(String roomId, String sender, String message) {

        ChatMessage chatMessage = ChatMessage.builder()
                .chatRoomId(roomId)
                .sender(sender)
                .message(message)
                .timeStamp(LocalDateTime.now())
                .build();

        return chatRepository.save(chatMessage);
    }

    // 개인 채팅방 이전 기록 조회
    public List<ChatMessage> getChatMessage(String roomId) {
            return chatRepository.findAllByChatRoomId(roomId);
    }

}
