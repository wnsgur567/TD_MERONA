using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    // Ÿ��
    public Enemy m_Target;

    // ���� ����(����)
    protected Tower_TableExcel m_DevilInfo_Excel;
    // ���� ����
    [SerializeField] S_DevilData m_DevilInfo;

    protected delegate void DevilSkillHandler(DevilSkillArg arg);
    protected event DevilSkillHandler Skill01Event;
    protected event DevilSkillHandler Skill02Event;

    public delegate void DevilUpdateHPHandler(float max, float current);
    public event DevilUpdateHPHandler UpdateHPEvent;

    #region ���� ������Ʈ
    protected AttackRange m_AttackRange_Default;
    #endregion

    #region ���� ������Ƽ
    // ���� �Ŵ���
    protected DevilManager M_Devil => DevilManager.Instance;
    // Ÿ�� �Ŵ���
    protected TowerManager M_Tower => TowerManager.Instance;
    // ���� �Ŵ���
    protected BuffManager M_Buff => BuffManager.Instance;
    // ��ų �Ŵ���
    protected SkillManager M_Skill => SkillManager.Instance;
    // �� �Ŵ���
    protected EnemyManager M_Enemy => EnemyManager.Instance;

    // Ÿ�� ȸ�� �ӵ�
    protected float RotateSpeed
    {
        get
        {
            return m_DevilInfo.RotateSpeed * Time.deltaTime;
        }
    }
    // Ÿ�ٱ����� �Ÿ�
    protected float DistanceToTarget
    {
        get
        {
            return Vector3.Distance(transform.position, m_Target.transform.position);
        }
    }
    #endregion

    #region �ܺ� ������Ƽ
    public float MaxHP => m_DevilInfo_Excel.HP;
    public float HP => m_DevilInfo.m_HP;
    #endregion

    #region ����Ƽ �ݹ� �Լ�
    private void Update()
    {
        RotateToTarget();
        UpdateTarget();
        AttackTarget();
        ReduceSkillCooldown();
    }
    #endregion

    #region ���� �Լ�
    // ���� �ʱ�ȭ
    protected virtual void InitializeDevil(E_Devil no)
    {
        #region ���� ������ ����
        m_DevilInfo_Excel = M_Devil.GetData(no);
        #endregion

        #region ���� ������ ����
        m_DevilInfo.RotateSpeed = 5f;
        m_DevilInfo.InitialRotation = transform.eulerAngles;
        m_DevilInfo.ShouldFindTarget = true;

        // ���� �ǹ�
        // m_DevilInfo.AttackPivot ??= transform.GetChild("AttackPivot");
        if (null == m_DevilInfo.AttackPivot)
            m_DevilInfo.AttackPivot = transform.GetChild("AttackPivot");
        // �ǰ� �ǹ�
        // m_DevilInfo.HitPivot ??= transform.GetChild("HitPivot");
        if (null == m_DevilInfo.HitPivot)
            m_DevilInfo.HitPivot = transform.GetChild("HitPivot");

        // �⺻ ��ų ������
        m_DevilInfo.Condition_Default = M_Skill.GetConditionData(m_DevilInfo_Excel.Atk_Code);
        m_DevilInfo.Stat_Default = M_Skill.GetStatData(m_DevilInfo_Excel.Atk_Code);
        // �⺻ ��ų
        m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
        m_DevilInfo.AttackTimer_Default = m_DevilInfo.Stat_Default.CoolTime;

        m_DevilInfo.m_HP = m_DevilInfo_Excel.HP;
        m_DevilInfo.m_Def = m_DevilInfo_Excel.Def;
        #endregion

        #region ���� ������Ʈ
        if (null == m_AttackRange_Default)
        {
            m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
            m_AttackRange_Default.gameObject.layer = LayerMask.NameToLayer("TowerAttackRange");
        }
        m_AttackRange_Default.Range = m_DevilInfo.Stat_Default.Range;
        #endregion
    }
    // ���� ȸ��
    protected void RotateToTarget()
    {
        // ȸ���� ����
        Vector3 dir;

        // Ÿ���� ������
        if (null == m_Target)
        {
            // �ʱ� �������� ���� ����
            dir = m_DevilInfo.InitialRotation;
        }
        // Ÿ���� ������
        else
        {
            // Ÿ�� �������� ���� ����
            dir = m_Target.transform.position - transform.position;
        }

        // ȸ��
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), RotateSpeed);
    }
    // Ÿ�� ������Ʈ
    protected void UpdateTarget()
    {
        // Ÿ�� ���� ���ؿ� ����
        switch ((E_TargetType)m_DevilInfo.Condition_Default.Target_type)
        {
            case E_TargetType.CloseTarget:
                if (m_DevilInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange_Default.GetNearTarget();
                    m_DevilInfo.ShouldFindTarget = false;
                }
                break;
            case E_TargetType.RandTarget:
                if (m_DevilInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange_Default.GetRandomTarget();
                    m_DevilInfo.ShouldFindTarget = false;
                }
                break;
            // FixTarget (Ÿ���� ��Ÿ��� ����ų� ���� ��� ����)
            case E_TargetType.FixTarget:
                if (null == m_Target || // ����ó��
                    DistanceToTarget > m_DevilInfo.Stat_Default.Range) // Ÿ���� ��Ÿ��� ��� ���
                {
                    m_Target = m_AttackRange_Default.GetNearTarget();
                }
                break;
        }
    }
    // ���� ����
    protected void AttackTarget()
    {
        #region �⺻ ��ų
        // �⺻ ��ų Ÿ�̸�
        if (m_DevilInfo.AttackTimer_Default < m_DevilInfo.AttackSpeed_Default)
        {
            m_DevilInfo.AttackTimer_Default += Time.deltaTime;
        }
        // �⺻ ��ų ����
        else if (null != m_Target)
        {
            // ���� ������ ����
            m_DevilInfo.AttackTimer_Default -= m_DevilInfo.AttackSpeed_Default;
            m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
            m_DevilInfo.ShouldFindTarget = true;

            // �⺻ ��ų ������ �ҷ�����
            SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
            SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

            // �⺻ ����� ����
            statData.Dmg *= m_DevilInfo_Excel.Atk;
            statData.Dmg += statData.Dmg_plus;

            // �⺻ ��ų ����ü ����
            int DefaultSkillCode = conditionData.projectile_prefab;
            if ((E_TargetType)m_DevilInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
            {
                List<Enemy> EnemyList = M_Enemy.GetEnemyList();

                for (int i = 0; i < EnemyList.Count; ++i)
                {
                    Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);
                    DefaultSkill.transform.position = m_DevilInfo.AttackPivot.position;
                    DefaultSkill.enabled = true;
                    DefaultSkill.gameObject.SetActive(true);

                    // �⺻ ��ų ������ ����
                    DefaultSkill.InitializeSkill(EnemyList[i], conditionData, statData);
                }
            }
            else
            {
                Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);
                DefaultSkill.transform.position = m_DevilInfo.AttackPivot.position;
                DefaultSkill.enabled = true;
                DefaultSkill.gameObject.SetActive(true);

                // �⺻ ��ų ������ ����
                DefaultSkill.InitializeSkill(m_Target, conditionData, statData);
            }
        }
        #endregion
    }
    // ���� ��ų ��Ÿ�� ����
    protected void ReduceSkillCooldown()
    {
        ReduceSkill01Cooldown(Time.deltaTime);
        ReduceSkill02Cooldown(Time.deltaTime);
    }

    protected IEnumerator SK003()
    {
        yield break;
    }
    protected IEnumerator SK004()
    {
        if (m_DevilInfo.m_HP <= m_DevilInfo_Excel.HP * 0.5f)
        {
            m_DevilInfo.m_DefaultSkill_LifeSteal = 1f;
        }
        else
        {
            m_DevilInfo.m_DefaultSkill_LifeSteal = 0f;
        }
        yield break;
    }
    protected IEnumerator SK005()
    {
        yield break;
    }
    protected IEnumerator SK006()
    {
        yield break;
    }
    #endregion

    #region �ܺ� �Լ�
    // ��ų01 ��Ÿ�� ����
    public void ReduceSkill01Cooldown(float time)
    {
        if (m_DevilInfo.m_Skill01.m_CurrentCharge < m_DevilInfo.m_Skill01.m_MaxCharge)
        {
            m_DevilInfo.m_Skill01.m_CooltimeTimer -= time;

            if (m_DevilInfo.m_Skill01.m_CooltimeTimer <= 0f)
            {
                m_DevilInfo.m_Skill01.m_CooltimeTimer += m_DevilInfo.m_Skill01.m_Cooltime;

                ++m_DevilInfo.m_Skill01.m_CurrentCharge;
            }
        }
    }
    // ��ų02 ��Ÿ�� ����
    public void ReduceSkill02Cooldown(float time)
    {
        if (m_DevilInfo.m_Skill02.m_CurrentCharge < m_DevilInfo.m_Skill02.m_MaxCharge)
        {
            m_DevilInfo.m_Skill02.m_CooltimeTimer -= time;

            if (m_DevilInfo.m_Skill02.m_CooltimeTimer <= 0f)
            {
                m_DevilInfo.m_Skill02.m_CooltimeTimer += m_DevilInfo.m_Skill02.m_Cooltime;

                ++m_DevilInfo.m_Skill02.m_CurrentCharge;
            }
        }
    }
    public void OnSkill01(DevilSkillArg arg)
    {
        if (m_DevilInfo.m_Skill01.m_CurrentCharge > 0)
        {
            --m_DevilInfo.m_Skill01.m_CurrentCharge;

            Skill01Event?.Invoke(arg);
        }
    }
    public void OnSkill02(DevilSkillArg arg)
    {
        if (m_DevilInfo.m_Skill02.m_CurrentCharge > 0)
        {
            --m_DevilInfo.m_Skill02.m_CurrentCharge;

            Skill02Event?.Invoke(arg);
        }
    }
    public void GetDamage(float damage)
    {
        float Damage = damage - m_DevilInfo.m_Def;
        if (Damage < 1f)
            Damage = 1f;

        m_DevilInfo.m_HP -= Damage;

        UpdateHPEvent?.Invoke(MaxHP, HP);
    }
    #endregion

    [System.Serializable]
    public struct S_DevilData
    {
        // ȸ�� �ӵ�
        public float RotateSpeed;
        // �ʱ� ȸ�� ��
        public Vector3 InitialRotation;
        // �� ���� ����
        public bool ShouldFindTarget;
        // ���� �ǹ�
        public Transform AttackPivot;
        // �ǰ� �ǹ�
        public Transform HitPivot;

        // �⺻ ��ų ������
        public SkillCondition_TableExcel Condition_Default;
        public SkillStat_TableExcel Stat_Default;
        // �⺻ ��ų ���� �ӵ�
        public float AttackSpeed_Default;
        // �⺻ ��ų Ÿ�̸�
        public float AttackTimer_Default;

        // ��ų
        public S_DevilSkillData m_Skill01;
        public S_DevilSkillData m_Skill02;

        public float m_HP;
        public float m_Def;
        public float m_DefaultSkill_LifeSteal;
    }
    // ���� ��ų ����
    [System.Serializable]
    public struct S_DevilSkillData
    {
        public SkillCondition_TableExcel m_ConditionData;
        public SkillStat_TableExcel m_StatData;

        public E_SkillType m_SkillType;
        public E_SkillRangeType m_SkillRangeType;
        public int m_MaxCharge;
        public int m_CurrentCharge;
        public float m_Cooltime;
        public float m_CooltimeTimer;
    }

    public struct DevilSkillArg
    {
        public S_DevilSkillData skillData;
        public E_Direction dir;
    }

    public enum E_SkillType
    {
        None,

        Active,
        Passive,
    }
    public enum E_SkillRangeType
    {
        None,

        Fix,
        Direction,
        All
    }
}
