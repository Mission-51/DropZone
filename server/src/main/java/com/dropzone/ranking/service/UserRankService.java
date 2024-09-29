package com.dropzone.ranking.service;

import com.dropzone.ranking.dto.UserRankDTO;

import java.util.List;
import java.util.Optional;

public interface UserRankService {
    // 페이지 번호만을 받아 처리하는 메서드
    List<UserRankDTO> getRankings(Optional<Integer> page);
}
