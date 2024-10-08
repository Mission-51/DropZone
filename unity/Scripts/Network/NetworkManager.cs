using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject lobbyPanel;
    public GameObject waitingPanel;
    public GameObject LobbyMasterServerConnect;

    private static NetworkManager m_instance;
    private string selCharacter;
    private string gameVersion = "1"; // 게임 버전

    public static NetworkManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<NetworkManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // 초기화 로직
    }

    public void ServerConnect()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = LobbyManager.instance.GetNick();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속했습니다.");
        // lobbyPanel.SetActive(true); // 로비 UI 활성화
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 완료");
        PhotonNetwork.JoinLobby(); // 로비에 접속
        LobbyMasterServerConnect.SetActive(false);
    }

    public void OnQuickMatchButtonClicked()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties
                (new ExitGames.Client.Photon.Hashtable
                { { "character", selCharacter },
                      { "nickname", LobbyManager.instance.GetNick() } });
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            Debug.LogWarning("서버에 연결되지 않았거나 로비에 없습니다.");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("방이 없어 새로운 방을 생성합니다.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 2 }); // 최대 2명
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("새로운 방이 생성되었습니다.");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("유저가 방에 들어왔습니다.");
        UpdateMatchingUser();
        // 방에 입장할 때마다 플레이어 수 체크
        CheckPlayersAndLoadScene();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("유저 접속함");
        // 새로운 유저가 방에 들어왔을 때 플레이어 수 체크
        UpdateMatchingUser();
        CheckPlayersAndLoadScene();
    }

    private void CheckPlayersAndLoadScene()
    {
        // 방의 플레이어 수가 최대 플레이어 수와 같으면 씬 로드
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("모든 유저가 방에 들어왔습니다. 씬을 로드합니다.");
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    public void setSelCharacter(string idx)
    {
        this.selCharacter = idx;
        Debug.Log(idx);
    }

    // 매칭 취소
    public void OnCancelMatchmakingButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            UpdateMatchingUser();
            Debug.Log("매칭이 취소되고 방을 나갔습니다.");
        }
        else
        {
            Debug.Log("현재 방에 없습니다.");
        }
    }

    // 매칭 취소
    public void MatchStop()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            UpdateMatchingUser();
            Debug.Log("매칭이 취소되고 방을 나갔습니다.");
        }
        else
        {
            Debug.Log("현재 방에 없습니다.");
        }
    }

    public void UpdateMatchingUser()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
        LobbyManager.instance.SetCurrentUser($"{PhotonNetwork.CurrentRoom.PlayerCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}");
    }
}
