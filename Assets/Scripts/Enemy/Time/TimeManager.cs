using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    //엑셀에서 데이터 가져와야됨

    //기다리는 시간(정비 시간)
    public List<float> M_Wait_Next_StageTime;

    //몬스터가 소환되고 기다리는 시간
    public List<float> M_Enemy_Atk_Time;

    public Slider slTimer;

    private List<float> Next_Time;

    private int m_StageIndex = 0;

    private SpawnManager spawnmanager;

    private void Awake()
    {
        Next_Time = new List<float>();
    }

    void Start()
    {
        spawnmanager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();

        for (int i = 0; i < M_Enemy_Atk_Time.Count; i++)
        {
            Next_Time.Add(M_Wait_Next_StageTime[i]);
            Next_Time.Add(M_Enemy_Atk_Time[i]);
        }

        slTimer.maxValue = Next_Time[m_StageIndex];
        slTimer.value = Next_Time[m_StageIndex];
    }

    void Update()
    {
        if (m_StageIndex != Next_Time.Count)
        {
            if (slTimer.value > 0.0f)
            {
                // 시간이 변경한 만큼 slider Value 변경을 합니다.
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
        slTimer.maxValue = Next_Time[m_StageIndex];
        slTimer.value = Next_Time[m_StageIndex];

        //기다리는 시간 끝나면 몬스터 소환
        if (m_StageIndex % 2 == 0)
        {
            spawnmanager.Start_Stage();
        }

        //enemy가 공격하는 시간
        else if (m_StageIndex % 2 == 1)
        {
            
        }

        m_StageIndex++;
    }
}
