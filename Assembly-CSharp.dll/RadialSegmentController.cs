using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002E7 RID: 743
public class RadialSegmentController : MonoBehaviour
{
	// Token: 0x060010DE RID: 4318 RVA: 0x000EF308 File Offset: 0x000ED508
	public void UpdateSegment(FirstPersonItemController.InventorySlot newSlot)
	{
		this.slot = newSlot;
		this.segmentAngleSpace = 360f / (float)FirstPersonItemController.Instance.slots.Count;
		this.angle = (float)this.slot.index * this.segmentAngleSpace;
		this.toAngle = this.angle + this.segmentAngleSpace;
		this.segmentLineRect.localEulerAngles = new Vector3(0f, 0f, this.angle);
		this.elementLineRect.localEulerAngles = new Vector3(0f, 0f, this.angle + this.segmentAngleSpace * 0.5f);
		this.elementRect.eulerAngles = Vector3.zero;
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x000EF3C4 File Offset: 0x000ED5C4
	public void OnUpdateContent()
	{
		this.stolenIcon.gameObject.SetActive(false);
		if (this.slot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
		{
			Interactable slotInteractable = this.slot.GetInteractable();
			if (slotInteractable == null)
			{
				this.text.text = Strings.Get("ui.interface", "Empty", Strings.Casing.firstLetterCaptial, false, false, false, null);
				this.img.sprite = InterfaceControls.Instance.iconReference.Find((InterfaceControls.IconConfig item) => item.iconType == InterfaceControls.Icon.empty).sprite;
				return;
			}
			this.text.text = slotInteractable.GetName();
			if (slotInteractable.preset.iconOverride != null)
			{
				this.img.sprite = slotInteractable.preset.iconOverride;
			}
			else if (slotInteractable.evidence != null)
			{
				this.img.sprite = slotInteractable.evidence.preset.iconSpriteLarge;
			}
			else
			{
				this.img.sprite = InterfaceControls.Instance.iconReference.Find((InterfaceControls.IconConfig item) => item.iconType == InterfaceControls.Icon.hand).sprite;
			}
			if (StatusController.Instance.activeFineRecords.Exists((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.theft && item.objectID == slotInteractable.id))
			{
				this.stolenIcon.gameObject.SetActive(true);
				return;
			}
		}
		else
		{
			FirstPersonItem firstPersonItem = this.slot.GetFirstPersonItem();
			if (firstPersonItem != null)
			{
				this.text.text = Strings.Get("evidence.names", firstPersonItem.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
				this.img.sprite = firstPersonItem.selectionIcon;
				return;
			}
			this.text.text = Strings.Get("ui.interface", "nothing", Strings.Casing.firstLetterCaptial, false, false, false, null);
			this.img.sprite = InterfaceControls.Instance.iconReference.Find((InterfaceControls.IconConfig item) => item.iconType == InterfaceControls.Icon.empty).sprite;
		}
	}

	// Token: 0x0400151A RID: 5402
	[Header("Components")]
	public RectTransform rect;

	// Token: 0x0400151B RID: 5403
	public RectTransform segmentLineRect;

	// Token: 0x0400151C RID: 5404
	public RectTransform elementLineRect;

	// Token: 0x0400151D RID: 5405
	public RectTransform elementRect;

	// Token: 0x0400151E RID: 5406
	public RectTransform stolenIcon;

	// Token: 0x0400151F RID: 5407
	public TextMeshProUGUI text;

	// Token: 0x04001520 RID: 5408
	public Image img;

	// Token: 0x04001521 RID: 5409
	public List<CanvasRenderer> renderers = new List<CanvasRenderer>();

	// Token: 0x04001522 RID: 5410
	[Header("Inventory")]
	public FirstPersonItemController.InventorySlot slot;

	// Token: 0x04001523 RID: 5411
	[Header("Calculations")]
	[Tooltip("The space each segment takes up")]
	[ReadOnly]
	public float segmentAngleSpace;

	// Token: 0x04001524 RID: 5412
	[ReadOnly]
	[Tooltip("The anlge of this slot")]
	public float angle;

	// Token: 0x04001525 RID: 5413
	[ReadOnly]
	public float toAngle;
}
