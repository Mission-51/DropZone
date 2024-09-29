package com.dropzone.chat.repository;

import com.dropzone.chat.entity.ChatMessage;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ChatRepository extends MongoRepository<ChatMessage, String> {

    List<ChatMessage> findAllByChatRoomId(String roomId);
}
