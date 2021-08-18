using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int m_CodeTemp;
    public float m_SizeTemp;

    // Ÿ��
    public Enemy m_Target;

    // Ÿ�� ����(����)
    [SerializeField]
    protected Tower_TableExcel m_TowerInfo_Excel;
    // Ÿ�� ����
    public S_TowerData m_TowerInfo;

    #region 내부 컴포넌트
    [SerializeField]
    protected Animator m_Animator;

    [SerializeField]
    protected AttackRange m_AttackRange_Default;
    [SerializeField]
    protected AttackRange m_AttackRange_Skill01;
    [SerializeField]
    protected AttackRange m_AttackRange_Skill02;
    #endregion

    #region 내부 프로퍼티
    // 타워 매니져
    protected TowerManager M_Tower => TowerManager.Instance;
    // 스킬 매니져
    protected SkillManager M_Skill => SkillManager.Instance;
    // 적 매니져
    protected EnemyManager M_Enemy => EnemyManager.Instance;
    // 마왕 매니져
    protected DevilManager M_Devil => DevilManager.Instance;

    // 타워 회전 속도
    protected float RotateSpeed
    {
        get
        {
            return m_TowerInfo.RotateSpeed * Time.deltaTime;
        }
    }
    // 타겟까지의 거리
    protected float DistanceToTarget
    {
        get
        {
            return Vector3.Distance(transform.position, m_Target.transform.position);
        }
    }
    #endregion

    #region 외부 프로퍼티
    public Tower_TableExcel ExcelData => m_TowerInfo_Excel;
    public int SynergyCode1 => m_TowerInfo_Excel.Type1;
    public int SynergyCode2 => m_TowerInfo_Excel.Type2;
    #endregion

    #region 내부 함수
    // 타워 회전
    protected void RotateToTarget()
    {
        // 회전할 방향
        Vector3 dir;

        // 타겟이 없으면
        if (null == m_Target)
        {
            // 초기 방향으로 방향 설정
            dir = transform.position + m_TowerInfo.InitialRotation;
        }
        // 타겟이 있으면
        else
        {
            // 타겟 방향으로 방향 설정
            dir = m_Target.transform.position - transform.position;
        }

        // y 회전 방지
        dir.y = transform.position.y;
        Debug.Log("�ٶ� ��ġ: " + dir);

        // 회전
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(dir), RotateSpeed);
    }
    // Ÿ�� ������Ʈ
    protected void UpdateTarget()
    {
        // Ÿ�� ���� ���ؿ� ����
        switch ((E_TargetType)m_TowerInfo.Condition_Default.Target_type)
        {
            case E_TargetType.CloseTarget:
                if (m_TowerInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange_Default.GetNearTarget();
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
                    m_Target = m_AttackRange_Default.GetRandomTarget();
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
                    DistanceToTarget > m_TowerInfo.Stat_Default.Range) // Ÿ���� ��Ÿ��� ��� ���
                {
                    m_Target = m_AttackRange_Default.GetNearTarget();
                }
                break;
            case E_TargetType.TileTarget:
                if (m_TowerInfo.ShouldFindTarget)
                {
                    m_Target = m_AttackRange_Default.GetNearTarget();
                    m_TowerInfo.ShouldFindTarget = false;
                }
                break;
        }
    }
    // Ÿ�� ����
    protected void AttackTarget()
    {
        #region �⺻ ��ų
        // �⺻ ��ų Ÿ�̸�
        if (m_TowerInfo.AttackTimer_Default < m_TowerInfo.AttackSpeed_Default)
        {
            m_TowerInfo.AttackTimer_Default += Time.deltaTime;
        }
        // �⺻ ��ų ����
        else if (null != m_Target)
        {
            // ���� ������ ����
            m_TowerInfo.AttackTimer_Default -= m_TowerInfo.AttackSpeed_Default;

            Attack();
        }
        #endregion
        #region ��ų01
        // ��ų01
        if (m_TowerInfo.AttackTimer_Skill01 < m_TowerInfo.AttackSpeed_Skill01)
        {
            m_TowerInfo.AttackTimer_Skill01 += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            m_TowerInfo.AttackTimer_Skill01 -= m_TowerInfo.AttackSpeed_Skill01;

            Skill01();
        }
        #endregion
        #region ��ų02
        // ��ų02
        if (m_TowerInfo.AttackTimer_Skill02 < m_TowerInfo.AttackSpeed_Skill02)
        {
            m_TowerInfo.AttackTimer_Skill02 += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            m_TowerInfo.AttackTimer_Skill02 -= m_TowerInfo.AttackSpeed_Skill02;

            Skill02();
        }
        #endregion
    }

    public void Attack()
    {
        m_Animator.SetTrigger("Attack");
    }
    public void Skill01()
    {
        m_Animator.SetTrigger("Skill01");
    }
    public void Skill02()
    {
        m_Animator.SetTrigger("Skill02");
    }
    #endregion

    #region �ܺ� �Լ�
    // Ÿ�� �ʱ�ȭ
    public void InitializeTower(int code, float size = 1.0f)
    {
        #region ���� ������ ����
        m_TowerInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region ���� ������ ����
        m_TowerInfo.RotateSpeed = 5f;
        m_TowerInfo.ShouldFindTarget = true;

        // ���� �ǹ�
        m_TowerInfo.AttackPivot = transform.GetChild("AttackPivot");

        // �⺻ ��ų ������
        m_TowerInfo.Condition_Default = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.Stat_Default = M_Skill.GetStatData(m_TowerInfo.Condition_Default.PassiveCode);
        // �⺻ ��ų
        m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
        m_TowerInfo.AttackTimer_Default = m_TowerInfo.Stat_Default.CoolTime;

        // ��ų1 ������
        m_TowerInfo.Condition_Skill01 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
        m_TowerInfo.Stat_Skill01 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill01.PassiveCode);
        // ��ų1
        m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
        m_TowerInfo.AttackTimer_Skill01 = 0f;

        // ��ų2 ������
        m_TowerInfo.Condition_Skill02 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
        m_TowerInfo.Stat_Skill02 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill02.PassiveCode);
        // ��ų2
        m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
        m_TowerInfo.AttackTimer_Skill02 = 0f;

        // �ó���
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
        m_TowerInfo.BuffList = new List<BuffCC_TableExcel>();
        m_TowerInfo.BerserkerBuffList = new List<BuffCC_TableExcel>();

        // ���� ��ų
        m_TowerInfo.DevilSkillBuffList = new List<BuffCC_TableExcel>();
        #endregion

        #region ���� ������Ʈ
        m_Animator = GetComponentInChildren<Animator>();
        m_Animator.transform.localScale = Vector3.one * size;

        m_AttackRange_Default = transform.Find("AttackRange_Default").AddComponent<AttackRange>();
        m_AttackRange_Default.Range = m_TowerInfo.Stat_Default.Range;

        m_AttackRange_Skill01 = transform.Find("AttackRange_Skill01").AddComponent<AttackRange>();
        m_AttackRange_Skill01.Range = m_TowerInfo.Stat_Skill01.Range;

        m_AttackRange_Skill02 = transform.Find("AttackRange_Skill02").AddComponent<AttackRange>();
        m_AttackRange_Skill02.Range = m_TowerInfo.Stat_Skill02.Range;
        #endregion
    }

    public void CallAttack()
    {
        m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
        m_TowerInfo.ShouldFindTarget = true;

        // �⺻ ��ų ������ �ҷ�����
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Default;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Default;

        // �⺻ ����� ����
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region ����
        // ������ ���� ����Ʈ
        List<BuffCC_TableExcel> BuffList;

        // �ó��� ����
        BuffList = m_TowerInfo.BuffList;
        // ���� ���� Ȯ��
        List<float> BuffRand = new List<float>();
        // ���� ���� ����
        List<bool> BuffApply = new List<bool>();
        // ���� ���� ���
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // ������ ���� ��Ƶ� ����
        S_Buff buff;

        // ���� ����
        #region ���� �տ���
        BuffList = m_TowerInfo.BuffList;
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
                            m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                            case E_BuffType.Atk:
                                statData.Dmg += BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range += BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
                                m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                                case E_BuffType.Atk:
                                    statData.Dmg += BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
                                    m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                                m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                                    m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                                        m_TowerInfo.AttackSpeed_Default -= BuffAmount;
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                            m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                                m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
                                    m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                                m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                                    m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                                        m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                            m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                                m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
                                    m_TowerInfo.AttackSpeed_Default *= BuffAmount;
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
        m_AttackRange_Default.Range = statData.Range;
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
            conditionData.Atk_type = (int)m_TowerInfo.Synergy_Atk_type;
            statData.Target_num = m_TowerInfo.BounceCount;
        }

        // ���� ��Ÿ�� ����
        if (m_TowerInfo.ReduceCooldown)
        {
            float ReduceCooldownRand = Random.Range(0.00001f, 1f);
            bool ReduceCooldownApply = (ReduceCooldownRand <= m_TowerInfo.ReduceCooldownRand);

            if (ReduceCooldownApply)
            {
                M_Devil.Devil.ReduceSkill01Cooldown(1f);
                M_Devil.Devil.ReduceSkill02Cooldown(1f);
            }
        }
        #endregion

        // �⺻ ��ų ����ü ����
        int DefaultSkillCode = conditionData.projectile_prefab;
        if ((E_TargetType)m_TowerInfo.Condition_Default.Target_type == E_TargetType.TileTarget)
        {
            List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

            for (int i = 0; i < EnemyList.Count; ++i)
            {
                Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

                switch ((E_FireType)conditionData.Atk_pick)
                {
                    case E_FireType.Select_point:
                        break;
                    case E_FireType.Select_self:
                        DefaultSkill.transform.position = m_TowerInfo.AttackPivot.position;
                        break;
                    case E_FireType.Select_enemy:
                        GameObject pivot = new GameObject();
                        pivot.transform.position = m_Target.transform.position;
                        DefaultSkill.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                        break;
                }

                DefaultSkill.enabled = true;
                DefaultSkill.gameObject.SetActive(true);

                // �⺻ ��ų ������ ����
                DefaultSkill.InitializeSkill(EnemyList[i], conditionData, statData);
            }
        }
        else
        {
            Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);

            switch ((E_FireType)conditionData.Atk_pick)
            {
                case E_FireType.Select_point:
                    break;
                case E_FireType.Select_self:
                    DefaultSkill.transform.position = m_TowerInfo.AttackPivot.position;
                    break;
                case E_FireType.Select_enemy:
                    GameObject pivot = new GameObject();
                    pivot.transform.position = m_Target.transform.position;
                    DefaultSkill.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                    break;
            }

            DefaultSkill.enabled = true;
            DefaultSkill.gameObject.SetActive(true);

            // �⺻ ��ų ������ ����
            DefaultSkill.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    public void CallSkill01()
    {
        // ���� ������ ����
        m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
        m_TowerInfo.ShouldFindTarget = true;

        // ��ų01 ������ �ҷ�����
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill01;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill01;

        // �⺻ ����� ����
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region ����
        // ������ ���� ����Ʈ
        List<BuffCC_TableExcel> BuffList;

        // �ó��� ����
        BuffList = m_TowerInfo.BuffList;
        // ���� ���� Ȯ��
        List<float> BuffRand = new List<float>();
        // ���� ���� ����
        List<bool> BuffApply = new List<bool>();
        // ���� ���� ���
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // ������ ���� ��Ƶ� ����
        S_Buff buff;

        // ���� ����
        #region ���� �տ���
        BuffList = m_TowerInfo.BuffList;
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
                            case E_BuffType.Atk:
                                statData.Dmg += BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range += BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
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
                                case E_BuffType.Atk:
                                    statData.Dmg += BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
        m_AttackRange_Skill01.Range = statData.Range;
        #endregion

        // ��ų01 ����ü ����
        int Skill01Code = conditionData.projectile_prefab;

        if ((E_TargetType)m_TowerInfo.Condition_Skill01.Target_type == E_TargetType.TileTarget)
        {
            List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

            for (int i = 0; i < EnemyList.Count; ++i)
            {
                Skill Skill01 = M_Skill.SpawnProjectileSkill(Skill01Code);

                switch ((E_FireType)conditionData.Atk_pick)
                {
                    case E_FireType.Select_point:
                        break;
                    case E_FireType.Select_self:
                        Skill01.transform.position = m_TowerInfo.AttackPivot.position;
                        break;
                    case E_FireType.Select_enemy:
                        GameObject pivot = new GameObject();
                        pivot.transform.position = m_Target.transform.position;
                        Skill01.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                        break;
                }

                Skill01.enabled = true;
                Skill01.gameObject.SetActive(true);

                // ��ų01 ������ ����
                Skill01.InitializeSkill(EnemyList[i], conditionData, statData);
            }
        }
        else
        {
            Skill Skill01 = M_Skill.SpawnProjectileSkill(Skill01Code);

            switch ((E_FireType)conditionData.Atk_pick)
            {
                case E_FireType.Select_point:
                    break;
                case E_FireType.Select_self:
                    Skill01.transform.position = m_TowerInfo.AttackPivot.position;
                    break;
                case E_FireType.Select_enemy:
                    GameObject pivot = new GameObject();
                    pivot.transform.position = m_Target.transform.position;
                    Skill01.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                    break;
            }

            Skill01.enabled = true;
            Skill01.gameObject.SetActive(true);

            // ��ų01 ������ ����
            Skill01.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    public void CallSkill02()
    {
        // ���� ������ ����
        m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
        m_TowerInfo.ShouldFindTarget = true;

        // ��ų02 ������ �ҷ�����
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill02;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill02;

        // �⺻ ����� ����
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region ����
        // ������ ���� ����Ʈ
        List<BuffCC_TableExcel> BuffList;

        // �ó��� ����
        BuffList = m_TowerInfo.BuffList;
        // ���� ���� Ȯ��
        List<float> BuffRand = new List<float>();
        // ���� ���� ����
        List<bool> BuffApply = new List<bool>();
        // ���� ���� ���
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
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
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // ������ ���� ��Ƶ� ����
        S_Buff buff;

        // ���� ����
        #region ���� �տ���
        BuffList = m_TowerInfo.BuffList;
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
                            case E_BuffType.Atk:
                                statData.Dmg += BuffAmount;
                                break;
                            case E_BuffType.Range:
                                statData.Range += BuffAmount;
                                break;
                            case E_BuffType.Atk_spd:
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
                                case E_BuffType.Atk:
                                    statData.Dmg += BuffAmount;
                                    break;
                                case E_BuffType.Range:
                                    statData.Range += BuffAmount;
                                    break;
                                case E_BuffType.Atk_spd:
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType1,
                        BuffList[i].AddType1,
                        BuffList[i].BuffAmount1,
                        BuffList[i].BuffRand1,
                        BuffList[i].Summon1
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType2,
                            BuffList[i].AddType2,
                            BuffList[i].BuffAmount2,
                            BuffList[i].BuffRand2,
                            BuffList[i].Summon2
                            );
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
                            buff = new S_Buff(
                                BuffList[i].BuffType3,
                                BuffList[i].AddType3,
                                BuffList[i].BuffAmount3,
                                BuffList[i].BuffRand3,
                                BuffList[i].Summon3
                                );
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
                buff = new S_Buff(
                    BuffList[i].BuffType1,
                    BuffList[i].AddType1,
                    BuffList[i].BuffAmount1,
                    BuffList[i].BuffRand1,
                    BuffList[i].Summon1
                    );
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
                    buff = new S_Buff(
                        BuffList[i].BuffType2,
                        BuffList[i].AddType2,
                        BuffList[i].BuffAmount2,
                        BuffList[i].BuffRand2,
                        BuffList[i].Summon2
                        );
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
                        buff = new S_Buff(
                            BuffList[i].BuffType3,
                            BuffList[i].AddType3,
                            BuffList[i].BuffAmount3,
                            BuffList[i].BuffRand3,
                            BuffList[i].Summon3
                            );
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
        m_AttackRange_Skill02.Range = statData.Range;
        #endregion

        // ��ų02 ����ü ����
        int Skill02Code = conditionData.projectile_prefab;

        if ((E_TargetType)m_TowerInfo.Condition_Skill02.Target_type == E_TargetType.TileTarget)
        {
            List<Enemy> EnemyList = M_Enemy.GetEnemyList(m_TowerInfo.Direction);

            for (int i = 0; i < EnemyList.Count; ++i)
            {
                Skill Skill02 = M_Skill.SpawnProjectileSkill(Skill02Code);

                switch ((E_FireType)conditionData.Atk_pick)
                {
                    case E_FireType.Select_point:
                        break;
                    case E_FireType.Select_self:
                        Skill02.transform.position = m_TowerInfo.AttackPivot.position;
                        break;
                    case E_FireType.Select_enemy:
                        GameObject pivot = new GameObject();
                        pivot.transform.position = m_Target.transform.position;
                        Skill02.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                        break;
                }

                Skill02.enabled = true;
                Skill02.gameObject.SetActive(true);

                // ��ų02 ������ ����
                Skill02.InitializeSkill(EnemyList[i], conditionData, statData);
            }
        }
        else
        {
            Skill Skill02 = M_Skill.SpawnProjectileSkill(Skill02Code);

            switch ((E_FireType)conditionData.Atk_pick)
            {
                case E_FireType.Select_point:
                    break;
                case E_FireType.Select_self:
                    Skill02.transform.position = m_TowerInfo.AttackPivot.position;
                    break;
                case E_FireType.Select_enemy:
                    GameObject pivot = new GameObject();
                    pivot.transform.position = m_Target.transform.position;
                    Skill02.transform.position = pivot.transform.position; // �� �ǰ� ��ġ�� �������� ���� �ʿ�
                    break;
            }

            Skill02.enabled = true;
            Skill02.gameObject.SetActive(true);

            // ��ų02 ������ ����
            Skill02.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    #endregion

    #region ����Ƽ �ݹ� �Լ�
    private void Awake()
    {
        //InitializeTower(m_CodeTemp, m_SizeTemp);
    }

    private void Update()
    {
        RotateToTarget();
        UpdateTarget();
        AttackTarget();
    }
    #endregion

    // Ÿ�� ����
    [System.Serializable]
    public struct S_TowerData
    {
        // Ÿ�� ����
        public E_Direction Direction;
        // ȸ�� �ӵ�
        public float RotateSpeed;
        // �ʱ� ȸ�� ��
        public Vector3 InitialRotation;
        // �� ���� ����
        public bool ShouldFindTarget;
        // ���� �ǹ�
        public Transform AttackPivot;

        // �⺻ ��ų ������
        public SkillCondition_TableExcel Condition_Default;
        public SkillStat_TableExcel Stat_Default;
        // �⺻ ��ų ���� �ӵ�
        public float AttackSpeed_Default;
        // �⺻ ��ų Ÿ�̸�
        public float AttackTimer_Default;

        // ��ų01 ������
        public SkillCondition_TableExcel Condition_Skill01;
        public SkillStat_TableExcel Stat_Skill01;
        // ��ų01 ���� �ӵ�
        public float AttackSpeed_Skill01;
        // ��ų01 Ÿ�̸�
        public float AttackTimer_Skill01;

        // ��ų02 ������
        public SkillCondition_TableExcel Condition_Skill02;
        public SkillStat_TableExcel Stat_Skill02;
        // ��ų02 ���� �ӵ�
        public float AttackSpeed_Skill02;
        // ��ų02 Ÿ�̸�
        public float AttackTimer_Skill02;

        #region �ó���
        // ����
        public List<BuffCC_TableExcel> BuffList;

        // ���� Ÿ�� ����
        public E_AttackType Synergy_Atk_type;
        public int BounceCount;

        // ����Ŀ
        public bool Berserker;
        public int BerserkerStack;
        public int BerserkerMaxStack;
        public List<BuffCC_TableExcel> BerserkerBuffList;

        // ���� ��Ÿ�� ����
        public bool ReduceCooldown;
        public float ReduceCooldownRand;
        #endregion

        // ���� ��ų ����
        public List<BuffCC_TableExcel> DevilSkillBuffList;
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
