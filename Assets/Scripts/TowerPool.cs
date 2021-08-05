using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPool : ObjectPool<TowerPool>
{
    public override void __Initialize()
    {
        base.__Initialize();

        // ���߿� Ÿ������ ����
        m_Origin = M_Resources.GetGameObject("Tower", "TowerPrefab");

        m_Pools.Add("Tower", new MemoryPool(m_Origin, m_PoolSize, transform));
    }
}
