package com.dropzone.declaration.dto;

import lombok.Getter;
import lombok.Setter;

import java.time.LocalDateTime;

@Getter
@Setter
public class DeclarationChatDto {
    private Long id;

    private Long userId;
    private String chatContent;
    private LocalDateTime chatDate;


}
