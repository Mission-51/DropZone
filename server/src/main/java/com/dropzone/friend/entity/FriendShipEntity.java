package com.dropzone.friend.entity;

import com.dropzone.user.entity.UserEntity;
import jakarta.persistence.*;
import lombok.*;
import org.springframework.data.annotation.CreatedDate;

import java.time.LocalDateTime;

@Entity
@Getter
@Setter
@ToString
@Builder
@AllArgsConstructor
@NoArgsConstructor
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
    private FriendShipStatus status;

    @Column(name = "is_from")
    private boolean isFrom;

    @Column(name = "counter_part_id")
    private Long counterpartId;

    @CreatedDate
    @Column(name = "request_created_at")
    private LocalDateTime requestCreatedAt;

    public void acceptFriendShipRequest() {
        this.status = FriendShipStatus.ACCEPT;
    }

}
