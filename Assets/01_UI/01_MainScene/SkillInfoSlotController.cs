using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfoSlotController : MonoBehaviour
{
    [SerializeField] List<SkillInfoSlot> m_slots;

    private void Awake()
    {
        var slots = this.transform.GetComponentsInChildren<SkillInfoSlot>();
        foreach (var item in slots)
        {
            m_slots.Add(item);
        }
    }

    public void SetInfos(List<SkillCondition_TableExcel> data)
    {
        for (int i = 0; i < m_slots.Count; i++)
        {
            // TODO : skill icon sprite �ҷ�����
            m_slots[i].Set(null, data[i].Skill_text);
        }
    }
}
