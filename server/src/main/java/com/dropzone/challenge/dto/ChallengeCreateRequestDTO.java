package com.dropzone.challenge.dto;

import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@NoArgsConstructor
public class ChallengeCreateRequestDTO {
    private String challengeContent; // 도전 과제 내용
    private String challengeField; // 도전 과제 조건 필드
    private String challengeOperator; // 비교 연산자
    private int challengeValue; // 비교할 값
}
