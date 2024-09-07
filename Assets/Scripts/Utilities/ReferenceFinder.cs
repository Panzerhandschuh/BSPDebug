using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReferenceFinder
{
	public static T Find<T>(Transform parent, int index) where T : MonoBehaviour
	{
		var obj = parent.Find($"{typeof(T).Name}_{index}");
		if (obj == null)
			return null;
		
		return obj.GetComponent<T>();
	}
}
