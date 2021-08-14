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

    private void Awake()
    {
        m_EnemyData = M_DataTable.GetDataTable<Monster_TableExcelLoader>();
    }

    public S_EnemyData_Excel GetData(int code)
    {
        Monster_TableExcel origin = m_EnemyData.DataList.Where(item => item.Code == code).Single();
        S_EnemyData_Excel result = new S_EnemyData_Excel(origin);
        return result;
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
