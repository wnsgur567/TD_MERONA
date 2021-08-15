using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    // 마왕 코드(임시)
    public int m_TempCode;
    // 타겟
    public GameObject m_Target;

    // 마왕 정보(엑셀)
    protected Tower_TableExcel m_DevilInfo_Excel;
    // 마왕 정보
    protected S_DevilData m_DevilInfo;

    protected delegate void DevilSkillHandler(DevilSkillArg arg);
    protected event DevilSkillHandler Skill01Event;
    protected event DevilSkillHandler Skill02Event;

    #region 내부 컴포넌트
    protected AttackRange m_AttackRange;
    #endregion

    #region 내부 프로퍼티
    // 타워 매니져
    protected TowerManager M_Tower => TowerManager.Instance;
    // 버프 매니져
    protected BuffManager M_Buff => BuffManager.Instance;
    // 스킬 매니져
    protected SkillManager M_Skill => SkillManager.Instance;

    // 타워 회전 속도
    protected float RotateSpeed
    {
        get
        {
            return m_DevilInfo.RotateSpeed * Time.deltaTime;
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
    public float MaxHP => m_DevilInfo_Excel.HP;
    public float HP => m_DevilInfo.m_HP;
    #endregion

    #region 유니티 콜백 함수
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

    #region 내부 함수
    // 타워 초기화
    protected virtual void InitializeTower(int code)
    {
        #region 엑셀 데이터 정리
        m_DevilInfo_Excel = M_Tower.GetData(code);
        #endregion

        #region 내부 데이터 정리
        m_DevilInfo.RotateSpeed = 5f;
        m_DevilInfo.InitialRotation = transform.eulerAngles;
        m_DevilInfo.ShouldFindTarget = true;

        // 기본 스킬 데이터
        m_DevilInfo.Condition_Default = M_Skill.GetConditionData(m_DevilInfo_Excel.Atk_Code);
        m_DevilInfo.Stat_Default = M_Skill.GetStatData(m_DevilInfo_Excel.Atk_Code);
        // 기본 스킬
        m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
        m_DevilInfo.AttackTimer_Default = m_DevilInfo.Stat_Default.CoolTime;
        #endregion

        #region 내부 컴포넌트
        m_AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_AttackRange.Initialize();
        m_AttackRange.SetRange(m_DevilInfo.Stat_Default.Range);
        #endregion
    }
    // 타워 회전
    protected void RotateToTarget()
    {
        // 회전할 방향
        Vector3 dir;

        // 타겟이 없으면
        if (null == m_Target)
        {
            // 초기 방향으로 방향 설정
            dir = m_DevilInfo.InitialRotation;
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
    protected void UpdateTarget()
    {
        // 타겟 변경 기준에 따라
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
            // FixTarget (타겟이 사거리를 벗어나거나 죽은 경우 변경)
            case E_TargetType.FixTarget:
                if (null == m_Target || // 예외처리
                    DistanceToTarget > m_DevilInfo.Stat_Default.Range) // 타겟이 사거리를 벗어난 경우
                {
                    m_Target = m_AttackRange.GetNearTarget();
                }
                break;
        }
    }
    // 타워 공격
    protected void AttackTarget()
    {
        if (m_DevilInfo.AttackTimer_Default < m_DevilInfo.AttackSpeed_Default)
        {
            m_DevilInfo.AttackTimer_Default += Time.deltaTime;
        }
        else if (null != m_Target)
        {
            // 내부 데이터 정리
            m_DevilInfo.AttackTimer_Default -= m_DevilInfo.AttackSpeed_Default;
            m_DevilInfo.AttackSpeed_Default = m_DevilInfo.Stat_Default.CoolTime;
            m_DevilInfo.ShouldFindTarget = true;

            // 스킬 데이터 불러오기
            SkillCondition_TableExcel conditionData = m_DevilInfo.Condition_Default;
            SkillStat_TableExcel statData = m_DevilInfo.Stat_Default;

            // 기본 스킬 투사체 생성
            int SkillCode = conditionData.projectile_prefab;
            Skill Skill = M_Skill.SpawnProjectileSkill(SkillCode);
            Skill.transform.position = transform.position;
            Skill.enabled = true;
            Skill.gameObject.SetActive(true);

            // 스킬 데이터 설정
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

    #region 외부 함수
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
        // 회전 속도
        public float RotateSpeed;
        // 초기 회전 값
        public Vector3 InitialRotation;
        // 적 감지 여부
        public bool ShouldFindTarget;
        
        // 기본 스킬 데이터
        public SkillCondition_TableExcel Condition_Default;
        public SkillStat_TableExcel Stat_Default;
        // 기본 스킬 공격 속도
        public float AttackSpeed_Default;
        // 기본 스킬 타이머
        public float AttackTimer_Default;

        // 스킬
        public S_DevilSkillData m_Skill01;
        public S_DevilSkillData m_Skill02;

        public float m_HP;
        public float m_DefaultSkill_LifeSteal;
    }
    // 마왕 스킬 정보
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
