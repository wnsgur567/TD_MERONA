using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Monster_TableExcel
{
	public int No;
	public string Name_KR;
	public string Name_EN;
	public int Code;
	public int Move_Type;
	public float Atk;
	public float HP;
	public float Def;
	public int Shild;
	public float Move_spd;
	public int CC_Rgs1;
	public int CC_Rgs2;
	public int CC_Rgs3;
	public int Atk_Code;
	public int Skill1Code;
	public int Skill2Code;
	public float HPSkillCast;
	public int Prefeb;
}



//////////////////////////

[CreateAssetMenu(fileName = "Monster_TableLoader", menuName = "Scriptable Object/Monster_TableLoader")]
public class  Monster_TableExcelLoader : ScriptableObject
{
	[SerializeField] string filepath;
	public List<Monster_TableExcel> DataList;

	private Monster_TableExcel Read(string line)
	{
		line = line.TrimStart('\n');

		Monster_TableExcel data = new Monster_TableExcel();
		int idx = 0;
		string[] strs = line.Split('`');

		data.No = int.Parse(strs[idx++]);
		data.Name_KR = strs[idx++];
		data.Name_EN = strs[idx++];
		data.Code = int.Parse(strs[idx++]);
		data.Move_Type = int.Parse(strs[idx++]);
		data.Atk = float.Parse(strs[idx++]);
		data.HP = float.Parse(strs[idx++]);
		data.Def = float.Parse(strs[idx++]);
		data.Shild = int.Parse(strs[idx++]);
		data.Move_spd = float.Parse(strs[idx++]);
		data.CC_Rgs1 = int.Parse(strs[idx++]);
		data.CC_Rgs2 = int.Parse(strs[idx++]);
		data.CC_Rgs3 = int.Parse(strs[idx++]);
		data.Atk_Code = int.Parse(strs[idx++]);
		data.Skill1Code = int.Parse(strs[idx++]);
		data.Skill2Code = int.Parse(strs[idx++]);
		data.HPSkillCast = float.Parse(strs[idx++]);
		data.Prefeb = int.Parse(strs[idx++]);

		return data;
	}
	[ContextMenu("파일 읽기")]
	public void ReadAllFromFile()
	{
		DataList = new List<Monster_TableExcel>();

		string currentpath = System.IO.Directory.GetCurrentDirectory();
		string allText = System.IO.File.ReadAllText(System.IO.Path.Combine(currentpath, filepath));
		string[] strs = allText.Split(';');

		foreach (var item in strs)
		{
			if (item.Length < 2)
				continue;
			Monster_TableExcel data = Read(item);
			DataList.Add(data);
		}
	}
	}
