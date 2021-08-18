using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySkill : MonoBehaviour
{
    // ��ų ���� (����)
    public SkillCondition_TableExcel m_ConditionInfo;
    public SkillStat_TableExcel m_StatInfo;

    #region ���� ������Ƽ

    // ��ų �Ŵ���
    protected EnemySkillManager enemyskillmanager => EnemySkillManager.Instance;

    // ���� �Ŵ���
    protected BuffManager M_Buff => BuffManager.Instance;

    // Ÿ�� ��ġ
    protected Vector3 TargetPos => new Vector3(0f, 0f, 0f);

    // ��ų �̵� �ӵ�
    protected float MoveSpeed => m_StatInfo.Speed * Time.deltaTime;

    // Ÿ�ٱ����� ����
    protected Vector3 TargetDir => TargetPos - transform.position;

    // Ÿ�ٿ��� ���� ����
    protected bool ArrivedToTarget;

    private float m_damage;

    #endregion

    private void OnTriggerEnter(Collider other)
    {
        //���� �߰�
        ArrivedToTarget = true;
    }

    private void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("EnemyProjectile");
    }

    private void Update()
    {
        if (CheckToDespawn())
        {
            Despawn();
            return;
        }

        RotateSkill();
        MoveSkill();
    }

    #region ���� �Լ�

    protected bool CheckToDespawn()
    {
        switch ((E_AttackType)m_ConditionInfo.Atk_type)
        {
            case E_AttackType.NormalFire:
                return ArrivedToTarget;
        }

        return  ArrivedToTarget;
    }

    protected void Despawn()
    {
        enemyskillmanager.DespawnProjectileSkill(this);
    }

    protected void RotateSkill()
    {
        switch ((E_MoveType)m_ConditionInfo.Move_type)
        {
            case E_MoveType.Straight:
                transform.LookAt(transform.position + TargetDir);
                break;
        }
    }

    protected void MoveSkill()
    {
        switch ((E_MoveType)m_ConditionInfo.Move_type)
        {
            case E_MoveType.Straight:
                StraightMove();
                break;
        }
    }

    protected void StraightMove()
    {
        transform.position += TargetDir.normalized * MoveSpeed;
    }
    
    #endregion

    #region �ܺ� �Լ�

    //Enemy ��ũ��Ʈ�� �ִ� ������ ��������
    public void InitializeSkill(float damage, SkillCondition_TableExcel conditionData, SkillStat_TableExcel statData)
    {
        m_ConditionInfo = conditionData;
        m_StatInfo = statData;

        m_damage = damage;
    }

    #endregion
}
