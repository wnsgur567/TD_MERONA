using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string Component;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Object/Buff Data")]
public class BuffData : ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_BuffData_Excel> DataList;

    private S_BuffData_Excel Read(string line)
    {
        S_BuffData_Excel data = new S_BuffData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);
        
        data.Buff1.BuffType = (E_BuffType)int.Parse(strs[idx++]);
        data.Buff1.AddType = (E_AddType)int.Parse(strs[idx++]);
        data.Buff1.BuffAmount = float.Parse(strs[idx++]);
        data.Buff1.BuffRand = float.Parse(strs[idx++]);
        data.Buff1.Summon = int.Parse(strs[idx++]);

        data.Buff2.BuffType = (E_BuffType)int.Parse(strs[idx++]);
        data.Buff2.AddType = (E_AddType)int.Parse(strs[idx++]);
        data.Buff2.BuffAmount = float.Parse(strs[idx++]);
        data.Buff2.BuffRand = float.Parse(strs[idx++]);
        data.Buff2.Summon = int.Parse(strs[idx++]);

        data.Buff3.BuffType = (E_BuffType)int.Parse(strs[idx++]);
        data.Buff3.AddType = (E_AddType)int.Parse(strs[idx++]);
        data.Buff3.BuffAmount = float.Parse(strs[idx++]);
        data.Buff3.BuffRand = float.Parse(strs[idx++]);
        data.Buff3.Summon = int.Parse(strs[idx++]);

        data.Duration = float.Parse(strs[idx++]);

        data.Prefab = int.Parse(strs[idx++]);
        data.Component = strs[idx++];

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_BuffData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_BuffData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_BuffData_Excel GetData(int code)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_BuffData_Excel);
    }
}