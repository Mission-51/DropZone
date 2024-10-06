//package com.dropzone.auth.service;
//
//import com.dropzone.auth.repository.LoginSessionRepository;
//import com.dropzone.auth.session.LoginSession;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.stereotype.Service;
//
//import java.util.Optional;
//
//@Service
//public class LoginSessionServiceImpl implements LoginSessionService {
//
//    private final LoginSessionRepository loginSessionRepository;
//
//    @Autowired
//    public LoginSessionServiceImpl(LoginSessionRepository loginSessionRepository) {
//        this.loginSessionRepository = loginSessionRepository;
//    }
//
//    // 로그인 세션 저장
//    @Override
//    public void saveLoginSession(int userId, String token) {
//        LoginSession loginSession = new LoginSession(userId, token, System.currentTimeMillis(), true);
//        loginSessionRepository.save(loginSession);  // MongoDB에 세션 저장
//    }
//
//    // 로그인 세션 삭제 (로그아웃 시)
//    @Override
//    public void removeLoginSession(int userId) {
//        loginSessionRepository.deleteByUserId(userId);  // MongoDB에서 해당 세션 삭제
//    }
//
//    // 중복 로그인 여부 확인
//    @Override
//    public boolean isUserLoggedIn(int userId, String token) {
//        Optional<LoginSession> session = loginSessionRepository.findByUserId(userId);
//        if (session.isPresent()) {
//            // 세션이 존재하고, 토큰이 일치하면 중복 로그인 상태
//            return !session.get().getToken().equals(token);
//        }
//        // 세션이 없으면 중복 로그인 아님
//        return false;
//    }
//}
