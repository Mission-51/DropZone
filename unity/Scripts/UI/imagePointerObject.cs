using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Image를 다루기 위해 필요

public class ImagePointerObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Image imageComponent; // Image 컴포넌트
    public GameObject targetObject; // 마우스를 올렸을 때 활성화할 오브젝트

    void Start()
    {
        // Image 컴포넌트 가져오기
        imageComponent = GetComponent<Image>();

        // 처음에 targetObject 비활성화
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }

    // 마우스를 오브젝트 위에 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            // 마우스를 올렸을 때 targetObject 활성화
            targetObject.SetActive(true);
        }
    }

    // 마우스를 오브젝트에서 내렸을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetObject != null)
        {
            // 마우스를 벗어났을 때 targetObject 비활성화
            targetObject.SetActive(false);
        }
    }
}
