using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DevilToolTipRaycastChecker : MonoBehaviour
{   
    // Update is called once per frame
    void Update()
    {
        //
        // check click world position (for summoned towers)
        // when tower hit by raycast
        // activate tooltip
        if (Input.GetMouseButtonDown(1))
        {
            /// perspective only
            // if mouse is out of UI 
            if (false == EventSystem.current.IsPointerOverGameObject())
            {
                // raycast ( mouse -> summoned tower)
                Vector3 mouse_pos = Input.mousePosition;
                mouse_pos.z = Camera.main.farClipPlane;

                int layermask = 1 << LayerMask.NameToLayer("Node");
                RaycastHit hitinfo;
                Ray ray = new Ray(Camera.main.transform.position,
                    Camera.main.ScreenToWorldPoint(mouse_pos));

                if (Physics.Raycast(ray,
                    out hitinfo,
                    Camera.main.farClipPlane,
                   layermask))
                {
                    Node hit_node = hitinfo.collider.gameObject.GetComponent<Node>();
                    Tower tower = hit_node.m_Tower;
                    if (tower != null && tower.Direction == E_Direction.None)
                        DevilToolTipManager.Instance.ActivateToolTip(
                            DevilManager.Instance.Devil.transform.position
                            );
                }
            }
        }
    }
}
