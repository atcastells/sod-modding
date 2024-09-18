using System;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x02000594 RID: 1428
public class StickyNoteController : MonoBehaviour
{
	// Token: 0x06001F2E RID: 7982 RVA: 0x001B00B8 File Offset: 0x001AE2B8
	private void OnEnable()
	{
		if (this.rect == null)
		{
			this.rect = base.gameObject.GetComponent<RectTransform>();
		}
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.windowContent == null)
		{
			this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		}
		this.input.SetTextWithoutNotify(this.parentWindow.passedEvidence.GetNote(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1])));
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x001B0148 File Offset: 0x001AE348
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x001B0169 File Offset: 0x001AE369
	public void OnNoteEdit()
	{
		this.parentWindow.passedEvidence.SetNote(Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[1]), this.input.text);
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x001B0191 File Offset: 0x001AE391
	public void SetPlayerTextInput(bool val)
	{
		InterfaceController.Instance.SetPlayerTextInput(val);
	}

	// Token: 0x04002909 RID: 10505
	public RectTransform rect;

	// Token: 0x0400290A RID: 10506
	public WindowContentController windowContent;

	// Token: 0x0400290B RID: 10507
	public InfoWindow parentWindow;

	// Token: 0x0400290C RID: 10508
	public TMP_InputField input;

	// Token: 0x0400290D RID: 10509
	public TextMeshProUGUI text;
}
