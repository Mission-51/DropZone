package com.dropzone;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.data.jpa.repository.config.EnableJpaAuditing;

@EnableJpaAuditing
@SpringBootApplication
public class SpringSecurityMasterApplication {

    public static void main(String[] args) {
        SpringApplication.run(SpringSecurityMasterApplication.class, args);
    }

}
