using LibBSP;
using System.Collections.Generic;
using UnityEngine;

public class AreasDebug : MonoBehaviour
{
	public List<AreaDebug> areaDebugList;

	public void Init(Lump<Area> areas)
	{
		areaDebugList = new List<AreaDebug>();
		
		foreach (var area in areas)
		{
			var areaDebug = new AreaDebug();
			areaDebug.Init(area);
			
			areaDebugList.Add(areaDebug);
		}
	}
}
