package com.dropzone.character.dto;

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

    public Long getCharacterId() {
        return characterId;
    }

    public void setCharacterId(Long characterId) {
        this.characterId = characterId;
    }

    public String getCharacterName() {
        return characterName;
    }

    public void setCharacterName(String characterName) {
        this.characterName = characterName;
    }

    public String getCharacterWeapon() {
        return characterWeapon;
    }

    public void setCharacterWeapon(String characterWeapon) {
        this.characterWeapon = characterWeapon;
    }

    public int getCharacterValue() {
        return characterValue;
    }

    public void setCharacterValue(int characterValue) {
        this.characterValue = characterValue;
    }

    public int getCharacterStrength() {
        return characterStrength;
    }

    public void setCharacterStrength(int characterStrength) {
        this.characterStrength = characterStrength;
    }

    public int getCharacterSpeed() {
        return characterSpeed;
    }

    public void setCharacterSpeed(int characterSpeed) {
        this.characterSpeed = characterSpeed;
    }

    public int getCharacterHP() {
        return characterHP;
    }

    public void setCharacterHP(int characterHP) {
        this.characterHP = characterHP;
    }

    public int getCharacterDefensive() {
        return characterDefensive;
    }

    public void setCharacterDefensive(int characterDefensive) {
        this.characterDefensive = characterDefensive;
    }

    public String getCharacterContent() {
        return characterContent;
    }

    public void setCharacterContent(String characterContent) {
        this.characterContent = characterContent;
    }
}
