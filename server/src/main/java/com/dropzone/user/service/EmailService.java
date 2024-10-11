package com.dropzone.user.service;

import java.util.Map;


public interface EmailService {
    // 1. 이메일로 인증 코드를 발송하는 메소드
    public void sendAuthenticationCodeEmail(String to);

    // 2. 이메일과 인증 코드를 비교해 인증하는 메소드
    public boolean authenticateEmail(String email, String authenticationCode);
}
