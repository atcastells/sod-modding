using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// Token: 0x0200051F RID: 1311
public class InterfaceVideoController : MonoBehaviour
{
	// Token: 0x06001C67 RID: 7271 RVA: 0x00195DCC File Offset: 0x00193FCC
	public void Setup(VideoClip clip, Texture2D img)
	{
		if (clip == null || clip != this.player.clip)
		{
			if (this.renderTextureInstance != null)
			{
				this.player.targetTexture.Release();
			}
			Object.Destroy(this.renderTextureInstance);
			if (clip != null)
			{
				this.renderTextureInstance = Object.Instantiate<RenderTexture>(this.renderTexturePrefab);
				this.player.targetTexture = this.renderTextureInstance;
				this.image.texture = this.renderTextureInstance;
				this.player.clip = clip;
				return;
			}
			if (img != null)
			{
				this.image.texture = img;
			}
		}
	}

	// Token: 0x06001C68 RID: 7272 RVA: 0x00195E7F File Offset: 0x0019407F
	private void OnDestroy()
	{
		if (this.renderTextureInstance != null)
		{
			this.renderTextureInstance.Release();
		}
		Object.Destroy(this.renderTextureInstance);
	}

	// Token: 0x040025AE RID: 9646
	[Header("Components")]
	public VideoPlayer player;

	// Token: 0x040025AF RID: 9647
	public RawImage image;

	// Token: 0x040025B0 RID: 9648
	[Header("Settings")]
	public RenderTexture renderTexturePrefab;

	// Token: 0x040025B1 RID: 9649
	private RenderTexture renderTextureInstance;
}
