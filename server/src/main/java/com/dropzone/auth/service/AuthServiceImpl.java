package com.dropzone.auth.service;

import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import com.dropzone.user.service.UserService;
import lombok.RequiredArgsConstructor;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.Optional;

import static com.dropzone.user.dto.UserDTO.toUserDTO;

@Service
@RequiredArgsConstructor
public class AuthServiceImpl implements AuthService {

    private final UserRepository userRepository;

    // 이메일과 비밀번호로 로그인하는 메서드
    @Override
    public UserDTO loginByEmail(String email, String password) {

        // 이메일과 비밀번호로 사용자 찾기
        Optional<UserEntity> loginUserEntity = userRepository.findByUserEmailAndUserPassword(email, password);

        // 사용자가 존재할 경우, DTO로 변환하여 반환
        if (loginUserEntity.isPresent()) {
            UserEntity userEntity = loginUserEntity.get();
            return toUserDTO(userEntity); // UserEntity를 UserDTO로 변환하여 반환
        } else {
            // 사용자가 존재하지 않을 경우 예외 발생
            throw new RuntimeException("Invalid email or password");
        }
    }
}
