using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerToolTipManager : Singleton<TowerToolTipManager>
{
    [SerializeField] TowerToolTipUIController m_tooltip;

    
    private void Start()
    {
        DeActivateTooltip();
    }

    // TODO : tooltip position setting algorythm

    public void ActivateToolTip(Vector3 worldPos, Tower_TableExcel data)
    {
        m_tooltip.SetUIInfo(data);
        // TODO : camera world to screen pos
        m_tooltip.gameObject.SetActive(true);
    }
    public void ActivateToolTipOnUIClick(Vector2 mousePos, Tower_TableExcel data)
    {
        m_tooltip.SetUIInfo(data);
        m_tooltip.transform.position = mousePos;
        m_tooltip.gameObject.SetActive(true);
    }
    public void DeActivateTooltip()
    {        
        m_tooltip.gameObject.SetActive(false);
    }
}
