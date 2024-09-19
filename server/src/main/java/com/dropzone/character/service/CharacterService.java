package com.dropzone.character.service;

import com.dropzone.character.dto.CharacterDTO;
import com.dropzone.character.entity.CharacterEntity;

import java.util.List;

public interface CharacterService {

    // 캐릭터 생성 메소드
    void createCharacter(CharacterDTO characterDTO);

    // ID로 특정 캐릭터 조회
    CharacterDTO getCharacterById(Long characterId);

    // 캐릭터 정보 수정
    void updateCharacter(Long characterId, CharacterDTO characterDTO);

    // 캐릭터 삭제
    void deleteCharacter(Long characterId);

    // 전체 캐릭터 목록 조회
    List<CharacterDTO> getAllCharacters();

    // 캐릭터 정보 DTO로 엔티티 업데이트 (실제 필드 값 비교 및 업데이트)
    Character updateCharacterDTOFields(Character existingCharacter, CharacterDTO characterDTO);
}
