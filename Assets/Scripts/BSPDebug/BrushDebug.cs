using LibBSP;
using UnityEngine;

public class BrushDebug : MonoBehaviour, IDebugReference
{
	public int brushSideIndex;
	public int numBrushSides;
	public int textureIndex; // Quake only
	public int contents; // Source only

	public BrushSideDebug[] brushSideRefs;
	public LeafDebug parentLeafRef;
	
    public void Init(Brush brush)
	{
		brushSideIndex = brush.FirstSideIndex;
		numBrushSides = brush.NumSides;
		textureIndex = brush.TextureIndex;
		contents = brush.Contents;
	}

	public void InitReferences()
	{
		brushSideRefs = new BrushSideDebug[numBrushSides];
		for (int i = 0; i < numBrushSides; i++)
			brushSideRefs[i] = ReferenceFinder.Find<BrushSideDebug>(transform.parent, brushSideIndex + i);
	}

	private void OnDrawGizmosSelected()
	{
		foreach (var brushSide in brushSideRefs)
			brushSide.DrawBrushSide();
	}
}
