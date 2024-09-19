package com.dropzone.friend.entity;

import com.dropzone.user.entity.UserEntity;
import jakarta.persistence.*;
import lombok.*;

@Entity
@Getter
@ToString
@AllArgsConstructor
@NoArgsConstructor
public class FriendShipEntity {

    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long id;

    @ManyToOne
    @JoinColumn(name = "requester_id")
    private UserEntity requester;

    @ManyToOne
    @JoinColumn(name = "receiver_id")
    private UserEntity receiver;


}
