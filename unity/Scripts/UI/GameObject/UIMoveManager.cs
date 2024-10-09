using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMoveManager : MonoBehaviour
{
    public RectTransform uiObject; // 이동할 UI 오브젝트의 RectTransform
    public Vector2 targetPosition; // 이동할 목표 위치
    public float moveSpeed = 300f; // 이동 속도
    public Vector2 initialPosition; // 초기 위치
    public bool isMovingToTarget = false; // 이동 방향

    public GameObject friendList; // friendlist 오브젝트

    private void Start()
    {
        // 오브젝트의 초기 위치 저장
        initialPosition = uiObject.anchoredPosition;
        friendList.SetActive(false); // 초기에는 friendlist 비활성화
    }

    // friendlist를 활성화하는 함수
    public void ActivateFriendList()
    {
        friendList.SetActive(true);
    }

    // friendlist를 비활성화하는 함수
    public void DeactivateFriendList()
    {
        friendList.SetActive(false);
    }

    // UI 오브젝트를 이동시키는 코루틴
    public IEnumerator MoveUI(Vector2 startPos, Vector2 endPos)
    {
        float elapsedTime = 0f;
        float totalDistance = Vector2.Distance(startPos, endPos);

        while (elapsedTime < totalDistance / moveSpeed)
        {
            // 이동 진행
            uiObject.anchoredPosition = Vector2.Lerp(startPos, endPos, (elapsedTime * moveSpeed) / totalDistance);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // 최종 위치 설정
        uiObject.anchoredPosition = endPos;

        // 목표 위치에 도착하면 friendlist 비활성화
        if (!isMovingToTarget)
        {
            DeactivateFriendList();
        }
    }
}
