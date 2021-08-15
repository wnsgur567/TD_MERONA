using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public struct SynergySlotInfo
{
    public int index;
    public int Code;
    public int Rank;
    public string name;
    public Sprite icon_sprite;
   
    public string Synergy_text;
    public string Synergy_ability;
    // excel data...
    public Sprite image_sprite; 
}

public class SynergySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image m_synergy_image;

    [SerializeField] SynergySlotInfo m_info;
    
    Synergy_Tooltip S_Tooltip;
    private void Awake()
    {
        m_synergy_image = this.GetComponent<Image>();
        S_Tooltip = Synergy_Tooltip.Instance;
       
    }

    // info 가 업데이트 됫을 경우
    public void SetInfo(SynergySlotInfo info /*-> excel data*/)
    {
       // S_Tooltip.Set_Tooltip(info.Name_KR, info.Synergy_text, info.Synergy_Avility, m_synergy_image);
        OnInfoChanged();
    }

    private void OnInfoChanged()
    {
        m_synergy_image.sprite = m_info.image_sprite;
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector2 pos = Input.mousePosition;

        S_Tooltip.Set_Tooltip_Pos(pos);
        S_Tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        S_Tooltip.gameObject.SetActive(false);
    }
}
