using UnityEngine;

public class LoginUIManager : MonoBehaviour
{
    public GameObject loginPanel;    // 로그인 패널
    public GameObject registerPanel; // 회원가입 패널

    // 초기 설정에서 로그인 패널만 활성화
    void Start()
    {
        ShowLogin();
    }

    // 로그인 패널을 보이게 하고 회원가입 패널을 숨깁니다.
    public void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
    }

    // 회원가입 패널을 보이게 하고 로그인 패널을 숨깁니다.
    public void ShowRegister()
    {
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
    }
}
