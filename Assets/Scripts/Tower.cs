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
        m_TowerInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region ���� ������ ����
        // ��Ÿ
        m_TowerInfo.DefaultSkillCondition = Skill_Condition.GetData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.DefaultSkillStat = Skill_Stat.GetData(m_TowerInfo_Excel.Atk_Code);

        // ����
        m_TowerInfo.AttackSpeed = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.AttackTimer = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.IsAttacking = true;

        // �ó���
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
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
        if (m_TowerInfo.AttackTimer < m_TowerInfo.AttackSpeed)
        {
            m_TowerInfo.AttackTimer += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            // ���� ������ ����
            m_TowerInfo.AttackTimer -= m_TowerInfo.AttackSpeed;
            m_TowerInfo.AttackSpeed = m_TowerInfo_Excel.Atk_spd;
            m_TowerInfo.IsAttacking = true;

            // ��ų ����
            Skill skill = M_SkillPool.GetPool("Skill").Spawn().GetComponent<Skill>();
            skill.transform.position = transform.position;
            skill.enabled = true;
            skill.gameObject.SetActive(true);

            // ��ų ������ �ҷ�����
            S_SkillConditionData_Excel conditionData = m_TowerInfo.DefaultSkillCondition;
            S_SkillStatData_Excel statData = m_TowerInfo.DefaultSkillStat;

            #region �ó���

            #region ����
            // ���� ���� Ȯ�� ���
            float Buff1Rand = Random.Range(0f, 1f);
            float Buff2Rand = Random.Range(0f, 1f);
            float Buff3Rand = Random.Range(0f, 1f);
            // ���� ���� ���� ����
            bool Buff1Apply = (m_TowerInfo.Buff1.BuffType == E_BuffType.None) ? false : Buff1Rand <= m_TowerInfo.Buff1.BuffRand;
            bool Buff2Apply = (m_TowerInfo.Buff2.BuffType == E_BuffType.None) ? false : Buff2Rand <= m_TowerInfo.Buff2.BuffRand;
            bool Buff3Apply = (m_TowerInfo.Buff3.BuffType == E_BuffType.None) ? false : Buff3Rand <= m_TowerInfo.Buff3.BuffRand;

            // ����1 üũ
            if (Buff1Apply)
            {
                // ����1 �տ���
                if (m_TowerInfo.Buff1.AddType == E_AddType.Fix)
                {
                    switch (m_TowerInfo.Buff1.BuffType)
                    {
                        case E_BuffType.Atk:
                            statData.Dmg += m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Range:
                            statData.Range += m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Atk_spd:
                            m_TowerInfo.AttackSpeed -= m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Crit_rate:
                            break;
                        case E_BuffType.Crit_Dmg:
                            break;
                    }
                }

                // ����2 üũ
                if (Buff2Apply)
                {
                    // ����2 �տ���
                    if (m_TowerInfo.Buff2.AddType == E_AddType.Fix)
                    {
                        switch (m_TowerInfo.Buff2.BuffType)
                        {
                            case E_BuffType.Atk:
                                statData.Dmg += m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range += m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed -= m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Crit_rate:
                                break;
                            case E_BuffType.Crit_Dmg:
                                break;
                        }
                    }

                    // ����3 üũ
                    if (Buff3Apply)
                    {
                        // ����3 �տ���
                        if (m_TowerInfo.Buff3.AddType == E_AddType.Fix)
                        {
                            switch (m_TowerInfo.Buff3.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg += m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed -= m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }
                    }
                }

                // ����1 ������
                if (m_TowerInfo.Buff1.AddType == E_AddType.Percent)
                {
                    switch (m_TowerInfo.Buff1.BuffType)
                    {
                        case E_BuffType.Atk:
                            statData.Dmg *= m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Range:
                            statData.Range *= m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Atk_spd:
                            m_TowerInfo.AttackSpeed *= m_TowerInfo.Buff1.BuffAmount;
                            break;
                        case E_BuffType.Crit_rate:
                            break;
                        case E_BuffType.Crit_Dmg:
                            break;
                    }
                }

                // ����2 üũ
                if (Buff2Apply)
                {
                    // ����2 ������
                    if (m_TowerInfo.Buff1.AddType == E_AddType.Percent)
                    {
                        switch (m_TowerInfo.Buff2.BuffType)
                        {
                            case E_BuffType.Atk:
                                statData.Dmg *= m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range *= m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed *= m_TowerInfo.Buff2.BuffAmount;
                                break;
                            case E_BuffType.Crit_rate:
                                break;
                            case E_BuffType.Crit_Dmg:
                                break;
                        }
                    }

                    // ����3 üũ
                    if (Buff3Apply)
                    {
                        // ����3 ������
                        if (m_TowerInfo.Buff1.AddType == E_AddType.Percent)
                        {
                            switch (m_TowerInfo.Buff3.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg *= m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range *= m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed *= m_TowerInfo.Buff3.BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }
                    }
                }
            }
            
            // ��Ÿ� ������Ʈ
            m_AttackRange.SetRange(statData.Range);
            #endregion

            // ���� Ÿ�� ����
            if (m_TowerInfo.Synergy_Atk_type != E_AttackType.None)
            {
                conditionData.Atk_type = m_TowerInfo.Synergy_Atk_type;
            }

            #endregion

            // ��ų ������ ����
            skill.InitializeSkill(m_Target, conditionData, statData);
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

        // Ÿ�� ���� �ӵ�
        public float AttackSpeed;
        // Ÿ�� ���� Ÿ�̸�
        public float AttackTimer;
        // ���� Ʈ����
        public bool IsAttacking;

        // �Ϲ� ����
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;

        #region �ó��� ����
        // ����
        public S_Buff Buff1;
        public S_Buff Buff2;
        public S_Buff Buff3;
        // ���� Ÿ�� ����
        public E_AttackType Synergy_Atk_type;
        #endregion
    }
    //[System.Serializable]
    //public struct S_BuffStat
    //{
    //    // ���� Ȯ��
    //    public float BuffRand;

    //    // ���ݷ�
    //    public float Atk_Fix;
    //    public float Atk_Percent;
    //    // ��Ÿ�
    //    public float Range_Fix;
    //    public float Range_Percent;
    //    // ���� �ӵ�
    //    public float Atk_spd_Fix;
    //    public float Atk_spd_Percent;
    //    // ġ��Ÿ Ȯ��
    //    public float Crit_rate_Fix;
    //    public float Crit_rate_Percent;
    //    // ġ��Ÿ ����
    //    public float Crit_Dmg_Fix;
    //    public float Crit_Dmg_Percent;

    //    public void Initialize()
    //    {
    //        // ���� Ȯ��
    //        BuffRand = 1f;

    //        // ���ݷ�
    //        Atk_Fix = 0f;
    //        Atk_Percent = 1f;
    //        // ��Ÿ�
    //        Range_Fix = 0f;
    //        Range_Percent = 1f;
    //        // ���� �ӵ�
    //        Atk_spd_Fix = 0f;
    //        Atk_spd_Percent = 1f;
    //        // ġ��Ÿ Ȯ��
    //        Crit_rate_Fix = 0f;
    //        Crit_rate_Percent = 1f;
    //        // ġ��Ÿ ����
    //        Crit_Dmg_Fix = 0f;
    //        Crit_Dmg_Percent = 1f;
    //    }
    //}
}
