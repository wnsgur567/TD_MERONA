using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : Singleton<SpawnManager>
{
    [Serializable]
    public struct EnemyPrefebData
    {
        public int code;
        public string Name_EN;
    }

    [Serializable]
    public struct SpawnData
    {
        public int SponPosition;
        public int Create_num;
        public int Monster_Code;
        public float AppearSpeed;
        public float CreateSpeed;
    }
    private int enemynum = 0;

    //���� ����Ʈ ���� ��ġ
    public Transform[] spawnPoint;

    public EnemyPool enemyPool => EnemyPool.Instance;

    private List<StageEnemy_TableExcel> m_StageEnemyInfo_Excel;
    private Stage_EnemyManager M_StageEnemy;

    private SpawnData[] m_StageEnemyInfo;

    private EnemyManager enemymanager => EnemyManager.Instance;

    private string codename = "410000";

    private int countnum = 0;

    private Enemy_TableExcel m_Enemyinfo_Excel;
    private EnemyManager M_Enemy;

    private void Awake()
    {
        m_StageEnemyInfo_Excel = new List<StageEnemy_TableExcel>();
    }

    

    #region �ܺ� �Լ�
    //�������� ����
    public void Start_Stage(int code)
    {
        InitializeStageEnemy(code);

        for (int i = 0; i < countnum; i++)
        {
            StartCoroutine(Spawn((E_Direction)m_StageEnemyInfo[i].SponPosition, i));
        }
    }

    // ���� ��� �Լ�
    public void Despawn(Enemy enemy)
    {
        EnemyManager.Instance.Enemy_Direction[enemy.Get_Direction].Remove(enemy);
        enemyPool.GetPool(enemy.Get_EnemyName_EN).DeSpawn(enemy);
    }
    #endregion

    #region �ڷ�ƾ
    //Ȥ�� �� ���� ������ �������� ����
    //IEnumerator EndStage(int round)
    //{
    //    if (round != 0)
    //    {
    //        for (E_Direction i = 0; i < E_Direction.Max; ++i)
    //        {
    //            if (enemymanager.EnemyIndex_Direction[i][round - 1] != 0)
    //            {
    //                for (int j = 0; j < enemymanager.Enemy_Direction[i].Count; ++j)
    //                {
    //                    Despawn(enemymanager.Enemy_Direction[i][0]);
    //                }
    //            }
    //        }
    //        //yield return new WaitForSeconds(WaitStageTime);
    //    }
    //    yield return null;
    //}

    IEnumerator Spawn(E_Direction dir, int num)
    {
        //ó�� ���� �ӵ�
        yield return new WaitForSeconds(m_StageEnemyInfo[num].AppearSpeed);

        for (int i = 0; i < m_StageEnemyInfo[num].Create_num; ++i)
        {
            SpawnEnemy(dir, num);
            //���� �ӵ�
            yield return new WaitForSeconds(m_StageEnemyInfo[num].CreateSpeed);
        }

        SpawnEnemy(dir, num);
    }
    #endregion

    #region ���� �Լ�

    // PrefebData �ʱ�ȭ
    private string GetPrefebName(int code)
    {
        m_Enemyinfo_Excel = M_Enemy.GetData(code);

        return m_Enemyinfo_Excel.Name_EN;
    }

    // Stage �ʱ�ȭ
    // �ϵ�����
    private void InitializeStageEnemy(int code)
    {
        #region ���� ������ ����

        m_StageEnemyInfo_Excel = M_StageEnemy.GetListData(code);

        #endregion

        #region ���� ������ ����

        countnum = m_StageEnemyInfo_Excel.Count;

        m_StageEnemyInfo = new SpawnData[countnum];

        for (int i = 0; i < countnum; ++i)
        {
            m_StageEnemyInfo[i].SponPosition = m_StageEnemyInfo_Excel[i].SponPosition;
            m_StageEnemyInfo[i].Create_num = m_StageEnemyInfo_Excel[i].Create_num;
            m_StageEnemyInfo[i].Monster_Code = m_StageEnemyInfo_Excel[i].Monster_Code;
            m_StageEnemyInfo[i].AppearSpeed = m_StageEnemyInfo_Excel[i].AppearSpeed;
            m_StageEnemyInfo[i].CreateSpeed = m_StageEnemyInfo_Excel[i].CreateSpeed;
        }

        #endregion
    }

    private void SpawnEnemy(E_Direction dir, int num)
    {
        Enemy enemy = enemyPool.GetPool(GetPrefebName(m_StageEnemyInfo[num].Monster_Code)).Spawn();
        enemy.InitSetting(dir);
        enemy.transform.position = spawnPoint[(int)dir].position;

        enemy.gameObject.SetActive(true);

        enemymanager.Enemy_Direction[dir].Add(enemy);
    }

    public void SpawnEnemy(E_Direction dir, Vector3 pos, Transform target, int waypointindex, string enemy_name)
    {
        Enemy enemy = enemyPool.GetPool(enemy_name).Spawn();
        enemy.InitSetting(dir, target, waypointindex);
        enemy.transform.position = pos;

        enemy.gameObject.SetActive(true);

        enemymanager.Enemy_Direction[dir].Add(enemy);
    }
    #endregion

}
