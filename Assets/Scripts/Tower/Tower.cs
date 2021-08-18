using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int m_CodeTemp;
    public float m_SizeTemp;

    // 타겟
    public Enemy m_Target;

    // 타워 정보(엑셀)
    [SerializeField]
    protected Tower_TableExcel m_TowerInfo_Excel;
    // 타워 정보
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
    public int TowerCode => m_TowerInfo_Excel.Code;
    public int SynergyCode1 => m_TowerInfo_Excel.Type1;
    public int SynergyCode2 => m_TowerInfo_Excel.Type2;
    public string Name => m_TowerInfo_Excel.Name_EN;
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
        Debug.Log("바라볼 위치: " + dir);

        // 회전
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.LookRotation(dir), RotateSpeed);
    }
    // 타겟 업데이트
    protected void UpdateTarget()
    {
        // 타겟 변경 기준에 따라
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
            // FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
            case E_TargetType.FixTarget:
                if (null == m_Target || // 예외처리
                    DistanceToTarget > m_TowerInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
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
    // 타워 공격
    protected void AttackTarget()
    {
        #region 기본 스킬
        // 기본 스킬 타이머
        if (m_TowerInfo.AttackTimer_Default < m_TowerInfo.AttackSpeed_Default)
        {
            m_TowerInfo.AttackTimer_Default += Time.deltaTime;
        }
        // 기본 스킬 공격
        else if (null != m_Target)
        {
            // 내부 데이터 정리
            m_TowerInfo.AttackTimer_Default -= m_TowerInfo.AttackSpeed_Default;

            Attack();
        }
        #endregion
        #region 스킬01
        // 스킬01
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
        #region 스킬02
        // 스킬02
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

    #region 외부 함수
    // 타워 초기화
    public void InitializeTower(int code, float size = 1.0f)
    {
        #region 엑셀 데이터 정리
        m_TowerInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region 내부 데이터 정리
        m_TowerInfo.RotateSpeed = 5f;
        m_TowerInfo.ShouldFindTarget = true;

        // 공격 피벗
        m_TowerInfo.AttackPivot = transform.GetChild("AttackPivot");

        // 기본 스킬 데이터
        m_TowerInfo.Condition_Default = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.Stat_Default = M_Skill.GetStatData(m_TowerInfo.Condition_Default.PassiveCode);
        // 기본 스킬
        m_TowerInfo.AttackSpeed_Default = m_TowerInfo.Stat_Default.CoolTime;
        m_TowerInfo.AttackTimer_Default = m_TowerInfo.Stat_Default.CoolTime;

        // 스킬1 데이터
        m_TowerInfo.Condition_Skill01 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
        m_TowerInfo.Stat_Skill01 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill01.PassiveCode);
        // 스킬1
        m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
        m_TowerInfo.AttackTimer_Skill01 = 0f;

        // 스킬2 데이터
        m_TowerInfo.Condition_Skill02 = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
        m_TowerInfo.Stat_Skill02 = M_Skill.GetStatData(m_TowerInfo.Condition_Skill02.PassiveCode);
        // 스킬2
        m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
        m_TowerInfo.AttackTimer_Skill02 = 0f;

        // 시너지
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
        m_TowerInfo.BuffList = new List<BuffCC_TableExcel>();
        m_TowerInfo.BerserkerBuffList = new List<BuffCC_TableExcel>();

        // 마왕 스킬
        m_TowerInfo.DevilSkillBuffList = new List<BuffCC_TableExcel>();
        #endregion

        #region 내부 컴포넌트
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

        // 기본 스킬 데이터 불러오기
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Default;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Default;

        // 기본 대미지 설정
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region 버프
        // 적용할 버프 리스트
        List<BuffCC_TableExcel> BuffList;

        // 시너지 버프
        BuffList = m_TowerInfo.BuffList;
        // 버프 적용 확률
        List<float> BuffRand = new List<float>();
        // 버프 적용 여부
        List<bool> BuffApply = new List<bool>();
        // 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 버서커 버프
        BuffList = m_TowerInfo.BerserkerBuffList;
        // 버서커 버프 적용 확률
        List<float> BerserkerBuffRand = new List<float>();
        // 버서커 버프 적용 여부
        List<bool> BerserkerBuffApply = new List<bool>();
        // 버서커 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 마왕 스킬 버프
        BuffList = m_TowerInfo.DevilSkillBuffList;
        // 마왕 스킬 버프 적용 확률
        List<float> DevilSkillBuffRand = new List<float>();
        // 마왕 스킬 버프 적용 여부
        List<bool> DevilSkillBuffApply = new List<bool>();
        // 마왕 스킬 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 적용할 버프 담아둘 변수
        S_Buff buff;

        // 버프 적용
        #region 버프 합연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 합연산
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

                // 버프2 체크
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

                    // 버프2 합연산
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

                    // 버프3 체크
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

                        // 버프3 합연산
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
        #region 버서커 버프 합연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 합연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 합연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 합연산
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
        #region 버프 곱연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 곱연산
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

                // 버프2 체크
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

                    // 버프2 곱연산
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

                    // 버프3 체크
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

                        // 버프3 곱연산
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
        #region 버서커 버프 곱연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 곱연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 곱연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 곱연산
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
        #region 마왕 스킬 버프 곱연산
        BuffList = m_TowerInfo.DevilSkillBuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 마왕 스킬 버프1 체크
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

                // 마왕 스킬 버프1 곱연산
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

                // 마왕 스킬 버프2 체크
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

                    // 마왕 스킬 버프2 곱연산
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

                    // 마왕 스킬 버프3 체크
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

                        // 마왕 스킬 버프3 곱연산
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

        // 사거리 업데이트
        m_AttackRange_Default.Range = statData.Range;
        #endregion

        #region 시너지
        // 버서커
        if (m_TowerInfo.Berserker)
        {
            // 버서커 스택 증가
            if (m_TowerInfo.BerserkerStack < m_TowerInfo.BerserkerMaxStack)
            {
                ++m_TowerInfo.BerserkerStack;
            }
        }

        // 공격 타입 변경
        if (m_TowerInfo.Synergy_Atk_type != E_AttackType.None)
        {
            conditionData.Atk_type = (int)m_TowerInfo.Synergy_Atk_type;
            statData.Target_num = m_TowerInfo.BounceCount;
        }

        // 마왕 쿨타임 감소
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

        // 기본 스킬 투사체 생성
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
                        DefaultSkill.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                        break;
                }

                DefaultSkill.enabled = true;
                DefaultSkill.gameObject.SetActive(true);

                // 기본 스킬 데이터 설정
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
                    DefaultSkill.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                    break;
            }

            DefaultSkill.enabled = true;
            DefaultSkill.gameObject.SetActive(true);

            // 기본 스킬 데이터 설정
            DefaultSkill.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    public void CallSkill01()
    {
        // 내부 데이터 정리
        m_TowerInfo.AttackSpeed_Skill01 = m_TowerInfo.Stat_Skill01.CoolTime;
        m_TowerInfo.ShouldFindTarget = true;

        // 스킬01 데이터 불러오기
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill01;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill01;

        // 기본 대미지 설정
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region 버프
        // 적용할 버프 리스트
        List<BuffCC_TableExcel> BuffList;

        // 시너지 버프
        BuffList = m_TowerInfo.BuffList;
        // 버프 적용 확률
        List<float> BuffRand = new List<float>();
        // 버프 적용 여부
        List<bool> BuffApply = new List<bool>();
        // 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 버서커 버프
        BuffList = m_TowerInfo.BerserkerBuffList;
        // 버서커 버프 적용 확률
        List<float> BerserkerBuffRand = new List<float>();
        // 버서커 버프 적용 여부
        List<bool> BerserkerBuffApply = new List<bool>();
        // 버서커 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 마왕 스킬 버프
        BuffList = m_TowerInfo.DevilSkillBuffList;
        // 마왕 스킬 버프 적용 확률
        List<float> DevilSkillBuffRand = new List<float>();
        // 마왕 스킬 버프 적용 여부
        List<bool> DevilSkillBuffApply = new List<bool>();
        // 마왕 스킬 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 적용할 버프 담아둘 변수
        S_Buff buff;

        // 버프 적용
        #region 버프 합연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 합연산
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

                // 버프2 체크
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

                    // 버프2 합연산
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

                    // 버프3 체크
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

                        // 버프3 합연산
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
        #region 버서커 버프 합연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 합연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 합연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 합연산
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
        #region 버프 곱연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 곱연산
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

                // 버프2 체크
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

                    // 버프2 곱연산
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

                    // 버프3 체크
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

                        // 버프3 곱연산
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
        #region 버서커 버프 곱연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 곱연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 곱연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 곱연산
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
        #region 마왕 스킬 버프 곱연산
        BuffList = m_TowerInfo.DevilSkillBuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 마왕 스킬 버프1 체크
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

                // 마왕 스킬 버프1 곱연산
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

                // 마왕 스킬 버프2 체크
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

                    // 마왕 스킬 버프2 곱연산
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

                    // 마왕 스킬 버프3 체크
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

                        // 마왕 스킬 버프3 곱연산
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

        // 사거리 업데이트
        m_AttackRange_Skill01.Range = statData.Range;
        #endregion

        // 스킬01 투사체 생성
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
                        Skill01.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                        break;
                }

                Skill01.enabled = true;
                Skill01.gameObject.SetActive(true);

                // 스킬01 데이터 설정
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
                    Skill01.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                    break;
            }

            Skill01.enabled = true;
            Skill01.gameObject.SetActive(true);

            // 스킬01 데이터 설정
            Skill01.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    public void CallSkill02()
    {
        // 내부 데이터 정리
        m_TowerInfo.AttackSpeed_Skill02 = m_TowerInfo.Stat_Skill02.CoolTime;
        m_TowerInfo.ShouldFindTarget = true;

        // 스킬02 데이터 불러오기
        SkillCondition_TableExcel conditionData = m_TowerInfo.Condition_Skill02;
        SkillStat_TableExcel statData = m_TowerInfo.Stat_Skill02;

        // 기본 대미지 설정
        statData.Dmg *= m_TowerInfo_Excel.Atk;
        statData.Dmg += statData.Dmg_plus;

        #region 버프
        // 적용할 버프 리스트
        List<BuffCC_TableExcel> BuffList;

        // 시너지 버프
        BuffList = m_TowerInfo.BuffList;
        // 버프 적용 확률
        List<float> BuffRand = new List<float>();
        // 버프 적용 여부
        List<bool> BuffApply = new List<bool>();
        // 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BuffRand.Add(Random.Range(0.00001f, 1f));
            BuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 버서커 버프
        BuffList = m_TowerInfo.BerserkerBuffList;
        // 버서커 버프 적용 확률
        List<float> BerserkerBuffRand = new List<float>();
        // 버서커 버프 적용 여부
        List<bool> BerserkerBuffApply = new List<bool>();
        // 버서커 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            BerserkerBuffRand.Add(Random.Range(0.00001f, 1f));
            BerserkerBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 마왕 스킬 버프
        BuffList = m_TowerInfo.DevilSkillBuffList;
        // 마왕 스킬 버프 적용 확률
        List<float> DevilSkillBuffRand = new List<float>();
        // 마왕 스킬 버프 적용 여부
        List<bool> DevilSkillBuffApply = new List<bool>();
        // 마왕 스킬 버프 적용 계산
        for (int i = 0; i < BuffList.Count; ++i)
        {
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType1 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand1);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType2 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand2);
            DevilSkillBuffRand.Add(Random.Range(0.00001f, 1f));
            DevilSkillBuffApply.Add((E_BuffType)BuffList[i].BuffType3 == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].BuffRand3);
        }

        // 적용할 버프 담아둘 변수
        S_Buff buff;

        // 버프 적용
        #region 버프 합연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 합연산
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

                // 버프2 체크
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

                    // 버프2 합연산
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

                    // 버프3 체크
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

                        // 버프3 합연산
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
        #region 버서커 버프 합연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 합연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 합연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 합연산
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
        #region 버프 곱연산
        BuffList = m_TowerInfo.BuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 버프1 체크
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

                // 버프1 곱연산
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

                // 버프2 체크
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

                    // 버프2 곱연산
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

                    // 버프3 체크
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

                        // 버프3 곱연산
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
        #region 버서커 버프 곱연산
        BuffList = m_TowerInfo.BerserkerBuffList;
        if (m_TowerInfo.Berserker)
        {
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버서커 버프1 체크
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

                    // 버서커 버프1 곱연산
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

                    // 버서커 버프2 체크
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

                        // 버서커 버프2 곱연산
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

                        // 버서커 버프3 체크
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

                            // 버서커 버프3 곱연산
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
        #region 마왕 스킬 버프 곱연산
        BuffList = m_TowerInfo.DevilSkillBuffList;
        for (int i = 0; i < BuffList.Count; ++i)
        {
            // 마왕 스킬 버프1 체크
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

                // 마왕 스킬 버프1 곱연산
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

                // 마왕 스킬 버프2 체크
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

                    // 마왕 스킬 버프2 곱연산
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

                    // 마왕 스킬 버프3 체크
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

                        // 마왕 스킬 버프3 곱연산
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

        // 사거리 업데이트
        m_AttackRange_Skill02.Range = statData.Range;
        #endregion

        // 스킬02 투사체 생성
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
                        Skill02.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                        break;
                }

                Skill02.enabled = true;
                Skill02.gameObject.SetActive(true);

                // 스킬02 데이터 설정
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
                    Skill02.transform.position = pivot.transform.position; // 적 피격 위치에 생성으로 수정 필요
                    break;
            }

            Skill02.enabled = true;
            Skill02.gameObject.SetActive(true);

            // 스킬02 데이터 설정
            Skill02.InitializeSkill(m_Target, conditionData, statData);
        }
    }
    #endregion

    #region 유니티 콜백 함수
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

    // 타워 정보
    [System.Serializable]
    public struct S_TowerData
    {
        // 타워 방향
        public E_Direction Direction;
        // 회전 속도
        public float RotateSpeed;
        // 초기 회전 값
        public Vector3 InitialRotation;
        // 적 감지 여부
        public bool ShouldFindTarget;
        // 공격 피벗
        public Transform AttackPivot;

        // 기본 스킬 데이터
        public SkillCondition_TableExcel Condition_Default;
        public SkillStat_TableExcel Stat_Default;
        // 기본 스킬 공격 속도
        public float AttackSpeed_Default;
        // 기본 스킬 타이머
        public float AttackTimer_Default;

        // 스킬01 데이터
        public SkillCondition_TableExcel Condition_Skill01;
        public SkillStat_TableExcel Stat_Skill01;
        // 스킬01 공격 속도
        public float AttackSpeed_Skill01;
        // 스킬01 타이머
        public float AttackTimer_Skill01;

        // 스킬02 데이터
        public SkillCondition_TableExcel Condition_Skill02;
        public SkillStat_TableExcel Stat_Skill02;
        // 스킬02 공격 속도
        public float AttackSpeed_Skill02;
        // 스킬02 타이머
        public float AttackTimer_Skill02;

        #region 시너지
        // 버프
        public List<BuffCC_TableExcel> BuffList;

        // 공격 타입 변경
        public E_AttackType Synergy_Atk_type;
        public int BounceCount;

        // 버서커
        public bool Berserker;
        public int BerserkerStack;
        public int BerserkerMaxStack;
        public List<BuffCC_TableExcel> BerserkerBuffList;

        // 마왕 쿨타임 감소
        public bool ReduceCooldown;
        public float ReduceCooldownRand;
        #endregion

        // 마왕 스킬 버프
        public List<BuffCC_TableExcel> DevilSkillBuffList;
    }
    //[System.Serializable]
    //public struct S_BuffStat
    //{
    //    // 적용 확률
    //    public float BuffRand;

    //    // 공격력
    //    public float Atk_Fix;
    //    public float Atk_Percent;
    //    // 사거리
    //    public float Range_Fix;
    //    public float Range_Percent;
    //    // 공격 속도
    //    public float Atk_spd_Fix;
    //    public float Atk_spd_Percent;
    //    // 치명타 확률
    //    public float Crit_rate_Fix;
    //    public float Crit_rate_Percent;
    //    // 치명타 배율
    //    public float Crit_Dmg_Fix;
    //    public float Crit_Dmg_Percent;

    //    public void Initialize()
    //    {
    //        // 적용 확률
    //        BuffRand = 1f;

    //        // 공격력
    //        Atk_Fix = 0f;
    //        Atk_Percent = 1f;
    //        // 사거리
    //        Range_Fix = 0f;
    //        Range_Percent = 1f;
    //        // 공격 속도
    //        Atk_spd_Fix = 0f;
    //        Atk_spd_Percent = 1f;
    //        // 치명타 확률
    //        Crit_rate_Fix = 0f;
    //        Crit_rate_Percent = 1f;
    //        // 치명타 배율
    //        Crit_Dmg_Fix = 0f;
    //        Crit_Dmg_Percent = 1f;
    //    }
    //}
}
