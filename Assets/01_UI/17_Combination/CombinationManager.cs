using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinationManager : Singleton<CombinationManager>
{
    [SerializeField] Tower_TableExcelLoader m_towerLoader;
    private int MaxStar = 3;    // star maximum


    private bool IsMaximum(Tower tower)
    {
        if (tower.ExcelData.Star >= MaxStar)
            return true;
        return false;
    }


    // param : all towers in list need same code 
    public void CombinationProcess(List<Tower> tower_list)
    {
        // desapwn only 3 towers
        int current_star = tower_list[0].ExcelData.Star;
        for (int i = 0; i < 3; i++)
        {
            TowerManager.Instance.DespawnTower(tower_list[i]);
        }
        
    }


    public bool pleas_change_name()
    {
        Dictionary<int, List<Tower>> codeToCount_dic = new Dictionary<int, List<Tower>>();
        var tower_list = TowerManager.Instance.GetTowerList();
        foreach (var item in tower_list)
        {
            if (codeToCount_dic.ContainsKey(item.TowerCode))
            {
                codeToCount_dic[item.TowerCode].Add(item);
            }
            else
            {
                codeToCount_dic[item.TowerCode] = new List<Tower>();
                codeToCount_dic[item.TowerCode].Add(item);
            }
        }

        foreach (var item in codeToCount_dic)
        {
            if(item.Value.Count > 3)
            {
                if (IsMaximum(item.Value[0]))
                    continue;

                CombinationProcess(item.Value);
            }
        }



        return false;
    }

}
