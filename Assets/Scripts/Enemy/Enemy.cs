using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    [Serializable]
    public struct SkillStat_Data
    {
        public float CoolTime;
        public float Dmg;
        public float Dmg_plus;
        public float Range;
        public float Speed;
        public float Target_num;
        public float Size;
    }

    #region Get������Ƽ

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

    //ü�º��ȿ��
    public float Get_EnemyHPSkillCast => m_EnemyInfo.HPSkillCast;

    public int Get_EnemyPrefeb => m_EnemyInfo.Prefeb;

    public int Get_WayPointIndex => waypointIndex;

    public E_Direction Get_Direction => direc;

    #endregion

    private Dictionary<int, IEnumerator> debuff;

    private Dictionary<int, IEnumerator> m_buff;

    //���� �ٶ󺸰� �ִ� waypoint
    [SerializeField] Transform target;

    [SerializeField] int waypointIndex = 0;

    [SerializeField] E_Direction direc;

    private Animator animator;

    private bool isStun = false;

    [SerializeField] Enemy_Data m_EnemyInfo;

    private float MaxHp;

    //ü�¹�
    private Image image;

    private Enemy_TableExcel m_Enemyinfo_Excel;
    private EnemyManager M_Enemy => EnemyManager.Instance;

    private DataTableManager M_DataTable => DataTableManager.Instance;
    private SkillCondition_TableExcelLoader skillcondition_table => M_DataTable.GetDataTable<SkillCondition_TableExcelLoader>();
    private SkillStat_TableExcelLoader skillstat_table => M_DataTable.GetDataTable<SkillStat_TableExcelLoader>();
    private BuffCC_TableExcelLoader buffcc_table => M_DataTable.GetDataTable<BuffCC_TableExcelLoader>();

    #region �ó��� ����
    // ����
    public List<BuffCC_TableExcel> BuffList;
    #endregion

    private float Half_HP;
    private float Origin_HP;

    public bool isDivide = false;
    private bool isDefBuff = false;

    //����
    protected SphereCollider m_RangeCollider;

    //��ų ���� �ֺ� Enemy ����
    private List<Enemy> Enemy_obj;

    private float Atk_Timer;

    private EnemySkillManager enemyskillmanager;

    private SkillCondition_TableExcel atkconditiondata;
    private SkillStat_TableExcel atkstatdata;

    private SkillCondition_TableExcel skillconditiondata;
    [SerializeField] SkillStat_TableExcel skillstatdata;

    public Transform AttackPivot;
    public Transform HitPivot;

    private void OnTriggerStay(Collider other)
    {
        Enemy_obj.Add(this);

        for (int i = 0; i < skillstatdata.Target_num - 1; i++)
        {
            Enemy_obj.Add(other.gameObject.GetComponent<Enemy>());
        }
    }

    private void Start()
    {
        debuff = new Dictionary<int, IEnumerator>();
        m_buff = new Dictionary<int, IEnumerator>();

        gameObject.layer = LayerMask.NameToLayer("Enemy");

        image = transform.GetChild("Fill").GetComponent<Image>();

        enemyskillmanager = EnemySkillManager.Instance;

        transform.Find("EnemySkillRange").gameObject.layer = LayerMask.NameToLayer("EnemySkillRange");
        m_RangeCollider = transform.Find("EnemySkillRange").GetComponent<SphereCollider>();
        m_RangeCollider.isTrigger = true;

        BuffList = new List<BuffCC_TableExcel>();

        animator = transform.Find("Mesh").GetComponent<Animator>();

        Enemy_obj = new List<Enemy>();

        //��ų ������
        atkconditiondata = enemyskillmanager.GetConditionData(m_EnemyInfo.Atk_Code);

        atkstatdata = enemyskillmanager.GetStatData(atkconditiondata.PassiveCode);

        Atk_Timer = atkstatdata.CoolTime;

        image.fillAmount = m_EnemyInfo.HP;

        if (m_EnemyInfo.Name_EN == "Grffin02")
        {
            Half_HP = (float)(m_EnemyInfo.HP * 0.5);
            Origin_HP = m_EnemyInfo.HP;
        }

        //�� �������� Ÿ�� �ʱ�ȭ
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

        //��ų ���� ���͸�
        if (m_EnemyInfo.Skill1Code > 0)
        {
            skillconditiondata = enemyskillmanager.GetConditionData(m_EnemyInfo.Skill1Code);

            skillstatdata = enemyskillmanager.GetStatData(skillconditiondata.PassiveCode);

            m_RangeCollider.radius = skillstatdata.Range;

            Invoke("StartSkill", skillstatdata.CoolTime);
        }
    }

    private void Update()
    {
        //���ո� Ÿ������ ���
        //���̳� �߰��� ��ֹ��� �ִٸ� �ٲ����
        if (waypointIndex >= 3)
        {
            float Distance = Vector3.Distance(transform.position, new Vector3(0f,0f,0f));

            //�Ÿ� �ȿ� �ִٸ�
            if (Distance <= atkstatdata.Range) 
            {
                transform.rotation.SetLookRotation(new Vector3(0f, 0f, 0f));

                if (Atk_Timer >= atkstatdata.CoolTime)
                {
                    animator.SetTrigger("Attack");
                    Atk_Timer = 0f;
                }

                else
                {
                    Atk_Timer += Time.deltaTime;
                }
            }
        }

        if (!isStun)
        {
            //Vector3 dir = target.position - transform.position;
            //transform.Translate(dir.normalized * m_EnemyInfo.Move_spd * Time.deltaTime, Space.World);

            //if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            //{
            //    GetNextWayPoint();
            //}

            Vector3 dir = target.position - transform.position;
            transform.Translate(dir.normalized * 1f * Time.deltaTime, Space.World);

            if (Vector3.Distance(transform.position, target.position) <= 0.2f)
            {
                GetNextWayPoint();
            }
        }

        #region �׸���(�ϴ�)�� ü����

        if (m_EnemyInfo.Name_EN == "Grffin02" && !isDivide)
        {
            //HP�� �ݾƷ��� �Ǿ�����
            if (m_EnemyInfo.HP <= Half_HP)
            {
                ChangeMode();
            }
        }

        #endregion

        if (m_EnemyInfo.HP <= 0)
        {
            On_Death();
        }
    }

    #region �ܺ� �Լ�

    // Enemy �ʱ�ȭ
    public void InitializeEnemy(int code)
    {
        #region ���� ������ ����

        m_Enemyinfo_Excel = M_Enemy.GetData(code);
        #endregion

        #region ���� ������ ����

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

        MaxHp = m_EnemyInfo.HP;

        #endregion
    }

    // ����
    public void On_Stun(int code)
    {
        StartCoroutine(OnStun(code));
    }

    //��ȯ
    public void On_Summon(string name)
    {
        StartCoroutine(OnSummon(name));
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
        animator.SetBool("Die", true);
    }

    //������
    public void On_DaMage(float damage)
    {
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

        #region ����� �տ���

        for (int i = 0; i < BuffList.Count; ++i)
        {
            // �����1 üũ
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

                // �����1 �տ���
                if (buff.AddType == E_AddType.Fix)
                {
                    switch (buff.BuffType)
                    {
                        case E_BuffType.Dot_Dmg:
                            //�� ü�¿� buffamount�� �ۼ�Ʈ ��ŭ ��������
                            float amount = Get_EnemyHP * BuffAmount;

                            //�� ��Ʈ �������� �ʴ����� �������� �ٲٱ�
                            float dot_dmg = amount / Buff_Debufftime;

                            // ����� �ڵ�
                            int code = BuffList[i].Code;

                            // ����ǰ� �ִ� ����� �� ���� ����� �ڵ尡 ������
                            if (!debuff.ContainsKey(code))
                            {
                                // �߰�
                                debuff.Add(code, Dot_DmgTime(code, Buff_Debufftime, dot_dmg));
                            }
                            // ����� �ڵ尡 ������
                            else
                            {
                                if (debuff.TryGetValue(code, out IEnumerator coroutine))
                                {
                                    StopCoroutine(coroutine);
                                }
                                debuff[code] = Dot_DmgTime(code, Buff_Debufftime, dot_dmg);
                            }

                            StartCoroutine(debuff[code]);
                            //StartCoroutine(Dot_DmgTime(Buff_Debufftime, dot_dmg));
                            break;

                        case E_BuffType.Summon:
                            On_Summon(BuffList[i].Name_EN);
                            break;
                    }
                }

                // �����2 üũ
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

                    // ����&�����2 �տ���
                    if (buff.AddType == E_AddType.Fix)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Dot_Dmg:
                                //�� ü�¿� buffamount�� �ۼ�Ʈ ��ŭ ��������
                                float amount = Get_EnemyHP * BuffAmount;

                                //�� ��Ʈ �������� �ʴ����� �������� �ٲٱ�
                                float dot_dmg = amount / Buff_Debufftime;

                                // ����� �ڵ�
                                int code = BuffList[i].Code;

                                // ����ǰ� �ִ� ����� �� ���� ����� �ڵ尡 ������
                                if (!debuff.ContainsKey(code))
                                {
                                    // �߰�
                                    debuff.Add(code, Dot_DmgTime(code, Buff_Debufftime, dot_dmg));
                                }
                                // ����� �ڵ尡 ������
                                else
                                {
                                    if (debuff.TryGetValue(code, out IEnumerator coroutine))
                                    {
                                        StopCoroutine(coroutine);
                                    }
                                    debuff[code] = Dot_DmgTime(code, Buff_Debufftime, dot_dmg);
                                }

                                StartCoroutine(debuff[code]);
                                break;
                        }
                    }

                    // �����3 üũ
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

                        // �����3 �տ���
                        if (buff.AddType == E_AddType.Fix)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Dot_Dmg:

                                    //�ٲ�ߵ� tower ���ݷ�����
                                    //�� ü�¿� buffamount�� �ۼ�Ʈ ��ŭ ��������
                                    float amount = Get_EnemyHP * BuffAmount;

                                    //�� ��Ʈ �������� �ʴ����� �������� �ٲٱ�
                                    float dot_dmg = amount / Buff_Debufftime;

                                    // ����� �ڵ�
                                    int code = BuffList[i].Code;

                                    // ����ǰ� �ִ� ����� �� ���� ����� �ڵ尡 ������
                                    if (!debuff.ContainsKey(code))
                                    {
                                        // �߰�
                                        debuff.Add(code, Dot_DmgTime(code, Buff_Debufftime, dot_dmg));
                                    }
                                    // ����� �ڵ尡 ������
                                    else
                                    {
                                        if (debuff.TryGetValue(code, out IEnumerator coroutine))
                                        {
                                            StopCoroutine(coroutine);
                                        }
                                        debuff[code] = Dot_DmgTime(code, Buff_Debufftime, dot_dmg);
                                    }

                                    StartCoroutine(debuff[code]);
                                    break;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ����� ������

        for (int i = 0; i < BuffList.Count; ++i)
        {
            // �����1 üũ
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

                // �����1 ������
                if (buff.AddType == E_AddType.Percent)
                {

                    switch (buff.BuffType)
                    {
                        case E_BuffType.Def:
                            StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                            break;
                        case E_BuffType.Stun:

                            int code = BuffList[i].Code;

                            On_Stun(code);
                            break;
                        case E_BuffType.Insta_Kill:
                            On_Death();
                            break;
                        case E_BuffType.Move_spd:
                            StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                            break;
                    }
                }

                // �����2 üũ
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

                    // �����2 ������
                    if (buff.AddType == E_AddType.Percent)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Move_spd:
                                StartCoroutine(PersentBuff_DeBuffTime(Buff_Debufftime, buff.BuffType, BuffAmount));
                                break;
                        }
                    }

                    // �����3 üũ
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

                        // �����3 ������
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

    }

    public void On_SkillBuff()
    {
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

        #region ���� �տ���

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
                float Buff_Debufftime = BuffList[i].Duration;

                // ����1 �տ���
                if (buff.AddType == E_AddType.Percent)
                {
                    switch (buff.BuffType)
                    {
                        case E_BuffType.Heal:
                            // ���� �ڵ�
                            int code = BuffList[i].Code;

                            float amount = Get_EnemyHP * BuffAmount;

                            //�� ��Ʈ �������� �ʴ����� �������� �ٲٱ�
                            float hp_heal = amount / Buff_Debufftime;

                            // ����ǰ� �ִ� ���� �� ���� ���� �ڵ尡 ������
                            if (!m_buff.ContainsKey(code))
                            {
                                // �߰�
                                m_buff.Add(code, HealTime(code, Buff_Debufftime, hp_heal));
                            }
                            // ���� �ڵ尡 ������
                            else
                            {
                                if (m_buff.TryGetValue(code, out IEnumerator coroutine))
                                {
                                    StopCoroutine(coroutine);
                                }
                                m_buff[code] = HealTime(code, Buff_Debufftime, hp_heal);
                            }

                            StartCoroutine(m_buff[code]);
                            break;

                        case E_BuffType.Def:
                            code = BuffList[i].Code;

                            amount = Get_EnemyDef * BuffAmount;

                            // ����ǰ� �ִ� ���� �� ���� ���� �ڵ尡 ������
                            if (!m_buff.ContainsKey(code))
                            {
                                // �߰�
                                m_buff.Add(code, DefBuff(code, Buff_Debufftime, amount));
                            }
                            // ���� �ڵ尡 ������
                            else
                            {
                                if (m_buff.TryGetValue(code, out IEnumerator coroutine))
                                {
                                    StopCoroutine(coroutine);
                                }
                                m_buff[code] = DefBuff(code, Buff_Debufftime, amount);
                            }

                            StartCoroutine(m_buff[code]);
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
                    if (buff.AddType == E_AddType.Percent)
                    {
                        switch (buff.BuffType)
                        {

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

    #region ���� �Լ�

    private void StartSkill()
    {
        animator.SetTrigger("Skill");
    }

    private void ChangeMode()
    {
        //�׸��� �ϴ� �ڵ�� ������ ����
        InitializeEnemy(200009);
    }

    //���� waypoint ����
    private void GetNextWayPoint()
    {
        switch (direc)
        {
            case E_Direction.East:
                if (waypointIndex >= EastWayPoints.points.Length - 1)
                {
                    transform.position = EastWayPoints.points[EastWayPoints.points.Length - 1].position;
                    animator.SetTrigger("Attack");
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
                    animator.SetTrigger("Attack");
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
                    animator.SetTrigger("Attack");
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
                    animator.SetTrigger("Attack");
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
    IEnumerator OnStun(int code)
    {
        isStun = true;

        animator.SetTrigger("Stun");

        yield return new WaitForSeconds(1f);

        isStun = false;
    }

    //�п�
    IEnumerator OnSummon(string name)
    {
        SpawnManager.Instance.SpawnEnemy(direc, transform.localPosition, target, waypointIndex, name, animator);

        yield return null;
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

        //debuff[code] = null;
        BuffList.RemoveAt(0);
    }

    IEnumerator Dot_DmgTime(int code, float time, float dmg)
    {
        BuffList.RemoveAt(0);
        for (int i = 0; i < time; i++)
        {
            m_EnemyInfo.HP -= dmg;
            yield return new WaitForSeconds(1f);
        }

        debuff[code] = null;
        BuffList.RemoveAt(0);
    }

    IEnumerator HealTime(int code, float time, float hp_heal)
    {
        for (int i = 0; i < time; i++)
        {
            if (m_EnemyInfo.HP <= MaxHp)
            {
                m_EnemyInfo.HP += hp_heal;

                if (MaxHp < m_EnemyInfo.HP)
                {
                    m_EnemyInfo.HP = MaxHp;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        m_buff[code] = null;
        BuffList.RemoveAt(0);
    }

    IEnumerator DefBuff(int code, float time, float amount)
    {
        float origin = m_EnemyInfo.Def;

        m_EnemyInfo.Def = amount;

        yield return new WaitForSeconds(time);

        m_EnemyInfo.Def = origin;

        m_buff[code] = null;
        BuffList.RemoveAt(0);
    }

    #endregion

    #region Call�Լ�

    private void CallAttack()
    {
        m_EnemyInfo.Atk *= atkstatdata.Dmg;
        enemyskillmanager.SpawnProjectileSkill(atkconditiondata.projectile_prefab, m_EnemyInfo.Atk, atkconditiondata, atkstatdata);
    }

    private void CallSkill()
    {
        for (int i = 0; i < Enemy_obj.Count; i++)
        {
            if (Enemy_obj[i] != null)
            {
                BuffCC_TableExcel setbuff;

                setbuff = buffcc_table.DataList.Where(item => item.Code == m_EnemyInfo.Skill1Code).Single();

                Enemy_obj[i].BuffList.Add(setbuff);
                Enemy_obj[i].On_SkillBuff();
            }
        }
    }

    private void CallDie()
    {
        SpawnManager.Instance.Despawn(this);
    }

    #endregion
}