using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct SynergySlotInfo
{
    public int index;

    // excel data...
    public Sprite image_sprite;
}

public class SynergySlot : MonoBehaviour
{
    [SerializeField] Image m_synergy_image;
    [SerializeField] SynergySlotInfo m_info;

    private void Awake()
    {
        m_synergy_image = this.GetComponent<Image>();
    }

    // info 가 업데이트 됫을 경우
    public void SetInfo(SynergySlotInfo info /*-> excel data*/)
    {

        OnInfoChanged();
    }

    private void OnInfoChanged()
    {
        m_synergy_image.sprite = m_info.image_sprite;
    }
}
