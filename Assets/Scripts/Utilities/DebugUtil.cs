using UnityEngine;

public static class DebugUtil
{
	public static void DrawPlane(Vector3 position, Vector3 normal, Vector2 size, float duration = 0f)
	{
		DrawPlane(position, Quaternion.LookRotation(normal), size, duration);
	}

	public static void DrawPlane(Vector3 position, Quaternion rotation, Vector2 size, float duration = 0f)
	{
		DrawPlane(position, rotation, size, Color.green, duration);
	}

	public static void DrawPlane(Vector3 position, Quaternion rotation, Vector2 size, Color color, float duration = 0f)
	{
		var up = rotation.GetUp() * size.y / 2f;
		var right = rotation.GetRight() * size.x / 2f;

		var corner0 = position - right - up;
		var corner1 = position - right + up;
		var corner2 = position + right - up;
		var corner3 = position + right + up;

		Debug.DrawLine(corner0, corner2, color, duration); // Bottom edge
		Debug.DrawLine(corner1, corner3, color, duration); // Top edge
		Debug.DrawLine(corner0, corner1, color, duration); // Left edge
		Debug.DrawLine(corner2, corner3, color, duration); // Right edge
		//Debug.DrawLine(corner0, corner3, color, duration); // Bottom left to top right cross
		//Debug.DrawLine(corner1, corner2, color, duration); // Top left to bottom right cross
		Debug.DrawRay(position, rotation.GetForward(), Color.red, duration);
	}
}
