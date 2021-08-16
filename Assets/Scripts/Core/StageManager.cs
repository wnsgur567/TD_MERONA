using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    protected Stage_TableExcelLoader m_StageData;

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    private void Awake()
    {
        m_StageData = M_DataTable.GetDataTable<Stage_TableExcelLoader>();
    }

    public Stage_TableExcel GetData(int code)
    {
        Stage_TableExcel origin = m_StageData.DataList.Where(item => item.Code == code).Single();
        return origin;
    }

    public List<Stage_TableExcel> GetListData(int code)
    {
        List<Stage_TableExcel> origin = new List<Stage_TableExcel>();

        for (int i = 0; i < m_StageData.DataList.Count; i++)
        {
            if (m_StageData.DataList[i].Code == code)
                origin.Add(m_StageData.DataList[i]);
        }

        return origin;
    }
}
