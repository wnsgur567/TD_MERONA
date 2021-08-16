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

            GameObject originObj = M_PrefabData.GetPrefab(PrefabCode);
            if (originObj != null)
            {
                GameObject originClone = GameObject.Instantiate(originObj);
                string key = originClone.name = originObj.name;
                Debug.Log("Skill_" + PrefabCode + ": " + key);

                Skill origin = originClone.AddComponent<Skill>();

                GameObject attackRange = origin.transform.Find("AttackRange").gameObject;
                (origin.m_SkillInfo.AttackRange = attackRange.AddComponent<AttackRange>()).Initialize();

                AddPool(key, origin, transform);

                GameObject.Destroy(originClone);
            }
        }
    }
}