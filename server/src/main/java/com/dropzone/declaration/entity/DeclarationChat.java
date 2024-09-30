package com.dropzone.declaration.entity;

import com.dropzone.declaration.dto.DeclarationChatDto;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.bson.types.ObjectId;
import org.springframework.data.mongodb.core.mapping.Document;

import java.time.LocalDateTime;

@Getter
@Setter
@Document(collection = "declarationMessage")
public class DeclarationChat {

    @Id
    private ObjectId id;

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
        declarationChat.setChatContent(declarationChatDto.getChatContent());
        declarationChat.setChatDate(declarationChatDto.getChatDate().toLocalDateTime());
        declarationChat.setReportDate(LocalDateTime.now());

        return declarationChat;
    }
}
