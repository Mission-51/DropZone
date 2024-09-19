package com.dropzone.character.entity;

import jakarta.persistence.*;


@Entity
@Table(name = "character")
public class CharacterEntity {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long character_Id;

    @Column(nullable = false)
    private String character_Name;

    @Column(nullable = false)
    private String character_Weapon;

    @Column(nullable = false)
    private int character_Value;

    @Column(nullable = false)
    private int character_Strength;

    @Column(nullable = false)
    private int character_Speed;

    @Column(nullable = false)
    private int character_HP;

    @Column(nullable = false)
    private int character_Defensive;

    @Column(nullable = false, columnDefinition = "TEXT")
    private String character_Content;

    // 연관 관계 설정 - character_list와의 일대다 관계
//    @OneToMany(mappedBy = "character")
//    private List<CharacterList> characterLists;
}
