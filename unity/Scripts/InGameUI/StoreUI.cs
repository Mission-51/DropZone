using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;
public class StoreUI : MonoBehaviour
{
    public GameObject storeUI; // 상점 UI 전체 패널
    public Button closeButton; // Close 버튼
    
    // 캐릭터별 무기 아이콘 스프라이트들 (Boxer, Punk, Soldier, Archer 순서)
    public Sprite[] boxerWeaponSprites; // Boxer 캐릭터용 무기 아이콘
    public Sprite[] punkWeaponSprites;  // Punk 캐릭터용 무기 아이콘
    public Sprite[] soldierWeaponSprites; // Soldier 캐릭터용 무기 아이콘
    public Sprite[] archerWeaponSprites;  // Archer 캐릭터용 무기 아이콘

    // 근거리/원거리 속성 UI
    public GameObject meleeAttributeUI; // 근거리 속성 UI
    public GameObject rangedAttributeUI; // 원거리 속성 UI

    // 무기 아이콘 클릭 버튼들 (순서대로 T1, T2 속성 1~3, T3 속성 1~3)
    public Button[] weaponButtons;
        
    public int currentWeaponIndex = 0; // 현재 무기 인덱스 (0: T1, 1~3: T2 속성, 4~6: T3 속성)

    public WeaponManager weaponManager; // WeaponManager 참조

    private PlayerCharacterType characterType; // 현재 캐릭터 유형 (Boxer, Punk, Soldier, Archer)

    public TextMeshProUGUI alertMessageText; // 알림 메시지 텍스트

    // 테스트용, 추후 게임매니저의 코인과 동기화하여 관리할 예정
    public Text playerCoinText; // 플레이어 코인 표시
    private int playerCoins = 10000; // 임시로 설정한 플레이어 코인 (테스트용)

    public Inventory inventory; // Inventory 참조 추가


    // 현재 장착 중인 무기 버튼 위에 덮을 "E" 이미지
    public Image equippedOverlayImage;
        
    
    private void Start()
    {        
        storeUI.SetActive(false); // 초기에는 상점 UI 비활성화
        closeButton.onClick.AddListener(CloseStore);
        
        // 무기 버튼 이벤트 연결
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            int index = i; // 로컬 변수로 캡처 문제 방지
            weaponButtons[i].onClick.AddListener(() => BuyWeapon(index));
        }
         
        UpdatePlayerCoinUI();

