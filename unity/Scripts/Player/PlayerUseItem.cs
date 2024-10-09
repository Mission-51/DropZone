using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Items.Actions.Impact;
using Opsive.UltimateCharacterController.Objects;
using System;
using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerUseItem : MonoBehaviourPun
{
    private PlayerStatus playerStatus;
    private PlayerMovement playerMovement;
    private InGameUIManager inGameUIManager; // UI 매니저 참조
    private float cooldownTimer;
    private ItemData currentItem;
    private int currentItemId; // 현재 아이템의 ID

    private Coroutine healingCoroutine;

    private Animator anim;

    public HealItemData Bandage;  // 힐 아이템 (작은 회복)
    public HealItemData FirstAidKit;  // 힐 아이템 (큰 회복)
    public ItemData TaserGun;       // 테이저 건 (기절 상태)
    public ItemData Grenade;        // 수류탄
    public ItemData Glue;           // 끈끈이
    public ItemData Trap;           // 덫

    void Awake()
    {
        anim = GetComponent<Animator>();
        playerStatus = GetComponent<PlayerStatus>();
        playerMovement = GetComponent<PlayerMovement>();
        inGameUIManager = FindObjectOfType<InGameUIManager>(); // UI 매니저를 찾음
    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        // 플레이어가 죽었다면 더 이상 업데이트하지 않음
        if (playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return;
        }

        // 회복 중일 때 공격하거나 스킬을 사용하면 회복 취소
        if (healingCoroutine != null && (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)))
        {
            // 회복 취소 동기화
            photonView.RPC("CancelHeal", RpcTarget.All);
        }

        // 쿨타임 관리
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;

            if (inGameUIManager != null)
            {
                float cooldownProgress = cooldownTimer / currentItem.coolDownTime;
                inGameUIManager.UpdateItemCooldownUI(cooldownProgress, cooldownTimer);
            }

            if (cooldownTimer <= 0 && inGameUIManager != null)
            {
                inGameUIManager.ShowItemCooldownUI(false);
            }
        }
    }

    // 인벤토리에서 선택된 아이템을 받는 메서드    
    public void UseSelectedItem(Slot slot)
    {

        // 체력이 가득 차 있을 경우 회복 아이템 사용 차단
        if (slot.item is HealItemData && playerStatus.currentHP >= playerStatus.maxHP)
        {
            Debug.Log("HP is full. Cannot use heal item.");
            return;
        }

        // 이미 회복 중이라면 다른 회복 아이템 사용 차단
        if (healingCoroutine != null && slot.item is HealItemData)
        {
            Debug.Log("Already healing. Cannot use another heal item.");
            return;
        }

        // 쿨타임 확인 후 아이템 사용
        if (cooldownTimer <= 0 && slot.item != null)
        {
            currentItem = slot.item;
            currentItemId = currentItem.id; // 아이템 ID 추출

            cooldownTimer = slot.item.coolDownTime;

            // 아이템 사용            
            ActivateItem(currentItemId, slot);             

            // 사용 아이템 쿨타임 UI 활성화
            if (inGameUIManager != null)
            {
                inGameUIManager.ShowItemCooldownUI(true);
            }

        }
        else
        {
            Debug.Log("Item is on cooldown or invalid.");
        }
    }


    // 아이템을 사용하는 실제 로직
    void ActivateItem(int itemId, Slot slot)
    {
        switch (itemId)
        {
            case 0:
                StartCoroutine(UseHealItem(Bandage, slot));
                break;
            case 1:
                StartCoroutine(UseHealItem(FirstAidKit, slot));
                break;
            case 2:
                Vector3 taserDirection = GetShootDirection(); // 공통된 로직 호출
                photonView.RPC("UseTaserGun", RpcTarget.All, taserDirection);
                slot.SetSlotCount(-1);
                break;
            case 3:
                Vector3 grenadeDirection = GetShootDirection(); // 공통된 로직 호출
                photonView.RPC("UseGrenade", RpcTarget.All, grenadeDirection);
                slot.SetSlotCount(-1);
                break;
            case 4:
                Vector3 trapDirection = GetShootDirection(); // 공통된 로직 호출
                photonView.RPC("UseTrap", RpcTarget.All, trapDirection);
                slot.SetSlotCount(-1);
                break;
            case 5:
                Vector3 glueDirection = GetShootDirection(); // 공통된 로직 호출                
                photonView.RPC("UseGlue", RpcTarget.All, glueDirection);
                slot.SetSlotCount(-1);
                break;
            default:
                Debug.Log("Invalid item ID");
                break;
        }
    }

    // 마우스 방향으로 회전하고 회전 후 발사 방향 반환하는 메서드
    Vector3 GetShootDirection()
    {
        playerMovement.TurnTowardsMouse(); // 마우스 방향으로 회전
        return transform.forward; // 회전한 후의 방향 반환
    }

    // 힐 아이템 사용 코루틴    
    IEnumerator UseHealItem(HealItemData healItem, Slot slot)
    {
        // 회복이 시작되었음을 설정
        healingCoroutine = StartCoroutine(UseHealItemCoroutine(healItem, slot));
        yield return healingCoroutine;
        healingCoroutine = null; // 회복이 끝나면 null로 설정
    }

    IEnumerator UseHealItemCoroutine(HealItemData healItem, Slot slot)
    {
        // 회복 UI 업데이트 시작
        if (inGameUIManager != null)
        {
            inGameUIManager.ShowHealUI(true); // 회복 UI 활성화
        }

        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        int initialHP = playerStatus.currentHP;
        anim.SetBool("isHeal", true); // 힐 애니메이션 실행
        playerMovement.moveSpeed = playerMovement.moveSpeed * 0.3f; // 회복 아이템 사용 시 이동 속도 30% 적용


        while (elapsedTime < healItem.usingTime)
        {
            // 캐릭터가 피격을 당하면 힐 취소
            if (playerStatus.currentHP < initialHP)
            {
                Debug.Log("Healing canceled due to movement or damage.");
                photonView.RPC("CancelHeal", RpcTarget.All);
                yield break;
            }

            elapsedTime += Time.deltaTime;

            // UI 업데이트
            if (inGameUIManager != null)
            {
                float progress = elapsedTime / healItem.usingTime;
                float remainingTime = healItem.usingTime - elapsedTime;
                inGameUIManager.UpdateHealProgressUI(progress, remainingTime); // UI 업데이트
            }

            yield return null;
        }

        // 회복 완료 후 체력 회복 동기화 (RPC 호출)
        photonView.RPC("HealPlayerRPC", RpcTarget.All, healItem.healAmount);

        // 회복 UI 비활성화
        if (inGameUIManager != null)
        {
            inGameUIManager.ShowHealUI(false);
        }

        // 회복이 완료된 후 아이템 개수를 줄임
        if (slot != null)
        {
            slot.SetSlotCount(-1); // 사용 후 개수 줄이기
        }
    }


    // 회복 완료 후 체력 회복을 동기화하는 RPC
    [PunRPC]
    void HealPlayerRPC(int healAmount)
    {
        playerStatus.RestoreHealth(healAmount);
        Debug.Log("Healed " + healAmount);
        anim.SetBool("isHeal", false); // 힐 애니메이션 종료
        playerMovement.moveSpeed = playerMovement.defaultSpeed; // 이동 속도를 기본 속도로 복구
    }

    // 회복 아이템 사용 중 취소 처리
    [PunRPC]
    void CancelHeal()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
            healingCoroutine = null;
        }
        anim.SetBool("isHeal", false); // 힐 애니메이션 종료
        playerMovement.moveSpeed = playerMovement.defaultSpeed; // 이동 속도를 기본 속도로 복구

        // 회복 UI 비활성화
        if (inGameUIManager != null)
        {
            inGameUIManager.ShowHealUI(false);
        }

        Debug.Log("Healing canceled.");
    }

    [PunRPC]
    void SyncRotation()
    {
        // 다른 클라이언트도 동일하게 회전 상태로 만듦
        playerMovement.TurnTowardsMouse();
    }

    // 수류탄 사용 로직
    [PunRPC]
    void UseGrenade(Vector3 shootDirection)
    {       
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 캐릭터 위치의 Y 좌표를 높여 수류탄을 생성
        Vector3 grenadeStartPosition = transform.position + new Vector3(0, 1.5f, 0); // Y 좌표를 1.5만큼 높임

        // 수류탄 프리팹 생성
        GameObject grenadeObject = Instantiate(Grenade.itemPrefab, grenadeStartPosition, Quaternion.identity);
                

        // 던지기 (Rigidbody를 통해 목표 위치로 던짐)
        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 던질 방향 계산
            Vector3 throwDirection = transform.forward.normalized;
            float throwForce = 500f; // 던지는 힘의 크기
            rb.AddForce(throwDirection * throwForce);
            anim.SetTrigger("throwItem"); // 던지는 애니메이션 실행
        }

        // 수류탄 던진 후 1초 후에 폭발하도록 설정
        StartCoroutine(ExplodeAfterDelay(grenadeObject, (Grenade)Grenade, ((Grenade)Grenade).explosionRange, ((Grenade)Grenade).damage));        
    }

    // 수류탄이 폭발하는 로직
    IEnumerator ExplodeAfterDelay(GameObject grenadeObject, Grenade grenade, float explosionRange, int damage)
    {
        // 수류탄을 던진 후 1초 뒤에 폭발
        yield return new WaitForSeconds(1f);

        // 폭발 처리
        Explode(grenadeObject, grenade, explosionRange, damage);
    }

    // 폭발 처리 (3D 환경에서 OverlapSphere 사용, 구형 감지)
    void Explode(GameObject grenadeObject, Grenade grenade, float explosionRange, int damage)
    {
        Debug.Log("Grenade exploded!");

        // 폭발 이펙트 실행 (프리팹으로 설정한 폭발 이펙트가 있을 경우 사용)
        if (grenade.explosionEffect != null)
        {
            GameObject explosionEffect = Instantiate(grenade.explosionEffect, grenadeObject.transform.position, Quaternion.identity);
            // 폭발 이펙트는 일정 시간이 지나면 파괴되도록 설정 (예: 1초 후 파괴)
            Destroy(explosionEffect, 1f); // 1초 후 이펙트 파괴
        }

        // 폭발 범위 내의 플레이어에게 데미지 입힘
        Collider[] hits = Physics.OverlapSphere(grenadeObject.transform.position, explosionRange);
        foreach (Collider hit in hits)
        {
            // 수류탄을 던진 플레이어는 제외
            if (hit.gameObject == gameObject)
                continue;

            // PlayerStatus 컴포넌트를 찾아 데미지 적용
            PlayerStatus playerStatus = hit.GetComponent<PlayerStatus>();
            if (playerStatus != null)
            {                  
                playerStatus.TakeDamage(damage);
            }
        }

        // 수류탄 오브젝트 파괴
        Destroy(grenadeObject);
    }


    // 테이저 건 사용 로직
    [PunRPC]
    void UseTaserGun(Vector3 shootDirection)
    {
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 캐릭터 앞에 콜라이더 생성
        GameObject taserColliderObject = new GameObject("TaserCollider");
        taserColliderObject.transform.position = transform.position + transform.forward * 1.5f; // 캐릭터 앞에 생성        

        // 콜라이더 추가
        BoxCollider taserCollider = taserColliderObject.AddComponent<BoxCollider>();
        taserCollider.isTrigger = true; // 트리거로 설정하여 충돌 감지만 수행
        taserCollider.size = new Vector3(0.5f, 1.5f, 0.5f); // 크기 조정 (필요에 따라 조정)

        anim.SetTrigger("useTasergun"); // 테이저건  애니메이션 실행
        // 충돌 처리 로직
        taserColliderObject.AddComponent<Rigidbody>().isKinematic = true; // 콜라이더에 Rigidbody를 추가해 트리거가 작동하게 설정

        // 충돌 처리 컴포넌트 추가
        taserColliderObject.AddComponent<TaserCollisionHandler>().Initialize((TaserGun)TaserGun, this.gameObject);

        // 발사 이펙트 (필요시 프리팹을 연결해야 함)
        if (((TaserGun)TaserGun).shootEffect != null)
        {
            GameObject TaserShootEffect = Instantiate(((TaserGun)TaserGun).shootEffect, transform.position + transform.forward * 1f + new Vector3(0, 1.5f, 0), Quaternion.identity);
            Destroy(TaserShootEffect, 0.5f); // 0.5초 후 이펙트 파괴
        }

        // 일정 시간이 지나면 콜라이더 비활성화 (0.5초 후)
        Destroy(taserColliderObject, 0.5f);
    }

    // 테이저건 상태 적용 로직
    public class TaserCollisionHandler : MonoBehaviour
    {
        private TaserGun taser;
        private GameObject sourcePlayer; // 테이저 건을 사용한 플레이어

        // 초기화 함수
        public void Initialize(TaserGun taser, GameObject sourcePlayer)
        {
            this.taser = taser;
            this.sourcePlayer = sourcePlayer;
        }

        void OnTriggerEnter(Collider other)
        {
            // 자신을 제외한 다른 플레이어에만 충돌 처리
            if (other.gameObject != sourcePlayer)
            {
                PlayerStatus enemyStatus = other.GetComponent<PlayerStatus>();

                if (enemyStatus != null)
                {
                    // 적을 기절 상태로 만듦
                    enemyStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Stunned);
                    Debug.Log("Enemy stunned for " + taser.stunTime + " seconds.");
                }
            }
        }
    }


    // 끈끈이 물약 사용 로직
    [PunRPC]
    public void UseGlue(Vector3 shootDirection)
    {
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 캐릭터 위치의 Y 좌표를 높여 끈끈이 물약을 생성
        Vector3 startPosition = transform.position + new Vector3(0, 1.5f, 0);

        // 끈끈이 물약 프리팹 생성
        GameObject glueObject = Instantiate(((Glue)Glue).itemPrefab, startPosition, Quaternion.identity);

        // 던지기 (Rigidbody를 통해 목표 위치로 던짐)
        Rigidbody rb = glueObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 던질 방향 계산
            Vector3 throwDirection = transform.forward.normalized;
            float throwForce = 500f; // 던지는 힘의 크기
            rb.AddForce(throwDirection * throwForce);
            anim.SetTrigger("throwItem"); // 던지는 애니메이션 실행
        }

        // GlueCollision 컴포넌트를 추가하여 충돌 처리 준비
        GlueCollision glueCollision = glueObject.AddComponent<GlueCollision>();
        glueCollision.Initialize(((Glue)Glue));        
    }

    // 끈끈이 물약이 던져진 이후 로직
    public class GlueCollision : MonoBehaviour
    {
        private Glue glueData;

        // 끈끈이 데이터를 초기화하는 메서드
        public void Initialize(Glue glue)
        {
            glueData = glue;
        }

        // 물약이 바닥에 닿을 때 호출되는 메서드
        void OnCollisionEnter(Collision collision)
        {
            // 태그가 "Ground"인 오브젝트와 충돌했는지 확인
            if (collision.gameObject.CompareTag("Ground"))
            {
                Debug.Log("Glue collided with the ground. Spreading glue...");

                // 충돌 지점에 끈끈이 퍼지기
                SpreadGlue(transform.position);

                // 물약 오브젝트 파괴
                Destroy(gameObject);
            }
        }

        // 끈끈이 퍼지는 로직
        void SpreadGlue(Vector3 position)
        {
            // 끈끈이가 퍼지는 이펙트 생성
            GameObject stickyEffect = Instantiate(glueData.stickyPrefab, position, Quaternion.identity);
            Destroy(stickyEffect, 5.0f); // 끈끈이 5초 지속 후 파괴

            // 끈끈이가 퍼지는 애니메이션 (크기를 늘리면서 바닥에 퍼지게)
            stickyEffect.transform.localScale = new Vector3(glueData.spreadRange, 1f, glueData.spreadRange);

            // 끈끈이 영역에 MeshCollider 추가 (충돌 감지용)
            stickyEffect.AddComponent<GlueEffect>(); // 플레이어와의 충돌 처리
        }
    }


    // 덫 사용 로직
    [PunRPC]
    void UseTrap(Vector3 shootDirection)
    {
     
        // 회전 동기화: 전달받은 shootDirection으로 회전
        transform.rotation = Quaternion.LookRotation(shootDirection);

        // 캐릭터 위치의 Y 좌표를 높여 덫을 생성
        Vector3 startPosition = transform.position + new Vector3(0, 1.5f, 0);

        // 덫 프리팹 생성
        GameObject trapObject = Instantiate(((Trap)Trap).itemPrefab, startPosition, Quaternion.identity);

        // 던지기 (Rigidbody를 통해 목표 위치로 던짐)
        Rigidbody rb = trapObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 던질 방향 계산
            Vector3 throwDirection = transform.forward.normalized;
            float throwForce = 300f; // 던지는 힘의 크기
            rb.AddForce(throwDirection * throwForce);
            anim.SetTrigger("throwItem"); // 던지는 애니메이션 실행
        }

        // 덫이 Ground에 닿으면 고정시키기 위한 컴포넌트 추가
        TrapCollision trapCollision = trapObject.AddComponent<TrapCollision>();
        trapCollision.InitializeTrap(rb);

    }

    public class TrapCollision : MonoBehaviour
    {
        private Rigidbody rb;
        private bool isSet = false;  // 덫이 고정되었는지 여부를 확인하는 플래그
        private Collider trapCollider;  // 덫의 Collider를 저장

        // 덫의 Rigidbody를 초기화하는 메서드
        public void InitializeTrap(Rigidbody trapRigidbody)
        {
            rb = trapRigidbody;
            trapCollider = GetComponent<Collider>();  // 덫의 Collider를 가져옴
        }

        // 충돌이 발생했을 때 호출되는 메서드
        void OnCollisionEnter(Collision collision)
        {
            // Ground와 충돌했는지 확인
            if (collision.gameObject.CompareTag("Ground") && !isSet)
            {
                Debug.Log("Trap collided with the ground. Trap is set!");

                // 덫을 고정시키기 위해 Rigidbody 비활성화
                if (rb != null)
                {
                    rb.isKinematic = true;  // Rigidbody를 비활성화하여 덫이 움직이지 않게 함
                    rb.velocity = Vector3.zero;  // 속도 초기화
                    rb.angularVelocity = Vector3.zero;  // 각속도 초기화
                }

                // 덫이 고정된 후 Collider를 트리거로 설정
                if (trapCollider != null)
                {
                    trapCollider.isTrigger = true;  // 덫이 땅에 고정된 후 트리거로 전환
                }

                // 덫의 레이어를 플레이어와 충돌 가능한 레이어로 변경
                gameObject.layer = LayerMask.NameToLayer("Default");  // Trap 레이어로 변경

                // 덫이 땅에 고정되었음을 표시하는 플래그 설정
                isSet = true;
            }
        }

        // 트리거가 발생했을 때 호출되는 메서드 (덫이 고정된 이후)
        void OnTriggerEnter(Collider other)
        {
            // 플레이어가 덫을 밟았을 때 상태이상 적용
            if (isSet && other.CompareTag("Player"))
            {
                PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
                if (playerStatus != null)
                {
                    // 플레이어 상태를 속박(Immobilized)으로 변경
                    playerStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Immobilized);
                    Debug.Log("Player is immobilized by the trap!");

                    // 덫을 제거
                    Destroy(gameObject);
                }
            }
        }
    }


}