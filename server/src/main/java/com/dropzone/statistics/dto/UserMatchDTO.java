package com.dropzone.statistics.dto;


import lombok.AllArgsConstructor;
import lombok.Getter;

import java.sql.Time;

@Getter
@AllArgsConstructor
public class UserMatchDTO {
    private int userId;
    private int character_id;
    private int match_id;
    private int match_rank;
    private int match_dps;
    private int match_kills;
    private Time match_playtime;
}
