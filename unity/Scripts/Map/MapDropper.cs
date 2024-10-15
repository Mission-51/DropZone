using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapDropper : MonoBehaviourPunCallbacks
{
    public GameObject map;
    public float fallSpeed = 50f;
    public TextMeshProUGUI timerText;
    public GameObject warningText;
    public AudioSource siren;
    public GameObject loadingScreen;
    private bool timerStarted = false;

    private Transform[] tiles;
    private float time = 0;

    static int N = 20;
    static int totalTiles = N * N;

    private List<int[]> targetTiles = new List<int[]>();

    static int[][] directions = new int[][]
    {
        new int[] {-1, 0}, // 상
        new int[] {1, 0},  // 하
        new int[] {0, -1}, // 좌
        new int[] {0, 1}   // 우
    };

    void Start()
    {
        tiles = map.transform.Cast<Transform>().ToArray();
        loadingScreen.SetActive(true);

        int[,] mapGrid = new int[N, N];
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                mapGrid[i, j] = 1;
            }
        }

        System.Random rnd = new System.Random();
        int lastTileX = rnd.Next(0, N);
        int lastTileY = rnd.Next(0, N);
        (int, int) parentTile = (lastTileX, lastTileY);
        Debug.Log($"Last remaining tile: {parentTile}, Index: {lastTileX * N + lastTileY}");

        PlayRounds(mapGrid, totalTiles, parentTile, N);
    }

    void Update()
    {
        if (!timerStarted) return;

        ShowWarnText();
        PlaySiren();
        if (PhotonNetwork.IsMasterClient)
        {
            time += Time.deltaTime;
            photonView.RPC("SyncTime", RpcTarget.All, time);
            CheckRounds();
        }

        if (time > 0) loadingScreen.SetActive(false);

        UpdateTimerText();
    }

    [PunRPC]
    private void SyncTime(float syncedTime)
    {
        time = syncedTime;
    }

    private void UpdateTimerText()
    {
        timerText.text = Mathf.Round(time).ToString();
    }

    [PunRPC]
    public void StartTimer()
    {
        timerStarted = true;
        time = 0;
        Debug.Log("MapDropper timer started");
    }

    private void CheckRounds()
    {
        if (time > 60 && targetTiles.Count > 0)
        {
            WarnTiles(targetTiles[0]);
            if (time > 120) DropTiles(targetTiles[0]);
        }

        if (time > 180 && targetTiles.Count > 1)
        {
            WarnTiles(targetTiles[1]);
            if (time > 240) DropTiles(targetTiles[1]);
        }

        if (time > 300 && targetTiles.Count > 2)
        {
            WarnTiles(targetTiles[2]);
            if (time > 360) DropTiles(targetTiles[2]);
        }

        if (time > 420 && targetTiles.Count > 3)
        {
            WarnTiles(targetTiles[3]);
            if (time > 480) DropTiles(targetTiles[3]);
        }

        if (time > 540 && targetTiles.Count > 4)
        {
            WarnTiles(targetTiles[4]);
            if (time > 600) DropTiles(targetTiles[4]);
        }
    }

    private void ShowWarnText()
    {
        if (time > 60 && time < 70) warningText.SetActive(true);
        else if (time > 180 && time < 190) warningText.SetActive(true);
        else if (time > 300 && time < 310) warningText.SetActive(true);
        else if (time > 420 && time < 430) warningText.SetActive(true);
        else if (time > 480 && time < 490) warningText.SetActive(true);
        else warningText.SetActive(false);
    }

    private void PlaySiren()
    {
        if (time > 60 && time < 61) siren.Play();
        else if (time > 180 && time < 181) siren.Play();
        else if (time > 300 && time < 301) siren.Play();
        else if (time > 420 && time < 421) siren.Play();
        else if (time > 480 && time < 481) siren.Play();
        else if (time > 481 && time < 482) siren.Play();
    }

    private void WarnTiles(int[] renderIndices)
    {
        photonView.RPC("ChangeTileColorRPC", RpcTarget.All, renderIndices);
    }

    [PunRPC]
    private void ChangeTileColorRPC(int[] renderIndices)
    {
        foreach (int index in renderIndices)
        {
            Renderer tileColor = tiles[index].GetComponent<Renderer>();
            for (int i = 0; i < tileColor.materials.Length; i++)
            {
                tileColor.materials[i].color = new Color(255f / 255f, 50f / 255f, 50f / 255f);
            }
        }
    }

    private void DropTiles(int[] renderIndices)
    {
        photonView.RPC("StartTileFallRPC", RpcTarget.All, renderIndices);
    }

    [PunRPC]
    private void StartTileFallRPC(int[] renderIndices)
    {
        foreach (int index in renderIndices)
        {
            StartCoroutine(DropTileCoroutine(index));
        }
    }

    private IEnumerator DropTileCoroutine(int index)
    {
        Transform tile = tiles[index];
        Vector3 startPos = tile.position;
        Vector3 endPos = new Vector3(startPos.x, -100, startPos.z);
        float elapsedTime = 0f;
        float dropDuration = 2f;

        while (elapsedTime < dropDuration)
        {
            tile.position = Vector3.Lerp(startPos, endPos, elapsedTime / dropDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tile.position = endPos;
    }

    public int ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
    }

    public bool Dfs(int[,] mapGrid, (int, int) parentTile, int N)
    {
        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push(parentTile);

        bool[,] visited = new bool[N, N];
        visited[parentTile.Item1, parentTile.Item2] = true;
        int visitedCount = 1;

        while (stack.Count > 0)
        {
            var (x, y) = stack.Pop();

            foreach (var direction in directions)
            {
                int nx = x + direction[0];
                int ny = y + direction[1];

                if (nx >= 0 && ny >= 0 && nx < N && ny < N && mapGrid[nx, ny] == 1 && !visited[nx, ny])
                {
                    visited[nx, ny] = true;
                    stack.Push((nx, ny));
                    visitedCount++;
                }
            }
        }

        int remainingTiles = 0;
        foreach (var value in mapGrid)
        {
            if (value == 1) remainingTiles++;
        }

        return visitedCount == remainingTiles;
    }

    public (List<(int, int)>, int[]) RemoveTileBatch(int[,] mapGrid, int numToRemove, (int, int) parentTile, int N)
    {
        List<(int, int)> tiles = new List<(int, int)>();
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (mapGrid[i, j] == 1 && (i, j) != parentTile)
                {
                    tiles.Add((i, j));
                }
            }
        }

        tiles = tiles.OrderBy(tile => -ManhattanDistance(tile.Item1, tile.Item2, parentTile.Item1, parentTile.Item2)).ToList();

        List<(int, int)> removedTiles = new List<(int, int)>();
        List<int> removedTileIndices = new List<int>();

        float randomRemovalChance = 0.2f;
        System.Random rnd = new System.Random();

        while (removedTiles.Count < numToRemove && tiles.Count > 0)
        {
            (int x, int y) = (0, 0);
            if (rnd.NextDouble() < randomRemovalChance)
            {
                int randomIndex = rnd.Next(tiles.Count);
                (x, y) = tiles[randomIndex];
                tiles.RemoveAt(randomIndex);
            }
            else
            {
                (x, y) = tiles[0];
                tiles.RemoveAt(0);
            }

            mapGrid[x, y] = 0;

            if (Dfs(mapGrid, parentTile, N))
            {
                removedTiles.Add((x, y));
                removedTileIndices.Add(x * N + y);
            }
            else
            {
                mapGrid[x, y] = 1;
                tiles.Add((x, y));
            }
        }

        return (removedTiles, removedTileIndices.ToArray());
    }

    public void PlayRounds(int[,] mapGrid, int totalTiles, (int, int) parentTile, int N)
    {
        int[] rounds = { 300, 200, 100, 1 };
        int currentTileCount = totalTiles;

        for (int roundNum = 0; roundNum < rounds.Length; roundNum++)
        {
            int targetCount = rounds[roundNum];
            int numToRemove = currentTileCount - targetCount;

            var (removedTiles, removedTileIndices) = RemoveTileBatch(mapGrid, numToRemove, parentTile, N);
            targetTiles.Add(removedTileIndices);

            currentTileCount = targetCount;
        }
    }
}