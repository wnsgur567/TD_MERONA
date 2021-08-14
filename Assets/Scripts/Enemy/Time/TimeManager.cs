using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : Singleton<TimeManager>
{
    //�������� ������ �����;ߵ�

    //��ٸ��� �ð�(���� �ð�)
    public List<float> M_Wait_Next_StageTime;

    //���Ͱ� ��ȯ�ǰ� ��ٸ��� �ð�
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
        slTimer.maxValue = Next_Time[m_StageIndex];
        slTimer.value = Next_Time[m_StageIndex];

        //��ٸ��� �ð� ������ ���� ��ȯ
        if (m_StageIndex % 2 == 0)
        {
            spawnmanager.Start_Stage();
        }

        //enemy�� �����ϴ� �ð�
        else if (m_StageIndex % 2 == 1)
        {
            
        }

        m_StageIndex++;
    }
}
