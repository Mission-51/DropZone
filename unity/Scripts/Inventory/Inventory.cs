using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI coinText;

    [SerializeField]
    private GameObject go_Player; // player 게임 오브젝트

    [SerializeField]
    private GameObject go_SlotParant; // Slot들의 부모인 Grid Setting

    private Slot[] slots; // 슬롯들 배열

    private PlayerUseItem playerUseItem;

    public int coin;

    void Start()
    {
        slots = go_SlotParant.GetComponentsInChildren<Slot>();
        playerUseItem = go_Player.GetComponent<PlayerUseItem>();
    }

    private void Update()
    {
        // 아이템 사용 로직
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (slots[0].item != null)
            {
                // 아이템 사용 메서드
                playerUseItem.UseSelectedItem(slots[0]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (slots[1].item != null)
            {
                // 아이템 사용 메서드
                playerUseItem.UseSelectedItem(slots[1]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (slots[2].item != null)
            {
                // 아이템 사용 메서드
                playerUseItem.UseSelectedItem(slots[2]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (slots[3].item != null)
            {
                // 아이템 사용 메서드
                playerUseItem.UseSelectedItem(slots[3]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (slots[4].item != null)
            {
                // 아이템 사용 메서드
                playerUseItem.UseSelectedItem(slots[4]);
            }
        }
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

    public void coinPlus(int _coin)
    {
        coin += _coin;
        coinText.text = coin.ToString();
    }

    // 사용 아이템이 인벤토리에 2개 이상 들어가 있는지 여부 알려주는 메서드
    // 사용 아이템이 2개 이상이라면 true, 이하라면 false
    public bool UseItemCheck()
    {
        int useItemAmount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
            {
                if (slots[i].item.id > 1 && slots[i].item.id < 6)
                {
                    useItemAmount += slots[i].itemCount;
                }
            }
        }

        if (useItemAmount >= 2)
        {
            return true;
        }
        return false;
    }

    // 슬롯 자리바꾸기 메서드
    public void ChangeSlots(int _startSlot, int _endSlot)
    {
        Slot _tempSlot = slots[_startSlot];
        slots[_endSlot] = slots[_startSlot];
        slots[_startSlot] = _tempSlot;
    }

    // 인벤토리창에 마우스 진입 시 공격, 스킬 사용 막기
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스가 인벤토리 UI에 진입하였습니다.");
        if (go_Player != null)
        {
            GameManager.Instance.SetCanAttackForPlayer(go_Player, false);            
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("마우스가 인벤토리 UI에서 빠져나왔습니다.");
        if (go_Player != null)
        {
            GameManager.Instance.SetCanAttackForPlayer(go_Player, true);            
        }
    }

    public void SetPlayer(GameObject player)
    {
        go_Player = player;
        playerUseItem = player.GetComponent<PlayerUseItem>();

        if (playerUseItem == null)
        {
            Debug.LogError("PlayerUseItem 컴포넌트를 찾을 수 없습니다. 인벤토리와 연결되지 않습니다.");
        }
    }
}
