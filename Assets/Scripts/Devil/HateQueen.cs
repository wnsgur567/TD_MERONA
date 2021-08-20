using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HateQueen : Devil
{
    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 유니티 콜백
    private void Awake()
    {
        InitializeDevil(E_Devil.HateQueen);
    }
    #endregion

    #region 내부 함수
    protected override void InitializeDevil(E_Devil no)
    {
        base.InitializeDevil(no);

        #region 마왕 스킬 정리
        Skill01Event += Skill01;
        Skill02Event += Skill02;
        #endregion
    }

    protected void Skill01(DevilSkillArg arg)
    {
        StartCoroutine(SK001(arg));
    }
    protected void Skill02(DevilSkillArg arg)
    {
        StartCoroutine(SK002(arg));
    }

    protected IEnumerator SK001(DevilSkillArg arg)
    {
        BuffCC_TableExcel buffData = M_Buff.GetData(arg.skillData.m_StatData.Buff_CC);
        List<Tower> towerList = M_Tower.GetTowerList(arg.dir);

        foreach (var item in towerList)
        {
            item.m_TowerInfo.DevilSkillBuffList.Add(buffData);
        }

        yield return new WaitForSeconds(buffData.Duration);

        // 아마 버그날 듯
        foreach (var item in towerList)
        {
            item.m_TowerInfo.DevilSkillBuffList.Remove(buffData);
        }
    }
    protected IEnumerator SK002(DevilSkillArg arg)
    {
        BuffCC_TableExcel buffData = M_Buff.GetData(arg.skillData.m_StatData.Buff_CC);

        for (E_Direction i = 0; i < E_Direction.Max; ++i)
        {
            List<Tower> towerList = M_Tower.GetTowerList(i);

            foreach (var item in towerList)
            {
                item.m_TowerInfo.DevilSkillBuffList.Add(buffData);
            }

            yield return new WaitForSeconds(buffData.Duration);

            // 아마 버그날 듯
            foreach (var item in towerList)
            {
                item.m_TowerInfo.DevilSkillBuffList.Remove(buffData);
            }
        }
    }
    #endregion

    #region 외부 함수
    #endregion
}
