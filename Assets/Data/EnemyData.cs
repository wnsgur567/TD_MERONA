using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_EnemyData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public int Move_Type;
    public float Atk;
    public float HP;
    public float Def;
    public int Shild;
    public float Move_spd;
    public int CC_Rgs1;
    public int CC_Rgs2;
    public int CC_Rgs3;
    public int Atk_Code;
    public int Skill1Code;
    public int Skill2Code;
    public float HPSkillCast;
    public int Prefeb;

    public S_EnemyData_Excel(Monster_TableExcel origin)
    {
        No = origin.No;
        Name_KR = origin.Name_KR;
        Name_EN = origin.Name_EN;
        Code = origin.Code;
        Move_Type = origin.Move_Type;
        Atk = origin.Atk;
        HP = origin.HP;
        Def = origin.Def;
        Shild = origin.Shild;
        Move_spd = origin.Move_spd;
        CC_Rgs1 = origin.CC_Rgs1;
        CC_Rgs2 = origin.CC_Rgs1;
        CC_Rgs3 = origin.CC_Rgs3;
        Atk_Code = origin.Atk_Code;
        Skill1Code = origin.Skill1Code;
        Skill2Code = origin.Skill2Code;
        HPSkillCast = origin.HPSkillCast;
        Prefeb = origin.Prefeb;
    }
}