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

    #region ���� ������Ƽ
    protected TowerManager M_Tower => TowerManager.Instance;
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    protected BuffManager M_Buff => BuffManager.Instance;
    protected NodeManager M_Node => NodeManager.Instance;
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    #endregion

    private void Awake()
    {
        m_SynergyData = M_DataTable.GetDataTable<Synergy_TableExcelLoader>();

        m_Synergys = new Dictionary<E_Direction, List<Synergy_TableExcel>>();

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys.Add(i, new List<Synergy_TableExcel>());
        }

        M_Node.m_RotateEndEvent += UpdateSynergy;
    }

    public Synergy_TableExcel GetData(int code, int rank = 1)
    {
        var datas = m_SynergyData.DataList.Where(item => item.Code == code).ToList();
        Synergy_TableExcel synergy = datas.Where(item => item.Rank == rank).SingleOrDefault();

        return synergy;
    }

    public List<Synergy_TableExcel> GetSynergy(E_Direction dir)
    {
        return m_Synergys[dir];
    }
    public void UpdateSynergy()
    {
        Debug.Log("�ó��� ������Ʈ");

        UpdateSynergy(E_Direction.North);
        UpdateSynergy(E_Direction.East);
        UpdateSynergy(E_Direction.South);
        UpdateSynergy(E_Direction.West);

        UpdateSynergyEndEvent?.Invoke();
    }
    public void UpdateSynergy(E_Direction dir)
    {
        List<Tower> towers = M_Tower.GetTowerList(dir);
        List<Enemy> enemies = new List<Enemy>();// M_Enemy.GetEnemyList(dir);

        if (towers.Count <= 0)
            return;

        // �ó��� ���� ����Ʈ �ʱ�ȭ
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys[i].Clear();
        }
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

            // ��� �߰�
            m_BonusGold = 0;
        }

        // �ó��� �ڵ�, �ó��� ����� Ÿ����
        Dictionary<int, List<Tower>> SynergyTowers = new Dictionary<int, List<Tower>>();

        for (int i = 0; i < towers.Count; ++i)
        {
            // �ó��� �ڵ� ��������
            int code1 = towers[i].SynergyCode1;
            int code2 = towers[i].SynergyCode2;
            bool flag = false;

            #region �ó���1
            if (!SynergyTowers.ContainsKey(code1))
                SynergyTowers[code1] = new List<Tower>();

            foreach (var item in SynergyTowers[code1])
            {
                if (M_Tower.CheckSameTower(item, towers[i]))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                SynergyTowers[code1].Add(towers[i]);
            #endregion

            #region �ó���2
            if (!SynergyTowers.ContainsKey(code2))
                SynergyTowers[code2] = new List<Tower>();

            flag = false;
            foreach (var item in SynergyTowers[code2])
            {
                if (M_Tower.CheckSameTower(item, towers[i]))
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
                SynergyTowers[code2].Add(towers[i]);
            #endregion
        }

        foreach (var item in SynergyTowers)
        {
            int Rank = m_MaxRank;
            int TowerCount = item.Value.Count;
            Synergy_TableExcel data = new Synergy_TableExcel();
            S_SynergyEffect effect;
            BuffCC_TableExcel buffData;

            while (true)
            {
                while (Rank >= 0)
                {
                    data = GetData(item.Key, Rank--);
                }

                if (Rank < 0)
                    break;

                if (data.MemReq <= TowerCount)
                {
                    m_Synergys[dir].Add(data);

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
                                        enemyList = enemies;
                                    }
                                    // ��ü ���� ��
                                    if (data.TargetMem == 2)
                                    {
                                        enemyList = enemies;
                                    }

                                    // �ó��� ����
                                    for (int i = 0; i < enemyList.Count; ++i)
                                    {
                                        //enemyList[i].BuffList.Add(buffData);
                                    }
                                }
                                // ���� ����
                                else if (effect.EffectAmount == E_SynergyEffectAmount.King)
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
                                if (effect.EffectAmount == E_SynergyEffectAmount.King)
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

                                }
                                // ���� ����
                                else if (effect.EffectAmount == E_SynergyEffectAmount.King)
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
                                // ����
                                if (effect.EffectAmount == E_SynergyEffectAmount.King)
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

                    break;
                }
            }
        }
    }
}
