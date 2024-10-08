using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PasswordCheck : MonoBehaviour
{
    // 비밀번호와 비밀번호 확인 InputField
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    // 비밀번호가 일치할 때 보여줄 아이콘 및 텍스트
    
    public TextMeshProUGUI matchText; // 맞는 경우 보여줄 텍스트

    // 비밀번호가 일치하지 않을 때 보여줄 메시지
    public TextMeshProUGUI errorText;

    void Start()
    {
        // 처음에 아이콘과 메시지를 숨김
        
        matchText.gameObject.SetActive(false);
        errorText.gameObject.SetActive(false);

        // InputField 값 변경을 감지하는 리스너 추가
        passwordField.onValueChanged.AddListener(delegate { CheckPasswordMatch(); });
        confirmPasswordField.onValueChanged.AddListener(delegate { CheckPasswordMatch(); });
    }

    // 비밀번호와 비밀번호 확인 필드의 일치 여부를 확인하는 함수
    void CheckPasswordMatch()
    {
        if (!string.IsNullOrEmpty(passwordField.text) && passwordField.text == confirmPasswordField.text)
        {
            // 비밀번호가 일치할 경우
            
            matchText.gameObject.SetActive(true);
            errorText.gameObject.SetActive(false);
            matchText.text = "비밀번호가 일치합니다!"; // 원하는 메시지로 수정 가능
        }
        else
        {
            // 비밀번호가 일치하지 않을 경우
            
            matchText.gameObject.SetActive(false);
            errorText.gameObject.SetActive(true);
            errorText.text = "비밀번호가 일치하지 않습니다!"; // 원하는 오류 메시지로 수정 가능
        }
    }
}
