using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject go_SlotParant; // Slot들의 부모인 Grid Setting

    private Slot[] slots; // 슬롯들 배열

    void Start()
    {
        slots = go_SlotParant.GetComponentsInChildren<Slot>();
    }

    public void AcquireItem(ItemData _item, int _count = 1)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.itemName == _item.itemName)
                {
                    slots[i].SetSlotCount(_count);
                    return;
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }
}
