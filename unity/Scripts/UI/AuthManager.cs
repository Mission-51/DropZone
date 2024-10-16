using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // 씬 전환을 위해 추가
using TMPro;
using System.Threading.Tasks;

public class AuthManager : MonoBehaviour
{
    [System.Serializable]
    public class User
    {
        public string userEmail; // 이메일
        public string userPassword; // 비밀번호
        public string userNickname; // 닉네임
    }


    [System.Serializable]
    public class LoginResponse
    {
        public int id;  // 로그인 응답에서의 유저 ID     // 유저 ID
        public string message;
        public string accessToken;
        public string refreshToken;
    }

    private string baseURL = "http://localhost:8080/"; //local
    private string ec2URL = "https://j11d110.p.ssafy.io/"; //ec2
    private bool isLoginInProgress = false;

    //inspector
    public GameObject RequestCodeBtn;
    public GameObject VerifyBtn;
    public GameObject RegistBtn;
    public GameObject LoginBtn;
    public GameObject email;
    public GameObject verifyCodefield;
    public TMP_Text requestcode;
    public TMP_Text codecheck;
    public GameObject password;
    public GameObject passwordCheck;
    public GameObject nickname;
    public TMP_Text nicknameCheckResultText;   // 닉네임 중복 확인 결과를 보여줄 Text (UI에 표시)
    public GameObject LoginEamil;
    public GameObject LoginPassword;
    public TMP_Text loginErrorText;          // 로그인 실패 메시지 표시를 위한 텍스트 필드
    public TMP_Text RegistorErrorText;
    public TMP_InputField pwInputField;         // 비밀번호 입력 필드
    public TMP_InputField pwConfirmInputField;  // 비밀번호 확인 입력 필드

    public GameObject loginpanel;
    public GameObject registerPanel;

    private bool registeremailcheck = false;
    private bool registerpasswordcheck = false;
    private bool registernicknamecheck = false;
    private bool isRegisterInProgress = false; // 회원가입 중인지 체크하는 변수


    private void Update()
    {
        CheckForTabKey();
        CheckForEnterKey();

        if ((pwInputField != null && pwInputField.isFocused) || (pwConfirmInputField != null && pwConfirmInputField.isFocused) || (LoginPassword.GetComponent<TMP_InputField>() != null && LoginPassword.GetComponent<TMP_InputField>().isFocused))
        {
            Input.imeCompositionMode = IMECompositionMode.Off; // IME 비활성화 (영어로만 입력)
        }
        else
        {
            Input.imeCompositionMode = IMECompositionMode.On; // 다른 필드에서는 IME 자동 설정
        }
    }

