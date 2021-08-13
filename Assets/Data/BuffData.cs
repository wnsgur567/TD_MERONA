using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum
public enum E_BuffType
{
    None,

    Atk,
    Range,
    Def,
    Atk_spd,
    Move_spd,
    Crit_rate,
    Crit_Dmg,

    Stun,
    Dot_Dmg,
    Insta_Kill,
    CritDmg_less,
    CritDmg_more,

    Heal,
    Summon,
    Shield
}

public enum E_AddType
{
    None,

    Fix,
    Percent
}
#endregion
[System.Serializable]
public struct S_Buff
{
    public E_BuffType BuffType;
    public E_AddType AddType;
    public float BuffAmount;
    public float BuffRand;
    public int Summon;
}

[System.Serializable]
public struct S_BuffData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public S_Buff Buff1;
    public S_Buff Buff2;
    public S_Buff Buff3;
    public float Duration;
    public int Prefab;

    public S_BuffData_Excel(BuffCC_TableExcel origin)
    {
        No = origin.No;
        Name_KR = origin.Name_KR;
        Name_EN = origin.Name_EN;
        Code = origin.Code;
        Buff1.BuffType = (E_BuffType)origin.BuffType1;
        Buff1.AddType = (E_AddType)origin.AddType1;
        Buff1.BuffAmount = origin.BuffAmount1;
        Buff1.BuffRand = origin.BuffRand1;
        Buff1.Summon = origin.Summon1;
        Buff2.BuffType = (E_BuffType)origin.BuffType2;
        Buff2.AddType = (E_AddType)origin.AddType2;
        Buff2.BuffAmount = origin.BuffAmount2;
        Buff2.BuffRand = origin.BuffRand2;
        Buff2.Summon = origin.Summon2;
        Buff3.BuffType = (E_BuffType)origin.BuffType3;
        Buff3.AddType = (E_AddType)origin.AddType3;
        Buff3.BuffAmount = origin.BuffAmount3;
        Buff3.BuffRand = origin.BuffRand3;
        Buff3.Summon = origin.Summon3;
        Duration = origin.Duration;
        Prefab = origin.Prefeb;
    }
}