using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Button을 사용하기 위해 필요

public class ButtonClickManager : MonoBehaviour, IPointerClickHandler
{
    public Button button; // 버튼 컴포넌트를 참조
    public List<GameObject> closeGameObjects = new List<GameObject>(); // 비활성화할 게임 오브젝트 리스트
    public List<GameObject> openGameObjects = new List<GameObject>();  // 활성화할 게임 오브젝트 리스트

    // 클릭 이벤트 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        // closeGameObjects 리스트에 있는 오브젝트들 비활성화
        foreach (GameObject obj in closeGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // openGameObjects 리스트에 있는 오브젝트들 활성화
        foreach (GameObject obj in openGameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}
