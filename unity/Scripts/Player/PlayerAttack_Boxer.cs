using System.Collections;
using UnityEngine;
using static PlayerStatus;

public class PlayerAttack_Boxer : MonoBehaviour, IAttack
{
    private Animator anim;
    public WeaponManager weaponManager;
    private PlayerMovement playerMovement; // PlayerMovement 참조
    public GameObject attackCollider; // 공격 콜라이더 (Hit Box)

    public int damage; // 공격할 때 줄 데미지(무기에서 가져와서 적용)        
    public float fireDelay;
    private bool isAttack;
    private bool isFireReady;

    public float skillCoolDown = 7.0f; // 스킬 쿨다운
    private float lastSkillTime = -100f; // 마지막 스킬 사용 시간을 기록    

    public PlayerStatus playerStatus; // 플레이어 상태 참조 (슈퍼아머 적용을 위해)
    public GameObject superArmorEffect; // 슈퍼아머 이펙트를 위한 GameObject

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // PlayerMovement 컴포넌트 참조
        playerStatus = GetComponent<PlayerStatus>(); // PlayerStatus 컴포넌트 참조
        attackCollider.SetActive(false); // 처음에는 비활성화

        // PhotonView가 필요: 공격 및 스킬 사용과 같은 중요한 동작을 네트워크 상에 동기화해야 함
    }

    void Update()
    {
        fireDelay += Time.deltaTime;
        isFireReady = 1.5f < fireDelay;

        // 스킬은 네트워크 동기화가 필요하므로 PhotonView와 RPC를 사용하여 스킬 사용 시 이를 다른 클라이언트에 전파해야 함
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown) // 우클릭 입력
        {
            ActivateSuperArmor(); // 슈퍼아머 적용
            // 슈퍼아머 적용도 다른 클라이언트와 동기화되어야 하므로 RPC 사용 필요
        }
    
    }

    public void GetAttackInput(bool fDown)
    {
        if (fDown && isFireReady)
        {
            StartAttack();
            // 공격 동작은 다른 클라이언트에도 전달되어야 하므로 PhotonView를 사용해 동기화해야 함
        }
    }

    public void StartAttack()
    {
        isAttack = true;

        // 현재 장착된 무기의 데미지 가져오기
        damage = weaponManager.GetCurrentWeaponDamage();

        playerMovement.SetAttackState(true); // 공격 상태 전달
        playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전        
        anim.SetTrigger("doAttack");
        fireDelay = 0;

        StartCoroutine(PerformMultiAttack());
        Invoke("EndAttack", 1.0f); // 1초 후 공격 종료
    }

    private IEnumerator PerformMultiAttack()
    {
        for (int i = 0; i < 3; i++) // 3 콤보 어택
        {
            ActivateColliderAndParticle();
            yield return new WaitForSeconds(0.33f); // 공격 간격
        }
    }

    private void ActivateColliderAndParticle()
    {
        attackCollider.SetActive(true);
        Invoke("DeactivateCollider", 0.33f); // 0.3초 후 콜라이더 비활성화
    }

    private void DeactivateCollider()
    {
        attackCollider.SetActive(false);
    }

    private void EndAttack()
    {
        isAttack = false;
        playerMovement.SetAttackState(false); // 공격 상태 전달

        // 공격 종료도 다른 클라이언트에 동기화해야 하므로 PhotonView와 RPC 사용 필요
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus enemyStatus = other.GetComponent<PlayerStatus>();
            if (enemyStatus != null)
            {
                enemyStatus.TakeDamage(damage); // 상대 플레이어에게 데미지를 줌

                // 상태이상 적용 (ex. ice 속성인 경우 슬로우 상태를 부여)                                
                ApplyStatusEffectOnHit(enemyStatus);

                // 공격에 따른 상태 변화 역시 다른 클라이언트에 동기화가 필요하므로 PhotonView와 RPC 사용 고려

                // 현재 무기의 속성이 Blood인 경우 체력 회복
                if (weaponManager.GetCurrentWeaponAttribute() == WeaponAttribute.Blood)
                {
                    int healthToRestore = Mathf.CeilToInt(damage * 0.2f); // 데미지의 20%를 반올림하여 계산
                    playerStatus.RestoreHealth(healthToRestore); // 원하는 만큼의 회복량 설정
                    Debug.Log($"체력이 {healthToRestore} 회복되었습니다.");
                    // 체력 회복도 네트워크 동기화가 필요할 수 있음
                }

            }
        }
    }

    // 상태이상을 적용하는 함수
    private void ApplyStatusEffectOnHit(PlayerStatus enemyStatus)
    {
        // 현재 장착된 무기의 속성 가져오기
        WeaponAttribute currentWeaponAttribute = weaponManager.GetCurrentWeaponAttribute();

        // Ice 속성인 경우 상대방에게 Slow2 상태를 부여
        if (currentWeaponAttribute == WeaponAttribute.Ice)
        {
            enemyStatus.ApplyStatusEffect(StatusEffect.Slow2);
            Debug.Log("Applied Slow2 effect to the enemy.");

            // 상대방에게 상태이상을 부여하는 동작 역시 네트워크 동기화가 필요할 수 있음
        }
    }

    // 슈퍼아머를 활성화하는 함수 (5초간 적용)
    private void ActivateSuperArmor()
    {
        if (playerStatus.currentStatus != StatusEffect.SuperArmor)
        {
            playerStatus.ApplyStatusEffect(StatusEffect.SuperArmor); // 슈퍼아머 상태 적용
            Debug.Log("SuperArmor activated for 5 seconds");

            // 슈퍼아머 상태 변경은 다른 클라이언트에 동기화되어야 하므로 RPC로 처리 필요

            // 슈퍼아머 이펙트를 활성화
            if (superArmorEffect != null)
            {
                superArmorEffect.SetActive(true); // 이펙트 활성화
            }

            StartCoroutine(DeactivateSuperArmorAfterDelay(5.0f)); // 5초 후 슈퍼아머 해제

            // 스킬 사용 시간을 기록해 쿨다운을 적용
            lastSkillTime = Time.time; // 스킬 사용 시간을 기록
        }
    }


    // 5초 후 슈퍼아머를 비활성화하는 코루틴
    private IEnumerator DeactivateSuperArmorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerStatus.RemoveStatusEffect(); // 상태이상 해제 (슈퍼아머 해제)

        // 슈퍼아머 해제도 다른 클라이언트에 동기화되어야 하므로 RPC로 처리 필요

        // 슈퍼아머 이펙트를 비활성화
        if (superArmorEffect != null)
        {
            superArmorEffect.SetActive(false); // 이펙트 비활성화
        }

        Debug.Log("SuperArmor deactivated after 5 seconds");
    }

    public void ExecuteEvent()
    {
        // 공격 애니메이션에서 특정 타이밍에 이벤트를 실행하고 싶을 때
        Debug.Log("Attack animation event triggered!");
        // 필요한 로직 추가 (예: 데미지 적용 등)
    }

    // IAttack 인터페이스 구현
    public float GetSkillCooldown()
    {
        return skillCoolDown;
    }

    public float GetLastSkillTime()
    {
        return lastSkillTime;
    }
}
