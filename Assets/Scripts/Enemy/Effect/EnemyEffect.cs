using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEffect : MonoBehaviour
{
    public int m_PrefabCode;
    public E_EffectType m_Type;
    protected ParticleSystem m_Particle;
    protected float m_Timer;

    #region ���� ������Ʈ
    protected EnemyEffectManager M_Effect => EnemyEffectManager.Instance;
    #endregion

    #region ���� ������Ƽ
    #endregion

    #region �ܺ� ������Ƽ
    #endregion

    #region ���� �Լ�
    #endregion

    #region �ܺ� �Լ�
    public void InitializeEffect()
    {
        if (null == m_Particle)
        {
            m_Particle = GetComponentInChildren<ParticleSystem>(true);
        }
        m_Timer = 0f;
    }
    public void FinalizeEffect()
    {
        m_Timer = 0f;
    }
    #endregion

    #region ����Ƽ �ݹ� �Լ�
    void Update()
    {
        m_Timer += Time.deltaTime;

        if (m_Timer >= m_Particle.main.duration)
        {
            m_Particle.Stop(true);
            M_Effect.DespawnEffect(this);
        }
    }
    #endregion
}
