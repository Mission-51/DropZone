package com.dropzone.user.service;

import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.entity.UserEntity;
import org.apache.catalina.User;
import org.springframework.web.multipart.MultipartFile;

import java.util.List;

public interface UserService {

    // 이메일 중복 여부를 확인하는 메소드
    public boolean checkDuplicatedEmail(String userEmail);

    // 닉네임 중복 여부를 확인하는 메소드
    public boolean checkDuplicatedNickname(String userNickname);

    // 이메일 인증된 사용자 추가 (인증된 사용자의 이메일을 기록)
    public void addAuthenticatedUser(String userEmail);

    // 회원가입 메소드 (UserDTO를 받아 회원을 등록)
    public void signUp(UserDTO userDTO);

    // 회원 닉네임 수정 메소드 (기존 사용자 ID와 새로운 정보를 받아 회원 정보 수정)
    public void updateUserNickName(int existingId, UserDTO updateUserDTO);
    
    // 회원 닉네임 수정 시, UserDTO의 필드를 UserEntity로 업데이트 (실제 필드 값을 비교 및 업데이트)
    public UserEntity updateUserDTONickNameField(UserEntity updateUserEntity, UserDTO updateUserDTO);

    // 회원 비밀번호 수정 메소드
    public void updateUserPassword(int existingId, UserDTO updateUserDTO);
    
    // 회원 비밀번호 수정 시, UserDTO의 필드를 UserEntity로 업데이트 (실제 필드 값을 비교 및 업데이트)
    public UserEntity updateUserDTOPasswordField(UserEntity updateUserEntity, UserDTO updateUserDTO);

    // 회원 삭제 메소드 (ID로 회원을 삭제)
    public void deleteUser(int userId);

    // 전체 회원 목록을 검색하여 반환
    public List<UserSearchDTO> searchAllUser();

    // ID로 특정 회원을 검색하여 반환
    public UserSearchDTO searchById(int userId);

    // 이메일로 특정 회원을 검색하여 반환
    public UserDTO searchByEmail(String userEmail);

    // 클라이언트에서 사용하는 이메일로 특정 회원을 검색하여 반환
    public UserSearchDTO searchByEmailForClient(String userEmail);

    // 닉네임으로 특정 회원을 검색하여 반환
    public UserSearchDTO searchByNickname(String userNickname);

    // 이메일을 ID로 변환하는 메소드
    public int changeEmailToId(String userEmail);

}
