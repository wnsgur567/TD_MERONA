using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected GameObject m_Outline;
    public Tower m_Tower;

    #region ���� ������Ƽ
    protected NodeManager M_Node => NodeManager.Instance;
    protected TowerManager M_Tower => TowerManager.Instance;
    protected SynergyManager M_Synergy => SynergyManager.Instance;
    #endregion

    #region �ܺ� ������Ƽ
    public GameObject Outline => m_Outline;
    #endregion

    public void SetTower(Tower tower)
    {
        m_Tower = tower;
        m_Tower.transform.SetParent(transform);
        m_Tower.gameObject.SetActive(true);
        m_Tower.transform.localPosition = new Vector3(0f, 0.501f, 0f);
        m_Tower.transform.localEulerAngles = Vector3.zero;
        m_Tower.m_TowerInfo.node = this;
        m_Tower.m_TowerInfo.Direction = (E_Direction)Enum.Parse(typeof(E_Direction), transform.parent.name);
        m_Tower.m_TowerInfo.InitialRotation = Vector3.forward * 90f * (int)m_Tower.Direction;

        m_Tower.gameObject.SetActive(true);

        M_Tower.AddTower(tower, m_Tower.m_TowerInfo.Direction);
        M_Synergy.UpdateSynergy();
    }

    private void Awake()
    {
        m_Outline = transform.Find("Outline").gameObject;
        //m_Outline.SetActive(false);
    }
}