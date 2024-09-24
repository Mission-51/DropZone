package com.dropzone.statistics.repository;

import com.dropzone.statistics.entity.MatchInfoEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.Optional;

public interface MatchInfoRepository extends JpaRepository<MatchInfoEntity, Integer> {
    // 기본적인 CRUD 메서드가 자동으로 제공됨
    Optional<MatchInfoEntity> findByMatchId(int id);
}
