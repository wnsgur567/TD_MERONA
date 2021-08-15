using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct InventorySlotGUIInfo
{
    public int index;
    public bool isOccupied;
    public Tower_TableExcel tower_data;
}

[System.Serializable]
public class CKeyValue : System.IEquatable<CKeyValue>
{
    public int Code;
    public GameObject obj;

    public bool Equals(CKeyValue other)
    {
        if (Code == other.Code)
            return true;
        return false;
    }
}

public class InventorySlotGUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,IPointerClickHandler//IPointerEnterHandler,IPointerExitHandler
{
    public delegate void InfoChangeHandler();
    public event InfoChangeHandler OnInfoChangedEvent;

    [SerializeField] InventorySlotGUIInfo m_info;
    [SerializeField] TMPro.TextMeshProUGUI m_textPro;

    [Space(10)]
    [SerializeField] Prefab_TableExcelLoader m_loader;
    [SerializeField] Vector3 camera_distance;           // ������Ʈ�κ����� �Ÿ�
    [SerializeField] Vector3 camera_rotation;           // ī�޶� ȸ�� ��
    [SerializeField] Vector3 m_obj_position;            // ���� ���� ������ �����Ұ�
    [SerializeField] List<CKeyValue> m_showObj_list;    // �Ʒ��� ���� �ִ� ������Ʈ ����Ʈ
    GameObject m_showObj;   // ���� �����ְ� �ִ� ������Ʈ

    [Space(10)]
    [SerializeField] RawImage m_rawImage;
    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    RectTransform m_rt;

    TowerUI_Tooltip tower_tooltip;
   

    public bool IsOccupied { get { return m_info.isOccupied; } }

    private void Awake()
    {
        tower_tooltip = TowerUI_Tooltip.Instance;
        OnInfoChangedEvent += OnInfoChanged;
       
        m_rt = this.GetComponent<RectTransform>();
        SetRenderTexture();
    }

    private void Start()
    {
        
    }

    public void SetRenderTexture()
    {
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();

        Camera inven_cam_origin = Resources.Load<Camera>("InventoryCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(inven_cam_origin);

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;

        GameObject origin_obj = m_loader.GetPrefab(m_loader.DataList[0].Code);
        GameObject new_obj = GameObject.Instantiate(origin_obj);

        CKeyValue val = new CKeyValue 
            { Code = m_loader.DataList[0].Code, obj = new_obj };
        m_showObj_list.Add(val);

        foreach (var item in m_showObj_list)
        {   // ���� ���� ���·� ���� ��ġ�� ����            
            item.obj.transform.position = m_obj_position;
            item.obj.gameObject.SetActive(false);
        }

        m_rawImage.texture = m_renderTexture;
    }

    public void MoveRenderPosition(Vector3 delta)
    {
        m_renderCamera.transform.position += delta;
        foreach (var item in m_showObj_list)
        {
            item.obj.transform.position += delta;
        }
    }


    private void Update()
    {

    }

    // InvenSlot ���� ��ȯ
    private void SwapInfo(InventorySlotGUI slotGUI)
    {
        // tower swap
        var this_tower_data = m_info.tower_data;
        bool this_occupied = m_info.isOccupied;

        m_info.tower_data = slotGUI.m_info.tower_data;
        m_info.isOccupied = slotGUI.m_info.isOccupied;

        slotGUI.m_info.tower_data = this_tower_data;
        slotGUI.m_info.isOccupied = this_occupied;

        slotGUI.OnInfoChangedEvent?.Invoke();
        OnInfoChangedEvent?.Invoke();
    }

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    public void SetTower(Tower_TableExcel data)
    {
        m_info.tower_data = data;
        m_info.isOccupied = true;

        OnInfoChangedEvent?.Invoke();
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////

    private void OnInfoChanged()
    {
        if(IsOccupied == false)
        {   // ����ִ� ���
            m_showObj?.gameObject.SetActive(false);
            m_showObj = null;
            m_textPro.text = "�������";
            return;
        }

        // �ִ� ���
        int code = m_info.tower_data.Code;
        CKeyValue find = m_showObj_list.Find((item) => { return item.Code == code; });
        if(null != find)
        {
            m_showObj = find.obj;
            m_showObj.gameObject.SetActive(true);
            m_textPro.text = m_info.tower_data.Name_EN;
        }

        // TODO : fix ... �׽�Ʈ�� 1���� �׳� ����...
        m_showObj = m_showObj_list[0].obj;
        m_showObj.gameObject.SetActive(true);
        m_textPro.text = m_info.tower_data.Name_EN;
    }

   

    ////////////////////////////////////////////////////////////////////////////////////////////////

    Vector3 m_drag_startPos;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.IsPointerMoving() && m_info.isOccupied)
        {
            Debug.Log("tower image moving");
            m_rawImage.transform.position = eventData.position;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_info.isOccupied)
        {
            Debug.Log("tower image move start");
            m_drag_startPos = m_rawImage.transform.position;
        }

            
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        RaycastResult target = eventData.pointerCurrentRaycast;
        if (target.gameObject != null && target.gameObject.tag.Equals("InvenSlot"))
        {
            // �� �κ��丮�� Ÿ�� ���� ��ü
            SwapInfo(target.gameObject.GetComponent<InventorySlotGUI>());
        }
        else if (m_info.isOccupied)
        {

        }

        Debug.Log("tower image move end");
        m_rawImage.transform.position = m_drag_startPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {       
            Vector2 mousepos = Input.mousePosition;
            tower_tooltip.Set_TowerTT_Pos(mousepos);
            tower_tooltip.Set_TowerTT(m_info.tower_data);
        }
    }
    //�߰��ؾ��Ұ�: �����κ� �̿��� ���� Ŭ���� ���� �������ϱ�.
}
