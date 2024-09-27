package com.dropzone.challenge.entity;

import jakarta.persistence.*;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;

@Getter
@Setter
@Entity
@NoArgsConstructor
@Table(name = "challenge")
public class ChallengeEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int challengeId;

    @Column(name = "challenge_content", nullable = false)
    private String challengeContent;

    // 유저 통계 테이블에서 비교할 필드
    @Column(name = "challenge_field", nullable = false)
    private String challengeField;

    // 비교 연산자 ( >, <, == 등)
    @Column(name = "challenge_operator", nullable = false)
    private String challengeOperator;

    // 비교할 값
    @Column(name = "challenge_value", nullable = false)
    private int challengeValue;

    // 도전과제 달성여부
    @Column(name = "challenge_is_achieved", nullable = false)
    private boolean challengeIsAchieved = false;

    // 생성자 추가 (필드값들을 받는 생성자)
    public ChallengeEntity(String challengeContent, String challengeField, String challengeOperator, int challengeValue) {
        this.challengeContent = challengeContent;
        this.challengeField = challengeField;
        this.challengeOperator = challengeOperator;
        this.challengeValue = challengeValue;
    }
}
