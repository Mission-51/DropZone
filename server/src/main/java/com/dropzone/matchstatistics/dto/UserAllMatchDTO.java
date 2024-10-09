package com.dropzone.matchstatistics.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;

import java.util.List;

@Getter
@AllArgsConstructor
public class UserAllMatchDTO {
    private int userId;  // 사용자 ID
    private List<UserMatchResponseDTO> matches;  // 여러 매치 기록 리스트
}
