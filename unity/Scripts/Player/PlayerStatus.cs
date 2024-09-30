using Opsive.UltimateCharacterController.Traits;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스

public class PlayerStatus : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    private Animator anim;

    private Collider[] colliders; // 플레이어에 연결된 모든 콜라이더를 관리
    private Coroutine statusEffectCoroutine; // 현재 실행 중인 상태이상 코루틴

    public GameObject stunEffect; // 스턴 이펙트
    public GameObject slowEffect; // 슬로우 이펙트

    public Image hpBarImage;  // HP 바 UI 이미지

    public enum StatusEffect
    {
        None,         // 정상 상태
        Dead,         // 사망 상태
        Stunned,      // 기절 상태
        Immobilized,  // 속박 상태
        Slow1,        // 슬로우1 상태 (이동 속도 감소, 대쉬 불가)
        Slow2,        // 슬로우2 상태 (이동 속도 감소)
        Knockback,    // 넉백 상태
        SuperArmor,   // 슈퍼아머 상태 (Boxer 스킬)
        blood         // 체력 회복 속성 무기 장착 상태
    }

    public StatusEffect currentStatus = StatusEffect.None;  // 기본 상태는 None

    private PlayerMovement playerMovement;
    private Rigidbody rb;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHP = maxHP;  // HP 초기화
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();

        // 플레이어 오브젝트에 붙어 있는 모든 콜라이더를 가져옴
        colliders = GetComponentsInChildren<Collider>();

        // HP 이미지 초기화
        if (hpBarImage != null)
        {
            UpdateHPUI();
        }

        // PhotonView가 필요: 상태 정보(예: HP, 상태이상 등)를 네트워크 상에서 동기화해야 함
    }

    void Update()
    {
        HandleStatusEffects();  // 상태이상 처리
    }

    // 데미지 처리 및 사망 처리
    public void TakeDamage(int damage)
    {
        if (currentStatus == StatusEffect.Dead) return;  // 이미 죽은 상태면 데미지 처리 안함

        currentHP -= damage;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // HP가 음수로 내려가지 않게 설정
        Debug.Log("Player HP: " + currentHP);

        // HP가 변경되면 HP 바 업데이트
        UpdateHPUI();

        if (currentHP <= 0)
        {
            ApplyStatusEffect(StatusEffect.Dead);

            // 데미지나 사망 상태도 다른 클라이언트에 동기화해야 하므로 RPC가 필요
        }
    }

    // 상태이상 이펙트를 관리하는 함수 (중복 코드 정리)
    private void ManageStatusEffect(StatusEffect status, bool isActive)
    {
        switch (status)
        {
            case StatusEffect.Stunned:
                if (stunEffect != null)
                {
                    stunEffect.SetActive(isActive); // 스턴 이펙트 활성화/비활성화
                }
                break;

            case StatusEffect.Slow1:
            case StatusEffect.Slow2:
                if (slowEffect != null)
                {
                    slowEffect.SetActive(isActive); // 슬로우 이펙트 활성화/비활성화
                }
                break;
        }

        // 상태이상 이펙트도 다른 클라이언트에서 동기화되어야 하므로, PhotonView와 RPC가 필요
    }

    // 상태이상 처리
    void HandleStatusEffects()
    {
        switch (currentStatus)
        {
            case StatusEffect.None:
                // 정상 상태: 이동과 공격 가능
                playerMovement.canMove = true;
                playerMovement.canDash = true;
                break;

            case StatusEffect.Dead:
                // 죽음 상태: 이동과 공격 불가
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                gameObject.layer = LayerMask.NameToLayer("Dead");
                anim.SetBool("isDying", true);
                break;

            case StatusEffect.Stunned:
                // 기절 상태: 이동과 공격 불가
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                anim.SetTrigger("doStunned");
                ManageStatusEffect(StatusEffect.Stunned, true);  // 스턴 이펙트 활성화
                break;

            case StatusEffect.Immobilized:
                // 속박 상태: 대쉬 불가, 이동 불가
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                break;

            case StatusEffect.Slow1:
                // 슬로우1 상태: 이동 속도 감소, 대쉬 불가
                playerMovement.moveSpeed = playerMovement.defaultSpeed * 0.5f;
                playerMovement.canDash = false;
                ManageStatusEffect(StatusEffect.Slow1, true);  // 슬로우1 이펙트 활성화
                break;

            case StatusEffect.Slow2:
                // 슬로우2 상태: 이동 속도만 감소
                playerMovement.moveSpeed = playerMovement.defaultSpeed * 0.5f;
                ManageStatusEffect(StatusEffect.Slow2, true);  // 슬로우2 이펙트 활성화
                break;

            case StatusEffect.Knockback:
                // 넉백 처리 로직                
                break;

            case StatusEffect.SuperArmor:
                // 슈퍼아머 상태: 상태이상에 면역                
                playerMovement.isKnockbackImmune = true;
                break;

                // 상태이상 관련 변경 사항도 다른 클라이언트에 동기화해야 하므로, PhotonView와 RPC가 필요
        }
    }

    // 상태이상 부여 함수
    public void ApplyStatusEffect(StatusEffect newStatus)
    {
        // 상태가 None이거나, 넉백 상태에서는 다른 상태이상으로 전환을 허용 (일시적 면역)
        if (currentStatus != StatusEffect.None && currentStatus != StatusEffect.Knockback) return;

        // 슈퍼아머 상태일 때는 상태이상 적용되지 않음
        if (currentStatus == StatusEffect.SuperArmor) return;

        currentStatus = newStatus;
        Debug.Log("New Status: " + newStatus);

        // 기존 상태이상 해제 코루틴 중단
        if (statusEffectCoroutine != null)
        {
            StopCoroutine(statusEffectCoroutine);
        }

        // 상태별 지속 시간 설정
        float effectDuration = GetStatusEffectDuration(newStatus);

        if (effectDuration > 0f)
        {
            // 지속 시간이 0보다 크면 해당 시간 후에 상태 해제
            statusEffectCoroutine = StartCoroutine(RemoveStatusEffectAfterDelay(effectDuration));
        }

        // 상태 변경도 다른 클라이언트에 동기화해야 하므로, PhotonView와 RPC가 필요
    }

    // 상태별 지속 시간을 반환하는 함수
    private float GetStatusEffectDuration(StatusEffect status)
    {
        switch (status)
        {
            case StatusEffect.Slow1:
                return 2.0f;
            case StatusEffect.Slow2:
                return 2.0f;
            case StatusEffect.Knockback:
                return 1.5f; // 넉백은 1.5초 동안 지속
            case StatusEffect.Stunned:
                return 1.0f; // 스턴은 1초 동안 지속
            default:
                return 0f; // None, Dead 등은 지속 시간이 없으므로 0 반환
        }
    }

    // 상태이상 제거 함수 (중복 코드 정리)
    public void RemoveStatusEffect()
    {
        currentStatus = StatusEffect.None;
        playerMovement.moveSpeed = playerMovement.defaultSpeed; // 이동 속도를 기본 속도로 복구

        // 모든 상태이상 이펙트를 비활성화
        ManageStatusEffect(StatusEffect.Stunned, false);
        ManageStatusEffect(StatusEffect.Slow1, false);
        ManageStatusEffect(StatusEffect.Slow2, false);

        // 상태이상 해제도 동기화해야 함: 상태이상 제거 관련 로직도 PhotonView와 RPC로 전송
    }

    // 일정 시간 후 상태이상을 해제하는 코루틴
    private IEnumerator RemoveStatusEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        RemoveStatusEffect();
        Debug.Log("Status effect removed after delay");
    }

    // 넉백 상태에서 벽에 충돌 시 스턴 상태로 변경
    void OnCollisionEnter(Collision collision)
    {
        if (currentStatus == StatusEffect.Knockback && collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("벽에 부딪혔음!");
            ApplyStatusEffect(StatusEffect.Stunned); // 바로 스턴 상태로 전환     
            // 충돌 로직 및 상태이상 전환도 다른 클라이언트에 동기화해야 함
        }
    }

    // 슈퍼아머 활성화 함수 (Boxer 스킬 사용 시 호출)
    public void ActivateSuperArmor()
    {
        currentStatus = StatusEffect.SuperArmor;
        Debug.Log("SuperArmor activated");
        // 슈퍼아머 상태도 다른 클라이언트에 동기화해야 함
    }

    // 슈퍼아머 비활성화 함수 (스킬 지속시간이 끝날 때 호출)
    public void DeactivateSuperArmor()
    {
        currentStatus = StatusEffect.None;  // 슈퍼아머 상태에서 정상 상태로 돌아감
        Debug.Log("SuperArmor deactivated");
        // 슈퍼아머 비활성화도 다른 클라이언트에 동기화해야 함
    }

    // 공격 시 체력 회복 기능
    public void RestoreHealth(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP); // 체력이 최대치를 넘지 않도록 제한

        // 체력 회복 후 HP 바 업데이트
        UpdateHPUI();

        // 체력 회복 기능도 네트워크 상에 동기화가 필요할 수 있음
    }

    // HP UI 업데이트 함수
    private void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            // HP 비율에 따라 fillAmount 업데이트
            hpBarImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}
