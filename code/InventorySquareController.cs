using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x020005CE RID: 1486
public class InventorySquareController : ButtonController
{
	// Token: 0x060020A0 RID: 8352 RVA: 0x001BEDC8 File Offset: 0x001BCFC8
	public void Setup(FirstPersonItemController.InventorySlot newSlot)
	{
		this.slot = newSlot;
		if (this.slot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
		{
			this.background.sprite = BioScreenController.Instance.itemBGIcon;
			if (this.equipmentIcon != null)
			{
				this.equipmentIcon.gameObject.SetActive(false);
			}
		}
		else if (this.equipmentIcon != null)
		{
			this.equipmentIcon.gameObject.SetActive(true);
		}
		this.SetupReferences();
		this.OnUpdateContent();
		this.UpdateHotkeyDisplay();
	}

	// Token: 0x060020A1 RID: 8353 RVA: 0x001BEE50 File Offset: 0x001BD050
	public void OnUpdateContent()
	{
		if (this.stolenIcon != null)
		{
			this.stolenIcon.gameObject.SetActive(false);
		}
		if (this.icon != null)
		{
			this.icon.gameObject.SetActive(false);
		}
		this.text.text = string.Empty;
		if (this.slot.isStatic == FirstPersonItemController.InventorySlot.StaticSlot.nonStatic)
		{
			Interactable slotInteractable = this.slot.GetInteractable();
			if (slotInteractable == null)
			{
				this.text.text = Strings.Get("ui.interface", "Empty", Strings.Casing.firstLetterCaptial, false, false, false, null);
				this.slot.interactableID = -1;
			}
			else
			{
				this.text.text = slotInteractable.GetName();
				if (this.icon != null)
				{
					this.icon.sprite = slotInteractable.preset.iconOverride;
					this.icon.gameObject.SetActive(true);
				}
				if (StatusController.Instance.activeFineRecords.Exists((StatusController.FineRecord item) => item.crime == StatusController.CrimeType.theft && item.objectID == slotInteractable.id) && this.stolenIcon != null)
				{
					this.stolenIcon.gameObject.SetActive(true);
				}
			}
		}
		else
		{
			FirstPersonItem firstPersonItem = this.slot.GetFirstPersonItem();
			if (firstPersonItem != null)
			{
				this.text.text = Strings.Get("evidence.names", firstPersonItem.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
				if (this.icon != null)
				{
					this.icon.sprite = firstPersonItem.selectionIcon;
					this.icon.gameObject.SetActive(true);
				}
			}
			else
			{
				this.text.text = Strings.Get("ui.interface", "nothing", Strings.Casing.firstLetterCaptial, false, false, false, null);
			}
		}
		if (this.slot.hotkey != null && this.slot.hotkey.Length > 0)
		{
			TextMeshProUGUI text = this.text;
			text.text = text.text + " [" + this.slot.hotkey + "]";
		}
	}

	// Token: 0x060020A2 RID: 8354 RVA: 0x001BF072 File Offset: 0x001BD272
	public void UpdateHotkeyDisplay()
	{
		this.OnUpdateContent();
	}

	// Token: 0x060020A3 RID: 8355 RVA: 0x001BF07A File Offset: 0x001BD27A
	public override void OnHoverStart()
	{
		base.OnHoverStart();
		BioScreenController.Instance.HoverSlot(this.slot);
	}

	// Token: 0x060020A4 RID: 8356 RVA: 0x001BF092 File Offset: 0x001BD292
	public override void OnHoverEnd()
	{
		base.OnHoverEnd();
		if (BioScreenController.Instance.hoveredSlot == this.slot)
		{
			BioScreenController.Instance.HoverSlot(null);
		}
	}

	// Token: 0x060020A5 RID: 8357 RVA: 0x001BF0B7 File Offset: 0x001BD2B7
	public override void OnLeftClick()
	{
		base.OnLeftClick();
		if (BioScreenController.Instance.selectedSlot != this.slot)
		{
			BioScreenController.Instance.SelectSlot(this.slot, false, false);
		}
	}

	// Token: 0x060020A6 RID: 8358 RVA: 0x001BF0E3 File Offset: 0x001BD2E3
	public override void OnRightClick()
	{
		if (InterfaceController.Instance.desktopMode)
		{
			return;
		}
		base.OnRightClick();
		if (BioScreenController.Instance.selectedSlot != this.slot)
		{
			BioScreenController.Instance.SelectSlot(this.slot, true, false);
		}
	}

	// Token: 0x04002AD1 RID: 10961
	public List<CanvasRenderer> renderers = new List<CanvasRenderer>();

	// Token: 0x04002AD2 RID: 10962
	public FirstPersonItemController.InventorySlot slot;

	// Token: 0x04002AD3 RID: 10963
	public RectTransform stolenIcon;

	// Token: 0x04002AD4 RID: 10964
	public RectTransform equipmentIcon;

	// Token: 0x04002AD5 RID: 10965
	public RectTransform selected;
}
