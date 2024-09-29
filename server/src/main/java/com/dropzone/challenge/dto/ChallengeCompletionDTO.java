package com.dropzone.challenge.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class ChallengeCompletionDTO {
    private String challengeContent;
    private String challengeField; // 도전 과제 필드
    private int currentValue; // 유저 통계의 현재 값
    private int targetValue;  // 도전 과제 목표 값
    private boolean isCompleted;  // 도전 과제 달성 여부

    public ChallengeCompletionDTO(String challengeContent, String challengeField, int currentValue, int targetValue, boolean isCompleted) {
        this.challengeContent = challengeContent;
        this.challengeField = challengeField;
        this.currentValue = currentValue;
        this.targetValue = targetValue;
        this.isCompleted = isCompleted;
    }
}
