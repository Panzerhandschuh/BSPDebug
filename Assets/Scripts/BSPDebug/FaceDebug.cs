using LibBSP;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FaceDebug : MonoBehaviour, IDebugReference
{
	public enum FaceType
	{
		Polygon = 1,
		Patch,
		Mesh,
		Billboard
	}

	// Quake 3 format
	public int texture;
	public int effect;
	public FaceType type;
	public int firstVertexIndex; // When type is polygon, use this to generate surfaces edges
	public int numVertices;
	public int firstMeshVertexIndex;
	public int numMeshVertices;
	public int lightmapIndex;
	public Vector2 lightmapStart;
	public Vector2 lightmapSize;
	public Vector3 lightmapOrigin;
	public Vector3[] lightmapVecs = new Vector3[2];
	public Vector3 normal;
	public Vector2 size;
	public Vector2 qLightmapStart;
	public Vector2 qLightmapEnd;

	// Source format
	public int planeIndex;
	public bool side;
	public bool onNode;
	public int firstEdgeIndex;
	public int numEdges;
	public int textureInfoIndex;
	public int dispInfo;
	public int surfaceFogVolumeId;
	public byte[] styles = new byte[4];
	public int lightOffset;
	public float area;
	public Vector2 lightmapTextureMinsInLuxels;
	public Vector2 lightmapTextureSizeInLuxels;
	public int originalFace;
	public int numPrimitives;
	public int firstPrimitiveId;
	public int smoothingGroups;

	public Vector3[] vertexNormals;

	// Lightmap uvs for each face edge vertex
	public List<Vector2> lightmapTexCoords;

	public VertexDebug[] vertexRefs;
	public VertexDebug[] meshVertexRefs;
	public PlaneDebug planeRef;
	public SurfaceEdgeDebug[] edgeRefs;
	public TextureInfoDebug textureInfoRef;
	public PrimitiveDebug[] primitiveRefs;
	public Texture2D lightmapTexture;

	private MapType mapType;
	private Lump<UIVertex> vertices;
	private NumList meshVerts;

	public static int vertexCounter = 0; // Used for tracking vertex normals

	public void Init(Face face, int faceIndex)
	{
		mapType = face.Parent.Bsp.MapType;
		if (mapType.IsSubtypeOf(MapType.Quake3))
			InitQuake3(face);
		else if (mapType.IsSubtypeOf(MapType.Source))
			InitSource(face, faceIndex);
	}

	private void InitQuake3(Face face)
	{
		texture = face.TextureIndex;
		effect = face.Effect;
		type = (FaceType)face.Type;
		firstVertexIndex = face.FirstVertexIndex;
		numVertices = face.NumVertices;
		firstMeshVertexIndex = face.FirstIndexIndex;
		numMeshVertices = face.NumIndices;
		lightmapIndex = face.Lightmap;
		lightmapStart = face.LightmapStart;
		lightmapSize = face.LightmapSize;
		lightmapOrigin = face.LightmapOrigin;
		lightmapVecs[0] = face.LightmapUAxis;
		lightmapVecs[1] = face.LightmapVAxis;
		normal = face.Normal;
		size = face.PatchSize;

		vertices = face.Parent.Bsp.Vertices;
		meshVerts = face.Parent.Bsp.Indices;

		InitLightmapQuake3(face);
	}

	private void InitSource(Face face, int faceIndex)
	{
		planeIndex = face.PlaneIndex;
		side = face.PlaneSide;
		onNode = face.IsOnNode;
		firstEdgeIndex = face.FirstEdgeIndexIndex;
		numEdges = face.NumEdgeIndices;
		textureInfoIndex = face.TextureInfoIndex;
		dispInfo = face.DisplacementIndex;
		surfaceFogVolumeId = face.SurfaceFogVolumeID;
		styles = face.LightmapStyles;
		lightOffset = face.Lightmap;
		area = face.Area;
		lightmapTextureMinsInLuxels = face.LightmapStart;
		lightmapTextureSizeInLuxels = face.LightmapSize;
		originalFace = face.OriginalFaceIndex;
		numPrimitives = face.NumPrimitives;
		firstPrimitiveId = face.FirstPrimitive;
		smoothingGroups = face.SmoothingGroups;

		vertexNormals = new Vector3[numEdges];
		for (var i = 0; i < numEdges; i++)
		{
			var normalIndex = (int)face.Parent.Bsp.Indices[vertexCounter];
			vertexNormals[i] = face.Parent.Bsp.Normals[normalIndex];

			vertexCounter++;
		}

		// TODO: Source BSP version 20 uses lump 53 instead?
		InitLightmapSource(face);
		ComputeLightmapTexCoords(face);
	}

	private void InitLightmapSource(Face face)
	{
		var lightmap = face.Parent.Bsp.Lightmaps;
		if (lightmap.Data.Length == 0)
			return;
		
		var lightmapSize = new Vector2(face.LightmapSize.x + 1, face.LightmapSize.y + 1);
		var offset = face.Lightmap;
		lightmapTexture = new Texture2D((int)lightmapSize.x, (int)lightmapSize.y);

		for (var x = 0; x < (int)lightmapSize.x; x++)
		{
			for (var y = 0; y < (int)lightmapSize.y; y++)
			{
				var index = x + y * (int)lightmapSize.x;
				var r = lightmap.Data[offset + index * 4 + 0];
				var g = lightmap.Data[offset + index * 4 + 1];
				var b = lightmap.Data[offset + index * 4 + 2];
				var exp = (sbyte)lightmap.Data[offset + index * 4 + 3];

				var color = new Color(
					TexLightToLinear(r, exp),
					TexLightToLinear(g, exp),
					TexLightToLinear(b, exp),
					1f);
				//var color = new Color32(r, g, b, 255);
				lightmapTexture.SetPixel(x, y, color);
			}
		}

		lightmapTexture.Apply();
	}

	private void ComputeLightmapTexCoords(Face face)
	{
		var bsp = face.Parent.Bsp;
		var texInfo = bsp.TextureInfo[face.TextureInfoIndex];
		var lightmapUAxis = texInfo.LightmapUAxis;
		var lightmapVAxis = texInfo.LightmapVAxis;
		var lightmapTranslation = texInfo.LightmapTranslation;

		var lightmapStart = face.LightmapStart;
		var lightmapSize = face.LightmapSize;

		foreach (var edgeIndex in face.EdgeIndices)
		{
			var edge = bsp.Edges[Mathf.Abs(edgeIndex)];
			var vertex = edge.FirstVertex.position;

			var s = (Vector3.Dot(vertex, lightmapUAxis) + lightmapTranslation.x - lightmapStart.x) / lightmapSize.x;
			var t = (Vector3.Dot(vertex, lightmapVAxis) + lightmapTranslation.y - lightmapStart.y) / lightmapSize.y;

			lightmapTexCoords.Add(new Vector2(s, t));
		}
	}

	private void InitLightmapQuake3(Face face)
	{
		if (face.Lightmap < 0)
			return;

		var lightmap = face.Parent.Bsp.Lightmaps;

		const int lmSize = 128;
		var lightmapTotalSize = lmSize * lmSize * 3;
		var lmOffset = face.Lightmap * lightmapTotalSize;

		var uvMin = new Vector2(float.MaxValue, float.MaxValue);
		var uvMax = new Vector2(float.MinValue, float.MinValue);
		var vertices = face.Vertices.ToArray();
		foreach (var vert in vertices)
		{
			if (vert.uv1.x < uvMin.x)
				uvMin.x = vert.uv1.x;
			if (vert.uv1.y < uvMin.y)
				uvMin.y = vert.uv1.y;

			if (vert.uv1.x > uvMax.x)
				uvMax.x = vert.uv1.x;
			if (vert.uv1.y > uvMax.y)
				uvMax.y = vert.uv1.y;
		}

		var lmStart = new Vector2Int((int)Math.Floor(uvMin.x * lmSize), (int)Math.Floor(uvMin.y * lmSize));
		var lmEnd = new Vector2Int((int)Math.Ceiling(uvMax.x * lmSize), (int)Math.Ceiling(uvMax.y * lmSize));
		var lmTexSize = lmEnd - lmStart;

		qLightmapStart = lmStart;
		qLightmapEnd = lmEnd;

		if (lmTexSize.x <= 0 || lmTexSize.y <= 0 ||
			lmTexSize.x > lmSize || lmTexSize.y > lmSize)
		{
			Debug.Log("Invalid lightmap size: " + lmTexSize);
			return;
		}

		lightmapTexture = new Texture2D(lmTexSize.x, lmTexSize.y);

		for (var x = lmStart.x; x < lmEnd.x; x++)
		{
			for (var y = lmStart.y; y < lmEnd.y; y++)
			{
				var index = x + y * lmSize;
				var r = lightmap.Data[lmOffset + index * 3 + 0];
				var g = lightmap.Data[lmOffset + index * 3 + 1];
				var b = lightmap.Data[lmOffset + index * 3 + 2];

				var color = new Color32(r, g, b, 255);
				lightmapTexture.SetPixel(x - lmStart.x, y - lmStart.y, color);
			}
		}

		lightmapTexture.Apply();
	}

	private float TexLightToLinear(byte color, int exponent)
	{
		return color * (Mathf.Pow(2, exponent) / 255f);
	}

	private int GetNumLightStyles(Face face)
	{
		const int maxLightStyles = 4;
		
		int lightstyles;
		for (lightstyles = 0; lightstyles < maxLightStyles; lightstyles++)
		{
			if (face.LightmapStyles[lightstyles] == 255)
				break;
		}

		return lightstyles;
	}

	public void InitReferences()
	{
		if (mapType.IsSubtypeOf(MapType.Source))
		{
			if (planeIndex > -1)
				planeRef = ReferenceFinder.Find<PlaneDebug>(transform.parent, planeIndex);

			if (firstEdgeIndex > -1)
			{
				edgeRefs = new SurfaceEdgeDebug[numEdges];
				for (var i = 0; i < numEdges; i++)
					edgeRefs[i] = ReferenceFinder.Find<SurfaceEdgeDebug>(transform.parent, firstEdgeIndex + i);
			}

			if (textureInfoIndex > -1)
			{
				textureInfoRef = ReferenceFinder.Find<TextureInfoDebug>(transform.parent, textureInfoIndex);
			}

			if (firstPrimitiveId > -1)
			{
				primitiveRefs = new PrimitiveDebug[numPrimitives];
				for (var i = 0; i < numPrimitives; i++)
					primitiveRefs[i] = ReferenceFinder.Find<PrimitiveDebug>(transform.parent, firstPrimitiveId + i);
			}
		}
		else
		{
			if (firstVertexIndex > -1)
			{
				vertexRefs = new VertexDebug[numVertices];
				for (var i = 0; i < numVertices; i++)
					vertexRefs[i] = ReferenceFinder.Find<VertexDebug>(transform.parent, firstVertexIndex + i);
			}

			if (firstMeshVertexIndex > -1)
			{
				meshVertexRefs = new VertexDebug[numMeshVertices];
				for (var i = 0; i < numMeshVertices; i++)
				{
					var meshVertIndex = (int)meshVerts[firstMeshVertexIndex + i];
					meshVertexRefs[i] = ReferenceFinder.Find<VertexDebug>(transform.parent, meshVertIndex);
				}
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		if (mapType.IsSubtypeOf(MapType.Source))
			DebugDrawSource();
		else
			DebugDrawQuake3();
	}

	private void DebugDrawSource()
	{
		//planeRef.DebugDraw();

		DrawTextureInfo();

		foreach (var edgeRef in edgeRefs)
			edgeRef.DebugDraw();

		foreach (var primitive in primitiveRefs)
			primitive.DebugDraw();
	}

	private void DrawTextureInfo()
	{
		var centerPos = Vector3.zero;
		foreach (var edge in edgeRefs)
			centerPos += edge.vertexRef1.position.SwizzleYZ();

		centerPos /= edgeRefs.Length;
		textureInfoRef.DebugDraw(centerPos);
	}

	private void DebugDrawQuake3()
	{
		if (type == FaceType.Polygon)
			DebugDrawQuake3Polygon();
		else if (type == FaceType.Patch)
			DebugDrawQuake3Patch();

		// Surface edges
		//Gizmos.color = Color.white;
		//for (var i = 0; i < numVertices; i++)
		//{
		//	var v1 = vertexRefs[i];
		//	var v2 = vertexRefs[(i + 1) % numVertices];

		//	Gizmos.DrawLine(v2.transform.position, v1.transform.position);
		//}
	}

	private void DebugDrawQuake3Polygon()
	{
		Gizmos.color = Color.white;
		for (var i = 0; i < numMeshVertices; i += 3)
		{
			var v1 = vertexRefs[meshVerts[firstMeshVertexIndex + i]];
			var v2 = vertexRefs[meshVerts[firstMeshVertexIndex + i + 1]];
			var v3 = vertexRefs[meshVerts[firstMeshVertexIndex + i + 2]];

			Gizmos.DrawLine(v1.transform.position, v2.transform.position);
			Gizmos.DrawLine(v2.transform.position, v3.transform.position);
			Gizmos.DrawLine(v3.transform.position, v1.transform.position);
		}
	}

	private void DebugDrawQuake3Patch()
	{
		Gizmos.color = Color.green;

		for (var y = 0; y < size.y - 1; y += 2)
		{
			for (var x = 0; x < size.x - 1; x += 2)
			{
				var patchStartVertex = firstVertexIndex + x + y * (int)size.x;
				DrawPatch(patchStartVertex);
			}
		}
	}

	private void DrawPatch(int patchStartVertex)
	{
		var dispPower = 3; // Displacement power
		var subdiv = (1 << dispPower) + 1;
		for (var i = 0; i < subdiv; i++)
		{
			var widthT = i / (subdiv - 1f);
			for (var j = 0; j < subdiv; j++)
			{
				var heightT = j / (subdiv - 1f);

				var bp = BezierPatch(patchStartVertex, widthT, heightT, (int)size.x);
				Gizmos.DrawWireCube(bp.SwizzleYZ(), Vector3.one * 5f);
			}
		}
	}

	private Vector3 BezierPatch(int firstVertex, float widthT, float heightT, int patchWidth)
	{
		var bi = Bezier(widthT);
		var bj = Bezier(heightT);

		var result = Vector3.zero;
		for (var i = 0; i < 3; i++)
		{
			for (var j = 0; j < 3; j++)
			{
				result += vertices[firstVertex + i + j * patchWidth].position * bi[i] * bj[j];
			}
		}

		return result;
	}

	private float[] Bezier(float t)
	{
		return new float[]
		{
			(1f - t) * (1f - t),
			2f * t * (1f - t),
			t * t
		};
	}
}
