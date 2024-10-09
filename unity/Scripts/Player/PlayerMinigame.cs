using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMinigame : MonoBehaviour
{
    private List<Vector3> roundPositions = new List<Vector3>();

    private Transform playerPosition;

    private int level;

    void Start()
    {
        playerPosition = this.GetComponent<Transform>();

        if (MinigameManager.instance.playerPositions[0] != null)
        {
            for (int i = 0; i < 5; i++)
            {
                roundPositions.Add(MinigameManager.instance.playerPositions[0][i]);
                Debug.Log(roundPositions[i]);
            }
        }

        level = 0;

        Debug.Log($"Player {this.name} - Round Numbers: {MinigameManager.instance.roundOneNum}, {MinigameManager.instance.roundTwoNum}, {MinigameManager.instance.roundThreeNum}, {MinigameManager.instance.roundFourNum}");
    }

    private void Update()
    {
        if (level == 4)
        {
            MinigameManager.instance.isGameOver = true;
            MinigameManager.instance.nickname = PhotonNetwork.NickName;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Portal"))
        {
            int portalNum = int.Parse(other.gameObject.name);

            if (level == 0)
            {
                // 포탈번호와 랜덤번호 비교
                // 옳은 포탈이라면
                if (portalNum == MinigameManager.instance.roundOneNum)
                {
                    level++;
                    Debug.Log(level);
                    playerPosition.position = roundPositions[1];
                }
                // 틀린 포탈이라면
                else
                {
                    playerPosition.position = roundPositions[0];
                }
            }
            else if (level == 1)
            {
                // 포탈번호와 랜덤번호 비교
                // 옳은 포탈이라면
                if (portalNum == MinigameManager.instance.roundTwoNum)
                {
                    level++;
                    Debug.Log(level);
                    playerPosition.position = roundPositions[2];
                }
                // 틀린 포탈이라면
                else
                {
                    playerPosition.position = roundPositions[1];
                }
            }
            else if (level == 2)
            {
                // 포탈번호와 랜덤번호 비교
                // 옳은 포탈이라면
                if (portalNum == MinigameManager.instance.roundThreeNum)
                {
                    level++;
                    Debug.Log(level);
                    playerPosition.position = roundPositions[3];
                }
                // 틀린 포탈이라면
                else
                {
                    playerPosition.position = roundPositions[2];
                }
            }
            else
            {
                // 포탈번호와 랜덤번호 비교
                // 옳은 포탈이라면
                if (portalNum == MinigameManager.instance.roundFourNum)
                {
                    level++;
                    Debug.Log(level);
                    playerPosition.position = roundPositions[4];
                }
                // 틀린 포탈이라면
                else
                {
                    playerPosition.position = roundPositions[3];
                }
            }
        }
    }
}
