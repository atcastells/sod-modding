using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020005C6 RID: 1478
public class PulsateController : MonoBehaviour
{
	// Token: 0x06002066 RID: 8294 RVA: 0x001BC4B0 File Offset: 0x001BA6B0
	private void Start()
	{
		if (this.img == null)
		{
			this.img = base.gameObject.GetComponent<Image>();
		}
		if (this.getNormalColourAtStart)
		{
			this.normalColour = this.img.color;
		}
		this.progress = 0f;
	}

	// Token: 0x06002067 RID: 8295 RVA: 0x001BC500 File Offset: 0x001BA700
	private void Update()
	{
		Color color = Color.Lerp(this.normalColour, this.pulsateColour, this.progress);
		this.img.color = color;
		if (!this.onoff && this.progress < 1f)
		{
			this.progress += this.speed * Time.deltaTime;
		}
		else if (this.onoff && this.progress > 0f)
		{
			this.progress -= this.speed * Time.deltaTime;
		}
		if (this.progress >= 1f && !this.onoff)
		{
			this.onoff = true;
			return;
		}
		if (this.progress <= 0f && this.onoff)
		{
			this.onoff = false;
		}
	}

	// Token: 0x06002068 RID: 8296 RVA: 0x001BC5C8 File Offset: 0x001BA7C8
	private void OnDisable()
	{
		this.img.color = this.normalColour;
		this.progress = 0f;
	}

	// Token: 0x04002A7B RID: 10875
	public Image img;

	// Token: 0x04002A7C RID: 10876
	public bool getNormalColourAtStart = true;

	// Token: 0x04002A7D RID: 10877
	public Color normalColour = Color.white;

	// Token: 0x04002A7E RID: 10878
	public Color pulsateColour = Color.red;

	// Token: 0x04002A7F RID: 10879
	public float speed = 5f;

	// Token: 0x04002A80 RID: 10880
	public float progress;

	// Token: 0x04002A81 RID: 10881
	public bool onoff;
}
