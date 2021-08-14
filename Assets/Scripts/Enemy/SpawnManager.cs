using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : Singleton<SpawnManager>
{
    [Serializable]
    public struct SpawnData
    {
        public int No;
        public string Name_KR;
        public string Name_EN;
        public int Code;
        public int SponPosition;
        public int Create_num;
        public int Monster_Code;
        public float AppearSpeed;
        public float CreateSpeed;
    }

    //���� ������
    public Transform enemyPrefeb;

    //���� ����Ʈ ���� ��ġ
    public Transform[] spawnPoint;

    public EnemyPool enemyPool => EnemyPool.Instance;

    //���⺰ ���� ���� �����ϴ� ����Ʈ
    public Dictionary<E_Direction, List<Enemy>> Enemy_Direction;
    //���� ������ ����
    public Dictionary<E_Direction, List<int>> EnemyIndex_Direction;
    //���� �̸� ����Ʈ
    public Dictionary<E_Direction, List<string>> EnemyName_Direction;

    //��ü ����
    public List<Enemy> All_Enemy;

    //���� ��ȯ �ð� ����
    public float Summon_Time = 0.5f;

    private int Round = 0;

    private S_Stage_EnemyData_Excel data;

    private SpawnData spawndata;

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

    #region �ܺ� �Լ�
    //�������� ����
    public void Start_Stage()
    {
        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            if (EnemyIndex_Direction[i][Round] != 0)
            {
                StartCoroutine(Spawn(i, Round));
            }
        }

        ++Round;
    }

    // ���� ��� �Լ�
    public void Despawn(Enemy enemy)
    {
        Enemy_Direction[enemy.Get_Direction()].Remove(enemy);
        enemyPool.GetPool(enemy.Get_Enemy_Name()).DeSpawn(enemy);
    }
    #endregion

    #region �ڷ�ƾ
    //Ȥ�� �� ���� ������ �������� ����
    IEnumerator EndStage(int round)
    {
        if (round != 0)
        {
            for (E_Direction i = 0; i < E_Direction.Max; ++i)
            {
                if (EnemyIndex_Direction[i][round - 1] != 0)
                {
                    for (int j = 0; j < Enemy_Direction[i].Count; ++j)
                    {
                        Despawn(Enemy_Direction[i][0]);
                    }
                }
            }
            //yield return new WaitForSeconds(WaitStageTime);
        }
        yield return null;
    }

    IEnumerator Spawn(E_Direction dir, int round)
    {
        for (int i = 0; i < EnemyIndex_Direction[dir][round] - 1; ++i)
        {
            SpawnEnemy(dir);
            yield return new WaitForSeconds(Summon_Time);
        }
        SpawnEnemy(dir);
    }
    #endregion

    #region ���� �� �ϵ����� ��ȯ �Լ�
    //�������� ������ �̾ƿͼ� �ٲ�ߵ�
    public void SpawnEnemy(E_Direction dir, string enemy_name = "Defender01")
    {
        Enemy enemy = enemyPool.GetPool(enemy_name).Spawn();
        enemy.InitSetting(dir);
        enemy.transform.position = spawnPoint[(int)dir].position;

        enemy.gameObject.SetActive(true);

        Enemy_Direction[dir].Add(enemy);
    }

    public void SpawnEnemy(E_Direction dir, Vector3 pos, Transform target, int waypointindex, string enemy_name = "defender1")
    {
        Enemy enemy = enemyPool.GetPool(enemy_name).Spawn();
        enemy.InitSetting(dir, target, waypointindex);
        enemy.transform.position = pos;

        enemy.gameObject.SetActive(true);

        Enemy_Direction[dir].Add(enemy);
    }
    #endregion
}
