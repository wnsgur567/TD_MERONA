using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//�������� ������ �� ��
interface StageEndCallBack
{

}

//���� ������ ���� �� ��
interface Finalize
{

}

public class EnemyManager : Singleton<EnemyManager>
{
    protected Monster_TableExcelLoader m_EnemyData;

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
        m_EnemyData = M_DataTable.GetDataTable<Monster_TableExcelLoader>();

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

    public Monster_TableExcel GetData(int code)
    {
        Monster_TableExcel origin = m_EnemyData.DataList.Where(item => item.Code == code).Single();
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

    public delegate void CreateHandler(object sender, CreateEventArgs e);
    public event CreateHandler Create;

    public delegate void RetreveHandler(object sender, RetreveEventArgs e);
    public event RetreveHandler Retreve;

    protected virtual void On_Create(CreateEventArgs e)
    {
        CreateHandler handler = Create;
        handler?.Invoke(this, e);
    }

    protected virtual void On_Retreve(RetreveEventArgs e)
    {
        RetreveHandler handler = Retreve;
        handler?.Invoke(this, e);
    }

    public class CreateEventArgs
    {
        
    }

    public class RetreveEventArgs
    {

    }


}
