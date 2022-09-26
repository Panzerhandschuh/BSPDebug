using UnityEngine;

public class TextureDebug : MonoBehaviour
{
	public string textureName;
	public int flags;
	public int contents;

	public void Init(LibBSP.Texture texture)
	{
		textureName = texture.Name;
		flags = texture.Flags;
		contents = texture.Contents;
	}
}
