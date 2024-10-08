using Opsive.UltimateCharacterController.Traits;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI를 사용하기 위한 네임스페이스
using Photon.Pun; // Photon 네임스페이스 추가

public class PlayerStatus : MonoBehaviourPunCallbacks
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
        Slow1,        // 슬로우1 상태
        Slow2,        // 슬로우2 상태
        Knockback,    // 넉백 상태
        SuperArmor,   // 슈퍼아머 상태
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

        colliders = GetComponentsInChildren<Collider>();

        if (hpBarImage != null)
        {
            UpdateHPUI();
        }
    }

    void Update()
    {
        HandleStatusEffects();  // 상태이상 처리
    }

    //[PunRPC]
    public void TakeDamage(int damage)
    {
        // 데미지를 마스터 클라이언트에서만 처리하고, 이후 결과를 동기화합니다.
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentStatus == StatusEffect.Dead) return;

            currentHP -= damage;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
            UpdateHPUI();

            // HP를 다른 클라이언트에 동기화
            photonView.RPC("UpdateHealth", RpcTarget.All, currentHP);
            //UpdateHealth(currentHP);            

            if (currentHP <= 0)
            {
                ApplyStatusEffect(StatusEffect.Dead);
                return;  // 이후에 다른 상태로 변경되지 않도록 여기서 함수를 종료합니다.
            }
        }

    }

    [PunRPC]
    public void UpdateHealth(int newHealth)
    {
        currentHP = newHealth;
        UpdateHPUI();
    }

    private void ManageStatusEffect(StatusEffect status, bool isActive)
    {
        switch (status)
        {
            case StatusEffect.Stunned:
                if (stunEffect != null)
                {
                    stunEffect.SetActive(isActive);
                    photonView.RPC("RPCManageStunEffect", RpcTarget.Others, isActive);
                }
                break;

            case StatusEffect.Slow1:
            case StatusEffect.Slow2:
                if (slowEffect != null)
                {
                    slowEffect.SetActive(isActive);
                    photonView.RPC("RPCManageSlowEffect", RpcTarget.Others, isActive);
                }
                break;
        }
    }

    [PunRPC]
    public void RPCManageStunEffect(bool isActive)
    {
        if (stunEffect != null)
        {
            stunEffect.SetActive(isActive);
        }
    }

    [PunRPC]
    public void RPCManageSlowEffect(bool isActive)
    {
        if (slowEffect != null)
        {
            slowEffect.SetActive(isActive);
        }
    }

    void HandleStatusEffects()
    {
        switch (currentStatus)
        {
            case StatusEffect.None:
                playerMovement.canMove = true;
                playerMovement.canDash = true;
                break;

            case StatusEffect.Dead:
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                gameObject.layer = LayerMask.NameToLayer("Dead");
                anim.SetBool("isDying", true);
                break;

            case StatusEffect.Stunned:
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                anim.SetTrigger("doStunned");
                ManageStatusEffect(StatusEffect.Stunned, true);
                break;

            case StatusEffect.Immobilized:
                playerMovement.canMove = false;
                playerMovement.canDash = false;
                anim.SetBool("isRun", false);
                break;

            case StatusEffect.Slow1:
                playerMovement.moveSpeed = playerMovement.defaultSpeed * 0.5f;
                playerMovement.canDash = false;
                ManageStatusEffect(StatusEffect.Slow1, true);
                break;

            case StatusEffect.Slow2:
                playerMovement.moveSpeed = playerMovement.defaultSpeed * 0.5f;
                ManageStatusEffect(StatusEffect.Slow2, true);
                break;

            case StatusEffect.Knockback:
                // 넉백 처리 로직                
                break;

            case StatusEffect.SuperArmor:                
                playerMovement.isKnockbackImmune = true;
                break;
        }
    }

    public void ApplyStatusEffect(StatusEffect newStatus)
    {
        // 현재 상태가 Dead일 경우 다른 상태로 변경되지 않도록 합니다.

        if (currentStatus == newStatus || currentStatus == StatusEffect.Dead) return;

        RemoveStatusEffect();
        currentStatus = newStatus;

        // 상태를 다른 클라이언트와 동기화
        photonView.RPC("UpdateStatusEffect", RpcTarget.All, (int)newStatus);

        if (newStatus != StatusEffect.Slow1)
        {
            float effectDuration = GetStatusEffectDuration(newStatus);
            if (effectDuration > 0f)
            {
                if (statusEffectCoroutine != null)
                {
                    StopCoroutine(statusEffectCoroutine);
                }
                statusEffectCoroutine = StartCoroutine(RemoveStatusEffectAfterDelay(effectDuration));
            }
        }
    }

    [PunRPC]
    public void UpdateStatusEffect(int newStatus)
    {
        currentStatus = (StatusEffect)newStatus;
        HandleStatusEffects();
    }

    private float GetStatusEffectDuration(StatusEffect status)
    {
        switch (status)
        {
            case StatusEffect.Immobilized: return 1f;
            case StatusEffect.Slow1: return 0f;
            case StatusEffect.Slow2: return 2.0f;
            case StatusEffect.Knockback: return 1.5f;
            case StatusEffect.Stunned: return 1.0f;
            default: return 0f;
        }
    }

    [PunRPC]
    public void RemoveStatusEffect()
    {
        // 죽음 상태 해제 불가
        if (currentStatus == StatusEffect.Dead)
        {
            return;
        }

        currentStatus = StatusEffect.None;
        playerMovement.moveSpeed = playerMovement.defaultSpeed;


        ManageStatusEffect(StatusEffect.Stunned, false);
        ManageStatusEffect(StatusEffect.Slow1, false);
        ManageStatusEffect(StatusEffect.Slow2, false);

        // 다른 이동 제약 조건 해제
        playerMovement.canMove = true;
        playerMovement.canDash = true;
    }

    private IEnumerator RemoveStatusEffectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("RemoveStatusEffect", RpcTarget.All);        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (currentStatus == StatusEffect.Knockback && collision.gameObject.CompareTag("Untagged"))
        {
            photonView.RPC("ApplyStunOnCollision", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ApplyStunOnCollision()
    {
        ApplyStatusEffect(StatusEffect.Stunned);
    }

    public void ActivateSuperArmor()
    {
        if (currentStatus == StatusEffect.SuperArmor) return;

        photonView.RPC("RPCActivateSuperArmor", RpcTarget.All);
    }

    [PunRPC]
    public void RPCActivateSuperArmor()
    {
        currentStatus = StatusEffect.SuperArmor;
    }

    public void DeactivateSuperArmor()
    {
        if (currentStatus != StatusEffect.SuperArmor) return;

        photonView.RPC("RPCDeactivateSuperArmor", RpcTarget.All);
    }

    [PunRPC]
    public void RPCDeactivateSuperArmor()
    {
        currentStatus = StatusEffect.None;
    }

    public void RestoreHealth(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = (float)currentHP / maxHP;
        }
    }
}
