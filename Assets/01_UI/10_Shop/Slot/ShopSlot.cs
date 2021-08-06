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

    [SerializeField] TextMeshProUGUI m_nameTextPro;
    [SerializeField] TextMeshProUGUI m_synergy1TextPro;
    [SerializeField] TextMeshProUGUI m_synergy2TextPro;
    [SerializeField] TextMeshProUGUI m_goldTextPro;

    [SerializeField] Image m_towerImage;
    [SerializeField] Image m_goldImage;
    [SerializeField] Image m_synergy1Image;
    [SerializeField] Image m_synergy2Image;

    Outline[] m_outlines = null;

    private void Awake()
    {
        OnInfoChangedCallback += OnInfoChanged;
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

        OnInfoChangedCallback();
    }

    // slot 을 비워야 할 경우는 ClearInfo를 사용할 것    
    public void SetInfo(int rank, int cost, Tower_TableExcel excel)
    {
        m_info.isOccupied = true;

        m_info.rank = rank;
        m_info.cost = cost;
        m_info.excel_data = excel;

        OnInfoChangedCallback();
    }
    public ShopSlotInfo GetInfo()
    {
        return m_info;
    }


    // 현재 정보에 따라 ui 변경 process
    void OnInfoChanged()
    {
        if(m_info.isOccupied)
        {

        }
        else
        {

        }       
    }

    public void OnPointerClick(PointerEventData eventData)
    {        
        Debug.Log($"Shop Slot ({m_info.index})");

        return;

        // Invetory 에 빈 공간이 있는 경우
        if(false ==InventoryManager.Instance.IsAllOccupied())
        {
            // TODO : tower manager 를 통해 tower 생성하고
            Tower newTower = null;

            // TODO : 합성이 필요하다면 합성 process 를 진행

            // 생성한 타워를 인벤토리에 등록
            InventoryManager.Instance.AddNewTower(newTower);
        }
    }
}
