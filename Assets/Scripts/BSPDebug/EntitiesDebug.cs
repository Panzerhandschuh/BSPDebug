using UnityEngine;

public class EntitiesDebug : MonoBehaviour
{
	[TextArea(minLines: 1, maxLines: 10)]
	public string entities;

	public void Init(string entities)
	{
		this.entities = entities;
	}
}
