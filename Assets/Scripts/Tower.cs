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
        m_TowerInfo_Excel = M_Tower.m_TowerData.GetData(code);
        #endregion

        #region 내부 데이터 정리
        // 평타
        m_TowerInfo.DefaultSkillCondition = Skill_Condition.GetData(m_TowerInfo_Excel.Atk_Code);
        m_TowerInfo.DefaultSkillStat = Skill_Stat.GetData(m_TowerInfo_Excel.Atk_Code);

        // 공격
        m_TowerInfo.AttackTimer = m_TowerInfo_Excel.Atk_spd;
        m_TowerInfo.IsAttacking = true;
        // 시너지
        // 공격력
        m_TowerInfo.SynergyStat.Atk_Fix = 0f;
        m_TowerInfo.SynergyStat.Atk_Percent = 1f;
        // 사거리
        m_TowerInfo.SynergyStat.Range_Fix = 0f;
        m_TowerInfo.SynergyStat.Range_Percent = 1f;
        // 공격 속도
        m_TowerInfo.SynergyStat.Atk_spd_Fix = 0f;
        m_TowerInfo.SynergyStat.Atk_spd_Percent = 1f;
        // 치명타 확률
        m_TowerInfo.SynergyStat.Crit_rate_Fix = 0f;
        m_TowerInfo.SynergyStat.Crit_rate_Percent = 1f;
        // 치명타 배율
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

            // 시너지, 버프 관련
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

    // 타워 정보
    [System.Serializable]
    public struct S_TowerData
    {
        // 회전 속도
        public float RotateSpeed;
        // 초기 회전 값
        public Vector3 InitialRotation;

        // 타워 공격 타이머
        public float AttackTimer;
        // 공격 트리거
        public bool IsAttacking;

        // 일반 공격
        public S_SkillConditionData_Excel DefaultSkillCondition;
        public S_SkillStatData_Excel DefaultSkillStat;

        #region 시너지 관련
        public S_SynergyStat SynergyStat;
        public E_AttackType Synergy_Atk_type;
        #endregion
    }
    [System.Serializable]
    public struct S_SynergyStat
    {
        // 공격력
        public float Atk_Fix;
        public float Atk_Percent;
        // 사거리
        public float Range_Fix;
        public float Range_Percent;
        // 공격 속도
        public float Atk_spd_Fix;
        public float Atk_spd_Percent;
        // 치명타 확률
        public float Crit_rate_Fix;
        public float Crit_rate_Percent;
        // 치명타 배율
        public float Crit_Dmg_Fix;
        public float Crit_Dmg_Percent;
    }
}
