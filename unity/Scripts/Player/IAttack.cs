// IAttack.cs
public interface IAttack
{
    void GetAttackInput(bool fDown);   
    float GetSkillCooldown(); // 스킬 쿨타임을 반환하는 메서드
    float GetLastSkillTime(); // 마지막 스킬 사용 시간을 반환하는 메서드

}
