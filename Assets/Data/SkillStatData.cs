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

    public int LoadCode;
    public int LoadBuff;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "SkillStatData", menuName = "Scriptable Object/Skill Stat Data")]
public class SkillStatData : ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_SkillStatData_Excel> DataList;

    private S_SkillStatData_Excel Read(string line)
    {
        S_SkillStatData_Excel data = new S_SkillStatData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);

        data.Max_Charge = int.Parse(strs[idx++]);
        data.CoolTime = float.Parse(strs[idx++]);

        data.Dmg = float.Parse(strs[idx++]);
        data.Range = float.Parse(strs[idx++]);
        data.Speed = float.Parse(strs[idx++]);
        data.Target_num = int.Parse(strs[idx++]);

        data.Size = float.Parse(strs[idx++]);
        data.Life_Time = float.Parse(strs[idx++]);

        data.LoadCode = int.Parse(strs[idx++]);
        data.LoadBuff = int.Parse(strs[idx++]);

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_SkillStatData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_SkillStatData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_SkillStatData_Excel GetData(int code)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_SkillStatData_Excel);
    }
}