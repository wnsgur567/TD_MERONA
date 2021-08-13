using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_MonsterData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;

    public int Move_Type;

    public float Atk;
    public float Atk_spd;
    public float HP;
    public float Def;
    public float Move_spd;

    public int CC_Rgs1;
    public int CC_Rgs2;
    public int CC_Rgs3;

    public int Atk_Code;
    public int Skill1Code;
    public int Skill2Code;
}