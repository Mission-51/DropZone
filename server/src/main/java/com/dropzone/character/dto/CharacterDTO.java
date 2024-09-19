package com.dropzone.character.dto;

import com.dropzone.character.entity.CharacterEntity;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
public class CharacterDTO {

    private Long characterId;
    private String characterName;
    private String characterWeapon;
    private int characterValue;
    private int characterStrength;
    private int characterSpeed;
    private int characterHP;
    private int characterDefensive;
    private String characterContent;

    // 생성자, getter 및 setter

    public CharacterDTO() {
    }

    public static CharacterDTO toCharacterDTO(CharacterEntity characterEntity) {
        CharacterDTO characterDTO = new CharacterDTO();
        characterDTO.setCharacterId(characterEntity.getCharacterId());
        characterDTO.setCharacterName(characterEntity.getCharacterName());
        characterDTO.setCharacterWeapon(characterDTO.getCharacterWeapon());
        characterDTO.setCharacterValue(characterEntity.getCharacterValue());
        characterDTO.setCharacterStrength(characterEntity.getCharacterStrength());
        characterDTO.setCharacterSpeed(characterEntity.getCharacterSpeed());
        characterDTO.setCharacterHP(characterEntity.getCharacterHP());
        characterDTO.setCharacterDefensive(characterEntity.getCharacterDefensive());
        characterDTO.setCharacterContent(characterEntity.getCharacterContent());
        return characterDTO;
    }

}