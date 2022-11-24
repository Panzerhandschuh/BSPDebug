using LibBSP;
using UnityEngine;

public class PlaneDebug : MonoBehaviour
{
	public Vector3 normal;
	public float distance;
	public int type;

	public void Init(PlaneBSP plane)
	{
		normal = plane.Normal;
		distance = plane.Distance;
		type = plane.Type;
		
		var norm = normal.SwizzleYZ();
		transform.position = norm * distance;
		transform.rotation = Quaternion.LookRotation(norm) * Quaternion.AngleAxis(90f, Vector3.right);
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw();
	}

	public void DebugDraw()
	{
		DebugUtil.DrawPlane(transform.position, normal.SwizzleYZ(), new Vector2(25f, 25f));
	}
}
