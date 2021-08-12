using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPool : ObjectPool<TowerPool, Tower>
{
    public override void __Initialize()
    {
        base.__Initialize();

        for (E_Tower i = E_Tower.OrkGunner01; i < E_Tower.Max; ++i)
        {
            Tower tower = M_Resources.GetGameObject<Tower>("Tower", i.ToString());
            tower.m_TempCode = TowerManager.Instance.GetData(i).Code;
            AddPool(i.ToString(), tower, transform);
        }
    }
}
