using UnityEngine;
using System.Collections.Generic;

public class GameObjectManager : MonoBehaviour
{
    // 마우스 오버 효과를 적용할 오브젝트들의 리스트
    public List<GameObject> objectsToScale = new List<GameObject>();

    // 기본 크기 배율 설정 (1은 원래 크기, 1.2는 20% 증가)
    public float hoverScaleMultiplier = 1.2f;

    // 각 오브젝트의 원래 크기를 저장할 딕셔너리
    private Dictionary<GameObject, Vector3> originalScales = new Dictionary<GameObject, Vector3>();

    void Start()
    {
        // 리스트에 있는 모든 오브젝트의 원래 크기를 저장
        foreach (var obj in objectsToScale)
        {
            if (obj != null)
            {
                originalScales[obj] = obj.transform.localScale;
            }
        }
    }

    void Update()
    {
        // 마우스가 현재 어느 오브젝트 위에 있는지 확인
        foreach (var obj in objectsToScale)
        {
            if (obj != null)
            {
                if (IsMouseOver(obj))
                {
                    // 마우스가 오브젝트 위에 있을 때 크기 확대
                    obj.transform.localScale = originalScales[obj] * hoverScaleMultiplier;
                }
                else
                {
                    // 마우스가 오브젝트를 벗어나면 원래 크기로 복원
                    obj.transform.localScale = originalScales[obj];
                }
            }
        }
    }

    // 마우스가 특정 오브젝트 위에 있는지 확인하는 함수
    bool IsMouseOver(GameObject obj)
    {
        // 마우스 위치를 화면에서 월드 좌표로 변환
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Raycast를 사용하여 마우스가 오브젝트 위에 있는지 확인
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == obj)
            {
                return true;
            }
        }

        return false;
    }
}
