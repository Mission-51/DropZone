using UnityEngine;
using static Bullet;

public class Arrow : MonoBehaviour
{
    public int damage;  // 기본 데미지
    public float lifeTime = 1f;  // 화살이 파괴되기까지의 시간
    public GameObject hitEffectPrefab;  // 충돌 시 나타날 이펙트 프리팹
    public float effectOffset = -1f; // 이펙트 위치 조정을 위한 오프셋
    
    public enum ArrowType
    {
        Normal,
        Ice,        // 슬로우
        Gunpowder,   // 폭발탄
        Gear
    }

    public ArrowType arrowType;

    private void Start()
    {
        // 일정 시간이 지나면 화살을 파괴
        Destroy(gameObject, lifeTime);
    }

    public void SetDamage(int newDamage)
    {
        damage = newDamage;
    }

    // 트리거 충돌 처리
    private void OnTriggerEnter(Collider other)
    {
        

        // 충돌한 오브젝트가 플레이어일 때 데미지를 처리
        var player = other.GetComponent<PlayerStatus>();
        if (player != null)
        {
            player.TakeDamage(damage); // 플레이어의 체력에서 데미지를 차감
        }

        // arrow속성에 따라 효과 적용하기
        if (arrowType == ArrowType.Ice && other.gameObject.CompareTag("Player"))
        {
            // 슬로우 효과 적용
            PlayerStatus targetStatus = other.gameObject.GetComponent<PlayerStatus>();
            if (targetStatus != null)
            {
                targetStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Slow2);
            }
        }

        else if (arrowType == ArrowType.Gunpowder && other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Gunpowder");
        }





        // 충돌 이펙트를 생성
        if (hitEffectPrefab != null)
        {
            Vector3 effectPosition = transform.position + transform.forward * effectOffset;
            GameObject collisionEffect = Instantiate(hitEffectPrefab, effectPosition, Quaternion.identity);  // 오프셋 위치에 이펙트 생성

            Destroy(collisionEffect, lifeTime);
            
        }

        // 화살이 플레이어나 적, 벽에 닿으면 파괴
        Destroy(gameObject);
    }
}
