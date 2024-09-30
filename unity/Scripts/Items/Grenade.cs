using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Grenade")]
public class Grenade : ItemData
{
    [Header("폭발 범위")]
    [Range(0, 10)]
    public float explosionRange;
    [Header("대미지")]
    public int damage;
}
