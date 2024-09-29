//package com.dropzone.chat.config;
//
//import com.dropzone.chat.entity.ChatMessage;
//import org.springframework.context.annotation.Bean;
//import org.springframework.context.annotation.Configuration;
//import org.springframework.data.redis.connection.RedisConnectionFactory;
//import org.springframework.data.redis.core.RedisTemplate;
//import org.springframework.data.redis.serializer.GenericJackson2JsonRedisSerializer;
//import org.springframework.data.redis.serializer.StringRedisSerializer;
//
//@Configuration
//public class RedisConfig {
//
//    @Bean
//    public RedisTemplate<String, ChatMessage> redisTemplate(RedisConnectionFactory connectionFactory) {
//        RedisTemplate<String, ChatMessage> template = new RedisTemplate<>();
//        template.setConnectionFactory(connectionFactory);
//
//        // key String 형식으로 저장
//        template.setKeySerializer(new StringRedisSerializer());
//
//        // value를 직렬화 하기 위해 JSON 직렬화를 사용
//        template.setValueSerializer(new GenericJackson2JsonRedisSerializer());
//
//        return template;
//    }
//}
