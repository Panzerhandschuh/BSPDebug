using LibBSP;
using UnityEngine;

public class ModelDebug : MonoBehaviour, IDebugReference
{
    public Vector3 mins;
    public Vector3 maxs;
    public int face;
    public int numFaces;
    public int brush;
    public int numBrushes;

	// Source only
	public Vector3 origin;
	public int headNode;

	public FaceDebug[] faceRefs;
    public BrushDebug[] brushRefs;
	public NodeDebug headNodeRef;

	public void Init(Model model)
	{
		mins = model.Minimums;
		maxs = model.Maximums;
		face = model.FirstFaceIndex;
		numFaces = model.NumFaces;
		brush = model.FirstBrushIndex;
		numBrushes = model.NumBrushes;

		origin = model.Origin;
		headNode = model.HeadNodeIndex;
	}

	public void InitReferences()
	{
		faceRefs = new FaceDebug[numFaces];
		for (int i = 0; i < numFaces; i++)
			faceRefs[i] = GameObject.Find($"{nameof(FaceDebug)}_{face + i}").GetComponent<FaceDebug>();

		if (numBrushes > 0)
		{
			brushRefs = new BrushDebug[numBrushes];
			for (int i = 0; i < numBrushes; i++)
				brushRefs[i] = GameObject.Find($"{nameof(BrushDebug)}_{brush + i}").GetComponent<BrushDebug>();
		}

		if (headNode > -1)
			headNodeRef = GameObject.Find($"{nameof(NodeDebug)}_{headNode}").GetComponent<NodeDebug>();
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
