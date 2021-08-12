using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[System.Serializable]
public struct ShopSlotInfo
{
    public bool isOccupied;
    public int index;

    public int rank;
    public int cost;
    public Tower_TableExcel? excel_data;
}

// Shop slot 에 관한 정보를 담는 클래스
public class ShopSlot : MonoBehaviour , IPointerClickHandler
{
    delegate void OnInfoChangedHandler();
    OnInfoChangedHandler OnInfoChangedCallback;

    [SerializeField] ShopSlotInfo m_info;

    [Space(10)]
    [SerializeField] TextMeshProUGUI m_nameTextPro;
    [SerializeField] TextMeshProUGUI m_synergy1TextPro;
    [SerializeField] TextMeshProUGUI m_synergy2TextPro;
    [SerializeField] TextMeshProUGUI m_goldTextPro;

    [Space(10)]
    [SerializeField] Image m_goldImage;
    [SerializeField] Image m_synergy1Image;
    [SerializeField] Image m_synergy2Image;

    [Space(10)]
    [SerializeField] RawImage m_towerImage;
    [SerializeField] Prefab_TableExcelLoader m_loader;
    [SerializeField] Vector3 camera_distance;           // 오브젝트로부터의 거리
    [SerializeField] Vector3 camera_rotation;           // 카메라 회전 값
    [SerializeField] Vector3 m_obj_position;            // 간섭 없는 곳으로 셋팅할것
    [SerializeField] List<CKeyValue> m_showObj_list;
    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    Outline[] m_outlines = null;

    public bool IsOccupied { get { return m_info.isOccupied; } }

    private void Awake()
    {
        OnInfoChangedCallback += OnInfoChanged;
        SetRenderTexture();
    }


    public void SetRenderTexture()
    {
        

        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();

        Camera shop_cam_origin = Resources.Load<Camera>("ShopCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(shop_cam_origin);

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;

        // 아래 오브젝트들은 개수만큼 추가 되어야 함

        GameObject origin_obj = m_loader.GetPrefab(m_loader.DataList[0].Code);
        GameObject new_obj = GameObject.Instantiate(origin_obj);

        CKeyValue val = new CKeyValue
        { Code = m_loader.DataList[0].Code, obj = new_obj };
        m_showObj_list.Add(val);

        foreach (var item in m_showObj_list)
        {   // 전부 꺼논 상태로 같은 위치에 놓기            
            item.obj.transform.position = m_obj_position;
            item.obj.gameObject.SetActive(true);
        }

        m_towerImage.texture = m_renderTexture;
    }


    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    // index 는 초기화 하지 않음
    public void ClearInfo()
    {
        m_info.isOccupied = false;

        m_info.rank = 0;
        m_info.cost = 0;
        m_info.excel_data = null;

        OnInfoChangedCallback?.Invoke();
    }

    // slot 을 비워야 할 경우는 ClearInfo를 사용할 것    
    public void SetInfo(int rank, int cost, Tower_TableExcel excel)
    {
        m_info.isOccupied = true;

        m_info.rank = rank;
        m_info.cost = cost;
        m_info.excel_data = excel;

        var data = m_info.excel_data.Value;

        // ui setting
        m_nameTextPro.text = data.Name_EN;
        m_synergy1TextPro.text = data.Type1.ToString();
        m_synergy2TextPro.text = data.Type1.ToString(); ;
        m_goldTextPro.text = 10.ToString();
        // ui end

        OnInfoChangedCallback?.Invoke();
    }
    public ShopSlotInfo GetInfo()
    {
        return m_info;
    }


    // 현재 정보에 따라 ui 변경 process
    void OnInfoChanged()
    {       
        // TODO 아예 꺼버리는 대신에 비활성화 상태만 되도록 만들기
        if(IsOccupied)
        {
            SetActivateAllChildren();
        }
        else
        {
            SetDeActivateChilderen();
        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        Debug.Log($"Shop Slot ({m_info.index})");

        // 이 슬롯이 활성화 된 경우
        if(IsOccupied)
        {
            if(ShopManager.Instance.PurchaseProcess(m_info.index))
            {   // 구매에 성공한 경우
                ClearInfo();
            }
        }
    }

    public void SetActivateAllChildren()
    {
        Transform[] children = this.transform.GetComponentsInChildren<Transform>(true);
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);
        }        
    }
    public void SetDeActivateChilderen()
    { 
        Transform[] children = this.transform.GetComponentsInChildren<Transform>();
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
        }       
    }
}
