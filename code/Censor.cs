using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02000030 RID: 48
public class Censor : MonoBehaviour
{
	// Token: 0x060001BD RID: 445 RVA: 0x0000E388 File Offset: 0x0000C588
	private void Awake()
	{
		TextAsset textAsset = Resources.Load("profanity-blacklist") as TextAsset;
		TextAsset textAsset2 = Resources.Load("profanity-whitelist") as TextAsset;
		this.CensoredWords = new List<string>(Regex.Split(textAsset.ToString(), "\r\n|\r|\n"));
		this.OKWords = new List<string>(Regex.Split(textAsset2.ToString(), "\r\n|\r|\n"));
	}

	// Token: 0x060001BE RID: 446 RVA: 0x0000E3EB File Offset: 0x0000C5EB
	public string CensorText(string text)
	{
		text = this.checkThisText(text);
		text = this.checkNumbers(this.checkForLeet(text));
		return this.checkForReverses(text);
	}

	// Token: 0x060001BF RID: 447 RVA: 0x0000E40C File Offset: 0x0000C60C
	private string checkThisText(string text)
	{
		foreach (string text2 in this.CensoredWords)
		{
			if (text.ToLower().Contains(text2))
			{
				List<string> list = new List<string>();
				foreach (string text3 in this.OKWords)
				{
					if (text.Contains(text3))
					{
						list.Add(text3);
					}
				}
				if (list.Count > 0)
				{
					string text4 = text;
					foreach (string text5 in list)
					{
						int num = text4.IndexOf(text5);
						text4 = this.CensorText(text.Replace(text5, "")).Insert(num, text5);
					}
					text = text4;
				}
				else
				{
					string text6 = "";
					for (int i = 0; i < text2.Length; i++)
					{
						text6 += "*";
					}
					text = Regex.Replace(text, text2, text6, 1);
				}
			}
		}
		return text;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x0000E588 File Offset: 0x0000C788
	private string checkNumbers(string input)
	{
		return this.checkForInterspersedNumbers(this.checkForLeet(input));
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x0000E598 File Offset: 0x0000C798
	private string checkForInterspersedNumbers(string input)
	{
		string text = Regex.Replace(input, "[0-9]", "");
		string text2 = this.checkThisText(text);
		if (text2.Contains("*"))
		{
			return text2;
		}
		return input;
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000E5D0 File Offset: 0x0000C7D0
	private string checkForLeet(string input)
	{
		string text = input.Replace("0", "o").Replace("1", "i").Replace("3", "e").Replace("4", "a").Replace("5", "s").Replace("7", "t").Replace("8", "b").Replace("9", "g");
		string text2 = input.Replace("0", "o").Replace("1", "l").Replace("3", "e").Replace("4", "a").Replace("5", "s").Replace("7", "t").Replace("8", "b").Replace("9", "g");
		string text3 = this.checkThisText(text);
		string text4 = this.checkThisText(text2);
		if (text3.Contains("*"))
		{
			return text3;
		}
		if (text4.Contains("*"))
		{
			return text4;
		}
		return input;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000E700 File Offset: 0x0000C900
	private string checkForReverses(string input)
	{
		string text = this.checkThisText(this.Reverse(input));
		text = this.checkNumbers(text);
		return this.checkThisText(this.Reverse(text));
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000E730 File Offset: 0x0000C930
	private string Reverse(string s)
	{
		char[] array = s.ToCharArray();
		Array.Reverse<char>(array);
		return new string(array);
	}

	// Token: 0x0400010B RID: 267
	private IList<string> CensoredWords;

	// Token: 0x0400010C RID: 268
	private IList<string> OKWords;
}
