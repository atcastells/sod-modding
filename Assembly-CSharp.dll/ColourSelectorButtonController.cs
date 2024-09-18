using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200055E RID: 1374
public class ColourSelectorButtonController : ButtonController, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
{
	// Token: 0x14000033 RID: 51
	// (add) Token: 0x06001DE1 RID: 7649 RVA: 0x001A45E4 File Offset: 0x001A27E4
	// (remove) Token: 0x06001DE2 RID: 7650 RVA: 0x001A461C File Offset: 0x001A281C
	public event ColourSelectorButtonController.ChangeColour OnChangeColour;

	// Token: 0x06001DE3 RID: 7651 RVA: 0x001A4651 File Offset: 0x001A2851
	public override void VisualUpdate()
	{
		base.VisualUpdate();
		this.background.color = this.selectedColour;
	}

	// Token: 0x06001DE4 RID: 7652 RVA: 0x001A466A File Offset: 0x001A286A
	public override void OnPointerDown(PointerEventData eventData)
	{
		this.selector.gameObject.SetActive(true);
		if (this.tooltip != null)
		{
			this.tooltip.ForceClose();
		}
	}

	// Token: 0x06001DE5 RID: 7653 RVA: 0x001A4698 File Offset: 0x001A2898
	public override void OnPointerUp(PointerEventData eventData)
	{
		foreach (GameObject gameObject in eventData.hovered)
		{
			Button component = gameObject.GetComponent<Button>();
			if (component != null && this.colourButtons.Contains(component))
			{
				this.selectedColour = component.image.color;
				AudioController.Instance.Play2DSound(AudioControls.Instance.mapControlButton, null, 1f);
				this.VisualUpdate();
				if (this.OnChangeColour != null)
				{
					this.OnChangeColour();
				}
				return;
			}
		}
		this.selector.gameObject.SetActive(false);
	}

	// Token: 0x040027BA RID: 10170
	public RectTransform selector;

	// Token: 0x040027BB RID: 10171
	public List<Button> colourButtons = new List<Button>();

	// Token: 0x040027BC RID: 10172
	public Color selectedColour = Color.white;

	// Token: 0x0200055F RID: 1375
	// (Invoke) Token: 0x06001DE8 RID: 7656
	public delegate void ChangeColour();
}
