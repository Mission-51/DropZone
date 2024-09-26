using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Glue")]
public class Glue : ItemData
{
    [Header("감속비")]
    [Range(0, 100f)]
    public float slowAmount;
    [Header("전개 범위")]
    [Range(0, 10f)]
    public float spreadRange;
}
