using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum E_DataTableType
{
    None = -1,

    BuffCC,
    Level,
    Monster,
    Prefab,
    Shop,
    SkillCondition,
    SkillStat,
    Stage,
    StageMonster,
    Synergy,
    Tower,

    Max
}

public class DataTableManager : Singleton<DataTableManager>
{
    [SerializeField]
    protected List<ScriptableObject> m_DataTableList;
    protected Dictionary<E_DataTableType, ScriptableObject> m_DataTables;
#if UNITY_EDITOR
    [SerializeField, ReadOnly]
    protected DebugDictionary<E_DataTableType, ScriptableObject> m_Debug;
#endif

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public ScriptableObject GetDataTable(E_DataTableType type)
    {
        return m_DataTables[type];
    }
    public T GetDataTable<T>() where T : ScriptableObject
    {
        string typeName = typeof(T).ToString().Split('_')[0];
        E_DataTableType type;
        if (Enum.TryParse<E_DataTableType>(typeName, out type))
            return m_DataTables[type] as T;

        return null;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        m_DataTables = new Dictionary<E_DataTableType, ScriptableObject>();
#if UNITY_EDITOR
        m_Debug = new DebugDictionary<E_DataTableType, ScriptableObject>();
#endif

        for (E_DataTableType i = E_DataTableType.None + 1; i < E_DataTableType.Max; ++i)
        {
            m_DataTables.Add(i,
                m_DataTableList.
                Where(item => i.ToString() + "_TableLoader" == item.name).
                SingleOrDefault());

#if UNITY_EDITOR
            m_Debug.Add(i, m_DataTables[i]);
#endif
        }
    }
    #endregion
}
