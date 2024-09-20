package com.dropzone.statistics.service;

import com.dropzone.statistics.dto.MatchAllUserDTO;
import com.dropzone.statistics.dto.UserAllMatchDTO;
import com.dropzone.statistics.dto.UserMatchDTO;

public interface UserMatchStatisticsService {
    // 1. 특정 유저의 특정 매치 기록 조회
    UserMatchDTO getUserMatchStatistics(int userId, int matchId);

    // 2. 특정 유저의 모든 매치 기록 조회
    UserAllMatchDTO getUserAllMatchStatistics(int userId);

    // 3. 툭종 매치의 모든 유저 기록 조회
    MatchAllUserDTO getMatchAllUserStatistics(int matchId);
}
