using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class TextManager : MonoBehaviour
{
    // Unity 에디터에서 드래그 앤 드롭으로 관리할 수 있는 public 리스트
    public List<TextMeshProUGUI> textObjects;

    // Hover 시 색상과 크기 설정
    public Color hoverColor = Color.red;
    public float hoverScaleMultiplier = 1.2f;

    private Dictionary<TextMeshProUGUI, Vector3> originalScales = new Dictionary<TextMeshProUGUI, Vector3>();
    private Dictionary<TextMeshProUGUI, Color> originalColors = new Dictionary<TextMeshProUGUI, Color>();

    void Start()
    {
        // 각 텍스트의 원래 크기와 색상 저장
        foreach (var textObj in textObjects)
        {
            if (textObj != null)
            {
                originalScales[textObj] = textObj.transform.localScale;
                originalColors[textObj] = textObj.color;
            }
        }
    }

    void Update()
    {
        // 매 프레임마다 마우스 위치와 충돌하는 텍스트를 체크
        foreach (var textObj in textObjects)
        {
            if (textObj != null)
            {
                RectTransform rectTransform = textObj.GetComponent<RectTransform>();
                Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);

                // 마우스가 텍스트 위에 있는지 확인
                if (rectTransform.rect.Contains(localMousePosition))
                {
                    OnMouseEnter(textObj);
                }
                else
                {
                    OnMouseExit(textObj);
                }
            }
        }
    }

    // 마우스가 텍스트 위로 갔을 때
    void OnMouseEnter(TextMeshProUGUI textObj)
    {
        textObj.color = hoverColor;
        textObj.transform.localScale = originalScales[textObj] * hoverScaleMultiplier;
    }

    // 마우스가 텍스트에서 벗어났을 때
    void OnMouseExit(TextMeshProUGUI textObj)
    {
        textObj.color = originalColors[textObj];
        textObj.transform.localScale = originalScales[textObj];
    }
}
