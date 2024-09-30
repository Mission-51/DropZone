using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] leftWeapons;  // 왼손에 장착할 무기 배열 (Hierarchy에서 미리 위치해 있는 오브젝트)
    public GameObject[] rightWeapons; // 오른손에 장착할 무기 배열 (Hierarchy에서 미리 위치해 있는 오브젝트)

    public GameObject[] attributeEffects;  // 무기 속성에 따른 이펙트 배열

    private int currentWeaponIndex = 0; // 현재 선택된 무기 인덱스

    private Weapon currentWeapon; // 현재 선택된 무기

    void Start()
    {
        // 시작할 때 첫 번째 무기만 활성화
        SwitchWeapon(0);

        // 무기 변경 및 상태는 다른 클라이언트에 동기화되어야 하므로 PhotonView와 RPC 사용 필요
    }

    // 무기 교체 로직
    public void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(0); // 1번 무기
            // 무기 변경은 다른 클라이언트에 동기화되어야 함
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(1); // 2번 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(2); // 3번 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SwitchWeapon(3); // 4번 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            SwitchWeapon(4); // 5번 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            SwitchWeapon(5); // 6번 무기
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SwitchWeapon(6); // 7번 무기
        }

        // 무기 교체는 다른 클라이언트와 동기화해야 하므로, RPC로 무기 변경 상태를 전달할 수 있음
    }

    private void SwitchWeapon(int weaponIndex)
    {
        // 왼손 무기가 있는 경우에만 처리
        if (leftWeapons.Length > 0)
        {
            // 범위를 넘는 인덱스가 입력되지 않도록 예외 처리
            if (weaponIndex < leftWeapons.Length)
            {
                // 모든 왼손 무기를 비활성화하고 선택한 무기만 활성화
                for (int i = 0; i < leftWeapons.Length; i++)
                {
                    leftWeapons[i].SetActive(i == weaponIndex);
                }
                // 현재 선택된 왼손 무기의 Weapon 컴포넌트를 가져옴
                currentWeapon = leftWeapons[weaponIndex].GetComponent<Weapon>();
            }
        }

        // 오른손 무기가 있는 경우에만 처리
        if (rightWeapons.Length > 0)
        {
            // 범위를 넘는 인덱스가 입력되지 않도록 예외 처리
            if (weaponIndex < rightWeapons.Length)
            {
                // 모든 오른손 무기를 비활성화하고 선택한 무기만 활성화
                for (int i = 0; i < rightWeapons.Length; i++)
                {
                    rightWeapons[i].SetActive(i == weaponIndex);
                }
                // 현재 선택된 오른손 무기의 Weapon 컴포넌트를 가져옴
                currentWeapon = rightWeapons[weaponIndex].GetComponent<Weapon>();
            }
        }

        // 현재 무기 인덱스를 업데이트
        currentWeaponIndex = weaponIndex;

        // 무기 속성에 따른 공격 이펙트 활성화 (배열이 있는 경우에만, 근거리에만 사용)
        if (attributeEffects != null && attributeEffects.Length > 0)
        {
            ActivateEffectBasedOnAttribute();
        }

        // 무기 속성 및 무기 변경 상태도 네트워크 상에 동기화가 필요할 수 있음
    }

    // 무기 속성에 따른 이펙트를 활성화하는 함수
    private void ActivateEffectBasedOnAttribute()
    {
        // 모든 이펙트를 비활성화
        foreach (var effect in attributeEffects)
        {
            effect.SetActive(false);
        }

        // 현재 무기의 속성에 맞는 이펙트를 활성화
        if (currentWeapon != null)
        {
            switch (currentWeapon.weaponAttribute)
            {
                case WeaponAttribute.None:
                    attributeEffects[0].SetActive(true); // Ice 이펙트 활성화
                    break;
                case WeaponAttribute.Blood:
                    attributeEffects[1].SetActive(true); // Blood 이펙트 활성화
                    break;
                case WeaponAttribute.Ice:
                    attributeEffects[2].SetActive(true); // Ice 이펙트 활성화
                    break;
                case WeaponAttribute.Iron:
                    attributeEffects[3].SetActive(true); // Iron 이펙트 활성화
                    break;
                case WeaponAttribute.Gear:
                    attributeEffects[4].SetActive(true); // Gear 이펙트 활성화
                    break;
                case WeaponAttribute.Gunpowder:
                    attributeEffects[5].SetActive(true); // Gunpowder 이펙트 활성화
                    break;
            }
        }

        // 무기 속성에 따른 이펙트 변화 역시 네트워크 상에 동기화가 필요할 수 있음
    }

    // 현재 무기의 데미지 반환
    public int GetCurrentWeaponDamage()
    {
        return currentWeapon != null ? currentWeapon.damage : 0;
    }

    // 현재 무기의 속성 반환
    public WeaponAttribute GetCurrentWeaponAttribute()
    {
        return currentWeapon != null ? currentWeapon.weaponAttribute : WeaponAttribute.None;
    }
}
