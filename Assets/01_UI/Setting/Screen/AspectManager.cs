using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Size
{
    public int width;
    public int height;

    public Size(int width, int height)
    {
        this.width = width;
        this.height = height;
    }
}


public enum E_UISpaceType
{
    Equal,      // target �� ������ ������
    LetterBox,  // target ���� ���� canvas �� ���Ϸ� �� �� (���Ʒ��� letter box)
    PillarBox,  // target ���� ���� canvas �� �¿�� �� �� (�¿�� pillar box)
}

public class AspectManager : Singleton<AspectManager>
{
    Canvas m_main_canvas = null;
    RectTransform m_main_canvas_rt = null;

    const float m_tolerance = 0.0001f;   // ��� ����
    [SerializeField] E_UISpaceType m_UISpaceType;

    //// resoultion   // ���� preference ���� �����´ٴ� ��
    //[SerializeField] int m_target_width;    // target resoultion width
    //[SerializeField] int m_target_height;   // target resoultion height

    [SerializeField] float m_target_ratio;      // target resolution ����
    [SerializeField] float m_current_ratio;     // ���� ����� ���� ����(canvas ����)

    [SerializeField] Size m_main_panel_size;            // unity size ����
    [SerializeField] Vector2 m_main_panel_sizeDelta;    // ���� resolution ���� ����
    [SerializeField] Size m_canvas_size;                // unity size ����
    [SerializeField] Vector2 m_canvas_sizeDelta;        // ���� resolution ���� ����    

    [SerializeField] float m_scale;

    public E_UISpaceType GetSpaceType()
    {
        return m_UISpaceType;
    }
    public float GetCanvasRatio()
    {
        return m_current_ratio;
    }
    public float GetMainPanelRatio()
    {
        return m_target_ratio;
    }
    public Size GetCanvasSize()
    {
        return m_canvas_size;
    }
    public Size GetMainPanelSize()
    {
        return m_main_panel_size;
    }
    public Vector2 GetCanvasSizeDelta()
    {
        return m_canvas_sizeDelta;
    }
    public Vector2 GetMainPanelSizeDelta()
    {
        return m_main_panel_sizeDelta;
    }

    private void Awake()
    {
        m_main_canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        m_main_canvas_rt = m_main_canvas.GetComponent<RectTransform>();        
    }

    private void Start()
    {        
        //SettingManager.Instance.OnSettingChnagedEvent += __Init;

        //__Init();
    }

    private void Update()
    {
//#if UNITY_EDITOR
//        __Init(SettingManager.Instance.GetCurrentSetting());
//#endif
    }

    public void __Init(SettingArgs e)
    {
        Vector2 resolution = ScreenDataUtility.GetResoultion(e.screen_data);        
        m_target_ratio = resolution.x / resolution.y;
        m_current_ratio = Camera.main.aspect;   // ī�޶� ���� == ĵ���� ����
        m_canvas_size = new Size((int)m_main_canvas.pixelRect.width,
                                        (int)m_main_canvas.pixelRect.height);
        m_canvas_sizeDelta = new Vector2(m_main_canvas_rt.rect.width, m_main_canvas_rt.rect.height);

        //m_canvas_sizeDelta =

        // ��� ���� ���̸� ������ ������ �Ǵ���
        float _diff = m_current_ratio - m_target_ratio;
        if (_diff < m_tolerance && _diff > -m_tolerance)
        {
            m_main_panel_size = new Size(m_canvas_size.width, m_canvas_size.height);
            m_main_panel_sizeDelta = new Vector2(m_canvas_sizeDelta.x, m_canvas_sizeDelta.y);

            m_UISpaceType = E_UISpaceType.Equal;
            OnChangedAspectCallBack?.Invoke();
            return;
        }


        if (m_current_ratio > m_target_ratio)
        {   // ���� view �� target resolution ���� �¿�� �� �� ���
            m_UISpaceType = E_UISpaceType.PillarBox;

            float _width = m_canvas_size.width * (m_target_ratio / m_current_ratio);
            float _height = m_canvas_size.height;

            m_main_panel_size = new Size((int)_width, (int)_height);            
            float _delta_width = m_canvas_sizeDelta.x * (m_target_ratio / m_current_ratio);
            float _delta_height = m_canvas_sizeDelta.y;

            m_main_panel_sizeDelta = new Vector2(_delta_width, _delta_height);
            
            OnChangedAspectCallBack?.Invoke();
        }
        else
        {   // ���Ϸ� �� �� ���
            m_UISpaceType = E_UISpaceType.LetterBox;

            float _width = m_canvas_size.width;
            float _height = m_canvas_size.height * (m_current_ratio / m_target_ratio);

            m_main_panel_size = new Size((int)_width, (int)_height);
          
            float _delta_width = m_canvas_sizeDelta.x;
            float _delta_height = m_canvas_sizeDelta.y * (m_current_ratio / m_target_ratio);
            m_main_panel_sizeDelta = new Vector2(_delta_width, _delta_height);
           
            OnChangedAspectCallBack?.Invoke();
        }

        //Debug.LogFormat("panel size : {0} , {1}", m_main_panel_size.width, m_main_panel_size.height);
        //Debug.LogFormat("Left Top : {0} , {1}", m_main_panel_leftTop_position.x, m_main_panel_leftTop_position.y);
    }


    public delegate void OnChangedAspect();

    public OnChangedAspect OnChangedAspectCallBack;
}
