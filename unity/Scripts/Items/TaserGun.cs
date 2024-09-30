using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Taser Gun")]
public class TaserGun : ItemData
{
    [Header("기절 시간")]
    public int stunTime; 
}
