package com.dropzone.challenge.service;

import com.dropzone.challenge.dto.ChallengeCompletionDTO;
import com.dropzone.challenge.entity.ChallengeEntity;

import java.util.List;

public interface ChallengeService {
    // 도전 과제 생성 메서드
    ChallengeEntity createChallenge(String challengeContent, String challengeField, String challengeOperator, int challengeValue);

    // 특정 유저의 모든 도전 과제에 대한 달성 여부를 확인하는 메서드
    List<ChallengeCompletionDTO> checkAllChallengesForUser(int userId);
}
