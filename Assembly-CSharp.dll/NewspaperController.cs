using System;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

// Token: 0x0200039A RID: 922
public class NewspaperController : MonoBehaviour
{
	// Token: 0x1700009A RID: 154
	// (get) Token: 0x060014FC RID: 5372 RVA: 0x00132FAA File Offset: 0x001311AA
	public static NewspaperController Instance
	{
		get
		{
			return NewspaperController._instance;
		}
	}

	// Token: 0x060014FD RID: 5373 RVA: 0x00132FB1 File Offset: 0x001311B1
	private void Awake()
	{
		if (NewspaperController._instance != null && NewspaperController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NewspaperController._instance = this;
	}

	// Token: 0x060014FE RID: 5374 RVA: 0x00132FE0 File Offset: 0x001311E0
	public void UpdateNewspaperReferences(NewspaperDisplayController disp)
	{
		Game.Log("Updating newspaper references...", 2);
		if (disp.newspaperTitleText != null)
		{
			this.newspaperTitleText = disp.newspaperTitleText;
		}
		if (disp.newspaperDateText != null)
		{
			this.newspaperDateText = disp.newspaperDateText;
		}
		if (disp.mainArticleHeadline != null)
		{
			this.mainArticleHeadline = disp.mainArticleHeadline;
		}
		if (disp.mainArticleColumn1 != null)
		{
			this.mainArticleColumn1 = disp.mainArticleColumn1;
		}
		if (disp.mainArticleColumn2 != null)
		{
			this.mainArticleColumn2 = disp.mainArticleColumn2;
		}
		if (disp.mainArticleColumn3 != null)
		{
			this.mainArticleColumn3 = disp.mainArticleColumn3;
		}
		if (disp.article2Headline != null)
		{
			this.article2Headline = disp.article2Headline;
		}
		if (disp.article2Column1 != null)
		{
			this.article2Column1 = disp.article2Column1;
		}
		if (disp.article2Column2 != null)
		{
			this.article2Column2 = disp.article2Column2;
		}
		if (disp.article2Column3 != null)
		{
			this.article2Column3 = disp.article2Column3;
		}
		if (disp.article3Headline != null)
		{
			this.article3Headline = disp.article3Headline;
		}
		if (disp.article3Column1 != null)
		{
			this.article3Column1 = disp.article3Column1;
		}
		if (disp.article3Column2 != null)
		{
			this.article3Column2 = disp.article3Column2;
		}
		if (disp.ad1Text != null)
		{
			this.ad1Text = disp.ad1Text;
		}
		if (disp.ad2Text != null)
		{
			this.ad2Text = disp.ad2Text;
		}
		if (disp.ad3Text != null)
		{
			this.ad3Text = disp.ad3Text;
		}
		if (disp.ad4Text != null)
		{
			this.ad4Text = disp.ad4Text;
		}
		this.UpdateText(false);
	}

	// Token: 0x060014FF RID: 5375 RVA: 0x001331BC File Offset: 0x001313BC
	public void UpdateText(bool updateNewsTicker = false)
	{
		if (this.currentState.mainArticle == null || this.currentState.mainArticle.Length <= 0)
		{
			this.currentState.SerializeFields();
		}
		Game.Log("Gameplay: Updating newspaper text...", 2);
		this.SetTextForArticle(this.currentState.mainArticle, this.currentState.mainContext, this.mainArticleHeadline, new TextMeshProUGUI[]
		{
			this.mainArticleColumn1,
			this.mainArticleColumn2,
			this.mainArticleColumn3
		}, "\n\n");
		this.SetTextForArticle(this.currentState.article2, this.currentState.art2Context, this.article2Headline, new TextMeshProUGUI[]
		{
			this.article2Column1,
			this.article2Column2,
			this.article2Column3
		}, "\n\n");
		this.SetTextForArticle(this.currentState.article3, this.currentState.art3Context, this.article3Headline, new TextMeshProUGUI[]
		{
			this.article3Column1,
			this.article3Column2
		}, "\n\n");
		this.SetAdText(this.currentState.ad1, this.currentState.ad1Context, this.ad1Text);
		this.SetAdText(this.currentState.ad2, this.currentState.ad2Context, this.ad2Text);
		this.SetAdText(this.currentState.ad3, this.currentState.ad3Context, this.ad3Text);
		this.SetAdText(this.currentState.ad4, this.currentState.ad4Context, this.ad4Text);
		if (this.newspaperTitleText != null)
		{
			this.newspaperTitleText.text = Strings.ComposeText(Strings.Get("misc", "The |city.name| Herald", Strings.Casing.asIs, false, false, false, null), null, Strings.LinkSetting.forceNoLinks, null, null, false);
		}
		if (this.newspaperDateText != null)
		{
			this.newspaperDateText.text = SessionData.Instance.LongDateString(this.currentState.time, true, false, true, false, true, true, false, true);
		}
		if (updateNewsTicker)
		{
			string[] array = Strings.GetTextForComponent(this.currentState.mainArticle, this.GetContextObject(this.currentState.mainContext, this.currentState.seed), null, null, "\n", false, null, Strings.LinkSetting.forceNoLinks, null).Split('\n', 0);
			string text = string.Empty;
			if (array.Length != 0)
			{
				text = array[0];
			}
			if (array.Length > 1)
			{
				text = text + ": " + array[1];
			}
			TextToImageController.Instance.UpdateNewsTickerHeadline(text);
		}
	}

	// Token: 0x06001500 RID: 5376 RVA: 0x00133434 File Offset: 0x00131634
	public void SetTextForArticle(string msgID, int context, TextMeshProUGUI headline, TextMeshProUGUI[] columns, string lineBreaks = "\n\n")
	{
		if (headline == null || columns == null || columns.Length == 0)
		{
			return;
		}
		string text = Strings.GetTextForComponent(msgID, this.GetContextObject(context, this.currentState.seed), null, null, lineBreaks, false, null, Strings.LinkSetting.forceNoLinks, null);
		string[] array = text.Split(new char[]
		{
			'\n'
		}, 1);
		if (array.Length != 0)
		{
			headline.text = array[0];
			text = text.Substring(array[0].Length, text.Length - array[0].Length);
			string[] array2 = text.Split(' ', 0);
			int num = Mathf.CeilToInt((float)array2.Length / (float)columns.Length);
			int i = 0;
			int num2 = 0;
			int num3 = 0;
			string text2 = string.Empty;
			while (i < array2.Length)
			{
				if (num3 < columns.Length && i < num && num2 < array2.Length)
				{
					text2 = text2 + " " + array2[num2];
					i++;
					num2++;
				}
				else
				{
					if (num3 >= columns.Length)
					{
						break;
					}
					columns[num3].text = text2.Trim();
					num3++;
					i = 0;
					text2 = string.Empty;
				}
			}
		}
	}

	// Token: 0x06001501 RID: 5377 RVA: 0x00133548 File Offset: 0x00131748
	private object GetContextObject(int contextEnum, string seed)
	{
		object result = null;
		if (contextEnum == 1)
		{
			if (this.currentState.murderID > -1)
			{
				MurderController.Murder murder = MurderController.Instance.activeMurders.Find((MurderController.Murder item) => item.murderID == this.currentState.murderID);
				if (murder == null)
				{
					murder = MurderController.Instance.inactiveMurders.Find((MurderController.Murder item) => item.murderID == this.currentState.murderID);
				}
				result = murder;
			}
			else
			{
				Game.Log("Gameplay: Unable to find murder context for " + this.currentState.murderID.ToString(), 2);
			}
		}
		else if (contextEnum == 2)
		{
			result = Player.Instance;
		}
		else if (contextEnum == 3)
		{
			result = CityData.Instance.citizenDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.citizenDirectory.Count, seed, false)];
		}
		else if (contextEnum == 4)
		{
			List<Citizen> list = CityData.Instance.citizenDirectory.FindAll((Citizen item) => item.job != null && item.job.preset != null && item.job.preset.isCriminal);
			result = list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, seed, false)];
		}
		else if (contextEnum == 5)
		{
			result = GroupsController.Instance.groups[Toolbox.Instance.GetPsuedoRandomNumber(0, GroupsController.Instance.groups.Count, seed, false)];
		}
		return result;
	}

	// Token: 0x06001502 RID: 5378 RVA: 0x00133694 File Offset: 0x00131894
	public void SetAdText(string msgID, int context, TextMeshProUGUI adText)
	{
		if (adText != null)
		{
			adText.text = Strings.GetTextForComponent(msgID, this.GetContextObject(context, this.currentState.seed), null, null, string.Empty, false, null, Strings.LinkSetting.forceNoLinks, null);
		}
	}

	// Token: 0x06001503 RID: 5379 RVA: 0x001336D4 File Offset: 0x001318D4
	[Button(null, 0)]
	public void GenerateNewNewspaper()
	{
		this.currentState = new NewspaperController.NewspaperState();
		this.currentState.time = SessionData.Instance.gameTime;
		this.currentState.seed = Toolbox.Instance.GenerateSeed(16, false, "");
		MurderController.Murder murderRef = null;
		foreach (MurderController.Murder murder in MurderController.Instance.activeMurders)
		{
			if (murder.death != null && murder.death.reported && (this.currentState.murderID <= -1 || murder.murderID > this.currentState.murderID))
			{
				this.currentState.murderID = murder.murderID;
				murderRef = murder;
			}
		}
		foreach (MurderController.Murder murder2 in MurderController.Instance.inactiveMurders)
		{
			if (murder2.death != null && murder2.death.reported && (this.currentState.murderID <= -1 || murder2.murderID > this.currentState.murderID))
			{
				this.currentState.murderID = murder2.murderID;
				murderRef = murder2;
			}
		}
		bool murderSecond = false;
		if (murderRef != null)
		{
			if (MurderController.Instance.activeMurders.Exists((MurderController.Murder item) => item != murderRef && item.murdererID == murderRef.murdererID))
			{
				murderSecond = true;
			}
			else if (MurderController.Instance.inactiveMurders.Exists((MurderController.Murder item) => item != murderRef && item.murdererID == murderRef.murdererID))
			{
				murderSecond = true;
			}
		}
		Game.Log("Gameplay: Generating a newspaper using " + Toolbox.Instance.allArticleTrees.Count.ToString() + " articles...", 2);
		Toolbox.Instance.allArticleTrees.FindAll((DDSSaveClasses.DDSTreeSave item) => item.triggerPoint == DDSSaveClasses.TriggerPoint.newspaperArticle);
		List<DDSSaveClasses.DDSTreeSave> list = Toolbox.Instance.allArticleTrees.FindAll((DDSSaveClasses.DDSTreeSave item) => item.triggerPoint == DDSSaveClasses.TriggerPoint.newspaperArticle && ((this.currentState.murderID > -1 && !murderSecond && item.newspaperCategory == 3) || ((this.currentState.murderID > -1 & murderSecond) && item.newspaperCategory == 4) || (this.currentState.murderID <= -1 && item.newspaperCategory == 0)));
		List<string> list2 = new List<string>();
		if (list.Count > 0)
		{
			DDSSaveClasses.DDSMessageSettings ddsmessageSettings = null;
			int mainContext = 0;
			if (this.PickArticleFromTrees(ref list, out ddsmessageSettings, out mainContext, list2))
			{
				this.currentState.mainArticle = ddsmessageSettings.msgID;
				this.currentState.mainContext = mainContext;
				list2.Add(ddsmessageSettings.msgID);
			}
		}
		List<DDSSaveClasses.DDSTreeSave> list3 = Toolbox.Instance.allArticleTrees.FindAll((DDSSaveClasses.DDSTreeSave item) => item.triggerPoint == DDSSaveClasses.TriggerPoint.newspaperArticle && item.newspaperCategory == 0);
		if (list3.Count > 0)
		{
			DDSSaveClasses.DDSMessageSettings ddsmessageSettings2 = null;
			int art2Context = 0;
			if (this.PickArticleFromTrees(ref list3, out ddsmessageSettings2, out art2Context, list2))
			{
				this.currentState.article2 = ddsmessageSettings2.msgID;
				this.currentState.art2Context = art2Context;
				list2.Add(ddsmessageSettings2.msgID);
			}
		}
		List<DDSSaveClasses.DDSTreeSave> list4 = Toolbox.Instance.allArticleTrees.FindAll((DDSSaveClasses.DDSTreeSave item) => item.triggerPoint == DDSSaveClasses.TriggerPoint.newspaperArticle && item.newspaperCategory == 2);
		if (list4.Count > 0)
		{
			DDSSaveClasses.DDSMessageSettings ddsmessageSettings3 = null;
			int art3Context = 0;
			if (this.PickArticleFromTrees(ref list4, out ddsmessageSettings3, out art3Context, list2))
			{
				this.currentState.article3 = ddsmessageSettings3.msgID;
				this.currentState.art3Context = art3Context;
				list2.Add(ddsmessageSettings3.msgID);
			}
		}
		List<DDSSaveClasses.DDSTreeSave> list5 = Toolbox.Instance.allArticleTrees.FindAll((DDSSaveClasses.DDSTreeSave item) => item.triggerPoint == DDSSaveClasses.TriggerPoint.newspaperArticle && item.newspaperCategory == 1);
		if (list5.Count > 0)
		{
			DDSSaveClasses.DDSMessageSettings ddsmessageSettings4 = null;
			int num = 0;
			if (this.PickArticleFromTrees(ref list5, out ddsmessageSettings4, out num, list2))
			{
				this.currentState.ad1 = ddsmessageSettings4.msgID;
				this.currentState.ad1Context = num;
				list2.Add(ddsmessageSettings4.msgID);
				if (this.PickArticleFromTrees(ref list5, out ddsmessageSettings4, out num, list2))
				{
					this.currentState.ad2 = ddsmessageSettings4.msgID;
					this.currentState.ad2Context = num;
					list2.Add(ddsmessageSettings4.msgID);
					if (this.PickArticleFromTrees(ref list5, out ddsmessageSettings4, out num, list2))
					{
						this.currentState.ad3 = ddsmessageSettings4.msgID;
						this.currentState.ad3Context = num;
						list2.Add(ddsmessageSettings4.msgID);
						if (this.PickArticleFromTrees(ref list5, out ddsmessageSettings4, out num, list2))
						{
							this.currentState.ad4 = ddsmessageSettings4.msgID;
							this.currentState.ad4Context = num;
							list2.Add(ddsmessageSettings4.msgID);
						}
					}
				}
			}
		}
		this.UpdateText(true);
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00133BB0 File Offset: 0x00131DB0
	private bool PickArticleFromTrees(ref List<DDSSaveClasses.DDSTreeSave> trees, out DDSSaveClasses.DDSMessageSettings pickedArticle, out int pickedContext, List<string> ignoreMsgIDs = null)
	{
		pickedArticle = null;
		pickedContext = 0;
		List<DDSSaveClasses.DDSMessageSettings> list = new List<DDSSaveClasses.DDSMessageSettings>();
		List<int> list2 = new List<int>();
		foreach (DDSSaveClasses.DDSTreeSave ddstreeSave in trees)
		{
			foreach (DDSSaveClasses.DDSMessageSettings ddsmessageSettings in ddstreeSave.messages)
			{
				if (ignoreMsgIDs == null || !ignoreMsgIDs.Contains(ddsmessageSettings.msgID))
				{
					list.Add(ddsmessageSettings);
					list2.Add(ddstreeSave.newspaperContext);
				}
			}
		}
		if (list.Count > 0)
		{
			int num = Toolbox.Instance.Rand(0, list.Count, false);
			pickedArticle = list[num];
			pickedContext = list2[num];
			return true;
		}
		return false;
	}

	// Token: 0x040019D9 RID: 6617
	[Header("Components")]
	public TextMeshProUGUI newspaperTitleText;

	// Token: 0x040019DA RID: 6618
	public TextMeshProUGUI newspaperDateText;

	// Token: 0x040019DB RID: 6619
	[Space(7f)]
	public TextMeshProUGUI mainArticleHeadline;

	// Token: 0x040019DC RID: 6620
	public TextMeshProUGUI mainArticleColumn1;

	// Token: 0x040019DD RID: 6621
	public TextMeshProUGUI mainArticleColumn2;

	// Token: 0x040019DE RID: 6622
	public TextMeshProUGUI mainArticleColumn3;

	// Token: 0x040019DF RID: 6623
	[Space(7f)]
	public TextMeshProUGUI article2Headline;

	// Token: 0x040019E0 RID: 6624
	public TextMeshProUGUI article2Column1;

	// Token: 0x040019E1 RID: 6625
	public TextMeshProUGUI article2Column2;

	// Token: 0x040019E2 RID: 6626
	public TextMeshProUGUI article2Column3;

	// Token: 0x040019E3 RID: 6627
	[Space(7f)]
	public TextMeshProUGUI article3Headline;

	// Token: 0x040019E4 RID: 6628
	public TextMeshProUGUI article3Column1;

	// Token: 0x040019E5 RID: 6629
	public TextMeshProUGUI article3Column2;

	// Token: 0x040019E6 RID: 6630
	[Space(7f)]
	public TextMeshProUGUI ad1Text;

	// Token: 0x040019E7 RID: 6631
	public TextMeshProUGUI ad2Text;

	// Token: 0x040019E8 RID: 6632
	public TextMeshProUGUI ad3Text;

	// Token: 0x040019E9 RID: 6633
	public TextMeshProUGUI ad4Text;

	// Token: 0x040019EA RID: 6634
	[Header("State")]
	public NewspaperController.NewspaperState currentState;

	// Token: 0x040019EB RID: 6635
	private static NewspaperController _instance;

	// Token: 0x0200039B RID: 923
	[Serializable]
	public class NewspaperState
	{
		// Token: 0x06001508 RID: 5384 RVA: 0x00002265 File Offset: 0x00000465
		public void SerializeFields()
		{
		}

		// Token: 0x040019EC RID: 6636
		public float time;

		// Token: 0x040019ED RID: 6637
		public string seed;

		// Token: 0x040019EE RID: 6638
		public int murderID = -1;

		// Token: 0x040019EF RID: 6639
		public string mainArticle;

		// Token: 0x040019F0 RID: 6640
		public int mainContext;

		// Token: 0x040019F1 RID: 6641
		public string article2;

		// Token: 0x040019F2 RID: 6642
		public int art2Context;

		// Token: 0x040019F3 RID: 6643
		public string article3;

		// Token: 0x040019F4 RID: 6644
		public int art3Context;

		// Token: 0x040019F5 RID: 6645
		public string ad1;

		// Token: 0x040019F6 RID: 6646
		public int ad1Context;

		// Token: 0x040019F7 RID: 6647
		public string ad2;

		// Token: 0x040019F8 RID: 6648
		public int ad2Context;

		// Token: 0x040019F9 RID: 6649
		public string ad3;

		// Token: 0x040019FA RID: 6650
		public int ad3Context;

		// Token: 0x040019FB RID: 6651
		public string ad4;

		// Token: 0x040019FC RID: 6652
		public int ad4Context;
	}
}
