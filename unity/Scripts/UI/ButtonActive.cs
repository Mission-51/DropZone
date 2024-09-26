using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonActive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button button; // 로그인 버튼
    public Vector3 normalScale = new Vector3(1f, 1f, 1f);  // 버튼의 기본 크기
    public Vector3 enlargedScale = new Vector3(1.2f, 1.2f, 1.2f);  // 버튼의 확대된 크기

    public Color normalColor = Color.white; // 기본 색상
    public Color hoverColor = Color.green; // 마우스 오버 시 색상

    // 마우스가 버튼 위에 올라갔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 크기 확대
        button.transform.localScale = enlargedScale;
        // 색상 변경
        button.image.color = hoverColor;
    }

    // 마우스가 버튼에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 크기 원래대로
        button.transform.localScale = normalScale;
        // 색상 원래대로
        button.image.color = normalColor;
    }
}
