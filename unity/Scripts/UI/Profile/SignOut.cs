using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using WebSocketSharp;

public class SignOut : MonoBehaviour
{
    // 회원탈퇴 확인창
    public GameObject signOutConfirm;

    // 서버 URL
    private string ec2URL = "https://j11d110.p.ssafy.io";

    // JWT 토큰
    private string accessToken;

    private void Start()
    {
        accessToken = PlayerPrefs.GetString("accessToken", "");

        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다.");
        }
    }

    public void OnSignOutBtnClick()
    {
        signOutConfirm.SetActive(true);
    }

    public void OnYesBtnClick()
    {
        StartCoroutine(SignOutCoroutine());
    }

    public void OnNoBtnClick()
    {
        signOutConfirm.SetActive(false);
    }

    IEnumerator SignOutCoroutine()
    {
        if (accessToken.IsNullOrEmpty())
        {
            Debug.LogError("토큰이 없습니다. 닉네임 변경을 중단합니다.");
            yield break;
        }

        string url = $"{ec2URL}/api/users/delete";

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            // Delete는 기본적으로 DownloadHandler가 첨부되어 있지 않다, 설정 필요
            request.downloadHandler = new DownloadHandlerBuffer();

            // 토큰 추가
            AddAuthorizationHeader(request);

            // 요청 전송
            yield return request.SendWebRequest();

            // 응답 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("회원탈퇴 완료: " + request.downloadHandler.text);
                SceneManager.LoadScene("LoginScene");
            }
            else
            {
                Debug.Log("회원탈퇴 실패: " + request.error);
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
