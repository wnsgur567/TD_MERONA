using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// mouse input �� ���� gui ��
// ���� ���͸� ��ġ��ų slot �� object �� ���
public class InventoryManager : Singleton<InventoryManager>
{
    // objects
    [SerializeField] GameObject m_root_object;
    [SerializeField] InventorySlot m_origin;

    // gui
    [Space(10)]
    [SerializeField] Image m_root_panel;    // �� Panel �� ������ ��������
    CellSizeFitter m_root_sizeFitter;       // ���� slot count �� ���� size �� �����Ѵ�
    [SerializeField] InventorySlotGUI m_originGUI;

    // ���� list
    [Space(10)]
    [SerializeField] List<InventorySlot> m_slot_list;
    [SerializeField] List<InventorySlotGUI> m_slotGUI_list;      // Ŭ��

    private void Awake()
    {
        m_root_sizeFitter = m_root_panel.GetComponent<CellSizeFitter>();
    }

    private void Start()
    {
        __Initialize();
    }

    public void __Initialize()
    {
        Vector2Int _cellsize = m_root_sizeFitter.CellCount;
        
        // origin ����
        m_origin.gameObject.SetActive(false);
        m_originGUI.gameObject.SetActive(false);

        for (int i = 0; i < _cellsize.x; i++)
        {
            // object instatiate
            InventorySlot newSlot = GameObject.Instantiate<InventorySlot>(m_origin);
            newSlot.__Indexing(i);
            m_slot_list.Add(newSlot);
            newSlot.gameObject.SetActive(true);
            newSlot.transform.SetParent(m_root_object.transform);

            // gui instantiate
            InventorySlotGUI newSlotGUI = GameObject.Instantiate<InventorySlotGUI>(m_originGUI);
            newSlotGUI.__Indexing(i, newSlot);
            m_slotGUI_list.Add(newSlotGUI);
            newSlotGUI.gameObject.SetActive(true);
            newSlotGUI.transform.SetParent(m_root_panel.transform);
        }
    }



    // ���� ����ִ� ���� ����
    InventorySlot GetAvailableSlot()
    {
        foreach (var item in m_slot_list)
        {
            if (false == item.IsOccupied)
                return item;
        }
        return null;
    }

    public bool IsAllOccupied()
    {
        InventorySlot slot = GetAvailableSlot();
        if (null == slot)
            return true;
        return false;
    }

    public void AddNewTower(Tower tower)
    {
        InventorySlot slot = GetAvailableSlot();
        if (null == slot)
            return;

        slot.SetTower(tower);
    }
    
}
 