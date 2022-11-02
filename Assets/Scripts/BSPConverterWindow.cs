using UnityEditor;
using UnityEngine;

public class BSPLoaderWindow : EditorWindow
{
	[SerializeField]
	private string quakeFilePath;

	[SerializeField]
	private string outputDir;

	[MenuItem("BSP Loader/Loader")]
	public static void ShowWindow()
	{
		GetWindow(typeof(BSPLoaderWindow));
	}

	private void OnGUI()
	{
		GUILayout.Label("BSP Loader", EditorStyles.boldLabel);
		GUILayout.Label("Loads a Quake 3 or Source Engine BSP file.");
		GUILayout.Space(10);

		quakeFilePath = EditorGUILayout.TextField(quakeFilePath);
		if (GUILayout.Button("Browse"))
			quakeFilePath = EditorUtility.OpenFilePanel("Select BSP File", "", "bsp");
		
		GUILayout.Space(10);

		if (GUILayout.Button("Load BSP"))
			LoadBsp(quakeFilePath);

		GUILayout.Space(10);
	}

	private void LoadBsp(string path)
	{
		var bspLoader = new BSPLoader(path);
		bspLoader.Load();

		Debug.Log("Loaded BSP: " + path);
	}
}
