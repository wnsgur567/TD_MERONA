using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    // ȸ���� ����
    public float m_RotateAngle;
    // ȸ���� �ɸ��� �ð�
    public float m_Duration;
    // ȸ���� �߽���
    [ReadOnly]
    public Transform m_Center;

    // [Ÿ��(��, ��)][����(��, ��, ��, ��)]
    // ȸ����ų ��� ����Ʈ
    public List<Node>[][] m_NodeList;
    // ��� �θ� (��������)
    public Transform[][] m_NodeParentList;

    // ���� ���
    protected Node m_SelectedNode;
    // ȸ�� ���� ��� (ȸ���� ������ �� ������ ���)
    protected Node m_StandardNode_Rotation;
    // ȸ�� ����
    protected bool m_IsRotating;

    #region ���� ������Ƽ
    // ���� ����� Ÿ��
    protected E_NodeType? SelectedNodeType
    {
        get
        {
            if (null == m_SelectedNode)
                return null;

            for (E_NodeType i = 0; i < E_NodeType.Max; ++i)
            {
                for (E_Direction j = 0; j < E_Direction.Max; ++j)
                {
                    if (m_NodeList[(int)i][(int)j].Contains(m_SelectedNode))
                    {
                        return i;
                    }
                }
            }

            return null;
        }
    }
    // ���� ����� ����
    protected E_Direction? SelectedNodeDir
    {
        get
        {
            if (null == m_SelectedNode)
                return null;

            return (E_Direction)Enum.Parse(typeof(E_Direction), m_SelectedNode.transform.parent.name);
        }
    }
    // ȸ�� ���� ����� Ÿ��
    protected E_NodeType? StandardNodeType
    {
        get
        {
            if (null == m_StandardNode_Rotation)
                return null;

            for (E_NodeType i = 0; i < E_NodeType.Max; ++i)
            {
                for (E_Direction j = 0; j < E_Direction.Max; ++j)
                {
                    if (m_NodeList[(int)i][(int)j].Contains(m_StandardNode_Rotation))
                    {
                        return i;
                    }
                }
            }

            return null;
        }
    }
    // ���� ī�޶�
    protected Camera MainCamera => Camera.main;
    #endregion

    private void Awake()
    {
        // ȸ�� �߽��� ����
        m_Center = transform.Find("Center");
        if (null == m_Center)
        {
            Debug.LogError("ȸ���� �߽����� ���ų� �߸��� �̸��� �Է��߽��ϴ�.");
            Debug.LogError("ȸ���� �߽����� �̸��� \"Center\"���� �մϴ�.");
        }

        // �ʱ�ȭ
        m_NodeList = new List<Node>[(int)E_NodeType.Max][];
        m_NodeParentList = new Transform[(int)E_NodeType.Max][];

        for (E_NodeType i = 0; i < E_NodeType.Max; ++i)
        {
            m_NodeList[(int)i] = new List<Node>[(int)E_Direction.Max];
            m_NodeParentList[(int)i] = new Transform[(int)E_Direction.Max];

            for (E_Direction j = 0; j < E_Direction.Max; ++j)
            {
                m_NodeList[(int)i][(int)j] = new List<Node>();
                m_NodeParentList[(int)i][(int)j] = transform.Find(i.ToString()).Find(j.ToString());

                m_NodeParentList[(int)i][(int)j].GetComponentsInChildren<Node>(m_NodeList[(int)i][(int)j]);
            }
        }
    }

    private void Update()
    {
        // ���� ��� �˻�
        MouseProcess();

        // ȸ��
        RotateProcess();
    }

    // ���� ��� �˻�
    protected void MouseProcess()
    {
        // ��Ŭ�� ��
        if (Input.GetMouseButtonDown(0))
        {
            // Ŭ���� ������ ����ĳ��Ʈ
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            float maxDistance = MainCamera.farClipPlane;
            int layerMask = LayerMask.GetMask("Node");

            RaycastHit hit;

            // ����ĳ��Ʈ ���� ��
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // ���� ��� ����
                SelectNode(hit.transform.GetComponentInParent<Node>());
            }
            // ����ĳ��Ʈ ���� ��
            else
            {
                // ���� ��� ����
                SelectNode(null);
            }
        }
    }

    // �ƿ����� ����
    protected void UpdateOutline(bool active)
    {
        // ���� ó�� (null üũ)
        if (SelectedNodeType != null)
        {
            // �ƿ����� ������ ��� ����Ʈ
            List<Node>[] nodes = m_NodeList[(int)SelectedNodeType];

            // �ƿ����� on, off ����
            for (int i = 0; i < nodes.Length; ++i)
            {
                for (int j = 0; j < nodes[i].Count; ++j)
                {
                    nodes[i][j].m_Outline.SetActive(active);
                }
            }
        }
    }
    // ��� ����
    protected void SelectNode(Node node)
    {
        // ���� ��� �ƿ����� ����
        UpdateOutline(false);

        // ���� ��� ����
        m_SelectedNode = node;

        // ���� ��� �ƿ����� ����
        UpdateOutline(true);
    }

    // ȸ��
    protected void RotateProcess()
    {
        // ���� ��尡 �����ϸ�
        if (null != SelectedNodeType)
        {
            // ���콺 ȸ�� �˻�
            Rotate_Mouse();
            // Ű���� ȸ�� �˻�
            Rotate_Keyboard();
        }
    }

    // ���콺 ȸ��
    protected void Rotate_Mouse()
    {
        // ���� ó�� (�̹� ȸ�� ���� ���)
        if (m_IsRotating)
            return;

        // ��Ŭ����
        if (Input.GetMouseButtonUp(0))
        {
            // Ŭ���� ������ ����ĳ��Ʈ
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            float maxDistance = MainCamera.farClipPlane;
            int layerMask = LayerMask.GetMask("NodeRotate");

            RaycastHit hit;

            // ����ĳ��Ʈ ���� ��
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // ���� ���
                E_Direction from = (E_Direction)SelectedNodeDir;
                E_Direction to = (E_Direction)Enum.Parse(typeof(E_Direction), hit.transform.parent.name);

                // �ð� ���� ȸ��
                if ((int)to == (int)(from + 1) % (int)E_Direction.Max)
                {
                    StartCoroutine(RotateNode());
                }
                // �ݽð� ���� ȸ��
                else if ((int)to == (int)(from + (int)E_Direction.Max - 1) % (int)E_Direction.Max)
                {
                    StartCoroutine(RotateNode(false));
                }
            }
            // ����ĳ��Ʈ ���� ��
            else
            {
                // ���� ��� ����
                SelectNode(null);
            }
        }
    }
    // Ű���� ȸ��
    protected void Rotate_Keyboard()
    {
        // ���� ó�� (�̹� ȸ�� ���� ���)
        if (m_IsRotating)
            return;

        // QŰ�� ���� ���
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // �ݽð� ���� ȸ��
            StartCoroutine(RotateNode(false));
        }

        // ���� ó�� (�̹� ȸ�� ���� ���)
        if (m_IsRotating)
            return;

        // EŰ�� ���� ���
        if (Input.GetKeyDown(KeyCode.E))
        {
            // �ð� ���� ȸ��
            StartCoroutine(RotateNode());
        }
    }

    // �� ������ ȸ��
    protected IEnumerator RotateNode(bool clockwise = true)
    {
        // ȸ�� ���� ����
        m_IsRotating = true;
        // ȸ�� ���� ��� ����
        m_StandardNode_Rotation = m_SelectedNode;

        // ���⺰ ��� �θ�
        Transform[] node_parent_by_dir = m_NodeParentList[(int)StandardNodeType];

        // �� ����� �ð�
        float time = 0f;
        // ȸ���� ����
        int Dir = clockwise ? 1 : -1;

        // ���� ó�� (ȸ���� �ɸ��� �ð��� 0������ ���)
        if (m_Duration <= 0f)
        {
            // ȸ���� ������ŭ ��� ȸ��
            for (int i = 0; i < node_parent_by_dir.Length; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle);
                }
            }

            // ȸ�� ����
            yield break;
        }

        // ������ �ð����� ȸ��
        while (time < m_Duration)
        {
            // ���� �����ӱ��� �ɸ� �ð���ŭ ����Ͽ� ȸ��
            for (int i = 0; i < node_parent_by_dir.Length; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle * Time.deltaTime / m_Duration);
                }
            }

            // �� ��� �ð� ����
            time += Time.deltaTime;

            // ���� �����ӱ��� ���
            yield return null;
        }

        // ���� ���� ���� (���ϴ� �ð� ���� +�� ��ŭ �ݴ�������� ȸ��)
        for (int i = 0; i < node_parent_by_dir.Length; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle * (m_Duration - time) / m_Duration);
            }
        }

        #region �ν����� ������
