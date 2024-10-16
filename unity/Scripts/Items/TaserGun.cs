using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Taser Gun")]
public class TaserGun : ItemData
{
    [Header("기절 시간")]
    public int stunTime;

    [Header("발사 이펙트")]
    public GameObject shootEffect; // 발사 시 나타나는 이펙트
}
