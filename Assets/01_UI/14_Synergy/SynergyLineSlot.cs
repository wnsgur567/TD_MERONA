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
    [SerializeField] Synergy_TableExcelLoader m_synergy_loader;
    
    [Space(20)]
    [SerializeField] SynergySlot m_slot_origin;
    [SerializeField] List<SynergySlot> m_slot_list; // 관리할 하위 슬롯들

    [Space(20)]
    [SerializeField] Image m_panel;             // 메인 슬롯
    private List<SynergySlot> m_main_slot_list;
    [SerializeField] Image m_extend_panel;      // 확장 슬롯 (위 패널에서 보여주지 못하는 나머지)
    private List<SynergySlot> m_extend_slot_list;

    [Space(20)]
    [SerializeField] int m_showCount;           // m_panel 보여줄 슬롯 개수 (나머지는 추가 버튼으로 확인)
    [SerializeField] int m_showMaxCount;
    [SerializeField] bool IsShowExtendPanel;


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
        // 확장 창을 닫기
        IsShowExtendPanel = false;
        m_extend_panel.gameObject.SetActive(false);
    }

    void __Initialize()
    {
        m_slot_origin.gameObject.SetActive(false);

        for (int i = 0; i < 10; i++)    // 10 변동 가능성 있음
        {
            SynergySlot newSlot = GameObject.Instantiate<SynergySlot>(m_slot_origin);
            m_slot_list.Add(newSlot);
            newSlot.gameObject.SetActive(true);
        }

        for (int i = 0; i < m_showCount; i++)
        {   // 앞 3개를 m_panel
            m_main_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_panel.transform);
        }
        for (int i = m_showCount; i < 10; i++)
        {   // 뒤 7개를 m_extend_panel
            m_extend_slot_list.Add(m_slot_list[i]);
            m_slot_list[i].transform.SetParent(m_extend_panel.transform);
        }

        var temp_data = m_synergy_loader.DataList[0];
        // temp
        foreach (var item in m_slot_list)
        {
            item.SetInfo(new SynergySlotInfo
            {
                isActivated = true,
                name = temp_data.Name_KR,
                synergy_text = temp_data.Synergy_text,
                synergy_ability = temp_data.Synergy_Avility,
                sprite_code = temp_data.Synergy_icon
            }) ;
        }
    }


    // Slot들을 아래 조건에 따라 정렬하기
    // 1순위 : 시너지 랭크 내림차순
    // 2순위 : 시너지의 인원 수 ???
    // 3순위 : 시너지 코드 오름차순
    public void SortSlots()
    {
        // 임시
        List<TempSynergyData> list = new List<TempSynergyData>();

        SortedSet<TempSynergyData> set = new SortedSet<TempSynergyData>();

        list.Sort((item1, item2) =>
        {
            // 1순위 정렬
            int retval = item2.Rank.CompareTo(item1.Rank);
            if (retval == 0)
            {
                // 3순위 정렬
                retval = item1.Code.CompareTo(item2.Code);
            }
            return retval;
        });
    }

    // synergy 정보가 업데이트 된 경우
    public void __OnSynergyUpdate()
    {
        // list 정보를 가져오고

        // sorting 하고

        // ui 를 업데이트 하기
        foreach (var item in m_slot_list)
        {
            SynergySlotInfo data = new SynergySlotInfo();

            //S_SynergyData_Excel data2 = a.GetData(data.Code,data.Rank);
            item.SetInfo(data);
        }
    }

    // 확장 버튼을 클릭 했을 경우
    public void __OnExtednButtonClicked()
    {
        

        // 활성화 되어 있는 경우 
        if (IsShowExtendPanel)
        {   // 확장 창을 닫기
            m_extend_panel.gameObject.SetActive(false);
            IsShowExtendPanel = false;
        }
        else
        {   // 확장 창을 열기
            m_extend_panel.gameObject.SetActive(true);
            IsShowExtendPanel = true;
        }
    }
}
