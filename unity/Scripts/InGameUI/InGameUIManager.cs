using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{
    // 대쉬 쿨타임 UI
    public Image dashCooldownProgressImage; // 프로그레스 바 이미지
    public Image dashCooldownBackgroundImage;  // 항상 떠있는 기본 배경 이미지
    public TextMeshProUGUI dashCooldownText;

    // 스킬 쿨타임 UI
    public Image skillCooldownProgressImage; // 프로그레스 바 이미지
    public Image skillBackgroundImage;   // 항상 떠있는 기본 배경 이미지
    public TextMeshProUGUI skillCooldownText;

    // 장탄수 UI
    public TextMeshProUGUI ammoCountText;

    // 사용 아이템 쿨타임 UI
    public Image itemCooldownProgressImage;
    public TextMeshProUGUI itemCooldownText;

    // 회복 아이템 사용 시간 UI
    public Image healProgressImage;
    public TextMeshProUGUI healProgressText;

    // 캐릭터별 스킬 이미지들
    public Sprite boxerSkillImage;
    public Sprite punkSkillImage;
    public Sprite archerSkillImage;
    public Sprite soldierSkillImage;

    // PlayerMovement, PlayerStatus 및 PlayerAttack 스크립트 참조
    private PlayerMovement playerMovement;
    private PlayerStatus playerStatus;

    private IAttack playerAttack; // 모든 캐릭터의 공격 스크립트가 IAttack 인터페이스를 구현하므로 이를 사용
    private PlayerAttack_Soldier playerAttackSoldier; // Soldier 전용 스크립트 참조

    // 캐릭터 타입
    private PlayerCharacterType characterType;

    // HP바 UI 이미지
    public Image healthBarImage; 

    void Start()
    {
        // 기본 UI 초기화
        InitializeUI();
    }

    void Update()
    {
        // 대쉬 쿨타임 업데이트
        UpdateDashCooldownUI();

        // 스킬 쿨타임 업데이트
        UpdateSkillCooldownUI();

        // Soldier의 장탄수 UI 업데이트
        UpdateAmmoUI();

        // 기존 업데이트 메서드에 추가
        UpdateHealthUI();
    }

    // 로컬 플레이어 설정
    public void SetPlayer(GameObject player)
    {
        // PlayerMovement 및 PlayerAttack 스크립트 찾기
        playerMovement = player.GetComponent<PlayerMovement>(); // PlayerMovement 스크립트 연동
        playerStatus = player.GetComponent<PlayerStatus>(); // PlayerStatus 스크립트 연동
        // 캐릭터 이름에서 "(Clone)" 제거
        string characterName = player.name.Replace("(Clone)", "").Trim();

        // 캐릭터 타입 설정        
        switch (characterName)
        {
            case "1.Boxer":
                characterType = PlayerCharacterType.Boxer;
                break;
            case "2.Soldier":
                characterType = PlayerCharacterType.Soldier;
                break;
            case "3.Punk":
                characterType = PlayerCharacterType.Punk;
                break;
            case "4.Archer":
                characterType = PlayerCharacterType.Archer;
                break;
            default:
                Debug.LogError("알 수 없는 캐릭터 타입: " + characterName);
                return;
        }

        // 공격 스크립트 찾기
        FindPlayerAttackScript(player, characterType);

        UpdateHealthUI(); // 초기 HP UI 설정
        SetupSkillImage(characterType);  // 캐릭터 타입에 맞는 스킬 이미지 설정
        SetupAmmoUI(characterType);  // 캐릭터 타입에 따라 장탄수 UI 설정
    }

    // 캐릭터 타입에 따른 PlayerAttack 스크립트 찾기
    private void FindPlayerAttackScript(GameObject player, PlayerCharacterType characterType)
    {
        switch (characterType)
        {
            case PlayerCharacterType.Boxer:
                playerAttack = player.GetComponent<PlayerAttack_Boxer>();
                break;
            case PlayerCharacterType.Punk:
                playerAttack = player.GetComponent<PlayerAttack_Punk>();
                break;
            case PlayerCharacterType.Archer:
                playerAttack = player.GetComponent<PlayerAttack_Archer>();
                break;
            case PlayerCharacterType.Soldier:
                playerAttack = player.GetComponent<PlayerAttack_Soldier>();
                playerAttackSoldier = player.GetComponent<PlayerAttack_Soldier>();
                break;
            default:
                Debug.LogError("알 수 없는 캐릭터 타입입니다.");
                break;
        }
    }

    // 체력바 UI 업데이트
    private void UpdateHealthUI()
    {
        if (playerStatus != null)
        {
            // 현재 HP와 최대 HP를 이용해 fillAmount 계산
            float healthProgress = (float)playerStatus.currentHP / playerStatus.maxHP;
            healthBarImage.fillAmount = Mathf.Clamp01(healthProgress);
        }
    }

    // 대쉬 쿨타임 UI 업데이트
    private void UpdateDashCooldownUI()
    {
        float cooldownProgress = (playerMovement.dashCooldown - (Time.time - playerMovement.lastDashTime)) / playerMovement.dashCooldown;
        float remainingTime = playerMovement.dashCooldown - (Time.time - playerMovement.lastDashTime);

        // 프로그레스 바의 진행 상태 업데이트
        dashCooldownProgressImage.fillAmount = Mathf.Clamp01(cooldownProgress);

        // 남은 시간 텍스트 업데이트
        dashCooldownText.text = remainingTime > 0 ? Mathf.Ceil(remainingTime).ToString() : "";
    }

    // 스킬 쿨타임 UI 업데이트
    private void UpdateSkillCooldownUI()
    {
        if (playerAttack != null)
        {
            // 스킬 쿨타임 계산
            float cooldownProgress = (playerAttack.GetSkillCooldown() - (Time.time - playerAttack.GetLastSkillTime())) / playerAttack.GetSkillCooldown();
            float remainingTime = playerAttack.GetSkillCooldown() - (Time.time - playerAttack.GetLastSkillTime());

            // 스킬 쿨타임 UI 업데이트
            skillCooldownProgressImage.fillAmount = Mathf.Clamp01(cooldownProgress);
            skillCooldownText.text = remainingTime > 0 ? Mathf.Ceil(remainingTime).ToString() : "";
        }
    }

    // 사용 아이템 UI 초기화
    public void ShowItemCooldownUI(bool isActive)
    {
        itemCooldownProgressImage.gameObject.SetActive(isActive);
        itemCooldownText.gameObject.SetActive(isActive);
    }

    // 사용 아이템 쿨타임 UI 업데이트
    public void UpdateItemCooldownUI(float cooldownProgress, float remainingTime)
    {
        if (itemCooldownProgressImage != null)
        {
            // 쿨타임 진행 상태를 프로그레스 바에 업데이트
            itemCooldownProgressImage.fillAmount = Mathf.Clamp01(cooldownProgress);
        }

        if (itemCooldownText != null)
        {
            // 남은 시간을 텍스트로 표시
            itemCooldownText.text = remainingTime > 0 ? Mathf.Ceil(remainingTime).ToString() : "";
        }
    }

    // 회복 아이템 UI 업데이트
    public void UpdateHealProgressUI(float progress, float remainingTime)
    {
        healProgressImage.fillAmount = progress;
        healProgressText.text = remainingTime > 0 ? Mathf.Ceil(remainingTime).ToString() : "";
    }

    // 회복 UI 활성화/비활성화
    public void ShowHealUI(bool isActive)
    {
        healProgressImage.gameObject.SetActive(isActive);
        healProgressText.gameObject.SetActive(isActive);
    }

    // 기본 UI 초기화
    private void InitializeUI()
    {
        if (dashCooldownProgressImage == null || dashCooldownText == null || dashCooldownBackgroundImage == null)
            Debug.LogError("대쉬 쿨타임 UI가 설정되지 않았습니다.");
        if (skillCooldownProgressImage == null || skillCooldownText == null || skillCooldownProgressImage == null)
            Debug.LogError("스킬 쿨타임 UI가 설정되지 않았습니다.");
        if (ammoCountText == null)
            Debug.LogError("장탄수 UI가 설정되지 않았습니다.");
        if (itemCooldownProgressImage == null || itemCooldownText == null)
            Debug.LogError("사용 아이템 쿨타임 UI가 설정되지 않았습니다.");
        if (healProgressImage == null || healProgressText == null)
            Debug.LogError("회복 아이템 UI가 설정되지 않았습니다.");

        // UI 초기 비활성화
        ShowHealUI(false);
        ShowItemCooldownUI(false);
    }



    

    // 캐릭터에 따라 스킬 이미지를 설정하는 함수
    private void SetupSkillImage(PlayerCharacterType characterType)
    {
        switch (characterType)
        {
            case PlayerCharacterType.Boxer:
                skillCooldownProgressImage.sprite = boxerSkillImage;  // 복서 스킬 이미지
                skillBackgroundImage.sprite = boxerSkillImage;
                break;
            case PlayerCharacterType.Punk:
                skillCooldownProgressImage.sprite = punkSkillImage;  // 펑크 스킬 이미지
                skillBackgroundImage.sprite = punkSkillImage;
                break;
            case PlayerCharacterType.Archer:
                skillCooldownProgressImage.sprite = archerSkillImage;  // 궁수 스킬 이미지
                skillBackgroundImage.sprite = archerSkillImage;
                break;
            case PlayerCharacterType.Soldier:
                skillCooldownProgressImage.sprite = soldierSkillImage;  // 군인 스킬 이미지
                skillBackgroundImage.sprite = soldierSkillImage;
                break;
            default:
                Debug.LogError("알 수 없는 캐릭터 타입입니다.");
                break;
        }
    }

    // 장탄수 UI 설정
    private void SetupAmmoUI(PlayerCharacterType characterType)
    {
        switch (characterType)
        {
            case PlayerCharacterType.Boxer:
            case PlayerCharacterType.Punk:
                ammoCountText.gameObject.SetActive(false);  // 근접 캐릭터는 장탄수 표시 안함
                break;
            case PlayerCharacterType.Archer:
                ammoCountText.gameObject.SetActive(true);
                ammoCountText.text = "1 / ∞";  // 궁수는 고정된 장탄수
                break;
            case PlayerCharacterType.Soldier:
                ammoCountText.gameObject.SetActive(true);
                break;
        }
    }

    // 장탄수 UI 업데이트
    private void UpdateAmmoUI()
    {
        if (playerAttackSoldier != null && characterType == PlayerCharacterType.Soldier)
        {
            int currentAmmo = playerAttackSoldier.GetCurrentAmmo();
            int maxAmmo = playerAttackSoldier.GetMaxAmmo();

            // Soldier의 장탄수를 UI에 업데이트
            ammoCountText.text = $"{currentAmmo} / ∞";
        }
    }
}

