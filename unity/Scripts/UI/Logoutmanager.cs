using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class Logoutmanager : MonoBehaviourPun, IPointerClickHandler
{
    public static Logoutmanager Instance; // 싱글톤 인스턴스
    public Image logoutImage; // 로그아웃 버튼 역할을 할 Image
    private bool isQuitting = false; // 강제 종료 감지 여부
    private string ec2URL = "https://j11d110.p.ssafy.io/"; // 서버 URL
    private bool LogoutSuccessful = false; // 로그아웃 성공 여부

    // 모달 창 관련 변수
    public GameObject logoutConfirmationModal; // 모달 창 오브젝트
    public Button confirmLogoutButton; // "예" 버튼
    public Button cancelLogoutButton; // "아니오" 버튼
    public TMP_Text logoutConfirmationText; // "로그아웃을 하시겠습니까?" 텍스트

    private void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject); // 중복된 인스턴스가 있으면 파괴
            return; // 중복된 인스턴스가 있으면 아래 로직 실행 방지
        }

        // 강제 종료 이벤트 등록 (Awake에서 확실히 등록되도록 함)
        Application.quitting += OnApplicationQuit;
    }

    private void Start()
    {
        // logoutImage가 설정되지 않았다면 현재 오브젝트에서 Image 컴포넌트를 찾음
        if (logoutImage == null)
        {
            logoutImage = GetComponent<Image>();
        }

        // 모달 창의 버튼 리스너 설정
        confirmLogoutButton.onClick.AddListener(OnConfirmLogout); // "예" 버튼 리스너 추가
        cancelLogoutButton.onClick.AddListener(OnCancelLogout); // "아니오" 버튼 리스너 추가
    }

    private void OnEnable()
    {
        // 강제 종료 이벤트가 씬 전환 시에도 계속 등록될 수 있도록 보장
        Application.quitting -= OnApplicationQuit;
        Application.quitting += OnApplicationQuit;
    }

    private void OnDisable()
    {
        Application.quitting -= OnApplicationQuit; // 강제 종료 이벤트 해제
    }

    // IPointerClickHandler 인터페이스 구현 - 이미지 클릭 시 로그아웃 처리
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPress == logoutImage.gameObject)
        {
            Debug.Log("로그아웃 이미지 클릭됨. 로그아웃 처리 시작.");
            ShowLogoutConfirmation(); // 모달 창 표시
        }
    }
    // 모달 창을 띄우는 함수
    private void ShowLogoutConfirmation()
    {
        logoutConfirmationText.text = "로그아웃을 하시겠습니까?"; // 모달 창의 텍스트 설정
        logoutConfirmationModal.SetActive(true); // 모달 창 활성화
    }

    // "예" 버튼을 눌렀을 때 호출되는 함수
    private void OnConfirmLogout()
    {
        Debug.Log("로그아웃 확인됨. 로그아웃 처리 시작");
        StartCoroutine(LogoutAndCheckSuccess()); // 로그아웃 처리 후 성공 여부 확인
        logoutConfirmationModal.SetActive(false); // 모달 창 비활성화
    }

    // "아니오" 버튼을 눌렀을 때 호출되는 함수
    private void OnCancelLogout()
    {
        Debug.Log("로그아웃 취소됨");
        logoutConfirmationModal.SetActive(false); // 모달 창 비활성화
    }

    // 로그아웃 처리 후 성공 여부에 따라 씬 전환
    private IEnumerator LogoutAndCheckSuccess()
    {
        yield return StartCoroutine(LogoutCoroutine());

        // 로그아웃 성공 여부를 확인 후, 성공 시 로그인 씬으로 이동
        if (LogoutSuccessful)
        {
            Debug.Log("로그아웃 성공, 로그인 씬으로 이동");
            PhotonNetwork.Disconnect(); // 로그아웃시 마스터서버 연결 끊기
            SceneManager.LoadScene("LoginScene"); 

        }
        else
        {
            Debug.LogError("로그아웃 실패, 로그인 씬으로 이동하지 않음");
        }
    }

    // 강제 종료 또는 게임 종료 시 로그아웃 처리
    private void OnApplicationQuit()
    {
        Debug.Log("강제 종료 또는 게임 종료. 로그아웃 처리 시작.");
        isQuitting = true;
        StartCoroutine(LogoutCoroutine());
    }

    // 로그아웃 처리 코루틴
    public IEnumerator LogoutCoroutine()
    {
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        bool logoutSuccess = false; // 로그아웃 성공 여부를 저장

        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("로그인되지 않은 상태입니다.");
            yield break;
        }

        using (UnityWebRequest webRequest = new UnityWebRequest($"{ec2URL}api/auth/logout", "POST"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken);
            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes($"{{\"accessToken\": \"{accessToken}\"}}"));
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("로그아웃 성공");
                PlayerPrefs.DeleteKey("accessToken");
                PlayerPrefs.DeleteKey("refreshToken");
                PlayerPrefs.DeleteKey("userId");
                PlayerPrefs.DeleteKey("nickname"); // 닉네임도 삭제
                PlayerPrefs.Save();
                logoutSuccess = true; // 로그아웃 성공 표시
            }
            else
            {
                Debug.LogError("로그아웃 실패: " + webRequest.error);
                logoutSuccess = false; // 로그아웃 실패 표시
            }
        }

        // 강제 종료 중에는 로그인 씬으로 이동하지 않음
        if (!isQuitting)
        {
            LogoutSuccessful = logoutSuccess;
        }
    }
}
