# Amazon Corretto 17 이미지 사용
FROM amazoncorretto:17.0.12

# 빌드된 JAR 파일의 경로를 ARG로 설정
ARG JAR_FILE=build/libs/*.jar

# JAR 파일을 컨테이너에 복사
COPY ${JAR_FILE} app.jar

# JAR 파일 실행
ENTRYPOINT ["java", "-jar", "app.jar"]