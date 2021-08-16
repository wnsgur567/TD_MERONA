using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTest : MonoBehaviour
{
    public float m_NodeY = -0.5f;
    [ReadOnly]
    public List<GameObject> m_TowerList;
    [ReadOnly]
    public List<GameObject> m_NodeList;
    [ReadOnly]
    public List<Tower_TableExcel> m_OriginList;
    protected Tower_TableExcelLoader M_Tower;
    protected Prefab_TableExcelLoader M_Prefab;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    [ContextMenu("Tower")]
    public void Tower()
    {
        for (int i = 0; i < m_TowerList.Count; ++i)
        {
            GameObject.DestroyImmediate(m_TowerList[i]);
        }
        m_TowerList.Clear();

        m_OriginList = new List<Tower_TableExcel>(M_Tower.DataList);

        for (int i = 3; i < m_OriginList.Count; ++i)
        {
            GameObject tower = GameObject.Instantiate(M_Prefab.GetPrefab(m_OriginList[i].Prefab));
            m_TowerList.Add(tower);
            tower.transform.GetChild(0).localScale = Vector3.one * M_Prefab.DataList[i].Size;
            tower.transform.SetParent(transform);
            tower.transform.localPosition = new Vector3((i - 3) * -5f, 0f, 0f);
        }

        Node();
    }
    public void Node()
    {
        for (int i = 0; i < m_NodeList.Count; ++i)
        {
            GameObject.DestroyImmediate(m_NodeList[i]);
        }
        m_NodeList.Clear();

        GameObject node_origin = Resources.Load<GameObject>("Node");

        for (int i = 0; i < m_TowerList.Count; ++i)
        {
            GameObject node = GameObject.Instantiate(node_origin);
            m_NodeList.Add(node);
            node.transform.SetParent(m_TowerList[i].transform);
            node.transform.localPosition = new Vector3(0f, m_NodeY, 0f);
        }
    }
    //[ContextMenu("Delete Node")]
    //public void DeleteNode()
    //{
    //    for (int i = 0; i < m_OriginList.Count; ++i)
    //    {
    //        Transform[] nodes = m_OriginList[i].transform.GetChilderen("Node(Clone)");

    //        for (int j = 0; j < nodes.Length; ++j)
    //        {
    //            GameObject.DestroyImmediate(nodes[j].gameObject);
    //        }
    //    }
    //}
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        Tower();
    }

    void Update()
    {

    }
    #endregion
}
