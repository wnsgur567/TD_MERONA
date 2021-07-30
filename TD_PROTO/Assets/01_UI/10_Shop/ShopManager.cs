using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : Singleton<ShopManager>
{
    [SerializeField] Image m_root_panel;

    [SerializeField] ShopSlot m_origin;
    [SerializeField] int m_slotCount;
    [SerializeField] List<ShopSlot> m_slot_list;

    private void Awake()
    {
        __Initialize();
    }

    private void Start()
    {
        ShopReset();
    }

    public void __Initialize()
    {
        for (int i = 0; i < m_slotCount; i++)
        {
            ShopSlot newSlot = GameObject.Instantiate<ShopSlot>(m_origin);
            newSlot.__Indexing(i);
            m_slot_list.Add(newSlot);

            newSlot.transform.SetParent(m_root_panel.transform);
        }        
    }
    public void ShopReset()
    {
        
        
        
    }
    public void ShopLock()
    {

    }
    public void ShopUnLock()
    {

    }
}
