package com.dropzone.matchstatistics.dto;

import io.swagger.annotations.ApiModelProperty;
import lombok.AllArgsConstructor;
import lombok.Getter;

import java.util.List;

@Getter
@AllArgsConstructor
public class MatchAllUserResponseDTO {

    @ApiModelProperty(hidden = true)
    private int matchId;  // 매치 ID

    private List<UserMatchDTO> userRecords;
}
