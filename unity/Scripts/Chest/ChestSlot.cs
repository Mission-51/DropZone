using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Photon.Pun;

public class ChestSlot : MonoBehaviourPunCallbacks, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Canvas canvas;
    [SerializeField] Image image;
    [SerializeField] private SlotToolTip slotTooltip;

    private Inventory inventory;
    private Chest parentChest;

    private ItemData _item;
    public ItemData item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = item.itemImage;
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        parentChest = GetComponentInParent<Chest>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right && item != null && PhotonNetwork.IsMessageQueueRunning)
        {
            if (!PhotonNetwork.LocalPlayer.IsLocal) return; // 자신의 슬롯만 클릭 가능

            if (item.id >= 6 && item.id <= 8) // Money items
            {
                int coinAmount = (item.id - 5) * 100;
                inventory.coinPlus(coinAmount);
                Debug.Log($"Added {coinAmount} coins to inventory");
            }
            else
            {
                if (item.id > 1 && item.id < 6 && inventory.UseItemCheck())
                {
                    Debug.LogWarning("Cannot add more than 2 use items to inventory");
                    return;
                }

                inventory.AcquireItem(item);
                Debug.Log($"Added item {item.itemName} to inventory");
            }

            int itemIndex = System.Array.IndexOf(parentChest.GetComponentsInChildren<ChestSlot>(), this);
            parentChest.photonView.RPC("RemoveItemRPC", RpcTarget.All, itemIndex);
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null)
        {
            slotTooltip.ShowToolTip(item);
            Debug.Log($"Showing tooltip for item: {item.itemName}");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        slotTooltip.HideToolTip();
        Debug.Log("Hiding tooltip");
    }
}
