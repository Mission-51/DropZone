using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChestManager : MonoBehaviour
{
    public TextMeshProUGUI chestText;
    public Transform chestTopPos;
    public ParticleSystem openParticle;
    public GameObject chestBoxUI;
    public bool isChestOpened = false; // 박스의 오픈 여부
    private bool isPlayerNear; // 플레이어가 가까이 있는지 상태

    private void OnTriggerEnter(Collider other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            // 플레이어와 닿았을 때 텍스트 나타나도록
            chestText.gameObject.SetActive(true);

            // 플레이어가 있음을 전달
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other != null && other.CompareTag("Player"))
        {
            // 트리거 밖으로 나가면 텍스트 다시 비활성화
            chestText.gameObject.SetActive(false);

            // 플레이어가 밖으로 나갔음을 전달
            isPlayerNear = false;
        }
    }

    // 상자 열기 메서드
    public void OpenChest()
    {
        chestTopPos.rotation = Quaternion.Euler(0, 180, 0); // 상자 열기
        chestText.text = "to Close";
        chestBoxUI.SetActive(true);
        isChestOpened = !isChestOpened; // 상자 열림 처리
    }

    // 상자 닫기 메서드
    public void CloseChest()
    {
        chestTopPos.rotation = Quaternion.Euler(45, 180, 0); // 상자 닫기
        chestText.text = "to Open";
        chestBoxUI.SetActive(false);
        isChestOpened = !isChestOpened; // 상자 열림 처리
    }

    private void Update()
    {
        // 플레이어가 범위 안에 들어왔고, 상자가 열리지 않은 상태에서 F 키를 눌렀다면
        if (isPlayerNear && Input.GetKeyDown(KeyCode.F))
        {
            // 상자가 열린 상태인지, 닫힌 상태인지에 따라 다르게 적용
            if (!isChestOpened)
            {
                OpenChest();
            } else
            {
                CloseChest();
            }

            openParticle.Play(); // 파티클 재생
        }
    }
}
