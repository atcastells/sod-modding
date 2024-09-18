using System;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000205 RID: 517
public class MaterialCreator : MonoBehaviour
{
	// Token: 0x06000BCB RID: 3019 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CreateMaterial()
	{
	}

	// Token: 0x06000BCC RID: 3020 RVA: 0x00002265 File Offset: 0x00000465
	public static void SetTextureImporterFormat(Texture2D texture, bool isReadable)
	{
	}

	// Token: 0x06000BCD RID: 3021 RVA: 0x000AAF14 File Offset: 0x000A9114
	private float GetPixel(Texture2D tex, int x, int y)
	{
		return tex.GetPixel(x, y).grayscale;
	}

	// Token: 0x06000BCE RID: 3022 RVA: 0x000AAF34 File Offset: 0x000A9134
	public T SafeDestroyGameObject<T>(T component) where T : Component
	{
		if (component != null)
		{
			this.SafeDestroy<GameObject>(component.gameObject);
		}
		return default(T);
	}

	// Token: 0x06000BCF RID: 3023 RVA: 0x000AAF6C File Offset: 0x000A916C
	public T SafeDestroy<T>(T obj) where T : Object
	{
		if (Application.isEditor)
		{
			Object.DestroyImmediate(obj);
		}
		else
		{
			Object.Destroy(obj);
		}
		return default(T);
	}

	// Token: 0x04000D2B RID: 3371
	[Tooltip("Removes the mesh collider")]
	public bool removeCollider;

	// Token: 0x04000D2C RID: 3372
	[Tooltip("If true then add an interactable controller to this object")]
	public bool addInteractableController;

	// Token: 0x04000D2D RID: 3373
	[Tooltip("Duplicate the diffuse map as a normal map. This will not happen if a separate normal map is found.")]
	public bool duplicateDiffuseAndUseAsNormal;

	// Token: 0x04000D2E RID: 3374
	[Tooltip("Force the 'Colour' shader (alternate colour options and grub texture features). If false this may use the default unity 'Lit' shader if there isn't a colour or grub map...")]
	public bool forceColourShader = true;
}
