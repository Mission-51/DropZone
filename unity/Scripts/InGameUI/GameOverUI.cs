using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviourPunCallbacks
{
    public GameObject go_GameOverUI;
    public Image gameOverBackground;
    public TextMeshProUGUI gameOverTxt;
    public TextMeshProUGUI killAmount;
    public TextMeshProUGUI dealAmount;

    public void ShowGameOverUI(bool isWinner)
    {
        Debug.Log($"ShowGameOverUI method called. IsWinner: {isWinner}");
        if (go_GameOverUI == null)
        {
            Debug.LogError("go_GameOverUI is null!");
            return;
        }
        go_GameOverUI.SetActive(true);
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null!");
            return;
        }
        if (!GameManager.Instance.GetPlayerStats().TryGetValue(PhotonNetwork.LocalPlayer.NickName, out var localPlayerStats))
        {
            Debug.LogError($"Stats not found for player: {PhotonNetwork.LocalPlayer.NickName}");
            return;
        }

        if (isWinner)
        {
            gameOverTxt.text = "승리!";
            gameOverBackground.color = new Color(180f / 255f, 255f / 255f, 180f / 255f, 100f / 255f);
        }
        else
        {
            gameOverTxt.text = "패배";
            gameOverBackground.color = new Color(255f / 255f, 180f / 255f, 180f / 255f, 100f / 255f);
        }
        killAmount.text = "처치한 플레이어 수 : " + localPlayerStats.match_kills.ToString();
        dealAmount.text = "순위 : " + localPlayerStats.match_rank.ToString();
        Debug.Log("GameOver UI shown successfully");
    }

    public void ToMainPage()
    {
        Debug.Log("ToMainPage called");
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Player left the room");
        SceneManager.LoadScene("LobbyScene");
    }
}