using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField, ReadOnly]
    protected GameObject m_Outline;
    public GameObject Outline => m_Outline;

    #region 내부 프로퍼티
    protected NodeManager M_Node => NodeManager.Instance;
    #endregion

    private void Awake()
    {
        m_Outline = transform.Find("Outline").gameObject;
        m_Outline.SetActive(false);
    }
}
