using UnityEngine;

public class PlaneDebug : MonoBehaviour
{
	public Vector3 normal;
	public float distance;
	public int type;

	public void Init(Plane plane)
	{
		normal = plane.normal;
		distance = plane.distance;
		//type = plane.type;
		
		var norm = normal.SwizzleYZ();
		transform.position = norm * distance;
		transform.rotation = Quaternion.LookRotation(norm) * Quaternion.AngleAxis(90f, Vector3.right);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.up * 10f);
	}
}
