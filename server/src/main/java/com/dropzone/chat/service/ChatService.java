package com.dropzone.chat.service;


import com.dropzone.chat.entity.ChatMessage;
import lombok.Getter;
import lombok.Setter;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.redis.core.RedisTemplate;
import org.springframework.stereotype.Service;

import java.time.ZoneOffset;
import java.util.Set;

@Getter
@Setter
@Service
public class ChatService {

    @Autowired
    private RedisTemplate<String, ChatMessage> redisTemplate;

    // 채팅 메시지를 시간순으로 저장
    public void saveMessage(ChatMessage chatMessage) {
        // 타임스탬프를 score로 설정
        double score = chatMessage.getTimeStamp().toEpochSecond(ZoneOffset.ofHours(9));

        // Sorted Set에 메시지 저장
        redisTemplate.opsForZSet().add(chatMessage.getChatRoomId(), chatMessage, score);

    }

    // 특정 채팅방의 채팅 기록 조회 (시간 순서대로)
    public Set<ChatMessage> getChatMessage(String chatRoomId) {
        return (Set<ChatMessage>) redisTemplate.opsForZSet().range(chatRoomId, 0, -1);
    }

}
