package com.dropzone.statistics.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Table(name = "user_match_statistics")
public class UserMatchStatisticsEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "character_id")
    private int characterId;

    @Column(name = "match_rank")
    private int matchRank;

    @Column(name = "match_dps")
    private int matchDps;

    @Column(name = "match_kills")
    private int matchKills;

    @Column(name = "match_playtime")
    private int matchPlaytime;
}
