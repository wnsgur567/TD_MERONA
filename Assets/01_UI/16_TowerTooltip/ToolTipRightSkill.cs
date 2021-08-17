using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipRightSkill : MonoBehaviour
{
    [SerializeField] Sprite_TableExcelLoader m_spriteLoader;
    [SerializeField] Image m_skill_image;
    [SerializeField] TMPro.TextMeshProUGUI m_skill_text;

    public void SetUI(int sprite_code, string text)
    {
        m_skill_image.sprite = m_spriteLoader.GetSprite(sprite_code);
        m_skill_text.text = text;
    }
}
