using System.Collections;
using System.Collections.Generic;
using LibBSP;
using UnityEngine;

public class NodeDebug : MonoBehaviour, IDebugReference
{
	public int plane; // Indicates the plane that slices this node into separate nodes/leaves
	public int[] children = new int[2];
    public Vector3 mins;
    public Vector3 maxs;

	// Source only
	public int firstFaceIndex;
	public int numFaces;
	public int area;

	public PlaneDebug planeRef;
	public GameObject child1Ref;
	public GameObject child2Ref;
	public FaceDebug[] faceRefs;

	public void Init(BSP bsp, Node node)
	{
		plane = node.PlaneIndex;
		children[0] = node.Child1Index;
		children[1] = node.Child2Index;
		mins = node.Minimums;
		maxs = node.Maximums;

		// Source only
		firstFaceIndex = node.FirstFaceIndex;
		numFaces = node.NumFaceIndices;
		area = node.AreaIndex;

		var bspPlane = bsp.Planes[plane];
		var norm = bspPlane.Normal.SwizzleYZ();
		transform.position = norm * bspPlane.Distance;
		transform.rotation = Quaternion.LookRotation(norm) * Quaternion.AngleAxis(90f, Vector3.right);
	}

	public void InitReferences()
	{
		planeRef = GameObject.Find($"PlaneDebug_{plane}").GetComponent<PlaneDebug>();
		child1Ref = FindChild(children[0]);
		child2Ref = FindChild(children[1]);

		if (numFaces > 0)
		{
			faceRefs = new FaceDebug[numFaces];
			for (int i = 0; i < numFaces; i++)
				faceRefs[i] = GameObject.Find($"{nameof(FaceDebug)}_{firstFaceIndex + i}").GetComponent<FaceDebug>();
		}
	}

	private GameObject FindChild(int childIndex)
	{
		if (childIndex > 0) // Node index
			return GameObject.Find($"{nameof(NodeDebug)}_{childIndex}");
		else // Leaf index
			return GameObject.Find($"{nameof(LeafDebug)}_{-(childIndex + 1)}");
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	private void DebugDraw()
	{
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.red);

		foreach (var faceRef in faceRefs)
			faceRef.DebugDraw();
	}
}
