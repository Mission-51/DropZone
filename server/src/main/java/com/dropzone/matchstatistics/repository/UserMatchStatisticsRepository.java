package com.dropzone.matchstatistics.repository;

import com.dropzone.matchstatistics.entity.UserMatchStatisticsEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;
import java.util.Optional;

public interface UserMatchStatisticsRepository extends JpaRepository<UserMatchStatisticsEntity, Integer> {
    // 특정 회원의 특정 매치 기록을 조회하는 메서드
    Optional<UserMatchStatisticsEntity> findByUserIdAndMatch_MatchId(int userId, int matchId);

    // 특정 회원의 모든 매치 기록을 조회하는 메서드
    List<UserMatchStatisticsEntity> findByUserId(int userId);

    // 특정 매치의 모든 회원 기록을 조회하는 메서드
    List<UserMatchStatisticsEntity> findByMatch_MatchId(int matchId);
}