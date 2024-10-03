package com.dropzone.friend.service;

import com.dropzone.friend.dto.FriendReponseDto;
import com.dropzone.friend.dto.FriendListDto;
import com.dropzone.friend.dto.WaitingFriendListDto;
import com.dropzone.friend.entity.FriendShipEntity;
import com.dropzone.friend.entity.FriendShipStatus;
import com.dropzone.friend.repository.FriendShipRepository;
import lombok.Builder;
import lombok.RequiredArgsConstructor;
import com.dropzone.user.entity.UserEntity;
import com.dropzone.user.repository.UserRepository;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.ArrayList;
import java.util.List;

@Service
@Builder
@RequiredArgsConstructor
public class FriendService {

    // 의존성 주입
    private final UserRepository userRepository;
    private final FriendShipRepository friendShipRepository;

    // 친구요청 생성 메서드
    public ResponseEntity<?> createFriendship(String toEmail) throws Exception {

        // 현재 로그인 되어 있는 사람 (보내는 사람)
        Authentication authentication = SecurityContextHolder.getContext().getAuthentication();
        UserDetails userDetails = (UserDetails) authentication.getPrincipal();
        String fromEmail = userDetails.getUsername();
        
        // 유저 정보를 가져오기
        UserEntity fromUser = userRepository.findByUserEmail(fromEmail).orElseThrow(() -> new Exception("회원 조회 실패"));
        UserEntity toUser = userRepository.findByUserEmail(toEmail).orElseThrow(() -> new Exception("회원 조회 실패"));

        // 이미 상대방에게 요청을 보냈거나 상대방으로부터 요청을 받은 것이 있는지 확인
        // 내가 이미 상대방에 보낸 요청이 있으면 클라이언트에서 이미 보낸 요청이 있습니다.라는 문구를 띄우도록 설정
        // 내가 받은 요청이 있는 상태에서 그 상대에게 친구요청을 보내려고 하면 받은 요청이 있습니다. 라는 문구를 띄우도록 설정
        List<FriendShipEntity> friendShipList = friendShipRepository.findAll();
        
        for (FriendShipEntity friendShip : friendShipList) {
            if (friendShip.getUserEmail().equals(fromEmail) && friendShip.getFriendEmail().equals(toEmail)) {
                FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                        .message("이미 받은 친구 요청이 있습니다!")
                        .build();
                return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
            } else if (friendShip.getUserEmail().equals(toEmail) && friendShip.getFriendEmail().equals(fromEmail)) {
                FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                        .message("이미 보낸 친구 요청이 있습니다!")
                        .build();
                return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
            } else if (friendShip.getUserEmail().equals(friendShip.getFriendEmail())) {
                FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                        .message("자기 자신한테는 친구 요청을 보낼 수 없습니다!")
                        .build();

                return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
            }
        }

        // 받는 사람측에 저장될 친구 요청
        FriendShipEntity friendShipFrom =  FriendShipEntity.builder()
                .user(fromUser)
                .userEmail(fromEmail)
                .friendEmail(toEmail)
                .userNickname(fromUser.getUserNickname())
                .friendNickname(toUser.getUserNickname())
                .status(FriendShipStatus.WAITTING)
                .isFrom(true)
                .build();


        // 보내는 사람측에 저장될 친구 요청
        FriendShipEntity friendShipTo =  FriendShipEntity.builder()
                .user(toUser)
                .userEmail(toEmail)
                .friendEmail(fromEmail)
                .userNickname(toUser.getUserNickname())
                .friendNickname(fromUser.getUserNickname())
                .status(FriendShipStatus.WAITTING)
                .isFrom(false)
                .build();

        // 매칭되는 친구요청의 아이디를 서로 저장한다.
        friendShipRepository.save(friendShipTo);
        friendShipRepository.save(friendShipFrom);

        // 매칭되는 친구요청의 아이디를 서로 저장한다.
        friendShipTo.setCounterpartId(friendShipFrom.getId());
        friendShipFrom.setCounterpartId(friendShipTo.getId());

        // 변경 사항 저장
        friendShipRepository.save(friendShipTo);
        friendShipRepository.save(friendShipFrom);


        // 각각의 유저 리스트에 저장
        fromUser.getFriendshipList().add(friendShipTo);
        toUser.getFriendshipList().add(friendShipFrom);

        FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                .message("친구요청이 완료 되었습니다!")
                .build();

        return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
    }
    
    
    // 받은 친구 요청 중, 수락 되지 않은 요청을 조회하는 메서드
    @Transactional
    public ResponseEntity<?> getWaitingFriendList(String email) throws Exception {
        UserEntity user = userRepository.findByUserEmail(email).orElseThrow(() -> new Exception("회원 조회 실패"));
        List<FriendShipEntity> friendShipList = user.getFriendshipList();

        // 조회된 결과 객체를 담을 Dto 리스트
        List<WaitingFriendListDto> result = new ArrayList<>();

        for (FriendShipEntity request : friendShipList) {
            // 보낸 요청이 아니고 && 수락 대기중인 요청만 조회
            if (!request.isFrom() && request.getStatus() == FriendShipStatus.WAITTING) {
                UserEntity friend = userRepository.findByUserEmail(request.getFriendEmail()).orElseThrow(() -> new Exception("회원 조회 실패"));
                WaitingFriendListDto dto = WaitingFriendListDto.builder()
                        .friendShipId(request.getId())
                        .friendEmail(friend.getUserEmail())
                        .friendNickname(friend.getUserNickname())
                        .status(request.getStatus())
                        .build();
                result.add(dto);
            }
        }
        
        // 결과 반환
        return new ResponseEntity<>(result, HttpStatus.OK);
    }

