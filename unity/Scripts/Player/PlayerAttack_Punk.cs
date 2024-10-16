using System.Collections;
using UnityEngine;
using static PlayerStatus;
using Photon.Pun;

public class PlayerAttack_Punk : MonoBehaviourPun, IAttack
{
    private Animator anim;
    public WeaponManager weaponManager;
    private PlayerMovement playerMovement;
    public GameObject attackCollider; // 공격 콜라이더 (히트박스)
    public GameObject skillCollider; // 스킬 콜라이더 (히트박스)

    public int damage; // 공격할 때 줄 데미지(무기에서 가져와서 적용) 
    public float fireDelay;
    private bool isAttack;
    private bool isFireReady;

    public float skillCoolDown = 5.0f; // 스킬 쿨다운
    private float lastSkillTime = -100f; // 마지막 스킬 사용 시간 기록

    public float knockbackForce = 5.0f; // 넉백의 힘

    public PlayerStatus playerStatus;

    public AudioSource swingSound;
    public AudioSource skillSound;

    private bool canAttack = true;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<PlayerStatus>();
        attackCollider.SetActive(false);
        skillCollider.SetActive(false);

    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (playerMovement.isDash || !canAttack || playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return; // canAttack이 false이거나 플레이어가 죽은 상태면 공격하지 않음
        }

        fireDelay += Time.deltaTime;
        isFireReady = 1.5f < fireDelay;

        
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown)
        {
            playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전

            // 회전한 후 공격 방향 얻기
            Vector3 shootDirection = transform.forward;

            photonView.RPC("UseSkill", RpcTarget.All, shootDirection);
            
        }
    }

    public void GetAttackInput(bool fDown)
    {
        if (fDown && isFireReady && canAttack && !playerMovement.isDash)
        {
            playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전

            // 회전한 후 공격 방향 얻기
            Vector3 shootDirection = transform.forward;

            photonView.RPC("StartAttack", RpcTarget.All, shootDirection);
        }
    }

    // 공격 가능 여부 동기화
    [PunRPC]
    public void UpdateCanAttack(bool value)
    {
        canAttack = value;
    }

    [PunRPC]
    public void StartAttack(Vector3 shootDirection)
    {
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 현재 장착된 무기의 데미지 가져오기
        damage = weaponManager.GetCurrentWeaponDamage();

        isAttack = true;
        playerMovement.SetAttackState(true);
        anim.SetTrigger("doAttack");
        fireDelay = 0;

        StartCoroutine(PerformMultiAttack());
        Invoke("EndAttack", 1.0f);

        // 공격 상태도 다른 클라이언트에 동기화해야 하므로 RPC 필요
    }

    private IEnumerator PerformMultiAttack()
    {
        for (int i = 0; i < 2; i++) // 2 콤보 어택
        {
            if (photonView.IsMine) swingSound.Play();
            ActivateAttackCollider();
            yield return new WaitForSeconds(0.5f); // 공격 간격
        }
    }
        
    private void ActivateAttackCollider()
    {
        attackCollider.SetActive(true);
        Invoke("DeactivateAttackCollider", 0.5f); // 일정 시간 이후 콜라이더 비활성화
    }

    private void DeactivateAttackCollider()
    {
        attackCollider.SetActive(false);
    }

    private void EndAttack()
    {
        isAttack = false;
        playerMovement.SetAttackState(false);        
    }

    // 공격 및 스킬 모두 처리하는 OnTriggerEnter
    private void OnTriggerEnter(Collider other)
    {
        // 공격 콜라이더와 스킬 콜라이더를 구분
        if (other.CompareTag("Player"))
        {
            // 공격 콜라이더에 맞았을 때
            if (attackCollider.activeSelf)
            {
                HandleAttackCollision(other);
            }

            // 스킬 콜라이더에 맞았을 때
            if (skillCollider.activeSelf)
            {
                HandleSkillCollision(other);
            }

            // 트리거 충돌 및 효과도 다른 클라이언트에 동기화되어야 하므로 PhotonView와 RPC 사용 고려
        }
    }

    private void HandleAttackCollision(Collider other)
    {
        PlayerStatus enemyStatus = other.GetComponent<PlayerStatus>();
        if (enemyStatus != null)
        {
            // 데미지 적용
            enemyStatus.TakeDamage(damage, photonView.ViewID);

            // 상태이상 적용
            ApplyStatusEffectOnHit(enemyStatus);
        }
    }

    private void HandleSkillCollision(Collider other)
    {
        Rigidbody enemyRb = other.GetComponent<Rigidbody>();
        PlayerStatus enemyStatus = other.GetComponent<PlayerStatus>();

        if (enemyRb != null && enemyStatus != null)
        {
            // 데미지 적용 (스킬데미지 = 데미지 * 1.5)
            enemyStatus.TakeDamage(Mathf.RoundToInt(damage * 1.5f), photonView.ViewID);            

            // 상대방이 슈퍼아머 상태라면 넉백만 무시
            if (enemyStatus.currentStatus == PlayerStatus.StatusEffect.SuperArmor)
            {                
                return;
            }

            // 넉백 방향 계산
            Vector3 knockbackDirection = (other.transform.position - transform.position).normalized;
            knockbackDirection.y = 0f; // y축을 고정하여 위/아래 움직임을 방지

            // 적을 밀어냄 (넉백 효과)
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);

            // 상태이상을 Knockback으로 설정
            enemyStatus.ApplyStatusEffect(StatusEffect.Knockback);  
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
        }
    }

    // 스킬 사용
    [PunRPC]
    private void UseSkill(Vector3 shootDirection)
    {
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 현재 장착된 무기의 데미지 가져오기
        damage = weaponManager.GetCurrentWeaponDamage();

        lastSkillTime = Time.time;
        playerMovement.SetAttackState(true);        
        anim.SetTrigger("doSkill");


        // 스킬 콜라이더 활성화
        Invoke("ActivateSkillCollider", 0.6f);
        // 스킬 사용 후 비활성화
        Invoke("DeactivateSkillCollider", 0.8f);

        Invoke("EndAttack", 0.8f);
    }

    private void ActivateSkillCollider()
    {
        if (photonView.IsMine) skillSound.Play();
        skillCollider.SetActive(true);
    }

    private void DeactivateSkillCollider()
    {
        skillCollider.SetActive(false);
    }

    public void ExecuteEvent()
    {
        Debug.Log("Attack animation event triggered!");
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
