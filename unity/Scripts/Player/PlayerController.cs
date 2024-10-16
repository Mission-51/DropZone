using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
    public PlayerMovement playerMovement;
    public PlayerStatus playerStatus;
    public IAttack playerAttack;
    public WeaponManager weaponManager;

    void Awake()
    {
        playerAttack = GetComponent<IAttack>();
    }

    void Update()
    {
        // 플레이어가 죽었다면 더 이상 업데이트하지 않음
        if (playerStatus.currentStatus == PlayerStatus.StatusEffect.Dead)
        {
            return;
        }

        // 현재 클라이언트의 플레이어인지 확인
        if (photonView.IsMine)
        {
            playerMovement.GetInput(); // 입력을 받음
            playerMovement.Move(); // 움직임 처리

            // 공격 입력 처리
            if (Input.GetButton("Fire1"))
            {                
                PerformAttack();
            }
        }
    }
        
    public void PerformAttack()
    {
        // 공격 입력을 처리
        playerAttack.GetAttackInput(true);
    }
}