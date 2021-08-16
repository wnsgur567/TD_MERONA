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

    public int cost;
    public Tower_TableExcel? excel_data;
}

// Shop slot �� ���� ������ ��� Ŭ����
public class ShopSlot : MonoBehaviour , IPointerClickHandler
{
    delegate void OnInfoChangedHandler();
    OnInfoChangedHandler OnInfoChangedCallback;

    [SerializeField] ShopSlotInfo m_info;

    [Space(30)]
    [SerializeField] TextMeshProUGUI m_nameTextPro;
    [SerializeField] TextMeshProUGUI m_synergy1TextPro;
    [SerializeField] TextMeshProUGUI m_synergy2TextPro;
    [SerializeField] TextMeshProUGUI m_goldTextPro;

    [Space(30)]
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Synergy_TableExcelLoader m_synergyLoader;
    [SerializeField] Image m_goldImage;
    [SerializeField] Image m_synergy1Image;
    [SerializeField] Image m_synergy2Image;

    [Space(30)]
    [SerializeField] RawImage m_towerImage;
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    [SerializeField] Prefab_TableExcelLoader m_prefabLoader;
    [SerializeField] Vector3 camera_distance;           // ������Ʈ�κ����� �Ÿ�
    [SerializeField] Vector3 camera_rotation;           // ī�޶� ȸ�� ��
    [SerializeField] Vector3 m_obj_position;            // ���� ���� ������ �����Ұ�
    [SerializeField] List<CKeyValue> m_showObj_list;
    GameObject m_showObj = null;

    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    Dictionary<int, Color> m_rankToColor_dic;

    public bool IsOccupied { get { return m_info.isOccupied; } }

    private void Awake()
    {
        m_rankToColor_dic = new Dictionary<int, Color>();
        m_rankToColor_dic.Add(1, Color.gray);
        m_rankToColor_dic.Add(2, Color.green);
        m_rankToColor_dic.Add(3, Color.blue);
        m_rankToColor_dic.Add(4, new Color(0.7f,0f,1f));
        m_rankToColor_dic.Add(5, Color.yellow);



        OnInfoChangedCallback += OnInfoChanged;
        SetRenderTexture();
    }

    public void MoveRenderPosition(Vector3 delta)
    {
        m_obj_position += delta;
        m_renderCamera.transform.position += delta;
        foreach (var item in m_showObj_list)
        {
            item.obj.transform.position += delta;
        }
    }
    public void SetRenderTexture()
    {
        // create render texture
        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();

        /// camera setting
        Camera shop_cam_origin = Resources.Load<Camera>("ShopCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(shop_cam_origin);
        m_renderCamera.transform.SetParent(this.transform);

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;
        /// camera setting end

        /// tower objects setting
        // create all tower for this ShopSlotUI
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
        m_towerImage.texture = m_renderTexture;
    }

       

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    // index �� �ʱ�ȭ ���� ����
    public void ClearInfo()
    {
        m_info.isOccupied = false;

        m_info.cost = 0;
        m_info.excel_data = null;

        OnInfoChangedCallback?.Invoke();
    }

    // slot �� ����� �� ���� ClearInfo�� ����� ��    
    public void SetInfo(int cost, Tower_TableExcel excel)
    {
        m_info.isOccupied = true;
        
        m_info.cost = cost;
        m_info.excel_data = excel;     

        OnInfoChangedCallback?.Invoke();
    }
    public ShopSlotInfo GetInfo()
    {
        return m_info;
    }

    void DeActivateAll()
    {
        foreach (var item in m_showObj_list)
        {
            item.obj.SetActive(false);
        }
    }


    // ���� ������ ���� ui ���� process
    void OnInfoChanged()
    {       
        // TODO �ƿ� �������� ��ſ� ��Ȱ��ȭ ���¸� �ǵ��� �����
        if(IsOccupied)
        {
            ///tower obj
            // deactivate previous obj
            m_showObj?.SetActive(false);
            // get current data
            var data = m_info.excel_data.Value;
            // activate current obj
            m_showObj = m_showObj_list.Find((item) => { return item.Code == data.Code; }).obj;
            m_showObj.SetActive(true);
            /// tower obj end

            /// synergy icon & text          
            // find synergy data
            var synergy1_data = m_synergyLoader.DataList.Find(
                (item) => { return item.Code == data.Type1; });
            var synergy2_data = m_synergyLoader.DataList.Find(
                (item) => { return item.Code == data.Type2; });

            // set sprite
            Sprite synergy1_sprite = m_spriteLoader.GetSprite(synergy1_data.Synergy_icon);
            m_synergy1Image.sprite = synergy1_sprite;
            Sprite synergy2_sprite = m_spriteLoader.GetSprite(synergy2_data.Synergy_icon);
            m_synergy2Image.sprite = synergy2_sprite;             


            // set text
            m_nameTextPro.text = data.Name_KR;
            m_nameTextPro.color = m_rankToColor_dic[data.Rank];
            m_synergy1TextPro.text = synergy1_data.Name_KR;
            m_synergy2TextPro.text = synergy2_data.Name_KR;
            m_goldTextPro.text = ((int)data.Price).ToString();
            /// synergy icon & text end           


            SetActivateAllChildren();
        }
        else
        {
            // deactivate previous obj
            m_showObj?.SetActive(false);

            SetDeActivateChilderen();
        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        Debug.Log($"Shop Slot ({m_info.index})");

        // �� ������ Ȱ��ȭ �� ���
        if(IsOccupied)
        {
            if(ShopManager.Instance.PurchaseProcess(m_info.index))
            {   // ���ſ� ������ ���
                ClearInfo();
            }
        }
    }

    public void SetActivateAllChildren()
    {
        RectTransform[] children = this.transform.GetComponentsInChildren<RectTransform>(true);
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(true);
        }        
    }
    public void SetDeActivateChilderen()
    {
        RectTransform[] children = this.transform.GetComponentsInChildren<RectTransform>();
        for (int i = 1; i < children.Length; i++)
        {
            children[i].gameObject.SetActive(false);
        }       
    }
}
