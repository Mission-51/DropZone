using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using WebSocketSharp;

public class NicknameUpdate : MonoBehaviour
{
    // JSON 데이터 변환을 위한 객체
    [System.Serializable]
    public class Nickname
    {
        public string userNickname; // 유저 닉네임
    }

    // 닉네임 인풋필드
    public TMP_InputField nicknameInput;

    // 색변경을 위한 부모 버튼
    public Button editBtn;

    // 수정(펜) 이미지
    public GameObject penImg;

    // 확인(체크) 이미지
    public GameObject checkImg;

    // 닉네임 변경 확인 문구
    public TextMeshProUGUI isNickCanChange;

    // 서버 URL
    private string ec2URL = "https://j11d110.p.ssafy.io";

    // JWT 토큰
    private string accessToken;

    // 바뀌기 전 닉네임
    private string originalNickname;

    // 바꾼 닉네임
    private string editedNickname;

    // 닉네임 사용가능여부
    private bool canUseNickname;

    private void Start()
    {
        nicknameInput = GetComponentInChildren<TMP_InputField>();
        nicknameInput.text = LobbyManager.instance.GetNick();
        nicknameInput.interactable = false;
        accessToken = PlayerPrefs.GetString("accessToken", "");

        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다.");
        }

        // 인풋필드 값 바뀔때마다 이벤트 호출
        nicknameInput.onValueChanged.AddListener(OnInputFieldChanged);
    }

    // 인풋필드의 값을 저장하는 메서드
    public void OnInputFieldChanged(string inputValue)
    {
        editedNickname = inputValue;
        Debug.Log(editedNickname);
    }

    public void OnEditBtnClicked()
    {
        // 토글해주기 펜 > 체크 or 체크 > 펜
        // 펜 모양일때 (인풋필드가 활성화 되지 않았을 때)
        if (penImg.activeSelf && !checkImg.activeSelf)
        {
            nicknameInput.interactable = true;
            originalNickname = nicknameInput.text;
            editBtn.image.color = new Color(0, 255, 0, 100);
            penImg.SetActive(false);
            checkImg.SetActive(true);
        }
        // 체크 모양일 때 (인풋필드가 활성화 되었을 때) => 수정 요청을 보낼 때
        else
        {
            // 닉네임을 바꿨다면, 닉네임이 빈 값이 아니라면
            if (originalNickname != editedNickname && !editedNickname.IsNullOrEmpty())
            {
                // 닉네임 중복검사 시작
                StartCoroutine(NicknameCheck(editedNickname));
                // 코루틴 시작
                StartCoroutine(EditNickname($"{ec2URL}/api/users/update/nickName", editedNickname));
            }
            else
            {
                nicknameInput.text = originalNickname;
            }
            nicknameInput.interactable = false;
            editBtn.image.color = new Color(255, 255, 255);
            checkImg.SetActive(false);
            penImg.SetActive(true);
        }
    }

    public void OnProfileClose()
    {
        // 텍스트 초기화, 인풋필드 
        isNickCanChange.text = null;
        nicknameInput.interactable = false;
    }

    IEnumerator EditNickname(string url, string newNickname)
    {
        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다. 닉네임 변경을 중단합니다.");
            yield break;
        }
        // JSON 데이터 생성
        Nickname nickname = new Nickname { userNickname = newNickname };
        string jsonData = JsonUtility.ToJson(nickname);
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
                if (!canUseNickname)
                {
                    nicknameInput.text = originalNickname;
                    isNickCanChange.text = "<color=red>중복된 닉네임입니다.</color>";
                    yield break;
                }
                // 닉네임 변경 처리
                LobbyManager.instance.SetNick(newNickname);

                PhotonNetwork.NickName = newNickname; // 현재 접속중인 유저의  네트워크 닉네임 변경

                nicknameInput.text = newNickname;
                isNickCanChange.text = "<color=green>닉네임 변경 성공!</color>";
                Debug.Log("닉네임 변경 성공: " + request.downloadHandler.text);
            }
            else
            {
                nicknameInput.text = originalNickname;
                isNickCanChange.text = "<color=red>닉네임 변경 실패</color>";
                Debug.Log("닉네임 변경 실패: " + request.error);
            }
        }
    }

    IEnumerator NicknameCheck(string nickname)
    {
        using (
            UnityWebRequest request = UnityWebRequest.Get(
                $"{ec2URL}/api/users/checkDuplicated/user_nickname/{nickname}"
            )
        )
        {
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("닉네임 중복 X");
                canUseNickname = true;
            }
            else
            {
                if (request.responseCode == 409)
                {
                    Debug.Log("닉네임 중복 O: " + request.error);
                    canUseNickname = false;
                    isNickCanChange.text = "<color=red>중복된 닉네임입니다.</color>";
                }
                else
                {
                    Debug.Log("서버 에러: " + request.error);
                    isNickCanChange.text = "<color=red>닉네임 변경 실패</color>";
                }
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
