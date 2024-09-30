package com.dropzone.friend.controller;


import com.dropzone.friend.service.FriendService;
import com.dropzone.user.dto.UserSearchDTO;
import com.dropzone.user.service.UserServiceImpl;
import io.swagger.v3.oas.annotations.Operation;
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
@Tag(name = "친구 API", description = " 친구 추가 / 친구 요청 수락 / 친구 요청 거절 / 친구 목록 조회 ")
public class FriendController {

    // 의존성 주입 받음
    private final UserServiceImpl userService;
    private final FriendService friendService;


    @PostMapping("/friends/{nickname}")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "친구추가 API", description = "친구의 email을 이용한 친구 추가")
    public ResponseEntity<?> sendFriendShipRequest(@Valid @PathVariable("nickname") String nickName) throws Exception {
        try {
            // searchByNickname은 UserSearchDTO 객체를 반환하므로 이를 확인
            UserSearchDTO userSearchDTO = userService.searchByNickname(nickName);
            
            // userSearchDTO에서 이메일 정보를 가져오기
            String email = userSearchDTO.getUserEmail();

            // 이메일 정보를 통해서 친구 추가 요청을 처리하는 로직
            return friendService.createFriendship(email);

        } catch (EntityNotFoundException e) {
            // 사용자가 존재하지 않을 경우 예외 처리
            throw new Exception("대상 회원이 존재하지 않습니다.");
        }
    }

    @GetMapping("/friends/received")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "받은 친구 요청 API", description = "친구가 나에게 친구 추가 요청을 보낸 것을 확인할 수 있는 API")
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

    @GetMapping("friends/list")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "친구 목록 API", description = "친구 목록을 가져오는 API")
    public ResponseEntity<?> getFriendList() throws Exception {
        // 현재 로그인된 사용자 정보를 가져옴
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();

        if (authentication == null || !(authentication.getPrincipal() instanceof UserDetails)) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body("로그인 되지 않은 사람입니다.");
        }
        
        // 사용자 정보에서 이메일 추출
        UserDetails userDetails = (UserDetails) authentication.getPrincipal();
        String email = userDetails.getUsername();

        if (email == null) {
            throw new Exception("로그인된 사용자를 찾을 수 없습니다.");
        }

        // 이메일로 친구 목록을 조회하고 반환
        return friendService.getFriendList(email);
    }

    // 친구 요청 수락
    @PostMapping("/friends/approve/{friendShipId}")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "친구 요청 수락 API", description = "친구에게서 온 친구 요청을 수락하는 API")
    public ResponseEntity<?> approveFriendShip (@Valid @PathVariable("friendShipId") Long friendShipId) throws Exception {
        return friendService.approveFriendShipRequest(friendShipId);
    }

    // 친구 요청 거절
    @PostMapping("/friends/refuse/{friendShipId}")
    @ResponseStatus(HttpStatus.OK)
    @Operation(summary = "친구 요청 거절 API", description = "친구에게서 온 친구 요청을 거절하는 API")
    public ResponseEntity<?> refuseFriendShip (@Valid @PathVariable("friendShipId") Long friendShipId) throws Exception {
        return friendService.refuseFriendShipRequest(friendShipId);
    }
}
