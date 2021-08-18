using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct TempSynergyData
{
    public int Code;
    public int Rank;
}

// synergy manager �κ��� �̺�Ʈ�� �޾� ���� UI �� ���
public class SynergyLineSlot : MonoBehaviour
{
    // ���� ������ ǥ���� ���� ��ġ
    E_Direction m_dir;

    [SerializeField] Synergy_TableExcelLoader m_synergy_loader;    

    [Space(20)]
    [SerializeField] SynergySlot m_slot_origin;
    [SerializeField] List<SynergySlot> m_slot_list; // ������ ���� ���Ե�

    [Space(20)]
    [SerializeField] Image m_panel;             // ���� ����
    private List<SynergySlot> m_main_slot_list;
    [SerializeField] Image m_extend_panel;      // Ȯ�� ���� (�� �гο��� �������� ���ϴ� ������)
    private List<SynergySlot> m_extend_slot_list;

    [Space(20)]
    [SerializeField] int m_showCount;           // m_panel ������ ���� ���� (�������� �߰� ��ư���� Ȯ��)
    [SerializeField] int m_showMaxCount;
    [SerializeField] bool IsShowExtendPanel;

    [Space(20)]
    [SerializeField] List<Synergy_TableExcel> m_synergy_list;   // data from synergy manager on this line

    private void Awake()
    {
        m_main_slot_list = new List<SynergySlot>();
        m_extend_slot_list = new List<SynergySlot>();

        if (m_showCount <= 0)
            m_showCount = 3;
        if (m_showMaxCount <= 0)
            m_showMaxCount = 10;

        __Initialize();
    }
    void Start()
    {
        // Ȯ�� â�� �ݱ�
        IsShowExtendPanel = false;
        m_extend_panel.gameObject.SetActive(false);

        // link event               
        SynergyManager.Instance.UpdateSynergyEndEvent += __OnSynergyUpdate;        
    }
       

    void __Initialize()
    {
        m_slot_origin.gameObject.SetActive(false);

        for (int i = 0; i < 10; i++)    // 10 ���� ���ɼ� ����
        {
            SynergySlot newSlot = GameObject.Instantiate<SynergySlot>(m_slot_origin);
            m_slot_list.Add(newSlot);
            newSlot.gameObject.SetActive(true);
        }

        for (int i = 0; i < m_showCount; i++)
        {   // �� 3���� m_panel
            m_main_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_panel.transform);
        }
        for (int i = m_showCount; i < 10; i++)
        {   // �� 7���� m_extend_panel
            m_extend_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_extend_panel.transform);
        }

        var temp_data = m_synergy_loader.DataList[0];
        // temp
        foreach (var item in m_slot_list)
        {
            item.SetInfo(new SynergySlotInfo
            {
                isActivated = false,
                name = temp_data.Name_KR,
                synergy_text = temp_data.Synergy_text,
                synergy_ability = temp_data.Synergy_Avility,
                sprite_code = temp_data.Synergy_icon
            }) ;
        }
    }
    public void __Indexing(int index)
    {
        m_dir = (E_Direction)index;
    }


    // Slot���� �Ʒ� ���ǿ� ���� �����ϱ�
    // 1���� : �ó��� ��ũ ��������
    // 2���� : �ó����� �ο� �� ???
    // 3���� : �ó��� �ڵ� ��������
    public void SortSynergyList()
    {
        // SortedSet<Synergy_TableExcel> set = new SortedSet<Synergy_TableExcel>();

        m_synergy_list.Sort((item1, item2) =>
        {
            // 1���� ����
            int retval = item2.Rank.CompareTo(item1.Rank);
            if (retval == 0)
            {
                // 3���� ����
                retval = item1.Code.CompareTo(item2.Code);
            }
            return retval;
        });
    }

    // synergy ������ ������Ʈ �� ���
    public void __OnSynergyUpdate()
    {
        Debug.Log("SynergyLineSlot : Event is Called");

        // 1. list ������ synergy manager �� ���� ��������
        m_synergy_list.Clear();
        var synergy_list = SynergyManager.Instance.GetSynergyList(m_dir);
        foreach (var item in synergy_list)
        {
            m_synergy_list.Add(item);
        }

        // 2. copy �� ����Ʈ m_synergy_list �� sorting �ϰ�
        SortSynergyList();

        // ui �� ������Ʈ �ϱ�
        for (int i = 0; i < m_synergy_list.Count; i++)
        {
            var cur_data = m_synergy_list[i];
            m_slot_list[i].SetInfo(new SynergySlotInfo()
            {
                isActivated = true,
                name = cur_data.Name_KR,
                synergy_text = cur_data.Synergy_text,
                synergy_ability = cur_data.Synergy_Avility,
                sprite_code = cur_data.Synergy_icon
            });
        }
        for (int i = m_synergy_list.Count; i < m_slot_list.Count; i++)
        {
            m_slot_list[i].SetInfo(new SynergySlotInfo()
            {
                isActivated = false
            });
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
