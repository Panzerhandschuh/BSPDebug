using LibBSP;
using UnityEngine;

public class TextureInfoDebug : MonoBehaviour, IDebugReference
{
	public Vector3[] textureVecs = new Vector3[2];
	public Vector3[] lightmapVecs = new Vector3[2];
	public int flags;
	public int textureDataIndex;

	public TextureDataDebug textureDataRef;

	public void Init(TextureInfo textureInfo)
	{
		textureVecs[0] = textureInfo.UAxis;
		textureVecs[1] = textureInfo.VAxis;
		lightmapVecs[0] = textureInfo.LightmapUAxis;
		lightmapVecs[1] = textureInfo.LightmapVAxis;
		flags = textureInfo.Flags;
		textureDataIndex = textureInfo.TextureIndex;
	}

	public void InitReferences()
	{
		if (textureDataIndex > -1)
			textureDataRef = GameObject.Find($"TextureDataDebug_{textureDataIndex}").GetComponent<TextureDataDebug>();
	}

	private void OnDrawGizmosSelected()
	{
		//var bounds = new Bounds();
		//bounds.min = textureVecs[0].SwizzleYZ() * 10f;
		//bounds.max = textureVecs[1].SwizzleYZ() * 10f;

		//DebugExtension.DebugBounds(bounds, Color.red);

		//Gizmos.color = Color.green;
		//Gizmos.DrawWireCube(textureVecs[0].SwizzleYZ() * 10f, Vector3.one * 2.5f);
		//Gizmos.DrawWireCube(textureVecs[1].SwizzleYZ() * 10f, Vector3.one * 2.5f);

		DebugExtension.DrawArrow(Vector3.zero, textureVecs[0].SwizzleYZ() * 10f, Color.green);
		DebugExtension.DrawArrow(Vector3.zero, textureVecs[1].SwizzleYZ() * 10f, Color.green);
	}
}
