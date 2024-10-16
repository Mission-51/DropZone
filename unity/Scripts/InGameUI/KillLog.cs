using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class KillLog : MonoBehaviour
{
    public GameObject killLogPrefab;  // 로그 아이템 Prefab
    public Transform logParent;       // 로그를 표시할 부모 객체 (Vertical Layout Group이 붙은 Panel)
    public float logDisplayTime = 5f; // 로그가 표시될 시간

    // 킬이 발생했을 때 호출되는 함수
    public void AddKillLog(string killedPlayer)
    {
        // 새 로그 아이템 생성
        GameObject newLog = Instantiate(killLogPrefab, logParent);

        // 텍스트 설정
        TextMeshProUGUI logText = newLog.GetComponentInChildren<TextMeshProUGUI>();
        logText.text = killedPlayer;

        // 로그가 일정 시간 후에 사라지도록 코루틴 실행
        StartCoroutine(RemoveLogAfterDelay(newLog, logDisplayTime));
    }

    // 일정 시간이 지나면 로그 삭제
    IEnumerator RemoveLogAfterDelay(GameObject logItem, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(logItem);  // 로그 삭제
    }
}
