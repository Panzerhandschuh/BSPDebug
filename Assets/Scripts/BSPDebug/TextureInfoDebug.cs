using LibBSP;
using UnityEngine;

public class TextureInfoDebug : MonoBehaviour, IDebugReference
{
	public Vector3[] textureVecs = new Vector3[2];
	public Vector2 textureTranslation;
	public Vector3[] lightmapVecs = new Vector3[2];
	public Vector2 lightmapTranslation;
	public int flags;
	public int textureDataIndex;

	public TextureDataDebug textureDataRef;

	public void Init(TextureInfo textureInfo)
	{
		textureVecs[0] = textureInfo.UAxis;
		textureVecs[1] = textureInfo.VAxis;
		textureTranslation = textureInfo.Translation;
		lightmapVecs[0] = textureInfo.LightmapUAxis;
		lightmapVecs[1] = textureInfo.LightmapVAxis;
		lightmapTranslation = textureInfo.LightmapTranslation;
		flags = textureInfo.Flags;
		textureDataIndex = textureInfo.TextureIndex;
	}

	public void InitReferences()
	{
		if (textureDataIndex > -1)
			textureDataRef = ReferenceFinder.Find<TextureDataDebug>(transform.parent, textureDataIndex);
	}

	private void OnDrawGizmosSelected()
	{
		DebugDraw(Vector3.zero);
	}

	public void DebugDraw(Vector3 position)
	{
		//var bounds = new Bounds();
		//bounds.min = textureVecs[0].SwizzleYZ() * 10f;
		//bounds.max = textureVecs[1].SwizzleYZ() * 10f;

		//DebugExtension.DebugBounds(bounds, Color.red);

		//Gizmos.color = Color.green;
		//Gizmos.DrawWireCube(textureVecs[0].SwizzleYZ() * 10f, Vector3.one * 2.5f);
		//Gizmos.DrawWireCube(textureVecs[1].SwizzleYZ() * 10f, Vector3.one * 2.5f);

		DebugExtension.DrawArrow(position, textureVecs[0].SwizzleYZ() * 2.5f, Color.green);
		DebugExtension.DrawArrow(position, textureVecs[1].SwizzleYZ() * 2.5f, Color.green);
	}
}
