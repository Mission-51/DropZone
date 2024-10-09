using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChestTrigger : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI chestText;
    public Transform chestTopPos;
    public ParticleSystem openParticle;
    public GameObject chestBoxUI;
    public bool isChestOpened = false;
    private bool isPlayerNear;
    private SlotToolTip slotTooltip;

    private void Start()
    {
        slotTooltip = FindObjectOfType<SlotToolTip>();
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == PhotonNetwork.LocalPlayer.TagObject as GameObject)
        {
            chestText.gameObject.SetActive(true);
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject == PhotonNetwork.LocalPlayer.TagObject as GameObject)
        {
            CloseChestAndResetUI();
            isPlayerNear = false;
        }
    }

    private void CloseChestAndResetUI()
    {
        chestText.gameObject.SetActive(false);
        chestBoxUI.SetActive(false);
        slotTooltip.HideToolTip();

        if (isChestOpened)
        {
            photonView.RPC("ToggleChest", RpcTarget.All, false, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    [PunRPC]
    public void ToggleChest(bool open, int actorNumber)
    {
        isChestOpened = open;
        chestTopPos.rotation = open ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(45, 180, 0);

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            chestBoxUI.SetActive(open);
            chestText.text = open ? "to Close" : "to Open";

            if (open)
            {
                Debug.Log("상자를 열었습니다. 공격이 제한됩니다.");
                GameManager.instance.SetCanAttackForPlayer(PhotonNetwork.LocalPlayer.TagObject as GameObject, false);
            }
            else
            {
                Debug.Log("상자를 닫았습니다. 공격이 재개됩니다.");
                GameManager.instance.SetCanAttackForPlayer(PhotonNetwork.LocalPlayer.TagObject as GameObject, true);
                if (slotTooltip != null)
                {
                    slotTooltip.HideToolTip();
                }
            }
        }
        else
        {
            chestBoxUI.SetActive(false);
            if (slotTooltip != null) slotTooltip.HideToolTip();
        }

        if (openParticle != null)
        {
            openParticle.Play();
        }
    }

    private void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            photonView.RPC("ToggleChest", RpcTarget.All, !isChestOpened, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
}