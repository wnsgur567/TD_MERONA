using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    Monster,
    King
}

[System.Serializable]
public struct S_SynergyEffect
{
    public E_SynergyEffectType EffectType;
    public E_SynergyEffectAmount EffectAmount;
    public int EffectCode;
    public E_AttackType EffectChange;
    public int EffectReq;
    public float BuffRand;
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
    public string Component;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "SynergyData", menuName = "Scriptable Object/Synergy Data")]
public class SynergyData : ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_SynergyData_Excel> DataList;

    private S_SynergyData_Excel Read(string line)
    {
        S_SynergyData_Excel data = new S_SynergyData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);
        data.Rank = int.Parse(strs[idx++]);

        data.MemReq = int.Parse(strs[idx++]);
        data.TargetMem = int.Parse(strs[idx++]);

        data.Effect1.EffectType = (E_SynergyEffectType)int.Parse(strs[idx++]);
        data.Effect1.EffectAmount = (E_SynergyEffectAmount)int.Parse(strs[idx++]);
        data.Effect1.EffectCode = int.Parse(strs[idx++]);
        data.Effect1.EffectChange = (E_AttackType)int.Parse(strs[idx++]);
        data.Effect1.EffectReq = int.Parse(strs[idx++]);
        data.Effect1.BuffRand = float.Parse(strs[idx++]);

        data.Effect2.EffectType = (E_SynergyEffectType)int.Parse(strs[idx++]);
        data.Effect2.EffectAmount = (E_SynergyEffectAmount)int.Parse(strs[idx++]);
        data.Effect2.EffectCode = int.Parse(strs[idx++]);
        data.Effect2.EffectChange = (E_AttackType)int.Parse(strs[idx++]);
        data.Effect2.EffectReq = int.Parse(strs[idx++]);
        data.Effect2.BuffRand = float.Parse(strs[idx++]);

        data.Prefab = int.Parse(strs[idx++]);
        data.Component = strs[idx++];

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_SynergyData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_SynergyData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_SynergyData_Excel? GetData(int code, int rank)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code && item.Rank == rank)
                return item;
        }

        return null;
    }
}