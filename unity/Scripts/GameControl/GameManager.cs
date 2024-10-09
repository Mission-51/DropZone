using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class PlayerStats
{
    public string playerName;
    public int kills;
    public float damageDeal;
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] characterPrefabs; // 캐릭터 프리팹 목록
    private List<Transform> spawnPoints = new List<Transform>(); // 스폰 포인트 목록
    private int spawnidx;
    public bool canAttack; // 공격 가능 여부
    public bool canMove; // 이동 가능 여부

    private List<GameObject> players = new List<GameObject>(); // 현재 스폰된 플레이어 목록
    public bool isGameOver = false; // 게임 종료 여부
    private Dictionary<string, PlayerStats> playerStatsDict = new Dictionary<string, PlayerStats>(); // 플레이어 통계 저장
    private static GameManager m_instance; // 싱글톤 인스턴스
        
    private InGameUIManager inGameUIManager; // 인게임 UI 매니저 연결
    private StoreUI storeUI; // 상점 UI 연결

    public static GameManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<GameManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 이동과 공격 가능 여부 초기 설정
        canAttack = true;
        canMove = true;

        updateSpawnList();
        // 방에 연결되어 있다면 플레이어 스폰
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            reNewSpawnPlayers();
        }
    }

    public void updateSpawnList()
    {
        Transform[] positions = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();
        foreach (Transform pos in positions)
        {
            spawnPoints.Add(pos);
        }
    }

    public void reNewSpawnPlayers()
    {
        var player = PhotonNetwork.LocalPlayer;
        spawnidx = Random.Range(0, spawnPoints.Count);
        GameObject characterPrefab = GetCharacterPrefab(player);

        // 캐릭터를 인스턴스화
        GameObject character = PhotonNetwork.Instantiate(characterPrefab.name, spawnPoints[spawnidx].position, Quaternion.identity);
        Debug.Log("Spawn Point : " + spawnPoints[spawnidx]);
        spawnPoints.RemoveAt(spawnidx);    

        // 로컬 플레이어일 경우 Inventory에 설정
        if (player.IsLocal)
        {
            // TagObject 설정 - 로컬 플레이어의 오브젝트 참조
            PhotonNetwork.LocalPlayer.TagObject = character;

            Inventory inventory = FindObjectOfType<Inventory>();
            inventory.SetPlayer(character);
                        
            // 인게임 UI 매니저에 로컬 캐릭터 설정
            inGameUIManager = FindObjectOfType<InGameUIManager>();  // 인게임 UI 매니저 찾기
            if (inGameUIManager != null)
            {
                inGameUIManager.SetPlayer(character);
            }

            // Store UI 찾기 및 설정
            storeUI = FindObjectOfType<StoreUI>(); // 상점 UI 찾기
            if (storeUI != null)
            {
                storeUI.SetPlayer(character);
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

    [PunRPC]
    public void DeclareWinner(string winnerName)
    {
        Debug.Log(winnerName + " 경기 승리!");
        // 승자 정보를 처리하는 추가 코드 필요
    }

    private void Update()
    {
        // 플레이어가 1명 남았을 때 승자 선언
        if (players.Count == 1)
        {
            var winnerName = players[0].GetComponent<PhotonView>().Owner.NickName;
            DeclareWinner(winnerName);
        }
    }

    public void PlayerEliminated(GameObject player)
    {
        players.Remove(player);
        PhotonNetwork.Destroy(player);
        var playerName = player.GetComponent<PhotonView>().Owner.NickName;
        if (playerStatsDict.ContainsKey(playerName))
        {
            playerStatsDict[playerName].kills++;
        }
    }

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public Dictionary<string, PlayerStats> GetPlayerStats()
    {
        return playerStatsDict;
    }

    // 캐릭터 공격 제어 동기화
    public void SetCanAttackForPlayer(GameObject player, bool value)
    {
        Debug.Log($"공격제어를 동기화 합니다. {player}");
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();
        if (playerPhotonView != null)
        {
            playerPhotonView.RPC("UpdateCanAttack", RpcTarget.AllBuffered, value);
            Debug.Log($"공격상태제어 : {value}");
        }
    }

    // 캐릭터 이동 제어 동기화
    public void SetCanMoveForPlayer(GameObject player, bool value)
    {
        Debug.Log($"이동제어를 동기화 합니다. {player}");
        PhotonView playerPhotonView = player.GetComponent<PhotonView>();

        if (playerPhotonView != null)
        {
            playerPhotonView.RPC("UpdateCanMove", RpcTarget.AllBuffered, value);
            Debug.Log($"이동상태제어 : {value}");
        }
    }
}
