package me.heongyukim.springbootdeveloper;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class TestController {
    @GetMapping("/test")
    public String test() {
        return "Hello, world!!!@@@@ 찐막 CICD 테스트...황준 멍청이" +
                "황준, 손동희, 김동준, 차상곤, 박재영, 김헌규 레츠고!";
    }
}
