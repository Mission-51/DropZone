package com.dropzone.DropZone_BackEnd;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.autoconfigure.domain.EntityScan;
import org.springframework.context.annotation.ComponentScan;
import org.springframework.data.jpa.repository.config.EnableJpaAuditing;
import org.springframework.data.jpa.repository.config.EnableJpaRepositories;


@SpringBootApplication
@ComponentScan(basePackages = {"com.dropzone"})
@EntityScan("com.dropzone")
@EnableJpaRepositories("com.dropzone")
@EnableJpaAuditing

public class DropZoneBackEndApplication {
	public static void main(String[] args) {
		SpringApplication.run(DropZoneBackEndApplication.class, args);
	}

}
