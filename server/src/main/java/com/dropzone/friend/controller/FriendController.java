package com.dropzone.friend.controller;

import com.dropzone.friend.service.FriendService;
import com.dropzone.user.dto.UserDTO;
import com.dropzone.user.service.UserServiceImpl;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.persistence.EntityNotFoundException;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.bind.annotation.*;

@RestController
@Slf4j
@CrossOrigin("*")
@RequiredArgsConstructor
@RequestMapping("/api/users")
@Tag(name = "친구 API", description = " 친구 추가 / 친구 요청 승인 / 친구 목록 조회 ")
public class FriendController {

    // 의존성 주입 받음
    private final UserServiceImpl userService;
    private final FriendService friendService;


    @PostMapping("/friends/{email}")
    @ResponseStatus(HttpStatus.OK)
    public String sendFriendShipRequest(@Valid @PathVariable("email") String email) throws Exception {
        try {
            // searchByNickname은 UserSearchDTO 객체를 반환하므로 이를 확인
            UserDTO userDTO = userService.searchByEmail(email);
            
            // 친구 추가 요청을 처리하는 로직
            friendService.createFriendship(email);

            return "친구추가 성공";

        } catch (EntityNotFoundException e) {
            // 사용자가 존재하지 않을 경우 예외 처리
            throw new Exception("대상 회원이 존재하지 않습니다.");
        }
    }

    @GetMapping("/friends/received")
    @ResponseStatus(HttpStatus.OK)
    public ResponseEntity<?> getWaitingFriendInfo() throws Exception {

        // 현재 로그인된 사용자 정보를 가져옴
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();



        if (authentication == null || !(authentication.getPrincipal() instanceof UserDetails)) {

            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("로그인되지 않은 사용자입니다.");
        }
        
        // 사용자 정보에서 이메일 추출
        UserDetails userDetails = (UserDetails) authentication.getPrincipal();
        String email = userDetails.getUsername();

        if (email == null) {
            throw new Exception("로그인된 사용자를 찾을 수 없습니다.");
        }

        // 이메일로 친구 요청 대기 목록을 조회하고 반환
        return friendService.getWaitingFriendList(email);
    }

    @PostMapping("/friends/approve/{friendShipId}")
    @ResponseStatus(HttpStatus.OK)
    public String approveFriendShip (@Valid @PathVariable("friendShipId") Long friendShipId) throws Exception {
        return friendService.approveFriendShipRequest(friendShipId);
    }

}
