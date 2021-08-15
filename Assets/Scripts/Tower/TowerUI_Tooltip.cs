using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
public struct Tower_Data
{
    public float atk;
    public float hp;
    public float def;
    public float cooltime;//공격속도 빼오기.
    public float crit_rate;
    public float crit_dmg;

  
    //시너지 정보
    public string synergyname;
   

    public int price;
};
public struct Skill_Date
{
    public string skillname;
    public string skilltext; 
    public float skilldmg;
    public string skillavility1;
    public string skillavility_text1;
    public bool isskillavility;
    public string skillavility2;
    public string skillavility_text2;
}
public class TowerUI_Tooltip : Singleton<TowerUI_Tooltip>, IPointerClickHandler
{

    public TextMeshProUGUI Sale_Price;
    public TextMeshProUGUI Tower_Text;
    public TextMeshProUGUI Stats_Number;
    public TextMeshProUGUI Skill_Name1;
    public TextMeshProUGUI Skill_Name2;
    public TextMeshProUGUI Synergy_name1;
    public TextMeshProUGUI Synergy_name2;
    public TextMeshProUGUI Boss_stat_text;
    public TextMeshProUGUI Boss_name_text;
    public TextMeshProUGUI Boss_text;

    int Tower_Star;
    private Image Synergy_icon1;
    private Image Synergy_icon2;
    private Image Tower_Icon;
    private Image Skill_Icon1;
    private Image Skill_Icon2;
    private RectTransform Tower_TT_pos;

    Tower_Data towerdata;
    Skill_Date skilldata1,skilldata2;

    SkillManager skill;
    SkillCondition_TableExcel skillcondition_excel;
    SkillStat_TableExcel skillstate_excel;

    SynergyManager synergy;
    Synergy_TableExcel synergy_excel;

    Transform saleobj;
    float screen_maxy;
    float screen_maxx;
    //스킬아이콘 raycast
    public Canvas maincanvas;
    GraphicRaycaster raycast;
    PointerEventData pos;
    SkillTooltip skilltooltip;
    bool towertooltip_active;
   
