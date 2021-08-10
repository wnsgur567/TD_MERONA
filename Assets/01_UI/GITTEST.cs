using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GITTEST : MonoBehaviour
{
    [SerializeField] Prefab_TableExcelLoader loader;

    private void Start()
    {
        GameObject origin = loader.GetPrefab(100000);

        Debug.Log(origin.name);
    }
}
