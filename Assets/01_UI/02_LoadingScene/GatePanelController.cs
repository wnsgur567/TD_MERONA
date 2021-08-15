using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GatePanelController : MonoBehaviour
{
    [SerializeField] SceneLoader m_sceneLoader;

    [SerializeField] Image m_leftPanel;
    [SerializeField] Image m_rightPanel;
    [SerializeField] Image m_eyesPanel;

    [SerializeField] float m_speed;

    float m_leftWidth;
    float m_rightWidth;
    RectTransform left_rt;
    RectTransform right_rt;

    bool openProcessFlag;
    bool closeProcessFlag;

    private void Awake()
    {
        m_eyesPanel.gameObject.SetActive(false);

        left_rt = m_leftPanel.transform as RectTransform;
        right_rt = m_rightPanel.transform as RectTransform;

        m_leftWidth = left_rt.sizeDelta.x;
        m_rightWidth = right_rt.sizeDelta.x;

        left_rt.anchoredPosition = new Vector2(0, 0);
        right_rt.anchoredPosition = new Vector2(0, 0);

        openProcessFlag = true;
        closeProcessFlag = false;
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (openProcessFlag)
        {
            OpenGates();
        }
        if (closeProcessFlag)
        {
            CloseGates();
        }
    }


    private void OpenGates()
    {

        if (m_leftWidth - left_rt.anchoredPosition.x < 5f
            && -left_rt.anchoredPosition.x - m_rightWidth < 5f)
        {
            left_rt.anchoredPosition3D = new Vector3(m_leftWidth, 0, 0);
            right_rt.anchoredPosition3D = new Vector3(-m_rightWidth, 0, 0);

            openProcessFlag = false;
            m_eyesPanel.gameObject.SetActive(true);
            return;
        }

        left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(m_leftWidth, 0, 0), Time.deltaTime * m_speed);
        right_rt.anchoredPosition3D  = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(-m_rightWidth, 0, 0), Time.deltaTime * m_speed);
    }
    private void CloseGates()
    {
        if (left_rt.anchoredPosition.x < 5f
           && -right_rt.anchoredPosition.x < 5f)
        {
            left_rt.anchoredPosition = Vector2.zero;
            right_rt.anchoredPosition = Vector2.zero;

            m_eyesPanel.gameObject.SetActive(false);
            closeProcessFlag = false;

            m_sceneLoader.StartEventStart();
            m_sceneLoader.Release();

            return;
        }
        
        left_rt.anchoredPosition3D = Vector3.LerpUnclamped(left_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.deltaTime * m_speed);
        right_rt.anchoredPosition3D = Vector3.LerpUnclamped(right_rt.anchoredPosition3D, new Vector3(0, 0, 0), Time.deltaTime * m_speed);        
    }

    // event
    public void __OnSceneLoadCompleted()
    {
        closeProcessFlag = true;
    }
}
