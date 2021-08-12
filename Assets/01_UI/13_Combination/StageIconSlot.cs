using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageIconSlot : MonoBehaviour
{
    private Image m_image;
    private void Awake()
    {
        m_image = this.GetComponent<Image>();
    }

    public void ChnageImage(Sprite sprite)
    {
        m_image.sprite = sprite;
    }
}
