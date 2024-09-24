package com.dropzone.matchstatistics.repository;

import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface UserStatisticsRepository extends JpaRepository<UserStatisticsEntity, Integer> {
    // 유저 통계 확인 페이지: user_id를 통한 유저 통계 조회
    Optional<UserStatisticsEntity> findByUserId(int userId);
}