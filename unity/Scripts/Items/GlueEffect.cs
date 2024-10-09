using UnityEngine;

public class GlueEffect : MonoBehaviour
{
    // 트리거 안으로 들어왔을 때 호출되는 메서드
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                // 이미 슬로우 상태가 아닌 경우에만 슬로우 상태 적용
                if (playerStatus.currentStatus != PlayerStatus.StatusEffect.Slow1)
                {
                    playerStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Slow1);
                    Debug.Log("Player affected by Slow1 from glue.");
                }
            }
        }
    }

    // 트리거에서 벗어났을 때 호출되는 메서드
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {
                // 플레이어가 끈끈이 영역에서 벗어나면 상태 즉시 해제
                if (playerStatus.currentStatus == PlayerStatus.StatusEffect.Slow1)
                {
                    playerStatus.RemoveStatusEffect();  // 즉시 상태 해제
                    Debug.Log("Player left glue area, slow effect removed.");
                }
            }
        }
    }

    // 끈끈이 오브젝트가 삭제될 때 호출되는 메서드
    void OnDestroy()
    {
        // 현재 끈끈이 트리거 내에 있는 모든 객체를 확인
        Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.x / 2); // Sphere로 탐지

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                PlayerStatus playerStatus = collider.GetComponent<PlayerStatus>();
                if (playerStatus != null && playerStatus.currentStatus == PlayerStatus.StatusEffect.Slow1)
                {
                    // 끈끈이 오브젝트가 삭제되면 슬로우 상태 해제
                    playerStatus.RemoveStatusEffect();
                    Debug.Log("Player's slow effect removed due to glue destruction.");
                }
            }
        }
    }
}