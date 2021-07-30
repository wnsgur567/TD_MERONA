using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [ReadOnly]
    public GameObject m_Outline;

    #region ���� ������Ƽ
    protected NodeManager M_Node
    {
        get
        {
            return NodeManager.Instance;
        }
    }
    #endregion

    private void Awake()
    {
        m_Outline = transform.Find("Outline").gameObject;
        m_Outline.SetActive(false);
    }
}
