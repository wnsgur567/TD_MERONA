using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;

// ���� �۾����� Scene : A
// ������ �۾��ؾ� �� Scene : B , C

// A ���� SceneLoader �� ���Ե� Scene(LoaderScene)�� �ҷ�����
// SceneLoader�� ����Լ� LoadSecen �� ȣ���ϱ� (param ���� B , C �� �̸�)
// ������ �Ϸ�Ǹ�
// A �� Loader�� ���� �ݰ� ��
public class SceneLoader : Singleton<SceneLoader>
{   

    [SerializeField] float delay_second = 3f;
    [SerializeField] float timer;

    private List<string> m_load_scenes;
    private List<string> m_unload_scenes;
        

    public bool IsCompleted { get; private set; }

    private void Awake()
    {
        IsCompleted = false;
        m_load_scenes = new List<string>();
        m_unload_scenes = new List<string>();
        timer = 0f;
    }

    public void SetLoadingSceneInfomation(List<string> scenes, List<string> unload_scenes)
    {
        foreach (var item in scenes)
        {
            m_load_scenes.Add(item);
        }

        foreach (var item in unload_scenes)
        {
            m_unload_scenes.Add(item);
        }       
    }

    public void LoadProcess(Action callback)
    {
        StartCoroutine(Co_LoadSceneAsync(callback));
    }

    IEnumerator Co_LoadSceneAsync(Action callback)
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
                     
            while (!op.isDone)
            {
                timer += Time.deltaTime;

                yield return null;

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
        callback?.Invoke();       
    }
   
    public void OnAllProcessCompleted()
    {
        var objs = GameObject.FindGameObjectsWithTag("SceneStart");
        foreach (var item in objs)
        {
            var reciever = item.GetComponent<SceneStartEventReciever>();
            reciever.__Start();
        }

        SceneManager.UnloadSceneAsync("LoaderScene");
    }

    public void UnloadScenes()
    {   
        foreach (var item in m_unload_scenes)
        {
            SceneManager.UnloadSceneAsync(item);
        }
    }   
}
