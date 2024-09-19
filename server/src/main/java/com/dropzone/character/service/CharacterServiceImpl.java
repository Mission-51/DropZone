package com.dropzone.character.service;

import com.dropzone.character.dto.CharacterDTO;
import com.dropzone.character.entity.CharacterEntity;
import com.dropzone.character.repository.CharacterRepository;
import jakarta.persistence.EntityNotFoundException;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
@RequiredArgsConstructor
public class CharacterServiceImpl implements CharacterService {

    @Autowired
    private CharacterRepository characterRepository;

    @Override
    public void createCharacter(CharacterDTO characterDTO) {
        CharacterEntity characterEntity = CharacterEntity.toSaveEntity(characterDTO);
        characterRepository.save(characterEntity);
    }

    @Override
    public CharacterDTO getCharacterById(Long characterId) {
        CharacterEntity characterEntity = characterRepository.findById(characterId)
                .orElseThrow(() -> new EntityNotFoundException("Character with id " + characterId + " not found"));
        return CharacterDTO.toCharacterDTO(characterEntity);
    }

    @Override
    public List<CharacterDTO> getAllCharacters() {
        List<CharacterEntity> characterEntities = characterRepository.findAll();
        return characterEntities.stream()
                .map(CharacterDTO::toCharacterDTO)
                .collect(Collectors.toList());
    }

    @Override
    public Character updateCharacterDTOFields(Character existingCharacter, CharacterDTO characterDTO) {
        return null;
    }

    @Transactional
    @Override
    public void updateCharacter(Long characterId, CharacterDTO characterDTO) {
        Optional<CharacterEntity> findCharacterEntity = characterRepository.findById(characterId);
        if (findCharacterEntity.isPresent()) {
            CharacterEntity updateCharacterEntity = findCharacterEntity.get();
            characterRepository.save(updateCharacterDTOFields(updateCharacterEntity, characterDTO));
        } else {
            throw new EntityNotFoundException("Character with id " + characterId + " not found");
        }
    }

    @Transactional
    @Override
    public CharacterEntity updateCharacterDTOFields(CharacterEntity updateCharacterEntity, CharacterDTO characterDTO) {
        if (characterDTO.getCharacterName() != null) {
            updateCharacterEntity.setCharacterName(characterDTO.getCharacterName());
        }
        if (characterDTO.getCharacterWeapon() != null) {
            updateCharacterEntity.setCharacterWeapon(characterDTO.getCharacterWeapon());
        }
        if (characterDTO.getCharacterValue() != 0) {
            updateCharacterEntity.setCharacterValue(characterDTO.getCharacterValue());
        }
        if (characterDTO.getCharacterStrength() != 0) {
            updateCharacterEntity.setCharacterStrength(characterDTO.getCharacterStrength());
        }
        if (characterDTO.getCharacterSpeed() != 0) {
            updateCharacterEntity.setCharacterSpeed(characterDTO.getCharacterSpeed());
        }
        if (characterDTO.getCharacterHP() != 0) {
            updateCharacterEntity.setCharacterHP(characterDTO.getCharacterHP());
        }
        if (characterDTO.getCharacterDefensive() != 0) {
            updateCharacterEntity.setCharacterDefensive(characterDTO.getCharacterDefensive());
        }
        if (characterDTO.getCharacterContent() != null) {
            updateCharacterEntity.setCharacterContent(characterDTO.getCharacterContent());
        }
        return updateCharacterEntity;
    }

    @Override
    public void deleteCharacter(Long characterId) {
        Optional<CharacterEntity> findCharacterEntity = characterRepository.findById(characterId);
        if (findCharacterEntity.isPresent()) {
            characterRepository.delete(findCharacterEntity.get());
        } else {
            throw new EntityNotFoundException("Character with id " + characterId + " not found");
        }
    }
}
