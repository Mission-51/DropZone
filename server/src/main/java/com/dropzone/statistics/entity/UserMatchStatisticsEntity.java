package com.dropzone.statistics.entity;

import jakarta.persistence.*;
import lombok.Getter;

import java.sql.Time;


@Getter
@Entity
@Table(name = "user_match_statistics")
public class UserMatchStatisticsEntity {
    // PK
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "user_id")
    private int userId;

    @Column(name = "character_id")
    private int characterId;

    // FK: 부모 엔티티 MatchEntity를 참조하는 필드
    @ManyToOne
    @JoinColumn(name = "match_id") // 외래 키 컬럼
    private MatchEntity match; // 부모 엔티티인 MatchEntity와의 관계를 가리킴

    @Column(name = "match_rank")
    private int matchRank;

    @Column(name = "match_dps")
    private int matchDps;

    @Column(name = "match_kills")
    private int matchKills;

    @Column(name = "match_playtime")
    private Time matchPlaytime;
}
