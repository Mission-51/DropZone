using UnityEngine;
using Photon.Pun;

public class PlayerMinigame : MonoBehaviourPun
{
    private Transform playerTransform;
    private int currentLevel;
    private GameManager gameManager;

    void Start()
    {
        playerTransform = transform;
        currentLevel = 0;
        gameManager = GameManager.Instance;

        if (photonView.IsMine)
        {
            InitializePlayerPosition();
        }
    }

    private void InitializePlayerPosition()
    {
        int playerIndex = gameManager.GetLocalPlayerIndex();
        if (playerIndex != -1 && gameManager.playerPositions.Count > playerIndex)
        {
            playerTransform.position = gameManager.playerPositions[playerIndex][0];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine || !other.CompareTag("Portal"))
            return;

        int portalNum = int.Parse(other.gameObject.name);

        if (portalNum == gameManager.roundPortalNumbers[currentLevel])
        {
            MoveToNextLevel();
        }
        else
        {
            ResetCurrentLevel();
        }
    }

    private void MoveToNextLevel()
    {
        currentLevel++;
        if (currentLevel >= gameManager.playerPositions[0].Count)
        {
            photonView.RPC("WinGame", RpcTarget.All);
        }
        else
        {
            playerTransform.position = gameManager.playerPositions[gameManager.GetLocalPlayerIndex()][currentLevel];
        }
    }

    private void ResetCurrentLevel()
    {
        playerTransform.position = gameManager.playerPositions[gameManager.GetLocalPlayerIndex()][currentLevel];
    }

    [PunRPC]
    private void WinGame()
    {
        gameManager.TriggerGameOver(photonView.Owner.NickName);
        if (photonView.IsMine)
        {
            gameManager.AddPlayerMoney(300);
        }
    }
}