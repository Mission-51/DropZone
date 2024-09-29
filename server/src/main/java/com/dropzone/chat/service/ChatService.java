package com.dropzone.chat.service;


import com.dropzone.chat.entity.ChatMessage;
import com.dropzone.chat.repository.ChatRepository;
import lombok.Getter;
import lombok.Setter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Getter
@Setter
@Service
public class ChatService {

    @Autowired
    private ChatRepository chatRepository;


    // 채팅 메시지 저장
    public ChatMessage saveMessage(ChatMessage chatMessage) {
        return chatRepository.save(chatMessage);
    }

    // 개인 채팅방 이전 기록 조회
    public List<ChatMessage> getChatMessage(String roomId) {
            return chatRepository.findAllByChatRoomId(roomId);
    }

}
