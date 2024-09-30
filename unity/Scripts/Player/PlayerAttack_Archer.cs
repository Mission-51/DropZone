using Polyperfect.Universal;
using System.Collections;
using UnityEngine;

public class PlayerAttack_Archer : MonoBehaviour, IAttack
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
    public float fireDelay = 2.5f; // 공격 간격
    private bool isFireReady = true;

    public ParticleSystem attackEffect; // 공격(소닉붐) 이펙트

    public WeaponManager weaponManager; // WeaponManager 참조
    public PlayerMovement playerMovement; // PlayerMovement 참조
    public float skillCoolDown = 5.0f; // 스킬 쿨타임
    private float lastSkillTime = -100f; // 마지막 스킬 사용 시간을 기록
    private bool isCharging = false; // 차징 여부 
    private float currentChargeTime = 0f; // 차징시간 1

    

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>(); // PlayerMovement 참조
        weaponManager = GetComponent<WeaponManager>();
    }

    void Start()
    {
        // 시작할 때 첫 번째 속성 이펙트만 활성화
        SwitchAttributeEffect(0);
    }

    void Update()
    {
        // 공격 쿨타임 처리
        fireDelay += Time.deltaTime;
        isFireReady = fireDelay >= 0.7f;
        HandleAttributeSwitch();

        // 차징 중일 때 마우스 방향으로 회전
        if (isCharging)
        {
            currentChargeTime += Time.deltaTime;
            playerMovement.TurnTowardsMouse(); // 차징 중 마우스 방향으로 회전
            playerMovement.canDash = false;
            isFireReady = false; // 차징 중 공격 불가

        }

        // 우클릭(스킬) 입력 처리
        if (Input.GetMouseButtonDown(1) && Time.time >= lastSkillTime + skillCoolDown && !playerMovement.isDash )
        {
            StartCharging();
        }

        if (Input.GetMouseButtonUp(1) && isCharging)
        {
            ReleaseAndShootSkill();
        }
    }

    
    public void GetAttackInput(bool fDown)
    {
        if (fDown && isFireReady && !playerMovement.isDash) 
        {
            StartAttack();
        }
    }

    // 기본 공격 로직
    public void StartAttack()
    {        
        playerMovement.SetRangedAttackState(true);  // 원거리 공격 중 이동 불가
        isFireReady = false; // 공격 중 공격 불가
        playerMovement.SetAttackState(true); // 공격 상태 전달
        playerMovement.TurnTowardsMouse(); // 공격 시작 시 마우스 방향으로 회전  
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
    public void ReleaseAndShootSkill()
    {
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

        Invoke("EndAttack", 0.7f); // 0.7초 후 공격 종료
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
        isFireReady = true; // 공격 가능 상태로 전환
    }
    public void HandleAttributeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchAttributeEffect(0); // 1번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchAttributeEffect(1); // 2, 5번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchAttributeEffect(2); // 3, 6번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchAttributeEffect(3); // 4, 7번 속성
        }
    }

    private void SwitchAttributeEffect(int attributeIndex)
    {
        // 범위를 넘는 인덱스가 입력되지 않도록 예외 처리
        if (attributeIndex >= chargingAttributeEffect.Length || attributeIndex >= skillArrowPrefab.Length || attributeIndex >= ArrowPrefab.Length)
            return;

        // 모든 속성 공격 이펙트를 비활성화하고 선택한 속성 공격 이펙트만 활성화
        for (int i = 0; i < chargingAttributeEffect.Length; i++)
        {
            chargingAttributeEffect[i].SetActive(i == attributeIndex);
            ArrowPrefab[i].SetActive(i == attributeIndex);
            skillArrowPrefab[i].SetActive(i == attributeIndex);
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
