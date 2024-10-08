using UnityEngine;
using UnityEngine.EventSystems;
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

                // 각 오브젝트에 MouseHoverHandler 컴포넌트를 추가
                MouseHoverHandler hoverHandler = obj.AddComponent<MouseHoverHandler>();
                hoverHandler.Initialize(this, obj);
            }
        }
    }

    // 오브젝트 크기를 변경하는 함수
    public void ScaleObject(GameObject obj, bool isHovered)
    {
        if (isHovered)
        {
            obj.transform.localScale = originalScales[obj] * hoverScaleMultiplier;
        }
        else
        {
            obj.transform.localScale = originalScales[obj];
        }
    }
}

public class MouseHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameObjectManager gameObjectManager;
    private GameObject targetObject;

    // 초기화 함수
    public void Initialize(GameObjectManager manager, GameObject obj)
    {
        gameObjectManager = manager;
        targetObject = obj;
    }

    // 마우스가 오브젝트 위로 들어갈 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObjectManager.ScaleObject(targetObject, true);
    }

    // 마우스가 오브젝트를 벗어날 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObjectManager.ScaleObject(targetObject, false);
    }
}
