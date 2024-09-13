package io.security.springsecuritymaster;

import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;

@SpringBootTest
@TestPropertySource(properties = {
    "jasypt.encryptor.password=Dr@pZoneKey2024!@#$"
})
class SpringSecurityMasterApplicationTests {

    @Test
    void contextLoads() {
    }

}
