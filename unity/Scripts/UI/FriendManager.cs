using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class FriendManager : MonoBehaviour
{
    public TMP_InputField nicknameInput; // 친구 추가를 위한 닉네임 입력 필드
    public GameObject friendItemPrefab; // 친구 정보를 표시할 Prefab
    public GameObject friendRequestPrefab; // 친구 요청 알림 Prefab (수락/거절 버튼이 있는 UI)
    public Transform contentParent; // 복제된 오브젝트가 들어갈 부모 객체 (스크롤뷰의 Content)
    public Button refreshButton; // 새로고침 버튼
    public TMP_Text feedbackMessage; // 친구 신청 결과를 표시할 TextMeshPro 텍스트
    private List<GameObject> friendObjects = new List<GameObject>(); // 생성된 친구 오브젝트 리스트
    private List<int> existingRequests = new List<int>();

    private string ec2URL = "https://j11d110.p.ssafy.io/";
    private string accessToken; // JWT 토큰을 관리할 변수

    // 모달 창 관련 변수
    public GameObject deleteConfirmationModal; // 모달 창
    public TMP_Text deleteConfirmationText; // 모달 창의 텍스트
    public Button confirmDeleteButton; // "예" 버튼
    public Button cancelDeleteButton; // "아니오" 버튼

    private GameObject friendItemToDelete; // 삭제할 친구 오브젝트
    private int friendShipIdToDelete; // 삭제할 친구 ID


    void Start()
    {
        // PlayerPrefs에서 accessToken을 가져옴
         accessToken = PlayerPrefs.GetString("accessToken", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 로그인이 필요합니다.");
        }
        else
        {
            LoadFriendList(); // 친구 목록 초기 로드
            LoadReceivedFriendRequests(); // 받은 친구 요청 목록 로드
        }
        
        // 새로고침 버튼 클릭 이벤트 추가
        refreshButton.onClick.AddListener(OnRefreshButtonClick);

        // 모달 창 버튼 리스너 설정
        confirmDeleteButton.onClick.AddListener(OnConfirmDelete); // "예" 버튼 리스너 추가
        cancelDeleteButton.onClick.AddListener(OnCancelDelete); // "아니오" 버튼 리스너 추가
    }


    // 공통적으로 JWT 토큰을 요청에 추가하는 함수
    private UnityWebRequest AddAuthorizationHeader(UnityWebRequest request)
    {
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        return request;
    }

    // 친구 삭제 시 모달 창을 띄우는 함수
    public void ShowDeleteConfirmation(int friendShipId, GameObject friendItem, string nickname)
    {
        friendShipIdToDelete = friendShipId; // 삭제할 친구의 ID 저장
        friendItemToDelete = friendItem; // 삭제할 친구 오브젝트 저장
        deleteConfirmationText.text = $"{nickname}님을 삭제하시겠습니까?"; // 삭제 확인 메시지 설정
        deleteConfirmationModal.SetActive(true); // 모달 창 활성화
    }

    // "예" 버튼을 누르면 삭제를 진행하는 함수
    public void OnConfirmDelete()
    {
        StartCoroutine(DeleteFriend(friendShipIdToDelete, friendItemToDelete)); // 삭제 코루틴 실행
        deleteConfirmationModal.SetActive(false); // 모달 창 비활성화
    }

    // "아니오" 버튼을 누르면 삭제를 취소하는 함수
    public void OnCancelDelete()
    {
        deleteConfirmationModal.SetActive(false); // 모달 창 비활성화
    }
    // 친구 추가 요청
    public void AddFriend()
    {
        string nickname = nicknameInput.text;
        if (!string.IsNullOrEmpty(nickname))
        {
            StartCoroutine(AddFriendRequest(nickname));
        }
    }

    // 친구 추가 요청 API 호출
    private IEnumerator AddFriendRequest(string nickname)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 추가 요청을 할 수 없습니다.");
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Post($"{ec2URL}api/users/friends/{nickname}", "");
        AddAuthorizationHeader(webRequest); // 토큰 추가

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("친구 추가 요청 성공");
            ShowFeedbackMessage("친구 신청을 보냈습니다.", Color.green); // 성공 메시지 표시
        }
        else
        {
            Debug.LogError($"친구 추가 요청 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
            ShowFeedbackMessage("친구 신청에 실패했습니다.", Color.red); // 실패 메시지 표시
        }
    }

    // 피드백 메시지를 화면에 표시하는 함수
    private void ShowFeedbackMessage(string message, Color color)
    {
        feedbackMessage.text = message;
        feedbackMessage.color = color;
        feedbackMessage.gameObject.SetActive(true); // 메시지 표시
        StartCoroutine(HideFeedbackMessage()); // 일정 시간 후 메시지 숨김
    }

    // 일정 시간 후 피드백 메시지를 숨기는 코루틴
    private IEnumerator HideFeedbackMessage()
    {
        yield return new WaitForSeconds(3f); // 3초 후 메시지 숨김
        feedbackMessage.gameObject.SetActive(false);
    }

    // 받은 친구 요청 목록을 로드하여 알림을 표시
    // 받은 친구 요청 목록을 로드하여 알림을 표시
    private void LoadReceivedFriendRequests()
    {
        StartCoroutine(GetReceivedFriendRequests());
    }

    private IEnumerator GetReceivedFriendRequests()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 친구 요청을 불러올 수 없습니다.");
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/users/friends/received");
        AddAuthorizationHeader(webRequest);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log($"친구 요청 응답: {jsonResponse}");  // JSON 응답을 출력하여 구조 확인
            FriendRequest[] requests = JsonHelper.FromJson<FriendRequest>(jsonResponse);

            foreach (var request in requests)
            {
                // 이미 같은 friendShipId가 있는지 확인
                if (!existingRequests.Contains(request.friendShipId))
                {
                    // 같은 요청이 없으면 새로운 요청 프리팹 생성
                    CreateFriendRequestNotification(request);
                    existingRequests.Add(request.friendShipId);  // 요청 리스트에 추가
                }
            }
        }
        else
        {
            Debug.LogError($"친구 요청 불러오기 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
        }
    }


    // 수락 및 거절 버튼이 있는 알림 UI 생성 (친구 신청 닉네임 표시)
    private void CreateFriendRequestNotification(FriendRequest request)
    {
        // 요청을 UI에 표시하고 수락/거절 버튼을 추가
        GameObject newRequestItem = Instantiate(friendRequestPrefab, contentParent);
        TMP_Text requestText = newRequestItem.GetComponentInChildren<TMP_Text>();

        // 친구 신청한 사람의 닉네임 표시
        requestText.text = $"{request.friendNickName}님이 친구 요청을 하였습니다.";

        // 수락 버튼 설정
        Button acceptButton = newRequestItem.transform.Find("AcceptButton").GetComponent<Button>();
        acceptButton.onClick.AddListener(() => StartCoroutine(HandleFriendRequest(request.friendShipId, true, newRequestItem)));

        // 거절 버튼 설정
        Button refuseButton = newRequestItem.transform.Find("RefuseButton").GetComponent<Button>();
        refuseButton.onClick.AddListener(() => StartCoroutine(HandleFriendRequest(request.friendShipId, false, newRequestItem)));
    }

    // 친구 요청 수락/거절 처리
    // 수락/거절 후 요청을 삭제
    private IEnumerator HandleFriendRequest(int friendShipId, bool isAccepted, GameObject requestItem)
    {
        string apiEndpoint = isAccepted
            ? $"{ec2URL}api/users/friends/approve/{friendShipId}"
            : $"{ec2URL}api/users/friends/refuse/{friendShipId}";

        UnityWebRequest webRequest = UnityWebRequest.Post(apiEndpoint, "");
        AddAuthorizationHeader(webRequest);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(isAccepted ? "친구 요청 수락" : "친구 요청 거절");

            // 수락/거절 후 해당 요청 UI 제거
            Destroy(requestItem);
            existingRequests.Remove(friendShipId); // 리스트에서도 해당 요청 제거

            if (isAccepted)
            {
                // 수락한 경우 친구 목록을 업데이트
                LoadFriendList();
            }
        }
        else
        {
            Debug.LogError($"{(isAccepted ? "친구 요청 수락 실패" : "친구 요청 거절 실패")}: {webRequest.error}");
        }
    }

    // 친구 목록 로드
    private void LoadFriendList()
    {
        StartCoroutine(GetFriendList());
    }

    // 친구 목록 API 호출
    private IEnumerator GetFriendList()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 목록을 불러올 수 없습니다.");
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/users/friends/list");
        AddAuthorizationHeader(webRequest);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            Friend[] friends = JsonHelper.FromJson<Friend>(jsonResponse);
            

            // 기존 친구 목록 UI 초기화
            foreach (var friendObject in friendObjects)
            {
                Destroy(friendObject);
            }
            friendObjects.Clear();

            // 친구 목록을 UI에 추가
            foreach (var friend in friends)
            {
                Debug.Log("friends");
                Debug.Log(friend);
                CreateFriendItem(friend.friendShipId, friend.friendNickName); // 친구 삭제 버튼 포함하여 닉네임 표시
            }
        }
        else
        {
            Debug.LogError($"친구 목록 불러오기 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
        }
    }

    // 친구 정보를 표시할 UI 요소를 생성하고 삭제 버튼을 추가
    private void CreateFriendItem(int friendShipId, string nickname)
    {
        // 프리팹을 복제하여 새로운 친구 아이템 생성
        GameObject newFriendItem = Instantiate(friendItemPrefab, contentParent);
        newFriendItem.GetComponentInChildren<TMP_Text>().text = nickname; // 친구 닉네임 표시

        // 친구 삭제 버튼 설정
        // 친구 삭제 버튼 설정
        Button deleteButton = newFriendItem.transform.Find("DeleteButton").GetComponent<Button>();
        deleteButton.onClick.AddListener(() => ShowDeleteConfirmation(friendShipId, newFriendItem, nickname)); // 모달 창 띄우기

        // 생성된 친구 오브젝트를 리스트에 추가 (관리할 수 있도록)
        friendObjects.Add(newFriendItem);
    }

    // 친구 삭제 처리
    private IEnumerator DeleteFriend(int friendShipId, GameObject friendItem)
    {
        UnityWebRequest webRequest = UnityWebRequest.Delete($"{ec2URL}api/users/friends/delete/{friendShipId}");
        AddAuthorizationHeader(webRequest);

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("친구 삭제 성공");
            Destroy(friendItem); // 삭제된 친구 UI 제거
        }
        else
        {
            Debug.LogError($"친구 삭제 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
        }
    }

    // 새로고침 버튼 클릭 시 호출되는 함수
    private void OnRefreshButtonClick()
    {
        LoadFriendList(); // 친구 목록 새로고침
        LoadReceivedFriendRequests(); // 받은 친구 요청 새로고침
    }

    // JSON 배열 파싱을 위한 JsonHelper
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }

    // 친구 요청 및 정보 클래스
    [System.Serializable]
    public class FriendRequest
    {
        public int friendShipId;
        public string friendEmail;
        public string friendNickName;
        public string status;
    }

    [System.Serializable]
    public class Friend
    {
        public int friendShipId;
        public string friendEmail;
        public string friendNickName;
    }
}
