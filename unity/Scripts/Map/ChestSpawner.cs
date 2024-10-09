using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class ChestSpawner : MonoBehaviourPunCallbacks
{
    public List<Transform> spawnSpots;
    public GameObject chest;
    public List<ItemData> itemList;

    private static List<ItemData> moneys = new List<ItemData>();
    private static List<ItemData> useItems = new List<ItemData>();
    private static List<ItemData> healItems = new List<ItemData>();
    private HashSet<int> usedSpotIndices = new HashSet<int>();
    private bool chestsSpawned = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeItems();
            SpawnChests();
        }
    }

    private void InitializeItems()
    {
        Debug.Log("맛스타 :  아이템 별장난");
        // Initialize money items
        for (int i = 0; i < 10; i++) moneys.Add(itemList[0]);
        for (int i = 0; i < 7; i++) moneys.Add(itemList[1]);
        for (int i = 0; i < 3; i++) moneys.Add(itemList[2]);
        ShuffleList(moneys);

        // Initialize use items
        for (int i = 3; i <= 6; i++)
            for (int j = 0; j < 3; j++)
                useItems.Add(itemList[i]);
        ShuffleList(useItems);

        // Initialize heal items
        for (int i = 0; i < 12; i++) healItems.Add(itemList[7]);
        for (int i = 0; i < 6; i++) healItems.Add(itemList[8]);
        ShuffleList(healItems);

        Debug.Log($"아이템 초기화 - Money: {moneys.Count}, Item: {useItems.Count}, Heal: {healItems.Count}");
    }

    private void SpawnChests()
    {
        if (chestsSpawned) return;

        Debug.Log("마스터 : 위치 초기화 한다이");
        usedSpotIndices.Clear(); // 사용된 위치 초기화
        List<int> selectedIndices = new List<int>();

        for (int i = 0; i < 20 && usedSpotIndices.Count < spawnSpots.Count; i++)
        {
            int spotIndex = GetUniqueRandomSpotIndex();
            if (spotIndex != -1)
            {
                selectedIndices.Add(spotIndex);
                usedSpotIndices.Add(spotIndex);
                Debug.Log($"선택 지점 인덱스: {spotIndex}, 위치: {spawnSpots[spotIndex].position}");
            }
            else
            {
                Debug.LogWarning("사용 가능한 지점 없다");
                break;
            }
        }

        Debug.Log($"마스터 클라이언트: 선택된거 {selectedIndices.Count} 위치");
        Debug.Log($"여기 사용할 것: {string.Join(", ", usedSpotIndices)}");
        photonView.RPC("SyncChestPositions", RpcTarget.All, selectedIndices.ToArray());
    }

    private int GetUniqueRandomSpotIndex()
    {
        List<int> availableSpots = Enumerable.Range(0, spawnSpots.Count)
                                             .Except(usedSpotIndices)
                                             .ToList();

        if (availableSpots.Count == 0)
        {
            return -1; // 모든 위치가 사용됨
        }

        return availableSpots[Random.Range(0, availableSpots.Count)];
    }

    [PunRPC]
    private void SyncChestPositions(int[] indices)
    {
        if (chestsSpawned) return;
        Debug.Log($"여기에 {indices.Length} 체스트 생성된다이");
        StartCoroutine(SpawnChestsCoroutine(indices));
    }

    private IEnumerator SpawnChestsCoroutine(int[] indices)
    {
        HashSet<Vector3> spawnedPositions = new HashSet<Vector3>();

        foreach (int index in indices)
        {
            if (index >= 0 && index < spawnSpots.Count)
            {
                Transform spawnPoint = spawnSpots[index];
                Vector3 position = spawnPoint.position;

                if (!spawnedPositions.Contains(position))
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        GameObject spawnedChest = PhotonNetwork.Instantiate(chest.name, position, Quaternion.Euler(0, 180, 0));
                        Debug.Log($"체스트 생성 위치: {position}");
                    }
                    spawnedPositions.Add(position);
                }
                else
                {
                    Debug.LogWarning($"중복 감지: {position}. 체스트 스폰 넘어감");
                }
            }
            else
            {
                Debug.LogError($"유효하지 않은 인덱스: {index}");
            }

            yield return new WaitForEndOfFrame();
        }

        chestsSpawned = true;
        Debug.Log($"소환 종료 {spawnedPositions.Count} 체스트");
    }

    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static ItemData GetNextMoney()
    {
        if (moneys.Count > 0)
        {
            ItemData item = moneys[0];
            moneys.RemoveAt(0);
            return item;
        }
        return null;
    }

    public static ItemData GetNextUseItem()
    {
        if (useItems.Count > 0)
        {
            ItemData item = useItems[0];
            useItems.RemoveAt(0);
            return item;
        }
        return null;
    }

    public static ItemData GetNextHealItem()
    {
        if (healItems.Count > 0)
        {
            ItemData item = healItems[0];
            healItems.RemoveAt(0);
            return item;
        }
        return null;
    }
}