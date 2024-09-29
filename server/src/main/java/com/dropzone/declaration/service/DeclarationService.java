package com.dropzone.declaration.service;

import com.dropzone.declaration.dto.DeclarationChatDto;
import com.dropzone.declaration.entity.DeclarationChat;
import com.dropzone.declaration.repository.DeclarationRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class DeclarationService {

    @Autowired
    private DeclarationRepository declarationRepository;


    // 신고된 채팅 저장하는 로직
    public DeclarationChat saveDeclarationChat(DeclarationChatDto declarationChatDto) {
        DeclarationChat declarationChat = DeclarationChat.toDeclarationChatEntity(declarationChatDto);
        return declarationRepository.save(declarationChat);
    }
    
    // 모든 신고된 채팅을 조회하는 로직
    public List<DeclarationChat> getAllDeclarationChat() {
        return declarationRepository.findAll();
    }
    
    // 특정 유저의 신고된 채팅을 조회하는 로직
    public List<DeclarationChat> getUserDeclarationChat(Long userId) {
        return declarationRepository.findAllByUserId(userId);
    }


}
