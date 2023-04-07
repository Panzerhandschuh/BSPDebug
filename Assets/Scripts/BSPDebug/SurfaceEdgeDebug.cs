using System;
using LibBSP;
using UnityEngine;

public class SurfaceEdgeDebug : MonoBehaviour, IDebugReference
{
    public int vertex1;
    public int vertex2;
	public bool isReversed;

	public VertexDebug vertexRef1;
	public VertexDebug vertexRef2;

	public void Init(BSP bsp, int edgeIndex)
	{
		var edge = bsp.Edges[Math.Abs(edgeIndex)];
		if (edgeIndex < 0) // Edge direction is probably used for generating triangles
		{
			vertex1 = edge.SecondVertexIndex;
			vertex2 = edge.FirstVertexIndex;
			isReversed = true;
		}
		else
		{
			vertex1 = edge.FirstVertexIndex;
			vertex2 = edge.SecondVertexIndex;
			isReversed = false;
		}
	}

	public void InitReferences()
	{
		vertexRef1 = ReferenceFinder.Find<VertexDebug>(transform.parent, vertex1);
		vertexRef2 = ReferenceFinder.Find<VertexDebug>(transform.parent, vertex2);
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vertexRef1.transform.position, vertexRef2.transform.position);

		//var arrowDir = vertexRef2.transform.position - vertexRef1.transform.position;
		//DebugExtension.DrawArrow(vertexRef1.transform.position, arrowDir.normalized * 10f, Color.green);
	}
}
