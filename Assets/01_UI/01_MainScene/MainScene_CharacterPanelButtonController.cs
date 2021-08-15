using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void __OnSelectCompleteButton()
    {
        // ���� ������ �̵� �� �� (�ε� �� �θ���)
        List<string> loadScenes = new List<string>();
        List<string> unloadScenes = new List<string>();

        loadScenes.Add("Map");
        loadScenes.Add("UIScene");

        unloadScenes.Add("MainStartScene");

        SceneManager.LoadScene("LoaderScene",LoadSceneMode.Additive);

        SceneLoader.Instance.LoadScene(loadScenes, unloadScenes);
    }
}
