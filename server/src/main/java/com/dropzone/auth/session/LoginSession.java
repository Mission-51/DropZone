package com.dropzone.auth.session;

import jakarta.persistence.Id;
import lombok.Getter;
import lombok.Setter;
import org.springframework.data.mongodb.core.mapping.Document;


@Getter
@Setter
@Document(collection = "login_sessions")
public class LoginSession {

    @Id
    private String id;  // MongoDB에서 자동 생성되는 ID

    private int userId;  // 회원 ID (User 테이블의 user_id에 맞춤)
    private String token;  // JWT 토큰
    private long loginTime;  // 로그인 시간 (타임스탬프 형태로 저장)
    private boolean isOnline;  // 접속 여부 (true: 온라인, false: 오프라인)

    public LoginSession(int userId, String token, long loginTime, boolean isOnline) {
        this.userId = userId;
        this.token = token;
        this.loginTime = loginTime;
        this.isOnline = isOnline;
    }

}
