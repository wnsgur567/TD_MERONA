using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-98)]
public class TowerManager : Singleton<TowerManager>
{
    protected List<Tower> m_TowerList;
    protected Dictionary<E_Direction, List<Tower>> m_DirTowerList;

    #region 내부 프로퍼티
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected TowerPool M_TowerPool => TowerPool.Instance;
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
        m_TowerList = new List<Tower>();
        m_DirTowerList = new Dictionary<E_Direction, List<Tower>>();
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_DirTowerList.Add(i, new List<Tower>());
        }
    }

    public Tower SpawnTower(E_Tower tower)
    {
        Tower spawn = M_TowerPool.GetPool(tower.ToString()).Spawn();
        m_TowerList.Add(spawn);
        return spawn;
    }
    public Tower SpawnTower(E_Tower tower, E_Direction dir)
    {
        Tower spawn = SpawnTower(tower);
        m_DirTowerList[dir].Add(spawn);
        return spawn;
    }
    public S_TowerData_Excel GetData(E_Tower no)
    {
        foreach (var item in TowerData.DataList)
        {
            if (item.No == no)
                return item;
        }

        return default(S_TowerData_Excel);
    }
    public S_TowerData_Excel GetData(int code)
    {
        foreach (var item in TowerData.DataList)
        {
            if (item.Code == code)
                return item;
        }

        return default(S_TowerData_Excel);
    }
    public List<Tower> GetTowerList(E_Direction dir)
    {
        return m_DirTowerList[dir];
    }
    public bool CheckSameTower(Tower tower1, Tower tower2)
    {
        return tower1.m_TowerInfo_Excel.Code == tower2.m_TowerInfo_Excel.Code;
    }
}
