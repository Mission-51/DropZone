package com.dropzone.user.repository;

import com.dropzone.user.entity.UserEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository  // 스프링에서 해당 인터페이스가 데이터베이스와의 상호작용을 담당하는 저장소임을 나타내는 어노테이션
public interface UserRepository extends JpaRepository<UserEntity, Integer> {

    // 이메일로 사용자 정보 조회하는 메소드
    Optional<UserEntity> findByUserEmail(String userEmail);

    // 닉네임으로 사용자 정보 조회하는 메소드
    Optional<UserEntity> findByUserNickname(String userNickname);

    // 이메일과 암호화된 비밀번호로 사용자 정보를 조회하는 메소드 (로그인 시 사용)
    Optional<UserEntity> findByUserEmailAndUserPassword(String userEmail, String encryptedPassword);
}
