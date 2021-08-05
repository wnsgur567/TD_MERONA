using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPool : ObjectPool<SkillPool>
{
    public override void __Initialize()
    {
        base.__Initialize();

        m_Origin = M_Resources.GetGameObject("Skill", "SkillPrefab");

        m_Pools.Add("Skill", new MemoryPool(m_Origin, m_PoolSize, transform));
    }
}