using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPBarPool : ObjectPool<EnemyHPBarPool, EnemyHPBar>
{
    public EnemyHPBar m_Origin;

    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    public override void __Initialize()
    {
        base.__Initialize();

        m_Origin.Initialize();

        AddPool("EnemyHPBar", m_Origin, transform);

        m_Origin.gameObject.SetActive(false);
    }
    #endregion

    #region 외부 함수
    #endregion
}
