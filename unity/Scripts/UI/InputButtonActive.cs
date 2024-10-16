using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputButtonActive : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TMP_InputField inputField1;  // TMP_InputField로 변경
    public TMP_InputField inputField2;  // TMP_InputField로 변경
    public Button button; // 로그인 버튼
    public Color activeColor = Color.black; // 활성화 색상
    public Color inactiveColor = new Color(1, 1, 1, 0.5f); // 비활성화 색상

    public Vector3 normalScale = new Vector3(1f, 1f, 1f);  // 버튼의 기본 크기
    public Color hoverColor = Color.green; // 마우스가 버튼 위에 있을 때 색상

    private Color originalColor; // 원래 색상을 저장할 변수

    void Start()
    {
        UpdateButtonAppearance(); // 초기 색상 및 크기 업데이트
        if (inputField1 != null)
            inputField1.onValueChanged.AddListener(delegate { UpdateButtonAppearance(); });
        if (inputField2 != null)
            inputField2.onValueChanged.AddListener(delegate { UpdateButtonAppearance(); });
    }

    void UpdateButtonAppearance()
    {
        // 활성화 조건 검사
        bool isField1Active = inputField1 != null && !string.IsNullOrEmpty(inputField1.text);
        bool isField2Active = inputField2 != null && !string.IsNullOrEmpty(inputField2.text);

        // 두 입력 필드 중 하나만 존재하는 경우 그 필드의 내용만 검사
        if (inputField1 == null || inputField2 == null)
        {
            button.interactable = isField1Active || isField2Active;
        }
        // 두 입력 필드 모두 존재하는 경우, 두 필드 모두 내용이 있어야 활성화
        else
        {
            button.interactable = isField1Active && isField2Active;
        }

        // 버튼의 색상 변경
        button.image.color = button.interactable ? activeColor : inactiveColor;

        // 마우스가 버튼 위에 올라가거나 내려가는 이벤트를 통해 크기 변경 처리
    }

    // 마우스가 버튼 위에 올라갔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            button.image.color = hoverColor; // 마우스 오버 시 색상 변경
        }
    }

    // 마우스가 버튼에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            button.image.color = button.interactable ? activeColor : inactiveColor; // 크기 원래대로
        }
    }
}
