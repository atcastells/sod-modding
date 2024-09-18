using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Token: 0x0200058D RID: 1421
public class SalesLedgerContentController : MonoBehaviour
{
	// Token: 0x06001F0D RID: 7949 RVA: 0x001AE59C File Offset: 0x001AC79C
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

	// Token: 0x06001F0E RID: 7950 RVA: 0x001AE620 File Offset: 0x001AC820
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001F0F RID: 7951 RVA: 0x001AE680 File Offset: 0x001AC880
	public void CheckEnabled()
	{
		while (this.spawnedEntries.Count > 0)
		{
			Object.Destroy(this.spawnedEntries[0].gameObject);
			this.spawnedEntries.RemoveAt(0);
		}
		Company company = this.parentWindow.passedInteractable.node.gameLocation.thisAsAddress.company;
		company.sales.Sort((Company.SalesRecord p1, Company.SalesRecord p2) => p1.time.CompareTo(p2.time));
		float num = 0f;
		for (int i = 0; i < company.sales.Count; i++)
		{
			Company.SalesRecord newRecord = company.sales[i];
			SalesLedgerEntryController component = Object.Instantiate<GameObject>(this.entryPrefab, base.transform).GetComponent<SalesLedgerEntryController>();
			component.Setup(newRecord, this);
			component.rect.anchoredPosition = new Vector2(component.rect.anchoredPosition.x, num);
			num -= component.rect.sizeDelta.y;
			this.spawnedEntries.Add(component);
		}
		num -= 100f;
		this.windowContent.pageRect.sizeDelta = new Vector2(this.windowContent.pageRect.sizeDelta.x, Mathf.Max(this.windowContent.pageRect.sizeDelta.y, -num));
		this.windowContent.rect.sizeDelta = new Vector2(this.windowContent.pageRect.sizeDelta.x, Mathf.Max(this.windowContent.pageRect.sizeDelta.y, -num));
	}

	// Token: 0x040028D4 RID: 10452
	public WindowContentController windowContent;

	// Token: 0x040028D5 RID: 10453
	public InfoWindow parentWindow;

	// Token: 0x040028D6 RID: 10454
	public TextMeshProUGUI descriptionText;

	// Token: 0x040028D7 RID: 10455
	public GameObject entryPrefab;

	// Token: 0x040028D8 RID: 10456
	public List<SalesLedgerEntryController> spawnedEntries = new List<SalesLedgerEntryController>();

	// Token: 0x0200058E RID: 1422
	public class Transaction
	{
		// Token: 0x040028D9 RID: 10457
		public string text;

		// Token: 0x040028DA RID: 10458
		public int amount;
	}
}
