using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerPool : ObjectPool<TowerPool, Tower>
{
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected Tower_TableExcelLoader M_TowerData => M_DataTable.GetDataTable<Tower_TableExcelLoader>();
    protected Prefab_TableExcelLoader M_PrefabData => M_DataTable.GetDataTable<Prefab_TableExcelLoader>();

    public override void __Initialize()
    {
        base.__Initialize();

        for (int i = 3; i < M_TowerData.DataList.Count; ++i)
        {
            int PrefabCode = M_TowerData.DataList[i].Prefeb;
            string key = PrefabCode.ToString();
            Tower origin = M_PrefabData.GetPrefab(PrefabCode).GetComponent<Tower>();
            AddPool(key, origin, transform);
        }

        //for (E_Tower i = E_Tower.OrkGunner01; i < E_Tower.Max; ++i)
        //{
        //    S_TowerData_Excel data = M_Tower.GetData(i);
        //    Tower tower = M_Resources.GetGameObject<Tower>("Tower", i.ToString());
        //    tower.m_TempCode = data.Code;
        //    AddPool(data.Prefeb.ToString(), tower, transform);
        //}
    }
}
