package com.dropzone.chat.handler;

import com.dropzone.auth.jwt.JwtTokenProvider;
import com.dropzone.auth.service.CustomUserDetailsService;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Configuration;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.Message;
import org.springframework.messaging.MessageChannel;
import org.springframework.messaging.MessageDeliveryException;
import org.springframework.messaging.simp.stomp.StompCommand;
import org.springframework.messaging.simp.stomp.StompHeaderAccessor;
import org.springframework.messaging.support.ChannelInterceptor;
import org.springframework.messaging.support.MessageBuilder;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.web.client.HttpClientErrorException;

import java.nio.charset.StandardCharsets;

@Configuration
@RequiredArgsConstructor
public class ChatPreHandler implements ChannelInterceptor {

    @Autowired
    private JwtTokenProvider jwtTokenProvider;

    @Autowired
    private CustomUserDetailsService userDetailsService;

    private static final byte[] EMPTY_PAYLOAD = new byte[0];

    @Override
    public Message<?> preSend(Message<?> message, MessageChannel channel) {
        StompHeaderAccessor accessor = StompHeaderAccessor.wrap(message);

        try {
            // 연결 요청일 경우
            if (StompCommand.CONNECT.equals(accessor.getCommand())) {
                String token = accessor.getFirstNativeHeader("Authorization");

                if (token == null) {
                    token = accessor.getFirstNativeHeader("token");
                }

                if (token != null && token.startsWith("Bearer ")) {
                    token = token.substring(7); // Bearer 접두어 제거
                }

                if (token != null && jwtTokenProvider.validateToken(token)) {
                    String email = JwtTokenProvider.getEmailFromToken(token);
                    UserDetails userDetails = userDetailsService.loadUserByUsername(email);

                    // SecurityContext에 인증 정보 설정
                    UsernamePasswordAuthenticationToken authenication = new UsernamePasswordAuthenticationToken(userDetails, null, userDetails.getAuthorities());
                    SecurityContextHolder.getContext().setAuthentication(authenication);

                } else {
                    // 로그 및 예외 처리
                    System.out.println("Invalid JWT token");
                    return handleUnauthorizedException(message, new HttpClientErrorException(HttpStatus.UNAUTHORIZED));
                }
            }

        }
        catch (Exception ex) {
            return handleException(message, ex);
        }

        return message;

    }
    private Message<byte[]> handleException(Message<?> clientMessage, Throwable ex) {
        Throwable exception = converterThrowException(ex);

        if (exception instanceof HttpClientErrorException) {
            return handleUnauthorizedException(clientMessage, exception);
        }

        // 기타 예외는 기본 처리 로직으로 넘김
        return createErrorMessage(clientMessage, ex.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR.name());
    }

    private Throwable converterThrowException(Throwable exception) {
        if (exception instanceof MessageDeliveryException) {
            return exception.getCause();
        }
        return exception;
    }

    private Message<byte[]> handleUnauthorizedException(Message<?> clientMessage, Throwable ex) {
        return createErrorMessage(clientMessage, ex.getMessage(), HttpStatus.UNAUTHORIZED.name());
    }

    private Message<byte[]> createErrorMessage(Message<?> clientMessage, String message, String errorCode) {
        StompHeaderAccessor accessor = StompHeaderAccessor.create(StompCommand.ERROR);
        accessor.setMessage(errorCode);
        accessor.setLeaveMutable(true);

        // 기존 메시지의 receiptId 복사
        StompHeaderAccessor clientHeaderAccessor = StompHeaderAccessor.wrap(clientMessage);
        if (clientHeaderAccessor.getReceipt() != null) {
            accessor.setReceiptId(clientHeaderAccessor.getReceipt());
        }

        return MessageBuilder.createMessage(
                message != null ? message.getBytes(StandardCharsets.UTF_8) : EMPTY_PAYLOAD,
                accessor.getMessageHeaders()
        );
    }
}
