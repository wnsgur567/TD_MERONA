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
    protected NodeManager M_Node => NodeManager.Instance;
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

        M_Node.m_RotateEndEvent += UpdateTowerList;
    }

    public Tower SpawnTower(E_Tower tower)
    {
        Tower spawn = M_TowerPool.GetPool(GetData(tower).Name_EN).Spawn();
        m_TowerList.Add(spawn);
        return spawn;
    }
    public void AddTower(Tower tower, E_Direction dir)
    {
        m_DirTowerList[dir].Add(tower);
    }
    //public Tower SpawnTower(E_Tower tower, E_Direction dir)
    //{
    //    Tower spawn = SpawnTower(tower);
    //    m_DirTowerList[dir].Add(spawn);
    //    spawn.m_TowerInfo.Direction = dir;
    //    return spawn;
    //}

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
    public List<Tower> GetTowerList()
    {
        return m_TowerList;
    }
    public List<Tower> GetTowerList(E_Direction dir)
    {
        return m_DirTowerList[dir];
    }
    public void UpdateTowerList()
    {
        for (E_Direction i = 0; i < E_Direction.Max; i++)
        {
            m_DirTowerList[i].Clear();
            UpdateTowerList(i);
        }
    }
    public void UpdateTowerList(E_Direction dir)
    {
        List<Node> nodeList = M_Node.GetNodeList(dir);

        foreach (var item in nodeList)
        {
            if (item.m_Tower != null)
            {
                m_DirTowerList[dir].Add(item.m_Tower);
            }
        }
    }

    public bool CheckSameTower(Tower tower1, Tower tower2)
    {
        return tower1.TowerCode == tower2.TowerCode;
    }
}

public enum E_Tower
{
    None,

    // 타워
    OrkGunner01 = 4,
    OrkWarrior01,
    Cyclops01,
    Goblin01,
    NolWarrior01,
    TrollShamen01,
    Sparkmon01,
    Salamander01,
    LavaGolem01,
    Cerberos01,
    Balrog01,
    Minotaurus01,
    Satyr01,
    GrimReaper01,
    DeathKnight01,
    DarkSoul01,
    StoneGolem01,
    FireDemon01,
    Bat01,
    Wolf01,
    FightBear01,
    Clown01,
    FallenAngel01,
    Ipris01,
    Dragon02,
    Witch01,
    DarkElf01,

    OrkGunner02,
    OrkWarrior02,
    Cyclops02,
    Goblin02,
    NolWarrior02,
    TrollShamen02,
    Sparkmon02,
    Salamander02,
    LavaGolem02,
    Cerberos02,
    Balrog02,
    Minotaurus02,
    Satyr02,
    GrimReaper02,
    DeathKnight02,
    DarkSoul02,
    StoneGolem02,
    FireDemon02,
    Bat02,
    Wolf02,
    FightBear02,
    Clown02,
    FallenAngel02,
    Ipris02,
    Dragon03,
    Witch02,
    DarkElf02,

    OrkGunner03,
    OrkWarrior03,
    Cyclops03,
    Goblin04,
    NolWarrior03,
    TrollShamen03,
    Sparkmon03,
    Salamander03,
    LavaGolem03,
    Cerberos03,
    Balrog03,
    Minotaurus03,
    Satyr03,
    GrimReaper03,
    DeathKnight03,
    DarkSoul03,
    StoneGolem03,
    FireDemon03,
    Bat03,
    Wolf03,
    FightBear03,
    Clown03,
    FallenAngel03,
    Ipris03,
    Dragon04,
    Witch03,
    DarkElf03,

    Max
}