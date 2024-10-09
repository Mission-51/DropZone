package com.dropzone.matchstatistics.service;

import com.dropzone.matchstatistics.dto.*;

import java.util.List;

public interface UserMatchStatisticsService {
    // 1. 특정 유저의 특정 매치 기록 조회
    UserMatchResponseDTO getUserMatchStatistics(int userId, int matchId);

    // 2. 특정 유저의 모든 매치 기록 조회
    UserAllMatchDTO getUserAllMatchStatistics(int userId);

    // 3. 툭종 매치의 모든 유저 기록 조회
    MatchAllUserDTO getMatchAllUserStatistics(int matchId);

    // 4. 매치 기록을 DB에 저장 + userStatistics 테이블에 유저 통계를 기록
    void saveMatchRecords(List<MatchAllUserResponseDTO> matchRecords);
}
