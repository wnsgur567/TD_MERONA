using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynergyUIManager : Singleton<SynergyUIManager>
{
    [SerializeField] Image m_root_panel;
    CellSizeFitter m_fitter;

    [SerializeField] SynergyLineSlot m_origin;
    [SerializeField] List<SynergyLineSlot> m_lineSlots;

    private void Awake()
    {
        m_fitter = m_root_panel.GetComponent<CellSizeFitter>();
    }

    private void Start()
    {
        __Initialize();
    }

    void __Initialize()
    {
        int cellcount = m_fitter.CellCount.y;

        m_origin.gameObject.SetActive(false);

        for (int i = 0; i < cellcount; i++)
        {
            var newSlot = GameObject.Instantiate<SynergyLineSlot>(m_origin);

            // TODO : ...
            newSlot.gameObject.SetActive(true);
            newSlot.transform.SetParent(m_root_panel.transform);
            m_lineSlots.Add(newSlot);
        }
    }

}
