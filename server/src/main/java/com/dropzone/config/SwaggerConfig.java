package com.dropzone.config;

import io.swagger.v3.oas.models.Components;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.security.SecurityRequirement;
import io.swagger.v3.oas.models.security.SecurityScheme;
import io.swagger.v3.oas.models.security.SecurityScheme.In;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration // 스프링에서 설정 파일로 인식되도록 함
public class SwaggerConfig {

    // OpenAPI 설정을 정의하는 빈을 생성하여 Swagger UI에서 API 문서를 보여줌
    @Bean
    public OpenAPI OpenAPI() {
        // API 문서에 대한 정보를 설정함 (제목, 버전, 설명 등)
        Info info = new Info()
                .title("DropZone") // API 문서의 제목
                .version("1.0") // API 문서의 버전
                .description("DropZone"); // API 문서에 대한 설명

        // JWT 인증 방식의 이름을 정의
        String jwtSchemeName = "jwtAuth";

        // API 요청 시 보안 요구 사항으로 JWT 인증을 설정
        SecurityRequirement securityRequirement = new SecurityRequirement().addList(jwtSchemeName);

        // JWT 인증 스킴을 구성하는 컴포넌트를 정의
        Components components = new Components()
                .addSecuritySchemes(jwtSchemeName, new SecurityScheme().name(jwtSchemeName) // 스킴 이름 설정
                        .type(SecurityScheme.Type.HTTP) // 스킴 타입을 HTTP로 설정
                        .scheme("Bearer") // HTTP 인증 타입으로 Bearer 설정 (토큰 기반)
                        .bearerFormat("JWT")); // Bearer 인증 형식을 JWT로 설정

        // 설정한 정보(컴포넌트, 보안 요구 사항)를 OpenAPI에 추가하여 반환
        return new OpenAPI().components(new Components()) // 새로운 컴포넌트 객체 추가
                .info(info) // API 정보 설정
                .addSecurityItem(securityRequirement) // 보안 요구 사항 추가
                .components(components); // 인증 스킴 컴포넌트 추가
    }
}
