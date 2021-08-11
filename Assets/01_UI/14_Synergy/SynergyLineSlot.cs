using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TempSynergyData
{
    public int Code;
    public int Rank;
}

public class SynergyLineSlot : MonoBehaviour
{
    [SerializeField] List<SynergySlot> m_slots; // ������ ���� ���Ե�

    [Space(10)]
    [SerializeField] Image m_panel;             // 
    [SerializeField] Image m_extend_panel;      // Ȯ�� ���� (�� �гο��� �������� ���ϴ� ������)

    [SerializeField] int m_showCount;           // m_panel ������ ���� ���� (�������� �߰� ��ư���� Ȯ��)
    [SerializeField] bool IsShowExtendPanel;



    void Start()
    {
        // Ȯ�� â�� �ݱ�
        IsShowExtendPanel = false;
        m_extend_panel.gameObject.SetActive(false);
    }

    // Slot���� �Ʒ� ���ǿ� ���� �����ϱ�
    // 1���� : �ó��� ��ũ ��������
    // 2���� : �ó����� �ο� �� ???
    // 3���� : �ó��� �ڵ� ��������
    public void SortSlots()
    {
        // �ӽ�
        List<TempSynergyData> list = new List<TempSynergyData>();

        SortedSet<TempSynergyData> set = new SortedSet<TempSynergyData>();

        list.Sort((item1, item2) => {
            // 1���� ����
            int retval = item2.Rank.CompareTo(item1.Rank);
            if(retval == 0)
            {
                // 3���� ����
                retval = item1.Code.CompareTo(item2.Code);
            }
            return retval;
        });    
    }

    // synergy ������ ������Ʈ �� ���
    public void OnSynergyUpdate()
    {
        // list ������ ��������

        // sorting �ϰ�

        // ui �� ������Ʈ �ϱ�
        foreach (var item in m_slots)
        {
            item.SetInfo(new SynergySlotInfo());
        }
    }

    // Ȯ�� ��ư�� Ŭ�� ���� ���
    public void __OnExtednButtonClicked()
    {
        // Ȱ��ȭ �Ǿ� �ִ� ��� 
        if (IsShowExtendPanel)
        {   // Ȯ�� â�� �ݱ�
            m_extend_panel.gameObject.SetActive(false);
            IsShowExtendPanel = false;
        }
        else
        {   // Ȯ�� â�� ����
            m_extend_panel.gameObject.SetActive(true);
            IsShowExtendPanel = true;
        }
    }
}
