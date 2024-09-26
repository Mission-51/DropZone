package com.dropzone.challenge.repository;

import com.dropzone.challenge.entity.ChallengeEntity;
import org.springframework.data.jpa.repository.JpaRepository;

public interface ChallengeRepository extends JpaRepository<ChallengeEntity, Integer> {
}
