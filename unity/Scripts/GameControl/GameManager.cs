using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[System.Serializable]
public class UserRecords
{
    public int userId;
    public int character_id;
    public int match_rank;
    public int match_kills;
    public string match_playtime;
}

[System.Serializable]
public class UserRecordList
{
    public List<UserRecords> userRecordList;
}

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance { get; private set; }

    [Header("Prefabs and Spawning")]
    public GameObject[] characterPrefabs;
    private List<GameObject> players = new List<GameObject>();
    private Dictionary<int, GameObject> spawnedPlayers = new Dictionary<int, GameObject>();
    private List<Transform> spawnPoints = new List<Transform>();

    [Header("Game State")]
    public bool canAttack;
    public bool canMove;
    public bool isGameOver = false;
    public string winnerNickname;
    private bool isMinigameCompleted = false;
    private bool allPlayersLoaded = false;

    [Header("UI References")]
    public GameObject go_GameOverMsg;
    public TextMeshProUGUI gameOverMsg;
    public TextMeshProUGUI gameOverTimer;
    public TextMeshProUGUI survivorCountText;
    private InGameUIManager inGameUIManager;
    private StoreUI storeUI;
    private GameOverUI gameOverUI;
    public GameObject gameOVER;

    [Header("Player Stats")]
    private Dictionary<string, UserRecords> playerStatsDict = new Dictionary<string, UserRecords>();
    private UserRecordList userData = new UserRecordList { userRecordList = new List<UserRecords>() };
    private int survivorCount;

    [Header("Mini Game")]
    public List<List<Vector3>> playerPositions;
    public int[] roundPortalNumbers = new int[4];
    public GameObject chest;
    public ItemData money;
    public float miniGameSpeed = 5f;
    private Dictionary<string, float> originalSpeeds = new Dictionary<string, float>();

    [Header("Misc")]
    private MapDropper mapDropper;
    private KillLog killLog;
    private const string API_URL = "https://j11d110.p.ssafy.io/api/matches/statistics/record";
    private const string API_URL2 = "https://j11d110.p.ssafy.io/api/users/search/user_nickname/";
    private bool statsAlreadySent = false;
    private HashSet<string> processedKills = new HashSet<string>();
    private HashSet<int> loadedPlayers = new HashSet<int>();
    private Dictionary<string, int> playerApiIds = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeCommonVariables();
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        StartCoroutine(FetchAllPlayerIds());
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncGameManagerInstance", RpcTarget.AllBuffered);
        }
        killLog = FindObjectOfType<KillLog>();
    }

    private void InitializeCommonVariables()
    {
        canAttack = true;
        canMove = true;
    }

    [PunRPC]
    private void SyncGameManagerInstance()
    {
        Instance = this;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainScene")
        {
            StartCoroutine(InitializeMainGameScene());
        }
        else if (scene.name == "MiniGameScene")
        {
            StartCoroutine(InitializeMiniGameScene());
        }
    }

    private IEnumerator InitializeMainGameScene()
    {
        Debug.Log("Initializing Main Game Scene");
        yield return new WaitForSeconds(0.5f);

        UpdateSpawnPoints();
        ResetPlayerStats();
        SetupUI();
        mapDropper = FindObjectOfType<MapDropper>();

        survivorCount = PhotonNetwork.PlayerList.Length;
        photonView.RPC("SyncSurvivorCount", RpcTarget.All, survivorCount);

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncMainGameSetup", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void SyncMainGameSetup()
    {
        StartCoroutine(SpawnPlayers());
    }

    private void UpdateSpawnPoints()
    {
        spawnPoints.Clear();
        GameObject spawnPointParent = GameObject.Find("SpawnPoint");
        if (spawnPointParent != null)
        {
            spawnPoints.AddRange(spawnPointParent.GetComponentsInChildren<Transform>());
            spawnPoints.RemoveAt(0);
        }
        Debug.Log($"Found {spawnPoints.Count} spawn points");
    }

    private IEnumerator SpawnPlayers()
    {
        Debug.Log("Starting to spawn players");
        var shuffledSpawnPoints = spawnPoints.OrderBy(x => Random.value).ToList();

        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            Player player = PhotonNetwork.PlayerList[i];
            if (i < shuffledSpawnPoints.Count)
            {
                Vector3 spawnPosition = shuffledSpawnPoints[i].position;
                if (player.IsLocal)
                {
                    SpawnPlayer(player, spawnPosition);
                }
            }
            else
            {
                Debug.LogWarning($"Not enough spawn points for player {player.NickName}");
            }
        }

        yield return new WaitForSeconds(1f);
        photonView.RPC("FinishPlayerSpawning", RpcTarget.All);
    }

    private void SpawnPlayer(Player player, Vector3 spawnPosition)
    {
        GameObject characterPrefab = GetCharacterPrefab(player);
        if (characterPrefab != null)
        {
            GameObject playerObject = PhotonNetwork.Instantiate(characterPrefab.name, spawnPosition, Quaternion.identity);
            players.Add(playerObject);
            spawnedPlayers[player.ActorNumber] = playerObject;
            SetupLocalPlayer(playerObject);
            Debug.Log($"Spawned player {player.NickName} at {spawnPosition}");
        }
        else
        {
            Debug.LogError($"Failed to spawn player {player.NickName}: Character prefab not found");
        }
    }

    [PunRPC]
    private void FinishPlayerSpawning()
    {
        Debug.Log("All players have been spawned");
        if (mapDropper != null)
        {
            mapDropper.StartTimer();
            Debug.Log("MapDropper timer started");
        }
        else
        {
            Debug.LogError("MapDropper is null when trying to start timer!");
        }
    }

    private void SetupUI()
    {
        inGameUIManager = FindObjectOfType<InGameUIManager>();
        storeUI = FindObjectOfType<StoreUI>();
        if (gameOVER != null)
        {
            gameOverUI = gameOVER.GetComponent<GameOverUI>();
        }
    }

    private IEnumerator InitializeMiniGameScene()
    {
        yield return new WaitForSeconds(0.5f);
        InitializePlayerPositions();
        canAttack = false;
        Debug.Log("Mini game initialization started");

        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < 4; i++)
            {
                roundPortalNumbers[i] = Random.Range(0, 3);
            }
            photonView.RPC("SyncPortalNumbers", RpcTarget.All, roundPortalNumbers);
        }
    }

    private void InitializePlayerPositions()
    {
        playerPositions = new List<List<Vector3>>();
        for (int i = 0; i < 6; i++)
        {
            playerPositions.Add(new List<Vector3>());
            int xPos = -5 + 2 * i;
            for (int j = 0; j < 5; j++)
            {
                int yPos = 60 * j;
                int zPos = -8 + 20 * j;
                Vector3 roundPos = new Vector3(xPos, yPos, zPos);
                playerPositions[i].Add(roundPos);
            }
        }
    }

    [PunRPC]
    private void SyncPortalNumbers(int[] numbers)
    {
        roundPortalNumbers = numbers;
    }

    [PunRPC]
    public void PlayerEliminated(int eliminatedViewID, int killerViewID)
    {
        PhotonView eliminatedPlayerView = PhotonNetwork.GetPhotonView(eliminatedViewID);
        PhotonView killerPlayerView = PhotonNetwork.GetPhotonView(killerViewID);

        if (eliminatedPlayerView != null)
        {
            string match_timer = GameObject.Find("Timer").GetComponentInChildren<TextMeshProUGUI>().text;
            GameObject player = eliminatedPlayerView.gameObject;
            string playerName = eliminatedPlayerView.Owner.NickName;

            players.Remove(player);

            survivorCount = Mathf.Max(1, survivorCount - 1);
            photonView.RPC("SyncSurvivorCount", RpcTarget.All, survivorCount);

            if (playerStatsDict.TryGetValue(playerName, out UserRecords eliminatedStats))
            {
                eliminatedStats.match_rank = survivorCount + 1;
                eliminatedStats.match_playtime = match_timer;
            }

            if (killerPlayerView != null)
            {
                string killerPlayerName = killerPlayerView.Owner.NickName;
                if (playerStatsDict.TryGetValue(killerPlayerName, out UserRecords killerStats))
                {
                    killerStats.match_kills++;
                }
            }

            photonView.RPC("HandlePlayerCorpse", RpcTarget.All, eliminatedViewID);

            if (eliminatedPlayerView.IsMine)
            {
                photonView.RPC("AddKillLogRPC", RpcTarget.All, playerName);
                Debug.Log($"Player {playerName} eliminated. Showing GameOver UI for this player.");
                ShowGameOverUI(false);
            }
            else
            {
                Debug.Log($"Player {playerName} eliminated. Not showing GameOver UI for other players.");
            }

            if (survivorCount <= 1)
            {
                CheckForGameEnd(match_timer);
            }
            else
            {
                Debug.Log($"Game continues. Survivors remaining: {survivorCount}");
            }
        }
    }

    private void CheckForGameEnd(string match_timer)
    {
        if (players.Count == 1)
        {
            var winnerName = players[0].GetComponent<PhotonView>().Owner.NickName;
            Debug.Log($"Game ended. Winner: {winnerName}");
            if (playerStatsDict.TryGetValue(winnerName, out UserRecords winnerStats))
            {
                winnerStats.match_rank = 1;
                winnerStats.match_playtime = match_timer;
            }
            photonView.RPC("DeclareWinner", RpcTarget.All, winnerName);
        }
        else
        {
            Debug.LogError($"Unexpected state: CheckForGameEnd called with {players.Count} players remaining");
        }
    }

    [PunRPC]
    public void DeclareWinner(string winnerName)
    {
        Debug.Log($"DeclareWinner called. Winner: {winnerName}");

        isGameOver = true;
        this.winnerNickname = winnerName;

        if (PhotonNetwork.IsMasterClient && !statsAlreadySent)
        {
            StartCoroutine(SendAllPlayerStats());
            statsAlreadySent = true;
        }

        if (PhotonNetwork.LocalPlayer.NickName == winnerName || players.Any(p => p.GetComponent<PhotonView>().Owner == PhotonNetwork.LocalPlayer))
        {
            Debug.Log($"Showing GameOver UI for player {PhotonNetwork.LocalPlayer.NickName}. IsWinner: {PhotonNetwork.LocalPlayer.NickName == winnerName}");
            ShowGameOverUI(PhotonNetwork.LocalPlayer.NickName == winnerName);
        }
        else
        {
            Debug.Log($"Not showing GameOver UI for eliminated player {PhotonNetwork.LocalPlayer.NickName}");
        }
    }

    private void ShowGameOverUI(bool isWinner)
    {
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOverUI(isWinner);
        }
        else
        {
            Debug.LogError("GameOverUI is null!");
            GameOverUI foundGameOverUI = FindObjectOfType<GameOverUI>();
            if (foundGameOverUI != null)
            {
                foundGameOverUI.ShowGameOverUI(isWinner);
            }
            else
            {
                Debug.LogError("GameOverUI not found in the scene!");
            }
        }
    }

    [PunRPC]
    private void SyncSurvivorCount(int count)
    {
        survivorCount = count;
        UpdateSurvivorCountUI();
    }

    private void UpdateSurvivorCountUI()
    {
        if (survivorCountText != null)
        {
            survivorCountText.text = $"{survivorCount}";
        }
    }

    [PunRPC]
    private void HandlePlayerCorpse(int playerViewID)
    {
        PhotonView playerView = PhotonNetwork.GetPhotonView(playerViewID);
        if (playerView != null)
        {
            GameObject playerObject = playerView.gameObject;
            DisablePlayerComponents(playerObject);
            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(DestroyCorpseAfterDelay(playerViewID, 10f));
            }
        }
    }

    private void DisablePlayerComponents(GameObject playerObject)
    {
        foreach (MonoBehaviour script in playerObject.GetComponents<MonoBehaviour>())
        {
            if (script is not PhotonView)
            {
                script.enabled = false;
            }
        }

        foreach (Collider collider in playerObject.GetComponents<Collider>())
        {
            collider.enabled = false;
        }

        Rigidbody rb = playerObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Animator animator = playerObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
    }

    private IEnumerator DestroyCorpseAfterDelay(int playerViewID, float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(PhotonNetwork.GetPhotonView(playerViewID).gameObject);
    }

    private IEnumerator SendAllPlayerStats()
    {
        List<UserRecords> allStats = new List<UserRecords>();
        foreach (var playerStat in playerStatsDict.Values)
        {
            allStats.Add(playerStat);
        }

        string jsonData = "[" + JsonConvert.SerializeObject(new { userRecords = allStats }) + "]";
        Debug.Log("Sending player stats: " + jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(API_URL, jsonData))
        {
            www.SetRequestHeader("Content-Type", "application/json");
            www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error sending data: {www.error} - Response Code: {www.responseCode} - Response: {www.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Player stats sent successfully");
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isGameOver);
            stream.SendNext(winnerNickname);
        }
        else
        {
            isGameOver = (bool)stream.ReceiveNext();
            winnerNickname = (string)stream.ReceiveNext();
        }
    }

    public List<GameObject> GetPlayers()
    {
        return players;
    }

    public Dictionary<string, UserRecords> GetPlayerStats()
    {
        return playerStatsDict;
    }

    public void AddPlayerMoney(int amount)
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("money", out object moneyValue))
        {
            int currentMoney = moneyValue is int ? (int)moneyValue : 0;
            currentMoney += amount;
            Hashtable newProperties = new Hashtable { { "money", currentMoney } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
        }
    }

    public void CompleteMinigame()
    {
        isMinigameCompleted = true;
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MainScene");
        }
    }

    private void ResetGameManager()
    {
        players.Clear();
        spawnedPlayers.Clear();
        playerStatsDict.Clear();
        isMinigameCompleted = false;
        isGameOver = false;
        winnerNickname = null;
        allPlayersLoaded = false;
        statsAlreadySent = false;
        processedKills.Clear();
        loadedPlayers.Clear();
        survivorCount = 0;
        canAttack = true;
        canMove = true;

        spawnPoints.Clear();

        if (playerPositions != null)
        {
            foreach (var positionList in playerPositions)
            {
                positionList.Clear();
            }
            playerPositions.Clear();
        }

        for (int i = 0; i < roundPortalNumbers.Length; i++)
        {
            roundPortalNumbers[i] = 0;
        }

        originalSpeeds.Clear();

        inGameUIManager = null;
        storeUI = null;
        gameOverUI = null;
        killLog = null;
        mapDropper = null;
    }

    private void SetupLocalPlayer(GameObject character)
    {
        PhotonNetwork.LocalPlayer.TagObject = character;

        Inventory inventory = FindObjectOfType<Inventory>();
        inventory?.SetPlayer(character);

        inGameUIManager?.SetPlayer(character);
        storeUI?.SetPlayer(character);
    }

    public GameObject GetCharacterPrefab(Player player)
    {
        if (!player.CustomProperties.ContainsKey("character"))
        {
            Debug.LogError("Character not selected for: " + player.NickName);
            return null;
        }

        string characterName = (string)player.CustomProperties["character"];
        GameObject prefab = characterPrefabs.FirstOrDefault(p => p.name == characterName);
        if (prefab == null)
        {
            Debug.LogError("Prefab not found for character: " + characterName);
        }
        return prefab;
    }

    public int GetLocalPlayerIndex()
    {
        Player localPlayer = PhotonNetwork.LocalPlayer;
        Player[] sortedPlayers = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToArray();
        int index = System.Array.IndexOf(sortedPlayers, localPlayer);
        Debug.Log("현 유저 인덱스: " + index + ", 전체 플레이어 수: " + sortedPlayers.Length);
        return index;
    }

    private void SetPlayerSpeed(GameObject player, float speed)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            string playerName = player.GetComponent<PhotonView>().Owner.NickName;
            if (!originalSpeeds.ContainsKey(playerName))
            {
                originalSpeeds[playerName] = movement.moveSpeed;
            }
            movement.moveSpeed = speed;
        }
    }

    private void RestoreOriginalSpeed(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            string playerName = player.GetComponent<PhotonView>().Owner.NickName;
            if (originalSpeeds.ContainsKey(playerName))
            {
                movement.moveSpeed = originalSpeeds[playerName];
            }
        }
    }

    public void SetCanAttackForPlayer(GameObject player, bool value)
    {
        player.GetComponent<PhotonView>()?.RPC("UpdateCanAttack", RpcTarget.AllBuffered, value);
    }

    public void SetCanMoveForPlayer(GameObject player, bool value)
    {
        player.GetComponent<PhotonView>()?.RPC("UpdateCanMove", RpcTarget.AllBuffered, value);
    }

    public void TriggerGameOver(string winnerNickname)
    {
        isGameOver = true;
        this.winnerNickname = winnerNickname;
        photonView.RPC("SyncGameOver", RpcTarget.All, winnerNickname);

        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SendAllPlayerStats());
        }
    }

    [PunRPC]
    private void SyncGameOver(string winnerNickname)
    {
        isGameOver = true;
        this.winnerNickname = winnerNickname;

        GameOverUI gameOverUI = FindObjectOfType<GameOverUI>();
        if (gameOverUI != null)
        {
            gameOverUI.ShowGameOverUI(true);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void ResetPlayerStats()
    {
        playerStatsDict.Clear();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (playerApiIds.TryGetValue(player.NickName, out int userId))
            {
                playerStatsDict[player.NickName] = new UserRecords
                {
                    userId = userId,
                    character_id = 0, // You might want to get this from player properties
                    match_rank = 0,
                    match_kills = 0,
                    match_playtime = "0"
                };
            }
            else
            {
                Debug.LogError($"No API ID found for player: {player.NickName}");
            }
        }
    }

    [PunRPC]
    private void AddKillLogRPC(string killedPlayer)
    {
        if (killLog != null)
        {
            killLog.AddKillLog(killedPlayer);
        }
    }

    private IEnumerator FetchAllPlayerIds()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            yield return StartCoroutine(FetchUserId(player.NickName));
        }
        InitializePlayerStats();
    }

    private IEnumerator FetchUserId(string nickname)
    {
        string url = $"{API_URL2}{nickname}";
        Debug.Log("url");
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("hi");
                User user = JsonConvert.DeserializeObject<User>(webRequest.downloadHandler.text);
                playerApiIds[nickname] = user.userId;
                Debug.Log(user.userId);
                Debug.Log($"Fetched ID for {nickname}: {user.userId}");
            }
            else
            {
                Debug.LogError($"Failed to fetch user ID for {nickname}: {webRequest.error}");
            }
        }
    }

    private void InitializePlayerStats()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (playerApiIds.TryGetValue(player.NickName, out int userId))
            {
                playerStatsDict[player.NickName] = new UserRecords
                {
                    userId = userId,
                    character_id = 0, // You might want to get this from player properties
                    match_rank = 0,
                    match_kills = 0,
                    match_playtime = "0"
                };
            }
            else
            {
                Debug.LogError($"No API ID found for player: {player.NickName}");
            }
        }
    }
}

[System.Serializable]
public class User
{
    public int userId { get; set; }
    public string userEmail { get; set; }
    public string userPassword { get; set; }
    public string userNickname { get; set; }
    public System.DateTime userCreatedAt { get; set; }
    public System.DateTime? userDeletedAt { get; set; }
    public long userGameMoney { get; set; }
    public int userLevel { get; set; }
    public System.DateTime? userAttendance { get; set; }
    public System.DateTime? userLastLogin { get; set; }
    public string? userStatus { get; set; }
    public string? userProfileImage { get; set; }
    public string? userChosenCharacter { get; set; }
    public bool userIsOnline { get; set; }
}