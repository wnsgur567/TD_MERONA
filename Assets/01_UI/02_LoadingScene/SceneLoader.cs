using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// ���� �۾����� Scene : A
// ������ �۾��ؾ� �� Scene : B , C

// A ���� SceneLoader �� ���Ե� Scene(LoaderScene)�� �ҷ�����
// SceneLoader�� ����Լ� LoadSecen �� ȣ���ϱ� (param ���� B , C �� �̸�)
// ������ �Ϸ�Ǹ�
// A �� Loader�� ���� �ݰ� ��
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
        yield return null; // �ѹ��� ����

        List<AsyncOperation> ops_list = new List<AsyncOperation>();

        // �� �ϳ� �Ϸ�� ������ progressbar ����
        float rate = 1.0f / m_load_scenes.Count;

        foreach (var sceneName in m_load_scenes)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            ops_list.Add(op);
            op.allowSceneActivation = false;    // �Ϸ�� ���� �ٷ� �ε���� �ʵ��� ��

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
