using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Heal item")]
public class HealItemData : ItemData
{
    [Header("사용하는데 소요되는 시간")]
    public int usingTime; 
    [Header("힐량")]
    public int healAmount;
}
