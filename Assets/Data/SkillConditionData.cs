using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
// 스킬 이동 타입
public enum E_MoveType
{
    None,

    Straight, // 직선 이동
    Curve // 곡선 이동
}
// 스킬 공격 타입
public enum E_AttackType
{
    None,

    NormalFire, // 일반 공격
    FixedFire, // 지점 공격
    PenetrateFire, // 관통 공격
    BounceFire // 튕기는 공격
}
// 스킬 발사 타입
public enum E_FireType
{
    None,

    Select_point,
    Select_self,
    Select_enemy
}
// 타겟 선정 타입
public enum E_TargetType
{
    None,

    CloseTarget,
    RandTarget,
    FixTarget,
    TileTarget
}
#endregion

[System.Serializable]
public struct S_SkillConditionData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public int Ally;
    public bool Air_Attack;
    public E_AttackType Atk_type;
    public E_FireType Atk_pick;
    public E_TargetType Target_type;
    public E_MoveType Move_type;
    public float Move_Height;
    public int PassiveCode;
    public int Atk_prefab;
    public int projectile_prefab;
    public int damage_prefab;
    public int Skill_icon;
    public string Skill_text;

    public S_SkillConditionData_Excel(SkillCondition_TableExcel origin)
    {
        No = origin.No;
        Name_KR = origin.Name_KR;
        Name_EN = origin.Name_EN;
        Code = origin.Code;
        Ally = origin.Ally;
        Air_Attack = origin.Air_Attack;
        Atk_type = (E_AttackType)origin.Atk_type;
        Atk_pick = (E_FireType)origin.Atk_pick;
        Target_type = (E_TargetType)origin.Target_type;
        Move_type = (E_MoveType)origin.Move_type;
        Move_Height = origin.Move_Height;
        PassiveCode = origin.PassiveCode;
        Atk_prefab = origin.Atk_prefeb;
        projectile_prefab = origin.projectile_prefeb;
        damage_prefab = origin.damage_prefeb;
        Skill_icon = origin.Skill_icon;
        Skill_text = origin.Skill_text;
    }
}