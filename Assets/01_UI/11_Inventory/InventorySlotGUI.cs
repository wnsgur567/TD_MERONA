using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[System.Serializable]
public struct InventorySlotGUIInfo
{
    public int index;
    public InventorySlot slot;
}

public class InventorySlotGUI : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] bool Show;
    [SerializeField] InventorySlotGUIInfo m_info;

    Image m_panel;
    RectTransform m_rt;

    private void Awake()
    {
        m_panel = this.GetComponent<Image>();
        m_rt = this.GetComponent<RectTransform>();
    }

    private void Start()
    {
    }


    // InvenSlot 간의 교환
    private void SwapInfo(InventorySlotGUI slotGUI)
    {
        // tower swap
        InventorySlot thisSlot = m_info.slot;
        var this_tower = thisSlot.GetTower();

        InventorySlot otherSlot = slotGUI.m_info.slot;
        thisSlot.SetTower(otherSlot.GetTower());

        otherSlot.SetTower(this_tower);

        // 두 인벤토리에 있는 tower의 position을 갱신
        thisSlot.UpdateTowerPosOnThisSlot();
        otherSlot.UpdateTowerPosOnThisSlot();
    }

    public void __Indexing(int index, InventorySlot slot)
    {
        m_info.index = index;
        m_info.slot = slot;
    }
    public void SetSize(float width, float height)
    {
        m_rt.sizeDelta = new Vector2(width, height);
    }

    public void OnDrag(PointerEventData eventData)
    {   
        if(eventData.IsPointerMoving() && m_info.slot.IsOccupied)
        {   // 마우스가 움직이고 있는 상태이고
            // &&
            // 해당 슬롯에 타워가 있는 경우에
            // => 해당 타워를 mouse position 에 맞게 이동시키기
            m_info.slot.SetTowerPos(eventData.position);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(Show)
        {   // for debugging
            m_panel.color = Color.blue;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(Show)
        {   // for debugging
            m_panel.color = Color.white;
        }

        RaycastResult target = eventData.pointerCurrentRaycast;
        if (target.gameObject != null && target.gameObject.tag.Equals("InvenSlot"))
        {   
            // 투 인벤토리간 타워 정보 교체
            SwapInfo(target.gameObject.GetComponent<InventorySlotGUI>());
        }
        else if(m_info.slot.IsOccupied)
        {
            m_info.slot.UpdateTowerPosOnThisSlot();
        }
    }
}
