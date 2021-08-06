using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageInfo
{
    public int stage;
}

public class StageInfoManager : Singleton<StageInfoManager>
{
    public delegate void StageChangeHandler(int current_stage);
    public event StageChangeHandler OnStageChanged;

    [SerializeField] StageInfo m_info;

    public void NextStage()
    {
        ++m_info.stage;
        OnStageChanged(m_info.stage);
    }
    
}
