using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 최종 수정 09-26
public class MemoryPool : System.IDisposable
{
    // 오브젝트들을 담을 실제 풀
    private Queue<GameObject> queue = new Queue<GameObject>();
    // 생성한 오브젝트를 기억하고 있다가 디스폰 시 확인할 리스트
    private List<GameObject> despawnList = new List<GameObject>();
    // 풀에 담을 원본
    private GameObject original;
    // 초기 풀 사이즈
    private int poolSize;
    // 하이어라키 창에서 관리하기 쉽도록 parent 지정
    private Transform parent = null;

    public int preLoadedPoolSize = 1000;

    // 부모 지정하여 생성하는 경우
    public MemoryPool(GameObject _original, int _poolSize, Transform parent)
    {
        original = _original;
        poolSize = _poolSize;
        this.parent = parent;

        for (int i = 0; i < Mathf.Min(poolSize, preLoadedPoolSize); i++)
        {
            GameObject newItem = GameObject.Instantiate(original);  //new
            string[] strs = newItem.name.Split('(');
            newItem.name = strs[0];
            newItem.SetActive(false);
            newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }


        //if (poolSize > preLoadedPoolSize)
        //{
        //    StartCoroutine(MakePool());
        //}
    }

    //IEnumerator MakePool()
    //{
    //    for (int i = preLoadedPoolSize; i < poolSize; ++i)
    //    {
    //        GameObject newItem = GameObject.Instantiate(original);  //new
    //        string[] strs = newItem.name.Split('(');
    //        newItem.name = strs[0];
    //        newItem.SetActive(false);
    //        newItem.transform.SetParent(parent);
    //        queue.Enqueue(newItem);
    //    }

    //    yield return new WaitForSeconds(0.1f);
    //}

    // foreach 문을 위한 반복자
    public IEnumerator GetEnumerator()
    {
        foreach (GameObject item in queue)
            yield return item;
    }

    // 오브젝트 풀이 빌 경우 선택적으로 call
    // 절반만큼 증가
    void ExpandPoolSize()
    {
        int newSize = poolSize + poolSize / 2;
        for (int i = poolSize; i < newSize; i++)
        {
            GameObject newItem = GameObject.Instantiate(original);
            newItem.SetActive(false);
            if (parent != null)
                newItem.transform.SetParent(parent);
            queue.Enqueue(newItem);
            despawnList.Add(newItem);
        }
        poolSize = newSize;
    }

    // 모든 오브젝트 사용시 추가로 생성할 경우 
    // expand 를 true 로 설정
    public GameObject Spawn(bool expand = true)
    {
        if (expand && queue.Count <= 0)
        {
            ExpandPoolSize();
        }

        if (queue.Count > 0)
        {
            GameObject item = queue.Dequeue();
            return item.gameObject;
        }

        Debug.LogWarning("Pool Size Over");
        return null;
    }

    // 회수 작업
    public void DeSpawn(GameObject gameObject)
    {
        if (gameObject == null ||
            !despawnList.Contains(gameObject))
            return;

        gameObject.SetActive(false);
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = Vector3.zero;
        queue.Enqueue(gameObject);
    }

    // 메모리 해제
    public void Dispose()
    {
        foreach (GameObject item in queue)
            GameObject.DestroyImmediate(item);
        queue.Clear();
        queue = null;
        despawnList.Clear();
        despawnList = null;
    }
}