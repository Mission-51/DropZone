package com.dropzone.config;

import com.ulisesbocchio.jasyptspringboot.annotation.EnableEncryptableProperties;
import org.jasypt.encryption.StringEncryptor;
import org.jasypt.encryption.pbe.PooledPBEStringEncryptor;
import org.jasypt.encryption.pbe.config.SimpleStringPBEConfig;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration // 스프링에서 설정 파일로 사용됨
@EnableEncryptableProperties // Jasypt를 사용하여 암호화된 속성을 해독할 수 있도록 활성화함
public class JasyptConfig {

    // application.properties 또는 application.yml에서 암호화 키 값을 가져옴
    @Value("${jasypt.encryptor.key}")
    String key;

    // Jasypt StringEncryptor 빈을 생성하여 암호화/복호화에 사용됨
    @Bean(name = "jasyptStringEncryptor")
    public StringEncryptor stringEncryptor() {

        // PooledPBEStringEncryptor는 여러 스레드에서 사용할 수 있는 암호화 객체를 생성함
        PooledPBEStringEncryptor encryptor = new PooledPBEStringEncryptor();

        // SimpleStringPBEConfig는 Jasypt 암호화 설정을 단순하게 구성함
        SimpleStringPBEConfig config = new SimpleStringPBEConfig();

        // 암호화/복호화를 위해 사용할 키를 설정함
        config.setPassword(key);

        // 암호화 알고리즘을 PBEWithMD5AndDES로 설정함
        config.setAlgorithm("PBEWithMD5AndDES");

        // 암호화 시 몇 번 반복할지 설정함 (1000번 반복)
        config.setKeyObtentionIterations("1000");

        // 스레드 풀 사이즈를 1로 설정하여 병렬 처리를 제한함
        config.setPoolSize("1");

        // 암호화 알고리즘 제공자를 SunJCE로 설정함
        config.setProviderName("SunJCE");

        // 무작위 솔트를 생성하는 클래스를 지정함
        config.setSaltGeneratorClassName("org.jasypt.salt.RandomSaltGenerator");

        // 암호화된 문자열의 출력 형식을 base64로 설정함
        config.setStringOutputType("base64");

        // 설정을 암호화 객체에 적용함
        encryptor.setConfig(config);

        // 암호화 객체를 반환함
        return encryptor;
    }
}