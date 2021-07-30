using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectManager<T> : Singleton<T> where T : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 1000;
    [SerializeField]
    protected List<GameObject> m_Origins = null;
    protected List<MemoryPool> m_Pools = null;

    protected virtual void Awake()
    {
        __Initialize();
    }

    public virtual void __Initialize()
    {
        m_Pools = new List<MemoryPool>();

        for (int i = 0; i < m_Origins.Count; ++i)
        {
            GameObject parent = new GameObject();
            parent.transform.SetParent(this.transform);
            parent.name = m_Origins[i].name + "_parent";

            m_Pools.Add(new MemoryPool(m_Origins[i], m_PoolSize, parent.transform));
        }
    }

    public virtual void __Finalize()
    {
        for (int i = m_Pools.Count - 1; i >= 0; --i)
        {
            m_Pools[i].Dispose();
        }
        m_Pools.Clear();
        m_Pools = null;
    }

    public virtual MemoryPool GetPool(int index = 0)
    {
        return m_Pools[index];
    }
}
