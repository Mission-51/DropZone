using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour, IPointerClickHandler
{
    public List<GameObject> cardList = new List<GameObject>(); // 카드 리스트
    public CharacterManager characterManager; // CharacterManager 참조
    public Button selectButton; // 선택 버튼 (Inspector에서 드래그해서 연결)

    private GameObject currentlySelectedCard; // 현재 선택된 카드
    private int selectedCharacterIndex = -1; // 선택된 캐릭터의 인덱스

    void Start()
    {
        // 모든 카드에 클릭 이벤트를 설정하고, Outline 초기화
        foreach (var card in cardList)
        {
            CardSelector selector = card.AddComponent<CardSelector>(); // 각 카드에 CardSelector 추가
            selector.Initialize(this); // CardSelector에 CardManager 전달

            Outline outline = card.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = false; // 초기에는 Outline 비활성화
            }
        }

        // 시작 시 기본 캐릭터(첫 번째 캐릭터)를 활성화
        characterManager.SetSelectedCharacter(0);
    }

    // 카드 선택 시 호출
    public void SelectCard(GameObject newCard)
    {
        // 이전에 선택된 카드가 있다면 Outline 비활성화
        if (currentlySelectedCard != null)
        {
            Outline previousOutline = currentlySelectedCard.GetComponent<Outline>();
            if (previousOutline != null)
            {
                previousOutline.enabled = false;
            }
        }

        // 새로 선택된 카드의 Outline 활성화
        Outline newOutline = newCard.GetComponent<Outline>();
        if (newOutline != null)
        {
            newOutline.enabled = true;
            currentlySelectedCard = newCard; // 새로 선택된 카드 업데이트

            // 선택된 캐릭터 인덱스를 업데이트 (카드의 인덱스와 캐릭터 인덱스가 일치한다고 가정)
            selectedCharacterIndex = cardList.IndexOf(newCard);
            Debug.Log(selectedCharacterIndex);
        }
    }

    // IPointerClickHandler 구현
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.pointerPress == selectButton.gameObject) // 선택 버튼이 클릭되었을 때
        {
            if (selectedCharacterIndex != -1)
            {
                characterManager.SetSelectedCharacter(selectedCharacterIndex); // 선택된 캐릭터를 로비에 반영
                Debug.Log("Character Index Sent: " + selectedCharacterIndex);
            }
        }
    }
}
