using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

class DummyTowerClass
{

}

[System.Serializable]
public struct InvetorySlotInfo
{
    public int index;    

    public string name;
    public int rank;
    public string type;
    public int price;

    DummyTowerClass tower;
}

public class InventorySlot : MonoBehaviour
{
    delegate void OnInfoChangedHandler();
    OnInfoChangedHandler OnInfoChangedCallback;
    [SerializeField] bool isOccupied;
    [SerializeField] InvetorySlotInfo m_info;

    [SerializeField] TextMeshProUGUI m_nameText;
    [SerializeField] TextMeshProUGUI m_typeText;
    [SerializeField] TextMeshProUGUI m_goldText;
    [SerializeField] Image m_towerImage;

    public void __Indexing(int index)
    {
        m_info.index = index;
    }

    public void SetInfo(string name, int rank, string type, int price)
    {
        m_info.name = name;
        m_info.rank = rank;
        m_info.type = type;
        m_info.price = price;

        OnInfoChangedCallback();
    }
    public InvetorySlotInfo GetInfo()
    {
        return m_info;
    }

    void OnInfoChanged()
    {
        m_nameText.text = m_info.name;
        m_typeText.text = m_info.type;
        m_goldText.text = m_info.price.ToString();

        // 랭크에 따라 색 변경
    }
}
