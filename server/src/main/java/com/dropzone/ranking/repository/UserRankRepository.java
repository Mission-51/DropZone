package com.dropzone.ranking.repository;

import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface UserRankRepository extends JpaRepository<UserStatisticsEntity, Integer> {

    // 전체 유저의 랭킹 정보를 페이지네이션 방식으로 가져옴
    @Query("SELECT u FROM UserStatisticsEntity u ORDER BY u.rankingPoints DESC, u.totalWins DESC")
    Page<UserStatisticsEntity> findAllRankings(Pageable pageable);

    // 유저의 랭킹을 계산하는 쿼리
    @Query("SELECT COUNT(u) + 1 FROM UserStatisticsEntity u WHERE (u.rankingPoints > :rankingPoints OR (u.rankingPoints = :rankingPoints AND u.totalWins > :totalWins))")
    int findUserRank(int rankingPoints, int totalWins);

    @Query("SELECT u FROM UserStatisticsEntity u WHERE u.userId = :userId")
    UserStatisticsEntity findUserStatisticsByUserId(int userId);

}