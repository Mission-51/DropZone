# 📕 Project Skill Stack Version

| **Skill**  | **Version** |
| --- | --- |
| Java | OpenJDK 17 |
| SpringBoot | 3.3.2 |
| gradle | 8.5 |
| MySQL | 8.0.20 |
| MongoDB | 6.0 |
| Nginx | 1.18.0(Ubuntu) |
| Jenkins | 2.475 |
| Docker | 27.3.1 |

<br>

# 📘 사용 도구


- 이슈 관리 : Jira
- 형상 관리 : GitLab
- 커뮤니케이션 : Notion, Mattermost, Discord, KakaoTalk
- CI/CD : Jenkins, Docker, DockerHub

<br>

# 📙 개발 도구


- Unity: 2021.3.9F
- Visual Studio Code : 1.90.2
- IntelliJ : IDEA 2024.1.4 (Ultimate Edition)

# 📗 EC2 포트 번호

<br>

- **Backend**: `[백엔드 포트 번호 입력]`
- **MySQL**: `[MySQL 포트 번호 입력]`
- **MongoDB**: `[MongoDB 포트 번호 입력]`

<br>

# 🧿 환경 변수


- **jasypt.encryptor.key**: `[암호화 키 입력]`

<br>

# 📖 CI/CD 구축


백엔드 서버부터 구축

<br>

# Swap 메모리 설정

여러 빌드 동시처리시 물리적 메모리가 가득 찼을때 추가 작업을 위한 swap 메모리 설정

```bash
스왑 메모리 설정
// swap 파일을 생성해준다. 
// (메모리 상태 확인 시 swap이 있었지만 디렉토리 파일은 만들어줘야한다.)
sudo mkdir /var/spool/swap
sudo touch /var/spool/swap/swapfile
sudo dd if=/dev/zero of=/var/spool/swap/swapfile count=4096000 bs=1024

// swap 파일을 설정한다.
sudo chmod 600 /var/spool/swap/swapfile
sudo mkswap /var/spool/swap/swapfile
sudo swapon /var/spool/swap/swapfile

// swap 파일을 등록한다.
sudo echo '/var/spool/swap/swapfile none swap defaults 0 0' | sudo tee -a /etc/fstab

// 메모리 상태 확인
free -h
```

<br>

# JDK 설치

17로 진행

```bash
# 업데이트
sudo apt update

# 업그레이드
sudo apt upgrade

# 특정 버전 목록 조회
sudo apt list openjdk-17

# 설치
sudo apt install openjdk-17-jdk

# 설치 확인
java --version
```

<br>

# Docker 설치

```bash
# 의존성 설치
sudo apt update
sudo apt install ca-certificates curl gnupg lsb-release

# 레포지토리
sudo mkdir -p /etc/apt/keyrings
sudo curl -fsSL https://download.docker.com/linux/debian/gpg | sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg

# 레포지토리 추가
echo "deb [arch=$(dpkg --print-architecture) \
signed-by=/etc/apt/keyrings/docker.gpg] https://download.docker.com/linux/ubuntu \
$(. /etc/os-release && echo "$VERSION_CODENAME") stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

# 도커 설치하기
sudo apt update
sudo apt install docker-ce docker-ce-cli containerd.io docker-compose-plugin
```

<br>

# Jenkins 설치

Docker outside of Docker (DooD)방식으로 진행

젠킨스에서 도커 기반의 빌드,테스트 환경을 직접 관리

```bash
# 도커 소켓 마운트하여 Jenkins 컨테이너에서 Docker 명령어 실행 가능하도록 설정
docker run -itd --name jenkins -p 9005:8080 -v /var/run/docker.sock:/var/run/docker.sock -v /usr/bin/docker:/usr/bin/docker jenkins/jenkins:jdk21

# 만약 Jenkins에서 Docker 명령어가 실행되지 않거나 권한 오류가 발생하면, 아래 명령어로 권한 수정
sudo chmod 666 /var/run/docker.sock

# Jenkins 컨테이너의 초기 관리자 비밀번호 확인 명령어
docker exec jenkins cat /var/jenkins_home/secrets/initialAdminPassword

# Jenkins 컨테이너에 접속하여 Docker 명령어 실행 여부 확인
docker exec -it <컨테이너_이름_또는_ID> /bin/bash
docker exec -it jenkins /bin/bash

# Jenkins 컨테이너에 접속한 후 Docker 명령어가 작동하는지 확인
docker
```

