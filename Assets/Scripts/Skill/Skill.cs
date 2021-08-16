using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    // 스킬 정보 (엑셀)
    public SkillCondition_TableExcel m_ConditionInfo_Excel;
    public SkillStat_TableExcel m_StatInfo_Excel;
    // 스킬 정보
    public S_SkillData m_SkillInfo;

    // 타겟
    public GameObject m_Target;

    #region 내부 프로퍼티
    // 스킬 매니져
    protected SkillManager M_Skill => SkillManager.Instance;

    // 스킬 이동 속도
    protected float MoveSpeed
    {
        get
        {
            return m_StatInfo_Excel.Speed * Time.deltaTime;
        }
    }
    // 타겟까지의 방향
    protected Vector3 TargetDir
    {
        get
        {
            return (m_Target.transform.position - transform.position);
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
    // 타겟 잃어버림
    protected bool LostTarget
    {
        get
        {
            return null == m_Target;
        }
    }
    // 타겟에게 도착 여부
    protected bool ArrivedToTarget
    {
        get
        {
            return DistanceToTarget <= 0.001f;
        }
    }
    // 생존 시간 소진
    protected bool DepletedLifeTime
    {
        get
        {
            return m_SkillInfo.LifeTime <= 0f;
        }
    }
    // 튕김 카운트 소진
    protected bool DepletedBounceCount
    {
        get
        {
            return m_SkillInfo.BounceCount <= 0;
        }
    }
    #endregion

    private void Awake()
    {
        m_SkillInfo.AttackRange = transform.Find("AttackRange").GetComponent<AttackRange>();
        m_SkillInfo.CanOverlapBounce = true;
        m_SkillInfo.BounceTargetList = new List<GameObject>();
    }

    private void Update()
    {
        if (CheckToDespawn())
        {
            Despawn();
            return;
        }

        UpdateInfo();

        RotateBullet();
        MoveBullet();

        if (CheckToUpdateTarget())
        {
            UpdateTarget();
        }
    }

    private bool CheckToDespawn()
    {
        switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
        {
            case E_AttackType.NormalFire:
                return ArrivedToTarget || LostTarget;
            case E_AttackType.FixedFire:
            case E_AttackType.PenetrateFire:
                return DepletedLifeTime;
            case E_AttackType.BounceFire:
                return DepletedBounceCount || LostTarget;
        }

        return ArrivedToTarget || LostTarget;
    }
    private void Despawn()
    {
        Skill skill = M_Skill.SpawnProjectileSkill(m_StatInfo_Excel.LoadCode);
        SkillCondition_TableExcel condition = M_Skill.GetConditionData(m_StatInfo_Excel.LoadCode);
        SkillStat_TableExcel stat = M_Skill.GetStatData(condition.PassiveCode);
        skill?.InitializeSkill(m_Target, condition, stat);

        m_Target = null;

        m_SkillInfo.AttackRange.Clear();
        m_SkillInfo.BounceTargetList.Clear();

        M_Skill.DespawnProjectileSkill(this);
    }

    private bool CheckToUpdateTarget()
    {
        switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
        {
            case E_AttackType.BounceFire:
                return ArrivedToTarget;
        }

        return false;
    }
    private void UpdateTarget()
    {
        switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
        {
            case E_AttackType.BounceFire:
                m_SkillInfo.InitPos = m_Target.transform.position;

                if (m_SkillInfo.CanOverlapBounce)
                {
                    switch ((E_TargetType)m_ConditionInfo_Excel.Target_type)
                    {
                        case E_TargetType.CloseTarget:
                            m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
                            break;
                        case E_TargetType.RandTarget:
                            m_Target = m_SkillInfo.AttackRange.GetRandomTarget();
                            break;
                        case E_TargetType.FixTarget:
                            m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
                            break;
                        case E_TargetType.TileTarget:
                            m_Target = m_SkillInfo.AttackRange.GetNearTarget(true);
                            break;
                    }
                }
                else
                {
                    do
                    {
                        m_SkillInfo.AttackRange.RemoveTarget(m_Target);

                        switch ((E_TargetType)m_ConditionInfo_Excel.Target_type)
                        {
                            case E_TargetType.CloseTarget:
                                m_Target = m_SkillInfo.AttackRange.GetNearTarget();
                                break;
                            case E_TargetType.RandTarget:
                                m_Target = m_SkillInfo.AttackRange.GetRandomTarget();
                                break;
                            case E_TargetType.FixTarget:
                                m_Target = m_SkillInfo.AttackRange.GetNearTarget();
                                break;
                            case E_TargetType.TileTarget:
                                m_Target = m_SkillInfo.AttackRange.GetNearTarget();
                                break;
                        }

                    } while (m_SkillInfo.BounceTargetList.Contains(m_Target));

                    if (null != m_Target)
                    {
                        m_SkillInfo.BounceTargetList.Add(m_Target);
                    }
                }

                --m_SkillInfo.BounceCount;
                break;
        }
    }

    private void RotateBullet()
    {
        switch ((E_MoveType)m_ConditionInfo_Excel.Move_type)
        {
            case E_MoveType.Straight:
                transform.LookAt(transform.position + TargetDir);
                break;
            case E_MoveType.Curve:
                transform.LookAt(transform.position + GetCurveDir());
                break;
        }
    }

    private void MoveBullet()
    {
        switch ((E_MoveType)m_ConditionInfo_Excel.Move_type)
        {
            case E_MoveType.Straight:
                StraightMove();
                break;
            case E_MoveType.Curve:
                CurveMove();
                break;
        }
    }
    private void StraightMove()
    {
        transform.position += TargetDir.normalized * MoveSpeed;
    }
    private Vector3 GetCurveDir()
    {
        // 포물선 이동 제작
        // 출처: https://robatokim.tistory.com/entry/%EA%B2%8C%EC%9E%84%EC%88%98%ED%95%99-%EC%97%AD%ED%83%84%EB%8F%84%EA%B3%84%EC%82%B0%EC%9D%84-%EC%9D%B4%EC%9A%A9%ED%95%9C-%EB%91%90%EC%A0%90-%EC%82%AC%EC%9D%98-%ED%8F%AC%EB%AC%BC%EC%84%A0-%EA%B5%AC%ED%95%98%EA%B8%B0
        Vector3 StartPos = m_SkillInfo.InitPos;
        Vector3 EndPos = m_Target.transform.position;
        Vector3 Pos = transform.position;

        // 최대 높이
        float MaxHeight = Mathf.Max(StartPos.y, EndPos.y) + 2.5f;
        // 최대 높이까지 가는 시간
        float MaxTime = (MaxHeight - StartPos.y) / m_StatInfo_Excel.Speed;

        float EndHeight = EndPos.y - StartPos.y;
        float Height = MaxHeight - StartPos.y;

        float g = 2 * Height / Mathf.Pow(MaxTime, 2f);

        float V_Y = Mathf.Sqrt(2 * g * Height);

        float a = g;
        float b = -2 * V_Y;
        float c = 2 * EndHeight;

        float EndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        float V_X = -(StartPos.x - EndPos.x) / EndTime;
        float V_Z = -(StartPos.z - EndPos.z) / EndTime;

        Vector3 InitPos = StartPos;
        InitPos.y = 0f;
        EndPos.y = 0f;
        float Max = Vector3.Distance(InitPos, EndPos);

        Pos.y = 0f;
        float time = (1f + Time.deltaTime - Vector3.Distance(Pos, EndPos) / Max) * EndTime;

        Pos.x = StartPos.x + V_X * time;
        Pos.y = StartPos.y + (V_Y * time) - (g * time * time * 0.5f);
        Pos.z = StartPos.z + V_Z * time;

        Vector3 dir = Pos - transform.position;

        return dir;
    }
    private void CurveMove()
    {
        transform.position += GetCurveDir().normalized * MoveSpeed;
    }

    private void UpdateInfo()
    {
        switch ((E_AttackType)m_ConditionInfo_Excel.Atk_type)
        {
            case E_AttackType.NormalFire:
            case E_AttackType.BounceFire:
                break;
            case E_AttackType.FixedFire:
            case E_AttackType.PenetrateFire:
                m_SkillInfo.LifeTime -= Time.deltaTime;
                break;
        }
    }

    public void InitializeSkill(GameObject target, SkillCondition_TableExcel conditionData, SkillStat_TableExcel statData)
    {
        m_Target = target;

        m_ConditionInfo_Excel = conditionData;
        m_StatInfo_Excel = statData;

        if ((E_AttackType)m_ConditionInfo_Excel.Atk_type == E_AttackType.BounceFire)
        {
            // 다음 타겟 찾는 사거리 = 타워 사거리의 1 / 4
            m_SkillInfo.AttackRange.SetRange(m_StatInfo_Excel.Range * 0.25f);
        }

        m_SkillInfo.BounceCount = m_StatInfo_Excel.Target_num;
        m_SkillInfo.LifeTime = m_StatInfo_Excel.Life_Time;
        m_SkillInfo.InitPos = transform.position;

        if (!m_SkillInfo.CanOverlapBounce &&
            null != m_Target)
        {
            m_SkillInfo.BounceTargetList.Add(m_Target);
        }
    }

    [System.Serializable]
    public struct S_SkillData
    {
        public bool CanOverlapBounce;
        public int BounceCount;
        public List<GameObject> BounceTargetList;
        public float LifeTime;
        public AttackRange AttackRange;
        public Vector3 InitPos;
    }
}
