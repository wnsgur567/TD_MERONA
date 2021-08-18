using System;
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
        m_Tower.gameObject.SetActive(true);
        m_Tower.transform.localPosition = new Vector3(0f, 0.501f, 0f);
        m_Tower.transform.localEulerAngles = Vector3.zero;
        m_Tower.m_TowerInfo.Direction = (E_Direction)Enum.Parse(typeof(E_Direction), transform.parent.name);
        m_Tower.m_TowerInfo.InitialRotation = (m_Tower.transform.forward * 10f);
        Debug.Log("타워 앞: " + m_Tower.transform.forward);
        Debug.Log("타워 위치: " + m_Tower.transform.position);
        Debug.Log("초기 회전 값: " + m_Tower.m_TowerInfo.InitialRotation);

        m_Tower.gameObject.SetActive(true);
    }

    private void Awake()
    {
        m_Outline = transform.Find("Outline").gameObject;
        //m_Outline.SetActive(false);
    }
}