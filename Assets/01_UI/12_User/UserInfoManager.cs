using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserInfo
{
    public int SelectedDevilCode;
    public int level;
    public int exp;
    public int gold;
}

[DefaultExecutionOrder(-40)]
public class UserInfoManager : Singleton<UserInfoManager>
{
    public delegate void LevelChangeHandler(int current_level);
    public event LevelChangeHandler OnLevelChanged;
    public delegate void GoldChangeHandler(int current_gold);
    public event GoldChangeHandler OnGoldChangedEvent;

    [SerializeField] UserInfo m_info;
    [SerializeField] Level_TableExcelLoader m_levelLadoer;
    [SerializeField] Stage_TableExcelLoader m_stageLoader;

    public int DevilCode { get { return m_info.SelectedDevilCode; } }
    public int Level { get { return m_info.level; } }
    public int Gold { get { return m_info.gold; } }
    public int EXP { get { return m_info.exp; } }


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (Level == 0)
            m_info.level = 1;
    }
    private void Start()
    {
        
    }

    public void UpdateAllInfo()
    {
        OnLevelChanged?.Invoke(m_info.level);
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }

    public void SetDevilCode(int code)
    {
        m_info.SelectedDevilCode = code;
    }

    public void ResetLevel()
    {
        m_info.level = 1;
        OnLevelChanged?.Invoke(m_info.level);
    }
    public void ResetGold()
    {
        m_info.gold = 0;
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }

    public void AddLevel(int val)
    {
        m_info.level += val;
        OnLevelChanged?.Invoke(m_info.level);
    }

    public void AddGold(int val)
    {
        m_info.gold += val;
        OnGoldChangedEvent?.Invoke(m_info.gold);
    }
    public bool UseGold(int val)
    {
        if (val > Gold)
            return false;

        m_info.gold -= val;
        OnGoldChangedEvent?.Invoke(m_info.gold);

        return true;
    }

    public void AddExp(int val)
    {
        var cur_level_data = m_levelLadoer.DataList[m_info.level - 1];

        // 현재 요구 경험치
        int require_exp_to_next_level = cur_level_data.LvUP_Exp;
        int cur_exp = m_info.exp + val;
        if(cur_exp >= require_exp_to_next_level)
        {   // 요구 경험치를 초과하면
            // level up
            m_info.exp = cur_exp - require_exp_to_next_level;
            AddLevel(1);
        }
    }
}
