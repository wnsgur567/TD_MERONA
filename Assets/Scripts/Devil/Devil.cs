using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    // ���� �ڵ�(�ӽ�)
    public int m_TempCode;
    // Ÿ��
    public GameObject m_Target;

    // ���� ����(����)
    protected Tower_TableExcel m_DevilInfo_Excel;
    // ���� ����
    protected S_DevilData m_DevilInfo;

    protected delegate void DevilSkillHandler(DevilSkillArg arg);
    protected event DevilSkillHandler Skill01Event;
    protected event DevilSkillHandler Skill02Event;

    #region ���� ������Ʈ
    protected AttackRange m_AttackRange;
    #endregion

    #region ���� ������Ƽ
    // Ÿ�� �Ŵ���
    protected TowerManager M_Tower => TowerManager.Instance;
    // ���� �Ŵ���
    protected BuffManager M_Buff => BuffManager.Instance;
    // ��ų �Ŵ���
    protected SkillManager M_Skill => SkillManager.Instance;

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
    private void Awake()
    {
        InitializeTower(m_TempCode);
    }

    private void Update()
    {
        RotateToTarget();
        UpdateTarget();
        AttackTarget();
    }
    #endregion

    #region ���� �Լ�
    // Ÿ�� �ʱ�ȭ
    protected virtual void InitializeTower(int code)
    {
        #region ���� ������ ����
        m_DevilInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region ���� ������ ����
        m_DevilInfo.RotateSpeed = 5f;
        m_DevilInfo.InitialRotation = transform.eulerAngles;
        m_DevilInfo.ShouldFindTarget = true;

        // �⺻ ��ų ������
        m_DevilInfo.Condition_Default = M_Skill.GetConditionData(m_DevilInfo_Excel.Atk_Code);
        m_DevilInfo.Stat_Default = M_Skill.GetStatData(m_DevilInfo_Excel.Atk_Code);
        // �⺻ ��ų
        m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
        m_DevilInfo.AttackTimer_Default = m_DevilInfo.Stat_Default.CoolTime;
        #endregion

        #region ���� ������Ʈ
        m_AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_AttackRange.Initialize();
        m_AttackRange.SetRange(m_DevilInfo.Stat_Default.Range);
        #endregion
    }
    // Ÿ�� ȸ��
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
                    m_Target = m_AttackRange.GetNearTarget();
                    m_DevilInfo.ShouldFindTarget = false;
                }
                break;
            case E_TargetType.RandTarget:
                if (m_DevilInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange.GetRandomTarget();
                    m_DevilInfo.ShouldFindTarget = false;
                }
                break;
            // FixTarget (Ÿ���� ��Ÿ��� ����ų� ���� ��� ����)
            case E_TargetType.FixTarget:
                if (null == m_Target || // ����ó��
                    DistanceToTarget > m_DevilInfo.Stat_Default.Range) // Ÿ���� ��Ÿ��� ��� ���
                {
                    m_Target = m_AttackRange.GetNearTarget();
                }
                break;
        }
    }
    // Ÿ�� ����
    protected void AttackTarget()
    {
        if (m_DevilInfo.AttackTimer_Default < m_DevilInfo.AttackSpeed_Default)
        {
            m_DevilInfo.AttackTimer_Default += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            // ���� ������ ����
            m_DevilInfo.AttackTimer_Default -= m_DevilInfo.AttackSpeed_Default;
            m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
            m_DevilInfo.ShouldFindTarget = true;

            // ��ų ������ �ҷ�����
            SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
            SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

            // �⺻ ��ų ����ü ����
            int SkillCode = conditionData.projectile_prefab;
            Skill Skill = M_Skill.SpawnProjectileSkill(SkillCode);
            Skill.transform.position = transform.position;
            Skill.enabled = true;
            Skill.gameObject.SetActive(true);

            // ��ų ������ ����
            Skill.InitializeSkill(m_Target, conditionData, statData);
        }
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
    public void OnSkill01(DevilSkillArg arg)
    {
        Skill01Event?.Invoke(arg);
    }
    public void OnSkill02(DevilSkillArg arg)
    {
        Skill02Event?.Invoke(arg);
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
        public int m_Cooldown;
        public int m_CooldownTimer;
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
