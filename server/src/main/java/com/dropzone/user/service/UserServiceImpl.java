package com.dropzone.user.service;

import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import jakarta.persistence.EntityNotFoundException;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import org.springframework.web.multipart.MultipartFile;

import java.util.List;
import java.util.Optional;
import java.util.Set;
import java.util.concurrent.ConcurrentHashMap;
import java.util.stream.Collectors;

import static org.springframework.data.jpa.domain.AbstractPersistable_.id;

@Service
@RequiredArgsConstructor
public class UserServiceImpl implements UserService {

    @Autowired
    private UserRepository userRepository;

    private final Set<String> authenticatedUsers = ConcurrentHashMap.newKeySet();

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
        if (!authenticatedUsers.contains(userDTO.getUserEmail())) {
            throw new IllegalStateException("이메일 인증이 완료되지 않았습니다.");
        }
        UserEntity userEntity = UserEntity.toSaveEntity(userDTO);
        userRepository.save(userEntity);
        authenticatedUsers.remove(userDTO.getUserEmail());
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

    @Transactional
    @Override
    public void updateUser(int existingId, UserDTO updateUserDTO) {
        Optional<UserEntity> findUserEntity = userRepository.findById(existingId);
        if (findUserEntity.isPresent()) {
            UserEntity updateUserEntity = findUserEntity.get();
            userRepository.save(updateUserDTOFields(updateUserEntity, updateUserDTO));
        } else {
            throw new EntityNotFoundException("User with id " + existingId + " not found");
        }
    }

    @Override
    @Transactional
    public UserEntity updateUserDTOFields(UserEntity updateUserEntity, UserDTO updateUserDTO) {
        if (updateUserDTO.getUserNickname() != null) {
            updateUserEntity.setUserNickname(updateUserDTO.getUserNickname());
        }
        if(updateUserDTO.getUserPassword() != null) {
            updateUserEntity.setUserPassword(updateUserDTO.getUserPassword());
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
