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
        yield return null; // �ѹ��� ����

        List<AsyncOperation> ops_list = new List<AsyncOperation>();

        // �� �ϳ� �Ϸ�� ������ progressbar ����
        float rate = 1.0f / m_load_scenes.Count;

        foreach (var sceneName in m_load_scenes)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
            ops_list.Add(op);
            op.allowSceneActivation = false;    // �Ϸ�� ���� �ٷ� �ε���� �ʵ��� ��

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
