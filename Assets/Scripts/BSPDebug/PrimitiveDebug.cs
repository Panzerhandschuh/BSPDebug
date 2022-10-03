using System.Linq;
using LibBSP;
using UnityEngine;

public class PrimitiveDebug : MonoBehaviour
{
	public Primitive.PrimitiveType type;
	public int firstIndex;
	public int indexCount;
	public int firstVertex;
	public int vertexCount;

	private int[] indices;
	private Vector3[] vertices;

	public void Init(Primitive primitive)
	{
		type = primitive.Type;
		firstIndex = primitive.FirstIndex;
		indexCount = primitive.IndexCount;
		firstVertex = primitive.FirstVertex;
		vertexCount = primitive.VertexCount;

		indices = primitive.Indices.ToArray();
		vertices = primitive.Vertices.ToArray();
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		Gizmos.color = Color.white;
		for (var i = 0; i < indexCount; i += 3)
		{
			var v1 = vertices[indices[i]].SwizzleYZ();
			var v2 = vertices[indices[i + 1]].SwizzleYZ();
			var v3 = vertices[indices[i + 2]].SwizzleYZ();

			Gizmos.DrawLine(v1, v2);
			Gizmos.DrawLine(v2, v3);
			Gizmos.DrawLine(v3, v1);
		}
	}
}
