using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    public delegate void NodeEventHandler();
    // ��� ȸ�� ����� ȣ��Ǵ� �̺�Ʈ
    public event NodeEventHandler m_RotateEndEvent;

    // ȸ���� ����
    [SerializeField]
    protected float m_RotateAngle;
    // ȸ���� �ɸ��� �ð�
    [SerializeField]
    protected float m_Duration;
    // ȸ���� �߽���
    [SerializeField, ReadOnly]
    protected Transform m_Center;

    // [Ÿ��(��, ��)][����(��, ��, ��, ��)]
    // ȸ����ų ��� ����Ʈ
    protected Dictionary<E_NodeType, Dictionary<E_Direction, List<Node>>> m_NodeList;
    // ��� �θ� (��������)
    protected Dictionary<E_NodeType, Dictionary<E_Direction, Transform>> m_NodeParentList;

    // ���� ���
    protected Node m_SelectedNode;
    // ȸ�� ���� ��� (ȸ���� ������ �� ������ ���)
    protected Node m_RotationStandardNode;
    // ȸ�� ����
    [SerializeField]
    protected bool m_IsRotating;
    // Ÿ�� �ٶ� ���� ȸ���� ������Ʈ
    protected GameObject m_LookingDir;
    // ���� ī�޶�
    protected Camera m_Camera;

    #region ���� ������Ƽ
    // ���� ����� Ÿ��
    protected E_NodeType SelectedNodeType => m_SelectedNode.m_NodeType;
    protected E_Direction SelectedNodeDir => m_SelectedNode.m_Direction;
    protected E_NodeType StandardNodeType => m_RotationStandardNode.m_NodeType;
    // ���� ī�޶�
    protected Camera MainCamera
    {
        get
        {
            if (null == m_Camera)
            {
                m_Camera = Camera.main;
            }

            return m_Camera;
        }
    }
    #endregion

    private void Awake()
    {
        Initialize();
    }

    protected void Initialize()
    {
        // ȸ�� �߽��� ����
        m_Center = transform.Find("Center");
        if (null == m_Center)
        {
            GameObject center = new GameObject("Center");
            center.transform.SetParent(transform);
            m_Center = center.transform;
        }

        // �ʱ�ȭ
        m_NodeList = new Dictionary<E_NodeType, Dictionary<E_Direction, List<Node>>>();//new List<Node>[(int)E_NodeType.Max][];
        m_NodeParentList = new Dictionary<E_NodeType, Dictionary<E_Direction, Transform>>();//new Transform[(int)E_NodeType.Max][];

        // Ÿ�Ժ� (��, ��)
        for (E_NodeType i = 0; i < E_NodeType.Max; ++i)
        {
            m_NodeList[i] = new Dictionary<E_Direction, List<Node>>();
            m_NodeParentList[i] = new Dictionary<E_Direction, Transform>();

            // ���⺰
            for (E_Direction j = 0; j < E_Direction.Max; ++j)
            {
                m_NodeList[i][j] = new List<Node>();
                m_NodeParentList[i][j] = transform.Find(i.ToString()).Find(j.ToString());

                m_NodeParentList[i][j].GetComponentsInChildren<Node>(m_NodeList[i][j]);

                foreach (var item in m_NodeList[i][j])
                {
                    item.m_NodeType = i;
                    item.m_Direction = j;
                }
            }
        }

        m_LookingDir = new GameObject();
        m_LookingDir.transform.SetParent(transform);
        m_LookingDir.SetActive(false);
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
        // ���콺 �����Ͱ� UI���� ���� ��
        if (false == UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject())
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
                    SelectNode(hit.transform.GetComponent<Node>());
                }
                // ����ĳ��Ʈ ���� ��
                else
                {
                    // ���� ��� ����
                    SelectNode(null);
                }
            }
        }
    }
    #region MouseProcess
    // �ƿ����� ������Ʈ
    protected void UpdateOutline(bool active)
    {
        // ���� ó�� (������ ��尡 ���� ��)
        if (null == m_SelectedNode)
            return;

        // �ƿ����� ������ ��� ����Ʈ
        Dictionary<E_Direction, List<Node>> nodes = m_NodeList[SelectedNodeType];

        // �ƿ����� on, off ����
        for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
        {
            for (int j = 0; j < nodes[i].Count; ++j)
            {
                nodes[i][j].Outline.SetActive(active);
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
    #endregion

    // ȸ��
    protected void RotateProcess()
    {
        // ���콺 ȸ�� �˻�
        Rotate_Mouse();
        // Ű���� ȸ�� �˻�
        Rotate_Keyboard();
    }
    #region RotateProcess
    // ���콺 ȸ��
    protected void Rotate_Mouse()
    {
        // ���� ó�� (�̹� ȸ�� ���� ���)
        if (m_IsRotating)
            return;

        // ���� ó�� (������ ��尡 ���� ��)
        if (null == m_SelectedNode)
            return;

        // ���콺 �����Ͱ� UI���� ���� ��
        if (false == UnityEngine.EventSystems.EventSystem.current?.IsPointerOverGameObject())
        {
            // ��Ŭ����
            if (Input.GetMouseButtonUp(0))
            {
                // Ŭ���� ������ ����ĳ��Ʈ
                Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
                float maxDistance = MainCamera.farClipPlane;
                int layerMask = LayerMask.GetMask("Node", "NodeRotate");

                RaycastHit hit;

                // ����ĳ��Ʈ ���� ��
                if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
                {
                    // ���� ���
                    E_Direction from = SelectedNodeDir;
                    E_Direction to = (E_Direction)Enum.Parse(typeof(E_Direction), hit.transform.parent.name);

                    // �ð� ���� ȸ��
                    if ((int)to == (int)(from + 1) % (int)E_Direction.Max)
                    {
                        CWRotate();
                    }
                    // �ݽð� ���� ȸ��
                    else if ((int)to == (int)(from + (int)E_Direction.Max - 1) % (int)E_Direction.Max)
                    {
                        CCWRotate();
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
    }
    // Ű���� ȸ��
    protected void Rotate_Keyboard()
    {
        // ���� ó�� (�̹� ȸ�� ���� ���)
        if (m_IsRotating)
            return;

        // ���� ó�� (������ ��尡 ���� ��)
        if (null == m_SelectedNode)
            return;

        // QŰ�� ���� ���
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // �ݽð� ���� ȸ��
            CCWRotate();
            return;
        }

        // EŰ�� ���� ���
        if (Input.GetKeyDown(KeyCode.E))
        {
            // �ð� ���� ȸ��
            CWRotate();
        }
    }
    // �� ������ ȸ��
    // ����: http://devkorea.co.kr/bbs/board.php?bo_table=m03_qna&wr_id=95809
    protected IEnumerator RotateNode(bool clockwise = true)
    {
        // ȸ�� ���� ����
        m_IsRotating = true;
        // ȸ�� ���� ��� ����
        m_RotationStandardNode = m_SelectedNode;

        // ���⺰ ��� �θ�
        Dictionary<E_Direction, Transform> node_parent_by_dir = m_NodeParentList[StandardNodeType];

        // �� ����� �ð�
        float time = 0f;
        // ȸ���� ����
        int Dir = clockwise ? 1 : -1;
        // ȸ���� ����
        float angle;

        // ���� ó�� (ȸ���� �ɸ��� �ð��� 0������ ���)
        if (m_Duration <= 0f)
        {
            // ȸ���� ���� ���
            angle = Dir * m_RotateAngle;
            // ȸ���� ������ŭ ��� ȸ��
            for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
                }
            }
            // Ÿ���� �ٶ� ���⵵ ȸ��
            UpdateTowerLookingDir(angle);

            // ��� ������Ʈ
            UpdateNode(clockwise);

            // Ÿ�� ���� ���� ����
            UpdateTowerAttack(true);

            // ȸ�� ���� ����
            m_IsRotating = false;

            // ȸ�� ���� �̺�Ʈ ȣ��
            m_RotateEndEvent?.Invoke();

            // ȸ�� ����
            yield break;
        }

        // Ÿ�� ���� ���� ����
        UpdateTowerAttack(false);

        // ���� �̵�
        for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                node_parent_by_dir[i].GetChild(j).Translate(Vector3.up * 3f);
            }
        }

        // ������ �ð����� ȸ��
        while (time < m_Duration)
        {
            // ȸ���� ���� ���
            angle = Dir * m_RotateAngle * Time.deltaTime / m_Duration;
            // ȸ��
            for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
                }
            }
            // Ÿ���� �ٶ� ���⵵ ȸ��
            UpdateTowerLookingDir(angle);

            // �� ��� �ð� ����
            time += Time.deltaTime;

            // ���� �����ӱ��� ���
            yield return null;
        }

        // ȸ���� ���� ���
        angle = Dir * m_RotateAngle * (m_Duration - time) / m_Duration;
        // ���� ���� ���� (���ϴ� �ð� ���� +�� ��ŭ �ݴ�������� ȸ��)
        for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, angle);
            }
        }
        // Ÿ���� �ٶ� ���⵵ ���� ���� ����
        UpdateTowerLookingDir(angle);

        // �Ʒ��� �̵�
        for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                node_parent_by_dir[i].GetChild(j).Translate(Vector3.down * 3f);
            }
        }

        #region �ν����� ������
