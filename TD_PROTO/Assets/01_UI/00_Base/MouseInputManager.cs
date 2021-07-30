using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public struct MouseArgs
{
    public Vector2 beforePos;
    public Vector2 currentPos;

    public bool isLeftDown;
    public bool isRightDown;
}

//Attach this script to your Canvas GameObject.
//Also attach a GraphicsRaycaster component to your canvas by clicking the Add Component button in the Inspector window.
//Also make sure you have an EventSystem in your hierarchy.
public class MouseInputManager : Singleton<MouseInputManager>
{
    [SerializeField] Canvas m_canvas;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    public delegate void MouseEventHandler(GameObject _selected, List<RaycastResult> results, MouseArgs args);
    public event MouseEventHandler MouseLeftDownEvent;
    public event MouseEventHandler MouseLeftDoubleDownEvent;
    public event MouseEventHandler MouseRightDownEvent;
    public event MouseEventHandler MouseLeftUpEvent;
    public event MouseEventHandler MouseRightUpEvent;
    public event MouseEventHandler MouseLeftHoldEvent;
    public event MouseEventHandler MouseRightHoldEvent;

    [SerializeField] GameObject current_gameObj;
    [SerializeField] MouseArgs current_args;

    [SerializeField] float current_mousedown_time;  // double click 체크용, 첫 down 과  두번째 down의 차이로 판별함
    [SerializeField] float tolerance;       // double click 으로 판단하는 시간 차
        

    private void Awake()
    {
        m_canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        current_args = new MouseArgs();
        current_mousedown_time = Time.time;
    }

    void Start()
    {
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = m_canvas.GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = m_canvas.GetComponent<EventSystem>();
    }

    void Update()
    {
        current_args.beforePos = current_args.currentPos;
        current_args.currentPos = Input.mousePosition;

        //Check if the left Mouse button is clicked
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            

            current_args.isLeftDown = true;
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);
                        

            if (results.Count > 0)
            {
                // 이전 클리과의 시간 차
                float delta = Time.time - current_mousedown_time;
                if (delta < tolerance)
                {   // call double click event
                    MouseLeftDoubleDownEvent?.Invoke(results[0].gameObject, results, current_args);
                    current_mousedown_time = Time.time;
                    return;
                }

                current_gameObj = results[0].gameObject;
                MouseLeftDownEvent?.Invoke(current_gameObj, results, current_args);
                current_mousedown_time = Time.time;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            current_args.isLeftDown = false;


            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            if (results.Count > 0)
            {
                MouseLeftUpEvent?.Invoke(results[0].gameObject, results, current_args);
            }

            current_gameObj = null;
        }

        //Debug.Log(current_args.isLeftDown);
        if (current_args.isLeftDown)
        {
            MouseLeftHoldEvent?.Invoke(current_gameObj, null, current_args);
        }
        if (current_args.isRightDown)
        {
            MouseRightHoldEvent?.Invoke(current_gameObj, null, current_args);
        }
    }
}
