using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : Singleton<SpawnManager>
{
    [Serializable]
    public struct SpawnData
    {
        public int SponPosition;
        public int Create_num;
        public int Monster_Code;
        public float AppearSpeed;
        public float CreateSpeed;
    }

    //웨이 포인트 시작 위치
    public Transform[] spawnPoint;

    public EnemyPool enemyPool => EnemyPool.Instance;
    private EnemyManager enemymanager => EnemyManager.Instance;
    private Stage_EnemyDataManager M_StageEnemy => Stage_EnemyDataManager.Instance;

    private List<StageEnemy_TableExcel> m_StageEnemyInfo_Excel;

    [SerializeField] SpawnData[] m_StageEnemyInfo;

    [SerializeField] int countnum = 0;

    [SerializeField] int startnum = 0;

    private Enemy_TableExcel m_Enemyinfo_Excel;

    private void Awake()
    {
        m_StageEnemyInfo_Excel = new List<StageEnemy_TableExcel>();
    }

    #region 외부 함수
    //스테이지 시작
    public void Start_Stage(int code)
    {
        ++startnum;

        InitializeStageEnemy(code);

        for (int i = 0; i < countnum; i++)
        {
            StartCoroutine(Spawn((E_Direction)m_StageEnemyInfo[i].SponPosition, i));
        }
    }

    // 몬스터 사망 함수
    public void Despawn(Enemy enemy)
    {
        EnemyManager.Instance.Enemy_Direction[enemy.Get_Direction].Remove(enemy);
        enemy.FinializeEnemy();
        enemyPool.GetPool(enemy.Get_EnemyName_EN).DeSpawn(enemy);
    }

    #endregion

    #region 내부 함수

    // PrefebData 초기화
    private string GetPrefebName(int code)
    {
        m_Enemyinfo_Excel = enemymanager.GetData(code);

        return m_Enemyinfo_Excel.Name_EN;
    }

    // Stage 초기화
    // 북동남서
    private void InitializeStageEnemy(int code)
    {
        #region 엑셀 데이터 정리

        m_StageEnemyInfo_Excel = M_StageEnemy.GetListData(code);

        #endregion

        #region 내부 데이터 정리

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
        enemy.InitSetting(dir - 1);
        enemy.transform.position = spawnPoint[(int)dir - 1].position;

        enemy.gameObject.SetActive(true);

        enemymanager.Enemy_Direction[dir].Add(enemy);
    }

    public void SpawnEnemy(E_Direction dir, Vector3 pos, Transform target, int waypointindex, string enemy_name, Animator animator)
    {
        Enemy enemy = enemyPool.GetPool(enemy_name).Spawn();
        enemy.InitSetting(dir, target, waypointindex);
        enemy.transform.position = pos;

        enemy.gameObject.SetActive(true);

        enemy.InitializeEnemy(200009);

        animator.SetBool("Skill", true);

        enemymanager.Enemy_Direction[dir].Add(enemy);
    }
    #endregion

    #region 코루틴
    //혹시 모를 라운드 끝나면 없어지는 몬스터
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
        //처음 등장 속도
        yield return new WaitForSeconds(m_StageEnemyInfo[num].AppearSpeed);

        for (int i = 0; i < m_StageEnemyInfo[num].Create_num - 1; ++i)
        {
            SpawnEnemy(dir, num);
            //생성 속도
            yield return new WaitForSeconds(m_StageEnemyInfo[num].CreateSpeed);
        }

        SpawnEnemy(dir, num);
    }
    #endregion
}
