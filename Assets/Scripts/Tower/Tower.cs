using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // 타워 코드(임시)
    public int m_TempCode;
    // 타겟
    public GameObject m_Target;

    // 타워 정보(엑셀)
    [SerializeField]
    protected S_TowerData_Excel m_TowerInfo_Excel;
    // 타워 정보
    public S_TowerData m_TowerInfo;

    #region 내부 컴포넌트
    protected AttackRange m_AttackRange;
    #endregion

    #region 내부 프로퍼티
    // 타워 매니져
    protected TowerManager M_Tower => TowerManager.Instance;
    // 스킬 매니져
    protected SkillManager M_Skill => SkillManager.Instance;

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
    #endregion

    private void Awake()
    {
        InitializeTower(m_TempCode);
    }

    // 타워 초기화
    public void InitializeTower(int code)
    {
        #region 엑셀 데이터 정리
        m_TowerInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region 내부 데이터 정리
        m_TowerInfo.RotateSpeed = 5f;
        m_TowerInfo.InitialRotation = transform.eulerAngles;
        m_TowerInfo.ShouldFindTarget = true;

        // 기본 스킬 데이터
        m_TowerInfo.DefaultSkillCondition = M_Skill.GetConditionData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.DefaultSkillStat = M_Skill.GetStatData(m_TowerInfo.DefaultSkillCondition.PassiveCode);
        // 기본 스킬
        m_TowerInfo.DefaultSkillAttackSpeed = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.DefaultSkillTimer = m_TowerInfo_Excel.Atk_spd;

        // 스킬1 데이터
        m_TowerInfo.Skill01Condition = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill1Code);
        m_TowerInfo.Skill01Stat = M_Skill.GetStatData(m_TowerInfo.Skill01Condition.PassiveCode);
        // 스킬1
        m_TowerInfo.Skill01AttackSpeed = m_TowerInfo.Skill01Stat.CoolTime;
        m_TowerInfo.Skill01Timer = 0f;

        // 스킬2 데이터
        m_TowerInfo.Skill02Condition = M_Skill.GetConditionData(m_TowerInfo_Excel.Skill2Code);
        m_TowerInfo.Skill02Stat = M_Skill.GetStatData(m_TowerInfo.Skill02Condition.PassiveCode);
        // 스킬2
        m_TowerInfo.Skill02AttackSpeed = m_TowerInfo.Skill02Stat.CoolTime;
        m_TowerInfo.Skill02Timer = 0f;

        // 시너지
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
        m_TowerInfo.BuffList = new List<S_BuffData_Excel>();
        m_TowerInfo.BerserkerBuffList = new List<S_BuffData_Excel>();

        // 마왕 스킬
        m_TowerInfo.DevilSkillBuffList = new List<S_BuffData_Excel>();
        #endregion

        #region 내부 컴포넌트
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

    // 타워 회전
    public void RotateToTarget()
    {
        // 회전할 방향
        Vector3 dir;

        // 타겟이 없으면
        if (null == m_Target)
        {
            // 초기 방향으로 방향 설정
            dir = m_TowerInfo.InitialRotation;
        }
        // 타겟이 있으면
        else
        {
            // 타겟 방향으로 방향 설정
            dir = m_Target.transform.position - transform.position;
        }

        // 회전
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), RotateSpeed);
    }
    // 타겟 업데이트
    public void UpdateTarget()
    {
        // 타겟 변경 기준에 따라
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
            // FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
            case E_TargetType.FixTarget:
                if (null == m_Target || // 예외처리
                    DistanceToTarget > m_TowerInfo.DefaultSkillStat.Range) // 타겟이 사거리를 벗어난 경우
                {
                    m_Target = m_AttackRange.GetNearTarget();
                }
                break;
        }
    }
    // 타워 공격
    public void AttackTarget()
    {
        if (m_TowerInfo.DefaultSkillTimer < m_TowerInfo.DefaultSkillAttackSpeed)
        {
            m_TowerInfo.DefaultSkillTimer += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            // 내부 데이터 정리
            m_TowerInfo.DefaultSkillTimer -= m_TowerInfo.DefaultSkillAttackSpeed;
            m_TowerInfo.DefaultSkillAttackSpeed = m_TowerInfo_Excel.Atk_spd;
            m_TowerInfo.ShouldFindTarget = true;

            // 기본 스킬 데이터 불러오기
            S_SkillConditionData_Excel conditionData = m_TowerInfo.DefaultSkillCondition;
            S_SkillStatData_Excel statData = m_TowerInfo.DefaultSkillStat;

            #region 버프
            // 적용할 버프 리스트
            List<S_BuffData_Excel> BuffList = null;

            // 시너지 버프
            BuffList = m_TowerInfo.BuffList;
            // 버프 적용 확률
            List<float> BuffRand = new List<float>();
            // 버프 적용 여부
            List<bool> BuffApply = new List<bool>();
            // 버프 적용 계산
            for (int i = 0; i < BuffList.Count; ++i)
            {
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : BuffRand[BuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
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
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : BerserkerBuffRand[BerserkerBuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
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
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff1.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff1.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff2.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff2.BuffRand);
                BuffRand.Add(Random.Range(0f, 1f));
                BuffApply.Add(BuffList[i].Buff3.BuffType == E_BuffType.None ? false : DevilSkillBuffRand[DevilSkillBuffRand.Count - 1] <= BuffList[i].Buff3.BuffRand);
            }

            #region 버프 합연산
            BuffList = m_TowerInfo.BuffList;
            for (int i = 0; i < BuffList.Count; ++i)
            {
                // 버프1 체크
                if (BuffApply[i * 3])
                {
                    S_Buff buff = BuffList[i].Buff1;
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
                                m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                        buff = BuffList[i].Buff2;
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
                                    m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                            buff = BuffList[i].Buff3;
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
                                        m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                        S_Buff buff = BuffList[i].Buff1;
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
                                    m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                            buff = BuffList[i].Buff2;
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
                                        m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                                buff = BuffList[i].Buff3;
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
                                            m_TowerInfo.DefaultSkillAttackSpeed -= BuffAmount;
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
                    S_Buff buff = BuffList[i].Buff1;
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
                                m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                        buff = BuffList[i].Buff2;
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
                                    m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                            buff = BuffList[i].Buff3;
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
                                        m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                        S_Buff buff = BuffList[i].Buff1;
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
                                    m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                            buff = BuffList[i].Buff2;
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
                                        m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                                buff = BuffList[i].Buff3;
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
                                            m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                    S_Buff buff = BuffList[i].Buff1;
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
                                m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                        buff = BuffList[i].Buff2;
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
                                    m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
                            buff = BuffList[i].Buff3;
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
                                        m_TowerInfo.DefaultSkillAttackSpeed *= BuffAmount;
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
            m_AttackRange.SetRange(statData.Range);
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
                conditionData.Atk_type = m_TowerInfo.Synergy_Atk_type;
                statData.Target_num = m_TowerInfo.BounceCount;
            }

            #endregion

            // 기본 스킬 투사체 생성
            int DefaultSkillCode = conditionData.projectile_prefab;
            Skill DefaultSkill = M_Skill.SpawnProjectileSkill(DefaultSkillCode);
            DefaultSkill.transform.position = transform.position;
            DefaultSkill.enabled = true;
            DefaultSkill.gameObject.SetActive(true);

            // 기본 스킬 데이터 설정
            DefaultSkill.InitializeSkill(m_Target, conditionData, statData);
        }
    }

    // 타워 정보
    [System.Serializable]
    public struct S_TowerData
    {
        // 회전 속도
        public float RotateSpeed;
        // 초기 회전 값
        public Vector3 InitialRotation;
        // 적 감지 여부
        public bool ShouldFindTarget;

        // 기본 스킬 데이터
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;
        // 기본 스킬 공격 속도
        public float DefaultSkillAttackSpeed;
        // 기본 스킬 타이머
        public float DefaultSkillTimer;

        // 스킬1 데이터
        public S_SkillConditionData_Excel Skill01Condition;
        public S_SkillStatData_Excel Skill01Stat;
        // 스킬1 공격 속도
        public float Skill01AttackSpeed;
        // 스킬1 타이머
        public float Skill01Timer;

        // 스킬2 데이터
        public S_SkillConditionData_Excel Skill02Condition;
        public S_SkillStatData_Excel Skill02Stat;
        // 스킬2 공격 속도
        public float Skill02AttackSpeed;
        // 스킬2 타이머
        public float Skill02Timer;

        #region 시너지 관련
        // 버프
        public List<S_BuffData_Excel> BuffList;

        // 공격 타입 변경
        public E_AttackType Synergy_Atk_type;
        public int BounceCount;

        // 버서커
        public bool Berserker;
        public int BerserkerStack;
        public int BerserkerMaxStack;
        public List<S_BuffData_Excel> BerserkerBuffList;

        #endregion

        // 마왕 스킬 버프
        public List<S_BuffData_Excel> DevilSkillBuffList;
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
