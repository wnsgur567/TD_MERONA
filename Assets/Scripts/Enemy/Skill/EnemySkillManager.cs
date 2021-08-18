using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySkillManager : Singleton<EnemySkillManager>
{
    protected SkillCondition_TableExcelLoader m_SkillConditionData;
    protected SkillStat_TableExcelLoader m_SkillStatData;
    protected Prefab_TableExcelLoader m_PrefabData;

    #region ���� ������Ʈ
    #endregion

    #region ���� ������Ƽ
    // ������ ���̺�
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    // ��ų �޸�Ǯ
    protected EnemySkillPool M_SkillPool => EnemySkillPool.Instance;
    #endregion

    #region �ܺ� ������Ƽ
    #endregion

    #region ���� �Լ�
    #endregion

    #region �ܺ� �Լ�
    public SkillCondition_TableExcel GetConditionData(int code)
    {
        SkillCondition_TableExcel skillConditionData = m_SkillConditionData.DataList.Where(item => item.Code == code).SingleOrDefault();

        return skillConditionData;
    }
    public SkillStat_TableExcel GetStatData(int code)
    {
        SkillStat_TableExcel skillStatData = m_SkillStatData.DataList.Where(item => item.Code == code).SingleOrDefault();

        return skillStatData;
    }

    public EnemySkill SpawnProjectileSkill(int prefabCode, float m_damage, SkillCondition_TableExcel condition, SkillStat_TableExcel stat)
    {
        string key = m_PrefabData.GetPrefab(prefabCode)?.name;

        EnemySkill skill = M_SkillPool.GetPool(key)?.Spawn();
        skill.InitializeSkill(m_damage, condition, stat);

        return skill;
    }

    public void DespawnProjectileSkill(EnemySkill skill)
    {
        int projectPrefabCode = GetConditionData(skill.m_ConditionInfo.Code).projectile_prefab;
        string key = m_PrefabData.GetPrefab(projectPrefabCode).name;
        M_SkillPool.GetPool(key)?.DeSpawn(skill);
    }

    public int Condition_NoToCode(int no)
    {
        return m_SkillConditionData.DataList.Where(item => item.No == no).SingleOrDefault().Code;
    }
    public int Condition_CodeToNo(int code)
    {
        return m_SkillConditionData.DataList.Where(item => item.Code == code).SingleOrDefault().No;
    }
    public int Stat_NoToCode(int no)
    {
        return m_SkillStatData.DataList.Where(item => item.No == no).SingleOrDefault().Code;
    }
    public int Stat_CodeToNo(int code)
    {
        return m_SkillStatData.DataList.Where(item => item.Code == code).SingleOrDefault().No;
    }
    #endregion

    #region ����Ƽ �ݹ� �Լ�
    void Awake()
    {
        m_SkillConditionData = M_DataTable.GetDataTable<SkillCondition_TableExcelLoader>();
        m_SkillStatData = M_DataTable.GetDataTable<SkillStat_TableExcelLoader>();
        m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
    }
    #endregion
}