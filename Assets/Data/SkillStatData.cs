using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_SkillStatData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public int Max_Charge;
    public float CoolTime;
    public float Dmg;
    public float Range;
    public float Speed;
    public int Target_num;
    public float Size;
    public float Life_Time;
    public int Buff_CC;
    public int LoadCode;

    public S_SkillStatData_Excel(SkillStat_TableExcel origin)
    {
        No = origin.No;
        Name_KR = origin.Name_KR;
        Name_EN = origin.Name_EN;
        Code = origin.Code;
        Max_Charge = origin.Max_Charge;
        CoolTime = origin.CoolTime;
        Dmg = origin.Dmg;
        Range = origin.Range;
        Speed = origin.Speed;
        Target_num = origin.Target_num;
        Size = origin.Size;
        Life_Time = origin.Life_Time;
        Buff_CC = origin.Buff_CC;
        LoadCode = origin.LoadCode;
    }
}