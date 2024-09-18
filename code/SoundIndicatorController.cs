using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020004F0 RID: 1264
public class SoundIndicatorController : MonoBehaviour
{
	// Token: 0x06001B4A RID: 6986 RVA: 0x0018C256 File Offset: 0x0018A456
	public void SetSoundEvent(AudioEvent newEvent, bool updateEvent = true)
	{
		if (newEvent != this.currentEvent)
		{
			this.currentEvent = newEvent;
			if (updateEvent)
			{
				this.UpdateCurrentEvent();
			}
		}
	}

	// Token: 0x06001B4B RID: 6987 RVA: 0x0018C278 File Offset: 0x0018A478
	public void UpdateCurrentEvent()
	{
		if (this.currentEvent == null)
		{
			this.currentIconCount = 0;
			this.previousIconCount = 0;
			base.enabled = true;
			return;
		}
		AudioController.SoundMaterialOverride soundMaterialOverride = null;
		if (this.isFootstep)
		{
			Vector3 vector = Vector3.zero;
			if (this.rightFoot)
			{
				vector = Player.Instance.transform.TransformPoint(new Vector3(0.13f, 0.1f, 0f));
			}
			else
			{
				vector = Player.Instance.transform.TransformPoint(new Vector3(-0.13f, 0.1f, 0f));
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, Vector3.down, ref raycastHit, 2.6f, AudioController.Instance.footstepLayerMask, 1))
			{
				MaterialOverrideController component = raycastHit.transform.GetComponent<MaterialOverrideController>();
				if (component != null)
				{
					soundMaterialOverride = new AudioController.SoundMaterialOverride(component.concrete, component.wood, component.carpet, component.tile, component.plaster, component.fabric, component.metal, component.glass);
				}
				else
				{
					InteractableController component2 = raycastHit.transform.GetComponent<InteractableController>();
					if (component2 != null && component2.interactable != null && component2.interactable.preset.useMaterialOverride)
					{
						soundMaterialOverride = component2.interactable.preset.materialOverride;
					}
					else
					{
						MeshFilter component3 = raycastHit.transform.GetComponent<MeshFilter>();
						if (component3 != null)
						{
							FurniturePreset furnitureFromMesh = Toolbox.Instance.GetFurnitureFromMesh(component3.sharedMesh);
							if (furnitureFromMesh != null)
							{
								soundMaterialOverride = new AudioController.SoundMaterialOverride(furnitureFromMesh.concrete, furnitureFromMesh.wood, furnitureFromMesh.carpet, furnitureFromMesh.tile, furnitureFromMesh.plaster, furnitureFromMesh.fabric, furnitureFromMesh.metal, furnitureFromMesh.glass);
							}
						}
					}
				}
			}
			if (soundMaterialOverride == null)
			{
				soundMaterialOverride = new AudioController.SoundMaterialOverride(Player.Instance.currentRoom.floorMaterial.concrete, Player.Instance.currentRoom.floorMaterial.wood, Player.Instance.currentRoom.floorMaterial.carpet, Player.Instance.currentRoom.floorMaterial.tile, Player.Instance.currentRoom.floorMaterial.plaster, Player.Instance.currentRoom.floorMaterial.fabric, Player.Instance.currentRoom.floorMaterial.metal, Player.Instance.currentRoom.floorMaterial.glass);
			}
		}
		int num;
		bool flag;
		List<NewRoom> list;
		AudioController.Instance.GetOcculusion(Player.Instance.currentNode, Player.Instance.currentNode, this.currentEvent, 1f, Player.Instance, soundMaterialOverride, out num, out this.currentListeners, out flag, out list, out this.currentHearingRange, null, false);
		this.currentIconCount = Mathf.FloorToInt(this.currentHearingRange / AudioController.Instance.soundIconRangeUnit);
		if (this.currentIconCount > this.previousIconCount && this.juice != null)
		{
			this.juice.Nudge(new Vector2(1.2f, 1.2f), Vector2.zero, true, true, true);
		}
		this.previousIconCount = this.currentIconCount;
		base.enabled = true;
	}

	// Token: 0x06001B4C RID: 6988 RVA: 0x0018C59C File Offset: 0x0018A79C
	private void Update()
	{
		bool flag = true;
		int num = 0;
		if (this.currentIconCount > 0)
		{
			num = 5;
		}
		if (this.spawnedIcons.Count < num)
		{
			foreach (SoundIndicatorController.AudioIcon audioIcon in this.spawnedIcons)
			{
				audioIcon.remove = false;
			}
			SoundIndicatorController.AudioIcon audioIcon2 = new SoundIndicatorController.AudioIcon();
			GameObject gameObject = Object.Instantiate<GameObject>(PrefabControls.Instance.soundIndicatorIcon, this.rect);
			audioIcon2.rect = gameObject.GetComponent<RectTransform>();
			audioIcon2.img = gameObject.GetComponent<Image>();
			audioIcon2.rect.anchoredPosition = new Vector2((float)this.spawnedIcons.Count * -32f + this.iconOffset.x, audioIcon2.rect.anchoredPosition.y + this.iconOffset.y);
			if (this.spawnedIcons.Count < this.currentIconCount)
			{
				audioIcon2.img.color = ControlsDisplayController.Instance.audioFullColor;
				this.fullIcons.Add(audioIcon2);
			}
			else
			{
				audioIcon2.img.color = ControlsDisplayController.Instance.audioEmptyColor;
			}
			this.spawnedIcons.Add(audioIcon2);
			flag = false;
		}
		else if (this.spawnedIcons.Count > num)
		{
			for (int i = 0; i < this.spawnedIcons.Count; i++)
			{
				if (i > this.currentIconCount - 1)
				{
					this.spawnedIcons[i].remove = true;
					this.fullIcons.Remove(this.spawnedIcons[i]);
					flag = false;
				}
			}
		}
		for (int j = 0; j < this.spawnedIcons.Count; j++)
		{
			SoundIndicatorController.AudioIcon audioIcon3 = this.spawnedIcons[j];
			if (audioIcon3.remove)
			{
				if (audioIcon3.fadeIn > 0f)
				{
					audioIcon3.fadeIn -= Time.deltaTime * 5f;
					audioIcon3.img.canvasRenderer.SetAlpha(audioIcon3.fadeIn);
					audioIcon3.rect.localScale = new Vector3(audioIcon3.fadeIn, audioIcon3.fadeIn, audioIcon3.fadeIn);
					flag = false;
				}
				else
				{
					Object.Destroy(audioIcon3.rect.gameObject);
					this.spawnedIcons.RemoveAt(j);
					j--;
				}
			}
			else if (audioIcon3.fadeIn < 1f)
			{
				audioIcon3.fadeIn += Time.deltaTime * Mathf.Max(8f - (float)j, 2f);
				audioIcon3.fadeIn = Mathf.Clamp01(audioIcon3.fadeIn);
				audioIcon3.img.canvasRenderer.SetAlpha(audioIcon3.fadeIn);
				audioIcon3.rect.localScale = new Vector3(audioIcon3.fadeIn, audioIcon3.fadeIn, audioIcon3.fadeIn);
				flag = false;
			}
		}
		if (this.currentListeners.Count > 0)
		{
			if (this.colourLerp < 1f)
			{
				this.colourLerp += Time.deltaTime * 4f;
				this.colourLerp = Mathf.Clamp01(this.colourLerp);
				this.col = Color.Lerp(ControlsDisplayController.Instance.audioFullColor, InterfaceControls.Instance.heardSoundIconColour, this.colourLerp);
				flag = false;
				foreach (SoundIndicatorController.AudioIcon audioIcon4 in this.fullIcons)
				{
					audioIcon4.img.color = this.col;
				}
				if (this.additionalGraphic != null)
				{
					this.additionalGraphic.color = this.col;
				}
				if (this.juice != null)
				{
					this.juice.elements[0].originalColour = this.col;
				}
			}
		}
		else if (this.colourLerp > 0f)
		{
			this.colourLerp -= Time.deltaTime * 4f;
			this.colourLerp = Mathf.Clamp01(this.colourLerp);
			this.col = Color.Lerp(ControlsDisplayController.Instance.audioFullColor, InterfaceControls.Instance.heardSoundIconColour, this.colourLerp);
			flag = false;
			foreach (SoundIndicatorController.AudioIcon audioIcon5 in this.fullIcons)
			{
				audioIcon5.img.color = this.col;
			}
			if (this.additionalGraphic != null)
			{
				this.additionalGraphic.color = this.col;
			}
			if (this.juice != null)
			{
				this.juice.elements[0].originalColour = this.col;
			}
		}
		if (flag)
		{
			base.enabled = false;
		}
	}

	// Token: 0x040023FF RID: 9215
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x04002400 RID: 9216
	public JuiceController juice;

	// Token: 0x04002401 RID: 9217
	public Image additionalGraphic;

	// Token: 0x04002402 RID: 9218
	[Tooltip("Check true if this is for footstep sounds, as it will simlate the surface being walked on...")]
	[Header("State")]
	public bool isFootstep;

	// Token: 0x04002403 RID: 9219
	[Tooltip("Keep this updated when checking for footsteps...")]
	public bool rightFoot;

	// Token: 0x04002404 RID: 9220
	public AudioEvent currentEvent;

	// Token: 0x04002405 RID: 9221
	private EventDescription description;

	// Token: 0x04002406 RID: 9222
	public List<AudioController.ActiveListener> currentListeners = new List<AudioController.ActiveListener>();

	// Token: 0x04002407 RID: 9223
	public float currentHearingRange;

	// Token: 0x04002408 RID: 9224
	public int currentIconCount;

	// Token: 0x04002409 RID: 9225
	private int previousIconCount;

	// Token: 0x0400240A RID: 9226
	public float colourLerp;

	// Token: 0x0400240B RID: 9227
	public Color col = Color.white;

	// Token: 0x0400240C RID: 9228
	public Vector2 iconOffset = Vector2.zero;

	// Token: 0x0400240D RID: 9229
	public List<SoundIndicatorController.AudioIcon> spawnedIcons = new List<SoundIndicatorController.AudioIcon>();

	// Token: 0x0400240E RID: 9230
	public List<SoundIndicatorController.AudioIcon> fullIcons = new List<SoundIndicatorController.AudioIcon>();

	// Token: 0x020004F1 RID: 1265
	[Serializable]
	public class AudioIcon
	{
		// Token: 0x0400240F RID: 9231
		public RectTransform rect;

		// Token: 0x04002410 RID: 9232
		public Image img;

		// Token: 0x04002411 RID: 9233
		public float fadeIn;

		// Token: 0x04002412 RID: 9234
		public bool remove;
	}
}
