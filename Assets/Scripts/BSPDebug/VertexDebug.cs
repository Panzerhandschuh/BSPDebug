using LibBSP;
using UnityEngine;

public class VertexDebug : MonoBehaviour
{
	public Vector3 position;
	public Vector2 texCoord;
	public Vector2 lightmapTexCoord;
	public Vector3 normal;
	public Color32 color;

	public void Init(UIVertex vertex)
	{
		position = vertex.position;
		texCoord = vertex.uv0;
		lightmapTexCoord = vertex.uv1;
		normal = vertex.normal;
		color = vertex.color;

		transform.position = position.SwizzleYZ();
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(position.SwizzleYZ(), Vector3.one * 5f);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(position.SwizzleYZ(), normal.SwizzleYZ() * 5f);
	}
}
