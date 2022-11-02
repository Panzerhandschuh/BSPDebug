using LibBSP;
using UnityEngine;

public class ModelDebug : MonoBehaviour, IDebugReference
{
    public Vector3 mins;
    public Vector3 maxs;
    public int faceIndex;
    public int numFaces;
    public int brushIndex;
    public int numBrushes;

	// Source only
	public Vector3 origin;
	public int headNodeIndex;

	public FaceDebug[] faceRefs;
    public BrushDebug[] brushRefs;
	public NodeDebug headNodeRef;

	public void Init(Model model)
	{
		mins = model.Minimums;
		maxs = model.Maximums;
		faceIndex = model.FirstFaceIndex;
		numFaces = model.NumFaces;
		brushIndex = model.FirstBrushIndex;
		numBrushes = model.NumBrushes;

		origin = model.Origin;
		headNodeIndex = model.HeadNodeIndex;
	}

	public void InitReferences()
	{
		faceRefs = new FaceDebug[numFaces];
		for (int i = 0; i < numFaces; i++)
			faceRefs[i] = ReferenceFinder.Find<FaceDebug>(transform.parent, faceIndex + i);

		if (numBrushes > 0)
		{
			brushRefs = new BrushDebug[numBrushes];
			for (int i = 0; i < numBrushes; i++)
				brushRefs[i] = ReferenceFinder.Find<BrushDebug>(transform.parent, brushIndex + i);
		}

		if (headNodeIndex > -1)
			headNodeRef = ReferenceFinder.Find<NodeDebug>(transform.parent, headNodeIndex);
	}

	private void OnDrawGizmosSelected()
	{
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.yellow);

		foreach (var face in faceRefs)
			face.DebugDraw();
	}
}
