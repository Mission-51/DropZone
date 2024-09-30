using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Trap")]
public class Trap : ItemData
{
    [Header("속박 시간")]
    public int rootTime;
    [Header("대미지")]
    public int damage;
}
