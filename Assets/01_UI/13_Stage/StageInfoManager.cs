using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StageInfoManager : Singleton<StageInfoManager>
{
    public delegate void StageChangeHandler(int current_stage, int current_stage_type);
    public event StageChangeHandler OnStageChangedEvent;
    public delegate void TimerUpdateHandler(float remainTime, float progress);
    public event TimerUpdateHandler OnTimeChangedEvent;

    [SerializeField] Stage_TableExcel m_info;
    [SerializeField] Stage_TableExcelLoader m_excel_loader;

    [SerializeField] bool m_startFlag;
    [SerializeField] float m_timer;

    // 현재 스테이지 진행 상황(백분율 0.0f ~ 1.0f )
    public float Progress
    {
        get
        {
            return (1.0f - (m_info.StageTime - m_timer) / m_info.StageTime);
        }
    }

    private void Awake()
    {

    }

    private void Start()
    {
        m_startFlag = false;
        m_info = m_excel_loader.DataList[0];
        m_timer = 0.0f;
    }

    private void Update()
    {
        if (m_startFlag)
        {
            m_timer += Time.deltaTime;

            float remain_time = m_info.StageTime - m_timer;
            OnTimeChangedEvent?.Invoke(remain_time, Progress);

            if(Progress >= 1.0f)
            {
                m_timer = 0.0f;
                GoNextStage();
            }
        }
    }

    public void StageStart()
    {
        m_startFlag = true;
    }

    // 다음 스테이지로 이동
    public void GoNextStage()
    {
        m_info = m_excel_loader.DataList[m_info.Stage_Num];

        OnStageChangedEvent?.Invoke(m_info.Stage_Num, m_info.StageType);
    }
}
