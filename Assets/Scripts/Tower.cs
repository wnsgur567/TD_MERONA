using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int m_TempCode;

    // 타워 정보 (엑셀)
    public S_TowerData_Excel m_TowerInfo_Excel;
    // 타워 정보
    public S_TowerData m_TowerInfo;

    // 타겟
    public GameObject m_Target;

    #region 내부 컴포넌트
    public AttackRange m_AttackRange;
    #endregion

    #region 내부 프로퍼티
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
    // 스킬 메모리풀
    protected SkillPool M_SkillPool => SkillPool.Instance;
    // 리소스 매니져
    protected ResourcesManager M_Resources => ResourcesManager.Instance;
    // 타워 매니져
    protected TowerManager M_Tower => TowerManager.Instance;
    // 스킬 컨디션 정보
    protected SkillConditionData Skill_Condition
    {
        get
        {
            return M_Resources.GetScriptableObject<SkillConditionData>("Skill", "SkillConditionData");
        }
    }
    // 스킬 스탯 정보
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

        #region 내부 컴포넌트
        m_AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_AttackRange.SetRange(m_TowerInfo.DefaultSkillStat.Range);
        #endregion
    }

    // 타워 초기화
    public void InitializeTower(int code)
    {
        #region 엑셀 데이터 정리
        m_TowerInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region 내부 데이터 정리
        // 평타
        m_TowerInfo.DefaultSkillCondition = Skill_Condition.GetData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.DefaultSkillStat = Skill_Stat.GetData(m_TowerInfo_Excel.Atk_Code);

        // 공격
        m_TowerInfo.AttackSpeed = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.AttackTimer = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.IsAttacking = true;

        // 시너지
        m_TowerInfo.Synergy_Atk_type = E_AttackType.None;
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
    // 타겟 변경 기준에 따른 타겟 업데이트
    public void UpdateTarget()
    {
        // 타겟 변경 기준에 따라
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
        if (m_TowerInfo.AttackTimer < m_TowerInfo.AttackSpeed)
        {
            m_TowerInfo.AttackTimer += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            // 내부 데이터 정리
            m_TowerInfo.AttackTimer -= m_TowerInfo.AttackSpeed;
            m_TowerInfo.AttackSpeed = m_TowerInfo_Excel.Atk_spd;
            m_TowerInfo.IsAttacking = true;

            // 스킬 생성
            Skill skill = M_SkillPool.GetPool("Skill").Spawn().GetComponent<Skill>();
            skill.transform.position = transform.position;
            skill.enabled = true;
            skill.gameObject.SetActive(true);

            // 스킬 데이터 불러오기
            S_SkillConditionData_Excel conditionData = m_TowerInfo.DefaultSkillCondition;
            S_SkillStatData_Excel statData = m_TowerInfo.DefaultSkillStat;

            #region 시너지

            #region 버프
            // 버프 적용 확률 계산
            float Buff1Rand = Random.Range(0f, 1f);
            float Buff2Rand = Random.Range(0f, 1f);
            float Buff3Rand = Random.Range(0f, 1f);
            // 버프 적용 여부 저장
            bool Buff1Apply = (m_TowerInfo.Buff1.BuffType == E_BuffType.None) ? false : Buff1Rand <= m_TowerInfo.Buff1.BuffRand;
            bool Buff2Apply = (m_TowerInfo.Buff2.BuffType == E_BuffType.None) ? false : Buff2Rand <= m_TowerInfo.Buff2.BuffRand;
            bool Buff3Apply = (m_TowerInfo.Buff3.BuffType == E_BuffType.None) ? false : Buff3Rand <= m_TowerInfo.Buff3.BuffRand;

            // 버프1 체크
            if (Buff1Apply)
            {
                // 버프1 합연산
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

                // 버프2 체크
                if (Buff2Apply)
                {
                    // 버프2 합연산
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

                    // 버프3 체크
                    if (Buff3Apply)
                    {
                        // 버프3 합연산
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

                // 버프1 곱연산
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

                // 버프2 체크
                if (Buff2Apply)
                {
                    // 버프2 곱연산
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

                    // 버프3 체크
                    if (Buff3Apply)
                    {
                        // 버프3 곱연산
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
            
            // 사거리 업데이트
            m_AttackRange.SetRange(statData.Range);
            #endregion

            // 공격 타입 변경
            if (m_TowerInfo.Synergy_Atk_type != E_AttackType.None)
            {
                conditionData.Atk_type = m_TowerInfo.Synergy_Atk_type;
            }

            #endregion

            // 스킬 데이터 설정
            skill.InitializeSkill(m_Target, conditionData, statData);
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

        // 타워 공격 속도
        public float AttackSpeed;
        // 타워 공격 타이머
        public float AttackTimer;
        // 공격 트리거
        public bool IsAttacking;

        // 일반 공격
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;

        #region 시너지 관련
        // 버프
        public S_Buff Buff1;
        public S_Buff Buff2;
        public S_Buff Buff3;
        // 공격 타입 변경
        public E_AttackType Synergy_Atk_type;
        #endregion
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
