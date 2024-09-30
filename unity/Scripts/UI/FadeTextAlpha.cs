using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class FadeTextAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CanvasGroup fadeText1Group;  // 첫 번째 텍스트의 CanvasGroup
    public CanvasGroup fadeText2Group;  // 두 번째 텍스트의 CanvasGroup

    public float fadeDuration = 0.5f;  // 알파값이 변하는 시간 (초)

    private bool isAlphaLocked = false; // Alpha가 고정 상태인지 여부를 체크

    private void Start()
    {
        SetAlpha(fadeText1Group, 0);
        SetAlpha(fadeText2Group, 0);

    }

    // 마우스가 플레이 텍스트 위에 올라갔을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Alpha 값이 1로 고정된 상태가 아니면 페이드 인 실행
        if (!isAlphaLocked)
        {
            StartCoroutine(FadeInText(fadeText1Group));
            StartCoroutine(FadeInText(fadeText2Group));
        }
    }

    // 마우스가 플레이 텍스트에서 벗어났을 때
    public void OnPointerExit(PointerEventData eventData)
    {
        // Alpha 값이 고정 상태가 아니면 즉시 페이드 아웃
        if (!isAlphaLocked)
        {
            StopAllCoroutines(); // 코루틴 중지 (페이드 인 중일 때 즉시 페이드 아웃되도록)
            fadeText1Group.alpha = 0;
            fadeText2Group.alpha = 0;

            fadeText1Group.gameObject.SetActive(false);
            fadeText2Group.gameObject.SetActive(false);
        }
    }

    // 플레이 텍스트를 클릭했을 때 alpha 값을 1로 고정하거나 0으로 설정
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isAlphaLocked)
        {
            // 이미 Alpha가 1로 고정된 경우 -> Alpha를 0으로 설정하고 비활성화
            SetAlpha(fadeText1Group, 0);
            SetAlpha(fadeText2Group, 0);
            isAlphaLocked = false;
        }
        else
        {
            // Alpha를 1로 고정하고 활성화
            SetAlpha(fadeText1Group, 1);
            SetAlpha(fadeText2Group, 1);
            isAlphaLocked = true;
        }
    }

    // Alpha 값을 서서히 증가시키는 함수
    private IEnumerator FadeInText(CanvasGroup canvasGroup)
    {
        canvasGroup.gameObject.SetActive(true);
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 1; // 최종적으로 알파값을 1로 고정
    }

    // Alpha 값을 설정하고 활성화/비활성화 상태를 변경하는 함수
    private void SetAlpha(CanvasGroup canvasGroup, float alphaValue)
    {
        canvasGroup.alpha = alphaValue;
        canvasGroup.gameObject.SetActive(alphaValue > 0);
    }
}
