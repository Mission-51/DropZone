package com.dropzone.statistics.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;
import org.hibernate.annotations.CreationTimestamp;

import java.time.LocalDateTime;
import java.util.List;

@Getter
@Entity
@Table(name = "match")
public class MatchEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int matchId;

    @CreationTimestamp
    @Column(name = "match_created_at", updatable = false)
    private LocalDateTime matchCreatedAt;

    // 자식 엔티티인 UserMatchStatisticsEntity와 일대다 관계 설정
    @OneToMany(mappedBy = "match_id")
    private List<UserMatchStatisticsEntity> userMatchStatistics;
}
