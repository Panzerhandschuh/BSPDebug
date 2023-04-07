using LibBSP;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AreaDebug
{
	public int numAreaPortals;
	public int firstAreaPortal;

	public void Init(Area area)
	{
		numAreaPortals = area.NumAreaPortals;
		firstAreaPortal = area.FirstAreaPortal;
	}
}
