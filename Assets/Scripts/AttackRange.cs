using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRange : MonoBehaviour
{
    protected SphereCollider m_RangeCollider;
    protected List<GameObject> m_TargetList;

    private void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        m_RangeCollider = GetComponent<SphereCollider>();
        m_RangeCollider ??= gameObject.AddComponent<SphereCollider>();
        m_RangeCollider.isTrigger = true;

        m_TargetList = new List<GameObject>();
    }

    public void SetRange(float range)
    {
        m_RangeCollider.radius = range;
    }
    public void Clear()
    {
        m_TargetList.Clear();
    }
    public bool RemoveTarget(GameObject target)
    {
        return m_TargetList.Remove(target);
    }
    public GameObject GetFirstTarget()
    {
        return m_TargetList[0];
    }
    public GameObject GetNearTarget(bool exceptFirst = false)
    {
        GameObject target;

        var tempList = m_TargetList
            .OrderBy(obj =>
            {
                return (transform.position - obj.transform.position).sqrMagnitude;
            })
            .ToList();

        if (exceptFirst)
        {
            if (tempList.Count > 1)
            {
                target = tempList[1];
            }
            else
            {
                target = null;
            }
        }
        else
        {
            target = tempList.FirstOrDefault();
        }

        return target;
    }
    public GameObject GetRandomTarget()
    {
        int max = m_TargetList.Count;
        if (max <= 0)
        {
            return null;
        }

        int index = Random.Range(0, max);

        return m_TargetList[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        m_TargetList.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        m_TargetList.Remove(other.gameObject);
    }
}
