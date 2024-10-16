using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class MatchRecord : MonoBehaviour
{
    public GameObject matchItemPrefab; // 매치 기록을 표시할 Prefab
    public Transform contentParent; // 복제된 오브젝트가 들어갈 부모 객체 (스크롤뷰의 Content)
    private string ec2URL = "https://j11d110.p.ssafy.io/";
    private string accessToken; // JWT 토큰을 관리할 변수
    public GameObject noMatchItemPrefab; // 매치 기록이 없을 때 표시할 Prefab
    private int userId; // 유저 아이디

    public Color winColor = Color.blue; // 승리 시 색상 (public으로 설정)
    public Color loseColor = Color.red; // 패배 시 색상 (public으로 설정)

    void Start()
    {
        // PlayerPrefs에서 accessToken과 userId를 가져옴
        accessToken = PlayerPrefs.GetString("accessToken", "");
        userId = PlayerPrefs.GetInt("userId");

        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 로그인이 필요합니다.");
        }
        else
        {
            LoadMatchHistory(); // 매치 기록 초기 로드
        }
    }

    // 공통적으로 JWT 토큰을 요청에 추가하는 함수
    private UnityWebRequest AddAuthorizationHeader(UnityWebRequest request)
    {
        request.SetRequestHeader("Authorization", "Bearer " + accessToken);
        return request;
    }

    // 매치 기록 로드
    private void LoadMatchHistory()
    {
        StartCoroutine(GetMatchHistory());
    }

    // 매치 기록 API 호출
    private IEnumerator GetMatchHistory()
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            Debug.LogError("Access Token이 없습니다. 기록을 불러올 수 없습니다.");
            yield break;
        }

        UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/matches/statistics/user/{userId}");
        AddAuthorizationHeader(webRequest); // 토큰 추가

        yield return webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = webRequest.downloadHandler.text;
            MatchHistory matchHistory = JsonUtility.FromJson<MatchHistory>(jsonResponse);

            // 기존 매치 목록 UI 초기화
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }

            // 매치가 있는지 확인
            if (matchHistory.matches.Count == 0)
            {
                Debug.Log("no");
                // 매치가 없을 때, "매치 기록이 없습니다" 프리팹을 생성
                CreateNoMatchItem();
            }
            else
            {
                // 매치 목록을 UI에 추가
                foreach (var match in matchHistory.matches)
                {
                    CreateMatchItem(match);
                }
            }
        }
        else
        {
            Debug.LogError($"매치 기록 불러오기 실패: {webRequest.error} - {webRequest.downloadHandler.text}");
        }
    }
    // 매치 기록이 없을 때 표시할 프리팹 생성
    private void CreateNoMatchItem()
    {
        // 프리팹을 복제하여 새로운 "매치 기록 없음" 아이템 생성
        GameObject newNoMatchItem = Instantiate(noMatchItemPrefab, contentParent);
        TMP_Text noMatchText = newNoMatchItem.GetComponentInChildren<TMP_Text>();
        noMatchText.text = "매치 기록이 없습니다.";
        Debug.Log("no123");
    }

    // 매치 기록을 표시할 UI 요소를 생성
    private void CreateMatchItem(Match match)
    {
        // 프리팹을 복제하여 새로운 매치 아이템 생성
        GameObject newMatchItem = Instantiate(matchItemPrefab, contentParent);

        // 매치 결과에 따라 "승리" 또는 "패배" 텍스트 추가
        TMP_Text resultText = newMatchItem.transform.Find("ResultText").GetComponent<TMP_Text>();
        if (match.match_rank == 1)
        {
            resultText.text = "승리";
        }
        else
        {
            resultText.text = "패배";
        }

        // 매치 정보 표시
        newMatchItem.transform.Find("CharacterIdText").GetComponent<TMP_Text>().text = $"캐릭터: {match.character_id}";
        newMatchItem.transform.Find("MatchRankText").GetComponent<TMP_Text>().text = $"순위: {match.match_rank}";
        newMatchItem.transform.Find("DpsText").GetComponent<TMP_Text>().text = $"딜량: {match.match_dps}";
        newMatchItem.transform.Find("KillsText").GetComponent<TMP_Text>().text = $"킬수: {match.match_kills}";
        newMatchItem.transform.Find("PlaytimeText").GetComponent<TMP_Text>().text = $"경기시간: {match.match_playtime}";

        // 이미지 색상 변경 (public 변수로 색상 조절)
        Image matchImage = newMatchItem.GetComponent<Image>();
        if (match.match_rank == 1)
        {
            matchImage.color = winColor; // 승리 시 색상
        }
        else
        {
            matchImage.color = loseColor; // 패배 시 색상
        }
    }

    // JSON 배열 파싱을 위한 클래스
    [System.Serializable]
    public class MatchHistory
    {
        public int userId;
        public List<Match> matches;
    }

    [System.Serializable]
    public class Match
    {
        public int userId;
        public int character_id;
        public int match_id;
        public int match_rank;
        public int match_dps;
        public int match_kills;
        public string match_playtime;
    }
}
