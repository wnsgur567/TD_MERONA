using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HateQueen : Devil
{
    #region ���� ������Ʈ
    #endregion

    #region ���� ������Ƽ
    #endregion

    #region ���� ������Ƽ
    #endregion

    #region ����Ƽ �ݹ�
    private void Awake()
    {
        InitializeDevil(E_Devil.HateQueen);
    }
    #endregion

    #region ���� �Լ�
    protected override void InitializeDevil(E_Devil no)
    {
        base.InitializeDevil(no);

        #region ���� ��ų ����
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

        // �Ƹ� ���׳� ��
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

            // �Ƹ� ���׳� ��
            foreach (var item in towerList)
            {
                item.m_TowerInfo.DevilSkillBuffList.Remove(buffData);
            }
        }
    }
    #endregion

    #region �ܺ� �Լ�
    #endregion
}
