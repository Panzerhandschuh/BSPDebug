using LibBSP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipNodeDebug : MonoBehaviour, IDebugReference
{
	public int planeNum;
	public int child1;
	public int child2;

	public PlaneDebug planeRef;
	public ClipNodeDebug child1Ref;
	public ClipNodeDebug child2Ref;

	public void Init(ClipNode clipNode)
	{
		planeNum = clipNode.PlaneIndex;
		child1 = clipNode.Child1Index;
		child2 = clipNode.Child2Index;
	}

	public void InitReferences()
	{
		planeRef = ReferenceFinder.Find<PlaneDebug>(transform.parent, planeNum);
		transform.position = planeRef.transform.position;
		transform.rotation = planeRef.transform.rotation;

		if (child1 >= 0)
			child1Ref = ReferenceFinder.Find<ClipNodeDebug>(transform.parent, child1);
		
		if (child2 >= 0)
			child2Ref = ReferenceFinder.Find<ClipNodeDebug>(transform.parent, child2);
	}

	private void OnDrawGizmosSelected()
	{
		planeRef.DrawPlane();
		DrawClipNode(child1Ref, child2Ref);
	}

	private void DrawClipNode(ClipNodeDebug child1, ClipNodeDebug child2)
	{
		if (child1 != null)
		{
			child1.planeRef.DrawPlane();
			child1.DrawClipNode(child1.child1Ref, child1.child2Ref);
		}

		if (child2 != null)
		{
			child2.planeRef.DrawPlane();
			child2.DrawClipNode(child2.child1Ref, child2.child2Ref);
		}
	}
}
