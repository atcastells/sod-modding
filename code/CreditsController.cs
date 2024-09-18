using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x020004F2 RID: 1266
public class CreditsController : MonoBehaviour
{
	// Token: 0x170000C8 RID: 200
	// (get) Token: 0x06001B4F RID: 6991 RVA: 0x0018CAEB File Offset: 0x0018ACEB
	public static CreditsController Instance
	{
		get
		{
			return CreditsController._instance;
		}
	}

	// Token: 0x06001B50 RID: 6992 RVA: 0x0018CAF2 File Offset: 0x0018ACF2
	private void Awake()
	{
		if (CreditsController._instance != null && CreditsController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			CreditsController._instance = this;
		}
		base.enabled = false;
	}

	// Token: 0x06001B51 RID: 6993 RVA: 0x0018CB28 File Offset: 0x0018AD28
	public string GetFormattedCreditsText()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < this.credits.Count; i++)
		{
			CreditsController.CreditCategory creditCategory = this.credits[i];
			string text = creditCategory.name;
			if (creditCategory.localize)
			{
				text = Strings.Get("ui.interface", text, Strings.Casing.asIs, false, false, false, null);
			}
			text = "<uppercase><size=120%><b><u>" + text + "</u></b><size=100%></uppercase>";
			stringBuilder.Append(text);
			if (creditCategory.extra != null && creditCategory.extra.Length > 0)
			{
				string text2 = creditCategory.extra;
				if (creditCategory.localizeExtra)
				{
					text2 = Strings.Get("ui.interface", text2, Strings.Casing.asIs, false, false, false, null);
				}
				text2 = "<line-height=150%><smallcaps>" + text2 + "</smallcaps><line-height=100%>";
				stringBuilder.AppendLine();
				stringBuilder.Append(text2);
			}
			stringBuilder.AppendLine();
			for (int j = 0; j < creditCategory.credits.Count; j++)
			{
				CreditsController.CreditEntry creditEntry = creditCategory.credits[j];
				stringBuilder.AppendLine();
				if (creditEntry.title != null && creditEntry.title.Length > 0)
				{
					stringBuilder.Append("<smallcaps><size=100%><b>" + Strings.Get("ui.interface", creditEntry.title, Strings.Casing.asIs, false, false, false, null) + "</b><size=100%></smallcaps>");
				}
				stringBuilder.AppendLine();
				for (int k = 0; k < creditEntry.names.Count; k++)
				{
					CreditsController.CreditName creditName = creditEntry.names[k];
					string text3 = string.Empty;
					if (creditName.additional != null && creditName.additional.Length > 0)
					{
						text3 = Strings.Get("ui.interface", creditName.additional, Strings.Casing.asIs, false, false, false, null) + ": ";
					}
					text3 += creditName.name;
					stringBuilder.Append("<size=90%>" + text3 + "<size=100%>");
					stringBuilder.AppendLine();
				}
			}
			stringBuilder.AppendLine();
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04002413 RID: 9235
	[Header("Credits")]
	public List<CreditsController.CreditCategory> credits = new List<CreditsController.CreditCategory>();

	// Token: 0x04002414 RID: 9236
	private static CreditsController _instance;

	// Token: 0x020004F3 RID: 1267
	[Serializable]
	public class CreditCategory
	{
		// Token: 0x04002415 RID: 9237
		public string name;

		// Token: 0x04002416 RID: 9238
		public bool localize;

		// Token: 0x04002417 RID: 9239
		public string extra;

		// Token: 0x04002418 RID: 9240
		public bool localizeExtra;

		// Token: 0x04002419 RID: 9241
		public List<CreditsController.CreditEntry> credits = new List<CreditsController.CreditEntry>();
	}

	// Token: 0x020004F4 RID: 1268
	[Serializable]
	public class CreditEntry
	{
		// Token: 0x0400241A RID: 9242
		public string title;

		// Token: 0x0400241B RID: 9243
		public List<CreditsController.CreditName> names = new List<CreditsController.CreditName>();
	}

	// Token: 0x020004F5 RID: 1269
	[Serializable]
	public class CreditName
	{
		// Token: 0x0400241C RID: 9244
		public string name;

		// Token: 0x0400241D RID: 9245
		public string additional;
	}
}
