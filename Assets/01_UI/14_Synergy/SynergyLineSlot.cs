using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;



// synergy manager �κ��� �̺�Ʈ�� �޾� ���� UI �� ���
public class SynergyLineSlot : MonoBehaviour
{
    private class SynergyData : System.IEquatable<SynergyData>
    {
        public int Code;
        public Synergy_TableExcel cur_data;
        public List<Synergy_TableExcel> data_list;
        public bool IsActivated;
        public bool Equals(SynergyData other)
        {
            return Code == other.Code;
        }

    }

    // ���� ������ ǥ���� ���� ��ġ
    E_Direction m_dir;

    [SerializeField] Synergy_TableExcelLoader m_synergy_loader;
    [Space(20)]
    [SerializeField] TMPro.TextMeshProUGUI m_lineTextpro;
    [Space(20)]
    [SerializeField] SynergySlot m_slot_origin;
    [SerializeField] List<SynergySlot> m_slot_list; // ������ ���� ���Ե�

    [Space(20)]
    [SerializeField] Image m_panel;             // ���� ����
    private List<SynergySlot> m_main_slot_list;
    [SerializeField] Image m_extend_root_panel;
    [SerializeField] Image m_extend_panel;      // Ȯ�� ���� (�� �гο��� �������� ���ϴ� ������)
    private List<SynergySlot> m_extend_slot_list;

    [Space(20)]
    [SerializeField] int m_showCount;           // m_panel ������ ���� ���� (�������� �߰� ��ư���� Ȯ��)
    [SerializeField] int m_showMaxCount;
    [SerializeField] bool IsShowExtendPanel;

    [Space(20)]
    [SerializeField] List<SynergyData> m_synergy_list;

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
        m_extend_root_panel.gameObject.SetActive(false);

        // link event               
        SynergyManager.Instance.UpdateSynergyEndEvent += __OnSynergyUpdated;
    }


    void __Initialize()
    {
        m_slot_origin.gameObject.SetActive(false);

        m_synergy_list = new List<SynergyData>();
        foreach (var item in m_synergy_loader.DataList)
        {
            var data = m_synergy_list.Find((inner) => { return item.Code == inner.Code; });
            if (data == null)
            {   // new code
                data = new SynergyData()
                {
                    Code = item.Code,
                    cur_data = item,
                    data_list = new List<Synergy_TableExcel>(),
                    IsActivated = false
                };
                m_synergy_list.Add(data);
            }
            data.data_list.Add(item);
        }
        int total_synergy_count = m_synergy_list.Count;

        // create slots as much as total count
        for (int i = 0; i < total_synergy_count; i++)
        {
            SynergySlot newSlot = GameObject.Instantiate<SynergySlot>(m_slot_origin);
            m_slot_list.Add(newSlot);
            newSlot.gameObject.SetActive(true);
        }

        for (int i = 0; i < m_showCount; i++)
        {   // add slots as much as show count
            m_main_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_panel.transform);
        }
        for (int i = m_showCount; i < total_synergy_count; i++)
        {   // add slots the others
            m_extend_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_extend_panel.transform);
        }

        // ��� �ó��� ������ slot �� ����
        // ��� ��Ȱ��ȭ ����
        for (int i = 0; i < total_synergy_count; i++)
        {
            var data = m_synergy_list[i].data_list[0];  // rank 1

            m_slot_list[i].SetInfo(new SynergySlotInfo
            {
                isActivated = false,
                name = data.Name_KR,
                synergy_text = data.Synergy_text,
                synergy_ability = data.Synergy_Avility,
                sprite_code = data.Synergy_icon
            });
        }
        UIUpdate();
    }
    public void __Indexing(int index)
    {
        m_dir = (E_Direction)index;
        m_lineTextpro.text = m_dir.ToString().ToUpper();
    }

    // synergy ������ ������Ʈ �� ���
    public void __OnSynergyUpdated()
    {
        Debug.Log("SynergyLineSlot : Event is Called");

        // get activated synergy from manager
        var synergy_list = SynergyManager.Instance.GetSynergyList(m_dir);

        // data flush
        for (int i = 0; i < m_synergy_list.Count; i++)
        {
            var tmp_data = m_synergy_list[i];
            tmp_data.IsActivated = false;
            m_synergy_list[i] = tmp_data;
        }

        // current in synergy list's data update
        foreach (var item in synergy_list)
        {
            int index = m_synergy_list.FindIndex((inner) => { return item.Code == inner.Code; });


            // struct
            var tmp_data = m_synergy_list[index];
            tmp_data.cur_data =
                m_synergy_list[index].data_list.Find((inner_2) => { return inner_2.Rank == item.Rank; });
            tmp_data.IsActivated = true;
            m_synergy_list[index] = tmp_data;
        }

        // sorting
        //�ó��� ��ũ �������� �� (���� ���� ����)
        //�ó����� �ο� �� / ���� x
        //�ó��� �ڵ� �������� �� (���� ���� ����)
        m_synergy_list = m_synergy_list.            
            OrderBy(item => item.Code).
            OrderByDescending((item) => item.cur_data.Rank).
            OrderByDescending(item => item.IsActivated).
            ToList();
        //var query = from data in m_synergy_list
        //            where synergy_list.Find((item) => { return item.Code == data.Code; }).Code != 0 // not default
        //            orderby data.cur_data.Rank descending
        //            orderby data.cur_data.Code
        //            select ;                


        // sort complete

        // ui update
        UIUpdate();
    }

    void UIUpdate()
    {
        // synergy ui update
        for (int i = 0; i < m_synergy_list.Count; i++)
        {
            var cur_data = m_synergy_list[i];
            m_slot_list[i].SetInfo(new SynergySlotInfo()
            {
                isActivated = cur_data.IsActivated,
                name = cur_data.cur_data.Name_KR,
                synergy_text = cur_data.cur_data.Synergy_text,
                synergy_ability = cur_data.cur_data.Synergy_Avility,
                sprite_code = cur_data.cur_data.Synergy_icon
            });
        }
    }

    [Space(30)]
    [SerializeField] Image m_synergy_root_paenl;
    float margin = 30.0f;

    // Ȯ�� ��ư�� Ŭ�� ���� ���
    public void __OnExtednButtonClicked()  
    {
        // Ȱ��ȭ �Ǿ� �ִ� ��� 
        if (IsShowExtendPanel)
        {   // Ȯ�� â�� �ݱ�
            DeActivateExtendPanel();
        }
        else
        {            
            // ���ο� ���� �г� ��ġ�� ����
            switch (m_dir)
            {
                case E_Direction.North:                    
                case E_Direction.East:
                    m_extend_root_panel.transform.position =
                        m_synergy_root_paenl.transform.position;

                    m_extend_root_panel.rectTransform.anchoredPosition +=
                        new Vector2(m_extend_root_panel.rectTransform.sizeDelta.x + margin, 0);
                       
                    break;

                case E_Direction.South:                    
                case E_Direction.West:
                    m_extend_root_panel.transform.position =
                        m_synergy_root_paenl.transform.position;


                    m_extend_root_panel.rectTransform.anchoredPosition +=
                        new Vector2(m_extend_root_panel.rectTransform.sizeDelta.x + margin, 0);

                    break;
            }

            // Ȯ�� â�� ����
            m_extend_root_panel.gameObject.SetActive(true);
            IsShowExtendPanel = true;
        }
    }

    public void DeActivateExtendPanel()
    {
        m_extend_root_panel.gameObject.SetActive(false);
        IsShowExtendPanel = false;
    }
}
