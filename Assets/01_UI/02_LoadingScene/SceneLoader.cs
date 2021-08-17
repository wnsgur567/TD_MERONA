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
    public UnityEvent m_load_complete_event;
    public UnityEvent m_all_complete_event;

    [SerializeField] float delay_second = 3f;
    [SerializeField] float timer;

    private List<string> m_load_scenes;
    private List<string> m_unload_scenes;
    //[SerializeField] Slider m_progressBar;

    public bool IsCompleted { get; private set; }

    private void Awake()
    {
        IsCompleted = false;
        m_load_scenes = new List<string>();
        m_unload_scenes = new List<string>();
        timer = 0f;
    }

    private void Start()
    {
        m_all_complete_event.AddListener(UnloadThisScene);
    }

    public void LoadScene(List<string> scenes, List<string> unload_scenes)
    {
        foreach (var item in scenes)
        {
            m_load_scenes.Add(item);
        }

        foreach (var item in unload_scenes)
        {
            m_unload_scenes.Add(item);
        }

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
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            ops_list.Add(op);
            op.allowSceneActivation = false;    // 완료된 씬이 바로 로드되지 않도록 함

            float beforeProgress = 0.0f;
            while (!op.isDone)
            {
                timer += Time.deltaTime;

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

        if(timer < delay_second)
            yield return new WaitForSeconds(delay_second);

        IsCompleted = true;
        m_load_complete_event?.Invoke();
    }
   
    public void OnAllCompleted()
    {
        m_all_complete_event?.Invoke();
    }

    private void UnloadThisScene()
    {
        SceneManager.UnloadSceneAsync("LoaderScene");
    }

    public void UnloadScenes()
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
            var reciever = item.GetComponent<SceneStartEventReciever>();
            reciever.__Start();
        }
    }
}
