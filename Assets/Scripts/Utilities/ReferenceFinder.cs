using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ReferenceFinder
{
	public static T Find<T>(Transform parent, int index) where T : MonoBehaviour
	{
		return parent.Find($"{typeof(T).Name}_{index}").GetComponent<T>();
	}
}
