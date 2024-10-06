package com.dropzone.friend.service;

import org.assertj.core.api.Assertions;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.test.context.TestPropertySource;

import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

@SpringBootTest
@TestPropertySource(properties = "jasypt.encryptor.key=Dr@pZoneKey2024!@#$")
class FriendServiceTest {

    @Autowired
    private FriendService friendService;

    @Test
    void getWaitingFriendList() throws Exception {
        // given
        String email = "khg6436@naver.com";

        // when
        ResponseEntity<?> result = friendService.getWaitingFriendList(email);
        List<?> body = (List<?>) result.getBody();

        // then
        Assertions.assertThat(result.getStatusCode()).isEqualTo(HttpStatus.OK);
        Assertions.assertThat(body).isEmpty();

    }

    @Test
    void getFriendList() throws Exception {
        // given
        String email = "khg6436@naver.com";

        // when
        ResponseEntity<?> result = friendService.getFriendList(email);
        List<?> body = (List<?>) result.getBody();

        // then
        Assertions.assertThat(body).isEmpty();
    }

}