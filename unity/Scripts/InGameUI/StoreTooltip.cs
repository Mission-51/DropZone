using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class StoreTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip; // 툴팁 UI 오브젝트
    public TextMeshProUGUI tooltipText; // 툴팁 텍스트

    // 마우스를 물음표 아이콘 위에 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowTooltip();
    }

    // 마우스를 물음표 아이콘에서 떼었을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    // 툴팁 보여주기
    private void ShowTooltip()
    {
        tooltip.SetActive(true); // 툴팁 활성화        
    }

    // 툴팁 숨기기
    private void HideTooltip()
    {
        tooltip.SetActive(false); // 툴팁 비활성화
    }
}
