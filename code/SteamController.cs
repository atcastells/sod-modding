using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000491 RID: 1169
public class SteamController : MonoBehaviour
{
	// Token: 0x060018FF RID: 6399 RVA: 0x0017251C File Offset: 0x0017071C
	public void Setup(NewRoom newRoom)
	{
		this.room = newRoom;
		this.room.steamController = this;
		this.glassMaterial = Object.Instantiate<Material>(this.glassMaterialOriginal);
		this.glassMaterial.SetFloat("_DistortionBlurScale", Mathf.Lerp(this.blurScale.x, this.blurScale.y, this.steamLevel));
		foreach (MeshRenderer meshRenderer in this.glassPanels)
		{
			meshRenderer.sharedMaterial = this.glassMaterial;
		}
		this.SteamStateChanged();
	}

	// Token: 0x06001900 RID: 6400 RVA: 0x001725D0 File Offset: 0x001707D0
	public void SteamStateChanged()
	{
		this.existingSteamLevel = this.steamLevel;
		base.enabled = true;
	}

	// Token: 0x06001901 RID: 6401 RVA: 0x001725E8 File Offset: 0x001707E8
	private void Update()
	{
		if (this.room == null)
		{
			return;
		}
		if (this.room.steamOn)
		{
			this.steamLevel = Mathf.Clamp01(this.existingSteamLevel + (SessionData.Instance.gameTime - this.room.steamLastSwitched) / this.steamTime);
		}
		else
		{
			this.steamLevel = Mathf.Clamp01(this.existingSteamLevel - (SessionData.Instance.gameTime - this.room.steamLastSwitched) / this.desteamTime);
		}
		this.glassMaterial.SetFloat("_DistortionBlurScale", Mathf.Lerp(this.blurScale.x, this.blurScale.y, this.steamLevel));
		foreach (Interactable interactable in this.room.mainLights)
		{
			if (interactable.lightController != null)
			{
				interactable.lightController.SetVolumentricAtmosphere(this.room.preset.baseRoomAtmosphere * interactable.lightController.preset.atmosphereMultiplier + this.steamLevel);
			}
		}
		if (this.room.steamOn && this.steamLevel >= 1f)
		{
			base.enabled = false;
			return;
		}
		if (!this.room.steamOn && this.steamLevel <= 0f)
		{
			base.enabled = false;
		}
	}

	// Token: 0x0400219F RID: 8607
	public NewRoom room;

	// Token: 0x040021A0 RID: 8608
	[Tooltip("The steam level previously existing in this room")]
	private float existingSteamLevel;

	// Token: 0x040021A1 RID: 8609
	[Tooltip("The steam level in this room.")]
	public float steamLevel;

	// Token: 0x040021A2 RID: 8610
	[Tooltip("The scale to blur glass panels.")]
	public Vector2 blurScale = new Vector2(0.05f, 0.75f);

	// Token: 0x040021A3 RID: 8611
	[Tooltip("The time it takes to completely steam up a room")]
	public float steamTime = 0.2f;

	// Token: 0x040021A4 RID: 8612
	[Tooltip("The time it takes to completely de-steam a room")]
	public float desteamTime = 0.6f;

	// Token: 0x040021A5 RID: 8613
	public List<MeshRenderer> glassPanels;

	// Token: 0x040021A6 RID: 8614
	public Material glassMaterialOriginal;

	// Token: 0x040021A7 RID: 8615
	public Material glassMaterial;
}
