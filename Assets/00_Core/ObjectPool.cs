using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-97)]
public abstract class ObjectPool<Pool, Origin> : Singleton<Pool> where Pool : MonoBehaviour where Origin : MonoBehaviour
{
    [ReadOnly(true)]
    public int m_PoolSize = 100;

    public Dictionary<string, Origin> m_Origins = null;
    protected Dictionary<string, MemoryPool<Origin>> m_Pools = null;

#if UNITY_EDITOR
    [ReadOnly]
    public DebugDictionary<string, Origin> m_DebugOrigin = null;
#endif

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
        m_Origins = new Dictionary<string, Origin>();
        m_Pools = new Dictionary<string, MemoryPool<Origin>>();

#if UNITY_EDITOR
        m_DebugOrigin = new DebugDictionary<string, Origin>();
#endif

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

    protected void AddPool(string key, Origin origin, Transform parent)
    {
        if (m_Origins.ContainsKey(key))
            return;

        m_Origins.Add(key, origin);

        GameObject Parent = new GameObject();
        Parent.name = origin.name;
        Parent.transform.SetParent(parent);
        Parent.SetActive(false);

        m_Pools.Add(key, new MemoryPool<Origin>(origin, m_PoolSize, Parent.transform));

#if UNITY_EDITOR
        m_DebugOrigin.Add(key, origin);
#endif
    }
    public virtual MemoryPool<Origin> GetPool(string key)
    {
        if (m_Pools.ContainsKey(key))
            return m_Pools[key];

        return null;
    }

    //public abstract GameObject Spawn();
    //public abstract void Despawn(GameObject obj);
}
