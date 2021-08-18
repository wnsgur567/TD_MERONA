using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct StageChangedEventArgs
{
    public string stageName;
    public int stage_num;   // 순번
    public int stage_type;
    public float stage_time;  // stage 시간
}

public class StageInfoManager : Singleton<StageInfoManager>
{
    public delegate void StageChangeHandler(StageChangedEventArgs args);
    public event StageChangeHandler OnStageChangedEvent;
    public delegate void TimerUpdateHandler(float remainTime, float progress);
    public event TimerUpdateHandler OnTimeChangedEvent;

    [SerializeField] Stage_TableExcel m_current_stageInfo;
    [SerializeField] int current_stage;
    [SerializeField] Stage_TableExcelLoader m_excel_loader;

    [SerializeField] bool m_startFlag;
    [SerializeField] float m_timer;

    // 현재 스테이지 진행 상황(백분율 0.0f ~ 1.0f )
    public float Progress
    {
        get
        {
            return (1.0f - (m_current_stageInfo.StageTime - m_timer) / m_current_stageInfo.StageTime);
        }
    }

    private void Awake()
    {
        current_stage = 0;
        m_startFlag = true;
        GoNextStage();
    }

    private void Start()
    {       
        m_timer = 0.0f;
    }

    private void Update()
    {
        if (m_startFlag)
        {
            m_timer += Time.deltaTime;

            float remain_time = m_current_stageInfo.StageTime - m_timer;
            OnTimeChangedEvent?.Invoke(remain_time, Progress);

            if(Progress >= 1.0f)
            {
                m_timer = 0.0f;
                GoNextStage();
            }
        }
    }


    // 다음 스테이지로 이동
    public void GoNextStage()
    {        
        m_current_stageInfo = m_excel_loader.DataList[current_stage];
        ++current_stage;

        // synergy bonus gold
         int bonus_gold = SynergyManager.Instance.BonusGold;

        // user info update
        UserInfoManager.Instance.AddGold(m_current_stageInfo.Gold + bonus_gold);
        UserInfoManager.Instance.AddExp(m_current_stageInfo.Exp);
        

        // else
        OnStageChangedEvent?.Invoke(
            new StageChangedEventArgs
            {
                stageName = m_current_stageInfo.Name_EN,
                stage_num = m_current_stageInfo.Stage_Num,
                stage_type = m_current_stageInfo.StageType,
                stage_time = m_current_stageInfo.StageTime,
            });
    }
}
