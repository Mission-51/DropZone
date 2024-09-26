using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerStatus;

public class WeaponEffectManager : MonoBehaviour
{
    public GameObject[] attributeEffects;  // 속성 공격 이펙트 배열 (Attack_Point 하위의 이펙트들)
    public GameObject[] attributeEffects_Left;  // 속성 무기 이펙트 배열 (Weapon_Effect_L 하위의 이펙트들)
    public GameObject[] attributeEffects_Right;  // 속성 무기 이펙트 배열 (Weapon_Effect_R 하위의 이펙트들)

    // 각 속성에 해당하는 상태이상을 정의 (ice -> Slow2으로 연결)
    public StatusEffect[] attributeStatusEffects = { StatusEffect.None, StatusEffect.Slow2, StatusEffect.None, StatusEffect.None };

    private int currentAttributeIndex = 0; // 현재 속성 인덱스

    public int healthRestoreAmount = 5; // 공격 시 회복량 설정 (기본 5)

    void Start()
    {
        // 시작할 때 첫 번째 속성 이펙트만 활성화
        SwitchAttributeEffect(0);
    }

    // 속성 이펙트 교체 로직
    public void HandleAttributeSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchAttributeEffect(0); // 4번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchAttributeEffect(1); // 5번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchAttributeEffect(2); // 6번 속성
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchAttributeEffect(3); // 7번 속성
        }
    }

    private void SwitchAttributeEffect(int attributeIndex)
    {
        // 범위를 넘는 인덱스가 입력되지 않도록 예외 처리
        if (attributeIndex >= attributeEffects.Length || attributeIndex >= attributeEffects_Left.Length || attributeIndex >= attributeEffects_Right.Length)
            return;

        // 모든 속성 공격 이펙트를 비활성화하고 선택한 속성 공격 이펙트만 활성화
        for (int i = 0; i < attributeEffects.Length; i++)
        {
            attributeEffects[i].SetActive(i == attributeIndex);
        }

        // 모든 왼손 무기 속성 이펙트를 비활성화하고 선택한 왼손 이펙트만 활성화
        for (int i = 0; i < attributeEffects_Left.Length; i++)
        {
            attributeEffects_Left[i].SetActive(i == attributeIndex);
        }

        // 모든 오른손 무기 속성 이펙트를 비활성화하고 선택한 오른손 이펙트만 활성화
        for (int i = 0; i < attributeEffects_Right.Length; i++)
        {
            attributeEffects_Right[i].SetActive(i == attributeIndex);
        }

        // 현재 속성 인덱스를 업데이트
        currentAttributeIndex = attributeIndex;
    }

    // 현재 속성에 맞는 상태이상을 반환하는 함수
    public StatusEffect GetCurrentStatusEffect()
    {
        if (currentAttributeIndex < attributeStatusEffects.Length)
        {
            return attributeStatusEffects[currentAttributeIndex];
        }

        return StatusEffect.None; // 기본적으로 상태이상이 없을 때
    }

    // 공격 시 호출되는 함수
    public void OnAttackHit()
    {
        if (currentAttributeIndex == 0) // 4번 슬롯에 해당할 경우
        {
            RestoreHealth();
        }
    }

    // 플레이어의 체력을 회복하는 함수
    private void RestoreHealth()
    {
        PlayerStatus playerStatus = GetComponent<PlayerStatus>(); // 플레이어 상태 가져오기
        if (playerStatus != null)
        {
            playerStatus.RestoreHealth(healthRestoreAmount); // 체력 회복
        }
    }
}
