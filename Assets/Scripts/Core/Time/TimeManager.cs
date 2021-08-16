using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    [System.Serializable]
    public struct Stage_TableData
    {
        public int Stage_Num;

        //1 �޽� 2 ����
        public int StageType;
        public float StageTime;
        public int StageMonsterTable;
        public int Exp;
        public int Gold;
        public int Prefab;
    }

    public Slider slTimer;

    private SpawnManager spawnmanager => SpawnManager.Instance;

    private Stage_TableExcel m_StageInfo_Excel;
    private StageManager M_Stage;

    private Stage_TableData m_StageInfo;

    private int TotalGold = 0;

    public int NowGold => TotalGold;

    private float TotalExp = 0f;

    public float NowExp => TotalExp;

    private string codename = "410000";

    private int Stage_Num = 1;

    void Start()
    {
        int stagecode = int.Parse(codename) + Stage_Num;

        InitializeStage(stagecode);

        slTimer.maxValue = m_StageInfo.StageTime;
        slTimer.value = m_StageInfo.StageTime;
    }

    void Update()
    {
        if (m_StageInfo.StageTime != 41)
        {
            if (slTimer.value > 0.0f)
            {
                // �ð��� ������ ��ŭ slider Value ������ �մϴ�.
                slTimer.value -= Time.deltaTime;
            }
            else
            {
                M_End_Time();
            }
        }
    }

    public void M_End_Time()
    {
        //enemy ���� �ð��� ������
        if (Stage_Num % 2 == 0)
        {
            TotalGold += SynergyManager.Instance.BonusGold;
        }

        //��ٸ��� �ð� ������ ���� ��ȯ
        else if (Stage_Num % 2 == 1)
        {
            spawnmanager.Start_Stage(m_StageInfo.StageMonsterTable);
        }

        ++Stage_Num;

        int stagecode = int.Parse(codename) + Stage_Num;

        InitializeStage(stagecode);

        slTimer.maxValue = m_StageInfo.StageTime;
        slTimer.value = m_StageInfo.StageTime;

    }

    public void InitializeStage(int code)
    {
        #region ���� ������ ����

        m_StageInfo_Excel = M_Stage.GetData(code);

        #endregion

        #region ���� ������ ����

        m_StageInfo.Stage_Num = m_StageInfo_Excel.Stage_Num;
        m_StageInfo.StageType = m_StageInfo_Excel.StageType;
        m_StageInfo.StageTime = m_StageInfo_Excel.StageTime;
        m_StageInfo.StageMonsterTable = m_StageInfo_Excel.StageMonsterTable;
        TotalExp += m_StageInfo_Excel.Exp;
        TotalGold += m_StageInfo_Excel.Gold;

        if (TotalExp >= 10)
        {
            TotalExp -= 10;
            //������ �Լ� �ҷ��;ߵ�
        }

        #endregion
    }
}
