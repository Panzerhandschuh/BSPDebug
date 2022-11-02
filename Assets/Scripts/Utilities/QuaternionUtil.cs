using UnityEngine;

public static class QuaternionUtil
{
	/// <summary>
	/// Returns the forward vector of a quaternion
	/// </summary>
	public static Vector3 GetForward(this Quaternion q)
	{
		return q * Vector3.forward;
	}

	/// <summary>
	/// Returns the up vector of a quaternion
	/// </summary>
	public static Vector3 GetUp(this Quaternion q)
	{
		return q * Vector3.up;
	}

	/// <summary>
	/// Returns the right vector of a quaternion
	/// </summary>
	public static Vector3 GetRight(this Quaternion q)
	{
		return q * Vector3.right;
	}
}
