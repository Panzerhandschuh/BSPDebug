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
		Object.DestroyImmediate(GameObject.Find(bsp.MapName));
		worldRoot = new GameObject(bsp.MapName);

		LoadEntities(bsp.Entities);
		LoadTextures(bsp.Textures);
		if (bsp.MapType.IsSubtypeOf(MapType.Source))
		{
			LoadTextureInfo(bsp.TextureInfo);
			LoadTextureData(bsp.TextureData);
			LoadTextureDataStringTable(bsp.TextureTable);
			LoadEdges(bsp.FaceEdges);
		}
		LoadPlanes(bsp.Planes);
		LoadNodes(bsp.Nodes);
		LoadLeafs(bsp.Leaves);
		LoadLeafFaces(bsp.LeafFaces);
		if (bsp.MapType.IsSubtypeOf(MapType.Source) || bsp.MapType.IsSubtypeOf(MapType.Quake3))
		{
			LoadLeafBrushes(bsp.LeafBrushes);
			LoadBrushes(bsp.Brushes);
			LoadBrushSides(bsp.BrushSides);
			LoadVisibility(bsp.Visibility);
		}
		//LoadLeafBrushes(bsp.LeafBrushes);
		LoadModels(bsp.Models);
		LoadVertices(bsp.Vertices);
		//LoadMeshVerts(bsp.MeshVerts); // Ignore NumList
		//LoadEffects(bsp.Effects);
		LoadFaces(bsp.Faces);
		LoadLightmaps(bsp.Lightmaps);
		//LoadLightVols(bsp.);
		//if (bsp.VisibilityLoaded)
		if (bsp.MapType.IsSubtypeOf(MapType.Source))
		{
			LoadDisplacements(bsp.Displacements);
			LoadPrimitives(bsp.Primitives);
		}

		LoadReferences();

		// Clear vertex counter before loading next map
		FaceDebug.vertexCounter = 0;
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
			var instance = InstantiatePrefab<TextureDebug>(i);
			instance.Init(textures[i]);
		}
	}

	private void LoadTextureInfo(Lump<TextureInfo> textureInfo)
	{
		for (var i = 0; i < textureInfo.Count; i++)
		{
			var instance = InstantiatePrefab<TextureInfoDebug>(i);
			instance.Init(textureInfo[i]);
		}
	}

	private void LoadTextureData(Lump<TextureData> textureData)
	{
		for (var i = 0; i < textureData.Count; i++)
		{
			var instance = InstantiatePrefab<TextureDataDebug>(i);
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
			var instance = InstantiatePrefab<SurfaceEdgeDebug>(i);
			instance.Init(bsp, (int)faceEdges[i]);
		}
	}

	private void LoadPlanes(Lump<PlaneBSP> planes)
	{
		for (var i = 0; i < planes.Count; i++)
		{
			var instance = InstantiatePrefab<PlaneDebug>(i);
			instance.Init(planes[i]);
		}
	}

	private void LoadNodes(Lump<Node> nodes)
	{
		for (var i = 0; i < nodes.Count; i++)
		{
			var instance = InstantiatePrefab<NodeDebug>(i);
			instance.Init(nodes[i]);
		}
	}

	private void LoadLeafs(Lump<Leaf> leaves)
	{
		for (var i = 0; i < leaves.Count; i++)
		{
			var instance = InstantiatePrefab<LeafDebug>(i);
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
			var instance = InstantiatePrefab<ModelDebug>(i);
			instance.Init(models[i]);
		}
	}

	private void LoadBrushes(Lump<Brush> brushes)
	{
		for (var i = 0; i < brushes.Count; i++)
		{
			var instance = InstantiatePrefab<BrushDebug>(i);
			instance.Init(brushes[i]);
		}
	}

	private void LoadBrushSides(Lump<BrushSide> brushSides)
	{
		for (var i = 0; i < brushSides.Count; i++)
		{
			var instance = InstantiatePrefab<BrushSideDebug>(i);
			instance.Init(bsp, brushSides[i]);
		}
	}

	private void LoadVertices(Lump<UIVertex> vertices)
	{
		for (var i = 0; i < vertices.Count; i++)
		{
			var instance = InstantiatePrefab<VertexDebug>(i);
			instance.Init(vertices[i]);
		}
	}

	private void LoadFaces(Lump<Face> faces)
	{
		for (var i = 0; i < faces.Count; i++)
		{
			var instance = InstantiatePrefab<FaceDebug>(i);
			instance.Init(faces[i], i);
		}
	}

	private void LoadLightmaps(Lightmaps lightmaps)
	{
		var instance = InstantiatePrefab<LightmapDebug>(0);
		instance.Init(lightmaps);
	}

	private void LoadVisibility(Visibility visibility)
	{
		if (visibility.Data.Length == 0)
			return;

		var instance = worldRoot.AddComponent<VisDataDebug>();
		instance.Init(visibility);
	}

	private void LoadPrimitives(Lump<Primitive> primitives)
	{
		for (var i = 0; i < primitives.Count; i++)
		{
			var instance = InstantiatePrefab<PrimitiveDebug>(i);
			instance.Init(primitives[i]);
		}
	}

	private void LoadDisplacements(Lump<Displacement> displacements)
	{
		for (var i = 0; i < displacements.Count; i++)
		{
			var instance = InstantiatePrefab<DisplacementDebug>(i);
			instance.Init(displacements[i]);
		}
	}

	private void LoadReferences()
	{
		foreach (Transform child in worldRoot.transform)
		{
			var behaviour = child.GetComponent<MonoBehaviour>();
			if (behaviour is IDebugReference)
				((IDebugReference)behaviour).InitReferences();
		}
	}

	private T InstantiatePrefab<T>(int index) where T : Component
	{
		var prefabName = typeof(T).Name;
		var instance = Object.Instantiate(Resources.Load(prefabName)) as GameObject;
		instance.transform.SetParent(worldRoot.transform);
		instance.name = $"{prefabName}_{index}";

		return instance.GetComponent<T>();
	}
}
