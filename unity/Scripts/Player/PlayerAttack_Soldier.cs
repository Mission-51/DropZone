using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack_Soldier : MonoBehaviourPun, IAttack
{
    private Animator anim;
    public GameObject bulletPrefab; // 발사체 프리팹
    public GameObject piercingBulletPrefab; // 관통탄 프리팹
    public Transform bulletPos; // 발사 위치
    public float bulletSpeed = 50f; // 발사체 속도
    public float piercingBulletSpeed = 70f; // 관통탄 발사체 속도
    public float fireDelay = 1.5f; // 공격 간격
    private bool isFireReady = true;
    private bool isAttacking = false; // 공격 중인지 관리하는 플래그

    public int maxAmmo = 20; // 최대 탄약 수
    public int currentAmmo;  // 현재 탄약 수
    public float reloadTime = 2.0f; // 재장전 시간
    private bool isReloading = false; // 재장전 여부

    public WeaponManager weaponManager; // weaponManager 참조
    public PlayerMovement playerMovement; // PlayerMovement 참조
    public float skillCoolDown = 5.0f; // 스킬 쿨타임
    private float lastSkillTime = -100f; // 마지막 스킬 사용 시간을 기록

    public GameObject fireEffect; // 발사 이펙트 (활성화/비활성화 할 이펙트)

    public PlayerStatus playerStatus;

    private bool canAttack = true;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // PlayerMovement 참조
        playerStatus = GetComponent<PlayerStatus>();
        currentAmmo = maxAmmo; // 탄약 초기화

        // 발사 이펙트를 비활성화한 상태로 시작
        if (fireEffect != null)
        {
            fireEffect.SetActive(false); // 발사 이펙트 비활성화
        }
        
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

        // 공격 쿨타임 처리
        fireDelay += Time.deltaTime;
        isFireReady = fireDelay >= 0.4f;

        // 수동 재장전 (R키 입력 시)
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
            return;
        }

        // 스킬 사용은 다른 클라이언트와 동기화가 필요하므로 PhotonView와 RPC 사용이 필요함
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown)
        {
            playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전

            // 회전한 후 발사 방향 얻기
            Vector3 shootDirection = bulletPos.forward;

            photonView.RPC("UseSkill", RpcTarget.All, shootDirection);
        }
    }

    public void GetAttackInput(bool fDown)
    {
        if (fDown && isFireReady && canAttack && !playerMovement.isDash)
        {
            playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전
                                               
        
            // 회전한 후 발사 방향 얻기
            Vector3 shootDirection = bulletPos.forward;

            // PhotonView를 통해 RPC 호출 시 현재 회전 방향도 전달
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
    // 일반 공격
    public void StartAttack(Vector3 shootDirection)
    {
        // 재장전 중이라면 공격 불가
        if (isReloading)
        {
            Debug.Log("재장전 중에는 공격할 수 없습니다.");
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("탄약이 부족합니다! 재장전이 필요합니다.");
            StartCoroutine(Reload());
            return;
        }

        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        isFireReady = false; // 공격 중 공격을 막음
        isAttacking = true; // 공격 중 상태 설정
        playerMovement.SetAttackState(true); // 공격 상태 전달
        
        anim.SetTrigger("doAttack"); // 공격 애니메이션 실행
        fireDelay = 0; // 쿨타임 초기화

        // 공격 시 발사체도 다른 클라이언트에 동기화해야 하므로 발사체 생성 및 발사 동작을 RPC로 전파

        // 발사 이펙트를 활성화
        if (fireEffect != null)
        {
            fireEffect.transform.position = bulletPos.position; // 발사 위치에 이펙트 배치
            fireEffect.SetActive(true); // 이펙트 활성화
            Invoke("DisableFireEffect", 0.2f); // 0.2초 후 이펙트를 비활성화
        }

        // 발사체 생성
        GameObject bullet = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation * Quaternion.Euler(0, 180, 0));
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            // 무기의 데미지를 발사체에 설정
            bulletScript.damage = weaponManager.GetCurrentWeaponDamage();
            bulletScript.shooter = gameObject; // 발사자를 현재 게임 오브젝트로 설정

            // 무기의 속성에 따라 발사체의 속성을 설정
            WeaponAttribute currentAttribute = weaponManager.GetCurrentWeaponAttribute();
            switch (currentAttribute)
            {
                case WeaponAttribute.Ice:
                    bulletScript.bulletType = Bullet.BulletType.Ice;
                    break;
                case WeaponAttribute.Gunpowder:
                    bulletScript.bulletType = Bullet.BulletType.Gunpowder;
                    break;
                default:
                    bulletScript.bulletType = Bullet.BulletType.Normal;
                    break;
            }
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = bulletPos.forward * bulletSpeed; // 발사체 속도 적용
        }

        currentAmmo--; // 탄약 감소
        Invoke("TryEndAttack", 1.3f);// 1.3초 후 공격 종료 시도

        // 발사체와 관련된 모든 동작도 네트워크 상에 동기화 필요
    }

    // 스킬 사용 (우클릭으로 사용)
    [PunRPC]
    public void UseSkill(Vector3 shootDirection)
    {
        // 재장전 중이라면 공격 불가
        if (isReloading)
        {
            Debug.Log("재장전 중에는 공격할 수 없습니다.");
            return;
        }

        if (currentAmmo <= 0)
        {
            Debug.Log("탄약이 부족합니다! 재장전이 필요합니다.");
            StartCoroutine(Reload());
            return;
        }

        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        lastSkillTime = Time.time; // 스킬 사용 시간 기록
        playerMovement.SetAttackState(true); // 공격 상태 전달          
        anim.SetTrigger("doAttack"); // 공격 애니메이션 실행

        // 관통탄 발사 동작은 다른 클라이언트에도 동기화가 필요하므로 RPC 사용 고려

        // 발사 이펙트를 활성화
        if (fireEffect != null)
        {
            fireEffect.transform.position = bulletPos.position;// 발사 위치에 이펙트 배치
            fireEffect.SetActive(true); // 이펙트 활성화
            Invoke("DisableFireEffect", 0.2f); // 0.2초 후 이펙트를 비활성화
        }

        // 관통탄 발사
        GameObject piercingBullet = Instantiate(piercingBulletPrefab, bulletPos.position, bulletPos.rotation * Quaternion.Euler(0, 180, 0));
        Bullet bulletScript = piercingBullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.damage = weaponManager.GetCurrentWeaponDamage(); // 무기의 데미지를 관통탄에 설정
            bulletScript.shooter = gameObject; // 발사자를 현재 게임 오브젝트로 설정

            WeaponAttribute currentAttribute = weaponManager.GetCurrentWeaponAttribute();
            switch (currentAttribute)
            {
                case WeaponAttribute.Ice:
                    bulletScript.bulletType = Bullet.BulletType.Ice;
                    break;
                case WeaponAttribute.Gunpowder:
                    bulletScript.bulletType = Bullet.BulletType.Gunpowder;
                    break;
                default:
                    bulletScript.bulletType = Bullet.BulletType.Normal;
                    break;
            }
        }

        Rigidbody rb = piercingBullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = bulletPos.forward * piercingBulletSpeed; // 관통탄 속도 적용
        }

        currentAmmo--; // 탄약 감소
        Invoke("TryEndAttack", 1.3f); // 1.3초 후 공격 종료 시도
    }

    private void DisableFireEffect()
    {
        if (fireEffect != null)
        {
            fireEffect.SetActive(false); // 이펙트 비활성화
        }
    }

    private void TryEndAttack()
    {
        if (!isFireReady && isAttacking)
        {
            // 아직 공격이 끝나지 않았으므로 종료하지 않음
            Debug.Log("계속 공격 중입니다. 종료하지 않습니다.");
        }
        else
        {
            // 공격이 끝났다면 공격 종료
            EndAttack();
        }
    }

    // 공격 종료
    private void EndAttack()
    {
        playerMovement.SetAttackState(false); // 공격 상태 전달
        isAttacking = false; // 공격 상태 종료
        isFireReady = true; // 공격 가능 상태로 전환
        Debug.Log("공격 종료");

        // 공격 종료 상태도 다른 클라이언트에 동기화 필요
    }

    // 재장전 메서드
    private IEnumerator Reload()
    {
        if (isReloading) yield break;                   

        // 모든 클라이언트에 재장전 시작을 알림
        photonView.RPC("StartReloadingRPC", RpcTarget.All);

        yield return new WaitForSeconds(reloadTime); // 재장전 시간 대기

        // 모든 클라이언트에 재장전 완료를 알림
        photonView.RPC("EndReloadingRPC", RpcTarget.All);
    }

    [PunRPC]
    private void StartReloadingRPC()
    {
        isReloading = true;
        anim.SetTrigger("doReload"); // 재장전 애니메이션 실행 (필요에 따라 추가)
        Debug.Log("재장전 중...");
    }

    [PunRPC]
    private void EndReloadingRPC()
    {
        currentAmmo = maxAmmo; // 탄약 충전
        isReloading = false;
        Debug.Log("재장전 완료!");
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

    // Soldier 전용 장탄수 메서드
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
}
