package com.dropzone.matchstatistics.dto;

import io.swagger.annotations.ApiModelProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;

import java.util.List;

@Getter
@AllArgsConstructor
public class MatchAllUserDTO {

    @ApiModelProperty(hidden = true)
    private int matchId;  // 매치 ID

    private List<UserMatchResponseDTO> userRecords;  // 매치에 참여한 유저들의 기록 리스트
}
