using LibBSP;
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

	public VertexDebug[] vertexRefs;
	public VertexDebug[] meshVertexRefs;
	public PlaneDebug planeRef;
	public SurfaceEdgeDebug[] edgeRefs;
	public TextureInfoDebug textureInfoRef;
	public PrimitiveDebug[] primitiveRefs;
	public Texture2D lightmapTexture;

	private NumList meshVerts;

	public void Init(Face face)
	{
		if (face.Parent.Bsp.MapType.IsSubtypeOf(MapType.Quake3))
			InitQuake3(face);
		else if (face.Parent.Bsp.MapType.IsSubtypeOf(MapType.Source))
			InitSource(face);
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

		meshVerts = face.Parent.Bsp.Indices;
	}

	private void InitSource(Face face)
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

		// TODO: Source BSP version 20 uses lump 53 instead?
		InitLightmapSource(face);
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
		if (firstVertexIndex > -1)
		{
			vertexRefs = new VertexDebug[numVertices];
			for (var i = 0; i < numVertices; i++)
				vertexRefs[i] = GameObject.Find($"VertexDebug_{firstVertexIndex + i}").GetComponent<VertexDebug>();
		}

		if (firstMeshVertexIndex > -1)
		{
			meshVertexRefs = new VertexDebug[numMeshVertices];
			for (var i = 0; i < numMeshVertices; i++)
			{
				var meshVertIndex = (int)meshVerts[firstMeshVertexIndex + i];
				meshVertexRefs[i] = GameObject.Find($"VertexDebug_{meshVertIndex}").GetComponent<VertexDebug>();
			}
		}

		if (planeIndex > -1)
			planeRef = GameObject.Find($"PlaneDebug_{planeIndex}").GetComponent<PlaneDebug>();

		if (firstEdgeIndex > -1)
		{
			edgeRefs = new SurfaceEdgeDebug[numEdges];
			for (var i = 0; i < numEdges; i++)
				edgeRefs[i] = GameObject.Find($"{nameof(SurfaceEdgeDebug)}_{firstEdgeIndex + i}").GetComponent<SurfaceEdgeDebug>();
		}

		if (textureInfoIndex > -1)
		{
			var textureInfoObj = GameObject.Find($"TextureInfoDebug_{textureInfoIndex}");
			if (textureInfoObj != null)
				textureInfoRef = textureInfoObj.GetComponent<TextureInfoDebug>();
		}

		if (firstPrimitiveId > -1)
		{
			primitiveRefs = new PrimitiveDebug[numPrimitives];
			for (var i = 0; i < numPrimitives; i++)
				primitiveRefs[i] = GameObject.Find($"{nameof(PrimitiveDebug)}_{firstPrimitiveId + i}").GetComponent<PrimitiveDebug>();
		}
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		DebugDrawQuake3();
		DebugDrawSource();
	}

	private void DebugDrawQuake3()
	{
		// Mesh vertices
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

		// Surface edges
		//Gizmos.color = Color.white;
		//for (var i = 0; i < numVertices; i++)
		//{
		//	var v1 = vertexRefs[i];
		//	var v2 = vertexRefs[(i + 1) % numVertices];

		//	Gizmos.DrawLine(v2.transform.position, v1.transform.position);
		//}
	}

	private void DebugDrawSource()
	{
		foreach (var edgeRef in edgeRefs)
			edgeRef.DebugDraw();

		foreach (var primitive in primitiveRefs)
			primitive.DebugDraw();
	}
}