    // 수락한 친구 요청 조회하는 메서드(친추된 목록)
    @Transactional
    public ResponseEntity<?> getFriendList(String email) throws Exception {
        UserEntity user = userRepository.findByUserEmail(email).orElseThrow(() -> new Exception("회원 조회 실패"));
        List<FriendShipEntity> friendShipList = user.getFriendshipList();

        // 조회된 결과 객체를 담을 Dto 리스트
        List<FriendListDto> result = new ArrayList<>();

        for (FriendShipEntity friendShip : friendShipList) {
            // 수락된 친구 요청만 dto에 담기
            if (friendShip.getStatus() == FriendShipStatus.ACCEPT) {
                FriendListDto dto = FriendListDto.builder()
                        .friendShipId(friendShip.getId())
                        .friendEmail(friendShip.getFriendEmail())
                        .friendNickname(friendShip.getFriendNickname())
                        .build();
                result.add(dto);
            }
        }

        return new ResponseEntity<>(result, HttpStatus.OK);
    }


    // 친구 요청 수락
    public ResponseEntity<?> approveFriendShipRequest(Long friendShipId) throws Exception {

        // 누를 친구 요청과 매칭되는 상대방 친구 요청 둘다 가져옴
        FriendShipEntity friendShip = friendShipRepository.findById(friendShipId).orElseThrow(() -> new Exception("친구 요청 조회 실패"));
        FriendShipEntity counterFriendShip = friendShipRepository.findById(friendShip.getCounterpartId()).orElseThrow(() -> new Exception("친구 요청 조회 실패"));

        // 둘다 상태를 ACCEPT로 변경함
        friendShip.acceptFriendShipRequest();
        counterFriendShip.acceptFriendShipRequest();

        // 변경 사항 저장
        friendShipRepository.save(friendShip);
        friendShipRepository.save(counterFriendShip);

        FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                .message("친구 추가 되었습니다!")
                .build();

        return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
    }

    // 친구 요청 거절
    public ResponseEntity<?> refuseFriendShipRequest(Long friendShipId) throws Exception {
        // 누를 친구 요청과 매칭되는 상대방 친구 요청 둘다 가져옴
        FriendShipEntity friendShip = friendShipRepository.findById(friendShipId).orElseThrow(() -> new Exception("친구 요청 조회 실패"));
        FriendShipEntity counterFriendShip = friendShipRepository.findById(friendShip.getCounterpartId()).orElseThrow(() -> new Exception("친구 요청 조회 실패"));

        // 변경 사항 저장
        friendShipRepository.delete(friendShip);
        friendShipRepository.delete(counterFriendShip);

        FriendReponseDto friendReponseDto = FriendReponseDto.builder()
                .message("거절이 완료되었습니다!")
                .build();

        return new ResponseEntity<>(friendReponseDto, HttpStatus.OK);
    }
    
    // 친구 삭제 기능
    public ResponseEntity<?> deleteFriendship(Long friendshipId) throws Exception {

        // 수락된 자신의 친구 요청과 상대방의 친구 요청을 찾아오기
        FriendShipEntity friendShip = friendShipRepository.findById(friendshipId).orElseThrow(() -> new Exception("친구 요청 조회 실패"));
        FriendShipEntity counterFriendShip = friendShipRepository.findById(friendShip.getCounterpartId()).orElseThrow(() -> new Exception("친구 요청 조회 실패"));
        
        // 만약 수락되지 않은 친구 요청이면 예외 반환
        if (friendShip.getStatus() != FriendShipStatus.WAITTING || counterFriendShip.getStatus() != FriendShipStatus.WAITTING) {
            return new ResponseEntity<>("친구 상태가 아닙니다!" ,HttpStatus.NOT_FOUND);
        }

        // 친구 삭제
        friendShipRepository.delete(friendShip);
        friendShipRepository.delete(counterFriendShip);

        return new ResponseEntity<>("친구 삭제가 완료되었습니다!", HttpStatus.OK);

    }
}
