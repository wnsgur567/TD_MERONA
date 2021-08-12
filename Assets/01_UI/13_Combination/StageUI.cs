using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageUI : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_stage_textprro;
    [Space(10)]
    [SerializeField] Image m_radialImage;
    [SerializeField] TMPro.TextMeshProUGUI m_time_textpro;

    private void Awake()
    {
        
    }

    public void OnStageChanged(int current_stage1, int current_stage2)
    {
        m_stage_textprro.text =
            current_stage1.ToString() + "-" + current_stage2.ToString();
    }
    public void OnCountChanged(int max_time,int remain_time)
    {
        float ratio = remain_time / max_time;
        m_time_textpro.text = remain_time.ToString();
        m_radialImage.fillAmount = ratio;
    }
}
