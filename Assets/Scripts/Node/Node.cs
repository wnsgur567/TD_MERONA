using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected GameObject m_Outline;
    public Tower m_Tower;

    #region 내부 프로퍼티
    protected NodeManager M_Node => NodeManager.Instance;
    #endregion

    #region 외부 프로퍼티
    public GameObject Outline => m_Outline;
    #endregion

    public void SetTower(Tower tower)
    {
        m_Tower = tower;
        m_Tower.transform.SetParent(transform);
        m_Tower.transform.localPosition = new Vector3(0f, 0.501f, 0f);
    }

    private void Awake()
    {
        m_Outline = transform.Find("Outline").gameObject;
        //m_Outline.SetActive(false);
    }
}