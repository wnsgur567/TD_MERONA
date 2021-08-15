using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageMonster_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int SponPosition;
	public int Create_num;
	public int Monster_Code;
	public float AppearSpeed;
	public float CreateSpeed;
}



//////////////////////////

[CreateAssetMenu(fileName = "StageMonster_TableLoader", menuName = "Scriptable Object/StageMonster_TableLoader")]
public class  StageMonster_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<StageMonster_TableExcel> DataList;

	private StageMonster_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		StageMonster_TableExcel data = new StageMonster_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.SponPosition = int.Parse(strs[idx++]);
		data.Create_num = int.Parse(strs[idx++]);
		data.Monster_Code = int.Parse(strs[idx++]);
		data.AppearSpeed = float.Parse(strs[idx++]);
		data.CreateSpeed = float.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<StageMonster_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			StageMonster_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
}
