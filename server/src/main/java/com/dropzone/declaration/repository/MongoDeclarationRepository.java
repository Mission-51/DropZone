package com.dropzone.declaration.repository;

import com.dropzone.declaration.entity.DeclarationChat;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface MongoDeclarationRepository extends MongoRepository<DeclarationChat, Integer> {
    List<DeclarationChat> findAllByUserId(Long userId);
}
