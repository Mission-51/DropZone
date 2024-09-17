package com.dropzone;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class HealthCheckController {
    @GetMapping("/api/health-check")
    public String healthCheck() {
        return "OK";
    }
}
