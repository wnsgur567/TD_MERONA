using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// 현재 작업중인 Scene : A
// 다음에 작업해야 할 Scene : B , C

// A 에서 SceneLoader 가 포함된 Scene(LoaderScene)을 불러오기
// SceneLoader의 멤버함수 LoadSecen 을 호출하기 (param 으로 B , C 의 이름)
// 진행이 완료되면
// A 와 Loader의 씬을 닫고 끝
public class SceneLoader : Singleton<SceneLoader>
{
    [SerializeField] UnityEvent m_startEvent; 
    [SerializeField] UnityEvent m_loadCompleteEvent; 
    
    private List<string> m_load_scenes;
    private List<string> m_unload_scenes;
    //[SerializeField] Slider m_progressBar;
        
    public bool IsCompleted { get; private set; }
    
    private void Awake()
    {
        IsCompleted = false;
    }

    public void LoadScene(List<string> scenes, List<string> unload_scenes)
    {
        m_load_scenes = scenes;
        m_unload_scenes = unload_scenes;
        StartCoroutine(Co_LoadSceneAsync());
    }

    IEnumerator Co_LoadSceneAsync()
    {
        yield return null; // 한박자 쉬고

        List<AsyncOperation> ops_list = new List<AsyncOperation>();

        // 씬 하나 완료당 증가할 progressbar 비율
        float rate = 1.0f / m_load_scenes.Count;

        foreach (var sceneName in m_load_scenes)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
            ops_list.Add(op);
            op.allowSceneActivation = false;    // 완료된 씬이 바로 로드되지 않도록 함

            float beforeProgress = 0.0f;
            while (!op.isDone)
            {
                yield return new WaitForSeconds(0.5f);

                //m_progressBar.value -= beforeProgress;
                //m_progressBar.value += op.progress;
                beforeProgress = op.progress;

                if (op.progress >= 0.9f)
                {                    
                    break;
                }
            }
        }


        foreach (var item in ops_list)
        {
            item.allowSceneActivation = true;
        }

        IsCompleted = true;
        m_loadCompleteEvent?.Invoke();        
    }

    public void Release()
    {
        foreach (var item in m_unload_scenes)
        {
            SceneManager.UnloadSceneAsync(item);
        }
    }

    public void StartEventStart()
    {
        var objs = GameObject.FindGameObjectsWithTag("SceneStart");
        foreach (var item in objs)
        {
            m_startEvent.AddListener(item.GetComponent<SceneStartEventReciever>().__Start);
        }
        m_startEvent?.Invoke();
    }
}
