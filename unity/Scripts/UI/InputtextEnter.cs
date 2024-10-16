using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InputtextEnter : MonoBehaviour
{
    public TMP_InputField inputField;  // 채팅 입력 필드 (TextMeshPro)
    public Button Button;  // '보내기' 버튼

    void Start()
    {
        // TextMeshPro InputField의 onSubmit 이벤트를 사용하여 엔터키를 눌렀을 때 버튼 클릭 이벤트를 트리거
        inputField.onSubmit.AddListener(delegate { Triggerclick(); });
    }

    // '보내기' 버튼 클릭과 같은 효과를 주는 메서드
    public void Triggerclick()
    {
        // 버튼 클릭 효과와 동일하게 처리
        Button.onClick.Invoke();
    }
}
