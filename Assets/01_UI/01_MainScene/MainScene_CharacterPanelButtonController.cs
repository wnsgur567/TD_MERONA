using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene_CharacterPanelButtonController : MonoBehaviour
{
    [SerializeField] Image m_main_button_panel;

    // ���� ��ư ���õ� ���� character select manager �� ����

    public void __OnBackButton()
    {        
        m_main_button_panel.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void __OnSelectCompleteButton(Button button)
    {
        EventSystem.current.SetSelectedGameObject(null);
        button.enabled = false;
        StartCoroutine(LoadLoadingScene());
    }

    IEnumerator LoadLoadingScene()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("LoaderScene", LoadSceneMode.Additive);
        //op.allowSceneActivation = false;    // �Ϸ�� ���� �ٷ� �ε���� �ʵ��� ��

        float beforeProgress = 0.0f;
        while (!op.isDone)
        {
            yield return new WaitForSeconds(0.5f);

            //m_progressBar.value -= beforeProgress;
            //m_progressBar.value += op.progress;
            beforeProgress = op.progress;

            if (op.progress >= 1.0f)
            {
                break;
            }
        }


        // ���� ������ �̵� �� �� (�ε� �� �θ���)
        List<string> loadScenes = new List<string>();
        List<string> unloadScenes = new List<string>();

        loadScenes.Add("Map");
        loadScenes.Add("UIScene");
        unloadScenes.Add("MainStartScene");

        var obj = GameObject.FindWithTag("SceneLoader");
        var sceneLoader = obj.GetComponent<SceneLoader>();
        sceneLoader.SetLoadingSceneInfomation(loadScenes, unloadScenes);

    }
}
