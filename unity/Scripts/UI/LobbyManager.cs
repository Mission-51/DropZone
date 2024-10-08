using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;
using WebSocketSharp;

public class LobbyManager : MonoBehaviour
{
    public TMP_Text nicknameText;
    public TMP_Text nicknameText2;
    public TMP_Text CurrentUser;

    private static string userNick;
    private string ec2URL = "https://j11d110.p.ssafy.io/";

    // ChatClient ������Ʈ�� ��������
    public ChatClient chatClient;

    private static LobbyManager m_instance;
    public delegate void NicknameReadyDelegate(string nickname);
    public static event NicknameReadyDelegate OnNicknameReady; // 이벤트 선언

    // 닉네임 변경 여부 확인해서 즉시 닉네임 변경하도록 하는 플래그
    private bool isNicknameChanged = false;

    public static LobbyManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<LobbyManager>();
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    [System.Serializable]
    public class UserProfileResponse
    {
        public string userNickname; // ������ userNickname �ʵ忡 ����
    }

    public void SetNick(string newNickname)
    {
        if (!newNickname.IsNullOrEmpty())
        {
            nicknameText.text = newNickname;
            nicknameText2.text = newNickname;
            userNick = newNickname;
        }
    }

    public string GetNick()
    {
        return userNick;
    }

    private void Start()
    {
        StartCoroutine(GetUserProfile());
    }

    private IEnumerator GetUserProfile()
    {
        // PlayerPrefs에서 userId 가져오기
        int userId = PlayerPrefs.GetInt("userId", 0); // 기본값 0

        Debug.Log("User ID: " + userId); // user_id 출력 확인

        using (
            UnityWebRequest webRequest = UnityWebRequest.Get(
                $"{ec2URL}api/users/search/user_id/{userId}"
            )
        )
        {
            // 서버로 요청을 보내기 위해 SendWebRequest() 호출
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("서버 응답: " + jsonResponse);

                // JSON 응답을 파싱하여 유저 프로필 정보 가져오기
                UserProfileResponse profile = JsonUtility.FromJson<UserProfileResponse>(
                    jsonResponse
                );

                // UI 텍스트 업데이트
                nicknameText.text = profile.userNickname;
                nicknameText2.text = profile.userNickname;
                userNick = profile.userNickname;

                Debug.Log("유저 닉네임 확인: " + userNick);
                if (OnNicknameReady != null)
                {
                    OnNicknameReady(userNick);
                }

                // 네트워크 연결 설정 (필요한 경우)
                NetworkManager.instance.ServerConnect();
            }
            else
            {
                // 오류 처리
                Debug.LogError("프로필 정보를 가져오는 데 실패했습니다: " + webRequest.error);
                Debug.LogError("HTTP 응답 코드: " + webRequest.responseCode);
                Debug.LogError("서버 응답 메시지: " + webRequest.downloadHandler.text);
            }
        }
    }

    public void SetCurrentUser(string UserCheck)
    {
        this.CurrentUser.text = UserCheck;
    }
}
