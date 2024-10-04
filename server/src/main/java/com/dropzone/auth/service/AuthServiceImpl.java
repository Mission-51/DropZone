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
    private final PasswordEncoder passwordEncoder;

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

                // 로그인 성공 시 is_online 값을 true로 설정
                userEntity.setUserIsOnline(true);
                userRepository.save(userEntity);

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

//    @Override
//    public void logout(String email) {
//        // 이메일로 사용자 찾기
//        Optional<UserEntity> loginUserEntity = userRepository.findByUserEmail(email);
//
//        // 사용자가 존재할 경우
//        if (loginUserEntity.isPresent()) {
//            UserEntity userEntity = loginUserEntity.get();
//
//            // 로그아웃 처리: is_online을 false로 설정
//            userEntity.setUserIsOnline(false);
//            userRepository.save(userEntity); // DB에 업데이트
//        } else {
//            // 사용자가 존재하지 않을 경우 예외 발생
//            throw new RuntimeException("Invalid email");
//        }
//    }
}
