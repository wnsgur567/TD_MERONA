using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPool : ObjectPool<SkillPool, Skill>
{
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected SkillCondition_TableExcelLoader M_SkillConditionData => M_DataTable.GetDataTable<SkillCondition_TableExcelLoader>();
    protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

    public override void __Initialize()
    {
        base.__Initialize();

        for (int i = 0; i < M_SkillConditionData.DataList.Count; ++i)
        {
            int PrefabCode = M_SkillConditionData.DataList[i].projectile_prefab;
            string key = PrefabCode.ToString();
            Skill origin = M_PrefabData.GetPrefab(PrefabCode).GetComponent<Skill>();
            AddPool(key, origin, transform);
        }
    }
}