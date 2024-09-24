package com.dropzone.statistics.dto;

import lombok.AllArgsConstructor;
import lombok.Getter;

import java.util.List;

@Getter
@AllArgsConstructor
public class MatchAllUserDTO {
    private int matchId;  // 매치 ID
    private List<UserMatchDTO> userRecords;  // 매치에 참여한 유저들의 기록 리스트
}
