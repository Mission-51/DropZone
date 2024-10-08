using Polyperfect.Universal;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerAttack_Archer : MonoBehaviourPun, IAttack
{
    private Animator anim;

    public GameObject[] ArrowPrefab; // 발사체 프리팹
    public GameObject[] skillArrowPrefab; // 스킬 발사체 프리팹
    public GameObject[] chargingAttributeEffect; // 차징 이펙트
    private int currentAttributeIndex = 0; // 현재 속성 인덱스


    public Transform ArrowPos; // 발사 위치
    public float ArrowSpeed = 30f; // 발사체 속도


    public float maxArrowSpeed = 50f;  // 최대 화살 속도
    public float maxChargeTime = 2.0f; // 최대 차징 시간
    public float minChargeTime = 0.5f; // 최소 차징 시간
    public float fireDelay = 2.0f; // 공격 간격
    private bool isFireReady = true;

    public ParticleSystem attackEffect; // 공격(소닉붐) 이펙트

    public WeaponManager weaponManager; // WeaponManager 참조
    public PlayerMovement playerMovement; // PlayerMovement 참조
    public float skillCoolDown = 5.0f; // 스킬 쿨타임
    private float lastSkillTime = -100f; // 마지막 스킬 사용 시간을 기록
    private bool isCharging = false; // 차징 여부 
    private float currentChargeTime = 0f; // 차징시간 1

    public PlayerStatus playerStatus;

    private bool canAttack = true;

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // PlayerMovement 참조
        weaponManager = GetComponent<WeaponManager>();
        playerStatus = GetComponent<PlayerStatus>();
    }

    void Start()
    {
        // 시작할 때 첫 번째 속성 이펙트만 활성화
        photonView.RPC("SwitchAttributeEffect", RpcTarget.All, 0);        
    }

    void Update()
    {
        if(!photonView.IsMine)
        {
            return;
        }

        if (playerMovement.isDash || !canAttack || playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return; // canAttack이 false이거나 플레이어가 죽은 상태면 공격하지 않음
        }

        // 공격 쿨타임 처리
        fireDelay += Time.deltaTime;
        isFireReady = fireDelay >= 0.7f;

        // WeaponManager의 무기 인덱스를 가져와 속성 업데이트
        int weaponIndex = weaponManager.GetCurrentWeaponIndex();
        if (weaponIndex != currentAttributeIndex) // 속성이 변경된 경우에만 업데이트
        {            
            photonView.RPC("SwitchAttributeEffect", RpcTarget.All, weaponIndex);
        }

        // 차징 중일 때 마우스 방향으로 회전
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            playerMovement.TurnTowardsMouse(); // 차징 중 마우스 방향으로 회전
            playerMovement.canDash = false;
            isFireReady = false; // 차징 중 공격 불가

        }

        // 우클릭(스킬) 입력 처리
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown)
        {            
            photonView.RPC("StartCharging", RpcTarget.All);
        }

        if (Input.GetMouseButtonUp(1) && isCharging)
        {
            Vector3 shootDirection = ArrowPos.forward;

            photonView.RPC("ReleaseAndShootSkill", RpcTarget.All, shootDirection);            
        }
    }

    
    public void GetAttackInput(bool fDown)
    {
        if (fDown && isFireReady && canAttack && !playerMovement.isDash)
        {
            playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전
                                               
            Vector3 shootDirection = ArrowPos.forward;

            photonView.RPC("StartAttack", RpcTarget.All, shootDirection);
        }
    }

    // 공격 가능 여부 동기화
    [PunRPC]
    public void UpdateCanAttack(bool value)
    {
        canAttack = value;
    }

    // 기본 공격 로직
    [PunRPC]
    public void StartAttack(Vector3 shootDirection)
    {

        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        playerMovement.SetRangedAttackState(true);  // 원거리 공격 중 이동 불가
        isFireReady = false; // 공격 중 공격 불가
        playerMovement.SetAttackState(true); // 공격 상태 전달         
        anim.SetTrigger("doAttack"); // 공격 애니메이션 실행
        fireDelay = 0; // 쿨타임 초기화


        // 발사체 생성 및 발사

        // 소닉붐 이펙트 실행
        if (attackEffect != null)
        {
            attackEffect.Play();
        }
        GameObject Arrow = Instantiate(ArrowPrefab[currentAttributeIndex], ArrowPos.position, ArrowPos.rotation);
        Rigidbody rb = Arrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = ArrowPos.forward * ArrowSpeed; // 발사체 속도 적용
        }

        // 플레이어와 화살의 충돌을 무시
        Collider playerCollider = GetComponent<Collider>();  // 플레이어의 콜라이더 가져오기
        Collider arrowCollider = Arrow.GetComponent<Collider>();  // 화살의 콜라이더 가져오기
        if (playerCollider != null && arrowCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, arrowCollider);
        }

        // WeaponManager를 통해 무기의 데미지를 가져와서 설정
        int weaponDamage = weaponManager.GetCurrentWeaponDamage();
        Arrow.GetComponent<Arrow>().SetDamage(weaponDamage);

        Invoke("EndAttack", 0.3f); // 0.3초 후 공격 종료
    }

    // 차징 시작
    [PunRPC]
    public void StartCharging()
    {        
        isCharging = true; // 차징 상태로 전환
        anim.SetBool("isCharging", true); // 차징 애니메이션 실행
        currentChargeTime = 0f; // 1
        playerMovement.moveSpeed /= 2f;   // 차징 중 이동속도 감소


        // 차징 이펙트 실행
        if (chargingAttributeEffect[currentAttributeIndex] != null)
        {
            ParticleSystem chargingEffect = chargingAttributeEffect[currentAttributeIndex].GetComponent<ParticleSystem>();
            if (chargingEffect != null)
            {
                chargingEffect.Play();
            }
        }
    }

    // 차징 후 스킬 발사
    [PunRPC]
    public void ReleaseAndShootSkill(Vector3 shootDirection)
    {
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        isCharging = false; // 차징 해제
        lastSkillTime = Time.time; // 스킬 사용 시간 기록             
        anim.SetBool("isCharging", false); // 차징 애니메이션 종료
        playerMovement.moveSpeed *= 2f; // 이동속도 원복

        float chargePercent = Mathf.Clamp01(currentChargeTime / maxChargeTime); // 0과 1 사이 값
        float skillArrowSpeed = Mathf.Lerp(ArrowSpeed, maxArrowSpeed, chargePercent); // 화살 속도 조정

        // 차징 보너스 데미지 계산 (-5, 0, 5, 10, 15 단계)
        int chargeBonus = CalculateDamage(chargePercent);

        // WeaponManager를 통해 무기의 데미지를 가져옴
        int weaponDamage = weaponManager.GetCurrentWeaponDamage();
        int finalDamage = weaponDamage + chargeBonus;

        // 차징 이펙트 중지
        if (chargingAttributeEffect[currentAttributeIndex] != null)
        {
            ParticleSystem chargingEffect = chargingAttributeEffect[currentAttributeIndex].GetComponent<ParticleSystem>();
            if (chargingEffect != null)
            {
                chargingEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        // 스킬 발사체 생성 및 발사

        // 소닉붐 이펙트 실행
        if (attackEffect != null)
        {
            attackEffect.Play();
        }

        GameObject skillArrow = Instantiate(skillArrowPrefab[currentAttributeIndex], ArrowPos.position, ArrowPos.rotation);
        Rigidbody rb = skillArrow.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.velocity = ArrowPos.forward * skillArrowSpeed; // 스킬 발사체 속도 적용
        }

        // 플레이어와 화살의 충돌을 무시
        Collider playerCollider = GetComponent<Collider>();  // 플레이어의 콜라이더 가져오기
        Collider arrowCollider = skillArrow.GetComponent<Collider>();  // 화살의 콜라이더 가져오기
        if (playerCollider != null && arrowCollider != null)
        {
            Physics.IgnoreCollision(playerCollider, arrowCollider);
        }

        // 최종 데미지 설정
        skillArrow.GetComponent<Arrow>().SetDamage(finalDamage);

        Invoke("EndAttack", 0.3f); // 0.3초 후 공격 종료
    }
    private int CalculateDamage(float chargePercent)
    {
        if (chargePercent >= 1.0f)
        {
            return 15;
        }
        else if (chargePercent >= 0.75f)
        {
            return 10;
        }
        else if (chargePercent >= 0.5f)
        {
            return 5;
        }
        else if (chargePercent >= 0.25f)
        {
            return 0;
        }
        else
        {
            return -5;
        }
    }

    // 공격 종료
    private void EndAttack()
    {    
        playerMovement.SetRangedAttackState(false);  // 원거리 공격 상태 해제
        playerMovement.SetAttackState(false); // 공격 상태 전달        
    }
 
    [PunRPC]
    private void SwitchAttributeEffect(int weaponIndex)
    {
        // 티어2와 티어3의 같은 속성 인덱스 처리
        int attributeIndex;


        if (weaponIndex >= 4 && weaponIndex <= 6) // 티어3 속성 1~3은 티어2 1~3 속성 이펙트와 동일
        {
            attributeIndex = weaponIndex - 3;
        }
        else if (weaponIndex < 4)  // 나머지는 동일
        {
            attributeIndex = weaponIndex;
        }
        else
        {
            Debug.LogError("잘못된 무기 인덱스입니다: " + weaponIndex);
            return;
        }

        // 범위를 넘는 인덱스가 입력되지 않도록 예외 처리
        if (attributeIndex >= chargingAttributeEffect.Length || attributeIndex >= skillArrowPrefab.Length || attributeIndex >= ArrowPrefab.Length)
            return;

        // 차징 이펙트 변경
        for (int i = 0; i < chargingAttributeEffect.Length; i++)
        {
            chargingAttributeEffect[i].SetActive(i == attributeIndex);                        
        }

        // 현재 속성 인덱스를 업데이트
        currentAttributeIndex = attributeIndex;
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
