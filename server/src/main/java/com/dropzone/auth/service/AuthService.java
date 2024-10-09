package com.dropzone.auth.service;

import com.dropzone.user.dto.UserDTO;

public interface AuthService {
    // 이메일과 비밀번호를 사용하여 로그인을 처리하는 메서드
    public UserDTO loginByEmail(String email, String password);

//    // 로그아웃
//    public void logout(String userId);
}
