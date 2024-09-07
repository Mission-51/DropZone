package me.heongyukim.springbootdeveloper;

import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
public class TestController {
    @GetMapping("/test")
    public String test() {
        return "제발되라...." +
                "황준, 손동희, 김동준, 차상곤, 박재영, 김헌규 레츠고!";
    }
}
