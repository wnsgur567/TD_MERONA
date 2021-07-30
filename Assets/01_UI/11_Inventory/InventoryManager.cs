using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] Image m_root_panel;

    [SerializeField] InventorySlot m_origin;
    [SerializeField] int m_slotCount;
    [SerializeField] List<InventorySlot> m_slot_list;

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    public void __Initialize()
    {
        for (int i = 0; i < m_slotCount; i++)
        {
            InventorySlot newSlot = GameObject.Instantiate<InventorySlot>(m_origin);
            newSlot.__Indexing(i);
            m_slot_list.Add(newSlot);

            newSlot.transform.SetParent(m_root_panel.transform);
        }
    }

    
}
 