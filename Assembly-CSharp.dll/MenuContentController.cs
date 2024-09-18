using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x02000581 RID: 1409
public class MenuContentController : MonoBehaviour
{
	// Token: 0x06001ED5 RID: 7893 RVA: 0x001AC81C File Offset: 0x001AAA1C
	private void OnEnable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		if (this.windowContent == null)
		{
			this.windowContent = base.gameObject.GetComponentInParent<WindowContentController>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
		this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		this.CheckEnabled();
	}

	// Token: 0x06001ED6 RID: 7894 RVA: 0x001AC8A0 File Offset: 0x001AAAA0
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001ED7 RID: 7895 RVA: 0x001AC900 File Offset: 0x001AAB00
	public void CheckEnabled()
	{
		Company company = (this.parentWindow.passedEvidence.controller as NewAddress).company;
		string text = string.Concat(new string[]
		{
			"\n\n<align=center><b><cspace=0.3em><smallcaps>",
			company.name,
			" ",
			Strings.Get("evidence.names", "Menu", Strings.Casing.asIs, false, false, false, null),
			"</smallcaps><align=left></b></cspace>"
		});
		if (company.retailOpenHours.x == 0f && company.retailOpenHours.y == 24f)
		{
			text = string.Concat(new string[]
			{
				text,
				"\n\n",
				Strings.Get("evidence.generic", "Open", Strings.Casing.asIs, false, false, false, null),
				" ",
				Strings.Get("evidence.generic", "24 hours", Strings.Casing.asIs, false, false, false, null),
				", "
			});
		}
		else
		{
			text = string.Concat(new string[]
			{
				text,
				"\n\n",
				Strings.Get("evidence.generic", "Open", Strings.Casing.asIs, false, false, false, null),
				" ",
				SessionData.Instance.DecimalToClockString(company.retailOpenHours.x, false),
				" - ",
				SessionData.Instance.DecimalToClockString(company.retailOpenHours.y, false),
				", "
			});
		}
		if (company.daysOpen.Count >= 7)
		{
			text += Strings.Get("evidence.generic", "every day", Strings.Casing.asIs, false, false, false, null);
		}
		else if (company.daysOpen.Count >= 5)
		{
			text = string.Concat(new string[]
			{
				text,
				Strings.Get("evidence.generic", "every day", Strings.Casing.asIs, false, false, false, null),
				" ",
				Strings.Get("evidence.generic", "except", Strings.Casing.asIs, false, false, false, null),
				" "
			});
			for (int i = 0; i < company.daysClosed.Count; i++)
			{
				text += Strings.Get("ui.interface", company.daysClosed[i].ToString(), Strings.Casing.asIs, false, false, false, null);
				if (i < company.daysClosed.Count - 1)
				{
					text += "& ";
				}
			}
		}
		else
		{
			for (int j = 0; j < company.daysClosed.Count; j++)
			{
				text += Strings.Get("ui.interface", company.daysClosed[j].ToString(), Strings.Casing.asIs, false, false, false, null);
				if (j < company.daysClosed.Count - 1)
				{
					text += ", ";
				}
			}
		}
		text += ".\n";
		Dictionary<RetailItemPreset, float> dictionary = new Dictionary<RetailItemPreset, float>();
		Dictionary<RetailItemPreset, float> dictionary2 = new Dictionary<RetailItemPreset, float>();
		Dictionary<RetailItemPreset, float> dictionary3 = new Dictionary<RetailItemPreset, float>();
		foreach (KeyValuePair<InteractablePreset, int> keyValuePair in company.prices)
		{
			if (keyValuePair.Key.retailItem != null)
			{
				if (keyValuePair.Key.retailItem.menuCategory == RetailItemPreset.MenuCategory.food)
				{
					dictionary.Add(keyValuePair.Key.retailItem, (float)keyValuePair.Value);
				}
				else if (keyValuePair.Key.retailItem.menuCategory == RetailItemPreset.MenuCategory.snacks)
				{
					dictionary2.Add(keyValuePair.Key.retailItem, (float)keyValuePair.Value);
				}
				else if (keyValuePair.Key.retailItem.menuCategory == RetailItemPreset.MenuCategory.drinks)
				{
					dictionary3.Add(keyValuePair.Key.retailItem, (float)keyValuePair.Value);
				}
			}
		}
		if (dictionary.Count > 0)
		{
			text = text + "\n\n<align=center><b><cspace=0.3em><smallcaps>" + Strings.Get("evidence.generic", "Food", Strings.Casing.asIs, false, false, false, null) + "</smallcaps><align=left></b></cspace>";
			foreach (KeyValuePair<RetailItemPreset, float> keyValuePair2 in dictionary)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n",
					Strings.Get("evidence.names", keyValuePair2.Key.name, Strings.Casing.asIs, false, false, false, null),
					"<pos=70%>",
					CityControls.Instance.cityCurrency,
					Toolbox.Instance.AddZeros(Toolbox.Instance.RoundToPlaces(keyValuePair2.Value, 2), 2)
				});
			}
		}
		if (dictionary2.Count > 0)
		{
			text = text + "\n\n<align=center><b><cspace=0.3em><smallcaps>" + Strings.Get("evidence.generic", "Snacks", Strings.Casing.asIs, false, false, false, null) + "</smallcaps><align=left></b></cspace>";
			foreach (KeyValuePair<RetailItemPreset, float> keyValuePair3 in dictionary2)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n",
					Strings.Get("evidence.names", keyValuePair3.Key.name, Strings.Casing.asIs, false, false, false, null),
					"<pos=70%>",
					CityControls.Instance.cityCurrency,
					Toolbox.Instance.AddZeros(Toolbox.Instance.RoundToPlaces(keyValuePair3.Value, 2), 2)
				});
			}
		}
		if (dictionary3.Count > 0)
		{
			text = text + "\n\n<align=center><b><cspace=0.3em><smallcaps>" + Strings.Get("evidence.generic", "Drink", Strings.Casing.asIs, false, false, false, null) + "</smallcaps><align=left></b></cspace>";
			foreach (KeyValuePair<RetailItemPreset, float> keyValuePair4 in dictionary3)
			{
				text = string.Concat(new string[]
				{
					text,
					"\n",
					Strings.Get("evidence.names", keyValuePair4.Key.name, Strings.Casing.asIs, false, false, false, null),
					"<pos=70%>",
					CityControls.Instance.cityCurrency,
					Toolbox.Instance.AddZeros(Toolbox.Instance.RoundToPlaces(keyValuePair4.Value, 2), 2)
				});
			}
		}
		this.descriptionText.text = text.Trim();
	}

	// Token: 0x040028A9 RID: 10409
	public WindowContentController windowContent;

	// Token: 0x040028AA RID: 10410
	public InfoWindow parentWindow;

	// Token: 0x040028AB RID: 10411
	public TextMeshProUGUI descriptionText;
}
