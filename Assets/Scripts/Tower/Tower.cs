using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Ÿ�� �ڵ�(�ӽ�)
    public int m_TempCode;
    // Ÿ��
    public GameObject m_Target;

    // Ÿ�� ����(����)
    protected S_TowerData_Excel m_TowerInfo_Excel;
    // Ÿ�� ����
    public S_TowerData m_TowerInfo;

    #region ���� ������Ʈ
    protected AttackRange m_AttackRange;
    #endregion

    #region ���� ������Ƽ
    // ���ҽ� �Ŵ���
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    // Ÿ�� �Ŵ���
    protected TowerManager M_Tower => TowerManager.Instance;
    // ��ų �޸�Ǯ
    protected SkillPool M_SkillPool => SkillPool.Instance;

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

    #region �ܺ� ������Ƽ
    public int TowerCode => m_TowerInfo_Excel.Code;
    public int SynergyCode1 => m_TowerInfo_Excel.Type1;
    public int SynergyCode2 => m_TowerInfo_Excel.Type2;
    #endregion

    private void Awake()
    {
        InitializeTower(m_TempCode);
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
        m_TowerInfo.ShouldFindTarget = true;

        // �ó���
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
        m_TowerInfo.BuffList = new List<S_BuffData_Excel>();
        m_TowerInfo.BerserkerBuffList = new List<S_BuffData_Excel>();

        // ���� ��ų
        m_TowerInfo.DevilSkillBuffList = new List<S_BuffData_Excel>();
        #endregion

        #region ���� ������Ʈ
        m_AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_AttackRange.Initialize();
        m_AttackRange.SetRange(m_TowerInfo.DefaultSkillStat.Range);
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
    // Ÿ�� ������Ʈ
    public void UpdateTarget()
    {
        // Ÿ�� ���� ���ؿ� ����
        switch (m_TowerInfo.DefaultSkillCondition.Target_type)
        {
            case E_TargetType.CloseTarget:
                if (m_TowerInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange.GetNearTarget();
                    m_TowerInfo.ShouldFindTarget = false;

                    if (m_TowerInfo.Berserker)
                    {
                        m_TowerInfo.BerserkerStack = 0;
                    }
                }
                break;
            case E_TargetType.RandTarget:
                if (m_TowerInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange.GetRandomTarget();
                    m_TowerInfo.ShouldFindTarget = false;

                    if (m_TowerInfo.Berserker)
                    {
                        m_TowerInfo.BerserkerStack = 0;
                    }
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
            m_TowerInfo.ShouldFindTarget = true;

            // ��ų ����
            Skill skill = M_SkillPool.GetPool("Skill").Spawn();
            skill.transform.position = transform.position;
            skill.enabled = true;
            skill.gameObject.SetActive(true);

            // ��ų ������ �ҷ�����
            S_SkillConditionData_Excel conditionData = m_TowerInfo.DefaultSkillCondition;
            S_SkillStatData_Excel statData = m_TowerInfo.DefaultSkillStat;

            #region ����
            // ������ ���� ����Ʈ
            List<S_BuffData_Excel> BuffList = null;

            // �ó��� ����
            BuffList = m_TowerInfo.BuffList;
            // ���� ���� Ȯ��
            List<float> BuffRand = new List<float>();
            // ���� ���� ����
            List<bool> BuffApply = new List<bool>();
            // ���� ���� ���
            for (int i = 0; i < BuffList.Count; ++i)
            {
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
            }

            // ����Ŀ ����
            BuffList = m_TowerInfo.BerserkerBuffList;
            // ����Ŀ ���� ���� Ȯ��
            List<float> BerserkerBuffRand = new List<float>();
            // ����Ŀ ���� ���� ����
            List<bool> BerserkerBuffApply = new List<bool>();
            // ����Ŀ ���� ���� ���
            for (int i = 0; i < BuffList.Count; ++i)
            {
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
            }

            // ���� ��ų ����
            BuffList = m_TowerInfo.DevilSkillBuffList;
            // ���� ��ų ���� ���� Ȯ��
            List<float> DevilSkillBuffRand = new List<float>();
            // ���� ��ų ���� ���� ����
            List<bool> DevilSkillBuffApply = new List<bool>();
            // ���� ��ų ���� ���� ���
            for (int i = 0; i < BuffList.Count; ++i)
            {
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
            }

            #region ���� �տ���
            BuffList = m_TowerInfo.BuffList;
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // ����1 üũ
                if (BuffApply[i * 3])
                {
                    S_Buff buff = BuffList[i].Buff1;
                    float BuffAmount = buff.BuffAmount;

                    // ����1 �տ���
                    if (buff.AddType == E_AddType.Fix)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Atk:
                                statData.Dmg += BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range += BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed -= BuffAmount;
                                break;
                            case E_BuffType.Crit_rate:
                                break;
                            case E_BuffType.Crit_Dmg:
                                break;
                        }
                    }

                    // ����2 üũ
                    if (BuffApply[i * 3 + 1])
                    {
                        buff = BuffList[i].Buff2;
                        BuffAmount = buff.BuffAmount;

                        // ����2 �տ���
                        if (buff.AddType == E_AddType.Fix)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg += BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed -= BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }

                        // ����3 üũ
                        if (BuffApply[i * 3 + 2])
                        {
                            buff = BuffList[i].Buff3;
                            BuffAmount = buff.BuffAmount;

                            // ����3 �տ���
                            if (buff.AddType == E_AddType.Fix)
                            {
                                switch (buff.BuffType)
                                {
                                    case E_BuffType.Atk:
                                        statData.Dmg += BuffAmount;
                                        break;
                                    case E_BuffType.Range:
                                        statData.Range += BuffAmount;
                                        break;
                                    case E_BuffType.Atk_spd:
                                        m_TowerInfo.AttackSpeed -= BuffAmount;
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
            }
            #endregion

            #region ����Ŀ ���� �տ���
            BuffList = m_TowerInfo.BerserkerBuffList;
            if (m_TowerInfo.Berserker)
            {
                for (int i = 0; i < BuffList.Count; ++i)
                {
                    // ����Ŀ ����1 üũ
                    if (BerserkerBuffApply[i * 3])
                    {
                        S_Buff buff = BuffList[i].Buff1;
                        float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                        // ����Ŀ ����1 �տ���
                        if (buff.AddType == E_AddType.Fix)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg += BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed -= BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }

                        // ����Ŀ ����2 üũ
                        if (BerserkerBuffApply[i * 3 + 1])
                        {
                            buff = BuffList[i].Buff2;
                            BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                            // ����Ŀ ����2 �տ���
                            if (buff.AddType == E_AddType.Fix)
                            {
                                switch (buff.BuffType)
                                {
                                    case E_BuffType.Atk:
                                        statData.Dmg += BuffAmount;
                                        break;
                                    case E_BuffType.Range:
                                        statData.Range += BuffAmount;
                                        break;
                                    case E_BuffType.Atk_spd:
                                        m_TowerInfo.AttackSpeed -= BuffAmount;
                                        break;
                                    case E_BuffType.Crit_rate:
                                        break;
                                    case E_BuffType.Crit_Dmg:
                                        break;
                                }
                            }

                            // ����Ŀ ����3 üũ
                            if (BerserkerBuffApply[i * 3 + 2])
                            {
                                buff = BuffList[i].Buff3;
                                BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                                // ����Ŀ ����3 �տ���
                                if (buff.AddType == E_AddType.Fix)
                                {
                                    switch (buff.BuffType)
                                    {
                                        case E_BuffType.Atk:
                                            statData.Dmg += BuffAmount;
                                            break;
                                        case E_BuffType.Range:
                                            statData.Range += BuffAmount;
                                            break;
                                        case E_BuffType.Atk_spd:
                                            m_TowerInfo.AttackSpeed -= BuffAmount;
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
                }
            }
            #endregion

            #region ���� ������
            BuffList = m_TowerInfo.BuffList;
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // ����1 üũ
                if (BuffApply[i * 3])
                {
                    S_Buff buff = BuffList[i].Buff1;
                    float BuffAmount = buff.BuffAmount;

                    // ����1 ������
                    if (buff.AddType == E_AddType.Percent)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Atk:
                                statData.Dmg *= BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range *= BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed *= BuffAmount;
                                break;
                            case E_BuffType.Crit_rate:
                                break;
                            case E_BuffType.Crit_Dmg:
                                break;
                        }
                    }

                    // ����2 üũ
                    if (BuffApply[i * 3 + 1])
                    {
                        buff = BuffList[i].Buff2;
                        BuffAmount = buff.BuffAmount;

                        // ����2 ������
                        if (buff.AddType == E_AddType.Percent)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg *= BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range *= BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed *= BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }

                        // ����3 üũ
                        if (BuffApply[i * 3 + 2])
                        {
                            buff = BuffList[i].Buff3;
                            BuffAmount = buff.BuffAmount;

                            // ����3 ������
                            if (buff.AddType == E_AddType.Percent)
                            {
                                switch (buff.BuffType)
                                {
                                    case E_BuffType.Atk:
                                        statData.Dmg *= BuffAmount;
                                        break;
                                    case E_BuffType.Range:
                                        statData.Range *= BuffAmount;
                                        break;
                                    case E_BuffType.Atk_spd:
                                        m_TowerInfo.AttackSpeed *= BuffAmount;
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
            }
            #endregion

            #region ����Ŀ ���� ������
            BuffList = m_TowerInfo.BerserkerBuffList;
            if (m_TowerInfo.Berserker)
            {
                for (int i = 0; i < BuffList.Count; ++i)
                {
                    // ����Ŀ ����1 üũ
                    if (BerserkerBuffApply[i * 3])
                    {
                        S_Buff buff = BuffList[i].Buff1;
                        float BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                        // ����Ŀ ����1 ������
                        if (buff.AddType == E_AddType.Percent)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg *= BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range *= BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed *= BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }

                        // ����Ŀ ����2 üũ
                        if (BerserkerBuffApply[i * 3 + 1])
                        {
                            buff = BuffList[i].Buff2;
                            BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                            // ����Ŀ ����2 ������
                            if (buff.AddType == E_AddType.Percent)
                            {
                                switch (buff.BuffType)
                                {
                                    case E_BuffType.Atk:
                                        statData.Dmg *= BuffAmount;
                                        break;
                                    case E_BuffType.Range:
                                        statData.Range *= BuffAmount;
                                        break;
                                    case E_BuffType.Atk_spd:
                                        m_TowerInfo.AttackSpeed *= BuffAmount;
                                        break;
                                    case E_BuffType.Crit_rate:
                                        break;
                                    case E_BuffType.Crit_Dmg:
                                        break;
                                }
                            }

                            // ����Ŀ ����3 üũ
                            if (BerserkerBuffApply[i * 3 + 2])
                            {
                                buff = BuffList[i].Buff3;
                                BuffAmount = buff.BuffAmount * m_TowerInfo.BerserkerStack;

                                // ����Ŀ ����3 ������
                                if (buff.AddType == E_AddType.Percent)
                                {
                                    switch (buff.BuffType)
                                    {
                                        case E_BuffType.Atk:
                                            statData.Dmg *= BuffAmount;
                                            break;
                                        case E_BuffType.Range:
                                            statData.Range *= BuffAmount;
                                            break;
                                        case E_BuffType.Atk_spd:
                                            m_TowerInfo.AttackSpeed *= BuffAmount;
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
                }
            }
            #endregion

            #region ���� ��ų ���� ������
            BuffList = m_TowerInfo.DevilSkillBuffList;
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // ���� ��ų ����1 üũ
                if (DevilSkillBuffApply[i * 3])
                {
                    S_Buff buff = BuffList[i].Buff1;
                    float BuffAmount = buff.BuffAmount;

                    // ���� ��ų ����1 ������
                    if (buff.AddType == E_AddType.Percent)
                    {
                        switch (buff.BuffType)
                        {
                            case E_BuffType.Atk:
                                statData.Dmg *= BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range *= BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed *= BuffAmount;
                                break;
                            case E_BuffType.Crit_rate:
                                break;
                            case E_BuffType.Crit_Dmg:
                                break;
                        }
                    }

                    // ���� ��ų ����2 üũ
                    if (DevilSkillBuffApply[i * 3 + 1])
                    {
                        buff = BuffList[i].Buff2;
                        BuffAmount = buff.BuffAmount;

                        // ���� ��ų ����2 ������
                        if (buff.AddType == E_AddType.Percent)
                        {
                            switch (buff.BuffType)
                            {
                                case E_BuffType.Atk:
                                    statData.Dmg *= BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range *= BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed *= BuffAmount;
                                    break;
                                case E_BuffType.Crit_rate:
                                    break;
                                case E_BuffType.Crit_Dmg:
                                    break;
                            }
                        }

                        // ���� ��ų ����3 üũ
                        if (DevilSkillBuffApply[i * 3 + 2])
                        {
                            buff = BuffList[i].Buff3;
                            BuffAmount = buff.BuffAmount;

                            // ���� ��ų ����3 ������
                            if (buff.AddType == E_AddType.Percent)
                            {
                                switch (buff.BuffType)
                                {
                                    case E_BuffType.Atk:
                                        statData.Dmg *= BuffAmount;
                                        break;
                                    case E_BuffType.Range:
                                        statData.Range *= BuffAmount;
                                        break;
                                    case E_BuffType.Atk_spd:
                                        m_TowerInfo.AttackSpeed *= BuffAmount;
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
            }
            #endregion

            // ��Ÿ� ������Ʈ
            m_AttackRange.SetRange(statData.Range);
            #endregion

            #region �ó���
            // ����Ŀ
            if (m_TowerInfo.Berserker)
            {
                // ����Ŀ ���� ����
                if (m_TowerInfo.BerserkerStack < m_TowerInfo.BerserkerMaxStack)
                {
                    ++m_TowerInfo.BerserkerStack;
                }
            }

            // ���� Ÿ�� ����
            if (m_TowerInfo.Synergy_Atk_type != E_AttackType.None)
            {
                conditionData.Atk_type = m_TowerInfo.Synergy_Atk_type;
                statData.Target_num = m_TowerInfo.BounceCount;
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
        // �� ���� ����
        public bool ShouldFindTarget;

        // �Ϲ� ����
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;

        #region �ó��� ����
        // ����
        public List<S_BuffData_Excel> BuffList;

        // ���� Ÿ�� ����
        public E_AttackType Synergy_Atk_type;
        public int BounceCount;

        // ����Ŀ
        public bool Berserker;
        public int BerserkerStack;
        public int BerserkerMaxStack;
        public List<S_BuffData_Excel> BerserkerBuffList;

        #endregion

        // ���� ��ų ����
        public List<S_BuffData_Excel> DevilSkillBuffList;
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
