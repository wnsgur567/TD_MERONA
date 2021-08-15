using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Devil
{
    None,

    HateQueen,
    HellLord,
    FrostLich,
}

public class DevilManager : Singleton<DevilManager>
{
    protected Devil m_Devil;
    
    #region 내부 컴포넌트
    #endregion

    #region 내부 프로퍼티
    #endregion

    #region 외부 프로퍼티
    #endregion

    #region 내부 함수
    #endregion

    #region 외부 함수
    public void SelectDevil(E_Devil devil)
    {
        switch (devil)
        {
            case E_Devil.HateQueen:
                m_Devil = new HateQueen();
                break;
            case E_Devil.HellLord:
                break;
            case E_Devil.FrostLich:
                break;
        }
    }
    #endregion

    #region 유니티 콜백 함수
    void Awake()
    {
        
    }

    void Update()
    {
        
    }
    #endregion
}