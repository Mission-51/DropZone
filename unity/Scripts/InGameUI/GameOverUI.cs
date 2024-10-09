using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class GameOverUI : MonoBehaviour
{
    // 게임오버 게임 오브젝트
    public GameObject go_GameOverUI;
    // 게임오버 배경 이미지
    public Image gameOverBackground;
    // 게임오버 텍스트
    public TextMeshProUGUI gameOverTxt;
    // 킬량
    public TextMeshProUGUI killAmount;
    // 딜량
    public TextMeshProUGUI dealAmount;

    // Update is called once per frame
    void Update()
    {
        // 게임 종료시
        if (GameManager.instance.isGameOver)
        {
            go_GameOverUI.SetActive(true);

            var winnerName = GameManager.instance.GetPlayers()[0].GetComponent<PhotonView>().Owner.NickName;
            // 승자일 경우
            if (PhotonNetwork.LocalPlayer.NickName == winnerName)
            {
                gameOverTxt.text = "승리!";
                gameOverBackground.color = new Color(180, 255, 180, 100);
            }
            // 패자일 경우
            else
            {
                gameOverTxt.text = "패배";
                gameOverBackground.color = new Color(255, 180, 180, 100);
            }

            // 킬량, 딜량 기록
            killAmount.text = "처치한 플레이어 수 : " + GameManager.instance.GetPlayerStats()[winnerName].kills.ToString();
            dealAmount.text = "입힌 피해량 : " + GameManager.instance.GetPlayerStats()[winnerName].damageDeal.ToString();
        }
    }

    public void ToMainPage()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