    // Tab 키가 눌렸는지 확인
    void CheckForTabKey()
    {
        // Tab 키가 눌렸고, 현재 아이디 입력 필드에 포커스가 있는지 확인
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (LoginEamil.GetComponent<TMP_InputField>().isFocused)
            {
                Debug.Log("Tab 키 눌림");
                // 비밀번호 입력 필드로 포커스 이동
                LoginPassword.GetComponent<TMP_InputField>().Select();
            }
            else if (email.GetComponent<TMP_InputField>().isFocused)
            {
                verifyCodefield.GetComponent<TMP_InputField>().Select();
            }
            else if (verifyCodefield.GetComponent<TMP_InputField>().isFocused)
            {
                password.GetComponent<TMP_InputField>().Select();
            }
            else if (password.GetComponent<TMP_InputField>().isFocused)
            {
                passwordCheck.GetComponent<TMP_InputField>().Select();
            }
            else if (passwordCheck.GetComponent<TMP_InputField>().isFocused)
            {
                nickname.GetComponent<TMP_InputField>().Select();
            }

        }


    }

    void CheckForEnterKey()
    {
        // 이메일 또는 비밀번호 입력 필드에 포커스가 있고 Enter 키가 눌렸는지 확인
        if ((LoginEamil.GetComponent<TMP_InputField>().isFocused || LoginPassword.GetComponent<TMP_InputField>().isFocused)
            && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))) // Enter 키 (Return & Keypad Enter)
        {
            Debug.Log("Enter 키가 눌렸습니다.");
            Login(); // 로그인 함수 호출
        }
    }


    //인증 번호 요청
    public void RequestVerificationCode()
    {
        if (email.GetComponent<TMP_InputField>().text.Length > 0)
        {
            StartCoroutine(RequestCodeCoroutine(email.GetComponent<TMP_InputField>().text));
        }
    }

    //인증 번호 요청 보내는 함수
    private IEnumerator RequestCodeCoroutine(string email)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post($"{ec2URL}api/users/sendEmail?email={email}", ""))
        {
            requestcode.color = Color.white;
            requestcode.text = "전송 중...";

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("인증 번호 발송 : " + webRequest.downloadHandler.text);
                requestcode.color = Color.green;
                requestcode.text = "전송에 성공했습니다.";
            }
            else
            {
                Debug.LogError("인증 번호 발송 실패 : " + webRequest.error);
                requestcode.color = Color.red;
                requestcode.text = "전송에 실패했습니다.";
            }
        }
    }

    //인증 번호 검증
    public void VerifyCode()
    {
        if (verifyCodefield.GetComponent<TMP_InputField>().text.Length > 0)
        {
            StartCoroutine(VerifyCodeCoroutine(email.GetComponent<TMP_InputField>().text, verifyCodefield.GetComponent<TMP_InputField>().text));
            Debug.Log(email.GetComponent<TMP_InputField>().text);
            Debug.Log(verifyCodefield.GetComponent<TMP_InputField>().text);
        }
    }

    //인증 번호 검증 실행 로직
    private IEnumerator VerifyCodeCoroutine(string email, string code)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/users/authenticateEmail?user_email={UnityWebRequest.EscapeURL(email)}&authenticationCode={UnityWebRequest.EscapeURL(code)}"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("인증 완료 : " + webRequest.downloadHandler.text);
                codecheck.color = Color.green;
                codecheck.text = "인증이 완료되었습니다.";
                registeremailcheck = true;
            }
            else
            {
                Debug.LogError("인증 실패: " + webRequest.error);
                codecheck.color = Color.red;
                codecheck.text = "인증에 실패했습니다.";

                registeremailcheck = false;
            }
        }
    }

    public void CheckPassword()
    {
        if (password.GetComponent<TMP_InputField>().text == passwordCheck.GetComponent<TMP_InputField>().text)
        {
            registerpasswordcheck = true;
        }
        else
        {
            registerpasswordcheck = false;
        }
    }


    public void CheckNicknameDuplicate()
    {


        if (string.IsNullOrEmpty(nickname.GetComponent<TMP_InputField>().text))
        {
            Debug.LogError("닉네임을 입력하세요.");
            return;
        }

        StartCoroutine(CheckNicknameCoroutine(nickname.GetComponent<TMP_InputField>().text));
    }

    // 닉네임 중복 확인 요청 로직
    private IEnumerator CheckNicknameCoroutine(string nickname)
    {
        string url = $"{ec2URL}api/users/checkDuplicated/user_nickname/{UnityWebRequest.EscapeURL(nickname)}";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 서버에서 받은 결과를 처리
                if (webRequest.downloadHandler.text.Contains("가능한"))
                {
                    Debug.Log("닉네임 사용 가능");
                    nicknameCheckResultText.text = "사용가능합니다.";
                    nicknameCheckResultText.color = Color.green;  // 텍스트 색을 녹색으로 변경
                    registernicknamecheck = true;
                }
            }
            else
            {
                if (webRequest.downloadHandler.text.Contains("중복"))
                {
                    Debug.Log("닉네임 중복");
                    nicknameCheckResultText.text = "중복되었습니다.";
                    nicknameCheckResultText.color = Color.red;  // 텍스트 색을 빨간색으로 변경
                    registernicknamecheck = false;
                }
                else
                {

                    Debug.LogError("닉네임 중복 확인 실패: " + webRequest.error);
                    nicknameCheckResultText.text = "서버 요청 실패.";
                    nicknameCheckResultText.color = Color.red;
                }
            }
        }
    }

    public void CheckAllConditionsAndRegister()
    {
        if (isRegisterInProgress) // 이미 회원가입이 진행 중이라면 다시 시도하지 않음
        {
            Debug.Log("이미 회원가입 시도가 진행 중입니다.");
            return;
        }

        // 세 가지 조건이 모두 true일 때만 회원가입 진행
        if (registeremailcheck && registerpasswordcheck && registernicknamecheck)
        {
            isRegisterInProgress = true; // 회원가입 진행 상태로 설정
            Register(); // 회원가입 함수 호출
        }
        else if (registeremailcheck == false)
        {
            Debug.LogError("이메일 인증이 되지 않았습니다.");
        }
        else if (registerpasswordcheck == false)
        {
            Debug.LogError("비밀번호가 일치하지 않습니다.");
        }
        else if (registernicknamecheck == false)
        {
            Debug.LogError("닉네임이 중복되었습니다.");
        }
    }


    //회원 가입
    public void Register()
    {
        string username = email.GetComponent<TMP_InputField>().text;
        string verifypassword = password.GetComponent<TMP_InputField>().text;
        string usernickname = nickname.GetComponent<TMP_InputField>().text;
        StartCoroutine(RegisterCoroutine(username, verifypassword, usernickname));
    }

    //회원 가입 실행 로직
    private IEnumerator RegisterCoroutine(string username, string password, string nickname)
    {
        User user = new User { userEmail = username, userPassword = password, userNickname = nickname };
        string jsonData = JsonUtility.ToJson(user);
        Debug.Log(jsonData);

        using (UnityWebRequest webRequest = UnityWebRequest.Post($"{ec2URL}api/users/signup", jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("가입완료 : " + webRequest.downloadHandler.text);
                loginpanel.SetActive(true);
                registerPanel.SetActive(false);


            }
            else
            {
                Debug.LogError("서버 응답: " + webRequest.downloadHandler.text);
                Debug.LogError("가입실패: " + webRequest.error);
                RegistorErrorText.color = Color.red;
                RegistorErrorText.text = "가입에 실패하였습니다.";
                
            }
            isRegisterInProgress = false; // 회원가입이 완료되었으므로 상태 초기화
        }
    }
    public void Login()
    {
        if (isLoginInProgress)
        {
            Debug.Log("이미 로그인 시도가 진행 중입니다.");
            return;
        }

        string username = LoginEamil.GetComponent<TMP_InputField>().text;
        string userpassword = LoginPassword.GetComponent<TMP_InputField>().text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userpassword))
        {
            Debug.LogError("이메일과 비밀번호를 입력하세요.");
            loginErrorText.text = "이메일과 비밀번호를 입력하세요."; // 실패 메시지 표시
            loginErrorText.color = Color.red; // 메시지 색상 설정 (빨간색)
            return;
        }

        StartCoroutine(LoginCoroutine(username, userpassword));
    }


    // 로그인 실행 로직
    private IEnumerator LoginCoroutine(string username, string password)
    {
        isLoginInProgress = true; // 로그인 중인 상태로 설정


        User user = new User { userEmail = username, userPassword = password };
        string jsonData = JsonUtility.ToJson(user);

        using (UnityWebRequest webRequest = UnityWebRequest.Post($"{ec2URL}api/auth/login", jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 서버 응답 로그 출력
                string responseText = webRequest.downloadHandler.text;
                Debug.Log("로그인 응답: " + responseText);

                // 응답에서 ID, AccessToken, RefreshToken 가져오기
                LoginResponse response = JsonUtility.FromJson<LoginResponse>(responseText);

                // ID, AccessToken, RefreshToken을 PlayerPrefs에 저장
                PlayerPrefs.SetInt("userId", response.id);
                PlayerPrefs.SetString("accessToken", response.accessToken);
                PlayerPrefs.SetString("refreshToken", response.refreshToken);
                PlayerPrefs.Save();

                // 로그인 성공 메시지 표시
                loginErrorText.text = "로그인 성공!";
                loginErrorText.color = Color.green; // 성공 메시지는 초록색으로 설정

                // 저장된 값 확인 (디버그 로그)
                Debug.Log("저장된 userId: " + PlayerPrefs.GetInt("userId", 0));
                Debug.Log("저장된 accessToken: " + PlayerPrefs.GetString("accessToken", "No Access Token Found"));

                // 로비 씬으로 이동
                SceneManager.LoadScene("LobbyScene");
            }
            else
            {
                // 로그인 실패 메시지 표시
                Debug.LogError("로그인 실패: " + webRequest.error);
                loginErrorText.text = "로그인에 실패했습니다. 다시 시도하세요.";
                loginErrorText.color = Color.red; // 실패 메시지는 빨간색으로 설정
            }
        }
        isLoginInProgress = false; // 로그인 완료 후 상태 초기화
    }

    
}




