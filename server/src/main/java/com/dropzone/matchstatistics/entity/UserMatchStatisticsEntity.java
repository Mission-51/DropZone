package com.dropzone.matchstatistics.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

import java.sql.Time;


@Getter
@Setter
@Entity
@Table(name = "user_match_statistics")
@IdClass(UserMatchStatisticsId.class) // 복합 키 클래스 적용
public class UserMatchStatisticsEntity {
    // PK
    @Id
    @Column(name = "user_id")
    private int userId;

    // FK: 부모 엔티티 MatchEntity를 참조하는 필드
    @Id
    @ManyToOne
    @JoinColumn(name = "match_id") // 외래 키 컬럼
    private MatchInfoEntity match; // 부모 엔티티인 MatchEntity와의 관계를 가리킴

    @Column(name = "character_id")
    private int characterId;

    @Column(name = "match_rank")
    private int matchRank;

    @Column(name = "match_kills")
    private int matchKills;

    @Column(name = "match_playtime")
    private Time matchPlaytime;
}
