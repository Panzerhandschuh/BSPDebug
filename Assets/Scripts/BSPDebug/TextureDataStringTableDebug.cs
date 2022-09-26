using LibBSP;
using UnityEngine;

public class TextureDataStringTableDebug : MonoBehaviour
{
	public string[] textures;

	public void Init(BSP bsp, NumList textureDataStringTable)
	{
		textures = new string[textureDataStringTable.Count];
		for (var i = 0; i < textureDataStringTable.Count; i++)
			textures[i] = bsp.Textures.GetTextureAtOffset((uint)textureDataStringTable[i]);
	}
}
