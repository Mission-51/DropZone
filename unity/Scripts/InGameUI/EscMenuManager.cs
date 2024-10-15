using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscMenuManager : MonoBehaviourPunCallbacks
{
    public GameObject escMenuCanvas; // EscMenuCanvas 참조
    private bool isEscMenuOpen = false;

    void Start()
    {
        // 게임 시작 시 PauseMenu는 비활성화
        escMenuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // ESC 키 입력 처리
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 메뉴가 열려있다면
            if (isEscMenuOpen)
            {
                EscMenuClose(); // 닫기
            }
            else
            {
                EscMenuOpen(); // 열기
            }
        }
    }

    // 게임을 일시 정지하고 메뉴를 표시
    public void EscMenuOpen()
    {
        escMenuCanvas.SetActive(true);        
        isEscMenuOpen = true;
                
        // ESC 메뉴 켰을 시 공격 불가
        if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.TagObject != null)
        {
            GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            GameManager.Instance.SetCanAttackForPlayer(player, false);            
        }
    }

    // 게임 재개
    public void EscMenuClose()
    {        
        escMenuCanvas.SetActive(false);
        isEscMenuOpen = false;
                
        // ESC 메뉴 종료 시 공격 재개
        if (PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.TagObject != null)
        {
            GameObject player = PhotonNetwork.LocalPlayer.TagObject as GameObject;
            GameManager.Instance.SetCanAttackForPlayer(player, true);            
        }
    }

    // 로비로 돌아가는 함수
    public void ExitToLobby()
    {
        // PhotonNetwork에서 방 나가기
        if (PhotonNetwork.InRoom)
        {
            //GameManager.Instance.ResetGameManager();
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            // 방에 없는 경우 바로 로비 씬으로 이동
            //GameManager.Instance.ResetGameManager();
            //SceneManager.LoadScene("LobbyScene");

        }
    }

    // 방을 성공적으로 떠났을 때 호출되는 콜백
    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene("LobbyScene"); // 로비 씬으로 이동
    }
}
