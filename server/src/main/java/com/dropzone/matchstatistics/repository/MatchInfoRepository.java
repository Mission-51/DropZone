package com.dropzone.matchstatistics.repository;

import com.dropzone.matchstatistics.entity.MatchInfoEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface MatchInfoRepository extends JpaRepository<MatchInfoEntity, Integer> {
    // 기본적인 CRUD 메서드가 자동으로 제공됨
    Optional<MatchInfoEntity> findByMatchId(int id);
}
