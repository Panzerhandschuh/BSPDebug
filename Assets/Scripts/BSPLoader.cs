using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LibBSP;
using UnityEngine;

public class BSPLoader
{
	private BSP bsp;
	private GameObject worldRoot;

	public BSPLoader(string path)
	{
		bsp = new BSP(new FileInfo(path));
	}

	public void Load()
	{
		Object.DestroyImmediate(GameObject.Find("World"));
		worldRoot = new GameObject("World");

		LoadEntities(bsp.Entities);
		LoadTextures(bsp.Textures);
		if (bsp.MapType.IsSubtypeOf(MapType.Source))
		{
			LoadTextureInfo(bsp.TextureInfo);
			LoadTextureData(bsp.TextureData);
			LoadTextureDataStringTable(bsp.TextureTable);
			LoadEdges(bsp.FaceEdges);
			//LoadOriginalFaces(bsp.OriginalFaces);
		}
		LoadPlanes(bsp.Planes);
		LoadNodes(bsp.Nodes);
		LoadLeafs(bsp.Leaves);
		LoadLeafFaces(bsp.LeafFaces);
		LoadLeafBrushes(bsp.LeafBrushes);
		LoadModels(bsp.Models);
		LoadBrushes(bsp.Brushes);
		LoadBrushSides(bsp.BrushSides);
		LoadVertices(bsp.Vertices);
		//LoadMeshVerts(bsp.MeshVerts); // Ignore NumList
		//LoadEffects(bsp.Effects);
		LoadFaces(bsp.Faces);
		//LoadLightmaps(bsp.Lightmaps);
		//LoadLightVols(bsp.);
		//if (bsp.VisibilityLoaded)
		LoadVisibility(bsp.Visibility);

		LoadReferences();
	}

	private void LoadEntities(Entities entities)
	{
		var instance = worldRoot.AddComponent<EntitiesDebug>();
		var sb = new StringBuilder();
		for (var i = 0; i < entities.Count; i++)
			sb.Append(entities[i].ToString());
		
		instance.Init(sb.ToString());
	}

	private void LoadTextures(Textures textures)
	{
		for (var i = 0; i < textures.Count; i++)
		{
			var instance = InstantiatePrefab<TextureDebug>("TextureDebug", i);
			instance.Init(textures[i]);
		}
	}

	private void LoadTextureInfo(Lump<TextureInfo> textureInfo)
	{
		for (var i = 0; i < textureInfo.Count; i++)
		{
			var instance = InstantiatePrefab<TextureInfoDebug>("TextureInfoDebug", i);
			instance.Init(textureInfo[i]);
		}
	}

	private void LoadTextureData(Lump<TextureData> textureData)
	{
		for (var i = 0; i < textureData.Count; i++)
		{
			var instance = InstantiatePrefab<TextureDataDebug>("TextureDataDebug", i);
			instance.Init(bsp, textureData[i]);
		}
	}

	private void LoadTextureDataStringTable(NumList textureTable)
	{
		var instance = worldRoot.AddComponent<TextureDataStringTableDebug>();
		instance.Init(bsp, textureTable);
	}

	private void LoadEdges(NumList faceEdges)
	{
		for (var i = 0; i < faceEdges.Count; i++)
		{
			var instance = InstantiatePrefab<SurfaceEdgeDebug>(nameof(SurfaceEdgeDebug), i);
			instance.Init(bsp, (int)faceEdges[i]);
		}
	}

	private void LoadPlanes(Lump<PlaneBSP> planes)
	{
		for (var i = 0; i < planes.Count; i++)
		{
			var instance = InstantiatePrefab<PlaneDebug>("PlaneDebug", i);
			instance.Init(planes[i]);
		}
	}

	private void LoadNodes(Lump<Node> nodes)
	{
		for (var i = 0; i < nodes.Count; i++)
		{
			var instance = InstantiatePrefab<NodeDebug>("NodeDebug", i);
			instance.Init(bsp, nodes[i]);
		}
	}

	private void LoadLeafs(Lump<Leaf> leaves)
	{
		for (var i = 0; i < leaves.Count; i++)
		{
			var instance = InstantiatePrefab<LeafDebug>("LeafDebug", i);
			instance.Init(leaves[i]);
		}
	}

	private void LoadLeafFaces(NumList leafFaces)
	{
		var instance = worldRoot.AddComponent<NumListDebug>();
		instance.listName = "Leaf Faces";
		instance.Init(leafFaces);
	}

	private void LoadLeafBrushes(NumList leafBrushes)
	{
		var instance = worldRoot.AddComponent<NumListDebug>();
		instance.listName = "Leaf Brushes";
		instance.Init(leafBrushes);
	}

	private void LoadModels(Lump<Model> models)
	{
		for (var i = 0; i < models.Count; i++)
		{
			var instance = InstantiatePrefab<ModelDebug>("ModelDebug", i);
			instance.Init(models[i]);
		}
	}

	private void LoadBrushes(Lump<Brush> brushes)
	{
		for (var i = 0; i < brushes.Count; i++)
		{
			var instance = InstantiatePrefab<BrushDebug>("BrushDebug", i);
			instance.Init(brushes[i]);
		}
	}

	private void LoadBrushSides(Lump<BrushSide> brushSides)
	{
		for (var i = 0; i < brushSides.Count; i++)
		{
			var instance = InstantiatePrefab<BrushSideDebug>("BrushSideDebug", i);
			instance.Init(bsp, brushSides[i]);
		}
	}

	private void LoadVertices(Lump<UIVertex> vertices)
	{
		for (var i = 0; i < vertices.Count; i++)
		{
			var instance = InstantiatePrefab<VertexDebug>("VertexDebug", i);
			instance.Init(vertices[i]);
		}
	}

	//private void LoadOriginalFaces(Lump<Face> originalFaces)
	//{
	//	for (var i = 0; i < originalFaces.Count; i++)
	//	{
	//		var instance = InstantiatePrefab<FaceDebug>("FaceDebug", i);
	//		instance.name = $"OriginalFaceDebug_{i}";
	//		instance.Init(bsp, originalFaces[i]);
	//	}
	//}

	private void LoadFaces(Lump<Face> faces)
	{
		for (var i = 0; i < faces.Count; i++)
		{
			var instance = InstantiatePrefab<FaceDebug>("FaceDebug", i);
			instance.Init(bsp, faces[i]);
		}
	}

	private void LoadVisibility(Visibility visibility)
	{
		if (visibility.Data.Length == 0)
			return;

		var instance = worldRoot.AddComponent<VisDataDebug>();
		instance.Init(visibility);
	}

	private void LoadReferences()
	{
		var debugRefs = Object.FindObjectsOfType<MonoBehaviour>().OfType<IDebugReference>();
		foreach (var debugRef in debugRefs)
			debugRef.InitReferences();
	}

	private T InstantiatePrefab<T>(string resourcePath, int index) where T : Component
	{
		var instance = Object.Instantiate(Resources.Load(resourcePath)) as GameObject;
		instance.transform.SetParent(worldRoot.transform);
		instance.name = $"{resourcePath}_{index}";

		return instance.GetComponent<T>();
	}
}
