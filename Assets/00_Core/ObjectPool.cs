using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<T> where T : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 100;
    [ReadOnly]
    public GameObject m_Origin;

    protected Dictionary<string, MemoryPool> m_Pools = null;

    protected ResourcesManager M_Resources => ResourcesManager.Instance;

    protected virtual void Awake()
    {
        __Initialize();
    }
    protected virtual void OnApplicationQuit()
    {
        __Finalize();
    }

    public virtual void __Initialize()
    {
        m_Pools = new Dictionary<string, MemoryPool>();

        //for (int i = 0; i < m_Origins.Count; ++i)
        //{
        //    GameObject parent = new GameObject();
        //    parent.transform.SetParent(this.transform);
        //    parent.name = m_Origins[i].name + "_parent";

        //    m_Pools.Add(m_Origins[i].name, new MemoryPool(m_Origins[i], m_PoolSize, parent.transform));
        //}
    }

    public virtual void __Finalize()
    {
        foreach (var item in m_Pools)
        {
            item.Value.Dispose();
        }

        m_Pools.Clear();
        m_Pools = null;
    }

    public virtual MemoryPool GetPool(string key)
    {
        return m_Pools[key];
    }

    //public abstract GameObject Spawn();
    //public abstract void Despawn(GameObject obj);
}
