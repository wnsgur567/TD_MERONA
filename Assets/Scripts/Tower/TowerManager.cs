using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
    protected Tower_TableExcelLoader m_TowerData;

    protected List<Tower> m_TowerList;
    protected Dictionary<E_Direction, List<Tower>> m_DirTowerList;

    #region 내부 프로퍼티
    protected TowerPool M_TowerPool => TowerPool.Instance;
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    #endregion

    private void Awake()
    {
        m_TowerData = M_DataTable.GetDataTable<Tower_TableExcelLoader>();

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
    public Tower_TableExcel GetData(E_Tower no)
    {
        Tower_TableExcel result = m_TowerData.DataList.Where(item => item.No == (int)no).SingleOrDefault();

        return result;
    }
    public Tower_TableExcel GetData(int code)
    {
        Tower_TableExcel result = m_TowerData.DataList.Where(item => item.Code == code).SingleOrDefault();

        return result;
    }
    public List<Tower> GetTowerList(E_Direction dir)
    {
        return m_DirTowerList[dir];
    }
    public bool CheckSameTower(Tower tower1, Tower tower2)
    {
        return tower1.TowerCode == tower2.TowerCode;
    }
}
