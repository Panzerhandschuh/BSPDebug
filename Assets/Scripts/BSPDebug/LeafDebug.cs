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
	//public int leafWaterDataId;

	public FaceDebug[] leafFaceRefs;
	public BrushDebug[] leafBrushRefs;

	private NumList leafFaces;
	private NumList leafBrushes;

	public void Init(BSP bsp, Leaf leaf)
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
		//leafWaterDataId = leaf.LeafWaterDataID;

		leafFaces = bsp.LeafFaces;
		leafBrushes = bsp.LeafBrushes;
	}

	public void InitReferences()
	{
		if (numLeafFaces > 0)
		{
			leafFaceRefs = new FaceDebug[numLeafFaces];
			for (int i = 0; i < numLeafFaces; i++)
			{
				var faceIndex = (int)leafFaces[firstLeafFace + i];
				leafFaceRefs[i] = GameObject.Find($"{nameof(FaceDebug)}_{faceIndex}").GetComponent<FaceDebug>();
			}
		}

		if (numLeafBrushes > 0)
		{
			leafBrushRefs = new BrushDebug[numLeafBrushes];
			for (int i = 0; i < numLeafBrushes; i++)
			{
				var brushIndex = (int)leafBrushes[firstLeafBrush + i];
				leafBrushRefs[i] = GameObject.Find($"{nameof(BrushDebug)}_{brushIndex}").GetComponent<BrushDebug>();
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.red);
	}
}
