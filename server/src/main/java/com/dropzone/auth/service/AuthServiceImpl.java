package com.dropzone.auth.service;

import com.dropzone.auth.session.LoginSessionService;
import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Optional;

import static com.dropzone.user.dto.UserDTO.toUserDTO;

@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final LoginSessionService loginSessionService;  // 로그인 세션 서비스 추가

    // 이메일과 비밀번호로 로그인하는 메서드
    @Override
    public UserDTO loginByEmail(String email, String password) {

        // 이메일로 사용자 찾기
        Optional<UserEntity> loginUserEntity = userRepository.findByUserEmail(email);

        // 사용자가 존재할 경우, 비밀번호 비교
        if (loginUserEntity.isPresent()) {
            UserEntity userEntity = loginUserEntity.get();

            // 입력된 비밀번호와 암호화된 비밀번호 비교
            if (passwordEncoder.matches(password, userEntity.getUserPassword())) {

                // 중복 로그인 여부 확인
                if (loginSessionService.isUserLoggedIn(userEntity.getUserId(), userEntity.getUserEmail())) {
                    throw new RuntimeException("이미 로그인된 사용자입니다.");
                }

                // 로그인 성공 시 is_online 값을 true로 설정
                userEntity.setUserIsOnline(true);
                userRepository.save(userEntity);

                // 로그인 세션을 MongoDB에 저장
                String token = "여기서 JWT 토큰 생성 로직";  // JWT 토큰 생성 로직 추가
                loginSessionService.saveLoginSession(userEntity.getUserId(), token);

                // 비밀번호가 일치할 경우 UserEntity를 UserDTO로 변환하여 반환
                return toUserDTO(userEntity);
            } else {
                // 비밀번호가 일치하지 않을 경우 예외 발생
                throw new RuntimeException("Invalid email or password");
            }
        } else {
            // 사용자가 존재하지 않을 경우 예외 발생
            throw new RuntimeException("Invalid email or password");
        }
    }

    // 로그아웃 메서드 (로그인 세션 삭제)
    @Override
    public void logout(String email) {
        // 이메일로 사용자 찾기
        Optional<UserEntity> loginUserEntity = userRepository.findByUserEmail(email);

        // 사용자가 존재할 경우
        if (loginUserEntity.isPresent()) {
            UserEntity userEntity = loginUserEntity.get();

            // 로그아웃 처리: is_online을 false로 설정
            userEntity.setUserIsOnline(false);
            userRepository.save(userEntity); // DB에 업데이트

            // 로그인 세션 삭제
            loginSessionService.removeLoginSession(userEntity.getUserId());
        } else {
            // 사용자가 존재하지 않을 경우 예외 발생
            throw new RuntimeException("Invalid email");
        }
    }
}
