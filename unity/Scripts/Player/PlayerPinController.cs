using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerPinController : MonoBehaviourPun
{
    public Transform playerPos;
    public Renderer pinRenderer; // 핀의 Renderer
    private Color originalColor = Color.white; // 기본 색상 (흰색)
    private Color enemyColor = Color.red; // 적 색상 (빨간색)
    private float pinScale = 5.0f; // 핀의 크기

    private void Awake()
    {
        Debug.Log("핀포인트");
        playerPos = photonView.transform;

        pinRenderer = GetComponent<Renderer>();
        if (pinRenderer != null)
        {
            pinRenderer.material.color = originalColor;
            transform.localScale = new Vector3(pinScale, pinScale, pinScale);
        }
    }

    private void Update()
    {
        this.transform.position = playerPos.position + new Vector3(0, 80, 0);
        UpdatePinColor();
    }

    private void UpdatePinColor()
    {
        if (photonView.IsMine)
        {
            pinRenderer.material.color = originalColor;
        }
        else
        {
            pinRenderer.material.color = enemyColor;
        }
    }

    private bool IsEnemy(Photon.Realtime.Player player)
    {
        return player != PhotonNetwork.LocalPlayer;
    }
}
