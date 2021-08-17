using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerToolTipUIController : MonoBehaviour
{    
    [SerializeField] TMPro.TextMeshProUGUI m_name_textpro;
    [Space(10)]
    [SerializeField] ToolTipTowerImage m_image;
    [Space(10)]
    [SerializeField] ToolTipLeftSynergy m_synergyline1;
    [SerializeField] ToolTipLeftSynergy m_synergyline2;
    [Space(10)]
    [SerializeField] ToolTipRightDataLine m_stat_data1;
    [SerializeField] ToolTipRightDataLine m_stat_data2;
    [SerializeField] ToolTipRightDataLine m_stat_data3;
    [SerializeField] ToolTipRightDataLine m_stat_data4;
    [Space(10)]
    [SerializeField] ToolTipRightSkill m_skill;
    

    [Space(20)]
    [SerializeField] Synergy_TableExcelLoader m_synergyLoader;  // for synergy info
    [SerializeField] SkillCondition_TableExcelLoader m_skill_loader; // for skill info overall
    [SerializeField] SkillStat_TableExcelLoader m_skillstat_loader; // for atk speed

    public void SetUIInfo(Tower_TableExcel data)
    {
        // name
        m_name_textpro.name = data.Name_KR;
        // raw image (render texture)
        m_image.SetUI(data.Code);

        // synergy
        int synergy1Code = data.Type1;
        int synergy2Code = data.Type2;

        var synergy1_data = m_synergyLoader.DataList.
            Find((item) => { return item.Code == synergy1Code; }); 
        var synergy2_data = m_synergyLoader.DataList.
            Find((item) => { return item.Code == synergy2Code; });
        m_synergyline1.SetUI(synergy1_data.Synergy_icon, synergy1_data.Name_KR);
        m_synergyline2.SetUI(synergy2_data.Synergy_icon, synergy1_data.Name_KR);


        // tower data (atk critical etc)
        // -atk
        m_stat_data1.SetUI("공격력", data.Atk.ToString());
        // -atk speed
        float atk_speed = m_skillstat_loader.DataList.Find((item) => { return item.Code == data.Atk_Code; }).Speed;
        m_stat_data2.SetUI(
            "공격속도",
            ((int)(1f / atk_speed * 100)).ToString() + "%");
        // -critical rate
        m_stat_data3.SetUI("치명타 확률", (data.Crit_rate * 100).ToString() + "%");
        // -critical damage
        m_stat_data4.SetUI("치명타 피해", (data.Crit_Dmg * 100).ToString() + "%");

        // skill
        var skill_data = m_skill_loader.DataList.Find((item) =>
        {
            return item.Code == data.Skill1Code;
        });

        m_skill.SetUI(skill_data.Skill_icon, skill_data.Name_KR);
    }

    public void SetPosition(Vector2 mousePos)
    {
        this.transform.position = mousePos;
    }

    private void Update()
    {
        
    }
}