        // 태그가 Player인 오브젝트들을 찾기
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            // 오브젝트 이름을 확인하여 캐릭터 타입 설정
            switch (player.name)
            {
                case "1.Boxer":
                    SetCharacterType(PlayerCharacterType.Boxer);
                    break;
                case "2.Soldier":
                    SetCharacterType(PlayerCharacterType.Soldier);
                    break;
                case "3.Punk":
                    SetCharacterType(PlayerCharacterType.Punk);
                    break;
                case "4.Archer":
                    SetCharacterType(PlayerCharacterType.Archer);
                    break;
                default:
                    Debug.LogError("알 수 없는 캐릭터 타입: " + player.name);
                    break;
            }
        }              

        // 초기 장착 무기를 티어1 무기로 설정 후 UI 업데이트
        currentWeaponIndex = 0;
        UpdateCurrentWeaponUI();

    }

    // 스페이스바 입력으로 상점 UI 활성화/비활성화
    private void Update()
    {       

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleStore();
        }
    }

    public void SetPlayer(GameObject player)
    {
        // 플레이어 프리팹에서 WeaponManager 컴포넌트를 가져와 설정합니다.
        weaponManager = player.GetComponent<WeaponManager>();

        if (weaponManager == null)
        {
            Debug.LogError("플레이어에서 WeaponManager를 찾을 수 없습니다. WeaponManager가 프리팹에 포함되어 있는지 확인하세요.");
        }

        // 메인 씬에서 Inventory 컴포넌트를 가져와 설정합니다.
        inventory = FindObjectOfType<Inventory>();

        if (inventory == null)
        {
            Debug.LogError("메인 씬에서 Inventory를 찾을 수 없습니다. Inventory가 씬에 포함되어 있는지 확인하세요.");
        }

        // 캐릭터 이름에서 "(Clone)" 제거
        string characterName = player.name.Replace("(Clone)", "").Trim();

        // 캐릭터 타입 설정        
        switch (characterName)
        {
            case "1.Boxer":
                SetCharacterType(PlayerCharacterType.Boxer);
                break;
            case "2.Soldier":
                SetCharacterType(PlayerCharacterType.Soldier);
                break;
            case "3.Punk":
                SetCharacterType(PlayerCharacterType.Punk);
                break;
            case "4.Archer":
                SetCharacterType(PlayerCharacterType.Archer);
                break;
            default:
                Debug.LogError("알 수 없는 캐릭터 타입: " + characterName);
                return;
        }

        // 초기 무기 설정 및 UI 업데이트
        currentWeaponIndex = 0;  // 기본 무기 인덱스 설정
        UpdateWeaponButtonUI();  // 무기 버튼 UI 업데이트
        UpdateAttributeUI();     // 속성 UI 업데이트
        UpdateCurrentWeaponUI(); // 현재 장착된 무기 UI 업데이트
    }


    // 캐릭터 타입 설정 - 캐릭터 타입에 따른 UI 이미지, 속성 이미지 변경
    public void SetCharacterType(PlayerCharacterType type)
    {
        characterType = type;        
        // 캐릭터 타입에 따른 UI 업데이트
        UpdateWeaponButtonUI();
        UpdateAttributeUI();
        UpdateCurrentWeaponUI();
    }

    // 무기 구매 처리
    public void BuyWeapon(int weaponIndex)
    {
        // 무기 구매 조건 검사 및 처리
        if (weaponIndex == 0) // T1 무기
        {
            currentWeaponIndex = 0;
        }
        else if (weaponIndex >= 1 && weaponIndex <= 3) // T2 무기 (속성 1~3)
        {
            if (currentWeaponIndex == 0 && inventory.coin >= 300) // T1에서 T2로 업그레이드
            {
                inventory.coin -= 300; // 인벤토리 코인 사용
                currentWeaponIndex = weaponIndex;
                ShowAlertMessage("T2 무기 업그레이드 성공!");
            }
            else if (currentWeaponIndex >= 1 && currentWeaponIndex <= 3 && inventory.coin >= 300) // T2 속성 변경
            {
                inventory.coin -= 300; // 인벤토리 코인 사용
                currentWeaponIndex = weaponIndex;
                ShowAlertMessage("T2 무기 속성 변경 성공!");
            }
            else
            {
                ShowAlertMessage("충분한 코인이 필요합니다.");
                return;
            }
        }
        else if (weaponIndex >= 4 && weaponIndex <= 6) // T3 무기 (속성 1~3)
        {
            if (currentWeaponIndex == 0) // T1에서 T3로 바로 가려고 할 때
            {
                ShowAlertMessage("T2 무기로 업그레이드 후 T3로 업그레이드할 수 있습니다.");
                return;
            }
            else if (currentWeaponIndex >= 1 && currentWeaponIndex <= 3 && inventory.coin >= 1000) // T2에서 T3로 업그레이드
            {
                inventory.coin -= 1000; // 인벤토리 코인 사용
                currentWeaponIndex = weaponIndex;
                ShowAlertMessage("T3 무기 업그레이드 성공!");
            }
            else if (currentWeaponIndex >= 4 && currentWeaponIndex <= 6 && inventory.coin >= 1000) // T3 속성 변경
            {
                inventory.coin -= 1000; // 인벤토리 코인 사용
                currentWeaponIndex = weaponIndex;
                ShowAlertMessage("T3 무기 속성 변경 성공!");
            }
            else
            {
                ShowAlertMessage("충분한 코인이 필요합니다.");
                return;
            }
        }

        // UI 업데이트 및 WeaponManager와의 동기화
        SyncWeaponWithManager();
        UpdatePlayerCoinUI();
        UpdateCurrentWeaponUI(); // 현재 장착 중인 무기 UI 업데이트        
        UpdateWeaponButtonInteractable(); // 무기 버튼 활성화/비활성화 업데이트

    }

    // 무기 버튼 활성화/비활성화 업데이트
    private void UpdateWeaponButtonInteractable()
    {
        // T1 무기 버튼 비활성화 (티어가 1 이상일 때)
        if (currentWeaponIndex >= 1)
        {
            weaponButtons[0].interactable = false;
        }

        // T2 무기 버튼 비활성화 (티어가 3으로 올라갔을 때)
        if (currentWeaponIndex >= 4)
        {
            for (int i = 1; i <= 3; i++) // T2 속성 1~3
            {
                weaponButtons[i].interactable = false;
            }
        }
    }

    // WeaponManager와 무기 동기화
    private void SyncWeaponWithManager()
    {
        weaponManager.SwitchWeapon(currentWeaponIndex);        
    }

    // 플레이어 코인 UI 업데이트
    private void UpdatePlayerCoinUI()
    {        
        inventory.coinText.text = inventory.coin.ToString();
    }

    // 무기 버튼 UI 초기화
    private void UpdateWeaponButtonUI()
    {
        Sprite[] selectedWeaponSprites = GetWeaponSpritesByCharacter();

        for (int i = 0; i < weaponButtons.Length; i++)
        {
            weaponButtons[i].image.sprite = selectedWeaponSprites[i]; // 버튼 이미지 동기화
        }
    }

    // 속성 UI 업데이트 (근거리/원거리 캐릭터에 따라)
    private void UpdateAttributeUI()
    {
        if (characterType == PlayerCharacterType.Boxer || characterType == PlayerCharacterType.Punk)
        {
            meleeAttributeUI.SetActive(true);
            rangedAttributeUI.SetActive(false);
        }
        else if (characterType == PlayerCharacterType.Soldier || characterType == PlayerCharacterType.Archer)
        {
            meleeAttributeUI.SetActive(false);
            rangedAttributeUI.SetActive(true);
        }
    }

    private void UpdateCurrentWeaponUI()
    {
        // 모든 무기 버튼에 대해 오버레이 이미지를 비활성화합니다.
        for (int i = 0; i < weaponButtons.Length; i++)
        {
            // 각 무기 버튼에 포함된 equippedOverlayImage를 비활성화
            Transform overlayTransform = weaponButtons[i].transform.Find("EquippedOverlay");
            if (overlayTransform != null)
            {
                overlayTransform.gameObject.SetActive(false);
            }
        }

        // 현재 장착된 무기의 버튼에만 오버레이 이미지를 활성화합니다.
        Transform currentOverlayTransform = weaponButtons[currentWeaponIndex].transform.Find("EquippedOverlay");
        if (currentOverlayTransform != null)
        {
            currentOverlayTransform.gameObject.SetActive(true);
        }
    }

    // 캐릭터 유형에 따른 무기 스프라이트 배열 반환
    private Sprite[] GetWeaponSpritesByCharacter()
    {
        switch (characterType)
        {
            case PlayerCharacterType.Boxer:
                return boxerWeaponSprites;
            case PlayerCharacterType.Punk:
                return punkWeaponSprites;
            case PlayerCharacterType.Soldier:
                return soldierWeaponSprites;
            case PlayerCharacterType.Archer:
                return archerWeaponSprites;
            default:
                Debug.LogError("잘못된 캐릭터 유형입니다: " + characterType);
                return null;
        }
    }

    // 상점 UI 토글
    private void ToggleStore() 
    {
        bool isActive = storeUI.activeSelf;
        storeUI.SetActive(!isActive);

        // 상점이 활성화되면 공격 불가, 비활성화되면 공격 가능
        if (weaponManager != null && weaponManager.gameObject != null)
        {
            GameManager.instance.SetCanAttackForPlayer(weaponManager.gameObject, !storeUI.activeSelf);
        }
    }

    // 상점 닫기 버튼 클릭 시 호출
    private void CloseStore()
    {
        storeUI.SetActive(false);

        // 상점을 닫으면 공격 가능하게 설정
        if (weaponManager != null && weaponManager.gameObject != null)
        {
            GameManager.instance.SetCanAttackForPlayer(weaponManager.gameObject, true);
        }
    }

    // 알림 메시지 표시
    private void ShowAlertMessage(string message)
    {
        alertMessageText.text = message;
        alertMessageText.gameObject.SetActive(true);
        StartCoroutine(HideAlertMessageAfterDelay(2.0f)); // 2초 후 메시지 숨김
    }

    // 알림 메시지를 일정 시간 후에 숨기는 코루틴
    private IEnumerator HideAlertMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        alertMessageText.gameObject.SetActive(false);
    }
}
