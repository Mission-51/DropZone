package com.dropzone.auth.service;

import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

@Service
public class CustomUserDetailsService implements UserDetailsService {

    // UserService를 통해 이메일로 사용자를 검색하기 위해 의존성을 주입
    @Autowired
    private UserService userService;

    /**
     * 이메일을 기반으로 사용자 정보를 조회하는 메서드
     *
     * @param email 로그인 시 입력된 이메일
     * @return UserDetails 객체 (Spring Security에서 사용됨)
     * @throws UsernameNotFoundException 사용자가 존재하지 않을 경우 발생
     */
    @Override
    public UserDetails loadUserByUsername(String email) throws UsernameNotFoundException {
        // 이메일을 사용해 사용자를 검색
        UserDTO user = userService.searchByEmail(email);

        // 사용자가 존재하지 않을 경우 예외 처리
        if (user == null) {
            throw new UsernameNotFoundException("User not found with email: " + email);
        }

        // 사용자가 이미 로그인된 상태라면 새로운 로그인 시도 방지
        if (user.isUserIsOnline()) {
            return null;
        }

        // 사용자의 이메일, 비밀번호, 권한을 포함한 UserDetails 객체를 반환
        return org.springframework.security.core.userdetails.User.withUsername(user.getUserEmail())
                .password(user.getUserPassword())
                .authorities("USER")  // 권한을 "USER"로 설정
                .build();
    }
}
