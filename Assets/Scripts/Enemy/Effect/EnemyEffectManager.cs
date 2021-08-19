using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffectManager : Singleton<EnemyEffectManager>
{
    protected Prefab_TableExcelLoader m_PrefabData;

    #region ���� ������Ʈ
    #endregion

    #region ���� ������Ƽ
    // ������ ���̺�
    protected DataTableManager M_DataTable => DataTableManager.Instance;
    protected EnemyEffectPool M_EffectPool => EnemyEffectPool.Instance;
    #endregion

    #region �ܺ� ������Ƽ
    #endregion

    #region ���� �Լ�
    #endregion

    #region �ܺ� �Լ�
    public EnemyEffect SpawnEffect(int prefabCode)
    {
        string key = m_PrefabData.GetPrefab(prefabCode)?.name;

        EnemyEffect effect = M_EffectPool.GetPool(key)?.Spawn();
        effect?.InitializeEffect();
        return effect;
    }
    public void DespawnEffect(EnemyEffect effect)
    {
        effect.FinalizeEffect();
        string key = m_PrefabData.GetPrefab(effect.m_PrefabCode).name;
        M_EffectPool.GetPool(key)?.DeSpawn(effect);
    }
    #endregion

    #region ����Ƽ �ݹ� �Լ�
    private void Awake()
    {
        m_PrefabData = M_DataTable.GetDataTable<Prefab_TableExcelLoader>();
    }
    #endregion
}
