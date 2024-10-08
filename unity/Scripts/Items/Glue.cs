using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/Glue")]
public class Glue : ItemData
{
    [Header("감속비")]
    [Range(0, 100f)]
    public float slowAmount; // 적의 이동 속도를 감소시키는 정도

    [Header("전개 범위")]
    [Range(0, 10f)]
    public float spreadRange; // 끈끈이가 퍼지는 범위

    public GameObject stickyPrefab; // 바닥에 퍼질 끈끈이 이펙트
}