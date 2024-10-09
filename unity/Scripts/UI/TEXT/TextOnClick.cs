using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TextOnClick : MonoBehaviour, IPointerClickHandler
{

    public TextMeshProUGUI TextMeshProUGUI;
    public List<GameObject> closeGameObjects = new List<GameObject>(); // 비활성화할 게임 오브젝트 리스트
    public List<GameObject> openGameObjects = new List<GameObject>();  // 활성화할 게임 오브젝트 리스트

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
