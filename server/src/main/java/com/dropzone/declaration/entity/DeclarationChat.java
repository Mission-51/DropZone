package com.dropzone.declaration.entity;

import com.dropzone.declaration.dto.DeclarationChatDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
@Setter
@Entity
public class DeclarationChat {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "id", updatable = false)
    private Long id;

    @Column(name = "user_id", nullable = false)
    private Long userId;

    @Column(name = "chat_content", nullable = false)
    private String chatContent;

    @Column(name = "chat_date",nullable = false)
    private LocalDateTime chatDate;

    @Column(name = "report_date", nullable = false)
    private LocalDateTime reportDate;


    // Dto -> Entity로 전환
    public static DeclarationChat toDeclarationChatEntity(DeclarationChatDto declarationChatDto) {
        DeclarationChat declarationChat = new DeclarationChat();
        declarationChat.setUserId(declarationChatDto.getUserId());
        declarationChat.setChatContent(declarationChat.getChatContent());
        declarationChat.setChatDate(declarationChatDto.getChatDate());
        declarationChat.setReportDate(LocalDateTime.now());

        return declarationChat;
    }
}
