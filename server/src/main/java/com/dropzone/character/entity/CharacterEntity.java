package com.dropzone.character.entity;

import com.dropzone.character.dto.CharacterDTO;
import jakarta.persistence.*;
import lombok.Getter;
import lombok.Setter;

@Getter
@Setter
@Entity
@Table(name = "`character`")
public class CharacterEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    @Column(name = "character_id")
    private Long characterId;

    @Column(name = "character_name", nullable = false)
    private String characterName;

    @Column(name = "character_weapon", nullable = false)
    private String characterWeapon;

    @Column(name = "character_value", nullable = false)
    private int characterValue;

    @Column(name = "character_strength", nullable = false)
    private int characterStrength;

    @Column(name = "character_speed", nullable = false)
    private int characterSpeed;

    @Column(name = "character_hp", nullable = false)
    private int characterHP;

    @Column(name = "character_defensive", nullable = false)
    private int characterDefensive;

    @Column(name = "character_content", nullable = false, columnDefinition = "TEXT")
    private String characterContent;


    public static CharacterEntity toSaveEntity(CharacterDTO characterDTO) {
        CharacterEntity characterEntity = new CharacterEntity();
        characterEntity.setCharacterId(characterDTO.getCharacterId());
        characterEntity.setCharacterName(characterDTO.getCharacterName());
        characterEntity.setCharacterWeapon(characterDTO.getCharacterWeapon());
        characterEntity.setCharacterValue(characterDTO.getCharacterValue());
        characterEntity.setCharacterStrength(characterDTO.getCharacterStrength());
        characterEntity.setCharacterSpeed(characterDTO.getCharacterSpeed());
        characterEntity.setCharacterHP(characterDTO.getCharacterHP());
        characterEntity.setCharacterDefensive(characterDTO.getCharacterDefensive());
        characterEntity.setCharacterContent(characterDTO.getCharacterContent());

        return characterEntity;
    }

    // 연관 관계 설정 - character_list와의 일대다 관계
//    @OneToMany(mappedBy = "character")
//    private List<CharacterList> characterLists;
}
