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


/// //////////////////////////



[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/Monster Data")]
public class MonsterData : ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_MonsterData_Excel> DataList;

    private S_MonsterData_Excel Read(string line)
    {
        S_MonsterData_Excel data = new S_MonsterData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);

        data.Move_Type = int.Parse(strs[idx++]);

        data.Atk = float.Parse(strs[idx++]);
        data.Atk_spd = float.Parse(strs[idx++]);
        data.HP = float.Parse(strs[idx++]);
        data.Def = float.Parse(strs[idx++]);
        data.Move_spd = float.Parse(strs[idx++]);

        data.CC_Rgs1 = int.Parse(strs[idx++]);
        data.CC_Rgs2 = int.Parse(strs[idx++]);
        data.CC_Rgs3 = int.Parse(strs[idx++]);

        data.Atk_Code = int.Parse(strs[idx++]);
        data.Skill1Code = int.Parse(strs[idx++]);
        data.Skill2Code = int.Parse(strs[idx++]);

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_MonsterData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_MonsterData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_MonsterData_Excel GetData(int code)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_MonsterData_Excel);
    }
}