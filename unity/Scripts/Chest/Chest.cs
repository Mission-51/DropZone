using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public List<ItemData> itemList;
    public List<ItemData> items;

    private static ChestSpawner m_instance;

    [SerializeField]
    private Transform slotParent;

    [SerializeField]
    private ChestSlot[] slots;


#if UNITY_EDITOR
    private void OnValidate()
    {
        slots = slotParent.GetComponentsInChildren<ChestSlot>();
    }
#endif

    private void Awake()
    {
        FreshSlot();
    }

    private void Start()
    {
        // 돈
        if (ChestSpawner.resources.money100 > 0)
        {
            this.AddItem(itemList[0]);
            ChestSpawner.resources.money100 -= 1;
        }
        else if (ChestSpawner.resources.money200 > 0)
        {
            this.AddItem(itemList[1]);
            ChestSpawner.resources.money200 -= 1;
        }
        else if (ChestSpawner.resources.money300 > 0)
        {
            this.AddItem(itemList[2]);
            ChestSpawner.resources.money300 -= 1;
        }

        // 사용 아이템
        if (ChestSpawner.resources.taserGun > 0)
        {
            this.AddItem(itemList[3]);
            ChestSpawner.resources.taserGun -= 1;
        }
        else if (ChestSpawner.resources.grenade > 0)
        {
            this.AddItem(itemList[4]);
            ChestSpawner.resources.grenade -= 1;
        }
        else if (ChestSpawner.resources.trap > 0)
        {
            this.AddItem(itemList[5]);
            ChestSpawner.resources.trap -= 1;
        }
        else if (ChestSpawner.resources.glue > 0)
        {
            this.AddItem(itemList[6]);
            ChestSpawner.resources.glue -= 1;
        }

        // 회복 아이템
        if (ChestSpawner.resources.bandage > 0)
        {
            this.AddItem(itemList[7]);
            ChestSpawner.resources.bandage -= 1;
        }
        else if (ChestSpawner.resources.firstAidKit > 0)
        {
            this.AddItem(itemList[8]);
            ChestSpawner.resources.firstAidKit -= 1;
        }
    }

    public void FreshSlot()
    {
        int i = 0;
        for (; i < items.Count && i < slots.Length; i++)
        {
            slots[i].item = items[i];
        }
        for (; i < slots.Length; i++)
        {
            slots[i].item = null;
        }
    }

    public void AddItem(ItemData _item)
    {
        if (items.Count < slots.Length)
        {
            items.Add(_item);
            FreshSlot();
        }
        else
        {
            print("슬롯이 가득 차 있습니다.");
        }
    }
}
