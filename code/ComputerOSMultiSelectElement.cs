using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000297 RID: 663
public class ComputerOSMultiSelectElement : ComputerOSUIComponent
{
	// Token: 0x06000ED8 RID: 3800 RVA: 0x000D5698 File Offset: 0x000D3898
	public void Setup(ComputerOSMultiSelect.OSMultiOption newOpt, ComputerOSMultiSelect newMulti)
	{
		this.multiSelect = newMulti;
		this.option = newOpt;
		if (this.option.salesRecord != null)
		{
			try
			{
				this.elementText.text = SessionData.Instance.TimeAndDate(this.option.salesRecord.time, false, true, true) + "\n" + this.option.salesRecord.GetPunter().GetInitialledName();
				this.elementText2.text = string.Empty;
				Company company = CityData.Instance.companyDirectory.Find((Company item) => item.companyID == this.option.salesRecord.companyID);
				for (int i = 0; i < this.option.salesRecord.items.Count; i++)
				{
					string interactableName = this.option.salesRecord.items[i];
					InteractablePreset interactablePreset = Toolbox.Instance.GetInteractablePreset(interactableName);
					float input = 0f;
					if (company != null && company.prices.ContainsKey(interactablePreset))
					{
						input = (float)company.prices[interactablePreset];
					}
					TextMeshProUGUI textMeshProUGUI = this.elementText2;
					textMeshProUGUI.text = string.Concat(new string[]
					{
						textMeshProUGUI.text,
						"<align=\"left\">",
						Strings.Get("evidence.names", interactablePreset.name, Strings.Casing.asIs, false, false, false, null),
						"  <align=\"right\">",
						CityControls.Instance.cityCurrency,
						Toolbox.Instance.RoundToPlaces(input, 2).ToString()
					});
					TextMeshProUGUI textMeshProUGUI2 = this.elementText2;
					textMeshProUGUI2.text += "\n";
				}
				return;
			}
			catch
			{
				Game.LogError("Unable to display sales record!", 2);
				return;
			}
		}
		string[] array = this.option.text.Split('\n', 0);
		if (array.Length != 0)
		{
			this.elementText.text = array[0];
		}
		if (array.Length > 1)
		{
			this.elementText2.text = array[1];
		}
	}

	// Token: 0x06000ED9 RID: 3801 RVA: 0x000D58A0 File Offset: 0x000D3AA0
	public override void OnLeftClick()
	{
		this.multiSelect.SetSelected(this);
		base.OnLeftClick();
	}

	// Token: 0x040011FC RID: 4604
	public ComputerOSMultiSelect multiSelect;

	// Token: 0x040011FD RID: 4605
	public RectTransform rect;

	// Token: 0x040011FE RID: 4606
	public TextMeshProUGUI elementText;

	// Token: 0x040011FF RID: 4607
	public TextMeshProUGUI elementText2;

	// Token: 0x04001200 RID: 4608
	public ComputerOSMultiSelect.OSMultiOption option;

	// Token: 0x04001201 RID: 4609
	public Image backgroundImage;

	// Token: 0x04001202 RID: 4610
	public Color backgroundColourNormal = Color.white;

	// Token: 0x04001203 RID: 4611
	public Color backgroundColourSelected = Color.cyan;

	// Token: 0x04001204 RID: 4612
	public bool selected;
}
