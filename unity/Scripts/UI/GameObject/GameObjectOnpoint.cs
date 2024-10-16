using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Image를 다루기 위해 필요

public class GameObjectOnpoint : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image imageComponent; // Image 컴포넌트
    private Image[] childImgs; // 자식 이미지들
    private Color originalColor; // 원래 색상
    public Color hoverColor = Color.red; // 마우스를 올렸을 때 바꿀 색상
    public AudioSource clickSound; // 클릭 시 재생할 사운드
    

    void Start()
    {
        // Image 컴포넌트 가져오기
        imageComponent = GetComponent<Image>();
        // 자식 컴포넌트들 가져오기
        childImgs = GetComponentsInChildren<Image>();

        // 원래 색상 저장
        if (imageComponent != null)
        {
            originalColor = imageComponent.color;
        }
    }

    // 마우스를 오브젝트 위에 올렸을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (imageComponent != null && childImgs != null)
        {
            imageComponent.color = hoverColor; // 마우스를 올리면 hoverColor로 변경
            
           foreach (Image img in childImgs)
            {
                if (img.color == originalColor)
                {
                    img.color = hoverColor;
                }
            }
        }
    }

    // 마우스를 오브젝트에서 내렸을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (imageComponent != null && childImgs != null)
        {
            imageComponent.color = originalColor; // 원래 색상으로 복귀

            foreach (Image img in childImgs)
            {
                if (img.color == hoverColor)
                {
                    img.color = originalColor;
                }
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if ( imageComponent != null && childImgs != null)
        {
            imageComponent.color = originalColor;

            foreach (Image img in childImgs)
            {
                if (img.color == hoverColor)
                {
                    img.color = originalColor;
                }
            }
        }
    }


   
}
