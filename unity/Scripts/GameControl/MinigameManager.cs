using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MinigameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    // 싱글톤 구현
    public static MinigameManager instance = null;
    // 상자
    public GameObject chest;
    // 돈 300원
    public ItemData money;
    // 유저 스폰포인트
    public List<List<Vector3>> playerPositions;
    // (테스트용) 플레이어 프리펩
    public GameObject playerPrefab;
    // 공격 가능 여부
    public bool canAttack;
    // 라운드별 맞는 포탈 번호
    public int roundOneNum;
    public int roundTwoNum;
    public int roundThreeNum;
    public int roundFourNum;
    // 캐릭터 프리펩들
    public GameObject[] characterPrefabs;
    // 게임오버 메시지 (게임 오브젝트)
    public GameObject go_GameOverMsg;
    // 게임오버 메시지
    public TextMeshProUGUI gameOverMsg;
    // 게임오버 타이머
    public TextMeshProUGUI gameOverTimer;
    // 게임 종료여부 확인
    public bool isGameOver = false;
    // 우승자 닉네임
    public string nickname;

    // 플레이어 목록
    private List<GameObject> players = new List<GameObject>();
    // 상자 스크립트
    private Chest chestScript;
    // 타이머 (5 > 0)
    private float timer = 5;

    

    private void Start()
    {
        // 미니게임에서는 공격, 스킬사용 불가능
        canAttack = false;

        roundOneNum = Random.Range(0, 3);
        roundTwoNum = Random.Range(0, 3);
        roundThreeNum = Random.Range(0, 3);
        roundFourNum = Random.Range(0, 3);

        // 방에 연결되어 있다면 플레이어 스폰
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            SpawnPlayers();
        }
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        //chestScript = chest.GetComponent<Chest>();
        //chestScript.AddItem(money);

        // 플레이어별 스폰 포인트들 만들어주기
        playerPositions = new List<List<Vector3>>();
        // 플레이어별 x좌표: 1: -5, 2: -3, 3: -1, 4: 1, 5: 3, 6: 5
        for (int i = 0; i < 6; i++)
        {
            // 자리 할당해주기
            playerPositions.Add(new List<Vector3>());
            // 각 플레이어들의 x좌표
            int xPos = -5 + 2 * i;
            for (int j = 0; j < 5; j++)
            {
                // y좌표 (플레이어별 동일)
                int yPos = 60 * j;
                // z좌표 (플레이어별 동일)
                int zPos = -8 + 20 * j;

                // 추가할 좌표값 (Vector3)
                Vector3 roundPos = new Vector3(xPos, yPos, zPos);
                // 플레이어포지션에 추가
                playerPositions[i].Add(roundPos);
            }
        }
    }

    private void Update()
    {
        if (isGameOver)
        {
            go_GameOverMsg.SetActive(true);
            gameOverMsg.text = $"<color=red>{nickname}</color>님이 게임에서 승리했습니다!\n\n본 게임 시작까지";
            gameOverTimer.text = Mathf.Round(timer).ToString();
            timer -= Time.deltaTime;
            StartCoroutine("GameSet");
        }
    }

    public void SpawnPlayers()
    {
        Debug.Log("현재 플레이어 수: " + PhotonNetwork.PlayerList.Length);

        foreach (var player in PhotonNetwork.PlayerList.Select((value, index) => new { value, index }))
        {
            // 이미 존재하는 캐릭터인지 확인
            if (!players.Exists(p => p.GetComponent<PhotonView>().Owner.NickName == player.value.NickName))
            {
                GameObject characterPrefab = GetCharacterPrefab(player.value);
                if (characterPrefab != null)
                {
                    GameObject character = PhotonNetwork.Instantiate(characterPrefab.name, playerPositions[player.index][0], Quaternion.identity);
                    players.Add(character);
                    Debug.Log("스폰된 캐릭터: " + characterPrefab.name);
                    Debug.Log("플레이어 이름: " + player.value.NickName);
                    SetPlayerNickname(character, player.value.NickName); // 닉네임 설정
                }
                else
                {
                    Debug.LogError("캐릭터 프리팹을 찾을 수 없습니다: " + player.value.NickName);
                }
            }
        }
    }

    private void SetPlayerNickname(GameObject character, string nickname)
    {
        Transform playerInfoCanvas = character.transform.Find("PlayerInfoCanvas");
        if (playerInfoCanvas != null)
        {
            Transform nicknameEmptyObject = playerInfoCanvas.Find("NickName");
            if (nicknameEmptyObject != null)
            {
                TMP_Text playerNameText = nicknameEmptyObject.GetComponentInChildren<TMP_Text>();
                if (playerNameText != null)
                {
                    playerNameText.text = nickname; // 닉네임 설정
                }
            }
        }
    }

    public GameObject GetCharacterPrefab(Player player)
    {
        if (!player.CustomProperties.ContainsKey("character"))
        {
            Debug.LogError("플레이어의 캐릭터 선택 정보가 없습니다: " + player.NickName);
            return null;
        }

        string characterName = (string)player.CustomProperties["character"];
        foreach (var prefab in characterPrefabs)
        {
            if (prefab.name == characterName)
            {
                return prefab;
            }
        }
        return null;
    }

    public IEnumerator GameSet()
    {
        yield return new WaitForSeconds(timer);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGameOver);
        }
        else
        {
            isGameOver = (bool)stream.ReceiveNext();
            if (isGameOver)
            {
                PhotonNetwork.LoadLevel("MainScene");
            }
        }
    }

    public void TriggerGameOver(string winnerNickname)
    {
        isGameOver = true;
        nickname = winnerNickname;
    }

    public void reNewSpawnPlayers()
    {
        var player = PhotonNetwork.LocalPlayer;
        GameObject characterPrefab = GetCharacterPrefab(player);
        GameObject character = PhotonNetwork.Instantiate(characterPrefab.name, playerPositions[0][0], Quaternion.identity);
        SetPlayerNickname(character, (string)player.CustomProperties["nickname"]);
    }
}