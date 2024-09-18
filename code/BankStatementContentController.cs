using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000547 RID: 1351
public class BankStatementContentController : MonoBehaviour
{
	// Token: 0x06001D76 RID: 7542 RVA: 0x001A084C File Offset: 0x0019EA4C
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
		if (this.parentWindow != null)
		{
			if (this.parentWindow.passedEvidence != null)
			{
				this.parentWindow.passedEvidence.OnDataKeyChange += this.CheckEnabled;
			}
			this.parentWindow.OnWindowRefresh += this.CheckEnabled;
		}
		this.CheckEnabled();
	}

	// Token: 0x06001D77 RID: 7543 RVA: 0x001A08EC File Offset: 0x0019EAEC
	private void OnDisable()
	{
		if (this.parentWindow == null)
		{
			this.parentWindow = base.gameObject.GetComponentInParent<InfoWindow>();
		}
		this.parentWindow.passedEvidence.OnDataKeyChange -= this.CheckEnabled;
		this.parentWindow.OnWindowRefresh -= this.CheckEnabled;
	}

	// Token: 0x06001D78 RID: 7544 RVA: 0x001A094C File Offset: 0x0019EB4C
	public void CheckEnabled()
	{
		while (this.spawnedEntries.Count > 0)
		{
			Object.Destroy(this.spawnedEntries[0].gameObject);
			this.spawnedEntries.RemoveAt(0);
		}
		if (this.parentWindow != null && this.parentWindow.passedEvidence != null)
		{
			Human reciever = this.parentWindow.passedEvidence.reciever;
			List<BankStatementContentController.Transaction> list = new List<BankStatementContentController.Transaction>();
			List<int> list2 = new List<int>();
			List<string> list3 = reciever.ParseDDSMessage(this.transactionMessageID, null, out list2, false, reciever, true);
			Game.Log(string.Concat(new string[]
			{
				"Bank statement: Parsed ",
				list3.Count.ToString(),
				" blocks from ",
				this.transactionMessageID,
				" ",
				reciever.citizenName
			}), 2);
			for (int i = 0; i < list3.Count; i++)
			{
				string text = list3[i];
				int num = 1;
				try
				{
					num = list2[i];
				}
				catch
				{
				}
				int amount = -Toolbox.Instance.GetPsuedoRandomNumber((num - 1) * 50, num * 50, text, false);
				if (num == 52 && reciever.job != null)
				{
					amount = Mathf.RoundToInt(reciever.job.salary * 1000f / 12f);
				}
				list.Add(new BankStatementContentController.Transaction
				{
					text = Strings.ComposeText(Strings.Get("dds.blocks", text, Strings.Casing.asIs, false, false, false, null), reciever, Strings.LinkSetting.automatic, null, null, false),
					amount = amount
				});
			}
			for (int j = 0; j < list.Count; j++)
			{
				BankStatementContentController.Transaction transaction = list[j];
				int psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(j, list.Count, reciever.citizenName + CityData.Instance.seed, false);
				list[j] = list[psuedoRandomNumber];
				list[psuedoRandomNumber] = transaction;
			}
			float num2 = 1f;
			using (List<CharacterTrait>.Enumerator enumerator = CitizenControls.Instance.savingsBoostTrait.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterTrait t = enumerator.Current;
					if (reciever.characterTraits.Exists((Human.Trait item) => item.trait == t))
					{
						num2 += 0.1f;
					}
				}
			}
			using (List<CharacterTrait>.Enumerator enumerator = CitizenControls.Instance.savingsDebuffTrait.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterTrait t = enumerator.Current;
					if (reciever.characterTraits.Exists((Human.Trait item) => item.trait == t))
					{
						num2 -= 0.1f;
					}
				}
			}
			int num3 = Mathf.RoundToInt(CitizenControls.Instance.societalClassSavingsCurve.Evaluate(reciever.societalClass * num2));
			float num4 = 0f;
			for (int k = 0; k < list.Count; k++)
			{
				BankStatementContentController.Transaction transaction2 = list[k];
				BankStatementEntryController component = Object.Instantiate<GameObject>(this.entryPrefab, base.transform).GetComponent<BankStatementEntryController>();
				num3 += transaction2.amount;
				component.Setup(transaction2.text, transaction2.amount, num3);
				num4 += component.rect.sizeDelta.y;
				this.spawnedEntries.Add(component);
			}
			if (this.windowContent != null && this.windowContent.pageRect != null)
			{
				Vector2 vector;
				vector..ctor(this.windowContent.pageRect.sizeDelta.x, Mathf.Max(this.windowContent.pageRect.sizeDelta.y, num4 + 100f));
				string text2 = "Bank statement: New scale: ";
				Vector2 vector2 = vector;
				Game.Log(text2 + vector2.ToString(), 2);
				this.windowContent.pageRect.sizeDelta = vector;
				this.windowContent.rect.sizeDelta = vector;
				this.windowContent.normalSize = vector;
				this.windowContent.UpdateFitScale();
			}
		}
	}

	// Token: 0x04002730 RID: 10032
	public WindowContentController windowContent;

	// Token: 0x04002731 RID: 10033
	public InfoWindow parentWindow;

	// Token: 0x04002732 RID: 10034
	public TextMeshProUGUI descriptionText;

	// Token: 0x04002733 RID: 10035
	public GameObject entryPrefab;

	// Token: 0x04002734 RID: 10036
	public string transactionMessageID;

	// Token: 0x04002735 RID: 10037
	public VerticalLayoutGroup entryLayoutGroup;

	// Token: 0x04002736 RID: 10038
	public List<BankStatementEntryController> spawnedEntries = new List<BankStatementEntryController>();

	// Token: 0x02000548 RID: 1352
	public class Transaction
	{
		// Token: 0x04002737 RID: 10039
		public string text;

		// Token: 0x04002738 RID: 10040
		public int amount;
	}
}
