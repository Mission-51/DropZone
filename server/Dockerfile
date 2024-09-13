# openjdk:17 이미지 사용
FROM openjdk:17

# tzdata 패키지 설치 및 타임존 설정
RUN ln -snf /usr/share/zoneinfo/Asia/Seoul /etc/localtime && echo Asia/Seoul > /etc/timezone

# 빌드된 JAR 파일의 경로를 ARG로 설정
ARG JAR_FILE=build/libs/0.0.1-SNAPSHOT.jar

# JAR 파일을 컨테이너에 복사
COPY ${JAR_FILE} app.jar

# JAR 파일 실행
ENTRYPOINT ["java", "-jar", "-Dspring.profiles.active=dev", "-Duser.timezone=Asia/Seoul", "/app.jar"]
