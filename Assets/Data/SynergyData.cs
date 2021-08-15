using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region enum
public enum E_SynergyEffectType
{
    None,

    Buff,
    ChangeAtkType,
    ReduceCooldown,
    Berserker,
    AddGold
}

public enum E_SynergyEffectAmount
{
    None,

    Tower,
    Enemy,
    King
}
#endregion

[System.Serializable]
public struct S_SynergyEffect
{
    public E_SynergyEffectType EffectType;
    public E_SynergyEffectAmount EffectAmount;
    public int EffectCode;
    public E_AttackType EffectChange;
    public int EffectReq;
    public float EffectRand;
}

[System.Serializable]
public struct S_SynergyData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public int Rank;
    public int MemReq;
    public int TargetMem;
    public S_SynergyEffect Effect1;
    public S_SynergyEffect Effect2;
    public int Prefab;

    public S_SynergyData_Excel(Synergy_TableExcel origin)
    {
        No = origin.No;
        Name_KR = origin.Name_KR;
        Name_EN = origin.Name_EN;
        Code = origin.Code;
        Rank = origin.Rank;
        MemReq = origin.MemReq;
        TargetMem = origin.TargetMem;
        Effect1.EffectType = (E_SynergyEffectType)origin.EffectType1;
        Effect1.EffectAmount = (E_SynergyEffectAmount)origin.EffectAmount1;
        Effect1.EffectCode = origin.EffectCode1;
        Effect1.EffectChange = (E_AttackType)origin.EffectChange1;
        Effect1.EffectReq = origin.EffectReq1;
        Effect1.EffectRand = origin.EffectRand1;
        Effect2.EffectType = (E_SynergyEffectType)origin.EffectType2;
        Effect2.EffectAmount = (E_SynergyEffectAmount)origin.EffectAmount2;
        Effect2.EffectCode = origin.EffectCode2;
        Effect2.EffectChange = (E_AttackType)origin.EffectChange2;
        Effect2.EffectReq = origin.EffectReq2;
        Effect2.EffectRand = origin.EffectRand2;
        Prefab = origin.Prefeb;
    }
}