package com.dropzone.ranking.dto;

import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class UserRankDTO {
    private int userId;
    private int rankingPoints;
    private int totalWins;
    private int rank; // 순위 필드

    public UserRankDTO(int userId, int rankingPoints, int totalWins, int rank) {
        this.userId = userId;
        this.rankingPoints = rankingPoints;
        this.totalWins = totalWins;
        this.rank = rank;
    }
}
