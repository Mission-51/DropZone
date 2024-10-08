using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class MapDropper : MonoBehaviourPun
{
    public GameObject map; // 맵
    public float fallSpeed = 50f; // 맵 떨어지는 속도
    public TextMeshProUGUI timerText; // 타이머(텍스트)

    private Transform[] tiles; // 맵 타일들 (배열)
    private float time = 0; // 시간

    static int N = 20; // 맵 크기 설정
    static int totalTiles = N * N; // 총 타일 수

    private List<int[]> targetTiles = new List<int[]>(); // 라운드별 떨어질 좌표들

    static int[][] directions = new int[][]
    {
        new int[] {-1, 0}, // 상
        new int[] {1, 0},  // 하
        new int[] {0, -1}, // 좌
        new int[] {0, 1}   // 우
    };

    private void Start()
    {
        tiles = map.transform.Cast<Transform>().ToArray();

        // 맵 타일
        int[,] mapGrid = new int[N, N];

        // 맵을 초기화 (모든 타일이 1로 채워짐, 1은 타일이 있는 상태)
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                mapGrid[i, j] = 1;
            }
        }

        // 마지막까지 남을 타일을 랜덤으로 선택
        System.Random rnd = new System.Random();
        int lastTileX = rnd.Next(0, N);
        int lastTileY = rnd.Next(0, N);
        (int, int) parentTile = (lastTileX, lastTileY);
        Debug.Log($"Last remaining tile: {parentTile}, Index: {lastTileX * N + lastTileY}");

        PlayRounds(mapGrid, totalTiles, parentTile, N);
    }

    private void Update()
    {
        // 시간 세주기
        time += Time.deltaTime;
        timerText.text = Math.Round(time).ToString();

        // 각 라운드에 따라 타일 경고 및 제거
        CheckRounds();
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
    }

    private void WarnTiles(int[] renderIndices)
    {
        // 마스터 클라이언트에게 경고 요청
        if (PhotonNetwork.IsMasterClient)
        {
            ChangeTileColor(renderIndices);
        }
        else
        {
            photonView.RPC("ChangeTileColor", RpcTarget.MasterClient, renderIndices);
        }
    }

    [PunRPC]
    private void ChangeTileColor(int[] renderIndices)
    {
        foreach (int index in renderIndices)
        {
            Renderer tileColor = tiles[index].GetComponent<Renderer>();
            for (int i = 0; i < tileColor.materials.Length; i++)
            {
                tileColor.materials[i].color = Color.red;
            }
        }
    }

    private void DropTiles(int[] renderIndices)
    {
        // 마스터 클라이언트에게 타일을 떨어뜨리라는 요청
        if (PhotonNetwork.IsMasterClient)
        {
            StartTileFall(renderIndices);
        }
        else
        {
            photonView.RPC("StartTileFall", RpcTarget.MasterClient, renderIndices);
        }
    }

    [PunRPC]
    private void StartTileFall(int[] renderIndices)
    {
        foreach (int index in renderIndices)
        {
            tiles[index].position = Vector3.MoveTowards(tiles[index].position, new Vector3(tiles[index].position.x, -100, tiles[index].position.z), fallSpeed * Time.deltaTime);
        }
    }

    public int ManhattanDistance(int x1, int y1, int x2, int y2)
    {
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2);
    }

    public bool Dfs(int[,] mapGrid, (int, int) parentTile, int N)
    {
        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push(parentTile);

        bool[,] visited = new bool[N, N];
        visited[parentTile.Item1, parentTile.Item2] = true;
        int visitedCount = 1; // 부모 타일은 방문했으므로 1로 시작

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

        // DFS로 방문한 타일 개수와 현재 남아있는 타일 개수가 일치하는지 확인
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
        int[] rounds = { 300, 200, 100, 1 }; // 각 라운드의 목표 타일 개수
        int currentTileCount = totalTiles;   // 현재 남아 있는 타일 수

        for (int roundNum = 0; roundNum < rounds.Length; roundNum++)
        {
            int targetCount = rounds[roundNum];
            int numToRemove = currentTileCount - targetCount; // 이번 라운드에서 제거할 타일 수

            var (removedTiles, removedTileIndices) = RemoveTileBatch(mapGrid, numToRemove, parentTile, N);
            targetTiles.Add(removedTileIndices); // 제거대상 리스트 추가

            currentTileCount = targetCount; // 남은 타일 수 업데이트
        }
    }
}