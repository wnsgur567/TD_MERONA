using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerToolTipRaycastChecker : MonoBehaviour
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
                Debug.Log($"mouse pos:{Input.mousePosition}");
                Vector3 mouse_pos = Input.mousePosition;
                mouse_pos.z = Camera.main.farClipPlane;

                int layermask = 1 << LayerMask.NameToLayer("Tower");
                RaycastHit hitinfo;
                Ray ray = new Ray(Camera.main.transform.position,
                    Camera.main.ScreenToWorldPoint(mouse_pos));

                Debug.Log($"{ray},{Camera.main.ScreenPointToRay(Input.mousePosition)}");

                if (Physics.Raycast(ray,
                    out hitinfo,
                    Camera.main.farClipPlane,
                   layermask))
                {
                    Tower hit_tower = hitinfo.collider.gameObject.GetComponent<Tower>();
                    // TODO : 타워 관리자에서 해당 타워가 사라질 경우 처리 수 있는 조치가 되어있어야 함!!!
                    TowerToolTipManager.Instance.ActivateToolTip(
                        hit_tower.transform.position,
                        hit_tower,
                        hit_tower.ExcelData);
                }
            }
        }           
    }
}
