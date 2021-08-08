using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Tower
{
    None,

    // 마왕
    HateQueen = 1,
    HellLord,
    FrostLich,

    // 타워
    OrkGunner01 = 4,
    OrkWarrior01,
    Cyclops01,
    Goblin01,
    NolWarrior01,
    TrollShamen01,
    Sparkmon01,
    Salamander01,
    LavaGolem01,
    Cerberos01,
    Balrog01,
    Minotaurus01,
    Satyr01,
    GrimReaper01,
    DeathKnight01,
    DarkSoul01,
    StoneGolem01,
    FireDemon01,
    Bat01,
    Wolf01,
    FightBear01,
    Clown01,
    FallenAngel01,
    Ipris01,
    Dragon01,
    Witch01,
    DarkElf01,

    OrkGunner02,
    OrkWarrior02,
    Cyclops02,
    Goblin02,
    NolWarrior02,
    TrollShamen02,
    Sparkmon02,
    Salamander02,
    LavaGolem02,
    Cerberos02,
    Balrog02,
    Minotaurus02,
    Satyr02,
    GrimReaper02,
    DeathKnight02,
    DarkSoul02,
    StoneGolem02,
    FireDemon02,
    Bat02,
    Wolf02,
    FightBear02,
    Clown02,
    FallenAngel02,
    Ipris02,
    Dragon02,
    Witch02,
    DarkElf02,

    OrkGunner03,
    OrkWarrior03,
    Cyclops03,
    Goblin03,
    NolWarrior03,
    TrollShamen03,
    Sparkmon03,
    Salamander03,
    LavaGolem03,
    Cerberos03,
    Balrog03,
    Minotaurus03,
    Satyr03,
    GrimReaper03,
    DeathKnight03,
    DarkSoul03,
    StoneGolem03,
    FireDemon03,
    Bat03,
    Wolf03,
    FightBear03,
    Clown03,
    FallenAngel03,
    Ipris03,
    Dragon03,
    Witch03,
    DarkElf03,

    Max
}

[System.Serializable]
public struct S_TowerData_Excel
{
    public E_Tower No;
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

    public float Price;
    public int Prefeb;

    public string Component;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "TowerData", menuName = "Scriptable Object/Tower Data")]
public class TowerData : ScriptableObject
{
    [SerializeField] string filepath;
    public List<S_TowerData_Excel> DataList;

    private S_TowerData_Excel Read(string line)
    {
        S_TowerData_Excel data = new S_TowerData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = (E_Tower)int.Parse(strs[idx++]);
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

        data.Price = float.Parse(strs[idx++]);
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
}