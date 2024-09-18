using System;
using System.Text;
using UnityEngine;

// Token: 0x0200019D RID: 413
public class NameGenerator : MonoBehaviour
{
	// Token: 0x17000062 RID: 98
	// (get) Token: 0x06000A43 RID: 2627 RVA: 0x0009842F File Offset: 0x0009662F
	public static NameGenerator Instance
	{
		get
		{
			return NameGenerator._instance;
		}
	}

	// Token: 0x06000A44 RID: 2628 RVA: 0x00098436 File Offset: 0x00096636
	private void Awake()
	{
		if (NameGenerator._instance != null && NameGenerator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		NameGenerator._instance = this;
	}

	// Token: 0x06000A45 RID: 2629 RVA: 0x00098464 File Offset: 0x00096664
	public string GenerateName(string prefixList, float prefixChance, string mainList, float mainChance, string suffixList, float suffixChance, string useCustomSeed = "")
	{
		string text;
		string text2;
		string text3;
		bool flag;
		string text4;
		return this.GenerateName(prefixList, prefixChance, mainList, mainChance, suffixList, suffixChance, false, 0, 0, out text, out text2, out text3, out flag, out text4, useCustomSeed);
	}

	// Token: 0x06000A46 RID: 2630 RVA: 0x00098490 File Offset: 0x00096690
	public string GenerateName(string prefixList, float prefixChance, string mainList, float mainChance, string suffixList, float suffixChance, out string prefixOutput, out string mainOutput, out string suffixOutput, out bool needsSuffixForShortName, out string alternateTags, string useCustomSeed = "")
	{
		return this.GenerateName(prefixList, prefixChance, mainList, mainChance, suffixList, suffixChance, false, 0, 0, out prefixOutput, out mainOutput, out suffixOutput, out needsSuffixForShortName, out alternateTags, useCustomSeed);
	}

	// Token: 0x06000A47 RID: 2631 RVA: 0x000984BC File Offset: 0x000966BC
	public string GenerateName(string prefixList, float prefixChance, string mainList, float mainChance, string suffixList, float suffixChance, bool mainIsCitizenName, int prefixMainAlliterationWeight, int mainSuffixAlliterationWeight, out string prefixOutput, out string mainOutput, out string suffixOutput, out bool needsSuffixForShortName, out string alternateTags, string useCustomSeed = "")
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		bool flag2 = false;
		alternateTags = string.Empty;
		prefixOutput = string.Empty;
		mainOutput = string.Empty;
		suffixOutput = string.Empty;
		needsSuffixForShortName = false;
		float num;
		if (useCustomSeed.Length > 0)
		{
			num = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, useCustomSeed, out useCustomSeed);
		}
		else
		{
			num = Toolbox.Instance.Rand(0f, 1f, false);
		}
		if (prefixChance > 0f && num <= prefixChance)
		{
			if (prefixList.Substring(0, 5) == "this." && prefixList.Length > 5)
			{
				prefixOutput = prefixList.Substring(5);
			}
			else
			{
				string empty = string.Empty;
				bool flag3;
				prefixOutput = Strings.GetRandom(prefixList, out flag3, out empty, useCustomSeed);
				if (alternateTags.Length > 0)
				{
					alternateTags += ";";
				}
				if (empty.Length > 0)
				{
					alternateTags += empty;
				}
			}
			flag = true;
			stringBuilder.Append(prefixOutput);
		}
		if (useCustomSeed.Length > 0)
		{
			num = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, useCustomSeed, out useCustomSeed);
		}
		else
		{
			num = Toolbox.Instance.Rand(0f, 1f, false);
		}
		if (mainChance > 0f && num <= mainChance)
		{
			if (flag)
			{
				stringBuilder.Append(' ');
			}
			if (mainList.Length > 5 && mainList.Substring(0, 5) == "this.")
			{
				mainOutput = mainList.Substring(5);
			}
			else
			{
				string alliterationStr = string.Empty;
				if (prefixMainAlliterationWeight > 0 && flag && prefixOutput.Length >= 2)
				{
					alliterationStr = prefixOutput.Substring(0, 1);
				}
				else
				{
					prefixMainAlliterationWeight = 0;
				}
				string empty2 = string.Empty;
				mainOutput = Strings.GetRandom(mainList, alliterationStr, prefixMainAlliterationWeight, out needsSuffixForShortName, out empty2, useCustomSeed);
				if (alternateTags.Length > 0)
				{
					alternateTags += ";";
				}
				if (empty2.Length > 0)
				{
					alternateTags += empty2;
				}
			}
			if (mainIsCitizenName && mainOutput.Length > 0)
			{
				if (mainOutput.Substring(mainOutput.Length - 1) == "s")
				{
					mainOutput += "'";
				}
				else
				{
					mainOutput += "'s";
				}
			}
			stringBuilder.Append(mainOutput);
			flag2 = true;
		}
		if (useCustomSeed.Length > 0)
		{
			num = Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, useCustomSeed, out useCustomSeed);
		}
		else
		{
			num = Toolbox.Instance.Rand(0f, 1f, false);
		}
		if (suffixChance > 0f && num <= suffixChance)
		{
			if (flag2)
			{
				stringBuilder.Append(' ');
			}
			if (suffixList.Length > 5 && suffixList.Substring(0, 5) == "this.")
			{
				suffixOutput = suffixList.Substring(5);
			}
			else
			{
				string alliterationStr2 = string.Empty;
				if (mainSuffixAlliterationWeight > 0 && flag2 && mainOutput.Length >= 2)
				{
					alliterationStr2 = mainOutput.Substring(0, 1);
				}
				else
				{
					mainSuffixAlliterationWeight = 0;
				}
				string empty3 = string.Empty;
				bool flag3;
				suffixOutput = Strings.GetRandom(suffixList, alliterationStr2, mainSuffixAlliterationWeight, out flag3, out empty3, useCustomSeed);
				if (alternateTags.Length > 0)
				{
					alternateTags += ";";
				}
				if (empty3.Length > 0)
				{
					alternateTags += empty3;
				}
			}
			stringBuilder.Append(suffixOutput);
		}
		string text = stringBuilder.ToString();
		if (text == null || text.Length <= 0)
		{
			string empty4 = string.Empty;
			text = Strings.GetRandom(mainList, out needsSuffixForShortName, out empty4, useCustomSeed);
			if (alternateTags.Length > 0)
			{
				alternateTags += ";";
			}
			if (empty4.Length > 0)
			{
				alternateTags += empty4;
			}
			mainOutput = text;
			suffixOutput = text;
		}
		return text;
	}

	// Token: 0x04000A4D RID: 2637
	private static NameGenerator _instance;
}
