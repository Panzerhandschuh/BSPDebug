using BSPConversionLib;
using UnityEditor;
using UnityEngine;

public class BSPConverterWindow : EditorWindow
{
	[SerializeField]
	private string quakeBspPath;

	[SerializeField]
    private string sourceBspPath;

	[SerializeField]
	private string outputPath;

	[MenuItem("BSP Converter/Convert")]
	public static void ShowWindow()
	{
		GetWindow(typeof(BSPConverterWindow));
	}

	private void OnGUI()
	{
		GUILayout.Label("BSP Converter", EditorStyles.boldLabel);
		GUILayout.Label("Converts a BSP from Quake 3 into Source engine.");
		GUILayout.Space(10);

		GUILayout.Label("Quake 3 BSP", EditorStyles.boldLabel);
		quakeBspPath = EditorGUILayout.TextField(quakeBspPath);
		if (GUILayout.Button("Browse"))
			quakeBspPath = EditorUtility.OpenFilePanel("Select BSP File", "", "bsp");
		
		GUILayout.Space(10);

		GUILayout.Label("Source BSP", EditorStyles.boldLabel);
		sourceBspPath = EditorGUILayout.TextField(sourceBspPath);
		if (GUILayout.Button("Browse"))
			sourceBspPath = EditorUtility.OpenFilePanel("Select BSP File", "", "bsp");
		
		GUILayout.Space(10);

		GUILayout.Label("Output File", EditorStyles.boldLabel);
		outputPath = EditorGUILayout.TextField(outputPath);
		if (GUILayout.Button("Browse"))
			outputPath = EditorUtility.SaveFilePanel("Select Output File", "", "", "bsp");
		
		GUILayout.Space(10);

		if (GUILayout.Button("Load Quake 3 BSP"))
			LoadBsp(quakeBspPath);

		if (GUILayout.Button("Load Source BSP"))
			LoadBsp(sourceBspPath);

		if (GUILayout.Button("Convert"))
			Convert();
	}

	private void LoadBsp(string path)
	{
		var bspLoader = new BSPLoader(path);
		bspLoader.Load();
	}

	private void Convert()
	{
		var converter = new BSPConverter(quakeBspPath, sourceBspPath, outputPath);
		converter.Convert();
	}
}
