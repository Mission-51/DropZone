package com.dropzone.matchstatistics.dto;

import io.swagger.annotations.ApiModelProperty;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Getter;

import java.sql.Time;

@Getter
@Builder
@AllArgsConstructor
public class UserMatchResponseDTO {

    @ApiModelProperty(value = "회원 ID")
    private int userId;

    @ApiModelProperty(value = "캐릭터 ID")
    private int character_id;

    @ApiModelProperty(value = "매치 ID", hidden = true)
    private int match_id;

    @ApiModelProperty(value = "매치 등수")
    private int match_rank;

    @ApiModelProperty(value = "매치에서 입힌 데미지")
    private int match_dps;

    @ApiModelProperty(value = "매치에서 기록한 킬 수")
    private int match_kills;

    @ApiModelProperty(value = "매치 플레이 시간")
    private Time match_playtime;
}
