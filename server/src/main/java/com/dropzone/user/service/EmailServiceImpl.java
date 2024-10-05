package com.dropzone.user.service;

import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.stereotype.Service;

import java.security.SecureRandom;
import java.util.Map;
import java.util.concurrent.ConcurrentHashMap;

@Service
@Slf4j
class EmailServiceImpl implements EmailService {

    @Autowired
    private JavaMailSender mailSender;  // 스프링에서 제공하는 이메일 전송 도구

    @Autowired
    private UserService userService;  // 유저 서비스 (이메일 인증 후 유저를 인증된 상태로 처리하기 위해 사용)

    // 이메일과 인증 코드를 저장하는 ConcurrentHashMap (동시성 문제 해결)
    private final Map<String, String> authenticationStore = new ConcurrentHashMap<>();

    // 인증 코드를 포함한 이메일을 전송하는 메소드
    public void sendAuthenticationCodeEmail(String email) {
        String authenticationCode = generateAuthenticationCode();  // 6자리 인증 코드 생성
        String subject = "인증 코드를 확인해주세요";  // 이메일 제목
        String text = "이메일 인증을 완료하기 위해, 다음 코드를 입력해주세요: " + authenticationCode;  // 이메일 본문

        // 이메일 전송을 위한 메시지 설정
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(email);  // 수신자 이메일
        message.setSubject(subject);  // 제목
        message.setText(text);  // 내용
        mailSender.send(message);  // 이메일 전송

        // 인증 코드를 Map에 저장
        authenticationStore.put(email, authenticationCode);
        log.info(authenticationStore.get("yg9618@naver.com"));
    }

    // 6자리 인증 코드를 생성하는 메소드
    private String generateAuthenticationCode() {
        SecureRandom random = new SecureRandom();  // 보안 수준의 난수 생성기
        int num = random.nextInt(1000000);  // 0부터 999999까지의 난수 생성
        return String.format("%06d", num);  // 6자리 인증 코드 생성
    }

    // 이메일과 인증 코드를 비교하여 인증 여부를 반환하는 메소드
    @Override
    public boolean authenticateEmail(String email, String authenticationCode) {
        String storeCode = authenticationStore.get(email);  // 저장된 인증 코드 가져오기

        // 저장된 코드와 입력된 코드가 일치하면 인증 성공
        if (storeCode != null && storeCode.equals(authenticationCode)) {
            authenticationStore.remove(email);  // 인증이 완료된 후 맵에서 제거
            userService.addAuthenticatedUser(email);  // 인증된 유저로 추가
            return true;
        }

        return false;  // 인증 실패 시 false 반환
    }
}
