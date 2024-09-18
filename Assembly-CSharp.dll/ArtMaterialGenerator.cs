using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020001F3 RID: 499
public class ArtMaterialGenerator : MonoBehaviour
{
	// Token: 0x06000B8C RID: 2956 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void GenerateMaterialsAndPresets()
	{
	}

	// Token: 0x06000B8D RID: 2957 RVA: 0x00002265 File Offset: 0x00000465
	public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
	{
	}

	// Token: 0x04000CB5 RID: 3253
	[Header("Pointers")]
	public string textureSourceDirectory;

	// Token: 0x04000CB6 RID: 3254
	public string materialOutputDirectory;

	// Token: 0x04000CB7 RID: 3255
	public string presetOutputDirectory;

	// Token: 0x04000CB8 RID: 3256
	public ArtPreset presetTemplate;

	// Token: 0x04000CB9 RID: 3257
	public Material materialTemplate;
}
