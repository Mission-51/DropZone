package com.dropzone.friend.repository;

import com.dropzone.friend.entity.FriendShipEntity;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;


@Repository
public interface FriendShipRepository extends JpaRepository<FriendShipEntity, Long> {

}
