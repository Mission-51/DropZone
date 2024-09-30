using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChestSpawner : MonoBehaviour
{
    // 상자 스폰지점
    public List<Transform> spawnSpots;
    // 자원
    public static Resources resources;

    // 상자
    public GameObject chest;
    public static ChestSpawner instance = null;

    private void Awake()
    {
       if (null == instance)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        } 
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static ChestSpawner Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    void Start()
    {
        Chest _chest = chest.GetComponent<Chest>();
        // 스폰지점 섞기
        for (int i = spawnSpots.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            // 리스트의 i번째 요소와 j번째 요소를 교환

            Transform temp = spawnSpots[i];
            spawnSpots[i] = spawnSpots[j];
            spawnSpots[j] = temp;
        }

        for (int i = 0; i < 20; i++)
        {
            Instantiate(chest, spawnSpots[i]);
        }
    }
}
