using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

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
        public float HP;
        public string Name;
        public float Atk;
        public float Range;
        public float Def;
        public float Atk_spd;
        public float Move_spd;
        public float Crit_rate;
        public float Crit_Dmg;
    }

    public int m_TempCode;

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

    private void Start()
    {
        E_Direction direc = E_Direction.Max;

        //���� �����ͷ� �ٲ�ߵ�
        data.HP = 100;
        data.Def = 10;
        data.Name = "Defender";
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
        if (Input.GetKeyDown(KeyCode.Space) && ty == E_Enemy.Creep1_Knight)
        {
            M_OnDivide();
        }
    }

    #region get�Լ�

    public float Get_Enemy_HP()
    {
        return data.HP;
    }

    public string Get_Enemy_Name()
    {
        return data.Name;
    }

    public float Get_Enemy_Atk()
    {
        return data.Atk;
    }

    public float Get_Enemy_Range()
    {
        return data.Range;
    }

    public float Get_Enemy_Def()
    {
        return data.Def;
    }

    public float Get_Enemy_Atk_spd()
    {
        return data.Atk_spd;
    }

    public float Get_Enemy_Move_spd()
    {
        return data.Move_spd;
    }

    public float Get_Enemy_Crit_rate()
    {
        return data.Crit_rate;
    }

    public float Get_Enemy_Crit_Dmg()
    {
        return data.Crit_Dmg;
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
    public void M_OnStun()
    {
        StartCoroutine(OnStun());
    }

    //�п�
    public void M_OnDivide()
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

    //������
    public void On_DaMage(float damage, E_BuffType type)
    {
        if (damage <= Get_Enemy_Def())
        {
            data.HP -= 1;
        }

        else
        {
            damage -= Get_Enemy_Def();
            data.HP -= damage;
        }

        image.fillAmount -= (float)(damage * 0.01);

        //Ÿ�Կ� ���� ����ġ ������ ������
        //��) �п�, �⺻
        switch (type)
        {
            case E_BuffType.Atk:
                break;
            case E_BuffType.Range:
                break;
            case E_BuffType.Def:
                break;
            case E_BuffType.Atk_spd:
                break;
            case E_BuffType.Move_spd:
                break;
            case E_BuffType.Crit_rate:
                break;
            case E_BuffType.Crit_Dmg:
                break;
            case E_BuffType.Stun:
                break;
            case E_BuffType.Dot_Dmg:
                break;
            case E_BuffType.Insta_Kill:
                break;
            case E_BuffType.CritDmg_less:
                break;
            case E_BuffType.CritDmg_more:
                break;
            case E_BuffType.Heal:
                break;
            case E_BuffType.Summon:
                break;
            case E_BuffType.Shield:
                break;
        }

        if (Get_Enemy_HP() <= 0)
        {
            On_Death();
        }
    }

    //���
    public void On_Death()
    {
        SpawnManager.Instance.Despawn(this);
        //EnemyPool.Instance.GetPool(data.Name).DeSpawn(enemy);
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
