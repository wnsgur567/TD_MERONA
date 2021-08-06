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

// Shop slot �� ���� ������ ��� Ŭ����
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

    // index �� �ʱ�ȭ ���� ����
    public void ClearInfo()
    {
        m_info.isOccupied = false;

        m_info.rank = 0;
        m_info.cost = 0;
        m_info.excel_data = null;

        OnInfoChangedCallback();
    }

    // slot �� ����� �� ���� ClearInfo�� ����� ��    
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


    // ���� ������ ���� ui ���� process
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

        // Invetory �� �� ������ �ִ� ���
        if(false ==InventoryManager.Instance.IsAllOccupied())
        {
            // TODO : tower manager �� ���� tower �����ϰ�
            Tower newTower = null;

            // TODO : �ռ��� �ʿ��ϴٸ� �ռ� process �� ����

            // ������ Ÿ���� �κ��丮�� ���
            InventoryManager.Instance.AddNewTower(newTower);
        }
    }
}
