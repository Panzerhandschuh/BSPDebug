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

	public GameObject planeRef;
	public GameObject child1Ref;
	public GameObject child2Ref;

	public void Init(BSP bsp, Node node)
	{
		plane = node.PlaneIndex;
		children[0] = node.Child1Index;
		children[1] = node.Child2Index;
		mins = node.Minimums;
		maxs = node.Maximums;

		var bspPlane = bsp.Planes[plane];
		var norm = bspPlane.normal.SwizzleYZ();
		transform.position = norm * bspPlane.distance;
		transform.rotation = Quaternion.LookRotation(norm) * Quaternion.AngleAxis(90f, Vector3.right);
	}

	public void InitReferences()
	{
		planeRef = GameObject.Find($"PlaneDebug_{plane}");
		child1Ref = FindChild(children[0]);
		child2Ref = FindChild(children[1]);
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
		var bounds = new Bounds();
		bounds.min = mins.SwizzleYZ();
		bounds.max = maxs.SwizzleYZ();

		DebugExtension.DrawBounds(bounds, Color.red);
	}
}
