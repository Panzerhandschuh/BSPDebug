using LibBSP;
using UnityEngine;

public class LeafDebug : MonoBehaviour, IDebugReference
{
	public int contents; // Set to 0 when inside of map, 1 when outside (Source only)
	public int cluster;
	public int area;
	public int flags;
	public Vector3 mins;
	public Vector3 maxs;
	public int firstLeafFace;
	public int numLeafFaces;
	public int firstLeafBrush;
	public int numLeafBrushes;
	public int leafWaterDataId;

	public FaceDebug[] leafFaceRefs;
	public BrushDebug[] leafBrushRefs;

	private NumList leafFaces;
	private NumList leafBrushes;

	public void Init(Leaf leaf)
	{
		contents = leaf.Contents;
		cluster = leaf.Visibility;
		area = leaf.Area;
		flags = leaf.Flags;
		mins = leaf.Minimums;
		maxs = leaf.Maximums;
		firstLeafFace = leaf.FirstMarkFaceIndex;
		numLeafFaces = leaf.NumMarkFaceIndices;
		firstLeafBrush = leaf.FirstMarkBrushIndex;
		numLeafBrushes = leaf.NumMarkBrushIndices;
		leafWaterDataId = leaf.LeafWaterDataID;

		leafFaces = leaf.Parent.Bsp.LeafFaces;
		leafBrushes = leaf.Parent.Bsp.LeafBrushes;
	}

	public void InitReferences()
	{
		if (numLeafFaces > 0)
		{
			leafFaceRefs = new FaceDebug[numLeafFaces];
			for (int i = 0; i < numLeafFaces; i++)
			{
				var faceIndex = (int)leafFaces[firstLeafFace + i];
				leafFaceRefs[i] = ReferenceFinder.Find<FaceDebug>(transform.parent, faceIndex);
			}
		}

		if (numLeafBrushes > 0)
		{
			leafBrushRefs = new BrushDebug[numLeafBrushes];
			for (int i = 0; i < numLeafBrushes; i++)
			{
				var brushIndex = (int)leafBrushes[firstLeafBrush + i];
				leafBrushRefs[i] = ReferenceFinder.Find<BrushDebug>(transform.parent, brushIndex);
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.red);

		foreach (var faceRef in leafFaceRefs)
			faceRef.DebugDraw();
	}
}
