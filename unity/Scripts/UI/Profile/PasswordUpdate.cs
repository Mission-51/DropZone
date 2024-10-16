using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using WebSocketSharp;

public class PasswordUpdate : MonoBehaviour
{
    // JSON 데이터 변환을 위한 객체
    [System.Serializable]
    public class UserPassword
    {
        public string userPassword; // 유저 닉네임
    }

    // 변경 비밀번호 인풋필드
    public TMP_InputField pwChange;

    // 변경 비밀번호 확인 인풋필드
    public TMP_InputField pwChangeConfirm;

    // 비밀번호 확인 결과 텍스트
    public TextMeshProUGUI pwChangeTxt;

    // 서버 URL
    private string ec2URL = "https://j11d110.p.ssafy.io";

    // JWT 토큰
    private string accessToken;

    // 비밀번호(1차)
    private string pw1;

    // 비밀번호(2차)
    private string pw2;

    private void Start()
    {
        accessToken = PlayerPrefs.GetString("accessToken", "");

        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다.");
        }

        // 비밀번호 인풋필드 값 바뀔때마다 이벤트 호출
        pwChange.onValueChanged.AddListener(OnPwFieldChanged);
        // 비밀번호 확인 인풋필드 값 바뀔때마다 이벤트 호출
        pwChangeConfirm.onValueChanged.AddListener(OnPwConfirmFieldChanged);
    }

    private void Update()
    {
        // Tab키, Enter키 입력
        CheckForTabKey();
        CheckForEnterKey();

        // 유효성 검사(한글입력)
        if (pwChange.isFocused || pwChangeConfirm.isFocused)
        {
            Input.imeCompositionMode = IMECompositionMode.Off;
        }
        else
        {
            Input.imeCompositionMode = IMECompositionMode.Auto;
        }
    }

    public void CheckForTabKey()
    {
        // 탭버튼
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 비밀번호 입력창에 포커스 되어 있다면
            if (pwChange.isFocused)
            {
                Debug.Log("Tab키 입력");
                // 비밀번호 입력 확인 창으로 포커스 변경
                pwChangeConfirm.Select();
            }
        }
    }

    public void CheckForEnterKey()
    {
        // 엔터 버튼
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            // 비밀번호 입력 확인창에 포커스 되어 있다면
            if (pwChangeConfirm.isFocused)
            {
                Debug.Log("Enter키 입력");
                OnEditBtnClicked();
            }
        }
    }

    // 비밀번호 필드 변경시 업데이트
    public void OnPwFieldChanged(string inputValue)
    {
        // 비밀번호 확인란에 업데이트
        pw1 = inputValue;
    }

    // 비밀번호 확인 필드 변경시 없데이트
    public void OnPwConfirmFieldChanged(string inputValue)
    {
        pw2 = inputValue;

        if (pwChangeConfirm != null)
        {
            if (pw1 != pw2)
            {
                pwChangeTxt.text = "<color=red>비밀번호가 일치하지 않습니다.</color>";
            }
            else if (pw1 == pw2)
            {
                pwChangeTxt.text = "<color=green>비밀번호가 일치합니다.</color>";
            }
        }
    }


    // 버튼 클릭시 발생할 이벤트
    public void OnBtnClicked()
    {
        // 꺼져있으면 키기
        if (!this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(true);
        }
        // 켜져있으면 끄기
        else
        {
            pwChange.text = "";
            pwChangeConfirm.text = "";
            pwChangeTxt.text = "";
            this.gameObject.SetActive(false);
        }
    }

    // 변경 버튼 클릭 이벤트
    public void OnEditBtnClicked()
    {
        if (pw1 != pw2)
        {
            pwChange.text = "";
            pwChangeConfirm.text = "";
            return;
        }

        StartCoroutine(EditNickname($"{ec2URL}/api/users/update/password", pw2));
    }

    IEnumerator EditNickname(string url, string newPassword)
    {
        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다. 닉네임 변경을 중단합니다.");
            yield break;
        }
        // JSON 데이터 생성
        UserPassword password = new UserPassword { userPassword = newPassword };
        string jsonData = JsonUtility.ToJson(password);
        Debug.Log(jsonData);

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 방식 변경: PUT => PATCH
            request.method = "PATCH";
            // 토큰 추가
            AddAuthorizationHeader(request);

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                pwChangeTxt.text = "<color=green>비밀번호 변경 성공</color>";
                Debug.Log("비밀번호 변경 성공: " + request.downloadHandler.text);
            }
            else
            {
                pwChange.text = "";
                pwChangeConfirm.text = "";
                pwChangeTxt.text = "<color=red>비밀번호 변경 실패</color>";
                Debug.Log("닉네임 변경 실패: " + request.error);
            }
        }
    }

    // 토큰 추가 함수
    private UnityWebRequest AddAuthorizationHeader(UnityWebRequest request)
    {
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        return request;
    }
}
