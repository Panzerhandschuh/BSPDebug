using LibBSP;
using UnityEngine;

public class BrushDebug : MonoBehaviour, IDebugReference
{
	public int brushSide;
	public int numBrushSides;
	public int texture; // Quake only
	public int contents; // Source only

	public BrushSideDebug[] brushSideRefs;
	
    public void Init(Brush brush)
	{
		brushSide = brush.FirstSideIndex;
		numBrushSides = brush.NumSides;
		texture = brush.TextureIndex;
		contents = brush.Contents;
	}

	public void InitReferences()
	{
		brushSideRefs = new BrushSideDebug[numBrushSides];
		for (int i = 0; i < numBrushSides; i++)
			brushSideRefs[i] = GameObject.Find($"{nameof(BrushSideDebug)}_{brushSide + i}").GetComponent<BrushSideDebug>();
	}

	private void OnDrawGizmosSelected()
	{
		foreach (var brushSide in brushSideRefs)
		{
			var plane = brushSide.planeRef.GetComponent<PlaneDebug>();
			Gizmos.DrawWireCube(plane.transform.position, Vector3.one * 10f);
		}
	}
}
