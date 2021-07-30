using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int m_TempCode;

    // Ÿ�� ���� (����)
    public S_TowerData_Excel m_TowerInfo_Excel;
    // Ÿ�� ����
    public S_TowerData m_TowerInfo;

    // Ÿ��
    public GameObject m_Target;

    #region ���� ������Ʈ
    public AttackRange m_AttackRange;
    #endregion

    #region ���� ������Ƽ
    // Ÿ�� ȸ�� �ӵ�
    protected float RotateSpeed
    {
        get
        {
            return m_TowerInfo.RotateSpeed * Time.deltaTime;
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
    // ��ų �޸�Ǯ
    protected SkillPool M_SkillPool => SkillPool.Instance;
    // ���ҽ� �Ŵ���
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    // Ÿ�� �Ŵ���
    protected TowerManager M_Tower => TowerManager.Instance;
    // ��ų ����� ����
    protected SkillConditionData Skill_Condition
    {
        get
        {
            return M_Resources.GetScriptableObject<SkillConditionData>("Skill", "SkillConditionData");
        }
    }
    // ��ų ���� ����
    protected SkillStatData Skill_Stat
    {
        get
        {
            return M_Resources.GetScriptableObject<SkillStatData>("Skill", "SkillStatData");
        }
    }
    #endregion

    private void Awake()
    {
        InitializeTower(m_TempCode);

        #region ���� ������Ʈ
        m_AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_AttackRange.SetRange(m_TowerInfo.DefaultSkillStat.Range);
        #endregion
    }

    // Ÿ�� �ʱ�ȭ
    public void InitializeTower(int code)
    {
        #region ���� ������ ����
        m_TowerInfo_Excel = M_Tower.m_TowerData.GetData(code);
        #endregion

        #region ���� ������ ����
        // ��Ÿ
        m_TowerInfo.DefaultSkillCondition = Skill_Condition.GetData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.DefaultSkillStat = Skill_Stat.GetData(m_TowerInfo_Excel.Atk_Code);

        // ����
        m_TowerInfo.AttackTimer = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.IsAttacking = true;
        // �ó���
        // ���ݷ�
        m_TowerInfo.SynergyStat.Atk_Fix = 0f;
        m_TowerInfo.SynergyStat.Atk_Percent = 1f;
        // ��Ÿ�
        m_TowerInfo.SynergyStat.Range_Fix = 0f;
        m_TowerInfo.SynergyStat.Range_Percent = 1f;
        // ���� �ӵ�
        m_TowerInfo.SynergyStat.Atk_spd_Fix = 0f;
        m_TowerInfo.SynergyStat.Atk_spd_Percent = 1f;
        // ġ��Ÿ Ȯ��
        m_TowerInfo.SynergyStat.Crit_rate_Fix = 0f;
        m_TowerInfo.SynergyStat.Crit_rate_Percent = 1f;
        // ġ��Ÿ ����
        m_TowerInfo.SynergyStat.Crit_Dmg_Fix = 0f;
        m_TowerInfo.SynergyStat.Crit_Dmg_Percent = 1f;
        #endregion
    }

    private void Update()
    {
        RotateToTarget();
        UpdateTarget();
        AttackTarget();
    }

    // Ÿ�� ȸ��
    public void RotateToTarget()
    {
        // ȸ���� ����
        Vector3 dir;

        // Ÿ���� ������
        if (null == m_Target)
        {
            // �ʱ� �������� ���� ����
            dir = m_TowerInfo.InitialRotation;
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
    // Ÿ�� ���� ���ؿ� ���� Ÿ�� ������Ʈ
    public void UpdateTarget()
    {
        // Ÿ�� ���� ���ؿ� ����
        switch (m_TowerInfo.DefaultSkillCondition.Target_type)
        {
            case E_TargetType.CloseTarget:
                if (m_TowerInfo.IsAttacking)
                {
                    m_Target = m_AttackRange.GetNearTarget();
                    m_TowerInfo.IsAttacking = false;
                }
                break;
            case E_TargetType.RandTarget:
                if (m_TowerInfo.IsAttacking)
                {
                    m_Target = m_AttackRange.GetRandomTarget();
                    m_TowerInfo.IsAttacking = false;
                }
                break;
            // FixTarget (Ÿ���� ��Ÿ��� ����ų� ���� ��� ����)
            case E_TargetType.FixTarget:
                if (null == m_Target || // ����ó��
                    DistanceToTarget > m_TowerInfo.DefaultSkillStat.Range) // Ÿ���� ��Ÿ��� ��� ���
                {
                    m_Target = m_AttackRange.GetNearTarget();
                }
                break;
        }
    }
    // Ÿ�� ����
    public void AttackTarget()
    {
        float Atk_spd = (m_TowerInfo_Excel.Atk_spd - m_TowerInfo.SynergyStat.Atk_spd_Fix) * m_TowerInfo.SynergyStat.Atk_spd_Percent;

        if (m_TowerInfo.AttackTimer < Atk_spd)
        {
            m_TowerInfo.AttackTimer += Time.deltaTime * 1000;
        }
        else if (null != m_Target)
        {
            m_TowerInfo.AttackTimer -= m_TowerInfo_Excel.Atk_spd;
            m_TowerInfo.IsAttacking = true;

            Skill skill = M_SkillPool.GetPool("Skill").Spawn().GetComponent<Skill>();
            skill.transform.position = transform.position;
            skill.enabled = true;
            skill.gameObject.SetActive(true);

            S_SkillConditionData_Excel conditionData = m_TowerInfo.DefaultSkillCondition;
            S_SkillStatData_Excel statData = m_TowerInfo.DefaultSkillStat;

            // �ó���, ���� ����
            statData.Dmg += m_TowerInfo.SynergyStat.Atk_Fix;
            statData.Dmg *= m_TowerInfo.SynergyStat.Atk_Percent;

            statData.Range += m_TowerInfo.SynergyStat.Range_Fix;
            statData.Range *= m_TowerInfo.SynergyStat.Range_Percent;
            m_AttackRange.SetRange(statData.Range);
            //

            skill.InitializeSkill(m_Target, conditionData, statData);
            //skill.transform.SetParent(transform);
        }
    }

    // Ÿ�� ����
    [System.Serializable]
    public struct S_TowerData
    {
        // ȸ�� �ӵ�
        public float RotateSpeed;
        // �ʱ� ȸ�� ��
        public Vector3 InitialRotation;

        // Ÿ�� ���� Ÿ�̸�
        public float AttackTimer;
        // ���� Ʈ����
        public bool IsAttacking;

        // �Ϲ� ����
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;

        #region �ó��� ����
        public S_SynergyStat SynergyStat;
        public E_AttackType Synergy_Atk_type;
        #endregion
    }
    [System.Serializable]
    public struct S_SynergyStat
    {
        // ���ݷ�
        public float Atk_Fix;
        public float Atk_Percent;
        // ��Ÿ�
        public float Range_Fix;
        public float Range_Percent;
        // ���� �ӵ�
        public float Atk_spd_Fix;
        public float Atk_spd_Percent;
        // ġ��Ÿ Ȯ��
        public float Crit_rate_Fix;
        public float Crit_rate_Percent;
        // ġ��Ÿ ����
        public float Crit_Dmg_Fix;
        public float Crit_Dmg_Percent;
    }
}
