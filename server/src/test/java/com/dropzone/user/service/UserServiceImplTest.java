package com.dropzone.user.service;

import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.Mockito;
import org.mockito.junit.jupiter.MockitoExtension;
import org.springframework.boot.test.context.SpringBootTest;
import java.util.Optional;
import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.ArgumentMatchers.anyInt;
import static org.mockito.ArgumentMatchers.anyString;
import static org.mockito.BDDMockito.given;

@ExtendWith(MockitoExtension.class)
@SpringBootTest
class UserServiceImplTest {

    @InjectMocks
    private UserServiceImpl userService;

    @Mock
    private UserRepository userRepository;

    @Test
    void signUp() {
        // given
        UserDTO userDTO = new UserDTO();
        userDTO.setUserEmail("test@example.com");
        userDTO.setUserPassword("password123");
        userDTO.setUserNickname("TestUser");

        // 이메일 인증이 완료된 상태로 가정 (authenticatedUsers에 추가)
        userService.addAuthenticatedUser(userDTO.getUserEmail());

        given(userRepository.findByUserEmail(anyString())).willReturn(Optional.empty());

        // when
        userService.signUp(userDTO);

        // then
        Mockito.verify(userRepository, Mockito.times(1)).save(Mockito.any(UserEntity.class));
    }

    @Test
    void searchById() {
        // given
        UserEntity userEntity = new UserEntity();
        userEntity.setUserId(1);
        userEntity.setUserEmail("test@example.com");

        given(userRepository.findById(anyInt())).willReturn(Optional.of(userEntity));

        // when
        UserSearchDTO foundUser = userService.searchById(1);

        // then
        assertEquals(1, foundUser.getUserId());
        assertEquals("test@example.com", foundUser.getUserEmail());
    }

    @Test
    void searchByEmail() {
        // given
        UserEntity userEntity = new UserEntity();
        userEntity.setUserId(1);
        userEntity.setUserEmail("test@example.com");

        given(userRepository.findByUserEmail(anyString())).willReturn(Optional.of(userEntity));

        // when
        UserDTO foundUser = userService.searchByEmail("test@example.com");

        // then
        assertEquals(1, foundUser.getUserId());
        assertEquals("test@example.com", foundUser.getUserEmail());
    }

    @Test
    void updateUser() {
        // given
        UserEntity userEntity = new UserEntity();
        userEntity.setUserId(1);
        userEntity.setUserEmail("old@example.com");
        userEntity.setUserPassword("oldPassword");

        UserDTO updateUserDTO = new UserDTO();
        updateUserDTO.setUserPassword("newPassword");

        given(userRepository.findById(anyInt())).willReturn(Optional.of(userEntity));

        // when
        userService.updateUser(1, updateUserDTO);

        // then
        Mockito.verify(userRepository, Mockito.times(1)).save(userEntity);
        assertEquals("newPassword", userEntity.getUserPassword());
    }

    @Test
    void updateUserDTOFields() {
        // given
        UserEntity userEntity = new UserEntity();
        userEntity.setUserId(1);
        userEntity.setUserNickname("oldNickname");

        UserDTO updateUserDTO = new UserDTO();
        updateUserDTO.setUserNickname("newNickname");

        // when
        UserEntity updatedEntity = userService.updateUserDTOFields(userEntity, updateUserDTO);

        // then
        assertEquals("newNickname", updatedEntity.getUserNickname());
    }

    @Test
    void deleteUser() {
        // given
        UserEntity userEntity = new UserEntity();
        userEntity.setUserId(1);

        given(userRepository.findById(anyInt())).willReturn(Optional.of(userEntity));

        // when
        userService.deleteUser(1);

        // then
        Mockito.verify(userRepository, Mockito.times(1)).delete(userEntity);
    }
}
