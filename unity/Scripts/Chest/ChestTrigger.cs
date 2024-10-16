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
    public AudioSource chestOpenSound;
    public AudioSource chestCloseSound;
    private bool isPlayerNear;
    private SlotToolTip slotTooltip;
    private int interactingPlayerActorNumber = -1;
    private Chest chest;

    private void Start()
    {
        slotTooltip = FindObjectOfType<SlotToolTip>();
        chest = GetComponent<Chest>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PhotonView playerView = other.GetComponent<PhotonView>();
            if (playerView != null && playerView.IsMine)
            {
                chestText.gameObject.SetActive(true);
                isPlayerNear = true;
                Debug.Log($"Player {playerView.Owner.NickName} entered chest {chest.chestId} trigger");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isChestOpened) chestCloseSound.Play();
            PhotonView playerView = other.GetComponent<PhotonView>();
            if (playerView != null && playerView.IsMine)
            {
                photonView.RPC("CloseChestAndResetUIRPC", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, chest.chestId);
                isPlayerNear = false;
                Debug.Log($"Player {playerView.Owner.NickName} exited chest {chest.chestId} trigger");
            }
        }
    }

    [PunRPC]
    private void CloseChestAndResetUIRPC(int actorNumber, int chestId)
    {
        if (this.chest.chestId == chestId)
        {
            chestText.gameObject.SetActive(false);
            chestBoxUI.SetActive(false);
            if (slotTooltip != null)
            {
                slotTooltip.HideToolTip();
            }
        }

        if (isChestOpened && this.chest.chestId == chestId)
        {
            ToggleChest(false, actorNumber);
        }
    }

    [PunRPC]
    public void ToggleChest(bool open, int actorNumber)
    {
        isChestOpened = open;
        chestTopPos.rotation = open ? Quaternion.Euler(0, 180, 0) : Quaternion.Euler(45, 180, 0);

        if (open)
        {
            interactingPlayerActorNumber = actorNumber;
        }
        else
        {
            interactingPlayerActorNumber = -1;
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            chestBoxUI.SetActive(open);
            chestText.text = open ? "to Close" : "to Open";

            if (open)
            {
                Debug.Log($"Player {PhotonNetwork.LocalPlayer.NickName} opened chest {chest.chestId}. Attack restricted.");
                GameManager.Instance.SetCanAttackForPlayer(PhotonNetwork.LocalPlayer.TagObject as GameObject, false);
            }
            else
            {
                Debug.Log($"Player {PhotonNetwork.LocalPlayer.NickName} closed chest {chest.chestId}. Attack enabled.");
                GameManager.Instance.SetCanAttackForPlayer(PhotonNetwork.LocalPlayer.TagObject as GameObject, true);
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
            if (isChestOpened) chestCloseSound.Play();
            else if (!isChestOpened) chestOpenSound.Play();
            Debug.Log($"F key pressed by player {PhotonNetwork.LocalPlayer.NickName} for chest {chest.chestId}. Chest open state: {isChestOpened}, Interacting player: {interactingPlayerActorNumber}");
            if (interactingPlayerActorNumber == -1 || interactingPlayerActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                photonView.RPC("ToggleChest", RpcTarget.All, !isChestOpened, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else
            {
                Debug.Log($"Cannot interact. Chest {chest.chestId} is being used by player {interactingPlayerActorNumber}");
            }
        }

        if (!isChestOpened)
        {
            slotTooltip.HideToolTip();
        }
    }
}