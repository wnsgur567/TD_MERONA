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
    Enemy,
    Prefab,
    Shop,
    SkillCondition,
    SkillStat,
    Stage,
    StageEnemy,
    Synergy,
    Tower,

    Max
}

[DefaultExecutionOrder(-99)]
public class DataTableManager : Singleton<DataTableManager>
{
    // 인스펙터 추가용 리스트
    [SerializeField]
    protected List<ScriptableObject> m_DataTableList;
    // 타입별 딕셔너리


    #region 외부 함수
    public ScriptableObject GetDataTable(E_DataTableType type)
    {
        return m_DataTableList[(int)type]; //m_DataTables[type];
    }
    public T GetDataTable<T>() where T : ScriptableObject
    {
        string typeName = typeof(T).ToString().Split('_')[0];
        E_DataTableType type;
        if (Enum.TryParse<E_DataTableType>(typeName, out type))
            return m_DataTableList[(int)type] as T;
            //return m_DataTables[type] as T;

        return null;
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        // 딕셔너리 초기화
        //m_DataTables = new Dictionary<E_DataTableType, ScriptableObject>();

        // 테이블 타입별 초기화
        for (E_DataTableType i = E_DataTableType.None + 1; i < E_DataTableType.Max; ++i)
        {
            // 타입과 이름이 일치하는 스크립터블 오브젝트가 있으면 딕셔너리에 추가
            //m_DataTables.Add(i,
            //    m_DataTableList.
            //    Where(item => i.ToString() + "_TableLoader" == item.name).
            //    SingleOrDefault());

        }
    }
    #endregion
}
