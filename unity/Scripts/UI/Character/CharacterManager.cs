using UnityEngine;
using System.Collections.Generic;
using TMPro; // TextMeshPro 사용을 위해 추가

public class CharacterManager : MonoBehaviour
{
    public GameObject[] lobbyCharacters; // 로비에 있는 캐릭터들 (미리 배치된 상태)
    public TextMeshProUGUI characterNameText; // 캐릭터 이름을 표시할 TextMeshProUGUI
    public string[] characterNames; // 캐릭터 이름 리스트 (인덱스 순서대로 설정)
    public List<string> networkCharacters;
    public string userID; // 유저 ID (외부에서 할당

    private int selectedCharacterIndex = 0; // 선택된 캐릭터의 인덱스 (-1은 선택 안 됨 상태)

    void Start()
    {
        
        // 게임 시작 시 기본으로 첫 번째 캐릭터 활성화
        SetSelectedCharacter(0);
        updateList();
        characterNameText.text = characterNames[0];
        UpdateNetworkCharacter(0);
        
    }

    public void updateList()
    {
        for(int i = 0; i < lobbyCharacters.Length; i++)
        {
            string temp = i + 1 + "." + lobbyCharacters[i].name;
            networkCharacters.Add(temp);
        }
    }

    public void SetSelectedCharacter(int index)
    {
        Debug.Log(networkCharacters.Count);
        Debug.Log("1234");
        if (index >= 0 && index < lobbyCharacters.Length && index < networkCharacters.Count)
        {
            Debug.Log("12345");
            selectedCharacterIndex = index;
            Debug.Log("Selected Character Index: " + selectedCharacterIndex);
            UpdateLobbyCharacter(); // 로비 캐릭터 업데이트
            UpdateCharacterText(); // 캐릭터 텍스트 업데이트
            Debug.Log("인덱스 체크 체크 : " + index);
            UpdateNetworkCharacter(index);
        }
    }

    // 로비에 선택된 캐릭터만 활성화
    private void UpdateLobbyCharacter()
    {
        for (int i = 0; i < lobbyCharacters.Length; i++)
        {
            Debug.Log("Setting Active: " + i + " -> " + (i == selectedCharacterIndex));
            lobbyCharacters[i].SetActive(i == selectedCharacterIndex); // 선택된 캐릭터만 활성화, 나머지는 비활성화
        }
    }

    // 캐릭터 텍스트 업데이트
    private void UpdateCharacterText()
    {
        // 선택된 캐릭터의 이름을 텍스트로 표시 (characterNames 배열을 사용)
        if (characterNameText != null && characterNames != null && selectedCharacterIndex < characterNames.Length)
        {
            characterNameText.text = characterNames[selectedCharacterIndex]; // 인덱스 순서에 맞는 캐릭터 이름 반영
            Debug.Log("Updated Character Name: " + characterNames[selectedCharacterIndex]);
        }
    }
    // 빠른 시작 버튼을 눌렀을 때 호출
    
    //Photon 캐릭터 설정
    private void UpdateNetworkCharacter(int index)
    {
        Debug.Log(networkCharacters[index]);
        NetworkManager.instance.setSelCharacter(networkCharacters[index]);
    }
}
