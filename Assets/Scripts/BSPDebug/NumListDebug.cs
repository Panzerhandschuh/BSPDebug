using System.Collections;
using System.Collections.Generic;
using LibBSP;
using UnityEngine;

public class NumListDebug : MonoBehaviour
{
    public string listName;
	public List<int> list = new List<int>();

	public void Init(NumList numList)
	{
		foreach (var item in numList)
			list.Add((int)item);
	}
}
