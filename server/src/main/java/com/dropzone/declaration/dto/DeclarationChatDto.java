package com.dropzone.declaration.dto;

import lombok.Getter;
import lombok.Setter;
import java.time.OffsetDateTime;

@Getter
@Setter
public class DeclarationChatDto {
    private Long userId;
    private String chatContent;
    private OffsetDateTime chatDate;
}
