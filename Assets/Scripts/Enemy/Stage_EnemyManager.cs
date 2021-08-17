using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage_EnemyManager : Singleton<Stage_EnemyManager>
{
    private StageChangedEventArgs Now_StageData;

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    private Stage_TableExcelLoader stage_excel_loader;
    
    private int monsterCode;

    private void Awake()
    {
        stage_excel_loader = M_DataTable.GetDataTable<Stage_TableExcelLoader>();
    }

    void Start()
    {
        StageInfoManager.Instance.OnTimeChangedEvent += OnCountChanged;
        StageInfoManager.Instance.OnStageChangedEvent += OnStageChanged;
    }

    public void OnStageChanged(StageChangedEventArgs args)
    {
        Now_StageData.stage_num = args.stage_num;
        Now_StageData.stageName = args.stageName;
        Now_StageData.stage_time = args.stage_time;
        Now_StageData.stage_type = args.stage_type;
    }
    public void OnCountChanged(float remainTime, float progress)
    {
        if (Now_StageData.stage_num % 2 == 1 && progress >= 1.0f)
        {
            M_End_Time();
        }
    }

    public void M_End_Time()
    {
        monsterCode = stage_excel_loader.DataList[Now_StageData.stage_num].StageMonsterTable;
        SpawnManager.Instance.Start_Stage(monsterCode);
    }
}
