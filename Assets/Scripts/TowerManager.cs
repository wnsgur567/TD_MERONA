using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
    public Dictionary<E_Direction, List<Tower>> m_TowerList;

    #region 내부 프로퍼티
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected TowerData TowerData
    {
        get
        {
            return M_Resources.GetScriptableObject<TowerData>("Tower", "TowerData");
        }
    }
    #endregion

    private void Awake()
    {
        m_TowerList = new Dictionary<E_Direction, List<Tower>>();
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_TowerList.Add(i, new List<Tower>());
        }
    }

    public S_TowerData_Excel GetData(int code)
    {
        return TowerData.GetData(code);
    }
    public List<Tower> GetTowerList(E_Direction dir)
    {
        return m_TowerList[dir];
    }
    public bool CheckSameTower(Tower tower1, Tower tower2)
    {
        return tower1.m_TowerInfo_Excel.Code == tower2.m_TowerInfo_Excel.Code;
    }
}
