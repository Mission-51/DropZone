using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI; // Image와 Button을 사용하기 위해 필요
using TMPro;

public class GameOff : MonoBehaviour, IPointerClickHandler
{
    public Image clickImage; // 클릭할 Image 오브젝트
    public Logoutmanager logoutManager; // 로그아웃 매니저 참조

    // 모달 창 관련 변수
    public GameObject exitConfirmationModal; // 모달 창 오브젝트
    public Button confirmExitButton; // "예" 버튼
    public Button cancelExitButton; // "아니오" 버튼
    public TMP_Text exitConfirmationText; // "게임을 종료하시겠습니까?" 텍스트

    private void Start()
    {
        // clickImage가 설정되지 않았다면 현재 게임 오브젝트의 Image를 클릭 오브젝트로 설정
        if (clickImage == null)
        {
            clickImage = GetComponent<Image>();
        }

        // 로그아웃 매니저가 설정되지 않았을 경우 현재 씬에서 찾음
        if (logoutManager == null)
        {
            logoutManager = Logoutmanager.Instance;
        }

        // 모달 창의 버튼 리스너 설정
        confirmExitButton.onClick.AddListener(OnConfirmExit); // "예" 버튼
        cancelExitButton.onClick.AddListener(OnCancelExit); // "아니오" 버튼

        // 모달 창을 비활성화 상태로 설정
        exitConfirmationModal.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("이미지 클릭 이벤트 발생");

        // 클릭된 오브젝트가 설정된 clickImage인지 확인
        if (eventData.pointerPress == clickImage.gameObject)
        {
            Debug.Log("clickImage와 클릭된 오브젝트가 일치함. 모달 창 띄움");
            ShowExitConfirmation(); // 모달 창 표시
        }
        else
        {
            Debug.Log("clickImage와 클릭된 오브젝트가 일치하지 않음");
        }
    }

    // 모달 창을 띄우는 함수
    private void ShowExitConfirmation()
    {
        exitConfirmationText.text = "게임을 종료하시겠습니까?"; // 모달 창의 텍스트 설정
        exitConfirmationModal.SetActive(true); // 모달 창 활성화
    }

    // "예" 버튼을 눌렀을 때 호출되는 함수
    private void OnConfirmExit()
    {
        Debug.Log("게임 종료 확인됨. 로그아웃 후 종료 처리");
        StartCoroutine(LogoutAndExitGame()); // 로그아웃 후 게임 종료
        exitConfirmationModal.SetActive(false); // 모달 창 비활성화
    }

    // "아니오" 버튼을 눌렀을 때 호출되는 함수
    private void OnCancelExit()
    {
        Debug.Log("게임 종료 취소됨");
        exitConfirmationModal.SetActive(false); // 모달 창 비활성화
    }

    // 로그아웃 후 게임 종료를 처리하는 코루틴
    private IEnumerator LogoutAndExitGame()
    {
        // 로그아웃 매니저가 존재하면 로그아웃 처리
        if (logoutManager != null)
        {
            yield return logoutManager.LogoutCoroutine(); // 로그아웃 처리
        }
        else
        {
            Debug.LogWarning("로그아웃 매니저가 없습니다. 로그아웃 없이 게임을 종료합니다.");
        }

        // 로그아웃이 완료되거나 로그아웃 매니저가 없으면 게임 종료
        ExitGame();
    }

    // 게임 종료 기능
    public void ExitGame()
    {
        Debug.Log("ExitGame 호출됨");

        // 에디터에서 실행 중일 경우 에디터를 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 애플리케이션 종료
        Application.Quit();
#endif
    }
}
