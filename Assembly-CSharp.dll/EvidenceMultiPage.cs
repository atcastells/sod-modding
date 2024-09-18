using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000651 RID: 1617
public class EvidenceMultiPage : Evidence
{
	// Token: 0x1400005B RID: 91
	// (add) Token: 0x060023D6 RID: 9174 RVA: 0x001DB860 File Offset: 0x001D9A60
	// (remove) Token: 0x060023D7 RID: 9175 RVA: 0x001DB898 File Offset: 0x001D9A98
	public event EvidenceMultiPage.PageChanged OnPageChanged;

	// Token: 0x060023D8 RID: 9176 RVA: 0x001DB8CD File Offset: 0x001D9ACD
	public EvidenceMultiPage(EvidencePreset newPreset, string evID, Controller newController, List<object> newPassedObjects) : base(newPreset, evID, newController, newPassedObjects)
	{
		GameplayController.Instance.multiPageEvidence.Add(this);
	}

	// Token: 0x060023D9 RID: 9177 RVA: 0x001DB8F8 File Offset: 0x001D9AF8
	public int AddStringContentToNewPage(string newStr, string appendSeperation = "\n\n", int order = -1)
	{
		int num = -1;
		num = this.pageContent.FindIndex((EvidenceMultiPage.MultiPageContent item) => item.page == this.page && newStr == item.str);
		if (num > -1)
		{
			return num;
		}
		num = 0;
		foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.pageContent)
		{
			num = Mathf.Max(num, multiPageContent.page + 1);
		}
		this.AddStringContentToPage(num, newStr, appendSeperation, order);
		return num;
	}

	// Token: 0x060023DA RID: 9178 RVA: 0x001DB99C File Offset: 0x001D9B9C
	public void AddStringContentToPage(int page, string newStr, string appendSeperation = "\n\n", int order = -1)
	{
		if (this.pageContent == null)
		{
			this.pageContent = new List<EvidenceMultiPage.MultiPageContent>();
		}
		else if (this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => item.page == page && newStr == item.str))
		{
			return;
		}
		EvidenceMultiPage.MultiPageContent multiPageContent = new EvidenceMultiPage.MultiPageContent();
		multiPageContent.page = page;
		multiPageContent.str = newStr;
		multiPageContent.seperation = appendSeperation;
		multiPageContent.order = order;
		this.pageContent.Add(multiPageContent);
	}

	// Token: 0x060023DB RID: 9179 RVA: 0x001DBA28 File Offset: 0x001D9C28
	public int AddContainedMetaObjectToNewPage(MetaObject containedMetaObject)
	{
		int num = -1;
		num = this.pageContent.FindIndex((EvidenceMultiPage.MultiPageContent item) => containedMetaObject.id == item.meta);
		if (num > -1)
		{
			return num;
		}
		num = 0;
		foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.pageContent)
		{
			num = Mathf.Max(num, multiPageContent.page + 1);
		}
		this.AddContainedMetaObjectToPage(num, containedMetaObject);
		return num;
	}

	// Token: 0x060023DC RID: 9180 RVA: 0x001DBAC0 File Offset: 0x001D9CC0
	public void AddContainedMetaObjectToPage(int page, MetaObject containedMetaObject)
	{
		if (this.pageContent == null)
		{
			this.pageContent = new List<EvidenceMultiPage.MultiPageContent>();
		}
		else if (this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => containedMetaObject.id == item.meta))
		{
			return;
		}
		EvidenceMultiPage.MultiPageContent multiPageContent = new EvidenceMultiPage.MultiPageContent();
		multiPageContent.page = page;
		multiPageContent.meta = containedMetaObject.id;
		this.pageContent.Add(multiPageContent);
	}

	// Token: 0x060023DD RID: 9181 RVA: 0x001DBB34 File Offset: 0x001D9D34
	public int AddEvidenceToNewPage(Evidence evidenceToAdd)
	{
		int num = -1;
		num = this.pageContent.FindIndex((EvidenceMultiPage.MultiPageContent item) => item.page == this.page && evidenceToAdd.evID == item.evID);
		if (num > -1)
		{
			return num;
		}
		num = 0;
		foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.pageContent)
		{
			num = Mathf.Max(num, multiPageContent.page + 1);
		}
		this.AddEvidenceToPage(num, evidenceToAdd);
		return num;
	}

	// Token: 0x060023DE RID: 9182 RVA: 0x001DBBD4 File Offset: 0x001D9DD4
	public void AddEvidenceToPage(int page, Evidence evidenceToAdd)
	{
		if (this.pageContent == null)
		{
			this.pageContent = new List<EvidenceMultiPage.MultiPageContent>();
		}
		else if (this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => item.page == page && evidenceToAdd.evID == item.evID))
		{
			return;
		}
		EvidenceMultiPage.MultiPageContent multiPageContent = new EvidenceMultiPage.MultiPageContent();
		multiPageContent.page = page;
		multiPageContent.evID = evidenceToAdd.evID;
		this.pageContent.Add(multiPageContent);
	}

	// Token: 0x060023DF RID: 9183 RVA: 0x001DBC54 File Offset: 0x001D9E54
	public int AddEvidenceDiscoveryToNewPage(Evidence evidenceToApplyTo, Evidence.Discovery discovery)
	{
		int num = -1;
		num = this.pageContent.FindIndex((EvidenceMultiPage.MultiPageContent item) => item.page == this.page && evidenceToApplyTo.evID == item.discEvID && item.disc == discovery);
		if (num > -1)
		{
			return num;
		}
		num = 0;
		foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.pageContent)
		{
			num = Mathf.Max(num, multiPageContent.page + 1);
		}
		this.AddEvidenceDiscoveryToPage(num, evidenceToApplyTo, discovery);
		return num;
	}

	// Token: 0x060023E0 RID: 9184 RVA: 0x001DBD00 File Offset: 0x001D9F00
	public void AddEvidenceDiscoveryToPage(int page, Evidence evidenceToApplyTo, Evidence.Discovery discovery)
	{
		if (this.pageContent == null)
		{
			this.pageContent = new List<EvidenceMultiPage.MultiPageContent>();
		}
		else if (this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => item.page == page && evidenceToApplyTo.evID == item.discEvID && item.disc == discovery))
		{
			return;
		}
		EvidenceMultiPage.MultiPageContent multiPageContent = new EvidenceMultiPage.MultiPageContent();
		multiPageContent.page = page;
		multiPageContent.discEvID = evidenceToApplyTo.evID;
		multiPageContent.disc = discovery;
		this.pageContent.Add(multiPageContent);
	}

	// Token: 0x060023E1 RID: 9185 RVA: 0x001DBD94 File Offset: 0x001D9F94
	public void SetPage(int newPage, bool loopPages)
	{
		if (newPage != this.page)
		{
			if (this.pageContent.Count > 0)
			{
				if (this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => item.page == newPage))
				{
					this.page = newPage;
				}
				else if (loopPages)
				{
					int num = 999999;
					int num2 = -999999;
					foreach (EvidenceMultiPage.MultiPageContent multiPageContent in this.pageContent)
					{
						if (multiPageContent.page < num)
						{
							num = multiPageContent.page;
						}
						if (multiPageContent.page > num2)
						{
							num2 = multiPageContent.page;
						}
					}
					int tryPage = newPage;
					do
					{
						if (tryPage < num2)
						{
							int tryPage2 = tryPage;
							tryPage = tryPage2 + 1;
						}
						else
						{
							tryPage = num;
						}
					}
					while (!this.pageContent.Exists((EvidenceMultiPage.MultiPageContent item) => item.page == tryPage));
					this.page = tryPage;
				}
			}
			else
			{
				this.page = 0;
			}
			if (this.OnPageChanged != null)
			{
				this.OnPageChanged(this.page);
			}
		}
	}

	// Token: 0x060023E2 RID: 9186 RVA: 0x001DBEF0 File Offset: 0x001DA0F0
	public List<EvidenceMultiPage.MultiPageContent> GetContentForPage(int newPage)
	{
		List<EvidenceMultiPage.MultiPageContent> list = this.pageContent.FindAll((EvidenceMultiPage.MultiPageContent item) => item.page == newPage);
		list.Sort((EvidenceMultiPage.MultiPageContent p1, EvidenceMultiPage.MultiPageContent p2) => p1.order.CompareTo(p2.order));
		Game.Log(string.Concat(new string[]
		{
			"Returning ",
			list.Count.ToString(),
			" content for page ",
			newPage.ToString(),
			"..."
		}), 2);
		return list;
	}

	// Token: 0x060023E3 RID: 9187 RVA: 0x001DBF90 File Offset: 0x001DA190
	public string GetCurrentPageStringContent()
	{
		string text = string.Empty;
		List<EvidenceMultiPage.MultiPageContent> list = this.pageContent.FindAll((EvidenceMultiPage.MultiPageContent item) => item.page == this.page);
		list.Sort((EvidenceMultiPage.MultiPageContent p1, EvidenceMultiPage.MultiPageContent p2) => p1.order.CompareTo(p2.order));
		for (int i = 0; i < list.Count; i++)
		{
			EvidenceMultiPage.MultiPageContent multiPageContent = list[i];
			if (i > 0)
			{
				text += multiPageContent.seperation;
			}
			text += multiPageContent.str;
		}
		return text.Trim();
	}

	// Token: 0x04002DC8 RID: 11720
	public List<EvidenceMultiPage.MultiPageContent> pageContent = new List<EvidenceMultiPage.MultiPageContent>();

	// Token: 0x04002DC9 RID: 11721
	public int page;

	// Token: 0x02000652 RID: 1618
	[Serializable]
	public class MultiPageContent
	{
		// Token: 0x060023E5 RID: 9189 RVA: 0x001DC030 File Offset: 0x001DA230
		public Evidence GetEvidence()
		{
			Evidence result = null;
			if (this.evID != null && this.evID.Length > 0)
			{
				Toolbox.Instance.TryGetEvidence(this.evID, out result);
			}
			return result;
		}

		// Token: 0x04002DCB RID: 11723
		public int page;

		// Token: 0x04002DCC RID: 11724
		public string evID;

		// Token: 0x04002DCD RID: 11725
		public int meta;

		// Token: 0x04002DCE RID: 11726
		public string discEvID;

		// Token: 0x04002DCF RID: 11727
		public Evidence.Discovery disc;

		// Token: 0x04002DD0 RID: 11728
		public string seperation = "\n\n";

		// Token: 0x04002DD1 RID: 11729
		public string str;

		// Token: 0x04002DD2 RID: 11730
		public int order = -1;
	}

	// Token: 0x02000653 RID: 1619
	// (Invoke) Token: 0x060023E8 RID: 9192
	public delegate void PageChanged(int newPage);
}