#if UNITY_EDITOR
        for (E_Direction i = E_Direction.None + 1; i < E_Direction.Max; ++i)
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

        // ��� ������Ʈ
        UpdateNode(clockwise);

        // Ÿ�� ���� ���� ����
        UpdateTowerAttack(true);

        // ȸ�� ���� ����
        m_IsRotating = false;

        // ȸ�� ���� �̺�Ʈ ȣ��
        m_RotateEndEvent?.Invoke();
    }
    // ��� ������Ʈ
    protected void UpdateNode(bool clockwise = true)
    {
        // �ð� ����
        if (clockwise)
        {
            SwapList(E_Direction.North, E_Direction.East);
            SwapList(E_Direction.South, E_Direction.West);
            SwapList(E_Direction.North, E_Direction.South);
        }
        // �ݽð� ����
        else
        {
            SwapList(E_Direction.North, E_Direction.West);
            SwapList(E_Direction.South, E_Direction.East);
            SwapList(E_Direction.North, E_Direction.South);
        }
    }
    // ����Ʈ ��ȯ
    protected void SwapList(E_Direction first, E_Direction second)
    {
        List<Node> FirstList = m_NodeList[StandardNodeType][first];
        List<Node> SecondList = m_NodeList[StandardNodeType][second];
        List<Node> TempList = new List<Node>(FirstList);

        Transform First_T = m_NodeParentList[StandardNodeType][first];
        Transform Second_T = m_NodeParentList[StandardNodeType][second];
        List<Transform> Temp_T = new List<Transform>();

        int FirstCount = First_T.childCount;
        int SecondCount = Second_T.childCount;

        // 1 -> Temp
        for (int i = 0; i < FirstCount; ++i)
        {
            Temp_T.Add(First_T.GetChild(i));
        }
        // 2 -> 1
        FirstList.Clear();
        FirstList.AddRange(SecondList);
        for (int i = 0; i < SecondCount; ++i)
        {
            Second_T.GetChild(0).SetParent(First_T);
        }
        // Temp -> 2
        SecondList.Clear();
        SecondList.AddRange(TempList);
        for (int i = 0; i < FirstCount; ++i)
        {
            Temp_T[i].SetParent(Second_T);
        }

        // ���� ������Ʈ
        foreach (var item in FirstList)
        {
            // ��� ���� ������Ʈ
            item.m_Direction = first;

            // Ÿ�� ���� ������Ʈ
            if (null != item.m_Tower)
                item.m_Tower.Direction = first;
        }
        foreach (var item in SecondList)
        {
            // ��� ���� ������Ʈ
            item.m_Direction = second;

            // Ÿ�� ���� ������Ʈ
            if (null != item.m_Tower)
                item.m_Tower.Direction = second;
        }
    }
    // Ÿ�� �ٶ� ���� ������Ʈ
    protected void UpdateTowerLookingDir(float angle)
    {
        // Ÿ�� �ٶ� ���� ������Ʈ
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            foreach (var item in m_NodeList[StandardNodeType][i])
            {
                if (null != item.m_Tower)
                {
                    m_LookingDir.transform.position = item.m_Tower.m_TowerInfo.LookingDir;
                    m_LookingDir.transform.RotateAround(m_Center.transform.position, Vector3.up, angle);
                    item.m_Tower.m_TowerInfo.LookingDir = m_LookingDir.transform.position;
                }
            }
        }
    }
    // Ÿ�� ���� ���� ������Ʈ
    protected void UpdateTowerAttack(bool flag)
    {
        // Ÿ�� ���� ���� ������Ʈ
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            foreach (var item in m_NodeList[StandardNodeType][i])
            {
                if (null != item.m_Tower)
                {
                    item.m_Tower.CanAttack = flag;
                }
            }
        }
    }
    #endregion

    public List<Node> GetNodeList(E_Direction dir)
    {
        List<Node> result = new List<Node>();
        result.AddRange(m_NodeList[E_NodeType.Inner][dir]);
        result.AddRange(m_NodeList[E_NodeType.Outer][dir]);
        return result;
    }

    // �ð� ���� ȸ��
    protected void CWRotate()
    {
        StartCoroutine(RotateNode());
    }
    // �ݽð� ���� ȸ��
    protected void CCWRotate()
    {
        StartCoroutine(RotateNode(false));
    }
}

public enum E_NodeType
{
    None = -1,

    Inner,
    Outer,

    Max
}