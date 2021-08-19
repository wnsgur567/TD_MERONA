using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SynergyManager : Singleton<SynergyManager>
{
    public delegate void SynergyEventHandler();
    public event SynergyEventHandler UpdateSynergyEndEvent;

    protected Synergy_TableExcelLoader m_SynergyData;

    // �ó��� �ִ� ��ũ
    [SerializeField]
    protected int m_MaxRank = 3;
    // ��� �߰� �ó����� ���� �߰� ���
    protected int m_BonusGold;
    public int BonusGold => m_BonusGold;

    // ���⺰ �ó��� ����Ʈ
    protected Dictionary<E_Direction, List<Synergy_TableExcel>> m_Synergys = null;
    // ���⺰, �ó��� �ڵ庰 Ÿ�� ����
    protected Dictionary<E_Direction, Dictionary<int, int>> m_TowerCount = null;

    #region ���� ������Ƽ
    protected TowerManager M_Tower => TowerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected BuffManager M_Buff => BuffManager.Instance;
    protected NodeManager M_Node => NodeManager.Instance;
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected CombinationManager M_Combination => CombinationManager.Instance;
    #endregion

    private void Awake()
    {
        m_SynergyData = M_DataTable.GetDataTable<Synergy_TableExcelLoader>();

        m_Synergys = new Dictionary<E_Direction, List<Synergy_TableExcel>>();
        m_TowerCount = new Dictionary<E_Direction, Dictionary<int, int>>();
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys.Add(i, new List<Synergy_TableExcel>());
            m_TowerCount.Add(i, new Dictionary<int, int>());
        }
    }

    private void Start()
    {
        M_Node.m_RotateEndEvent += UpdateSynergy;
        M_Combination.OnCombinationCompleteEvent += UpdateSynergy_Combination;
    }

    public Synergy_TableExcel GetData(int code, int rank = 1)
    {
        var datas = m_SynergyData.DataList.Where(item => item.Code == code).ToList();
        Synergy_TableExcel synergy = datas.Where(item => item.Rank == rank).SingleOrDefault();

        return synergy;
    }
    public List<Synergy_TableExcel> GetSynergyList(E_Direction dir)
    {
        return m_Synergys[dir];
    }

    public void UpdateSynergy()
    {
        Debug.Log("�ó��� ������Ʈ");

        // �ó��� ���� ����Ʈ �ʱ�ȭ
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys[i].Clear();
        }

        // ��� �߰�
        m_BonusGold = 0;

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            UpdateSynergy(i);
            m_Synergys[i] = m_Synergys[i]
                .OrderByDescending(item => item.Rank)
                //.OrderByDescending(item => m_TowerCount[i][item.Code])
                .OrderBy(item => item.Code)
                .ToList();
        }

        UpdateSynergyEndEvent?.Invoke();
    }
    public void UpdateSynergy_Combination(bool flag)
    {
        if (flag)
        {
            UpdateSynergy();
        }
    }
    protected void LoadSynergyCode(Tower tower, ref Dictionary<int, List<Tower>> SynergyTowers)
    {
        // �ߺ� üũ��
        bool flag;
        // �ó��� �ڵ�
        int code1 = tower.SynergyCode1;
        int code2 = tower.SynergyCode2;

        #region �ó���1
        // ù üũ�� ����Ʈ �߰�
        if (!SynergyTowers.ContainsKey(code1))
        {
            SynergyTowers[code1] = new List<Tower>();
        }

        // �ߺ� Ÿ�� üũ
        flag = false;
        foreach (var item in SynergyTowers[code1])
        {
            if (item.TowerKind == tower.TowerKind)
            {
                flag = true;
                break;
            }
        }

        // �ߺ� Ÿ�� ������ �ó��� ����Ʈ�� �߰�
        if (!flag)
            SynergyTowers[code1].Add(tower);
        #endregion

        #region �ó���2
        // ù üũ�� ����Ʈ �߰�
        if (!SynergyTowers.ContainsKey(code2))
        {
            SynergyTowers[code2] = new List<Tower>();
        }

        // �ߺ� Ÿ�� üũ
        flag = false;
        foreach (var item in SynergyTowers[code2])
        {
            if (item.TowerKind == tower.TowerKind)
            {
                flag = true;
                break;
            }
        }

        // �ߺ� Ÿ�� ������ �ó��� ����Ʈ�� �߰�
        if (!flag)
            SynergyTowers[code2].Add(tower);
        #endregion
    }
    protected void UpdateSynergy(E_Direction dir)
    {
        List<Tower> towers = M_Tower.GetTowerList(dir);

        if (towers.Count <= 0)
            return;

        // �ó��� �ʱ�ȭ
        for (int i = 0; i < towers.Count; ++i)
        {
            // ����
            towers[i].m_TowerInfo.BuffList.Clear();

            // ���� Ÿ�� ����
            towers[i].m_TowerInfo.Synergy_Atk_type = E_AttackType.None;

            // ����Ŀ
            towers[i].m_TowerInfo.Berserker = false;
            towers[i].m_TowerInfo.BerserkerStack = 0;
            towers[i].m_TowerInfo.BerserkerMaxStack = 0;
            towers[i].m_TowerInfo.BerserkerBuffList.Clear();
        }

        // �ó��� �ڵ�, �ó��� ����� Ÿ����
        Dictionary<int, List<Tower>> SynergyTowers = new Dictionary<int, List<Tower>>();

        // �ó��� �ڵ� �ε�
        for (int i = 0; i < towers.Count; ++i)
        {
            LoadSynergyCode(towers[i], ref SynergyTowers);
        }

        foreach (var item in SynergyTowers)
        {
            int Rank = m_MaxRank;
            int TowerCount = item.Value.Count;
            m_TowerCount[dir][item.Key] = TowerCount;
            Synergy_TableExcel data;
            S_SynergyEffect effect;
            BuffCC_TableExcel buffData;

            while (true)
            {
                do
                {
                    data = GetData(item.Key, Rank--);
                } while (data.Code == 0 && Rank > 0);

                if (Rank < 0)
                    break;

                if (data.MemReq <= TowerCount)
                {
                    m_Synergys[dir].Add(data);

                    if (data.EffectType1 != 0)
                    {
                        effect = new S_SynergyEffect(
                            data.EffectType1,
                            data.EffectAmount1,
                            data.EffectCode1,
                            data.EffectChange1,
                            data.EffectReq1,
                            data.EffectRand1
                            );
                        buffData = M_Buff.GetData(effect.EffectCode);

                        // �ó���1 ����
                        switch (effect.EffectType)
                        {
                            case E_SynergyEffectType.Buff:
                                {
                                    // Ÿ�� ����
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = M_Tower.GetTowerList(dir);
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.BuffList.Add(buffData);
                                        }
                                    }
                                    // ���� �����
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Enemy)
                                    {
                                        // �ó��� ������ �� ����Ʈ
                                        List<Enemy> enemyList = null;

                                        // ���� ���� ��
                                        if (data.TargetMem == 1)
                                        {
                                            enemyList = M_Enemy.GetEnemyList(dir);
                                        }
                                        // ��ü ���� ��
                                        if (data.TargetMem == 2)
                                        {
                                            enemyList = M_Enemy.GetEnemyList();
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < enemyList.Count; ++i)
                                        {
                                            enemyList[i].BuffList.Add(buffData);
                                        }
                                    }
                                    // ���� ����
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.ChangeAtkType:
                                {
                                    // Ÿ��
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                            towerList[i].m_TowerInfo.BounceCount = effect.EffectReq;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.ReduceCooldown:
                                {
                                    // ����
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.Berserker:
                                {
                                    // Ÿ��
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Berserker = true;
                                            towerList[i].m_TowerInfo.BerserkerMaxStack = effect.EffectReq;
                                            towerList[i].m_TowerInfo.BerserkerBuffList.Add(buffData);
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.AddGold:
                                {
                                    m_BonusGold += effect.EffectReq;
                                }
                                break;
                        }
                    }
                    if (data.EffectType2 != 0)
                    {
                        effect = new S_SynergyEffect(
                            data.EffectType2,
                            data.EffectAmount2,
                            data.EffectCode2,
                            data.EffectChange2,
                            data.EffectReq2,
                            data.EffectRand2
                            );
                        buffData = M_Buff.GetData(effect.EffectCode);

                        // �ó���2 ����
                        switch (effect.EffectType)
                        {
                            case E_SynergyEffectType.Buff:
                                {
                                    // Ÿ�� ����
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.BuffList.Add(buffData);
                                        }
                                    }
                                    // ���� �����
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Enemy)
                                    {
                                        // �ó��� ������ �� ����Ʈ
                                        List<Enemy> enemyList = null;

                                        // ���� ���� ��
                                        if (data.TargetMem == 1)
                                        {
                                            enemyList = M_Enemy.GetEnemyList(dir);
                                        }
                                        // ��ü ���� ��
                                        if (data.TargetMem == 2)
                                        {
                                            enemyList = M_Enemy.GetEnemyList();
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < enemyList.Count; ++i)
                                        {
                                            enemyList[i].BuffList.Add(buffData);
                                        }
                                    }
                                    // ���� ����
                                    else if (effect.EffectAmount == E_SynergyEffectAmount.Devil)
                                    {

                                    }
                                }
                                break;
                            case E_SynergyEffectType.ChangeAtkType:
                                {
                                    // Ÿ��
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.ReduceCooldown:
                                {
                                    // Ÿ��
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            //towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.Berserker:
                                {
                                    // Ÿ��
                                    if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                                    {
                                        // �ó��� ������ Ÿ�� ����Ʈ
                                        List<Tower> towerList = null;

                                        // ���� �ó��� Ÿ���鸸
                                        if (data.TargetMem == 1)
                                        {
                                            towerList = item.Value;
                                        }
                                        // ���� ���� Ÿ�� ����
                                        else if (data.TargetMem == 2)
                                        {
                                            towerList = towers;
                                        }

                                        // �ó��� ����
                                        for (int i = 0; i < towerList.Count; ++i)
                                        {
                                            towerList[i].m_TowerInfo.Berserker = true;
                                            towerList[i].m_TowerInfo.BerserkerMaxStack = effect.EffectReq;
                                            towerList[i].m_TowerInfo.BerserkerBuffList.Add(buffData);
                                        }
                                    }
                                }
                                break;
                            case E_SynergyEffectType.AddGold:
                                {
                                    m_BonusGold += effect.EffectReq;
                                }
                                break;
                        }
                    }
                    break;
                }
            }
        }
    }
}

public enum E_SynergyEffectType
{
    None,

    Buff,
    ChangeAtkType,
    ReduceCooldown,
    Berserker,
    AddGold
}
public enum E_SynergyEffectAmount
{
    None,

    Tower,
    Enemy,
    Devil
}

[System.Serializable]
public struct S_SynergyEffect
{
    public E_SynergyEffectType EffectType;
    public E_SynergyEffectAmount EffectAmount;
    public int EffectCode;
    public E_AttackType EffectChange;
    public int EffectReq;
    public float EffectRand;

    public S_SynergyEffect(int effectType, int effectAmount, int effectCode, int effectChange, int effectReq, float effectRand)
    {
        EffectType = (E_SynergyEffectType)effectType;
        EffectAmount = (E_SynergyEffectAmount)effectAmount;
        EffectCode = effectCode;
        EffectChange = (E_AttackType)effectChange;
        EffectReq = effectReq;
        EffectRand = effectRand;
    }
}