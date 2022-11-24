using LibBSP;
using UnityEngine;

public class BrushSideDebug : MonoBehaviour, IDebugReference
{
	public int planeIndex;
	public int textureIndex;
	public int dispInfo; // Source only
	public bool bevel; // Source only

	public PlaneDebug planeRef;
	public TextureDebug textureRef;
	public TextureInfoDebug textureInfoRef;

	private MapType bspMapType;

	public void Init(BSP bsp, BrushSide brushSide)
	{
		planeIndex = brushSide.PlaneIndex;
		textureIndex = brushSide.TextureIndex;
		dispInfo = brushSide.DisplacementIndex;
		bevel = brushSide.IsBevel;

		bspMapType = bsp.MapType;
	}

	public void InitReferences()
	{
		planeRef = ReferenceFinder.Find<PlaneDebug>(transform.parent, planeIndex);
		transform.position = planeRef.transform.position;
		transform.rotation = planeRef.transform.rotation;

		// TODO: Lookup textures from Textures lump for quake
		if (bspMapType.IsSubtypeOf(MapType.Quake3))
			textureRef = ReferenceFinder.Find<TextureDebug>(transform.parent, textureIndex);
		else if (bspMapType.IsSubtypeOf(MapType.Source))
		{
			if (textureIndex >= 0)
				textureInfoRef = ReferenceFinder.Find<TextureInfoDebug>(transform.parent, textureIndex);
		}
	}

	private void OnDrawGizmosSelected()
	{
		DrawBrushSide();
	}

	public void DrawBrushSide()
	{
		planeRef.DebugDraw();
	}
}