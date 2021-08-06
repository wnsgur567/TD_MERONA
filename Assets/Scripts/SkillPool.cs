using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPool : ObjectPool<SkillPool, Skill>
{
    public override void __Initialize()
    {
        base.__Initialize();

        AddPool("Skill", M_Resources.GetGameObject<Skill>("Skill", "SkillPrefab"), transform);
    }
}