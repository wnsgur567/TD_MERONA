using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPool : ObjectPool<EnemyPool, Enemy>
{
    public override void __Initialize()
    {
        base.__Initialize();

        for (E_Enemy i = E_Enemy.Creep1_Knight; i < E_Enemy.Max; ++i)
        {
            Enemy enemy = M_Resources.GetGameObject<Enemy>("Enemy", i.ToString());
            //enemy.m_TempCode = EnemyManager.Instance.GetData(i).Code;
            AddPool(i.ToString(), enemy, transform);
        }
    }
}
