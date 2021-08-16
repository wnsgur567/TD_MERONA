using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Stage_EnemyManager : Singleton<Stage_EnemyManager>
{
    protected StageMonster_TableExcelLoader m_StageEnemyData;

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    private void Awake()
    {
        m_StageEnemyData = M_DataTable.GetDataTable<StageMonster_TableExcelLoader>();
    }

    public StageMonster_TableExcel GetData(int code)
    {
        StageMonster_TableExcel origin = m_StageEnemyData.DataList.Where(item => item.Code == code).Single();
        return origin;
    }
}
