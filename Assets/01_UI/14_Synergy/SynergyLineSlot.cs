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
    [SerializeField] List<SynergySlot> m_slots; // 관리할 하위 슬롯들

    [Space(10)]
    [SerializeField] Image m_panel;             // 
    [SerializeField] Image m_extend_panel;      // 확장 슬롯 (위 패널에서 보여주지 못하는 나머지)

    [SerializeField] int m_showCount;           // m_panel 보여줄 슬롯 개수 (나머지는 추가 버튼으로 확인)
    [SerializeField] bool IsShowExtendPanel;



    void Start()
    {
        // 확장 창을 닫기
        IsShowExtendPanel = false;
        m_extend_panel.gameObject.SetActive(false);
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

        list.Sort((item1, item2) => {
            // 1순위 정렬
            int retval = item2.Rank.CompareTo(item1.Rank);
            if(retval == 0)
            {
                // 3순위 정렬
                retval = item1.Code.CompareTo(item2.Code);
            }
            return retval;
        });    
    }

    // synergy 정보가 업데이트 된 경우
    public void OnSynergyUpdate()
    {
        // list 정보를 가져오고

        // sorting 하고

        // ui 를 업데이트 하기
        foreach (var item in m_slots)
        {
            item.SetInfo(new SynergySlotInfo());
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
