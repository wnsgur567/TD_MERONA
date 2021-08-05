using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-98)]
public class SynergyManager : Singleton<SynergyManager>
{
    public delegate void SynergyEventHandler();
    public event SynergyEventHandler UpdateSynergyEndEvent;

    [SerializeField]
    protected int m_MaxRank = 3;

    protected Dictionary<E_Direction, List<S_SynergyData_Excel>> m_Synergys = null;

    #region 내부 프로퍼티
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected TowerManager M_Tower => TowerManager.Instance;
    protected BuffManager M_Buff => BuffManager.Instance;
    protected NodeManager M_Node => NodeManager.Instance;
    protected SynergyData SynergyData
    {
        get
        {
            return M_Resources.GetScriptableObject<SynergyData>("Synergy", "SynergyData");
        }
    }
    #endregion

    private void Awake()
    {
        m_Synergys = new Dictionary<E_Direction, List<S_SynergyData_Excel>>();

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys.Add(i, new List<S_SynergyData_Excel>());
        }

        M_Node.RotateEndEvent += UpdateSynergy;
    }

    public S_SynergyData_Excel? GetData(int code, int rank = 1)
    {
        return SynergyData.GetData(code, rank);
    }

    public List<S_SynergyData_Excel> GetSynergy(E_Direction dir)
    {
        return m_Synergys[dir];
    }
    public void UpdateSynergy()
    {
        Debug.Log("시너지 업데이트");

        UpdateSynergy(E_Direction.North);
        UpdateSynergy(E_Direction.East);
        UpdateSynergy(E_Direction.South);
        UpdateSynergy(E_Direction.West);

        UpdateSynergyEndEvent?.Invoke();
    }
    public void UpdateSynergy(E_Direction dir)
    {
        List<Tower> towers = M_Tower.GetTowerList(dir);

        if (towers.Count <= 0)
            return;

        // 기존 시너지 초기화
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            m_Synergys[i].Clear();
        }
        for (int i = 0; i < towers.Count; ++i)
        {
            towers[i].m_TowerInfo.Buff1 = new S_Buff();
            towers[i].m_TowerInfo.Buff2 = new S_Buff();
            towers[i].m_TowerInfo.Buff3 = new S_Buff();

            towers[i].m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
        }

        // 시너지 코드, 시너지 적용될 타워들
        Dictionary<int, List<Tower>> SynergyTowers = new Dictionary<int, List<Tower>>();

        for (int i = 0; i < towers.Count; ++i)
        {
            // 시너지 코드 가져오기
            int code1 = towers[i].m_TowerInfo_Excel.Type1;
            int code2 = towers[i].m_TowerInfo_Excel.Type2;
            bool flag = false;

            #region 시너지1
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

            #region 시너지2
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
            S_SynergyData_Excel? data = null;

            while (true)
            {
                do
                {
                    data = SynergyData.GetData(item.Key, Rank--);
                    if (Rank <= 0)
                        break;
                } while (!data.HasValue);

                if (Rank <= 0)
                    break;

                S_SynergyData_Excel synergyData = data.Value;

                if (synergyData.MemReq <= TowerCount)
                {
                    m_Synergys[dir].Add(synergyData);

                    S_SynergyEffect effect = synergyData.Effect1;
                    S_BuffData_Excel buffData = M_Buff.GetData(effect.EffectCode);

                    // 시너지1 적용
                    switch (effect.EffectType)
                    {
                        case E_SynergyEffectType.Buff:
                            // 타워 버프
                            if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                            {
                                // 시너지 적용할 타워 리스트
                                List<Tower> towerList = null;

                                // 같은 시너지 타워들만
                                if (synergyData.TargetMem == 1)
                                {
                                    towerList = item.Value;
                                }
                                // 현재 라인 타워 전부
                                else if (synergyData.TargetMem == 2)
                                {
                                    towerList = towers;
                                }

                                // 시너지 적용
                                for (int i = 0; i < towerList.Count; ++i)
                                {
                                    towerList[i].m_TowerInfo.Buff1 = buffData.Buff1;
                                    towerList[i].m_TowerInfo.Buff2 = buffData.Buff2;
                                    towerList[i].m_TowerInfo.Buff3 = buffData.Buff3;
                                }
                            }
                            // 몬스터 디버프
                            else if (effect.EffectAmount == E_SynergyEffectAmount.Monster)
                            {

                            }
                            // 마왕 버프
                            else if (effect.EffectAmount == E_SynergyEffectAmount.King)
                            {

                            }
                            break;
                        case E_SynergyEffectType.ChangeAtkType:
                            // 타워
                            if (effect.EffectAmount == E_SynergyEffectAmount.Tower)
                            {
                                // 시너지 적용할 타워 리스트
                                List<Tower> towerList = null;

                                // 같은 시너지 타워들만
                                if (synergyData.TargetMem == 1)
                                {
                                    towerList = item.Value;
                                }
                                // 현재 라인 타워 전부
                                else if (synergyData.TargetMem == 2)
                                {
                                    towerList = towers;
                                }

                                // 시너지 적용
                                for (int i = 0; i < towerList.Count; ++i)
                                {
                                    towerList[i].m_TowerInfo.Synergy_Atk_type = effect.EffectChange;
                                }
                            }
                            break;
                        case E_SynergyEffectType.ReduceCooldown:
                            break;
                        case E_SynergyEffectType.Berserker:
                            break;
                        case E_SynergyEffectType.AddGold:
                            break;
                    }

                    break;
                }
            }
        }
    }
}
