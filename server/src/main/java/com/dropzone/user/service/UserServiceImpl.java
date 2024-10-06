package com.dropzone.user.service;

import com.dropzone.matchstatistics.entity.UserStatisticsEntity;
import com.dropzone.matchstatistics.repository.UserStatisticsRepository;
import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import jakarta.persistence.EntityNotFoundException;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.security.crypto.password.PasswordEncoder;

import java.sql.Time;
import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final Set<String> authenticatedUsers = ConcurrentHashMap.newKeySet();
    private final UserStatisticsRepository userStatisticsRepository;
    private final PasswordEncoder passwordEncoder;

    @Override
    public boolean checkDuplicatedEmail(String userEmail) {
        return userRepository.findByUserEmail(userEmail).isPresent();
    }

    public boolean checkDuplicatedNickname(String userNickname) {
        return userRepository.findByUserNickname(userNickname).isPresent();
    }

    @Override
    public void addAuthenticatedUser(String userEmail){
        authenticatedUsers.add(userEmail);
    }

    @Override
    public void signUp(UserDTO userDTO) {
        // 이메일 인증 확인 로직
        if (!authenticatedUsers.contains(userDTO.getUserEmail())) {
            throw new IllegalStateException("이메일 인증이 완료되지 않았습니다.");
        }

        // 비밀번호 암호화 처리
        String encodedPassword = passwordEncoder.encode(userDTO.getUserPassword());
        userDTO.setUserPassword(encodedPassword);

        // UserEntity로 변환 후 회원 정보 저장
        UserEntity userEntity = UserEntity.toSaveEntity(userDTO);
        userRepository.save(userEntity); // 회원 정보 저장

        // 회원가입 후 유저 통계 테이블에 기본 통계 정보 추가
        UserStatisticsEntity userStatisticsEntity = new UserStatisticsEntity();
        userStatisticsEntity.setUserId(userEntity.getUserId());
        userStatisticsEntity.setRankingPoints(100); // 초기값 100
        userStatisticsEntity.setTotalKills(0);
        userStatisticsEntity.setTotalDamage(0);
        userStatisticsEntity.setTotalPlaytime(Time.valueOf("00:00:00")); // 초기 플레이 시간 0
        userStatisticsEntity.setTotalGames(0);
        userStatisticsEntity.setTotalWins(0);

        // UserStatistics 테이블에 저장
        userStatisticsRepository.save(userStatisticsEntity);
    }

    @Override
    public List<UserSearchDTO> searchAllUser() {
        List<UserEntity> userEntities = userRepository.findAll();
        return userEntities.stream()
                .map(UserSearchDTO::toUserSearchDTO)
                .collect(Collectors.toList());
    }

    @Override
    public UserSearchDTO searchById(int id) {
        UserEntity userEntity = userRepository.findById(id)
                .orElseThrow(() -> new EntityNotFoundException("User with id " + id + " not found"));
        return UserSearchDTO.toUserSearchDTO(userEntity);
    }

    @Override
    public UserDTO searchByEmail(String userEmail) {
        UserEntity userEntity = userRepository.findByUserEmail(userEmail)
                .orElseThrow(() -> new EntityNotFoundException("User with email " + userEmail + " not found"));
        return UserDTO.toUserDTO(userEntity);
    }

    public UserSearchDTO searchByEmailForClient(String userEmail){
        UserEntity userEntity = userRepository.findByUserEmail(userEmail)
                .orElseThrow(() -> new EntityNotFoundException("User with email " + userEmail + " not found"));
        return UserSearchDTO.toUserSearchDTO(userEntity);
    }

    @Override
    public UserSearchDTO searchByNickname(String userNickname) {
        UserEntity userEntity = userRepository.findByUserNickname(userNickname)
                .orElseThrow(() -> new EntityNotFoundException("User with nickname " + userNickname + " not found"));
        return UserSearchDTO.toUserSearchDTO(userEntity);
    }

    // 닉네임 변경
    @Override
    @Transactional
    public void updateUserNickName(int existingId, UserDTO updateUserDTO) {
        Optional<UserEntity> findUserEntity = userRepository.findById(existingId);
        if (findUserEntity.isPresent()) {
            UserEntity updateUserEntity = findUserEntity.get();
            userRepository.save(updateUserDTONickNameField(updateUserEntity, updateUserDTO));
        } else {
            throw new EntityNotFoundException("User with id " + existingId + " not found");
        }
    }

    @Override
    @Transactional
    public UserEntity updateUserDTONickNameField(UserEntity updateUserEntity, UserDTO updateUserDTO) {
        if (updateUserDTO.getUserNickname() != null) {
            updateUserEntity.setUserNickname(updateUserDTO.getUserNickname());
        }
        return updateUserEntity;
    }

    // 비밀번호 변경
    @Override
    @Transactional
    public void updateUserPassword(int existingId, UserDTO updateUserDTO) {
        Optional<UserEntity> findUserEntity = userRepository.findById(existingId);
        if (findUserEntity.isPresent()) {
            UserEntity updateUserEntity = findUserEntity.get();
            userRepository.save(updateUserDTOPasswordField(updateUserEntity, updateUserDTO));
        } else {
            throw  new EntityNotFoundException("User with id " + existingId + " not found");
        }
    }

    @Override
    public UserEntity updateUserDTOPasswordField(UserEntity updateUserEntity, UserDTO updateUserDTO) {
        if (updateUserDTO.getUserPassword() != null) {
            String updatePassword = passwordEncoder.encode(updateUserDTO.getUserPassword());
            updateUserEntity.setUserPassword(updatePassword);
        }
        return updateUserEntity;
    }

    @Override
    public void deleteUser(int userId) {
        Optional<UserEntity> findUserEntity = userRepository.findById(userId);
        userRepository.delete(findUserEntity.get());
    }

    @Override
    public int changeEmailToId(String userEmail) {
        UserEntity userEntity = userRepository.findByUserEmail(userEmail)
                .orElseThrow(() -> new EntityNotFoundException("User with email " + userEmail + " not found"));
        return userEntity.getUserId();
    }
}
