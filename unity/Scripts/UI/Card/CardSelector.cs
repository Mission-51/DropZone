using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelector : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardManager cardManager;

    // CardManager와 연결
    public void Initialize(CardManager manager)
    {
        cardManager = manager;
    }

    // 카드가 클릭될 때 호출
    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardManager != null)
        {
            cardManager.SelectCard(gameObject); // 클릭된 카드 전달

        }
    }

    // 포인터가 카드 위로 들어올 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 포인터가 카드 위로 들어왔을 때 발생할 추가 동작을 여기에 작성할 수 있습니다.
    }

    // 포인터가 카드 위에서 나갈 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        // 포인터가 카드 위에서 나갔을 때 발생할 추가 동작을 여기에 작성할 수 있습니다.
    }
}
