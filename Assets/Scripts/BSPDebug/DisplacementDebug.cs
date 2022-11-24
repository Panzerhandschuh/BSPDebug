using LibBSP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class DisplacementDebug : MonoBehaviour, IDebugReference
{
	[Serializable]
	public class DisplacementNeighborDebug
	{
		[Serializable]
		public class DisplacementSubNeighborDebug
		{
			public int neighbor; // Index into dispinfo (0xFFFF if there is no neighbor)
			public int neighborOrientation; // (CCW) rotation wrt this displacement
			public int span;
			public int neighborSpan;

			public DisplacementSubNeighborDebug(Displacement.DisplacementNeighbor.DisplacementSubNeighbor subNeighbor)
			{
				neighbor = subNeighbor.NeighborIndex;
				neighborOrientation = subNeighbor.Orientation;
				span = subNeighbor.Span;
				neighborSpan = subNeighbor.NeighborSpan;
			}
		}

		public DisplacementSubNeighborDebug[] subNeighbors;

		public DisplacementNeighborDebug(Displacement.DisplacementNeighbor neighbor)
		{
			subNeighbors = new DisplacementSubNeighborDebug[2];

			for (var i = 0; i < neighbor.Subneighbors.Length; i++)
				subNeighbors[i] = new DisplacementSubNeighborDebug(neighbor.Subneighbors[i]);
		}
	}

	[Serializable]
	public class DisplacementCornerNeighborDebug
	{
		public int[] neighbors;
		public int numNeighbors;

		public DisplacementCornerNeighborDebug(Displacement.DisplacementCornerNeighbor cornerNeighbor)
		{
			neighbors = cornerNeighbor.NeighborIndices;
			numNeighbors = cornerNeighbor.NumNeighbors;
		}
	}

	public Vector3 startPosition;
	public int dispVertStart;
	public int dispTriStart;
	public int power;
	public int minTesselation;
	public float smoothingAngle;
	public int contents;
	public ushort faceIndex;
	public int lightmapAlphaStart;
	public int lightmapSamplePositionStart;
	public DisplacementNeighborDebug[] edgeNeighbors;
	public DisplacementCornerNeighborDebug[] cornerNeighbors;
	public uint[] allowedVerts;

	private DisplacementVertex[] dispVerts;
	public ushort[] dispTris;

	public FaceDebug faceRef;

	public void Init(Displacement displacement)
	{
		startPosition = displacement.StartPosition;
		dispVertStart = displacement.FirstVertexIndex;
		dispTriStart = displacement.FirstTriangleIndex;
		power = displacement.Power;
		minTesselation = displacement.MinimumTesselation;
		smoothingAngle = displacement.SmoothingAngle;
		contents = displacement.Contents;
		faceIndex = (ushort)displacement.FaceIndex;
		lightmapAlphaStart = displacement.LightmapAlphaStart;
		lightmapSamplePositionStart = displacement.LightmapSamplePositionStart;

		edgeNeighbors = new DisplacementNeighborDebug[4];
		for (var i = 0; i < displacement.Neighbors.Length; i++)
			edgeNeighbors[i] = new DisplacementNeighborDebug(displacement.Neighbors[i]);

		cornerNeighbors = new DisplacementCornerNeighborDebug[4];
		for (var i = 0; i < displacement.CornerNeighbors.Length; i++)
			cornerNeighbors[i] = new DisplacementCornerNeighborDebug(displacement.CornerNeighbors[i]);

		allowedVerts = displacement.AllowedVertices;

		dispVerts = displacement.Vertices.ToArray();
		dispTris = displacement.Triangles.ToArray();

		transform.position = startPosition.SwizzleYZ();
	}

	private void OnDrawGizmosSelected()
	{
		foreach (var vert in dispVerts)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(startPosition.SwizzleYZ() + vert.Normal.SwizzleYZ() * vert.Magnitude, Vector3.one * 5f);
		}

		faceRef.DebugDraw();
	}

	public void InitReferences()
	{
		faceRef = ReferenceFinder.Find<FaceDebug>(transform.parent, faceIndex);
	}
}
