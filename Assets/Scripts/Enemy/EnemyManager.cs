using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    protected Enemy_TableExcelLoader m_EnemyData => M_DataTable.GetDataTable<Enemy_TableExcelLoader>();

    protected DataTableManager M_DataTable => DataTableManager.Instance;

    //���⺰ ���� ���� �����ϴ� ����Ʈ
    public Dictionary<E_Direction, List<Enemy>> Enemy_Direction;
    //���� ������ ����
    public Dictionary<E_Direction, List<int>> EnemyIndex_Direction;
    //���� �̸� ����Ʈ
    public Dictionary<E_Direction, List<string>> EnemyName_Direction;

    //��ü ����
    public List<Enemy> All_Enemy;

    private void Awake()
    {
        All_Enemy = new List<Enemy>();

        Enemy_Direction = new Dictionary<E_Direction, List<Enemy>>();
        EnemyIndex_Direction = new Dictionary<E_Direction, List<int>>();
        EnemyName_Direction = new Dictionary<E_Direction, List<string>>();

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            Enemy_Direction[i] = new List<Enemy>();
            EnemyIndex_Direction[i] = new List<int>();
            EnemyName_Direction[i] = new List<string>();
        }
    }

    public Enemy_TableExcel GetData(int code)
    {
        Enemy_TableExcel origin = m_EnemyData.DataList.Where(item => item.Code == code).Single();
        return origin;
    }

    //��ü ���� ������ �̾ƿ��� �Լ�
    public List<Enemy> GetEnemyList()
    {
        return All_Enemy;
    }

    //�� ���� ��ü ���� �̾ƿ��� �Լ�
    public List<Enemy> GetEnemyList(E_Direction direc)
    {
        return Enemy_Direction[direc];
    }
}
