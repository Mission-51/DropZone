//package com.dropzone.auth.repository;
//
//import com.dropzone.auth.session.LoginSession;
//import org.springframework.data.mongodb.repository.MongoRepository;
//import java.util.Optional;
//
//public interface LoginSessionRepository extends MongoRepository<LoginSession, String> {
//
//    // 특정 userId로 로그인 세션을 찾는 메서드
//    Optional<LoginSession> findByUserId(int userId);
//
//    // 특정 userId로 로그인 세션을 삭제하는 메서드
//    void deleteByUserId(int userId);
//}
