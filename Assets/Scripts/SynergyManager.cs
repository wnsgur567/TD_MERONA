using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyManager : Singleton<SynergyManager>
{
    public List<S_SynergyData> m_SynergyDataList;

    #region 내부 프로퍼티
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    protected SynergyData Synergy
    {
        get
        {
            return M_Resources.GetScriptableObject<SynergyData>("Synergy", "SynergyData");
        }
    }
    #endregion

    private void Awake()
    {
        if (null == m_SynergyDataList)
            m_SynergyDataList = new List<S_SynergyData>();

        for (int i = 0; i < (int)E_Direction.Max; ++i)
        {
            S_SynergyData data = new S_SynergyData();
            data.m_Direction = (E_Direction)i;
            data.m_TowerList = new List<Tower>();
            data.m_Synergy = new Dictionary<int, int>();
            m_SynergyDataList.Add(data);
        }
    }

    private void Start()
    {
        GetSynergy(E_Direction.North);
    }

    public void GetSynergy(E_Direction dir)
    {
        List<Tower> towers = m_SynergyDataList[(int)dir].m_TowerList;

        SynergyData synergyData = M_Resources.GetScriptableObject<SynergyData>("Synergy", "SynergyData");
        Dictionary<int, int> Synergys = new Dictionary<int, int>();//m_SynergyDataList[(int)dir].m_Synergy;

        for (int i = 0; i < towers.Count; ++i)
        {
            int type1 = towers[i].m_TowerInfo_Excel.Type1;
            int type2 = towers[i].m_TowerInfo_Excel.Type2;

            if (!Synergys.ContainsKey(type1))
                Synergys.Add(type1, 0);

            ++Synergys[type1];

            if (type1 != type2)
            {
                if (!Synergys.ContainsKey(type2))
                    Synergys.Add(type2, 0);

                ++Synergys[type2];
            }
        }

        foreach (var item in Synergys)
        {
            S_SynergyData_Excel SynergyData = synergyData.GetData(item.Key);

            if (SynergyData.MemReq <= item.Value)
            {
                Debug.Log(item.Key + "시너지 적용");
            }
        }
    }

    [System.Serializable]
    public struct S_SynergyData
    {
        public E_Direction m_Direction;
        public List<Tower> m_TowerList;
        public Dictionary<int, int> m_Synergy;
    }
}
