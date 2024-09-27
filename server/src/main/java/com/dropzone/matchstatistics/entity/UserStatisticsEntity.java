package com.dropzone.matchstatistics.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Entity;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

import java.sql.Time;

@Entity
@Table(name = "user_statistics")
@NoArgsConstructor
@Getter
@Setter
public class UserStatisticsEntity {

    @Id
    @Column(name = "user_id")
    private int userId;

    @Column(name = "ranking_points", nullable = false)
    private int rankingPoints = 100;

    @Column(name = "total_kills", nullable = false)
    private int totalKills = 0;

    @Column(name = "total_damage", nullable = false)
    private int totalDamage = 0;

    @Column(name = "total_playtime", nullable = false)
    private Time totalPlaytime = Time.valueOf("00:00:00");

    @Column(name = "total_games", nullable = false)
    private int totalGames = 0;

    @Column(name = "total_wins", nullable = false)
    private int totalWins = 0;
}
