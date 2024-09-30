using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class ItemData : ScriptableObject
{
    public enum ItemType // 아이템 유형
    {
        Used,
        Heal,
        Money,
    }

    [Header("아이템 Id")]
    public int id;
    [Header("아이템 유형")]
    public ItemType itemType;
    [Header("쿨타임")]
    public int coolDownTime;
    [Header("아이템 이름")]
    public string itemName;
    [Header("아이템 설명")]
    [Multiline]
    public string toolTip;
    [Header("아이템 이미지")]
    public Sprite itemImage;
    [Header("아이템 프리펩")]
    public GameObject itemPrefab;
}
