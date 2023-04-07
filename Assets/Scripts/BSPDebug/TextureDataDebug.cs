using LibBSP;
using UnityEngine;

public class TextureDataDebug : MonoBehaviour
{
	public Color32 reflectivity;
	public int nameStringTableId;
	public Vector2 size;
	public Vector2 viewSize;

	public string textureName;

    public void Init(BSP bsp, TextureData textureData)
	{
		reflectivity = textureData.Reflectivity;
		nameStringTableId = textureData.TextureStringOffsetIndex;
		size = textureData.Size;
		viewSize = textureData.ViewSize;

		var textureDataStringTableOffset = (uint)bsp.TextureTable[nameStringTableId];
		textureName = bsp.Textures.GetTextureAtOffset(textureDataStringTableOffset);
	}
}
