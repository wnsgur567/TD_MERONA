using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum E_Devil
{
    None,

    HateQueen,
    HellLord,
    FrostLich,
}

public class DevilManager : Singleton<DevilManager>
{
    protected Tower_TableExcelLoader m_TowerData;
    protected Prefab_TableExcelLoader m_PrefabData;

    protected Devil m_Devil;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    #endregion

    #region 외부 프로퍼티
    public Devil Devil => m_Devil;
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public void SelectDevil(E_Devil no)
    {
        GameObject devil = GameObject.Instantiate(m_PrefabData.GetPrefab(GetData(no).Prefab));

        switch (no)
        {
            case E_Devil.HateQueen:
                m_Devil = devil.AddComponent<HateQueen>();
                break;
            case E_Devil.HellLord:
                m_Devil = devil.AddComponent<Devil>();
                break;
            case E_Devil.FrostLich:
                m_Devil = devil.AddComponent<Devil>();
                break;
        }
    }
    public Tower_TableExcel GetData(E_Devil no)
    {
        Tower_TableExcel result = m_TowerData.DataList.Where(item => item.No == (int)no).SingleOrDefault();

        return result;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        m_TowerData = M_DataTable.GetDataTable<Tower_TableExcelLoader>();
        m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
    }

    void Update()
    {

    }
    #endregion
}
