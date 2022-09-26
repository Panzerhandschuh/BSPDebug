using UnityEngine;

public static class Vector3Extensions
{
	/// <summary>
	/// Swaps the Y and Z coordinates of a Vector, converting between Quake's Z-Up coordinates to Unity's Y-Up.
	/// </summary>
	public static Vector3 SwizzleYZ(this Vector3 v)
	{
		return new Vector3(v.x, v.z, v.y);
	}
}
