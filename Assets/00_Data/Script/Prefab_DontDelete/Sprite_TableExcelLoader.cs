using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Sprite_TableExcel
{
    public int Code;
    public float Size;
    public string Unity_address;
}

[System.Serializable]
public class SpriteInfo
{
    public int Code;
    public GameObject obj;
}

//////////////////////////

[CreateAssetMenu(fileName = "Sprite_TableLoader", menuName = "Scriptable Object/Sprite_TableLoader")]
public class Sprite_TableExcelLoader : ScriptableObject
{
    [SerializeField] string filepath;
    public List<Prefab_TableExcel> DataList;
    [SerializeField] List<SpriteInfo> SpriteList;

    private Prefab_TableExcel Read(string line)
    {
        line = line.TrimStart('\n');

        Prefab_TableExcel data = new Prefab_TableExcel();
        int idx = 0;
        string[] strs = line.Split('`');

        data.Code = int.Parse(strs[idx++]);
        data.Size = float.Parse(strs[idx++]);
        data.Unity_address = strs[idx++];

        return data;
    }
    [ContextMenu("파일 읽기")]
    public void ReadAllFromFile()
    {
        DataList = new List<Prefab_TableExcel>();
        SpriteList = new List<SpriteInfo>();

        string currentpath = System.IO.Directory.GetCurrentDirectory();
        string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
        string[] strs = allText.Split(';');

        foreach (var item in strs)
        {
            if (item.Length < 2)
                continue;

            Prefab_TableExcel data = Read(item);
            DataList.Add(data);

            GameObject prefab_obj = Resources.Load<GameObject>(data.Unity_address);
            SpriteList.Add(new SpriteInfo()
            { Code = data.Code, obj = prefab_obj });
        }
    }

    public GameObject GetPrefab(int prefabCode)
    {
        Debug.Log(prefabCode);
        var info = SpriteList.Find((item) => { return item.Code == prefabCode; });
        return info.obj;
    }
}
