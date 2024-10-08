using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public GameObject rankingItemPrefab; // 랭킹 정보를 표시할 Prefab
    public Transform contentParent; // 복제된 오브젝트가 들어갈 부모 객체 (스크롤뷰의 Content)
    public Button prevPageButton; // 이전 페이지 버튼
    public Button nextPageButton; // 다음 페이지 버튼
    public TMP_Text pageText; // 현재 페이지 번호를 표시할 텍스트
    private string ec2URL = "https://j11d110.p.ssafy.io/";
    private int currentPage = 1; // 첫 페이지는 1로 설정
    private const int rankingsPerPage = 10; // 한 페이지에 표시할 랭킹 개수
    private List<RankingResponse> allRankings; // 모든 랭킹 정보를 저장할 리스트

    // 1, 2, 3등의 글자 크기를 설정
    public float firstPlaceFontSize = 40f;  // 1등 글자 크기
    public float secondPlaceFontSize = 35f; // 2등 글자 크기
    public float thirdPlaceFontSize = 30f;  // 3등 글자 크기
    public float defaultFontSize = 25f;     // 나머지 등수의 기본 글자 크기

    void Start()
    {
        LoadRankings(currentPage); // 첫 페이지 로드

        // 페이지 버튼 이벤트 등록
        prevPageButton.onClick.AddListener(OnPrevPage);
        nextPageButton.onClick.AddListener(OnNextPage);

        LoadRankings(currentPage); // 첫 페이지 로드
    }

    // 랭킹 데이터를 불러오는 함수 (API 호출)
    private void LoadRankings(int page)
    {
        StartCoroutine(GetRankings(page));
    }

    private IEnumerator GetRankings(int page)
    {
        // 페이지 번호를 API에 전달하여 데이터 요청
        UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/rankings?page={page}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            RankingResponse[] rankings = JsonHelper.FromJson<RankingResponse>(jsonResponse);

            // 현재 페이지의 랭킹 기록을 리스트에 저장
            allRankings = new List<RankingResponse>(rankings);

            Debug.Log($"서버 응답 (페이지 {page}): {jsonResponse}");
            Debug.Log($"받아온 랭킹 수: {allRankings.Count}");

            // 데이터 표시
            DisplayPage();
        }
        else
        {
            Debug.LogError($"랭킹 정보 가져오기 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
        }
    }

    // 페이지 변경 시 호출되는 함수
    private void DisplayPage()
    {
        Debug.Log($"페이지 {currentPage}의 랭킹 데이터 표시");

        // 기존 랭킹 목록 UI 초기화
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 받아온 랭킹 목록을 UI에 추가
        for (int i = 0; i < allRankings.Count; i++)
        {
            StartCoroutine(CreateRankingItem(allRankings[i]));
        }

        // 페이지 번호 업데이트
        pageText.text = $"{currentPage}";

        // 이전/다음 페이지 버튼 활성화/비활성화
        prevPageButton.interactable = currentPage > 1;  // 1보다 크면 이전 페이지 활성화
    }

    // 유저 ID로 닉네임을 가져오는 함수
    private IEnumerator GetUserNickname(int userId, System.Action<string> callback)
    {
        UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/users/search/user_id/{userId}");

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            UserResponse userResponse = JsonUtility.FromJson<UserResponse>(jsonResponse);
            callback(userResponse.userNickname);
        }
        else
        {
            Debug.LogError($"유저 닉네임 가져오기 실패: {webRequest.error}");
            callback($"유저 {userId}"); // 실패 시 유저ID 표시
        }
    }

    // 랭킹 정보를 표시할 UI 요소를 생성
    private IEnumerator CreateRankingItem(RankingResponse ranking)
    {
        // 프리팹을 복제하여 새로운 랭킹 아이템 생성
        GameObject newRankingItem = Instantiate(rankingItemPrefab, contentParent);

        // 각 랭킹 정보 표시
        TMP_Text rankText = newRankingItem.transform.Find("RankText").GetComponent<TMP_Text>();
        TMP_Text nicknameText = newRankingItem.transform.Find("NicknameText").GetComponent<TMP_Text>();
        TMP_Text pointsText = newRankingItem.transform.Find("PointsText").GetComponent<TMP_Text>();
        TMP_Text winsText = newRankingItem.transform.Find("WinsText").GetComponent<TMP_Text>();

        rankText.text = $"{ranking.rank} 등";
        pointsText.text = $"포인트: {ranking.rankingPoints}";
        winsText.text = $"승리수: {ranking.totalWins}";

        // 유저ID를 이용해 닉네임을 가져와 표시
        yield return StartCoroutine(GetUserNickname(ranking.userId, nickname => {
            nicknameText.text = $"{nickname}";
        }));

        // 랭킹에 따라 글자 크기 설정
        if (ranking.rank == 1)
        {
            rankText.fontSize = firstPlaceFontSize;
            nicknameText.fontSize = firstPlaceFontSize;
        }
        else if (ranking.rank == 2)
        {
            rankText.fontSize = secondPlaceFontSize;
            nicknameText.fontSize = secondPlaceFontSize;
        }
        else if (ranking.rank == 3)
        {
            rankText.fontSize = thirdPlaceFontSize;
            nicknameText.fontSize = thirdPlaceFontSize;
        }
        else
        {
            rankText.fontSize = defaultFontSize;
            nicknameText.fontSize = defaultFontSize;
        }
    }

    // 이전 페이지 버튼 클릭 시 호출되는 함수
    public void OnPrevPage()
    {

        if (currentPage > 1)  // 페이지가 1보다 작아지지 않음
        {
            currentPage--;
            Debug.Log($"이전 페이지로 이동: {currentPage}");
            LoadRankings(currentPage);  // 이전 페이지 데이터 요청
        }

    }

    // 다음 페이지 버튼 클릭 시 호출되는 함수
    public void OnNextPage()
    {
        currentPage++;  // 페이지 증가
        Debug.Log($"다음 페이지로 이동: {currentPage}");
        LoadRankings(currentPage);  // 다음 페이지 데이터 요청
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

    // 랭킹 응답 클래스
    [System.Serializable]
    public class RankingResponse
    {
        public int rank;
        public int userId; // 유저 ID 추가
        public int rankingPoints;
        public int totalWins;
    }

    // 유저 정보 응답 클래스
    [System.Serializable]
    public class UserResponse
    {
        public string userNickname;
    }
}