접속 후 테스트

<br>

# Nginx 설치

```bash
# 패키지 목록 업데이트
sudo apt update

# 시스템 패키지 업그레이드
sudo apt upgrade

# Nginx 설치
sudo apt install nginx

# Nginx 서비스 시작
sudo service nginx start

# Nginx 서비스 상태 확인
sudo service nginx status
```

<br>

# https 설정 (SSL)

무료 Let’s Encrypt

```bash
# Encrypt 설치
sudo apt-get install letsencrypt

# Certbot 설치
sudo apt-get install certbot python3-certbot-nginx

# Certbot 실행 (nginx 중지 후 실행해야 함)
sudo certbot --nginx

# 1번 방법: 도메인 혹은 IP 주소로 SSL 인증서 발급
sudo certbot --nginx -d [도메인 혹은 IP 주소]

# 2번 방법: Standalone 방식으로 SSL 인증서 발급
sudo letsencrypt certonly --standalone -d [도메인 혹은 IP 주소]

# Certbot 실행 후 옵션에서 1번 선택
# 강제 리다이렉트 설정 부분에서 http와 https를 모두 사용하기 위해 '아니오' 선택 (1번)

# Nginx 설정 적용
sudo service nginx restart
sudo systemctl reload nginx
```

<br>

# Jenkins, gitLab webhook 설정

깃랩 토큰 발급 → 젠킨스 플러그인 등록(gitlab) → 젠킨스에 API token Credentials 등록 → 연결확인

<br>

# Jenkins pipline 생성

클론을 하기 위한 기본 코드 부터 작성

```bash
pipeline {
    agent any
    
    stages {
        stage('Git Clone') {
            steps {
                git branch: 'main', credentialsId: '2d3de7ca-717d-4ee5-b413-70fa3069fb63', url: 'https://lab.ssafy.com/s11-metaverse-game-sub1/S11P21D110'
            }
            post {
                failure {
                    echo 'Repository clone 실패!'
                }
                success {
                    echo 'Repository clone 성공!'
                }
            }
        }
        stage('Build') {
            steps {
                // 프로젝트 권한 변경
                sh 'chmod +x ./server/gradlew'
                // 프로젝트 빌드
                withCredentials([string(credentialsId: 'JASYPT_KEY', variable: 'JASYPT_KEY')]) {
                    sh 'cd ./server && ./gradlew clean build -PJASYPT_KEY=$JASYPT_KEY'
                }
            }
        }
        stage('Docker Hub Login') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'DOCKER_USER', passwordVariable: 'DOCKER_PASSWORD', usernameVariable: 'DOCKER_USERNAME')]) {
                    sh 'echo "$DOCKER_PASSWORD" | docker login -u $DOCKER_USERNAME --password-stdin'
                }
            }
        }
        stage('Docker Build and Push') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'DOCKER_REPO', passwordVariable: 'DOCKER_PROJECT', usernameVariable: 'DOCKER_REPO')]) {
                    sh 'cd ./server && docker build -f Dockerfile -t $DOCKER_REPO/$DOCKER_PROJECT .'
                    sh 'cd ./server && docker push $DOCKER_REPO/$DOCKER_PROJECT'
                    echo 'docker push 성공!!'
                }
                echo 'docker push 성공!!'
            }
        }
        stage('Deploy') {
            steps {
                sshagent(credentials: ['my-ssh-credentials']) {
                    withCredentials([string(credentialsId: 'EC2_SERVER_IP', variable: 'IP')]) {
                        sh 'ssh -o StrictHostKeyChecking=no ubuntu@$IP "sudo sh deploy.sh"'
                    }
                }
            }
        }
    }
}

```

<br>

# 깃랩 웹훅 등록

URL, Secret Token, Trigger 작성

<br>

# Docker Hub Setting

로그인 후 컨테이너 생성

<br>

# Jenkins Credental Setting

- JASYPT → Secret text
- Docker Hub → Usernaem with password
- EC2 Server IP → Secret text

<br>

# SSH 접속설정

plugin 추가(SSH Agent Plugin)

