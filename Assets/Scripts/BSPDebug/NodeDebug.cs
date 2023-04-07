using System.Collections;
using System.Collections.Generic;
using LibBSP;
using UnityEngine;

public class NodeDebug : MonoBehaviour, IDebugReference
{
	public int planeIndex; // Indicates the plane that slices this node into separate nodes/leaves
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
	public NodeDebug parentNodeRef;

	public void Init(Node node)
	{
		planeIndex = node.PlaneIndex;
		children[0] = node.Child1Index;
		children[1] = node.Child2Index;
		mins = node.Minimums;
		maxs = node.Maximums;

		// Source only
		firstFaceIndex = node.FirstFaceIndex;
		numFaces = node.NumFaceIndices;
		area = node.AreaIndex;

		var bspPlane = node.Parent.Bsp.Planes[planeIndex];
		var norm = bspPlane.Normal.SwizzleYZ();
		transform.position = norm * bspPlane.Distance;
		transform.rotation = Quaternion.LookRotation(norm) * Quaternion.AngleAxis(90f, Vector3.right);
	}

	public void InitReferences()
	{
		planeRef = ReferenceFinder.Find<PlaneDebug>(transform.parent, planeIndex);
		child1Ref = FindChild(children[0]);
		child2Ref = FindChild(children[1]);
		
		UpdateParentNodeReference(child1Ref);
		UpdateParentNodeReference(child2Ref);

		if (numFaces > 0)
		{
			faceRefs = new FaceDebug[numFaces];
			for (int i = 0; i < numFaces; i++)
				faceRefs[i] = ReferenceFinder.Find<FaceDebug>(transform.parent, firstFaceIndex + i);
		}
	}

	private void UpdateParentNodeReference(GameObject child)
	{
		var node = child.GetComponent<NodeDebug>();
		if (node != null)
			node.parentNodeRef = this;
		else
			child.GetComponent<LeafDebug>().parentNodeRef = this;
	}

	private GameObject FindChild(int childIndex)
	{
		if (childIndex > 0) // Node index
			return ReferenceFinder.Find<NodeDebug>(transform.parent, childIndex).gameObject;
		else // Leaf index
			return ReferenceFinder.Find<LeafDebug>(transform.parent, -(childIndex + 1)).gameObject;
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
