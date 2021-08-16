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
        public string Name_EN;
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

    #region Get프로퍼티

    public string Get_EnemyName_EN => m_EnemyInfo.Name_EN;

    public int Get_EnemyMove_Type => m_EnemyInfo.Move_Type;

    public float Get_EnemyAtk => m_EnemyInfo.Atk;

    public float Get_EnemyHP => m_EnemyInfo.HP;

    public float Get_EnemyDef => m_EnemyInfo.Def;

    public int Get_EnemyShild => m_EnemyInfo.Shild;

    public float Get_EnemyMove_spd => m_EnemyInfo.Move_spd;

    public int Get_EnemyCC_Rgs1 => m_EnemyInfo.CC_Rgs1;

    public int Get_EnemyCC_Rgs2 => m_EnemyInfo.CC_Rgs2;

    public int Get_EnemyCC_Rgs3 => m_EnemyInfo.CC_Rgs3;

    public int Get_EnemyAtk_Code => m_EnemyInfo.Atk_Code;

    public int Get_EnemySkill1Code => m_EnemyInfo.Skill1Code;

    public int Get_EnemySkill2Code => m_EnemyInfo.Skill2Code;

    //체력비례효과
    public float Get_EnemyHPSkillCast => m_EnemyInfo.HPSkillCast;

    public int Get_EnemyPrefeb => m_EnemyInfo.Prefeb;

    public int Get_WayPointIndex => waypointIndex;

    public E_Direction Get_Direction => direc;

    #endregion

    //현재 바라보고 있는 waypoint
    private Transform target;

    private int waypointIndex = 0;

    private E_Direction direc;

    public E_Enemy ty;

    private Animator animator;

    private bool isStun = false;

    private Enemy_Data m_EnemyInfo;

    //체력바
    public Image image;

    private Enemy_TableExcel m_Enemyinfo_Excel;
    private EnemyManager M_Enemy;

    #region 시너지 관련
    // 버프
    public List<BuffCC_TableExcel> BuffList;
    #endregion

    private void Start()
    {
        E_Direction direc = E_Direction.Max;

        image.fillAmount = m_EnemyInfo.HP;

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
            transform.Translate(dir.normalized * m_EnemyInfo.Move_spd * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                GetNextWayPoint();
            }
        }

        //HP가 전부 없어지는 조건으로 바꿔야됨
        //타입도 정해줘야됨 분열되는 몬스터만 if문으로
        if (Input.GetKeyDown(KeyCode.Space) && ty == E_Enemy.Creep1_Knight)
        {
            //On_Divide();
        }

        if (m_EnemyInfo.HP <= 0)
        {
            On_Death();
        }
    }

    #region 외부 함수

    // Enemy 초기화
    public void InitializeEnemy(int code)
    {
        #region 엑셀 데이터 정리
        
        m_Enemyinfo_Excel = M_Enemy.GetData(code);
        #endregion

        #region 내부 데이터 정리

        m_EnemyInfo.Name_EN = m_Enemyinfo_Excel.Name_EN;
        m_EnemyInfo.Move_Type = m_Enemyinfo_Excel.Move_Type;
        m_EnemyInfo.Atk = m_Enemyinfo_Excel.Atk;
        m_EnemyInfo.HP = m_Enemyinfo_Excel.HP;
        m_EnemyInfo.Def = m_Enemyinfo_Excel.Def;
        m_EnemyInfo.Shild = m_Enemyinfo_Excel.Shild;
        m_EnemyInfo.Move_spd = m_Enemyinfo_Excel.Move_spd;
        m_EnemyInfo.CC_Rgs1 = m_Enemyinfo_Excel.CC_Rgs1;
        m_EnemyInfo.CC_Rgs2 = m_Enemyinfo_Excel.CC_Rgs2;
        m_EnemyInfo.CC_Rgs3 = m_Enemyinfo_Excel.CC_Rgs3;
        m_EnemyInfo.Atk_Code = m_Enemyinfo_Excel.Atk_Code;
        m_EnemyInfo.Skill1Code = m_Enemyinfo_Excel.Skill1Code;
        m_EnemyInfo.Skill2Code = m_Enemyinfo_Excel.Skill2Code;
        m_EnemyInfo.HPSkillCast = m_Enemyinfo_Excel.HPSkillCast;
        m_EnemyInfo.Prefeb = m_Enemyinfo_Excel.Prefab;

        #endregion
    }

    // 스턴
    public void On_Stun()
    {
        StartCoroutine(OnStun());
    }

    //분열
    public void On_Divide()
    {
        StartCoroutine(Divide());
    }

    //동서남북
    public void InitSetting(E_Direction p_waypoint)
    {
        direc = p_waypoint;
    }

    //분열한 적이 가지는 데이터
    //동서남북
    //바라보고 있던 waypoint
    //waypointindex값
    public void InitSetting(E_Direction p_waypoint, Transform _target, int _waypointindex)
    {
        direc = p_waypoint;
        target = _target;
        waypointIndex = _waypointindex;
    }

    //사망
    public void On_Death()
    {
        animator.SetBool("Die", true);
        
        SpawnManager.Instance.Despawn(this);
        //EnemyPool.Instance.GetPool(data.Name).DeSpawn(enemy);
    }

    //데미지
    public void On_DaMage(float damage)
    {
        if (damage <= Get_EnemyDef)
        {
            m_EnemyInfo.HP -= 1;
        }

        else
        {
            damage -= Get_EnemyDef;
            m_EnemyInfo.HP -= damage;
        }

        image.fillAmount -= (float)(damage * 0.01);

        if (m_EnemyInfo.HP <= 0)
        {
            On_Death();
        }

        // 버프 적용 확률
        List<float> BuffRand = new List<float>();
        // 버프 적용 여부
        List<bool> BuffApply = new List<bool>();
        // 버프 적용 계산
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

        //몬스터 버프 넣어야함
        #region 버프&디버프 합연산

        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프&디버프1 체크
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
                float Buff_Debufftime = BuffList[i].Duration;

                // 버프&디버프1 합연산
                if (buff.AddType == E_AddType.Fix)
                {
                    switch (buff.BuffType)
                    {
                        case E_BuffType.Dot_Dmg:
                            //총 체력에 buffamount의 퍼센트 만큼 가져오기
                            float amount = Get_EnemyHP * BuffAmount;

                            //총 도트 데미지를 초당으로 데미지로 바꾸기
                            float dot_dmg = amount / Buff_Debufftime;

                            StartCoroutine(Dot_DmgTime(Buff_Debufftime, dot_dmg));
                            break;
                    }
                }

                // 버프&디버프2 체크
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

                    // 버프&디버프2 합연산
                    if (buff.AddType == E_AddType.Fix)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Dot_Dmg:
                                //총 체력에 buffamount의 퍼센트 만큼 가져오기
                                float amount = Get_EnemyHP * BuffAmount;

                                //총 도트 데미지를 초당으로 데미지로 바꾸기
                                float dot_dmg = amount / Buff_Debufftime;

                                StartCoroutine(Dot_DmgTime(Buff_Debufftime, dot_dmg));
                                break;
                        }
                    }

                    // 버프&디버프3 체크
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

                        // 버프&디버프3 합연산
                        if (buff.AddType == E_AddType.Fix)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Dot_Dmg:
                                    //총 체력에 buffamount의 퍼센트 만큼 가져오기
                                    float amount = Get_EnemyHP * BuffAmount;

                                    //총 도트 데미지를 초당으로 데미지로 바꾸기
                                    float dot_dmg = amount / Buff_Debufftime;

                                    StartCoroutine(Dot_DmgTime(Buff_Debufftime, dot_dmg));
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region 버프&디버프 곱연산

        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프&디버프1 체크
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
                float Buff_Debufftime = BuffList[i].Duration;

                // 버프&디버프1 곱연산
                if (buff.AddType == E_AddType.Percent)
                {

                    switch (buff.BuffType)
                    {
                        case E_BuffType.Def:
                            StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                            break;
                        case E_BuffType.Stun:
                            On_Stun();
                            break;
                        case E_BuffType.Insta_Kill:
                            On_Death();
                            break;
                        case E_BuffType.Move_spd:
                            StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                            break;
                    }
                }

                // 버프&디버프2 체크
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

                    // 버프&디버프2 곱연산
                    if (buff.AddType == E_AddType.Percent)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Move_spd:
                                StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                                break;
                        }
                    }

                    // 버프&디버프3 체크
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

                        // 버프&디버프3 곱연산
                        if (buff.AddType == E_AddType.Percent)
                        {
                            switch (buff.BuffType)
                            {
                                
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region 내부 함수

    //다음 waypoint 정보
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

    #region 코루틴

    //스턴
    //애니메이션 추가
    IEnumerator OnStun()
    {
        isStun = true;

        yield return new WaitForSeconds(1f);

        isStun = false;
    }

    //분열
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

            SpawnManager.Instance.SpawnEnemy(direc, pos, target, waypointIndex, m_EnemyInfo.Name_EN);

            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator PersentBuff_DeBuffTime(float time, E_BuffType type, float amount)
    {
        float origin = 0f;

        switch (type)
        {
            case E_BuffType.Def:
                origin = m_EnemyInfo.Def;

                m_EnemyInfo.Def *= amount;
                break;

            case E_BuffType.Move_spd:
                origin = m_EnemyInfo.Move_spd;

                m_EnemyInfo.Move_spd *= amount;
                break;
        }

        yield return new WaitForSeconds(time);

        switch (type)
        {
            case E_BuffType.Def:
                m_EnemyInfo.Def = origin;
                break;

            case E_BuffType.Move_spd:
                m_EnemyInfo.Move_spd = origin;
                break;
        }
    }

    IEnumerator Dot_DmgTime(float time, float dmg)
    {
        for (int i = 0; i < time; i++)
        {
            m_EnemyInfo.HP -= dmg;
            yield return new WaitForSeconds(1f);
        }
    }
    #endregion
}