using UnityEngine;
using static PlayerStatus;
using Photon.Pun;
using System.Collections.Generic;
using System.Collections;

public class PlayerAttack_Boxer : MonoBehaviourPun, IAttack
{
    private Animator anim;
    public WeaponManager weaponManager;
    private PlayerMovement playerMovement;
    public GameObject attackCollider;

    public int damage;
    public float fireDelay;
    private bool isAttack;
    private bool isFireReady;

    public float skillCoolDown = 7.0f;
    private float lastSkillTime = -100f;

    public PlayerStatus playerStatus;
    public GameObject superArmorEffect;

    private bool canAttack = true;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        playerStatus = GetComponent<PlayerStatus>();
        attackCollider.SetActive(false);
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (playerMovement.isDash || !canAttack || playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return; // 대쉬중이거나 canAttack이 false이거나 플레이어가 죽은 상태면 공격하지 않음
        }

        fireDelay += Time.deltaTime;
        isFireReady = fireDelay >= 1.5f; // fireDelay는 1.5초가 되어야 공격 가능

        // 쿨타임이 다 돌면 스킬 사용 가능
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown)
        {
            ActivateSuperArmor();
        }
    }

    public void GetAttackInput(bool fDown)
    {
        // 공격 가능한 상태이고 대쉬 중이 아닐 때 공격 가능
        if (fDown && isFireReady && canAttack && !playerMovement.isDash)
        {
            playerMovement.TurnTowardsMouse();

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
        if (!photonView.IsMine) return;

        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        isAttack = true;
        damage = weaponManager.GetCurrentWeaponDamage();
        playerMovement.SetAttackState(true);
        
        anim.SetTrigger("doAttack");
        fireDelay = 0;

        StartCoroutine(PerformMultiAttack());
        Invoke("EndAttack", 1.0f);
    }

    private IEnumerator PerformMultiAttack()
    {
        for (int i = 0; i < 3; i++)
        {
            photonView.RPC("ActivateColliderAndParticle", RpcTarget.All);            
            yield return new WaitForSeconds(0.33f);
        }
    }

    [PunRPC]
    private void ActivateColliderAndParticle()
    {
        attackCollider.SetActive(true);
        Invoke("DeactivateCollider", 0.33f);
    }

    private void DeactivateCollider()
    {
        attackCollider.SetActive(false);
    }

    private void EndAttack()
    {
        isAttack = false;
        playerMovement.SetAttackState(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("TestEnemy"))
        {
            PlayerStatus enemyStatus = other.GetComponent<PlayerStatus>();
            if (enemyStatus != null)
            {
                photonView.RPC("DealDamage", RpcTarget.All, enemyStatus.photonView.ViewID, damage);
                ApplyStatusEffectOnHit(enemyStatus);

                if (weaponManager.GetCurrentWeaponAttribute() == WeaponAttribute.Blood)
                {
                    int healthToRestore = Mathf.CeilToInt(damage * 0.2f);
                    playerStatus.RestoreHealth(healthToRestore);
                }
            }
        }
    }

    [PunRPC]
    private void DealDamage(int enemyViewID, int damageAmount)
    {
        PhotonView enemyPhotonView = PhotonView.Find(enemyViewID);
        PlayerStatus enemyStatus = enemyPhotonView.GetComponent<PlayerStatus>();
        if (enemyStatus != null)
        {
            enemyStatus.TakeDamage(damageAmount);
        }
    }

    private void ApplyStatusEffectOnHit(PlayerStatus enemyStatus)
    {
        WeaponAttribute currentWeaponAttribute = weaponManager.GetCurrentWeaponAttribute();

        if (currentWeaponAttribute == WeaponAttribute.Ice)
        {
            enemyStatus.ApplyStatusEffect(StatusEffect.Slow2);
        }
    }

    private void ActivateSuperArmor()
    {
        if (playerStatus.currentStatus != StatusEffect.SuperArmor)
        {
            photonView.RPC("ActivateSuperArmorRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    private void ActivateSuperArmorRPC()
    {
        playerStatus.ApplyStatusEffect(StatusEffect.SuperArmor);
        if (superArmorEffect != null)
        {
            superArmorEffect.SetActive(true);
            playerMovement.moveSpeed += 1; // 슈퍼아머 사용 시 이동 속도 증가
        }

        StartCoroutine(DeactivateSuperArmorAfterDelay(5.0f));
        lastSkillTime = Time.time;
    }

    private IEnumerator DeactivateSuperArmorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        playerStatus.RemoveStatusEffect();
        playerMovement.moveSpeed = playerMovement.defaultSpeed; // 슈퍼아머 해제 시 이동 속도 원복
        if (superArmorEffect != null)
        {
            superArmorEffect.SetActive(false);
        }
    }

    public float GetSkillCooldown()
    {
        return skillCoolDown;
    }

    public float GetLastSkillTime()
    {
        return lastSkillTime;
    }
}
