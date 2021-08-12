using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UserInfo
{
    public int level;
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

    public int Level { get { return m_info.level; } }
    public int Gold { get { return m_info.gold; } }


    private void Awake()
    {
        if (Level == 0)
            m_info.level = 1;
    }
    private void Start()
    {
        
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
}
