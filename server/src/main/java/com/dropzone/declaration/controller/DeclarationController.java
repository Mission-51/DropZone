package com.dropzone.declaration.controller;

import com.dropzone.declaration.dto.DeclarationChatDto;
import com.dropzone.declaration.entity.DeclarationChat;
import com.dropzone.declaration.service.DeclarationService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.persistence.EntityNotFoundException;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;


@Slf4j
@CrossOrigin("*")
@RequiredArgsConstructor
@Tag(name = "채팅 신고 API", description = " 특정 유저 신고 / 특정 유저 신고 채팅 조회 / 모든 유저의 신고 채팅 조회 ")
@RestController
public class DeclarationController {

    private final DeclarationService declarationService;

    // 신고된 채팅 저장
    @PostMapping("/api/declaration/save")
    @Operation(summary = "신고된 채팅 저장 API", description = "다른 유저에 의해 신고된 채팅을 저장하는 로직")
    public ResponseEntity<DeclarationChat> saveDeclarationChat(DeclarationChatDto declarationChatDto) {
        return ResponseEntity.ok(declarationService.saveDeclarationChat(declarationChatDto));
    }  
    
    // 모든 유저의 신고된 채팅 조회
    @GetMapping("/api/declaration/all")
    @Operation(summary = "모든 유저의 신고된 채팅 조회 API", description = "특정 유저의 신고된 채팅을 조회하는 로직")
    public ResponseEntity<?> getAllDeclarationChat() {
        return ResponseEntity.ok(declarationService.getAllDeclarationChat());
    }
    
    // 특정 유저의 신고된 채팅 조회
    @GetMapping("/api/declaration/{user_id}")
    @Operation(summary = "특정 유저의 신고된 채팅 조회 API", description = "특정 유저의 신고된 채팅을 조회하는 로직")
    public ResponseEntity<List<DeclarationChat>> getUserDeclarationChat(@PathVariable("user_id") Long userId) {
        return ResponseEntity.ok(declarationService.getUserDeclarationChat(userId));
    }

}
