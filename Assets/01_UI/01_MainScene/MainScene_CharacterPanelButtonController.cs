using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainScene_CharacterPanelButtonController : MonoBehaviour
{
    [SerializeField] Image m_main_button_panel;

    // 선택 버튼 관련된 것은 character select manager 를 참조

    public void __OnBackButton()
    {
        m_main_button_panel.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void __OnSelectCompleteButton()
    {
        // 다음 씬으로 이동 할 것 (로딩 씬 부르기)
        List<string> loadScenes = new List<string>();
        List<string> unloadScenes = new List<string>();

        loadScenes.Add("Map");
        loadScenes.Add("UIScene");

        unloadScenes.Add("MainStartScene");

        SceneManager.LoadScene("LoaderScene",LoadSceneMode.Additive);

        SceneLoader.Instance.LoadScene(loadScenes, unloadScenes);
    }
}
