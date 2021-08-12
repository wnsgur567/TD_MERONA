using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StagePrivewController : MonoBehaviour
{
    private CellSizeFitter m_fitter;
    [SerializeField] Image m_root_panel;
    [SerializeField] StageIconSlot m_origin_slot;
    [SerializeField] List<StageIconSlot> m_slots;

    private void Awake()
    {
        m_fitter = this.GetComponent<CellSizeFitter>();
    }

    private void Start()
    {
        __Initailize();
    }

    private void __Initailize()
    {
        int cellCount = m_fitter.CellCount.x;

        // stage manager �� ���� ������ ��������

        m_origin_slot.gameObject.SetActive(false);
        for (int i = 0; i < cellCount; i++)
        {
            StageIconSlot newSlot = GameObject.Instantiate<StageIconSlot>(m_origin_slot);

            // TODO : manager ���� ������ ������ slot �� �ʱ�ȭ �ϱ�

            newSlot.gameObject.SetActive(true);
            newSlot.transform.SetParent(m_root_panel.transform);
            m_slots.Add(newSlot);
        }
    }
}
