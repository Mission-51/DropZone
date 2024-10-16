using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;  // Pointer 이벤트를 처리하기 위해 필요
using TMPro;

public class ProfileManager : MonoBehaviour, IPointerClickHandler
{
    [System.Serializable]
    public class MatchStatistics
    {
        public int userId;
        public Match[] matches;
    }

    [System.Serializable]
    public class Match
    {
        public int userId;
        public int character_id;
        public int match_id;
        public int match_rank;
       
        public int match_kills;
        public string match_playtime;  // 플레이타임은 문자열로 처리
    }

    [System.Serializable]
    public class RankingResponse
    {
        public int userId;
        public int rankingPoints;
        public int totalWins;
        public int rank;
    }

    public TMP_Text totalKillsText;
    public TMP_Text totalPlaytimeText;
    public TMP_Text totalGamesText;
    
    public TMP_Text totalWinsText;
    public TMP_Text rankingPointsText;  // 랭킹 포인트를 표시할 텍스트
    public TMP_Text rankText;  // 랭킹을 표시할 텍스트

    private string ec2URL = "https://j11d110.p.ssafy.io/";

    // IPointerClickHandler의 OnPointerClick 함수 구현
    public void OnPointerClick(PointerEventData eventData)
    {
        // 버튼 클릭 시 실행될 코드
        StartCoroutine(GetMatchStatistics());
    }

    private IEnumerator GetMatchStatistics()
    {
        // PlayerPrefs에서 accessToken과 userId 가져오기
        string accessToken = PlayerPrefs.GetString("accessToken", "");
        int userId = PlayerPrefs.GetInt("userId", 0); // 기본값 0

        Debug.Log("User ID: " + userId);

        // 매치 통계 API 요청
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/matches/statistics/user/{userId}"))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer " + accessToken); // JWT 토큰 추가
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 받은 응답을 로그로 출력
                string jsonResponse = webRequest.downloadHandler.text;

                // JSON 응답 파싱
                MatchStatistics statistics = JsonUtility.FromJson<MatchStatistics>(jsonResponse);

                // 통계 계산
                int totalKills = 0;
                
                int totalGames = statistics.matches.Length;
                int totalWins = 0;
                int totalPlaytimeInSeconds = 0;

                foreach (Match match in statistics.matches)
                {
                    totalKills += match.match_kills;
                    

                    // 승리 조건: match_rank >= 3
                    if (match.match_rank >= 3)
                    {
                        totalWins++;
                    }

                    // 플레이타임 계산 (00:05:29 형식의 문자열을 초로 변환)
                    string[] timeParts = match.match_playtime.Split(':');
                    int minutes = int.Parse(timeParts[1]);
                    int seconds = int.Parse(timeParts[2]);
                    totalPlaytimeInSeconds += (minutes * 60) + seconds;
                }

                // 플레이타임을 시:분:초 형식으로 변환
                string totalPlaytime = string.Format("{0:D2}:{1:D2}:{2:D2}",
                    totalPlaytimeInSeconds / 3600,
                    (totalPlaytimeInSeconds % 3600) / 60,
                    totalPlaytimeInSeconds % 60);

                // UI에 계산 결과 표시
                totalKillsText.text = "총 킬수" + "\n" + totalKills;
                totalPlaytimeText.text = "총 플레이타임" + "\n" + totalPlaytime;
                totalGamesText.text = "총 게임 수" + "\n" + totalGames;
                
                totalWinsText.text = "승리한 횟수" + "\n" + totalWins;

                // 랭킹 포인트 및 랭킹 가져오기 추가
                StartCoroutine(GetRanking(userId));  // 매치 통계를 가져온 후 랭킹 정보를 가져옴
            }
            else
            {
                Debug.LogError("매치 통계 가져오기 실패: " + webRequest.error);
            }
        }
    }

    // 유저 랭킹 정보를 가져오는 함수
    private IEnumerator GetRanking(int userId)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{ec2URL}api/rankings/{userId}"))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                // 서버로부터 받은 응답을 로그로 출력
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("랭킹 정보: " + jsonResponse);

                // JSON 응답 파싱
                RankingResponse ranking = JsonUtility.FromJson<RankingResponse>(jsonResponse);

                // UI에 랭킹 정보 표시
                rankingPointsText.text = "랭킹 포인트 " + "\n" + ranking.rankingPoints;
                rankText.text = "랭킹" + "\n" + ranking.rank;
            }
            else
            {
                Debug.LogError("랭킹 정보 가져오기 실패: " + webRequest.error);
            }
        }
    }
}
