using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : Singleton<TowerManager>
{
    public TowerData m_TowerData;

    protected ResourcesManager M_Resources => ResourcesManager.Instance;

    private void Awake()
    {
        m_TowerData = M_Resources.GetScriptableObject<TowerData>("Tower", "TowerData");
    }
}
