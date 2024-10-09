using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMoveObject : MonoBehaviour, IPointerClickHandler
{
    public UIMoveManager moveManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 이동 방향 변경
        moveManager.isMovingToTarget = !moveManager.isMovingToTarget;

        // 목표 위치로 이동하거나 다시 제자리로 이동
        if (moveManager.isMovingToTarget)
        {
            moveManager.ActivateFriendList(); // friendlist 활성화
            StartCoroutine(moveManager.MoveUI(moveManager.initialPosition, moveManager.targetPosition));
        }
        else
        {
            StartCoroutine(moveManager.MoveUI(moveManager.targetPosition, moveManager.initialPosition));
        }
    }
}
