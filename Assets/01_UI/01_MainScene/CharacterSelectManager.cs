using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectManager : Singleton<CharacterSelectManager>
{
    [SerializeField] Prefab_TableExcelLoader m_prefab_loader;
    [SerializeField] Tower_TableExcelLoader m_tower_loader;
    [SerializeField] SkillCondition_TableExcelLoader m_skill_loader;

    [Space(10)]
    [SerializeField] RawImage m_character_image;
    [SerializeField] Vector3 camera_distance;           // ������Ʈ�κ����� �Ÿ�
    [SerializeField] Vector3 camera_rotation;           // ī�޶� ȸ�� ��
    [SerializeField] Vector3 m_obj_position;            // ���� ���� ������ �����Ұ�
    [SerializeField] List<CKeyValue> m_showObj_list;    // �Ʒ��� ���� �ִ� ������Ʈ ����Ʈ
    GameObject m_showObj;   // ���� �����ְ� �ִ� ������Ʈ
    RenderTexture m_renderTexture;
    Camera m_renderCamera;

    [Space(10)]
    [SerializeField] List<Tower_TableExcel> m_character_dataList;
    Tower_TableExcel m_current_data;

    [Space(10)]
    [SerializeField] SkillInfoSlotController m_skillslot_controll;
    [SerializeField] CharacterInfoController m_charslot_controll;

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            m_character_dataList.Add(m_tower_loader.DataList[i]);
        }

        // ù��° �������� �⺻ ���� �ص�
        UserInfoManager.Instance.SetDevilCode(m_character_dataList[0].Code);
        m_current_data = m_character_dataList[0];

        // ���� �ؽ��� ���� ���ҽ� �Ҵ��ϱ�
        InitializeRenderTexture();
        // ù��° ���� ������Ʈ Ȱ��ȭ (���� �ؽ��� ��)
        m_showObj = m_showObj_list[0].obj.gameObject;
        m_showObj.SetActive(true);

        OnCharacterChanged();
    }

    public void OnStart()
    {
        OnCharacterChanged();
    }

    public void OnCharacterChanged()
    {
        SetSkillInfo();
        SetNameInfo();
    }

    public void InitializeRenderTexture()
    {
        int layer = LayerMask.NameToLayer("MainSceneCharUI");
        Debug.Log(layer);

        m_renderTexture = new RenderTexture(256, 256, 16);
        m_renderTexture.Create();
        

        Camera cam_origin = Resources.Load<Camera>("MainSceneCamera");
        m_renderCamera = GameObject.Instantiate<Camera>(cam_origin);
        m_renderCamera.cullingMask = 1 << layer;
        m_renderCamera.clearFlags = CameraClearFlags.SolidColor;
        m_renderCamera.backgroundColor = new Color(0, 0, 0, 0);

        m_renderCamera.targetTexture = m_renderTexture;
        m_renderCamera.transform.position = m_obj_position + camera_distance;
        m_renderCamera.transform.eulerAngles = camera_rotation;
        

        foreach (var item in m_character_dataList)
        {
            GameObject origin_obj = m_prefab_loader.GetPrefab(item.Prefab);
            GameObject new_obj = GameObject.Instantiate(origin_obj);

            Transform[] allChildren = new_obj.GetComponentsInChildren<Transform>(true);
            foreach (var child in allChildren)
            {
                child.gameObject.layer = layer;
            }
            

            CKeyValue val = new CKeyValue
            { Code = m_prefab_loader.DataList[0].Code, obj = new_obj };
            m_showObj_list.Add(val);
        }


        foreach (var item in m_showObj_list)
        {   // ���� ���� ���·� ���� ��ġ�� ����            
            item.obj.transform.position = m_obj_position;
            item.obj.gameObject.SetActive(false);
        }

        m_character_image.texture = m_renderTexture;
    }

    public void SetRenderTexture(int index)
    {
        m_showObj.SetActive(false);
        m_showObj = m_showObj_list[index].obj.gameObject;
        m_showObj.SetActive(true);
    }

    public void SetSkillInfo()
    {
        var skill1_data = m_skill_loader.DataList.Find((item) => { return item.Code == m_current_data.Skill1Code; });
        var skill2_data = m_skill_loader.DataList.Find((item) => { return item.Code == m_current_data.Skill2Code; });
        List<SkillCondition_TableExcel> data = new List<SkillCondition_TableExcel>();
        data.Add(skill1_data);
        data.Add(skill2_data);

        m_skillslot_controll.SetInfos(data);
    }
    public void SetNameInfo()
    {
        m_charslot_controll.Set(m_current_data.Name_KR, "Information");
    }

    public void __OnSelectButton(int index)
    {
        SetRenderTexture(index);
        m_current_data = m_character_dataList[index];
        UserInfoManager.Instance.SetDevilCode(m_current_data.Code);
        OnCharacterChanged();
    }

}
