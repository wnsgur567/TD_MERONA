using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Enum
// 스킬 이동 타입
public enum E_MoveType
{
    None,

    Straight, // 직선 이동
    Curve // 곡선 이동
}
// 스킬 공격 타입
public enum E_AttackType
{
    None,

    NormalFire, // 일반 공격
    FixedFire, // 지점 공격
    PenetrateFire, // 관통 공격
    BounceFire // 튕기는 공격
}
// 스킬 발사 타입
public enum E_FireType
{
    None,

    Select_point,
    Select_self,
    Select_enemy
}
// 타겟 선정 타입
public enum E_TargetType
{
    None,

    CloseTarget,
    RandTarget,
    FixTarget,
    TileTarget
}
#endregion

[System.Serializable]
public struct S_SkillConditionData_Excel
{
    public int No;
    public string Name_KR;
    public string Name_EN;
    public int Code;

    public int Ally;
    public int Air_Attack;

    public E_AttackType Atk_type;
    public E_FireType Atk_pick;
    public E_TargetType Target_type;

    public E_MoveType Move_type;
    public int PassiveCode;

    public int Atk_prefab;
    public int projectile_prefab;
    public int damage_prefab;

    // 비고
    public string Component;
}


/// //////////////////////////



[CreateAssetMenu(fileName = "SkillConditionData", menuName = "Scriptable Object/Skill Condition Data")]
public class SkillConditionData: ScriptableObject
{
    [SerializeField] string filepath;
    [SerializeField] List<S_SkillConditionData_Excel> DataList;

    private S_SkillConditionData_Excel Read(string line)
    {
        S_SkillConditionData_Excel data = new S_SkillConditionData_Excel();
        int idx = 0;

        string[] strs = line.Split(',');

        data.No = int.Parse(strs[idx++]);
        data.Name_KR = strs[idx++];
        data.Name_EN = strs[idx++];
        data.Code = int.Parse(strs[idx++]);

        data.Ally = int.Parse(strs[idx++]);
        data.Air_Attack = int.Parse(strs[idx++]);

        data.Atk_type = (E_AttackType)int.Parse(strs[idx++]);
        data.Atk_pick = (E_FireType)int.Parse(strs[idx++]);
        data.Target_type = (E_TargetType)int.Parse(strs[idx++]);

        data.Move_type = (E_MoveType)int.Parse(strs[idx++]);
        data.PassiveCode = int.Parse(strs[idx++]);

        data.Atk_prefab = int.Parse(strs[idx++]);
        data.projectile_prefab = int.Parse(strs[idx++]);
        data.damage_prefab = int.Parse(strs[idx++]);

        data.Component = strs[idx++];

        return data;
    }

    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<S_SkillConditionData_Excel>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        Debug.Log(currentpath);
        Debug.Log(System.IO.Path.Combine(currentpath, filepath));

        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split('\n');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;
            S_SkillConditionData_Excel data = Read(item);
            DataList.Add(data);
        }
    }
    public S_SkillConditionData_Excel GetData(int code)
    {
        foreach (var item in DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_SkillConditionData_Excel);
    }
}