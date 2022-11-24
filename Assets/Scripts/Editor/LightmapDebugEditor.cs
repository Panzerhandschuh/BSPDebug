using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LightmapDebug))]
public class LightmapDebugEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if (GUILayout.Button("Export Lightmap"))
			ExportLightmap();
	}

	private void ExportLightmap()
	{
		var pngPath = EditorUtility.SaveFilePanel("Export Lightmap", "", "", "png");
		if (!string.IsNullOrEmpty(pngPath))
		{
			var lightmapDebug = (LightmapDebug)target;
			for (var i = 0; i < lightmapDebug.lightmapTextures.Count; i++)
			{
				var lightmapPath = pngPath.Replace(".png", $"_{i}.png");
				var bytes = lightmapDebug.lightmapTextures[i].EncodeToPNG();
				
				System.IO.File.WriteAllBytes(lightmapPath, bytes);
			}
		}
	}
}
