using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterBoxManager : MonoBehaviour
{

    [SerializeField] Image m_main_panel = null;

    [SerializeField] bool m_letterbox_drawing_flag; // ���� �߶󳻱�
    [SerializeField] bool m_pillarbox_drawing_flag; // �¿� �߶󳻱�

    [SerializeField] Image Up = null;
    [SerializeField] Image Down = null;
    [SerializeField] Image Left = null;
    [SerializeField] Image Right = null;

    [SerializeField] E_UISpaceType m_UISpaceType;

    private void Awake()
    {
        m_main_panel = GameObject.FindWithTag("MainPanel").GetComponent<Image>();
    }

    void Start()
    {
        AspectManager.Instance.OnChangedAspectCallBack += __InitBoxes;
    }


    void Update()
    {

#if UNITY_EDITOR
        // test
        __InitBoxes();
#endif
    }

    public void __InitBoxes()
    {
        SetDrawFlag();
        SetBoxPivot();
        SetBoxAnckor();
        SetBoxPosition();
        SetBoxSize();
    }

    void SetDrawFlag()
    {
        Up.gameObject.SetActive(false);
        Down.gameObject.SetActive(false);
        Left.gameObject.SetActive(false);
        Right.gameObject.SetActive(false);

        m_UISpaceType = AspectManager.Instance.GetSpaceType();
        float _canvas_ratio = AspectManager.Instance.GetCanvasRatio();
        float _mainpanel_ratio = AspectManager.Instance.GetMainPanelRatio();

        switch (m_UISpaceType)
        {
            case E_UISpaceType.Equal:
                {
                    m_letterbox_drawing_flag = false;
                    m_pillarbox_drawing_flag = false;
                }
                break;
            case E_UISpaceType.LetterBox:
                {// Canvas �� main panel ���� �¿�� �� �� ���
                 // �� �쿡 pillar box�� �����ϵ��� ����
                    m_letterbox_drawing_flag = true;
                    m_pillarbox_drawing_flag = false;
                }
                break;
            case E_UISpaceType.PillarBox:
                {// Canvas �� main panel ���� ���Ϸ� �� ����
                 // �� �Ͽ� letter box�� �����ϵ��� ����                    
                    m_letterbox_drawing_flag = false;
                    m_pillarbox_drawing_flag = true;
                }
                break;
        }


        if (m_letterbox_drawing_flag)
        {   // letter box �� �׷��� �ϴ� ��� 
            // ���� �̹��� on
            Up.gameObject.SetActive(true);
            Down.gameObject.SetActive(true);
        }
        if (m_pillarbox_drawing_flag)
        {   // pillar box �� �׷��� �ϴ� ��� 
            // �¿� on
            Left.gameObject.SetActive(true);
            Right.gameObject.SetActive(true);
        }
    }

    // letter box �� pillar box�� PIVOT�� �����մϴ�
    void SetBoxPivot()
    {
        // up pivot : ��� �߰�
        Up.rectTransform.pivot = new Vector2(0.5f, 1.0f);
        // down pivot : �ϴ� �߰�
        Down.rectTransform.pivot = new Vector2(0.5f, 0.0f);
        // left pivot : ���� �߰�
        Left.rectTransform.pivot = new Vector2(0.0f, 0.5f);
        // right pivot : ���� �߰�
        Right.rectTransform.pivot = new Vector2(1.0f, 0.5f);
    }

    // letter box �� pillar box�� ANCKOR�� �����մϴ�
    void SetBoxAnckor()
    {
        // up Anckor
        Up.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        Up.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
        // down Anckor
        Down.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        Down.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
        // left anckor
        Left.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
        Left.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
        // right ankor
        Right.rectTransform.anchorMin = new Vector2(1.0f, 0.0f);
        Right.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
    }

    // letter box �� pillar box�� Position�� �����մϴ�
    void SetBoxPosition()
    {
        Up.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        Down.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        Left.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        Right.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
    }

    // letter box �� pillar box�� Size�� �����մϴ�
    void SetBoxSize()
    {
        Vector2 _canvas_size = AspectManager.Instance.GetCanvasSizeDelta();
        Vector2 _panel_size = AspectManager.Instance.GetMainPanelSizeDelta();

        // main panel ������ ����       
        //Debug.LogFormat("panel size : {0}, {1}", _panel_size.x, _panel_size.y);

        m_main_panel.rectTransform.anchoredPosition3D = new Vector3(0, 0, 0);
        m_main_panel.rectTransform.sizeDelta = new Vector2(_panel_size.x, _panel_size.y);


        // ������ �ڽ� ������ ����
        switch (m_UISpaceType)
        {
            case E_UISpaceType.Equal:
                // nothing
                break;
            case E_UISpaceType.LetterBox:
                {
                    float _size = (_canvas_size.y - _panel_size.y) / 2;
                    Up.rectTransform.sizeDelta = new Vector2(0, _size);
                    Down.rectTransform.sizeDelta = new Vector2(0, _size);
                }
                break;
            case E_UISpaceType.PillarBox:
                {
                    float _size = (_canvas_size.x - _panel_size.x) / 2;
                    Left.rectTransform.sizeDelta = new Vector2(_size, 0);
                    Right.rectTransform.sizeDelta = new Vector2(_size, 0);
                }
                break;
        }
    }
}
