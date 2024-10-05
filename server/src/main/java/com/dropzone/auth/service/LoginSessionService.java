package com.dropzone.auth.session;

public interface LoginSessionService {
    void saveLoginSession(int userId, String token);  // 로그인 세션 저장
    void removeLoginSession(int userId);  // 로그인 세션 삭제
    boolean isUserLoggedIn(int userId, String token);  // 중복 로그인 여부 확인
}
