using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using System.Linq;

public class Chest : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<ItemData> items = new List<ItemData>();
    [SerializeField] private Transform slotParent;
    [SerializeField] private ChestSlot[] slots;
    private PhotonView photonView;
    private Rigidbody rb;
    ChestSpawner spawner;

    [HideInInspector]
    public int chestId;

    private void Awake()
    {
        spawner = FindObjectOfType<ChestSpawner>();
        photonView = GetComponent<PhotonView>();
        slots = slotParent.GetComponentsInChildren<ChestSlot>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
        rb.useGravity = false;

        if (photonView.IsMine)
        {
            chestId = spawner.GetNextChestId();
            photonView.RPC("SyncChestId", RpcTarget.AllBuffered, chestId);
        }
    }

    [PunRPC]
    private void SyncChestId(int id)
    {
        chestId = id;
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            InitializeItems();
        }
    }

    private void InitializeItems()
    {
        Debug.Log("Initializing items for chest");
        while (items.Count < 3)
        {
            ItemData newItem = null;
            if (items.Count == 0) newItem = ChestSpawner.GetNextMoney();
            else if (items.Count == 1) newItem = ChestSpawner.GetNextUseItem();
            else if (items.Count == 2) newItem = ChestSpawner.GetNextHealItem();

            if (newItem != null)
            {
                items.Add(newItem);
                Debug.Log($"추가 아이템: {newItem.itemName} ");
            }
            else
            {
                Debug.LogWarning("더 추가할 아이템 읎읍디다");
                break;
            }
        }
        photonView.RPC("SyncItems", RpcTarget.All, items.Select(item => item.id).ToArray());
    }

    [PunRPC]
    private void SyncItems(int[] itemIds)
    {
        items.Clear();
        foreach (int id in itemIds)
        {
            ItemData item = spawner.itemList.Find(i => i.id == id);
            if (item != null)
            {
                items.Add(item);
            }
        }
        FreshSlot();
        Debug.Log($"연동된 아이템. 전체 아이템 수: {items.Count}");
    }

    public void FreshSlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].item = i < items.Count ? items[i] : null;
        }
    }

    [PunRPC]
    public void RemoveItemRPC(int itemIndex)
    {
        if (itemIndex >= 0 && itemIndex < items.Count)
        {
            Debug.Log($"[체스트] 아이템 제거 {itemIndex}");
            items.RemoveAt(itemIndex);
            FreshSlot();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MinigameManager.instance.canAttack = false;
        Debug.Log("Chest: Pointer entered, canAttack set to false");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MinigameManager.instance.canAttack = true;
        Debug.Log("Chest: Pointer exited, canAttack set to true");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(items.Count);
            foreach (var item in items)
            {
                stream.SendNext(item.id);
            }
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            int itemCount = (int)stream.ReceiveNext();
            items.Clear();
            for (int i = 0; i < itemCount; i++)
            {
                int itemId = (int)stream.ReceiveNext();
                ItemData item = spawner.itemList.Find(x => x.id == itemId);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            FreshSlot();
        }
    }    
}