package com.dropzone.declaration.repository;

import com.dropzone.declaration.entity.DeclarationChat;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface DeclarationRepository extends JpaRepository<DeclarationChat, Integer>{
    List<DeclarationChat> findAllByUserId(Long userId);
}