#if UNITY_EDITOR
        for (int i = 0; i < node_parent_by_dir.Length; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                Vector3 pos = node_parent_by_dir[i].GetChild(j).position;

                pos.x = Mathf.Round(pos.x * 10000f) * 0.0001f;
                pos.y = Mathf.Round(pos.y * 10000f) * 0.0001f;
                pos.z = Mathf.Round(pos.z * 10000f) * 0.0001f;

                node_parent_by_dir[i].GetChild(j).position = pos;
            }
        }
#endif
        #endregion

        // ��� �θ� ������Ʈ
        UpdateParent(StandardNodeType, clockwise);
        // ȸ�� ���� ����
        m_IsRotating = false;
    }

    // �θ� ������Ʈ
    protected void UpdateParent(E_NodeType? type, bool clockwise = true)
    {
        // �ð� ����
        if (clockwise)
        {
            SwapParent(type, E_Direction.North, E_Direction.East);
            SwapParent(type, E_Direction.South, E_Direction.West);
            SwapParent(type, E_Direction.North, E_Direction.South);
        }
        // �ݽð� ����
        else
        {
            SwapParent(type, E_Direction.North, E_Direction.West);
            SwapParent(type, E_Direction.South, E_Direction.East);
            SwapParent(type, E_Direction.North, E_Direction.South);
        }
    }
    // �θ� ��ȯ
    protected void SwapParent(E_NodeType? type, E_Direction first, E_Direction second)
    {
        Transform First_T = m_NodeParentList[(int)type][(int)first];
        Transform Second_T = m_NodeParentList[(int)type][(int)second];
        List<Transform> TempList = new List<Transform>();

        int FirstCount = First_T.childCount;
        int SecondCount = Second_T.childCount;

        // 1 -> Temp
        for (int i = 0; i < FirstCount; ++i)
        {
            TempList.Add(First_T.GetChild(i));
        }
        // 2 -> 1
        for (int i = 0; i < SecondCount; ++i)
        {
            Second_T.GetChild(0).SetParent(First_T);
        }
        // Temp -> 2
        for (int i = 0; i < FirstCount; ++i)
        {
            TempList[i].SetParent(Second_T);
        }
    }

    public enum E_NodeType
    {
        Inner,
        Outer,

        Max
    }
}