Jenkins Credentials - .pem키 복사붙여넣기

<br>

# Nginx 설정 변경

무중단 배포 (Blue-Green) 로 진행

무중단 배포 경로를 잡기 위한 [service-url.inc](http://service-url.inc), Deploy File 따로 작성

/api 밑으로 들어오면 8080,8081로 연결(socket통신 포함)

<br>

## nginx.conf

```bash
user www-data;
worker_processes auto;
pid /run/nginx.pid;
include /etc/nginx/modules-enabled/*.conf;

events {
    worker_connections 768;
    # multi_accept on;
}

http {

    ##
    # 기본 설정
    ##

    sendfile on;  # 파일 전송을 효율적으로 처리
    tcp_nopush on;  # TCP 패킷 푸시를 지연시켜 전송 성능 향상
    tcp_nodelay on;  # 작은 패킷들을 즉시 전송
    keepalive_timeout 65;  # 연결 유지 시간 설정
    types_hash_max_size 2048;  # MIME 타입 해시 테이블 크기 설정
    # server_tokens off;  # 서버 버전 정보를 숨김

    # server_names_hash_bucket_size 64;
    # server_name_in_redirect off;

    include /etc/nginx/mime.types;  # MIME 타입 파일 포함
    default_type application/octet-stream;  # 기본 파일 유형 설정

    ##
    # SSL 설정
    ##

    ssl_protocols TLSv1 TLSv1.1 TLSv1.2 TLSv1.3;  # SSLv3를 제외한 SSL/TLS 프로토콜 설정
    ssl_prefer_server_ciphers on;  # 서버가 우선적으로 사용할 암호 설정

    ##
    # 로그 설정
    ##

    access_log /var/log/nginx/access.log;  # 접근 로그 경로
    error_log /var/log/nginx/error.log;  # 에러 로그 경로

    ##
    # Gzip 설정
    ##

    gzip on;  # Gzip 압축 활성화

    # gzip_vary on;
    # gzip_proxied any;
    # gzip_comp_level 6;
    # gzip_buffers 16 8k;
    # gzip_http_version 1.1;
    # gzip_types text/plain text/css application/json application/javascript text/xml application/xml application/xml+rss text/javascript;

    ##
    # 가상 호스트 설정
    ##

    include /etc/nginx/conf.d/*.conf;  # conf.d 폴더의 모든 설정 파일 포함
    include /etc/nginx/sites-enabled/*;  # sites-enabled 폴더의 모든 설정 파일 포함
}

#mail {
#       # 예제 인증 스크립트는 다음을 참조하십시오:
#       # http://wiki.nginx.org/ImapAuthenticateWithApachePhpScript
#
#       # auth_http localhost/auth.php;
#       # pop3_capabilities "TOP" "USER";
#       # imap_capabilities "IMAP4rev1" "UIDPLUS";
#
#       server {
#               listen     localhost:110;
#               protocol   pop3;
#               proxy      on;
#       }
#
#       server {
#               listen     localhost:143;
#               protocol   imap;
#               proxy      on;
#       }
#}

```

<br>

## nginx/sites-enabled/default

```bash
server {
    listen 80 default_server;
    listen [::]:80 default_server;

    # SSL 설정
    #
    # listen 443 ssl default_server;
    # listen [::]:443 ssl default_server;
    #
    # 참고: SSL 트래픽에 대해 gzip을 비활성화해야 합니다.
    # 자세한 내용은 https://bugs.debian.org/773332를 참조하세요.
    #
    # 보안 구성을 위해 ssl_ciphers에 대해 읽어보세요.
    # 자세한 내용은 https://bugs.debian.org/765782를 참조하세요.
    #
    # ssl-cert 패키지로 생성된 자체 서명 인증서는
    # 실제 운영 서버에서는 사용하지 마세요!
    #
    # include snippets/snakeoil.conf;

    root /var/www/html;

    # PHP를 사용하는 경우 index.php를 목록에 추가
    index index.html index.htm index.nginx-debian.html;

    server_name [서버 이름 입력];

    # HTTP 요청을 HTTPS로 리다이렉트
    return 301 https://$host$request_uri;

    # PHP 스크립트를 FastCGI 서버로 전달하는 부분 (주석처리됨)
    #location ~ \.php$ {
    #       include snippets/fastcgi-php.conf;
    #
    #       # php-fpm (또는 다른 유닉스 소켓을 사용하는 경우):
    #       fastcgi_pass unix:/var/run/php/php7.4-fpm.sock;
    #       # php-cgi (또는 다른 tcp 소켓을 사용하는 경우):
    #       fastcgi_pass 127.0.0.1:9000;
    #}

    # .htaccess 파일에 대한 접근을 차단하는 부분 (Apache와의 문서 루트 충돌 방지)
    #location ~ /\.ht {
    #       deny all;
    #}
}

server {
    listen [::]:443 ssl ipv6only=on; # Certbot에서 관리
    listen 443 ssl; # Certbot에서 관리
    server_name [서버 이름 입력];

    ssl_certificate /etc/letsencrypt/live/[서버 이름]/fullchain.pem; # Certbot에서 관리
    ssl_certificate_key /etc/letsencrypt/live/[서버 이름]/privkey.pem; # Certbot에서 관리
    include /etc/letsencrypt/options-ssl-nginx.conf; # Certbot에서 관리
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem;

    include /etc/nginx/conf.d/service-url.inc;

    location / {
        return 403;  # 루트 디렉토리에 대한 접근을 차단
    }

    location /ws {
        proxy_pass $service_url;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "Upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_read_timeout 60m;
        proxy_send_timeout 60m;
    }

    location /api {
        proxy_pass $service_url;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_read_timeout 20m;
    }

    location /swagger-ui/ {
        proxy_pass $service_url;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }

    location /v3/api-docs {
        proxy_pass $service_url;
        proxy_http_version 1.1;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}

```

<br>

# service_url.inc

```bash
set $service_url http://127.0.0.1:8080;
```

<br>

# Jenkins pipline BE 작성

```bash
pipeline {
    agent any
    
    tools {
        jdk ("jdk17")  // JDK 17 사용
    }
    
    stages {
        stage('Git Clone') {
            steps {
                git branch: 'BE', credentialsId: 'GitLab_Login', url: 'https://[gitlabUrl]/[Project Directory]/[ProjectName].git'
            }
            post {
                failure {
                    echo 'Repository clone 실패 !'
                }
                success {
                    echo 'Repository clone 성공 !'
                }
            }
        }
        stage('Build') {
            steps {
                // 프로젝트 권한 변경
                sh 'chmod +x ./backEnd/gradlew'
                // 프로젝트 빌드
                withCredentials([string(credentialsId: 'JASYPT_KEY', variable: 'JASYPT_KEY')]) {
                    sh 'cd ./backEnd && ./gradlew clean build -PJASYPT_KEY=$JASYPT_KEY'
                }
            }
        }
        stage('Docker Hub Login') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'DOCKER_USER', passwordVariable: 'DOCKER_PASSWORD', usernameVariable: 'DOCKER_USERNAME')]) {
                    sh 'echo "$DOCKER_PASSWORD" | docker login -u $DOCKER_USERNAME --password-stdin'
                }
            }
        }
        stage('Docker Build and Push') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'DOCKER_HUB', passwordVariable: 'DOCKER_PROJECT', usernameVariable: 'DOCKER_REPO')]) {
                    sh 'cd ./backEnd && docker build -f Dockerfile -t $DOCKER_REPO/$DOCKER_PROJECT .'
                    sh 'cd ./backEnd && docker push $DOCKER_REPO/$DOCKER_PROJECT'
                    echo 'docker push 성공!!'
                }
                echo 'docker push 성공!!'
            }
        }
        stage('Deploy') {
            steps {
                sshagent(credentials: ['my-ssh-credentials']) {
                    withCredentials([string(credentialsId: 'EC2_SERVER_IP', variable: 'IP')]) {
                        sh 'ssh -o StrictHostKeyChecking=no ubuntu@$IP "sudo sh deploy.sh"'
                    }
                }
            }
        }
        stage('Notification') {
            steps {
                echo 'Jenkins 알림!'
            }
            post {
                success {
                    script {
                        def Author_ID = sh(script: "git show -s --pretty=%an", returnStdout: true).trim()
                        def Author_Name = sh(script: "git show -s --pretty=%ae", returnStdout: true).trim()
                        mattermostSend(color: 'good',
                            message: "백엔드 빌드 성공: ${env.JOB_NAME} #${env.BUILD_NUMBER} by ${Author_ID}(${Author_Name})\n(<${env.BUILD_URL}|Details>)",
                            endpoint: 'https://meeting.ssafy.com/hooks/uysnnytbyfymbdest5out5t9uy',
                            channel: 'Circus'
                        )
                    }
                }
                failure {
                    script {
                        def Author_ID = sh(script: "git show -s --pretty=%an", returnStdout: true).trim()
                        def Author_Name = sh(script: "git show -s --pretty=%ae", returnStdout: true).trim()
                        mattermostSend(color: 'danger',
                            message: "백엔드 빌드 실패: ${env.JOB_NAME} #${env.BUILD_NUMBER} by ${Author_ID}(${Author_Name})\n(<${env.BUILD_URL}|Details>)",
                            endpoint: 'https://meeting.ssafy.com/hooks/uysnnytbyfymbdest5out5t9uy',
                            channel: 'Circus'
                        )
                    }
                }
            }
        }
    }
}

```

<br>

# Deploy File 작성

```bash
# 1. 기존 컨테이너 확인 및 실행
EXIST_GITCHAN=$(sudo docker compose -p dropzone-8080 -f docker-compose.dropzone8080.yml ps | grep Up)

if [ -z "$EXIST_GITCHAN" ]; then
    echo "8080 컨테이너 실행"
    sudo docker compose -p dropzone-8080 -f /home/ubuntu/docker-compose.dropzone8080.yml up -d --force-recreate
    BEFORE_COLOR="8081"
    AFTER_COLOR="8080"
    BEFORE_PORT=8081
    AFTER_PORT=8080
else
    echo "8081 컨테이너 실행"
    sudo docker compose -p dropzone-8081 -f /home/ubuntu/docker-compose.dropzone8081.yml up -d --force-recreate
    BEFORE_COLOR="8080"
    AFTER_COLOR="8081"
    BEFORE_PORT=8080
    AFTER_PORT=8081
fi

echo "${AFTER_COLOR} 서버 실행 (포트: ${AFTER_PORT})"

# 2. 서버 응답 확인
for cnt in `seq 1 10`; do
    echo "서버 응답 확인 중 (${cnt}/10)"
    UP=$(curl -s http://127.0.0.1:${AFTER_PORT}/api/health-check)
    if [ "${UP}" != "OK" ]; then
        sleep 10
        continue
    else
        break
    fi
done

if [ $cnt -eq 10 ]; then
    echo "서버에 문제가 발생했습니다..."
    exit 1
fi

# 3. Nginx 설정 변경 및 재시작
sudo sed -i "s/${BEFORE_PORT}/${AFTER_PORT}/" /etc/nginx/conf.d/service-url.inc
sudo nginx -s reload
echo "배포 완료!!"

# 4. 이전 서버 종료
echo "$BEFORE_COLOR 서버 종료 (포트: ${BEFORE_PORT})"
sudo docker compose -p dropzone-${BEFORE_COLOR} -f docker-compose.dropzone${BEFORE_COLOR}.yml down

# 5. 사용하지 않는 이미지 삭제
sudo docker image prune -f
```

<br>

# Docker-compose File 작성

## docker-compose.dropzone8080.yml

```bash
version: '3.1'

services:
  api:
    image: khg9055/dropzone:latest  # 사용할 Docker 이미지 (최신 버전)
    container_name: dropzone-8080  # 컨테이너 이름 설정
    environment:
      - TZ=Asia/Seoul  # 시간대 설정 (한국 표준시)
      - LANG=ko_KR.UTF-8  # 시스템 언어 설정 (한국어)
      - HTTP_PORT=8080  # API 서비스가 사용할 포트 번호
      - jasypt.encryptor.key=${JASYPT_KEY}  # Jasypt 암호화 키 환경 변수
    ports:
      - "8080:8080"  # 외부와 내부 모두 8080 포트를 사용하여 API 서비스에 접근
    networks:
      - dropzone  # 사용할 네트워크 지정

networks:
  dropzone:
    external: true  # 외부 네트워크 사용
```

## docker-compose.dropzone8081.yml

```bash
version: '3.1'

services:
  api:
    image: khg9055/dropzone:latest  # 사용할 Docker 이미지 (최신 버전)
    container_name: dropzone-8081  # 컨테이너 이름 설정
    environment:
      - TZ=Asia/Seoul  # 시간대 설정 (한국 표준시)
      - LANG=ko_KR.UTF-8  # 시스템 언어 설정 (한국어)
      - HTTP_PORT=8081  # API 서비스가 사용할 포트 번호
      - jasypt.encryptor.key=${JASYPT_KEY}  # Jasypt 암호화 키 환경 변수
    ports:
      - "8081:8080"  # 외부에서 8081 포트로 API 서비스에 접근, 내부에서는 8080 사용
    networks:
      - dropzone  # 사용할 네트워크 지정

networks:
  dropzone:
    external: true  # 외부 네트워크 사용
```

<br>

## docker-compose.yml 작성

```yaml
version: '3.1'
services:
  mysql:
    image: mysql:8.0.20
    container_name: [MySQL 컨테이너 이름]
    restart: always
    volumes:
      - /home/ubuntu/mysqldata:/var/lib/mysql  # MySQL 데이터가 저장될 경로
    environment:
      - MYSQL_ROOT_PASSWORD=[MySQL 루트 비밀번호 입력]
      - MYSQL_DATABASE=[생성할 데이터베이스 이름 입력]
      - TZ=Asia/Seoul  # 시간대 설정
    ports:
      - "[외부 포트]:3306"  # MySQL 서비스에 접근할 포트
    networks:
      - dropzone

  mongo:
    image: mongo:6.0
    container_name: [MongoDB 컨테이너 이름]
    restart: always
    ports:
      - "[외부 포트]:27017"  # MongoDB 서비스에 접근할 포트
    environment:
      - TZ=Asia/Seoul  # 시간대 설정
    volumes:
      - /home/ubuntu/mongo_data:/data/db  # MongoDB 데이터가 저장될 경로
    networks:
      - dropzone

networks:
  dropzone:
    external: true  # 외부 네트워크 사용

volumes:
  mysql_data:
  mongo_data:
```

<br>

# application.yml 작성

```yaml
spring:
  datasource:
    url: jdbc:mysql://[서버 주소]:[포트 번호]/[데이터베이스명]?serverTimezone=Asia/Seoul
    username: [DB 사용자명 입력]
    password: [DB 비밀번호 입력]
    driver-class-name: com.mysql.cj.jdbc.Driver
  mail:
    host: smtp.gmail.com
    port: 587
    username: [이메일 사용자명 입력]
    password: [이메일 비밀번호 입력]
    properties:
      mail:
        smtp:
          auth: true
          timeout: 5000
          starttls:
            enable: true

  jpa:
    hibernate:
      ddl-auto: update  # 서버 환경에 맞게 테이블 검증
    properties:
      hibernate:
        dialect: org.hibernate.dialect.MySQLDialect  # MySQL용 Hibernate Dialect 추가
        format_sql: true
        jdbc:
          lob:
            non_contextual_creation: true
    show-sql: true  # SQL 쿼리 로깅

  data:
    mongodb:
      uri: mongodb://[MongoDB 서버 주소]:[포트 번호]/[데이터베이스명]
      auto-index-creation: true
    redis:
      host: [Redis 서버 주소 입력]
      port: 6379

  thymeleaf:
    cache: false  # Thymeleaf 캐시 비활성화

jasypt:
  encryptor:
    bean: jasyptStringEncryptor
    key: ${jasypt.encryptor.key}  # Jasypt 암호화 키 설정
```

<br>

# 유니티 환경 구축

## Unity 2021.3.9f1 환경 구축 매뉴얼

1. 개발 환경 준비
    1. Unity 버전
        - **Unity 2021.3.9f1** 사용.
        - Unity Hub를 통해 해당 버전을 설치.

    2. Unity Hub
        - 유니티 프로젝트 관리를 위해 Unity Hub 사용.
        - Unity Hub에서 프로젝트 생성 및 유니티 버전 관리.

    3. Visual Studio 2022
        - 코드 작성 및 디버깅을 위해 Visual Studio 2022 설치.
        - Unity와 연동하여 C# 스크립트 작성.

2. 프로젝트 설정
    1. 프로젝트 생성
        - Unity Hub에서 **새 프로젝트**를 생성.
        - 템플릿으로 **3D** 또는 **URP** 선택.
    2. 유니티 패키지 설치
    3. Photon PUN2: 멀티플레이어 기능을 위해 **Asset Store** 또는 **Package Manager**에서 Photon PUN2 설치.
        - Unity 상단 메뉴에서 `Window > Package Manager` 선택.
        - Photon PUN2 패키지를 검색하여 설치.
3. 플랫폼 설정
    1. 빌드 플랫폼 설정
        - 상단 메뉴에서 `File > Build Settings`로 이동.
        - 개발할 플랫폼(Windows, Intel 64-bit)을 선택.

4. Photon PUN2 환경 설정
    1. Photon App ID 등록
        - [Photon Dashboard](https://dashboard.photonengine.com/)에서 계정 생성.
        - 새로운 어플리케이션 등록 후, App ID 발급.
        - Unity에서 `Window > Photon Unity Networking`을 선택.
        - `PhotonServerSettings` 창에서 발급받은 App ID를 입력.
    2. 서버 설정
        - Photon PUN2에서 사용할 서버 위치 KR 설정

5. 기본 설정 완료 후 빌드
    1. 모든 설정이 완료되면 상단 메뉴에서 `File > Build Settings`로 이동.
    2. 빌드 타겟을 설정하고 프로젝트 빌드.
    3. 빌드 전 **Player Settings**에서 팀 명, 프로젝트 이름, 아이콘 등의 설정을 완료.

<br>

# 프로젝트에서 사용하는 외부 서비스들 (소셜인증, 포톤 클라우드, 코드 컴파일 등)

## 이메일 인증 기능 (Email Verification Feature)

1. 기능 설명 (Feature Overview)
    - 이메일 인증 기능은 회원 가입 시 사용자가 입력한 이메일 주소가 유효한지 확인하기 위해 사용.
    - 인증을 완료해야만 사용자는 계정을 활성화하고 로그인 가능.

2. 사용 목적 (Purpose)
    - 이메일 인증을 통해 유효한 사용자만이 서비스에 접근할 수 있도록 하여 보안을 강화.
    - 잘못된 이메일 주소를 입력하는 사용자를 방지하고, 스팸 계정 생성을 억제.

3. 프로세스 (Process)
    1. **회원 가입**: 사용자가 이메일 주소를 입력하고 회원 가입을 요청하면, 서버에서 이메일 인증 코드를 생성.
    2. **인증 이메일 발송**: 사용자가 입력한 이메일 주소로 인증 코드가 포함된 이메일이 전송. 이때, 사용자가 인증 코드를 이용하여 서비스 내에서 입력할 수 있도록 설정.
    3. **인증 코드 입력**: 사용자는 이메일로 받은 인증 코드를 입력하고 서버에서 이를 검증하여 이메일 주소를 인증 완료 처리.
    4. **계정 활성화**: 인증이 완료되면, 사용자의 계정이 활성화되어 로그인 및 서비스 이용이 가능.

4. 기술 스택 (Technology Stack)
    - **이메일 발송**: JavaMailSender (Spring Boot에서 제공)
    - **코드 관리**: ConcurrentHashMap을 사용하여 이메일과 인증 코드를 매칭하여 관리
    - **인증 코드 입력 및 검증**: 사용자로부터 입력된 인증 코드를 서버에서 비교하여 처리하는 방식

5. 설정 및 구성 (Configuration)
    - SMTP 서버 설정: 이메일 전송을 위해 SMTP 서버 정보가 필요.
    - 프로젝트의 `application.yml` 파일에 아래와 같은 설정을 추가
    
        ```
        spring:
        mail:
            host: [SMTP 서버 주소 입력]
            port: [SMTP 포트 번호 입력]
            username: [SMTP 사용자명 입력]
            password: [SMTP 비밀번호 입력]
            properties:
            mail:
                smtp:
                auth: true
                timeout: 5000
                starttls:
                    enable: true
        ```
    

6. 에러 처리 (Error Handling)
    - 만약 사용자가 잘못된 인증 코드를 입력할 경우 적절한 에러 메시지를 제공하고 재발송 옵션을 제공.