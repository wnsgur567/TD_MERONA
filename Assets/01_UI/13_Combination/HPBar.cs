using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Slider m_slider;

    [SerializeField, Range(0.0f, 1.0f)] float m_hp_ratio;

    private void Awake()
    {
        m_slider = this.GetComponent<Slider>();
        // link event....
    }

    public void OnHpChanged(int maxHp, int currentHp)
    {
        m_hp_ratio = currentHp / maxHp;
        m_slider.value = m_hp_ratio;
    }
}
