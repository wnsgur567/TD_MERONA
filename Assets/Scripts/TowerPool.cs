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
            AddPool(i.ToString(), M_Resources.GetGameObject<Tower>("Tower", i.ToString()), transform);
        }
    }

    public enum E_Tower
    {
        OrkGunner01 = 4,
        OrkWarrior01,
        Cyclops01,
        Goblin01,
        NolWarrior01,
        TrollShamen01,
        Sparkmon01,
        Salamander01,
        LavaGolem01,
        Cerberos01,
        Balrog01,
        Minotaurus01,
        Satyr01,
        GrimReaper01,
        DeathKnight01,
        DarkSoul01,
        StoneGolem01,
        FireDemon01,
        Bat01,
        Wolf01,
        FightBear01,
        Clown01,
        FallenAngel01,
        Ipris01,
        Dragon01,
        Witch01,
        DarkElf01,

        OrkGunner02,
        OrkWarrior02,
        Cyclops02,
        Goblin02,
        NolWarrior02,
        TrollShamen02,
        Sparkmon02,
        Salamander02,
        LavaGolem02,
        Cerberos02,
        Balrog02,
        Minotaurus02,
        Satyr02,
        GrimReaper02,
        DeathKnight02,
        DarkSoul02,
        StoneGolem02,
        FireDemon02,
        Bat02,
        Wolf02,
        FightBear02,
        Clown02,
        FallenAngel02,
        Ipris02,
        Dragon02,
        Witch02,
        DarkElf02,

        OrkGunner03,
        OrkWarrior03,
        Cyclops03,
        Goblin03,
        NolWarrior03,
        TrollShamen03,
        Sparkmon03,
        Salamander03,
        LavaGolem03,
        Cerberos03,
        Balrog03,
        Minotaurus03,
        Satyr03,
        GrimReaper03,
        DeathKnight03,
        DarkSoul03,
        StoneGolem03,
        FireDemon03,
        Bat03,
        Wolf03,
        FightBear03,
        Clown03,
        FallenAngel03,
        Ipris03,
        Dragon03,
        Witch03,
        DarkElf03,

        Max
    }
}
