using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// mouse input �� ���� gui ��
// ���� ���͸� ��ġ��ų slot �� object �� ���
public class InventoryManager : Singleton<InventoryManager>
{
    // gui
    [Space(10)]
    [SerializeField] Image m_root_panel;    // �� Panel �� ������ ��������
    CellSizeFitter m_root_sizeFitter;       // ���� slot count �� ���� size �� �����Ѵ�
    [SerializeField] InventorySlotGUI m_originGUI;

    // ���� list
    [Space(10)]
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
        m_originGUI.gameObject.SetActive(false);

        for (int i = 0; i < _cellsize.x; i++)
        {
            // gui instantiate
            InventorySlotGUI newSlotGUI = GameObject.Instantiate<InventorySlotGUI>(m_originGUI);
            newSlotGUI.__Indexing(i);
            m_slotGUI_list.Add(newSlotGUI);
            newSlotGUI.gameObject.SetActive(true);
            newSlotGUI.transform.SetParent(m_root_panel.transform);
        }
    }



    // ���� ����ִ� ���� ����
    InventorySlotGUI GetAvailableSlot()
    {
        foreach (var item in m_slotGUI_list)
        {
            if (false == item.IsOccupied)
                return item;
        }
        return null;
    }

    public bool IsAllOccupied()
    {
        InventorySlotGUI slot = GetAvailableSlot();
        if (null == slot)
            return true;
        return false;
    }

    public void AddNewTower(Tower_TableExcel data)
    {
        InventorySlotGUI slot = GetAvailableSlot();
        if (null == slot)
            return;

        Debug.Log(data.No);
        Tower newTower = TowerManager.Instance.SpawnTower((E_Tower)data.No);
        newTower.gameObject.SetActive(false);
        slot.SetTower(newTower,data);
    }
    public void RemoveTower(Tower tower)
    {

    }
}
 