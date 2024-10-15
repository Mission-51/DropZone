using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance { get; private set; }

    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject waitingPanel;
    [SerializeField] private GameObject LobbyMasterServerConnect;

    private string selCharacter;
    private string gameVersion = "1";
    private const float MaxWaitTime = 60f;
    private const int MaxRoomUser = 6;
    private const string GameStartedKey = "GameStarted";
    private Coroutine gameStartCoroutine;
    private bool isGameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ServerConnect()
    {
        Debug.Log("서버 연결 시도");
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = LobbyManager.instance.GetNick();
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 완료");
        PhotonNetwork.JoinLobby();
        LobbyMasterServerConnect.SetActive(false);
    }

    public void OnQuickMatchButtonClicked()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby)
        {
            Debug.Log("빠른 매치 시작");
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
            {
                { "character", selCharacter },
                { "nickname", LobbyManager.instance.GetNick() },
                { "money", 0 }
            });
            PhotonNetwork.JoinRandomRoom(new Hashtable { { GameStartedKey, false } }, MaxRoomUser);
        }
        else
        {
            Debug.LogWarning("서버에 연결되지 않았거나 로비에 없습니다.");
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("적합한 방이 없어 새로운 방을 생성합니다.");
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = MaxRoomUser,
            CustomRoomProperties = new Hashtable { { GameStartedKey, false } },
            CustomRoomPropertiesForLobby = new string[] { GameStartedKey }
        };
        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"새 플레이어 입장: {newPlayer.NickName}. 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}/{MaxRoomUser}");
        UpdateMatchingUser();
        CheckPlayersAndStartTimer();
    }

    private void CheckPlayersAndStartTimer()
    {
        if (PhotonNetwork.IsMasterClient && !isGameStarted)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == MaxRoomUser)
            {
                Debug.Log("최대 인원 도달. 게임 즉시 시작");
                StartGame();
            }
            //else if (PhotonNetwork.CurrentRoom.PlayerCount <= 2 && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            //{
            //    Debug.Log($"{MaxWaitTime}초 타이머 시작");
            //    if (gameStartCoroutine != null)
            //    {
            //        StopCoroutine(gameStartCoroutine);
            //    }
            //    gameStartCoroutine = StartCoroutine(StartGameAfterDelay());
            //}
        }
    }

    private IEnumerator StartGameAfterDelay()
    {
        float elapsedTime = 0f;
        while (elapsedTime < MaxWaitTime)
        {
            if (isGameStarted || PhotonNetwork.CurrentRoom.PlayerCount == MaxRoomUser)
            {
                yield break; // 게임이 이미 시작되었거나 최대 인원에 도달한 경우 코루틴 종료
            }
            yield return new WaitForSeconds(1f);
            elapsedTime += 1f;
        }

        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= 2 && !isGameStarted)
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (PhotonNetwork.IsMasterClient && !isGameStarted)
        {
            Debug.Log("게임 시작 신호 전송");
            isGameStarted = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(new Hashtable { { GameStartedKey, true } });
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    public void UpdateMatchingUser()
    {
        if (PhotonNetwork.InRoom)
        {
            LobbyManager.instance.SetCurrentUser($"{PhotonNetwork.CurrentRoom.PlayerCount} / {MaxRoomUser}");
        }
        else
        {
            LobbyManager.instance.SetCurrentUser("0 / 0");
        }
    }

    public void setSelCharacter(string idx)
    {
        this.selCharacter = idx;
        Debug.Log($"선택된 캐릭터: {idx}");
    }

    public void OnCancelMatchmakingButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            if (gameStartCoroutine != null)
            {
                StopCoroutine(gameStartCoroutine);
                gameStartCoroutine = null;
            }
            UpdateMatchingUser();
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.TryGetValue(GameStartedKey, out object gameStartedObj))
        {
            isGameStarted = (bool)gameStartedObj;
            if (isGameStarted)
            {
                if (gameStartCoroutine != null)
                {
                    StopCoroutine(gameStartCoroutine);
                    gameStartCoroutine = null;
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"방 입장 완료. 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}/{MaxRoomUser}");
        UpdateMatchingUser();

        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameStartedKey, out object gameStartedObj))
        {
            isGameStarted = (bool)gameStartedObj;
        }

        if (!isGameStarted)
        {
            CheckPlayersAndStartTimer();
        }
    }

    public override void OnLeftRoom()
    {
        Debug.Log("방에서 나감");
        UpdateMatchingUser();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"연결 끊김: {cause}");
        LobbyMasterServerConnect.SetActive(true);
    }
}