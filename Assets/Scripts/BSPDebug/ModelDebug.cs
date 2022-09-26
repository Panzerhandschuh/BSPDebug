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

    public GameObject[] faceRefs;
    public GameObject[] brushRefs;

	public void Init(Model model)
	{
		mins = model.Minimums;
		maxs = model.Maximums;
		face = model.FirstFaceIndex;
		numFaces = model.NumFaces;
		brush = model.FirstBrushIndex;
		numBrushes = model.NumBrushes;
	}

	public void InitReferences()
	{
		faceRefs = new GameObject[numFaces];
		for (int i = 0; i < numFaces; i++)
			faceRefs[i] = GameObject.Find($"{nameof(FaceDebug)}_{face + i}");

		if (numBrushes > 0)
		{
			brushRefs = new GameObject[numBrushes];
			for (int i = 0; i < numBrushes; i++)
				brushRefs[i] = GameObject.Find($"{nameof(BrushDebug)}_{brush + i}");
		}
	}

	private void OnDrawGizmosSelected()
	{
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.yellow);
	}
}
