using System.Collections.Generic;
using LibBSP;
using UnityEngine;

public class LightmapDebug : MonoBehaviour
{
	public List<Texture2D> lightmapTextures;

	private const int lmSize = 128;

	public void Init(Lightmaps lightmap)
	{
		if (lightmap.Bsp.MapType.IsSubtypeOf(MapType.Quake3))
			InitQuake3(lightmap);
	}

	private void InitQuake3(Lightmaps lightmap)
	{
		var lightmapTotalSize = lmSize * lmSize * 3;
		var numLightmaps = lightmap.Data.Length / lightmapTotalSize;

		for (var i = 0; i < numLightmaps; i++)
		{
			var lightmapTexture = new Texture2D(lmSize, lmSize);
			var offset = i * lightmapTotalSize;

			for (var x = 0; x < lmSize; x++)
			{
				for (var y = 0; y < lmSize; y++)
				{
					var index = x + y * lmSize;
					var color = new Color32(
						lightmap.Data[offset + index * 3 + 0],
						lightmap.Data[offset + index * 3 + 1],
						lightmap.Data[offset + index * 3 + 2],
						255);
					lightmapTexture.SetPixel(x, y, color);
				}
			}

			lightmapTexture.Apply();
			lightmapTextures.Add(lightmapTexture);
		}
	}
}
