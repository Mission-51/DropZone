using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    private Rigidbody rb;
    private Animator anim;

    public float defaultSpeed; // 기본 이동 속도
    public float moveSpeed;
    public float dashSpeedMultiplier = 2.0f; // 대쉬할 때 속도 배율
    public float dashDuration = 0.3f; // 대쉬 지속 시간
    public float dashCooldown; // 대쉬 쿨타임

    public bool canMove = true; // 이동 가능 여부
    public bool canDash = true; // 대쉬 가능 여부
    public bool isKnockbackImmune = false; // 넉백 면역 상태 여부

    private float hAxis;
    private float vAxis;
    public bool isDash; // 대쉬 상태 확인
    private bool isAttack; // 공격 상태 확인
    private bool isRangedAttack; // 원거리 공격 상태 확인

    public float lastDashTime = -100f;  // 마지막 대쉬 시간
    private Vector3 moveVec;
    private Vector3 rayVec;

    // 상태 제어를 위해 PlayerStatus 연결
    private PlayerStatus playerStatus;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 벽 뚫는 현상 처리 방지
        rb.interpolation = RigidbodyInterpolation.Extrapolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        anim = GetComponent<Animator>();
        moveSpeed = defaultSpeed; // 기본 이동 속도로 초기화

        playerStatus = GetComponent<PlayerStatus>(); // PlayerStatus 컴포넌트 가져오기
                                                     // UIManager 스크립트를 찾아 참조



        // PhotonView가 필요: PhotonView를 추가하여 플레이어의 위치 및 이동 동작을 네트워크 상에 동기화해야 함
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            GetInput(); // 입력 받기
            Turn(); // 회전 처리

            // 캐릭터의 높이가 -30 이하로 떨어지면 dead 상태로 변경
            if (transform.position.y < -30f)
            {
                playerStatus.ApplyStatusEffect(PlayerStatus.StatusEffect.Dead);
            }
        }
        
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            Move(); // 이동 처리       
        }
    }

    public void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        // 대쉬와 같은 중요한 입력은 다른 클라이언트에도 전파되어야 하므로, RPC로 대쉬 입력을 전송해야 함
        // PhotonView를 통해 Dash 관련 입력을 RPC로 전송할 필요가 있음
        if (Input.GetButtonDown("Fire3") && (playerStatus.currentStatus == PlayerStatus.StatusEffect.None || playerStatus.currentStatus == PlayerStatus.StatusEffect.SuperArmor) && !isAttack)
        {
            StartDash();
        }
    }

    public void Move()
    {
        if (!canMove) return;

        if (isRangedAttack) { return; } // 원거리 공격 중일 때는 이동 불가

        // 공격 중일 때 이동 속도를 반으로 줄임
        float adjustedMoveSpeed = isAttack ? moveSpeed / 2 : moveSpeed;

        // 상태에 따라 이동 불가 처리 (속박 상태인 경우)
        if (playerStatus.currentStatus == PlayerStatus.StatusEffect.Immobilized)
        {
            adjustedMoveSpeed = 0;
        }

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        // 벽 감지용 Ray 설정
        RaycastHit hit;
        float rayDistance = 1.0f; // 캐릭터 앞에 Ray를 쏠 거리

        // 캐릭터 바닥보다 위에서 ray를 쏘기 위해 위치 조정
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        // 디버그용 Ray 그리기
        Debug.DrawRay(rayOrigin, moveVec * rayDistance, Color.red);

      

        // 이동 방향으로 Ray를 쏨
        if (Physics.Raycast(rayOrigin, moveVec, out hit, rayDistance))
        {
            // 벽이 감지되면 이동 중지 (벽이 "Wall" 태그를 가진 경우)
            if (hit.collider.CompareTag("Untagged"))
            {
                return; // 이동 중지
            }
        }

        //transform.position += moveVec * adjustedMoveSpeed * Time.deltaTime; // 기존 방식 제거
        rb.MovePosition(transform.position + moveVec * adjustedMoveSpeed * Time.deltaTime); // Rigidbody를 통해 이동 처리

        anim.SetBool("isRun", moveVec != Vector3.zero); // 이동 애니메이션 실행
    }
  
    private void StartDash()
    {
        if (Time.time >= lastDashTime + dashCooldown && moveVec != Vector3.zero && !isAttack)
        {
            isDash = true;
            moveSpeed = moveSpeed * dashSpeedMultiplier; // 대쉬 시 속도 증가
            lastDashTime = Time.time;

            anim.SetTrigger("doDash"); // 대쉬 애니메이션 실행            

            // 대쉬 동작도 네트워크 상에 동기화해야 하므로 PhotonView를 통해 RPC 호출 필요
            Invoke("EndDash", dashDuration); // 대쉬 지속 시간이 지나면 종료
        }
    }

    private void EndDash()
    {
        isDash = false;
        moveSpeed = defaultSpeed; // 대쉬가 끝나면 원래 속도로 복구        
    }

    public void Turn()
    {
        // 공격, 대쉬 중일 때는 회전하지 않음
        if (isAttack || isDash) return;

        // 이동 중일 때는 이동 방향으로 회전
        if (moveVec != Vector3.zero && !isAttack)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveVec);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }        
    }

    // 공격할 때 마우스 방향으로 회전
    public void TurnTowardsMouse()
    {
        // 마우스 방향으로 즉시 회전
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        //{
        //    // 충돌한 오브젝트가 "Untagged"라면 무시
        //    if (hit.collider.CompareTag("Untagged"))
        //    {
        //        return; // "Untagged" 오브젝트일 경우 함수 종료
        //    }


        //    Vector3 targetPosition = hit.point;
        //    targetPosition.y = transform.position.y; // 캐릭터의 높이(y)는 유지

        //    // 즉시 회전
        //    transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        //}

        // Ray가 충돌하는 모든 오브젝트 가져오기
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
        foreach (RaycastHit hit in hits)
        {
            // "Untagged" 오브젝트는 무시
            if (!hit.collider.CompareTag("Untagged"))
            {
                Vector3 targetPosition = hit.point;
                targetPosition.y = transform.position.y; // 캐릭터의 높이(y)는 유지

                // 캐릭터 회전
                transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
                break; // 유효한 태그를 찾았으니 루프 종료
            }
        }

    }

    // 공격 상태를 업데이트하는 메서드 (외부에서 호출 가능)
    public void SetAttackState(bool isAttacking)
    {
        isAttack = isAttacking;
    }

    // 원거리 공격 상태를 업데이트하는 메서드 추가
    public void SetRangedAttackState(bool isRangedAttacking)
    {
        isRangedAttack = isRangedAttacking;
    }

    // 이동 가능 여부 동기화
    [PunRPC]
    public void UpdateCanMove(bool value)
    {
        Debug.Log($"무브번트안에 이동가능여부 체크{value}, {canMove}");
        canMove = value;        
    }
}
