using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStartEventReciever : MonoBehaviour
{
    private void Awake()
    {
        this.tag = "SceneStart";
    }

    public void __Start()
    {
        Debug.Log("asdfasdf");
    }
}
