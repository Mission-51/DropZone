package com.dropzone.friend.entity;

import com.dropzone.user.entity.UserEntity;
import jakarta.persistence.*;
import lombok.*;
import org.springframework.data.annotation.CreatedDate;
import org.springframework.data.jpa.domain.support.AuditingEntityListener;

import java.time.LocalDateTime;

@Entity
@Getter
@Setter
@ToString
@Builder
@AllArgsConstructor
@NoArgsConstructor
@EntityListeners(AuditingEntityListener.class)
@Table(name = "friend_ship")
public class FriendShipEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id")
    private UserEntity user;

    @Column(name = "user_nickname")
    private String userNickname;

    @Column(name = "user_email")
    private String userEmail;

    @Column(name = "friend_nickname")
    private String friendNickname;

    @Column(name = "friend_email")
    private String friendEmail;

    @Column(name = "friend_ship_status")
    @Enumerated(EnumType.STRING)
    private FriendShipStatus status;

    @Column(name = "is_from")
    private boolean isFrom;

    @Column(name = "counter_part_id")
    private Long counterpartId;

    @CreatedDate
    @Column(name = "request_created_at")
    private LocalDateTime requestCreatedAt;
    
    // 수락할 시 상태 변경
    public void acceptFriendShipRequest() {
        this.status = FriendShipStatus.ACCEPT;
    }

}
