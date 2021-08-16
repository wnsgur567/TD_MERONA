using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuffManager : Singleton<BuffManager>
{
    protected BuffCC_TableExcelLoader m_BuffCCData;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public BuffCC_TableExcel GetData(int code)
    {
        BuffCC_TableExcel result = m_BuffCCData.DataList.Where(item => item.Code == code).Single();

        return result;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        m_BuffCCData = M_DataTable.GetDataTable<BuffCC_TableExcelLoader>();
    }
    #endregion
}