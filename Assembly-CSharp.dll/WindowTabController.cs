using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200059B RID: 1435
public class WindowTabController : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	// Token: 0x06001F59 RID: 8025 RVA: 0x001B183E File Offset: 0x001AFA3E
	private void Awake()
	{
		this.rect = base.gameObject.GetComponent<RectTransform>();
		this.text = base.gameObject.GetComponentInChildren<TextMeshProUGUI>();
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x001B1864 File Offset: 0x001AFA64
	public void SetupButton()
	{
		base.gameObject.GetComponent<Image>().color = this.preset.colour;
		this.text.text = Strings.Get("ui.interface", this.preset.tabName, Strings.Casing.asIs, false, false, false, null);
		base.name = this.text.text;
		this.content.CentrePage();
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x001B18CD File Offset: 0x001AFACD
	public void OnPointerClick(PointerEventData eventData)
	{
		int clickCount = eventData.clickCount;
		AudioController.Instance.Play2DSound(AudioControls.Instance.tab, null, 1f);
		if (clickCount == 2)
		{
			this.content.CentrePage();
		}
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x00002265 File Offset: 0x00000465
	public void VisualUpdate()
	{
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x001B18FE File Offset: 0x001AFAFE
	public void SetNewItems(int newItemCount)
	{
		this.newItems = newItemCount;
		if (this.newItems <= 0 && this.pulsateController != null)
		{
			this.pulsateController.enabled = false;
		}
	}

	// Token: 0x0400292F RID: 10543
	public RectTransform rect;

	// Token: 0x04002930 RID: 10544
	public Button tabButton;

	// Token: 0x04002931 RID: 10545
	public WindowContentController content;

	// Token: 0x04002932 RID: 10546
	public WindowTabPreset preset;

	// Token: 0x04002933 RID: 10547
	public TextMeshProUGUI text;

	// Token: 0x04002934 RID: 10548
	public int newItems;

	// Token: 0x04002935 RID: 10549
	public PulsateController pulsateController;
}
