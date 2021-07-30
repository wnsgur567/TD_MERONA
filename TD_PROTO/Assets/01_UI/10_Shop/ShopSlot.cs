using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public struct ShopSlotInfo
{
    public int index;

    public string name;
    public int rank;
    public string type;
    public int price;
}

// Shop slot 에 관한 정보를 담는 클래스
public class ShopSlot : MonoBehaviour
{
    delegate void OnInfoChangedHandler();
    OnInfoChangedHandler OnInfoChangedCallback;
    [SerializeField] bool isOccupied;
    [SerializeField] ShopSlotInfo m_info;

    [SerializeField] TextMeshProUGUI m_nameText;
    [SerializeField] TextMeshProUGUI m_typeText;
    [SerializeField] TextMeshProUGUI m_goldText;
    [SerializeField] Image m_towerImage;
    [SerializeField] Image m_starImage;
    Dictionary<int, Color> m_rankToColor_dic;

    Outline[] m_outlines = null;   

    private void Awake()
    {
        m_outlines = this.transform.GetComponentsInChildren<Outline>();
        m_rankToColor_dic = new Dictionary<int, Color>();
        OnInfoChangedCallback += OnInfoChanged;
    }

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
    public ShopSlotInfo GetInfo()
    {
        return m_info;
    }
    public void SetOutlineColor(Color color)
    {
        foreach (var item in m_outlines)
        {
            item.effectColor = color;
        }        
    }

    void OnInfoChanged()
    {
        m_nameText.text = m_info.name;
        m_typeText.text = m_info.type;
        m_goldText.text = m_info.price.ToString();

        // 랭크에 따라 색 변경
    }
}
