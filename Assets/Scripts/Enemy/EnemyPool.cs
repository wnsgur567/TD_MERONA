using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyPool, Enemy>
{
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected Enemy_TableExcelLoader M_EnemyData => M_DataTable.GetDataTable<Enemy_TableExcelLoader>();
    protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

    public override void __Initialize()
    {
        base.__Initialize();

        for (int i = 3; i < M_EnemyData.DataList.Count; ++i)
        {
            int PrefabCode = M_EnemyData.DataList[i].Prefab;
            string key = PrefabCode.ToString();
            Enemy origin = M_PrefabData.GetPrefab(PrefabCode).GetComponent<Enemy>();
            AddPool(key, origin, transform);
        }

        //for (E_Enemy i = E_Enemy.Creep1_Knight; i < E_Enemy.Max; ++i)
        //{
        //    Enemy enemy = M_Resources.GetGameObject<Enemy>("Enemy", i.ToString());
        //    enemy.m_TempCode = EnemyManager.Instance.GetData(i).Code;
        //    AddPool(i.ToString(), enemy, transform);
        //}
    }
}
