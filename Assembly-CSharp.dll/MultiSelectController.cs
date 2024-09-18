using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000537 RID: 1335
public class MultiSelectController : MonoBehaviour
{
	// Token: 0x14000030 RID: 48
	// (add) Token: 0x06001D25 RID: 7461 RVA: 0x0019E5A4 File Offset: 0x0019C7A4
	// (remove) Token: 0x06001D26 RID: 7462 RVA: 0x0019E5DC File Offset: 0x0019C7DC
	public event MultiSelectController.Select OnSelect;

	// Token: 0x06001D27 RID: 7463 RVA: 0x0019E611 File Offset: 0x0019C811
	private void Start()
	{
		this.SetChosen(this.chosenIndex);
	}

	// Token: 0x06001D28 RID: 7464 RVA: 0x0019E620 File Offset: 0x0019C820
	public void SetChosen(int newIndex)
	{
		this.chosenIndex = Mathf.Clamp(newIndex, 0, this.optionButtons.Count - 1);
		Game.Log("Menu: New chosen multi-select: " + this.chosenIndex.ToString(), 2);
		for (int i = 0; i < this.optionButtons.Count; i++)
		{
			if (i == this.chosenIndex)
			{
				this.optionButtons[i].button.icon.enabled = true;
			}
			else
			{
				this.optionButtons[i].button.icon.enabled = false;
			}
		}
		this.OnValueChanged();
	}

	// Token: 0x06001D29 RID: 7465 RVA: 0x0019E6C4 File Offset: 0x0019C8C4
	public Color GetCurrentSelectedColourValue()
	{
		Color result = Color.clear;
		try
		{
			result = this.optionButtons[this.chosenIndex].colourValue;
		}
		catch
		{
		}
		return result;
	}

	// Token: 0x06001D2A RID: 7466 RVA: 0x0019E704 File Offset: 0x0019C904
	public InterfaceControls.EvidenceColours GetCurrentSelectedEvidenceColourValue()
	{
		InterfaceControls.EvidenceColours result = InterfaceControls.EvidenceColours.red;
		try
		{
			result = this.optionButtons[this.chosenIndex].evidenceColour;
		}
		catch
		{
		}
		return result;
	}

	// Token: 0x06001D2B RID: 7467 RVA: 0x0019E740 File Offset: 0x0019C940
	public void OnValueChanged()
	{
		if (this.OnSelect != null)
		{
			this.OnSelect();
		}
		if (this.playerPrefsID != null && this.playerPrefsID.Length > 0)
		{
			PlayerPrefsController.Instance.OnToggleChanged(this.playerPrefsID, true, this);
		}
	}

	// Token: 0x040026E8 RID: 9960
	[Header("Components")]
	[ReorderableList]
	public List<MultiSelectController.MultiSelectValue> optionButtons = new List<MultiSelectController.MultiSelectValue>();

	// Token: 0x040026E9 RID: 9961
	[Header("State")]
	public string playerPrefsID;

	// Token: 0x040026EA RID: 9962
	public int chosenIndex;

	// Token: 0x02000538 RID: 1336
	// (Invoke) Token: 0x06001D2E RID: 7470
	public delegate void Select();

	// Token: 0x02000539 RID: 1337
	[Serializable]
	public class MultiSelectValue
	{
		// Token: 0x040026EC RID: 9964
		public ButtonController button;

		// Token: 0x040026ED RID: 9965
		public Color colourValue;

		// Token: 0x040026EE RID: 9966
		public InterfaceControls.EvidenceColours evidenceColour;
	}
}
