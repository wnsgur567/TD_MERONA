using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct S_TowerData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;
    public int Rank;
    public int Level;

    public float Atk;
    public float Atk_spd;
    public float HP;
    public float Def;
    public float Crit_rate;
    public float Crit_Dmg;

    public int Atk_Code;
    public int Skill1Code;
    public int Skill2Code;

    public int Type1;
    public int Type2;

    public int Prefeb;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Object/Tower Data")]
public class TowerData : ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_TowerData_Excel> DataList;

    private S_TowerData_Excel Read(string line)
    {
        S_TowerData_Excel data = new S_TowerData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);
        data.Rank = int.Parse(strs[idx++]);
        data.Level = int.Parse(strs[idx++]);

        data.Atk = float.Parse(strs[idx++]);
        data.Atk_spd = float.Parse(strs[idx++]);
        data.HP = float.Parse(strs[idx++]);
        data.Def = float.Parse(strs[idx++]);
        data.Crit_rate = float.Parse(strs[idx++]);
        data.Crit_Dmg = float.Parse(strs[idx++]);

        data.Atk_Code = int.Parse(strs[idx++]);
        data.Skill1Code = int.Parse(strs[idx++]);
        data.Skill2Code = int.Parse(strs[idx++]);

        data.Type1 = int.Parse(strs[idx++]);
        data.Type2 = int.Parse(strs[idx++]);

        data.Prefeb = int.Parse(strs[idx++]);

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_TowerData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_TowerData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_TowerData_Excel GetData(int code)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_TowerData_Excel);
    }
}