using System;
using System.Linq;
using TMPro;
using UnityEngine;

// Token: 0x02000590 RID: 1424
public class SalesLedgerEntryController : MonoBehaviour
{
	// Token: 0x06001F15 RID: 7957 RVA: 0x001AE860 File Offset: 0x001ACA60
	public void Setup(Company.SalesRecord newRecord, SalesLedgerContentController newSalesLedger)
	{
		this.salesLedger = newSalesLedger;
		this.salesRecord = newRecord;
		Human punter = this.salesRecord.GetPunter();
		Strings.LinkData linkData = Strings.AddOrGetLink(punter.evidenceEntry, Enumerable.ToList<Evidence.DataKey>(new Evidence.DataKey[]
		{
			Evidence.DataKey.initialedName
		}));
		this.nameText.text = string.Concat(new string[]
		{
			"<link=",
			linkData.id.ToString(),
			">",
			punter.GetInitialledName(),
			"</link>"
		});
		this.descriptionText.text = string.Empty;
		float num = 38f;
		Company company = this.salesRecord.GetCompany();
		for (int i = 0; i < this.salesRecord.items.Count; i++)
		{
			string interactableName = this.salesRecord.items[i];
			InteractablePreset interactablePreset = Toolbox.Instance.GetInteractablePreset(interactableName);
			float input = 0f;
			if (company.prices.ContainsKey(interactablePreset))
			{
				input = (float)company.prices[interactablePreset];
			}
			TextMeshProUGUI textMeshProUGUI = this.descriptionText;
			textMeshProUGUI.text = string.Concat(new string[]
			{
				textMeshProUGUI.text,
				"<align=\"left\">",
				Strings.Get("evidence.names", interactablePreset.name, Strings.Casing.asIs, false, false, false, null),
				"  <align=\"right\">",
				CityControls.Instance.cityCurrency,
				Toolbox.Instance.RoundToPlaces(input, 2).ToString()
			});
			TextMeshProUGUI textMeshProUGUI2 = this.descriptionText;
			textMeshProUGUI2.text += "\n";
			this.descriptionText.rectTransform.sizeDelta = new Vector2(this.descriptionText.rectTransform.sizeDelta.x, this.descriptionText.rectTransform.sizeDelta.y + 20f);
			num += 20f;
		}
		this.rect.sizeDelta = new Vector2(this.rect.sizeDelta.x, num);
		Strings.LinkData linkData2 = Strings.AddOrGetLink(EvidenceCreator.Instance.GetTimeEvidence(this.salesRecord.time, this.salesRecord.time, "time", this.salesLedger.parentWindow.passedEvidence.evID, -1, -1), null);
		this.timeText.text = string.Concat(new string[]
		{
			"<link=",
			linkData2.id.ToString(),
			">",
			SessionData.Instance.ShortDateString(this.salesRecord.time, false),
			" ",
			SessionData.Instance.GameTimeToClock24String(this.salesRecord.time, false),
			"</link>"
		});
		this.priceText.text = CityControls.Instance.cityCurrency + Toolbox.Instance.RoundToPlaces(this.salesRecord.cost, 2).ToString();
	}

	// Token: 0x040028DD RID: 10461
	public RectTransform rect;

	// Token: 0x040028DE RID: 10462
	public SalesLedgerContentController salesLedger;

	// Token: 0x040028DF RID: 10463
	public Company.SalesRecord salesRecord;

	// Token: 0x040028E0 RID: 10464
	public TextMeshProUGUI descriptionText;

	// Token: 0x040028E1 RID: 10465
	public TextMeshProUGUI timeText;

	// Token: 0x040028E2 RID: 10466
	public TextMeshProUGUI nameText;

	// Token: 0x040028E3 RID: 10467
	public TextMeshProUGUI priceText;
}
