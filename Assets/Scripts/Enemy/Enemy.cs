using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum E_Enemy
{
    None,

    Creep1_Knight = 1,
    Creep2_Fighter,
    Creep4_Wizard,
    Creep5_Templar,
    Creep6_Bowman,
    Creep_Rogue,

    Defender01,
    Defender02,
    Defender03,
    Defender04,

    DwarfWarrior01,
    DwarfWarrior02,
    DwarfWarrior03,
    DwarfWarrior04,

    Max
}

public class Enemy : MonoBehaviour
{
    [Serializable]
    public struct Enemy_Data
    {
        public int No;
        public string Name_KR;
        public string Name_EN;
        public int Code;
        public int Move_Type;
        public float Atk;
        public float HP;
        public float Def;
        public int Shild;
        public float Move_spd;
        public int CC_Rgs1;
        public int CC_Rgs2;
        public int CC_Rgs3;
        public int Atk_Code;
        public int Skill1Code;
        public int Skill2Code;
        public float HPSkillCast;
        public int Prefeb;
    }

    //���� �ٶ󺸰� �ִ� waypoint
    private Transform target;

    private int waypointIndex = 0;

    private E_Direction direc;

    //���� ������
    public E_Enemy ty;

    private Animator animator;

    private bool isStun = false;

    //EnemyScriptable���� �����;ߵ�
    private Enemy_Data data;

    //ü�¹�
    public Image image;

    #region �ó��� ����
    // ����
    public List<BuffCC_TableExcel> BuffList;
    #endregion

    private void Start()
    {
        E_Direction direc = E_Direction.Max;

        //���� �����ͷ� �ٲ�ߵ�
        data.HP = 100;
        data.Def = 10;
        data.Name_EN = "Defender";
        data.Move_spd = 10f;

        image.fillAmount = data.HP;

        animator = GetComponent<Animator>();

        switch (ty)
        {
            case E_Enemy.Creep1_Knight:
            case E_Enemy.Creep2_Fighter:
            case E_Enemy.Creep4_Wizard:
            case E_Enemy.Creep5_Templar:
            case E_Enemy.Creep6_Bowman:
            case E_Enemy.Creep_Rogue:
            case E_Enemy.Defender01:
            case E_Enemy.Defender02:
            case E_Enemy.Defender03:
            case E_Enemy.Defender04:
            case E_Enemy.DwarfWarrior01:
            case E_Enemy.DwarfWarrior02:
            case E_Enemy.DwarfWarrior03:
            case E_Enemy.DwarfWarrior04:
                switch (direc)
                {
                    case E_Direction.East:
                        target = EastWayPoints.points[0];
                        break;

                    case E_Direction.West:
                        target = WestWayPoints.points[0];
                        break;

                    case E_Direction.South:
                        target = SouthWayPoints.points[0];
                        break;

                    case E_Direction.North:
                        target = NorthWayPoints.points[0];
                        break;
                }
                break;
        }
    }

    private void Update()
    {
        if (!isStun)
        {
            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * data.Move_spd * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                GetNextWayPoint();
            }
        }

        //HP�� ���� �������� �������� �ٲ�ߵ�
        //Ÿ�Ե� ������ߵ� �п��Ǵ� ���͸� if������
        if (Input.GetKeyDown(KeyCode.Space) && ty == E_Enemy.Creep1_Knight)
        {
            On_Divide();
        }

