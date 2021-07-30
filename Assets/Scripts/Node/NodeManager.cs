using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : Singleton<NodeManager>
{
    // 회전할 각도
    public float m_RotateAngle;
    // 회전에 걸리는 시간
    public float m_Duration;
    // 회전의 중심점
    [ReadOnly]
    public Transform m_Center;

    // [타입(안, 밖)][방향(북, 동, 남, 서)]
    // 회전시킬 노드 리스트
    public List<Node>[][] m_NodeList;
    // 노드 부모 (동서남북)
    public Transform[][] m_NodeParentList;

    // 선택 노드
    protected Node m_SelectedNode;
    // 회전 기준 노드 (회전을 시작할 때 선택한 노드)
    protected Node m_StandardNode_Rotation;
    // 회전 여부
    protected bool m_IsRotating;

    #region 내부 프로퍼티
    // 선택 노드의 타입
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
    // 선택 노드의 방향
    protected E_Direction? SelectedNodeDir
    {
        get
        {
            if (null == m_SelectedNode)
                return null;

            return (E_Direction)Enum.Parse(typeof(E_Direction), m_SelectedNode.transform.parent.name);
        }
    }
    // 회전 기준 노드의 타입
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
    // 현재 카메라
    protected Camera MainCamera => Camera.main;
    #endregion

    private void Awake()
    {
        // 회전 중심점 설정
        m_Center = transform.Find("Center");
        if (null == m_Center)
        {
            Debug.LogError("회전의 중심점이 없거나 잘못된 이름을 입력했습니다.");
            Debug.LogError("회전의 중심점의 이름은 \"Center\"여야 합니다.");
        }

        // 초기화
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
        // 선택 노드 검색
        MouseProcess();

        // 회전
        RotateProcess();
    }

    // 선택 노드 검색
    protected void MouseProcess()
    {
        // 좌클릭 시
        if (Input.GetMouseButtonDown(0))
        {
            // 클릭한 곳으로 레이캐스트
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            float maxDistance = MainCamera.farClipPlane;
            int layerMask = LayerMask.GetMask("Node");

            RaycastHit hit;

            // 레이캐스트 성공 시
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // 선택 노드 설정
                SelectNode(hit.transform.GetComponentInParent<Node>());
            }
            // 레이캐스트 실패 시
            else
            {
                // 선택 노드 제거
                SelectNode(null);
            }
        }
    }

    // 아웃라인 설정
    protected void UpdateOutline(bool active)
    {
        // 예외 처리 (null 체크)
        if (SelectedNodeType != null)
        {
            // 아웃라인 설정할 노드 리스트
            List<Node>[] nodes = m_NodeList[(int)SelectedNodeType];

            // 아웃라인 on, off 설정
            for (int i = 0; i < nodes.Length; ++i)
            {
                for (int j = 0; j < nodes[i].Count; ++j)
                {
                    nodes[i][j].m_Outline.SetActive(active);
                }
            }
        }
    }
    // 노드 선택
    protected void SelectNode(Node node)
    {
        // 선택 노드 아웃라인 제거
        UpdateOutline(false);

        // 선택 노드 변경
        m_SelectedNode = node;

        // 선택 노드 아웃라인 생성
        UpdateOutline(true);
    }

    // 회전
    protected void RotateProcess()
    {
        // 선택 노드가 존재하면
        if (null != SelectedNodeType)
        {
            // 마우스 회전 검사
            Rotate_Mouse();
            // 키보드 회전 검사
            Rotate_Keyboard();
        }
    }

    // 마우스 회전
    protected void Rotate_Mouse()
    {
        // 예외 처리 (이미 회전 중인 경우)
        if (m_IsRotating)
            return;

        // 좌클릭시
        if (Input.GetMouseButtonUp(0))
        {
            // 클릭한 곳으로 레이캐스트
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            float maxDistance = MainCamera.farClipPlane;
            int layerMask = LayerMask.GetMask("NodeRotate");

            RaycastHit hit;

            // 레이캐스트 성공 시
            if (Physics.Raycast(ray, out hit, maxDistance, layerMask))
            {
                // 방향 계산
                E_Direction from = (E_Direction)SelectedNodeDir;
                E_Direction to = (E_Direction)Enum.Parse(typeof(E_Direction), hit.transform.parent.name);

                // 시계 방향 회전
                if ((int)to == (int)(from + 1) % (int)E_Direction.Max)
                {
                    StartCoroutine(RotateNode());
                }
                // 반시계 방향 회전
                else if ((int)to == (int)(from + (int)E_Direction.Max - 1) % (int)E_Direction.Max)
                {
                    StartCoroutine(RotateNode(false));
                }
            }
            // 레이캐스트 실패 시
            else
            {
                // 선택 노드 제거
                SelectNode(null);
            }
        }
    }
    // 키보드 회전
    protected void Rotate_Keyboard()
    {
        // 예외 처리 (이미 회전 중인 경우)
        if (m_IsRotating)
            return;

        // Q키를 누른 경우
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // 반시계 방향 회전
            StartCoroutine(RotateNode(false));
        }

        // 예외 처리 (이미 회전 중인 경우)
        if (m_IsRotating)
            return;

        // E키를 누른 경우
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 시계 방향 회전
            StartCoroutine(RotateNode());
        }
    }

    // 매 프레임 회전
    protected IEnumerator RotateNode(bool clockwise = true)
    {
        // 회전 여부 설정
        m_IsRotating = true;
        // 회전 기준 노드 설정
        m_StandardNode_Rotation = m_SelectedNode;

        // 방향별 노드 부모
        Transform[] node_parent_by_dir = m_NodeParentList[(int)StandardNodeType];

        // 총 경과한 시간
        float time = 0f;
        // 회전할 방향
        int Dir = clockwise ? 1 : -1;

        // 예외 처리 (회전에 걸리는 시간이 0이하인 경우)
        if (m_Duration <= 0f)
        {
            // 회전할 각도만큼 즉시 회전
            for (int i = 0; i < node_parent_by_dir.Length; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle);
                }
            }

            // 회전 종료
            yield break;
        }

        // 정해진 시간동안 회전
        while (time < m_Duration)
        {
            // 현재 프레임까지 걸린 시간만큼 비례하여 회전
            for (int i = 0; i < node_parent_by_dir.Length; ++i)
            {
                for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
                {
                    node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle * Time.deltaTime / m_Duration);
                }
            }

            // 총 경과 시간 증가
            time += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }

        // 오차 범위 수정 (원하는 시간 보다 +된 만큼 반대방향으로 회전)
        for (int i = 0; i < node_parent_by_dir.Length; ++i)
        {
            for (int j = 0; j < node_parent_by_dir[i].childCount; ++j)
            {
                node_parent_by_dir[i].GetChild(j).RotateAround(m_Center.transform.position, Vector3.up, Dir * m_RotateAngle * (m_Duration - time) / m_Duration);
            }
        }

        #region 인스펙터 정리용
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

        // 노드 부모 업데이트
        UpdateParent(StandardNodeType, clockwise);
        // 회전 여부 설정
        m_IsRotating = false;
    }

    // 부모 업데이트
    protected void UpdateParent(E_NodeType? type, bool clockwise = true)
    {
        // 시계 방향
        if (clockwise)
        {
            SwapParent(type, E_Direction.North, E_Direction.East);
            SwapParent(type, E_Direction.South, E_Direction.West);
            SwapParent(type, E_Direction.North, E_Direction.South);
        }
        // 반시계 방향
        else
        {
            SwapParent(type, E_Direction.North, E_Direction.West);
            SwapParent(type, E_Direction.South, E_Direction.East);
            SwapParent(type, E_Direction.North, E_Direction.South);
        }
    }
    // 부모 교환
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
