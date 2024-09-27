package com.dropzone.challenge.service;

import com.dropzone.challenge.dto.ChallengeCompletionDTO;
import com.dropzone.challenge.entity.ChallengeEntity;
import com.dropzone.challenge.repository.ChallengeRepository;
import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import com.dropzone.matchstatistics.repository.UserStatisticsRepository;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.Optional;

@Slf4j
@Service // 스프링이 관리하는 Bean으로 등록
public class ChallengeServiceImpl implements ChallengeService {

    private final ChallengeRepository challengeRepository;
    private final UserStatisticsRepository userStatisticsRepository;

    @Autowired
    public ChallengeServiceImpl(ChallengeRepository challengeRepository, UserStatisticsRepository userStatisticsRepository) {
        this.challengeRepository = challengeRepository;
        this.userStatisticsRepository = userStatisticsRepository;
    }

    // 1. 도전과제 생성 메서드
    @Override
    public ChallengeEntity createChallenge(String challengeContent, String challengeField, String challengeOperator, int challengeValue) {
        ChallengeEntity newChallenge = new ChallengeEntity(challengeContent, challengeField, challengeOperator, challengeValue);
        return challengeRepository.save(newChallenge);
    }

    // 2. 특정 유저의 모든 도전 과제에 대한 달성 여부 확인 메서드
    @Override
    public List<ChallengeCompletionDTO> checkAllChallengesForUser(int userId) {
        // 유저 통계 정보 가져오기
        Optional<UserStatisticsEntity> userStatistics = userStatisticsRepository.findByUserId(userId);

        // 모든 도전 과제를 가져오기
        List<ChallengeEntity> challenges = challengeRepository.findAll();

        // 도전 과제 리스트를 순회하며 달성 여부 확인
        List<ChallengeCompletionDTO> completionDTOs = new ArrayList<>();
        for (ChallengeEntity challenge : challenges) {
            int currentValue = getFieldValue(userStatistics, challenge.getChallengeField());
            boolean isCompleted = checkCondition(currentValue, challenge.getChallengeOperator(), challenge.getChallengeValue());

            // 각 도전 과제의 달성 여부를 DTO로 변환하여 리스트에 추가
            completionDTOs.add(new ChallengeCompletionDTO(
                challenge.getChallengeContent(),
                challenge.getChallengeField(),
                currentValue,
                challenge.getChallengeValue(),
                isCompleted
            ));
        }

        return completionDTOs;
    }

    // 유저 통계 테이블에서 해당 필드의 값을 가져오는 메서드
    private int getFieldValue(Optional<UserStatisticsEntity> userStatistics, String challengeField) {
        UserStatisticsEntity statistics = userStatistics.orElseThrow(() -> new IllegalArgumentException("유저 통계 정보 찾기 실패"));

        switch (challengeField) {
            case "total_kills":
                return statistics.getTotalKills();
            case "total_damage":
                return statistics.getTotalDamage();
            case "total_wins":
                return statistics.getTotalWins();
            default:
                throw new IllegalArgumentException("올바르지 않은 challengeField: " + challengeField);
        }
    }

    // 도전 과제 조건을 확인하는 메서드
    private boolean checkCondition(int currentValue, String challengeOpertor, int challengeValue) {
        switch (challengeOpertor) {
            case ">=":
                return currentValue >= challengeValue;
            case ">":
                return currentValue > challengeValue;
            case "<=":
                return currentValue <= challengeValue;
            case "<":
                return currentValue < challengeValue;
            default:
                throw new IllegalArgumentException("Invalid challengeOpertor: " + challengeOpertor);
        }
    }
}
