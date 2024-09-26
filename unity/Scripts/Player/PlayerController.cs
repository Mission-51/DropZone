using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public PlayerStatus playerStatus; // PlayerStatus 참조
    public IAttack playerAttack; // 공통 인터페이스로 공격 스크립트 참조
    public WeaponManager weaponManager; // WeaponManager 참조    

    void Awake()
    {
        playerAttack = GetComponent<IAttack>(); // 동적으로 공격 스크립트를 할당
        // PhotonView가 필요: PhotonView를 할당하여 네트워크 상에서 이 플레이어가 내 것인지 확인하는 용도로 사용
    }

    void Update()
    {
        //플레이어가 죽은 상태일 때는 아무 동작도 하지 않음
        if (playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return;
        }

        // 이 부분에 PhotonView.IsMine 체크가 필요함: 로컬 플레이어가 아닐 경우 이동과 공격 등의 입력을 받지 않도록 처리
        playerMovement.GetInput(); // 이동 입력 받기
        playerMovement.Move(); // 이동        

        // 공격은 다른 클라이언트에 동기화해야 하므로 RPC로 전송
        // 이 부분에 PhotonView를 통해 RPC 호출이 필요함
        playerAttack.GetAttackInput(Input.GetButton("Fire1")); // 공격 입력

        // 무기 변경도 다른 클라이언트에 동기화해야 하므로 RPC로 전송
        // 무기 변경에 대한 처리도 PhotonView를 통해 동기화가 필요함
        weaponManager.HandleWeaponSwitch(); // 무기 변경 및 이펙트 변경 처리       
    }
}
