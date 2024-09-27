package com.dropzone;

import io.swagger.v3.oas.annotations.OpenAPIDefinition;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaAuditing;

@OpenAPIDefinition(servers = {
        @Server(url = "/", description = "Default Server URL")
})
@EnableJpaAuditing
@SpringBootApplication
public class SpringSecurityMasterApplication {

    public static void main(String[] args) {
        SpringApplication.run(SpringSecurityMasterApplication.class, args);
    }

}