    private void Awake()
    {
        skill = SkillManager.Instance;
        synergy = SynergyManager.Instance;
        skilltooltip = SkillTooltip.Instance;
        towerdata = new Tower_Data();
       
        Transform temp = this.transform.Find("TowerUI_Stats/Skill_Icon1");
        Skill_Icon1 = temp.gameObject.GetComponent<Image>();
        temp = this.transform.Find("TowerUI_Stats/Skill_Icon2");
        Skill_Icon2 = temp.gameObject.GetComponent<Image>();
        temp = this.transform.Find("TowerUI_Tower/Synergy_icon_1");
        Synergy_icon1 = temp.gameObject.GetComponent<Image>();

        temp = this.transform.Find("TowerUI_Tower/Synergy_icon_2");
        Synergy_icon2 = temp.gameObject.GetComponent<Image>();

        temp = this.transform.Find("TowerUI_Icon");
        Tower_Icon = temp.gameObject.GetComponent<Image>();
        
        Tower_TT_pos = this.gameObject.GetComponent<RectTransform>();
        screen_maxy = Screen.height;
        screen_maxx = Screen.width;

        Boss_stat_text.gameObject.SetActive(false);
        Boss_name_text.gameObject.SetActive(false);
        Boss_text.gameObject.SetActive(false);

        saleobj = this.transform.Find("TowerUI_Sale");

        raycast = maincanvas.GetComponent<GraphicRaycaster>();
        pos = new PointerEventData(null);
        towertooltip_active = false;
        skilldata1.isskillavility = false;
        skilldata2.isskillavility = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
       
    }
    public void OnMouse(Tower_TableExcel m_info)
    {
        
            Vector2 mousepos = Input.mousePosition;
            this.Set_TowerTT_Pos(mousepos);
            this.Set_TowerTT(m_info);
       
    }
    public void Set_TowerTT(Tower_TableExcel info)
    {
        Tower_Star = info.Star;
        Sale_Price.text = info.Price.ToString();
       
       
        #region 타워스탯
        towerdata.atk = info.Atk;
        towerdata.hp = info.HP;
        towerdata.def = info.Def;
        towerdata.crit_rate = info.Crit_rate;
        towerdata.crit_dmg = info.Crit_Dmg;

        //일반 쿨타임.
        skillcondition_excel = skill.GetConditionData(info.Atk_Code);
        skillstate_excel = skill.GetStatData(skillcondition_excel.PassiveCode);
        towerdata.cooltime = skillstate_excel.CoolTime;
        if(info.Rank==10)
        {   Boss_name_text.text = info.Name_KR;
            Tower_Text.gameObject.SetActive(false);
            Synergy_icon1.gameObject.SetActive(false);
            Synergy_icon2.gameObject.SetActive(false);
            Synergy_name1.gameObject.SetActive(false);
            Synergy_name2.gameObject.SetActive(false);
            saleobj.gameObject.SetActive(false);
            Skill_Icon2.gameObject.SetActive(true);
            Skill_Name2.gameObject.SetActive(true);
            Boss_name_text.gameObject.SetActive(true);
            Boss_text.gameObject.SetActive(true);
            Boss_stat_text.gameObject.SetActive(true);
            Stats_Number.text = String.Format("{0}\n{1}\n{2}\n{3}\n{4}\n{5}",
            towerdata.atk, towerdata.cooltime, towerdata.crit_rate, towerdata.crit_dmg, towerdata.hp, towerdata.def);
        }
        else 
        {   
            Tower_Text.text = info.Name_KR;
            Tower_Text.gameObject.SetActive(true);
            Synergy_icon1.gameObject.SetActive(true);
            Synergy_icon2.gameObject.SetActive(true);
            Synergy_name1.gameObject.SetActive(true);
            Synergy_name2.gameObject.SetActive(true);
            saleobj.gameObject.SetActive(true);
            Skill_Icon2.gameObject.SetActive(false);
            Skill_Name2.gameObject.SetActive(false);
            Boss_name_text.gameObject.SetActive(false);
            Boss_text.gameObject.SetActive(false);
            Boss_stat_text.gameObject.SetActive(false);
            Stats_Number.text = String.Format("{0}\n{1}\n{2}\n{3}",
            towerdata.atk, towerdata.cooltime, towerdata.crit_rate, towerdata.crit_dmg);
        }
        #endregion

        #region 스킬
        //스킬 1정보

        skillcondition_excel = skill.GetConditionData(info.Skill1Code);
        skillstate_excel = skill.GetStatData(skillcondition_excel.PassiveCode);
        Skill_Name1.text = skillstate_excel.Name_KR;
        skilldata1.skillname= skillstate_excel.Name_KR;
        skilldata1.skilltext = skillcondition_excel.Skill_text;
        skilldata1.skilldmg = skillstate_excel.Dmg;
        #region 스킬1의 어빌리티 데이터 저장.
       
        //skilldata1.skillavility1=skillcondition_excel.SkillAvility_Name1;
        //skilldata1.skillavility_text1=skillcondition_excel.SkillAvility_Text1;
        //skilldata1.skillavility2=skillcondition_excel.SkillAvility_Name2;
        //skilldata1.skillavility_text2=skillcondition_excel.SkillAvility_Text2;
        //if(skilldata1.skillavility=="-")
        //{
        //skilldata1.isskillavility = true;
        //}
        //else skilldata1.isskillavility=false;
        #endregion



        //스킬 2 정보
        skillcondition_excel = skill.GetConditionData(info.Skill2Code);
        skillstate_excel = skill.GetStatData(skillcondition_excel.PassiveCode);
        Skill_Name2.text = skillstate_excel.Name_KR;

        skilldata2.skillname = skillstate_excel.Name_KR;
        skilldata2.skilltext = skillcondition_excel.Skill_text;
        skilldata2.skilldmg = skillstate_excel.Dmg;

        #region 스킬2의 어빌리티 데이터 저장
        //skilldata2.skillavility1=skillcondition_excel.SkillAvility_Name1;
        //skilldata2.skillavility_text1=skillcondition_excel.SkillAvility_Text1;
        //skilldata2.skillavility2=skillcondition_excel.SkillAvility_Name2;
        //skilldata2.skillavility_text2=skillcondition_excel.SkillAvility_Text2;
        //if(skilldata2.skillavility=="-")
        //{
        //skilldata2.isskillavility = true;
        //}
        //else skilldata2.isskillavility=false;
        #endregion

        #endregion

        #region 시너지정보
        //시너지 1
        synergy_excel = synergy.GetData(info.Type1);
        Synergy_name1.text = synergy_excel.Name_KR;
       

        //시너지 2
        synergy_excel = synergy.GetData(info.Type2);
        Synergy_name2.text = synergy_excel.Name_KR;
        #endregion
       
       
    }
    public void Set_TowerTT_Pos(Vector2 pos)
    {
        float tempposx = pos.x;
        float tempposy = pos.y;

        if (pos.x + Tower_TT_pos.rect.width > screen_maxx)
        {
            tempposx -= Tower_TT_pos.rect.width;
        }
        if (pos.y - Tower_TT_pos.rect.height < 0)
        {
            tempposy += Tower_TT_pos.rect.height;
        }

        Vector2 pos2 = new Vector2(tempposx, tempposy);
        Tower_TT_pos.position = pos2;
        this.gameObject.SetActive(true);
        towertooltip_active = true;
    }
    // Update is called once per frame
    void Update()
    {
        #region 스킬아이콘에 마우스올리면
        if (towertooltip_active)
        {   pos.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            raycast.Raycast(pos, results);
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.CompareTag("Skill_Icon"))
                {
                    if (results[i].gameObject.name == "Skill_Icon1")
                        skilltooltip.Set_Skill_TT(skilldata1);
                    else
                        skilltooltip.Set_Skill_TT(skilldata2);

                    skilltooltip.Set_Skill_TT_pos(pos.position);
                    skilltooltip.gameObject.SetActive(true);
                    break;
                }
                else
                {
                    skilltooltip.gameObject.SetActive(false);
                }
            } 
        }
        #endregion
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        this.gameObject.SetActive(false);
        towertooltip_active = false;
    }
}
