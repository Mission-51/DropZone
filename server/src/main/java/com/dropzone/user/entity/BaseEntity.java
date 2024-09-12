package com.dropzone.user.entity;

import jakarta.persistence.Column;
import jakarta.persistence.EntityListeners;
import jakarta.persistence.MappedSuperclass;
import lombok.AllArgsConstructor;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.experimental.SuperBuilder;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.annotation.LastModifiedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import java.time.LocalDateTime;

@MappedSuperclass   // 이 클래스는 엔티티가 아닌 부모 클래스로, 테이블과 직접 매핑되지 않지만, 상속받는 자식 클래스에 공통 필드를 제공할 때 사용
@EntityListeners(AuditingEntityListener.class)  // 엔티티에 대해 이벤트가 발생할 때(Auditing 기능을 통해) 자동으로 createdAt과 updatedAt을 관리
@Getter  // Lombok을 사용하여 getter 메소드를 자동으로 생성
@NoArgsConstructor  // 기본 생성자 생성
@AllArgsConstructor  // 모든 필드를 인자로 받는 생성자 생성
@SuperBuilder  // 상속 구조에서 Builder 패턴을 사용할 수 있게 해주는 Lombok 어노테이션
public class BaseEntity {

    @CreatedDate  // 엔티티가 처음 저장될 때 생성일을 자동으로 저장
    @Column(updatable = false, name = "user_created_at", nullable = false)  // 데이터베이스의 "created_at" 컬럼에 매핑, 수정 불가 (최초 생성시만 값이 설정됨)
    private LocalDateTime userCreatedAt;
}
