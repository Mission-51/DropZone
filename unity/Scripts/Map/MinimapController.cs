using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class MinimapController : MonoBehaviourPun
{
    public Transform minimap; // 미니맵의 Transform
    public Camera minimapCam; // 미니맵 카메라

    private Transform playerPos; // 플레이어의 위치

    private void Start()
    {
        Debug.Log("HI");
        // 로컬 플레이어의 Transform을 가져옴
        if (photonView.IsMine)
        {
            playerPos = photonView.transform;
            minimapCam = GameObject.Find("MinimapCamera").GetComponent<Camera>();
            minimap = GameObject.Find("MinimapBg").GetComponent<Transform>();
            Debug.Log("로컬 플레이어의 위치: " + playerPos.position);
        }
        else
        {
            Debug.Log("로컬 플레이어가 아닙니다.");
        }
    }

    void Update()
    {
        // 미니맵 카메라를 플레이어의 위치에 따라 업데이트
        if (playerPos != null)
        {
            // 카메라의 위치를 플레이어 위치 + 높이 설정
            minimapCam.transform.position = playerPos.position + new Vector3(0, 100, 0);
            minimapCam.transform.rotation = Quaternion.Euler(90, 0, 0); // 카메라가 항상 위를 바라보도록 설정
        }

        // 미니맵 크기 조절
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            minimap.localScale *= 2;
            minimapCam.orthographicSize = 100;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            minimap.localScale /= 2;
            minimapCam.orthographicSize = 50;
        }
    }
}
