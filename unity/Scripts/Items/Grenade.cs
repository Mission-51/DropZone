using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Grenade")]
public class Grenade : ItemData
{
    [Header("Æø¹ß ¹üÀ§")]
    [Range(0, 10)]
    public float explosionRange;
    [Header("´ë¹ÌÁö")]
    public int damage;

    [Header("Æø¹ß ÀÌÆåÆ®")]
    public GameObject explosionEffect; // Æø¹ß ÀÌÆåÆ® ÇÁ¸®ÆÕ
}