        if (Get_Enemy_HP() <= 0)
        {
            On_Death();
        }
    }

    #region get�Լ�

    public int Get_EnemyNo()
    {
        return data.No;
    }

    public string Get_EnemyName_KR()
    {
        return data.Name_KR;
    }

    public string Get_EnemyName_EN()
    {
        return data.Name_EN;
    }

    public int Get_EnemyCode()
    {
        return data.Code;
    }

    public int Get_EnemyMove_Type()
    {
        return data.Move_Type;
    }

    public float Get_EnemyAtk()
    {
        return data.Atk;
    }

    public float Get_Enemy_HP()
    {
        return data.HP;
    }

    public float Get_EnemyDef()
    {
        return data.Def;
    }

    public int Get_EnemyShild()
    {
        return data.Shild;
    }

    public float Get_EnemyMove_spd()
    {
        return data.Move_spd;
    }

    public int Get_EnemyCC_Rgs1()
    {
        return data.CC_Rgs1;
    }

    public int Get_EnemyCC_Rgs2()
    {
        return data.CC_Rgs2;
    }

    public int Get_EnemyCC_Rgs3()
    {
        return data.CC_Rgs3;
    }

    public int Get_EnemyAtk_Code()
    {
        return data.Atk_Code;
    }

    public int Get_EnemySkill1Code()
    {
        return data.Skill1Code;
    }

    public int Get_EnemySkill2Code()
    {
        return data.Skill2Code;
    }

    //ü�º��ȿ��
    public float Get_EnemyHPSkillCast()
    {
        return data.HPSkillCast;
    }

    public int Get_EnemyPrefeb()
    {
        return data.Prefeb;
    }

    public int Get_WayPointIndex()
    {
        return waypointIndex;
    }

    public E_Direction Get_Direction()
    {
        return direc;
    }

    #endregion

    #region �ܺ� �Լ�
    // ����
    public void On_Stun()
    {
        StartCoroutine(OnStun());
    }

    //�п�
    public void On_Divide()
    {
        StartCoroutine(Divide());
    }

    //��������
    public void InitSetting(E_Direction p_waypoint)
    {
        direc = p_waypoint;
    }

    //�п��� ���� ������ ������
    //��������
    //�ٶ󺸰� �ִ� waypoint
    //waypointindex��
    public void InitSetting(E_Direction p_waypoint, Transform _target, int _waypointindex)
    {
        direc = p_waypoint;
        target = _target;
        waypointIndex = _waypointindex;
    }

    //���
    public void On_Death()
    {
        SpawnManager.Instance.Despawn(this);
        //EnemyPool.Instance.GetPool(data.Name).DeSpawn(enemy);
    }

    //������
    public void On_DaMage(float damage, List<BuffCC_TableExcel> m_bufflist, E_BuffType type = E_BuffType.None)
    {
        if (damage <= Get_EnemyDef())
        {
            data.HP -= 1;
        }

        else
        {
            damage -= Get_EnemyDef();
            data.HP -= damage;
        }

        image.fillAmount -= (float)(damage * 0.01);

        // �ó��� ����
        BuffList = m_bufflist;
        // ���� ���� Ȯ��
        List<float> BuffRand = new List<float>();
        // ���� ���� ����
        List<bool> BuffApply = new List<bool>();
        // ���� ���� ���
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        S_Buff buff;

        for (int i = 0; i < BuffList.Count; ++i)
        {
            // ����1 üũ
            if (BuffApply[i * 3])
            {
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
                float BuffAmount = buff.BuffAmount;

                //Atk, 1 -
                //Range, 2
                //Def, 3 -
                //Atk_spd, 4 -
                //Move_spd, 5 -
                //Crit_rate, 6
                //Crit_Dmg, 7

                //Stun, 8 -
                //Dot_Dmg, 9 -
                //Insta_Kill, 10 -
                //CritDmg_less, 11 -
                //CritDmg_more, 12 -

                //Heal, 13
                //Summon, 14
                //Shield 15

                // ����1 �տ���
                if (buff.AddType == E_AddType.Fix)
                {
                    switch (buff.BuffType)
                    {
                        case E_BuffType.Def:
                            data.Def *= BuffAmount;
                            break;
                        case E_BuffType.Stun:
                            On_Stun();
                            break;
                        case E_BuffType.Insta_Kill:
                            On_Death();
                            break;
                        case E_BuffType.Move_spd:
                            data.Move_spd *= BuffAmount;
                            break;
                        case E_BuffType.Dot_Dmg:
                            data.HP -= BuffAmount;
                            break;
                    }
                }

                // ����2 üũ
                if (BuffApply[i * 3 + 1])
                {
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
                    BuffAmount = buff.BuffAmount;

                    // ����2 �տ���
                    if (buff.AddType == E_AddType.Fix)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Dot_Dmg:
                                data.HP -= BuffAmount;
                                break;
                        }
                    }

                    // ����3 üũ
                    if (BuffApply[i * 3 + 2])
                    {
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
                        BuffAmount = buff.BuffAmount;

                        // ����3 �տ���
                        if (buff.AddType == E_AddType.Fix)
                        {
                            switch (buff.BuffType)
                            {
                                
                            }
                        }
                    }
                }
            }
            //Ÿ�Կ� ���� ����ġ ������ ������
            //��) �п�, �⺻
            //switch (type)
            //{
            //    case E_BuffType.Atk:
            //        break;
            //    case E_BuffType.Range:
            //        break;
            //    case E_BuffType.Def:
            //        break;
            //    case E_BuffType.Atk_spd:
            //        break;
            //    case E_BuffType.Move_spd:
            //        break;
            //    case E_BuffType.Crit_rate:
            //        break;
            //    case E_BuffType.Crit_Dmg:
            //        break;
            //    case E_BuffType.Stun:
            //        break;
            //    case E_BuffType.Dot_Dmg:
            //        break;
            //    case E_BuffType.Insta_Kill:
            //        break;
            //    case E_BuffType.CritDmg_less:
            //        break;
            //    case E_BuffType.CritDmg_more:
            //        break;
            //    case E_BuffType.Heal:
            //        break;
            //    case E_BuffType.Summon:
            //        break;
            //    case E_BuffType.Shield:
            //        break;
            //}

            if (Get_Enemy_HP() <= 0)
            {
                On_Death();
            }
        }
    }


    #endregion

    #region ���� �Լ�

    //���� waypoint ����
    private void GetNextWayPoint()
    {
        switch (direc)
        {
            case E_Direction.East:
                if (waypointIndex >= EastWayPoints.points.Length - 1)
                {
                    transform.position = EastWayPoints.points[EastWayPoints.points.Length - 1].position;
                    animator.SetBool("Attack", true);
                }

                else
                {
                    waypointIndex++;
                    target = EastWayPoints.points[waypointIndex];
                    transform.LookAt(target);
                }
                break;

            case E_Direction.West:
                if (waypointIndex >= WestWayPoints.points.Length - 1)
                {
                    transform.position = WestWayPoints.points[WestWayPoints.points.Length - 1].position;
                    animator.SetBool("Attack", true);
                }

                else
                {
                    waypointIndex++;
                    target = WestWayPoints.points[waypointIndex];
                    transform.LookAt(target);
                }
                break;

            case E_Direction.South:
                if (waypointIndex >= SouthWayPoints.points.Length - 1)
                {
                    transform.position = SouthWayPoints.points[SouthWayPoints.points.Length - 1].position;
                    animator.SetBool("Attack", true);
                }

                else
                {
                    waypointIndex++;
                    target = SouthWayPoints.points[waypointIndex];
                    transform.LookAt(target);
                }
                break;

            case E_Direction.North:
                if (waypointIndex >= NorthWayPoints.points.Length - 1)
                {
                    transform.position = NorthWayPoints.points[NorthWayPoints.points.Length - 1].position;
                    animator.SetBool("Attack", true);
                }

                else
                {
                    waypointIndex++;
                    target = NorthWayPoints.points[waypointIndex];
                    transform.LookAt(target);
                }
                break;
        }
    }
    #endregion

    #region �ڷ�ƾ
    //����
    //�ִϸ��̼� �߰�
    IEnumerator OnStun()
    {
        isStun = true;

        yield return new WaitForSeconds(1f);

        isStun = false;
    }

    //�п�
    IEnumerator Divide()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector3 pos = transform.localPosition;

            if (i == 0)
            {
                pos.x = pos.x - 0.2f;
                pos.z = pos.z - 0.2f;
            }

            else
            {
                pos.x = pos.x + 0.2f;
                pos.z = pos.z + 0.2f;
            }

            SpawnManager.Instance.SpawnEnemy(direc, pos, target, waypointIndex);

            yield return null;
        }

        Destroy(gameObject);
    }
    #endregion
}