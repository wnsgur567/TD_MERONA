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
{   // code to tower object
    public int Code;
    public GameObject obj;

    public bool Equals(CKeyValue other)
    {
        if (Code == other.Code)
            return true;
        return false;
    }
}

public class InventorySlotGUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler//IPointerEnterHandler,IPointerExitHandler
{
    public delegate void InfoChangeHandler();
    public event InfoChangeHandler OnInfoChangedEvent;

    [SerializeField] InventorySlotGUIInfo m_info;
    [SerializeField] TMPro.TextMeshProUGUI m_textPro;

    [Space(10)]
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    [SerializeField] Prefab_TableExcelLoader m_prefabLoader;
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

    //TowerUI_Tooltip tower_tooltip;


    public bool IsOccupied { get { return m_info.isOccupied; } }

    private void Awake()
    {
        //tower_tooltip = TowerUI_Tooltip.Instance;
        OnInfoChangedEvent += OnInfoChanged;

        m_rt = this.GetComponent<RectTransform>();
        SetRenderTexture();
    }

    private void Start()
    {

    }

    public void SetRenderTexture()
    {
        // create render texture
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();


        /// camera setting
        Camera inven_cam_origin = Resources.Load<Camera>("InventoryCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(inven_cam_origin);
        m_renderCamera.transform.SetParent(this.transform);

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;
        /// camera setting end

        /// tower objects setting
        // create all tower for this InvenSlotUI
        foreach (var item in m_towerLoader.DataList)
        {
            GameObject origin_obj = m_prefabLoader.GetPrefab(item.Prefab); // get only tower
            GameObject new_obj = GameObject.Instantiate(origin_obj);
            new_obj.transform.SetParent(this.transform);

            // regist to managing list
            CKeyValue val = new CKeyValue
            { Code = item.Code, obj = new_obj };
            m_showObj_list.Add(val);
        }

        // deactivate all created objects
        foreach (var item in m_showObj_list)
        {
            item.obj.transform.position = m_obj_position;
            item.obj.gameObject.SetActive(false);
        }
        /// tower setting end

        // set render texture
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


    // InvenSlot Swap
    private void SwapInfo(InventorySlotGUI otherSlotGUI)
    {
        // tower swap
        var this_tower_data = m_info.tower_data;
        bool this_occupied = m_info.isOccupied;

        m_info.tower_data = otherSlotGUI.m_info.tower_data;
        m_info.isOccupied = otherSlotGUI.m_info.isOccupied;

        otherSlotGUI.m_info.tower_data = this_tower_data;
        otherSlotGUI.m_info.isOccupied = this_occupied;

        otherSlotGUI.OnInfoChangedEvent?.Invoke();
        OnInfoChangedEvent?.Invoke();       // change ui
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
        m_showObj?.SetActive(false);

        if (IsOccupied == false)
        {   // is empty slot (tower)            
            m_showObj = null;
            m_textPro.text = "Empty";
            return;
        }

        // is occupied (tower)
        // 1. find tower using code
        // 2. set tower infomation to UI
        int code = m_info.tower_data.Code;
        m_showObj = m_showObj_list.Find((item) => { return item.Code == code; }).obj;
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
        m_drag_startPos = m_rawImage.transform.position;
        if (m_info.isOccupied)
        {
            Debug.Log("tower image move start");
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {

        RaycastResult target = eventData.pointerCurrentRaycast;
        if (target.gameObject != null && target.gameObject.tag.Equals("InvenSlot"))
        {
            // if.. begin drag on Slot UI && end drag on Slot UI
            // swap slot infomation
            SwapInfo(target.gameObject.GetComponent<InventorySlotGUI>());
        }
        else if (m_info.isOccupied)
        {

        }

        // reset moved image position
        Debug.Log("tower image move end");
        m_rawImage.transform.position = m_drag_startPos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {   // for tower tooltip panel
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Vector2 mousepos = Input.mousePosition;
            //tower_tooltip.Set_TowerTT_Pos(mousepos);
            //tower_tooltip.Set_TowerTT(m_info.tower_data);
        }
    }
}
