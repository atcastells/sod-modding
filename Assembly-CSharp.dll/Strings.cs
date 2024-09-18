using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using Rewired;
using UnityEngine;

// Token: 0x0200080A RID: 2058
public class Strings : MonoBehaviour
{
	// Token: 0x17000126 RID: 294
	// (get) Token: 0x06002629 RID: 9769 RVA: 0x001EA94A File Offset: 0x001E8B4A
	public static Strings Instance
	{
		get
		{
			return Strings._instance;
		}
	}

	// Token: 0x0600262A RID: 9770 RVA: 0x001EA954 File Offset: 0x001E8B54
	private void Awake()
	{
		if (Strings._instance != null && Strings._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Strings._instance = this;
		}
		if (LanguageConfigLoader.Instance == null)
		{
			Object.Instantiate<GameObject>(this.languageLoaderPrefab);
		}
		if (!PlayerPrefsController.Instance.loadedPlayerPrefs)
		{
			PlayerPrefsController.Instance.LoadPlayerPrefs(false);
		}
		this.LoadTextFiles();
	}

	// Token: 0x0600262B RID: 9771 RVA: 0x001EA9C4 File Offset: 0x001E8BC4
	public void LoadTextFiles()
	{
		string text = Application.streamingAssetsPath;
		if (SessionData.Instance.isDialogEdit && !Application.isEditor)
		{
			text = Path.GetFullPath(".");
		}
		Strings.stringTable.Clear();
		Strings.dictionaryPathnames.Clear();
		Strings.stringTableENG.Clear();
		Strings.randomEntryLists.Clear();
		Strings.randomEntryListsENG.Clear();
		if (!LanguageConfigLoader.Instance.loadedLanguageConfig)
		{
			LanguageConfigLoader.Instance.LoadLanguageConfig();
		}
		Strings.textFilesLoaded = false;
		Strings.backupENGLoaded = false;
		if (Game.Instance.forceEnglish)
		{
			Game.Instance.language = "English";
		}
		else if (!PlayerPrefsController.Instance.playedBefore)
		{
			Game.Log("Menu: Game has not been run before, autodetecting language...", 2);
			LanguageConfigLoader.LocInput locInput = LanguageConfigLoader.Instance.fileInputConfig.Find((LanguageConfigLoader.LocInput item) => item.systemLanguage == Application.systemLanguage);
			if (locInput != null)
			{
				Game.Log("Menu: ... Detected " + locInput.languageCode, 2);
				Game.Instance.language = locInput.languageCode;
				PlayerPrefs.SetString("language", locInput.languageCode);
			}
		}
		Game.Log("Menu: Loading text files (" + Game.Instance.language + ")", 2);
		Strings.loadedLanguage = LanguageConfigLoader.Instance.fileInputConfig.Find((LanguageConfigLoader.LocInput item) => item.systemLanguage == 10);
		if (Game.Instance.language.ToLower() != "english")
		{
			if (LanguageConfigLoader.Instance.fileInputConfig.Exists((LanguageConfigLoader.LocInput item) => item.languageCode.ToLower() == Game.Instance.language.ToLower()))
			{
				Strings.loadedLanguage = LanguageConfigLoader.Instance.fileInputConfig.Find((LanguageConfigLoader.LocInput item) => item.languageCode.ToLower() == Game.Instance.language.ToLower());
				Game.Log("CityGen: Loading English backup strings...", 2);
				foreach (FileInfo fileInfo in Enumerable.ToList<FileInfo>(new DirectoryInfo(text + "/Strings/English/").GetFiles("*.csv", 1)))
				{
					string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
					bool flag = false;
					foreach (string text3 in this.localisationIgnoreDirectoryList)
					{
						if (fileInfo.FullName.Contains(text3))
						{
							Game.Log(string.Concat(new string[]
							{
								"Loading ignored localization reference from ENG: ",
								text2,
								" (ignore dir ",
								text3,
								")"
							}), 2);
							flag = true;
						}
					}
					if (!flag && this.localisationIgnoreFileList.Contains(text2))
					{
						Game.Log("Loading ignored localization reference from ENG: " + text2, 2);
						flag = true;
					}
					if (!Strings.stringTableENG.ContainsKey(text2))
					{
						Strings.stringTableENG.Add(text2, new Dictionary<string, Strings.DisplayString>());
					}
					using (StreamReader streamReader = File.OpenText(fileInfo.FullName))
					{
						int num = 0;
						for (int i = 0; i < 3; i++)
						{
							streamReader.ReadLine();
							num++;
						}
						while (!streamReader.EndOfStream)
						{
							string input = streamReader.ReadLine();
							string empty = string.Empty;
							string text4 = string.Empty;
							string text5 = string.Empty;
							int frequency = 0;
							bool suffix = false;
							string text6;
							string text7;
							Strings.ParseLine(input, out empty, out text6, out text4, out text5, out frequency, out suffix, out text7, true);
							text4 = Strings.ConvertLineBreaksToDisplay(text4);
							text5 = Strings.ConvertLineBreaksToDisplay(text5);
							if (empty.Length > 0)
							{
								Strings.LoadIntoDictionaryENG(text2, num, empty, text4, text5, frequency, suffix);
								if (flag)
								{
									if (!Strings.stringTable.ContainsKey(text2))
									{
										Strings.stringTable.Add(text2, new Dictionary<string, Strings.DisplayString>());
									}
									if (!Strings.dictionaryPathnames.ContainsKey(text2))
									{
										Strings.dictionaryPathnames.Add(text2, fileInfo.FullName);
									}
									Strings.LoadIntoDictionary(text2, num, empty, text4, text5, frequency, suffix);
								}
							}
							num++;
						}
					}
				}
				Strings.backupENGLoaded = true;
			}
			else
			{
				Game.Instance.language = "English";
			}
		}
		foreach (FileInfo fileInfo2 in Enumerable.ToList<FileInfo>(new DirectoryInfo(Strings.loadedLanguage.path).GetFiles("*.csv", 1)))
		{
			FileInfo fileInfo3 = fileInfo2;
			string text8 = fileInfo2.Name.Substring(0, fileInfo2.Name.Length - 4);
			if (Strings.loadedLanguage.modOverrideFiles.ContainsKey(text8) && Strings.loadedLanguage.modOverrideFiles[text8] != null)
			{
				fileInfo3 = Strings.loadedLanguage.modOverrideFiles[text8];
			}
			if (!Strings.dictionaryPathnames.ContainsKey(text8))
			{
				Strings.dictionaryPathnames.Add(text8, fileInfo3.FullName);
			}
			if (Strings.templateFile == null && text8 == "template")
			{
				Strings.templateFile = fileInfo3;
			}
			else
			{
				Strings.LoadLanguageFileToGame(text8, fileInfo3.FullName, false);
			}
		}
		Strings.textFilesLoaded = true;
	}

	// Token: 0x0600262C RID: 9772 RVA: 0x001EAF78 File Offset: 0x001E9178
	public static void LoadLanguageFileToGame(string fileName, string path, bool loadAsENGBackup = false)
	{
		if (!Strings.stringTable.ContainsKey(fileName))
		{
			Strings.stringTable.Add(fileName, new Dictionary<string, Strings.DisplayString>());
		}
		int num = 0;
		using (StreamReader streamReader = File.OpenText(path))
		{
			int num2 = 0;
			for (int i = 0; i < 3; i++)
			{
				streamReader.ReadLine();
				num2++;
			}
			while (!streamReader.EndOfStream)
			{
				string input = streamReader.ReadLine();
				string empty = string.Empty;
				string text = string.Empty;
				string text2 = string.Empty;
				int frequency = 0;
				bool suffix = false;
				string text3;
				string text4;
				Strings.ParseLine(input, out empty, out text3, out text, out text2, out frequency, out suffix, out text4, true);
				text = Strings.ConvertLineBreaksToDisplay(text);
				text2 = Strings.ConvertLineBreaksToDisplay(text2);
				if (empty.Length > 0)
				{
					if (text == Strings.missingString)
					{
						text = Strings.GetENG(fileName, empty, false);
						text2 = Strings.GetENG(fileName, empty, true);
						num++;
					}
					if (loadAsENGBackup)
					{
						Strings.LoadIntoDictionaryENG(fileName, num2, empty, text, text2, frequency, suffix);
					}
					else
					{
						Strings.LoadIntoDictionary(fileName, num2, empty, text, text2, frequency, suffix);
					}
				}
				num2++;
			}
		}
		if (Game.Instance.language != "English")
		{
			Game.Log("CityGen: Backup English keys used for " + Game.Instance.language + ": " + num.ToString(), 2);
		}
	}

	// Token: 0x0600262D RID: 9773 RVA: 0x001EB0D0 File Offset: 0x001E92D0
	public static void ParseLine(string input, out string key, out string notes, out string display, out string alt, out int frequency, out bool suffix, out string misc, bool useFieldQuotations = true)
	{
		key = string.Empty;
		notes = string.Empty;
		display = string.Empty;
		alt = string.Empty;
		frequency = 0;
		suffix = false;
		misc = string.Empty;
		if (input == null)
		{
			return;
		}
		List<string> list;
		if (useFieldQuotations)
		{
			list = Enumerable.ToList<string>(new Regex(','.ToString() + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(input));
		}
		else
		{
			list = Enumerable.ToList<string>(input.Split(new char[]
			{
				','
			}, 0));
		}
		if (list.Count > 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Length >= 2 && list[i].get_Chars(0) == '"' && list[i].get_Chars(list[i].Length - 1) == '"')
				{
					list[i] = list[i].Substring(1, list[i].Length - 2);
				}
			}
			key = list[0].ToLower();
			if (list.Count > 1)
			{
				notes = list[1];
			}
			if (list.Count > 2)
			{
				display = list[2];
			}
			if (list.Count > 3)
			{
				alt = list[3];
			}
			if (list.Count > 4)
			{
				int.TryParse(list[4], ref frequency);
			}
			if (list.Count > 5 && list[5].Length > 0)
			{
				suffix = true;
			}
			if (list.Count > 6)
			{
				misc = list[6];
			}
		}
	}

	// Token: 0x0600262E RID: 9774 RVA: 0x001EB25C File Offset: 0x001E945C
	private static void LoadIntoDictionary(string fileName, int lineNo, string key, string display, string alternate, int frequency, bool suffix)
	{
		if (key == "")
		{
			return;
		}
		if (display == "")
		{
			return;
		}
		if (Strings.stringTable.ContainsKey(fileName))
		{
			if (Strings.stringTable[fileName].ContainsKey(key))
			{
				Game.LogError(string.Concat(new string[]
				{
					"CityGen: Duplicate ",
					key,
					" found in ",
					fileName,
					"."
				}), 2);
				return;
			}
			Strings.stringTable[fileName].Add(key, new Strings.DisplayString
			{
				displayStr = display,
				alternateStr = alternate
			});
		}
		if (frequency > 0)
		{
			if (!Strings.randomEntryLists.ContainsKey(fileName))
			{
				Strings.randomEntryLists.Add(fileName, new List<Strings.RandomDisplayString>());
			}
			frequency = Mathf.Clamp(frequency, 0, 100);
			Strings.RandomDisplayString randomDisplayString = new Strings.RandomDisplayString
			{
				displayStr = display,
				alternateStr = alternate,
				needsSuffixForShortName = suffix
			};
			for (int i = 0; i < frequency; i++)
			{
				Strings.randomEntryLists[fileName].Add(randomDisplayString);
			}
		}
	}

	// Token: 0x0600262F RID: 9775 RVA: 0x001EB368 File Offset: 0x001E9568
	private static void LoadIntoDictionaryENG(string fileName, int lineNo, string key, string display, string alternate, int frequency, bool suffix)
	{
		if (key == "")
		{
			return;
		}
		if (display == "")
		{
			return;
		}
		if (Strings.stringTableENG.ContainsKey(fileName))
		{
			if (Strings.stringTableENG[fileName].ContainsKey(key))
			{
				Game.LogError(string.Concat(new string[]
				{
					"CityGen: Duplicate ",
					key,
					" found in ",
					fileName,
					"."
				}), 2);
				return;
			}
			Strings.stringTableENG[fileName].Add(key, new Strings.DisplayString
			{
				displayStr = display,
				alternateStr = alternate
			});
		}
		if (frequency > 0)
		{
			if (!Strings.randomEntryListsENG.ContainsKey(fileName))
			{
				Strings.randomEntryListsENG.Add(fileName, new List<Strings.RandomDisplayString>());
			}
			frequency = Mathf.Clamp(frequency, 0, 100);
			Strings.RandomDisplayString randomDisplayString = new Strings.RandomDisplayString
			{
				displayStr = display,
				alternateStr = alternate,
				needsSuffixForShortName = suffix
			};
			for (int i = 0; i < frequency; i++)
			{
				Strings.randomEntryListsENG[fileName].Add(randomDisplayString);
			}
		}
	}

	// Token: 0x06002630 RID: 9776 RVA: 0x001EB474 File Offset: 0x001E9674
	public static string Get(string dictionary, string key, Strings.Casing casing = Strings.Casing.asIs, bool getAlternate = false, bool forceNoWrite = false, bool useGenderReference = false, Human genderReference = null)
	{
		if (!Strings.textFilesLoaded)
		{
			Strings.Instance.LoadTextFiles();
		}
		if (dictionary.Length <= 0 || key.Length <= 0)
		{
			Game.LogError("CityGen: Invalid key length for key " + key + " dictionary: " + dictionary, 2);
			return string.Empty;
		}
		string text = string.Empty;
		string text2 = dictionary.ToLower();
		string text3 = key.ToLower();
		if (Strings.stringTable.ContainsKey(text2))
		{
			Strings.DisplayString displayString = null;
			if (useGenderReference && genderReference != null)
			{
				if (genderReference.gender == Human.Gender.female)
				{
					string text4 = text3 + "_F";
					if (Strings.stringTable[text2].ContainsKey(text4))
					{
						displayString = Strings.stringTable[text2][text4];
					}
				}
				else if (genderReference.gender == Human.Gender.nonBinary)
				{
					string text5 = text3 + "_NB";
					if (Strings.stringTable[text2].ContainsKey(text5))
					{
						displayString = Strings.stringTable[text2][text5];
					}
				}
			}
			if (Strings.stringTable[text2].ContainsKey(text3))
			{
				if (displayString == null)
				{
					displayString = Strings.stringTable[text2][text3];
				}
				if (getAlternate)
				{
					text = displayString.alternateStr;
				}
				else
				{
					text = displayString.displayStr;
				}
			}
			else
			{
				if (Game.Instance.language != "English" && Strings.stringTableENG.ContainsKey(text2) && Strings.stringTableENG[text2].ContainsKey(text3))
				{
					displayString = Strings.stringTableENG[text2][text3];
					if (getAlternate)
					{
						text = displayString.alternateStr;
					}
					else
					{
						text = displayString.displayStr;
					}
				}
				if (text.Length <= 0)
				{
					Game.Log("CityGen: Unable to load entry " + text3 + " from dictionary " + text2, 2);
					if (!Game.Instance.writeUnfoundToTextFiles || forceNoWrite || !(Game.Instance.language == "English"))
					{
						Game.Log("Invalid String Address", 2);
						text = "<InvalidStringAddress>";
					}
				}
			}
		}
		else if (Game.Instance.language != "English")
		{
			if (Strings.stringTableENG.ContainsKey(text2))
			{
				if (Strings.stringTableENG[text2].ContainsKey(text3))
				{
					Strings.DisplayString displayString2 = Strings.stringTableENG[text2][text3];
					if (getAlternate)
					{
						text = displayString2.alternateStr;
					}
					else
					{
						text = displayString2.displayStr;
					}
				}
			}
			else
			{
				Game.LogError("Dictionary " + text2 + " appears to be missing: Please create the file.", 2);
			}
		}
		else
		{
			Game.LogError("Dictionary " + text2 + " appears to be missing: Please create the file.", 2);
		}
		return Strings.ApplyCasing(text, casing);
	}

	// Token: 0x06002631 RID: 9777 RVA: 0x001EB710 File Offset: 0x001E9910
	public static string GetENG(string dictionary, string key, bool getAlternate = false)
	{
		if (dictionary.Length <= 0 || key.Length <= 0)
		{
			Game.LogError("CityGen: Invalid key length for key " + key + " dictionary: " + dictionary, 2);
			return string.Empty;
		}
		string result = string.Empty;
		string text = dictionary.ToLower();
		string text2 = key.ToLower();
		if (Strings.stringTableENG.ContainsKey(text))
		{
			if (Strings.stringTableENG[text].ContainsKey(text2))
			{
				Strings.DisplayString displayString = Strings.stringTableENG[text][text2];
				if (getAlternate)
				{
					result = displayString.alternateStr;
				}
				else
				{
					result = displayString.displayStr;
				}
			}
			else
			{
				Game.Log("CityGen: Unable to load entry " + text2 + " from dictionary " + text, 2);
			}
		}
		else
		{
			Game.LogError("CityGen: Dictionary " + text + " appears to be missing: Please create the file.", 2);
		}
		return result;
	}

	// Token: 0x06002632 RID: 9778 RVA: 0x001EB7D8 File Offset: 0x001E99D8
	public static string GetLineFromFile(string dictionary, int lineNumber)
	{
		string result;
		using (StreamReader streamReader = File.OpenText(Strings.dictionaryPathnames[dictionary]))
		{
			for (int i = 0; i < lineNumber; i++)
			{
				streamReader.ReadLine();
			}
			result = streamReader.ReadLine();
		}
		return result;
	}

	// Token: 0x06002633 RID: 9779 RVA: 0x001EB830 File Offset: 0x001E9A30
	public static string ApplyCasing(string input, Strings.Casing casing = Strings.Casing.asIs)
	{
		if (casing == Strings.Casing.lowerCase)
		{
			input = input.ToLower();
		}
		else if (casing == Strings.Casing.upperCase)
		{
			input = input.ToUpper();
		}
		else if (casing == Strings.Casing.firstLetterCaptial)
		{
			if (input.Length <= 1)
			{
				input = input.ToUpper();
			}
			else
			{
				input = input.ToLower();
				input = input.Substring(0, 1).ToUpper() + input.Substring(1, input.Length - 1);
			}
		}
		else if (casing == Strings.Casing.pascalCase)
		{
			if (input == null)
			{
				return input;
			}
			if (input.Length < 2)
			{
				input = input.ToUpper();
			}
			string[] array = input.Split(new char[0], 1);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i];
				text = text + text2.Substring(0, 1).ToUpper() + text2.Substring(1);
				if (i < array.Length - 1)
				{
					text += " ";
				}
			}
			input = text;
		}
		return input;
	}

	// Token: 0x06002634 RID: 9780 RVA: 0x001EB91C File Offset: 0x001E9B1C
	public static void WriteToDictionary(string dictionaryName, string key, string notes, string display, string alternate = "", int frequency = 0, bool requiresSuffix = false, string misc = "")
	{
		string text = dictionaryName.ToLower();
		if (Strings.dictionaryPathnames.ContainsKey(text))
		{
			Game.Log("CityGen: Writing new key to dictionary " + dictionaryName + ": " + key, 2);
			string text2 = string.Empty;
			if (frequency > 0)
			{
				text2 = frequency.ToString();
			}
			string text3 = string.Empty;
			if (requiresSuffix)
			{
				text3 = "Y";
			}
			misc = DateTime.Now.ToString("HH:mm dd/MM/yyyy");
			string text4 = string.Concat(new string[]
			{
				"\"",
				key,
				"\",\"",
				notes,
				"\",\"",
				display,
				"\",\"",
				alternate,
				"\",",
				text2,
				",\"",
				text3,
				"\",\"",
				misc,
				"\""
			});
			using (StreamWriter streamWriter = File.AppendText(Strings.dictionaryPathnames[text]))
			{
				streamWriter.WriteLine(text4);
			}
			Strings.LoadIntoDictionary(text, Enumerable.Count<string>(File.ReadLines(Strings.dictionaryPathnames[text])) - 1, key, display, alternate, frequency, requiresSuffix);
		}
	}

	// Token: 0x06002635 RID: 9781 RVA: 0x001EBA60 File Offset: 0x001E9C60
	public static string GetRandom(string dictionary, out bool needsSuffixForShortName, out string alternate, string useCustomSeed = "")
	{
		if (!Strings.textFilesLoaded)
		{
			Strings.Instance.LoadTextFiles();
		}
		return Strings.GetRandom(dictionary, string.Empty, 0, out needsSuffixForShortName, out alternate, useCustomSeed);
	}

	// Token: 0x06002636 RID: 9782 RVA: 0x001EBA84 File Offset: 0x001E9C84
	public static string GetRandom(string dictionary, string alliterationStr, int alliterationWeight, out bool needsSuffixForShortName, out string alternate, string useCustomSeed = "")
	{
		if (!Strings.textFilesLoaded)
		{
			Strings.Instance.LoadTextFiles();
		}
		alternate = string.Empty;
		needsSuffixForShortName = false;
		if (dictionary.Length <= 0)
		{
			return "<InvalidKeyLength>";
		}
		Strings.RandomDisplayString randomDisplayString = null;
		string text = dictionary.ToLower();
		List<Strings.RandomDisplayString> list = null;
		try
		{
			list = Strings.randomEntryLists[text];
		}
		catch
		{
			Game.Log("CityGen: Cannot find " + text + " random word list", 2);
			return string.Empty;
		}
		if (list.Count <= 0)
		{
			Game.Log("CityGen: Found list " + text + " but it's empty!", 2);
		}
		int num;
		if (alliterationWeight > 0 && alliterationStr.Length > 0)
		{
			List<Strings.RandomDisplayString> list2 = new List<Strings.RandomDisplayString>();
			foreach (Strings.RandomDisplayString randomDisplayString2 in Strings.randomEntryLists[text])
			{
				if (randomDisplayString2.displayStr.Length > 0)
				{
					list2.Add(randomDisplayString2);
					if (randomDisplayString2.displayStr.Substring(0, 1) == alliterationStr)
					{
						for (int i = 0; i < alliterationWeight; i++)
						{
							list2.Add(randomDisplayString2);
						}
					}
				}
			}
			if (useCustomSeed.Length > 0)
			{
				num = Toolbox.Instance.RandContained(0, list2.Count, useCustomSeed, out useCustomSeed);
			}
			else
			{
				num = Toolbox.Instance.Rand(0, list2.Count, false);
			}
			try
			{
				randomDisplayString = list2[num];
				needsSuffixForShortName = list2[num].needsSuffixForShortName;
				goto IL_1E4;
			}
			catch
			{
				Game.Log("CityGen: No alliteration for" + dictionary, 2);
				return "<RandomInvalidStringAddress-Alliteration>";
			}
		}
		if (useCustomSeed.Length > 0)
		{
			num = Toolbox.Instance.RandContained(0, list.Count, useCustomSeed, out useCustomSeed);
		}
		else
		{
			num = Toolbox.Instance.Rand(0, list.Count, false);
		}
		try
		{
			randomDisplayString = list[num];
			needsSuffixForShortName = list[num].needsSuffixForShortName;
		}
		catch
		{
			return "<RandomInvalidStringAddress-NoAlliteration>";
		}
		IL_1E4:
		alternate = randomDisplayString.alternateStr;
		return randomDisplayString.displayStr;
	}

	// Token: 0x06002637 RID: 9783 RVA: 0x001EBCBC File Offset: 0x001E9EBC
	public static string[] CleanSplit(string input, char del, bool trimElements, bool removeEmpty = true)
	{
		StringSplitOptions stringSplitOptions = 1;
		if (!removeEmpty)
		{
			stringSplitOptions = 0;
		}
		char[] array = new char[]
		{
			del
		};
		string[] array2 = input.Split(array, stringSplitOptions);
		if (trimElements)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = array2[i].Trim();
			}
		}
		return array2;
	}

	// Token: 0x06002638 RID: 9784 RVA: 0x001EBD04 File Offset: 0x001E9F04
	public static string[] CleanSplit(string input, string[] del, bool trimElements)
	{
		string[] array = input.Split(del, 0);
		if (trimElements)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
		}
		return array;
	}

	// Token: 0x06002639 RID: 9785 RVA: 0x001EBD37 File Offset: 0x001E9F37
	public static string ConvertLineBreaksToSaveSafe(string input)
	{
		input = input.Replace("\n", "\\n");
		input = input.Replace("\r", "\\r");
		input = input.Replace("\t", "\\t");
		return input;
	}

	// Token: 0x0600263A RID: 9786 RVA: 0x001EBD70 File Offset: 0x001E9F70
	public static string ConvertLineBreaksToDisplay(string input)
	{
		input = input.Replace("\\n", "\n");
		input = input.Replace("\\r", "\r");
		input = input.Replace("\\t", "\t");
		return input;
	}

	// Token: 0x0600263B RID: 9787 RVA: 0x001EBDAC File Offset: 0x001E9FAC
	public static string GetTextForComponent(string msgID, object obj, Human from = null, Human to = null, string lineBreaks = "\n", bool skipFirstBlock = false, object additionalObject = null, Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic, List<Evidence.DataKey> dataKeys = null)
	{
		if (msgID == null || msgID.Length <= 0)
		{
			Game.LogError("No DDS message found: Null or message ID of length 0", 2);
			return string.Empty;
		}
		Acquaintance aq = null;
		if (to != null && from != null)
		{
			from.FindAcquaintanceExists(to, out aq);
		}
		List<string> list;
		if (from == null)
		{
			List<int> list2;
			list = Player.Instance.ParseDDSMessage(msgID, aq, out list2, false, obj, false);
		}
		else
		{
			List<int> list2;
			list = from.ParseDDSMessage(msgID, aq, out list2, false, obj, false);
		}
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		if (skipFirstBlock && list.Count > 1)
		{
			num = 1;
		}
		for (int i = num; i < list.Count; i++)
		{
			string text = Strings.ComposeText(Strings.Get("dds.blocks", list[i], Strings.Casing.asIs, false, false, true, from), obj, linkSetting, dataKeys, additionalObject, false);
			if (i < list.Count - 1)
			{
				text += lineBreaks;
			}
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600263C RID: 9788 RVA: 0x001EBEA0 File Offset: 0x001EA0A0
	[Button(null, 0)]
	public void OutputTextForLoc()
	{
		string streamingAssetsPath = Application.streamingAssetsPath;
		List<FileInfo> list = Enumerable.ToList<FileInfo>(new DirectoryInfo(streamingAssetsPath + "/Strings/English/").GetFiles("*.csv", 1));
		string text = streamingAssetsPath + "/Strings/Localization/SoD_Localization_All.csv";
		List<string> list2 = new List<string>();
		list2.Add("\"Key\",\"ENGLISH\",\"FRENCH\",\"GERMAN\",\"SPANISH\", \"BRAZILIAN PORTUGUESE\", \"SIMPLIFIED CHINESE\",\"TRADITIONAL CHINESE\",\"JAPANESE\",\"RUSSIAN\"");
		list2.Add(string.Empty);
		list2.Add(string.Empty);
		int num = 0;
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		int num2 = 0;
		int num3 = 0;
		foreach (FileInfo fileInfo in list)
		{
			string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
			bool flag = false;
			foreach (string text3 in this.localisationIgnoreDirectoryList)
			{
				if (fileInfo.FullName.Contains(text3))
				{
					Debug.Log(string.Concat(new string[]
					{
						"Skipping: ",
						text2,
						" (ignore dir ",
						text3,
						")"
					}));
					flag = true;
				}
			}
			if (!flag)
			{
				if (this.localisationIgnoreFileList.Contains(text2))
				{
					Debug.Log("Skipping File: " + text2 + "...");
					flag = true;
				}
				else
				{
					Debug.Log("Processing File: " + text2 + "...");
					using (StreamReader streamReader = File.OpenText(fileInfo.FullName))
					{
						int num4 = 0;
						for (int i = 0; i < 3; i++)
						{
							streamReader.ReadLine();
							num4++;
						}
						while (!streamReader.EndOfStream)
						{
							string input = streamReader.ReadLine();
							bool flag2 = false;
							string empty = string.Empty;
							string empty2 = string.Empty;
							string empty3 = string.Empty;
							string empty4 = string.Empty;
							string text4;
							int num5;
							bool flag3;
							Strings.ParseLine(input, out empty, out empty3, out empty2, out text4, out num5, out flag3, out empty4, true);
							if (this.useIgnoreFlagInNotes && empty3.ToLower().Contains(this.ignoreFlag.ToLower()))
							{
								Debug.Log("Ignoring line with key " + empty + "...");
								flag2 = true;
								num3++;
							}
							if (this.onlyOuputChangesSince && !flag2)
							{
								DateTime dateTime = default(DateTime);
								if (DateTime.TryParseExact(empty4, "HH:mm dd/MM/yyyy", new CultureInfo("en-GB"), 0, ref dateTime))
								{
									DateTime dateTime2 = default(DateTime);
									if (DateTime.TryParseExact(this.outputSinceDate, "HH:mm dd/MM/yyyy", new CultureInfo("en-GB"), 0, ref dateTime2))
									{
										if (dateTime > dateTime2)
										{
											Debug.Log(dateTime.ToString() + " edited after " + dateTime2.ToString() + ", including output...");
										}
										else
										{
											flag2 = true;
											num3++;
										}
									}
									else
									{
										Debug.LogError("Unable to parse date/time: " + this.outputSinceDate + " this will NOT be included in the output...");
										flag2 = true;
										num3++;
									}
								}
								else
								{
									Debug.LogError("Unable to parse date/time: " + empty4 + " this will NOT be included in the output...");
									flag2 = true;
									num3++;
								}
							}
							if (!flag && !flag2)
							{
								bool flag4 = true;
								int num6 = -1;
								if (dictionary.TryGetValue(empty2, ref num6))
								{
									num2++;
									if (this.condenseIdenticalEnglishContentIntoOneKey)
									{
										string[] array = list2[num6].Split(new char[]
										{
											'"',
											','
										}, 1);
										string text5 = string.Concat(new string[]
										{
											array[0].ToLower(),
											"<",
											text2,
											":",
											empty,
											">"
										});
										list2[num6] = string.Concat(new string[]
										{
											"\"",
											text5,
											"\",\"",
											empty2,
											"\""
										});
										Debug.Log("Combined keys: " + text5 + "...");
										flag4 = false;
									}
								}
								if (flag4)
								{
									string[] array2 = empty2.Split(new char[]
									{
										' ',
										'\n'
									}, 1);
									num += array2.Length;
									string text6 = string.Concat(new string[]
									{
										"\"<",
										text2,
										":",
										empty,
										">\",\"",
										empty2,
										"\""
									});
									for (int j = 0; j < this.extraLineBreaks; j++)
									{
										list2.Add(string.Empty);
									}
									list2.Add(text6);
									if (!dictionary.ContainsKey(empty2))
									{
										dictionary.Add(empty2, list2.Count - 1);
									}
								}
							}
							num4++;
						}
					}
				}
			}
		}
		Debug.Log(string.Concat(new string[]
		{
			"Outputting ",
			num.ToString(),
			" words to file at ",
			text,
			". There are ",
			num2.ToString(),
			" duplicate entries. There are ",
			num3.ToString(),
			" ignored keys."
		}));
		dictionary = new Dictionary<string, int>();
		File.WriteAllLines(text, list2);
	}

	// Token: 0x0600263D RID: 9789 RVA: 0x001EC40C File Offset: 0x001EA60C
	[Button(null, 0)]
	public void ImportNonEnglish()
	{
		Debug.Log("Importing non english from localization input file...");
		List<string> list = new List<string>();
		DateTime dateTime;
		if (!DateTime.TryParse(this.inputDate, ref dateTime))
		{
			Debug.Log("Unable to parse input date!");
			list.Add("Unable to parse input date!");
			return;
		}
		string streamingAssetsPath = Application.streamingAssetsPath;
		List<string> list2 = new List<string>();
		Dictionary<string, Dictionary<string, Dictionary<string, string>>> dictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
		int num = 0;
		List<string> list3 = new List<string>();
		List<string> list4 = new List<string>();
		using (StreamReader streamReader = File.OpenText(streamingAssetsPath + "/Strings/Localization/" + this.templateInputFile))
		{
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				list2.Add(text);
			}
		}
		if (list2.Count <= 0)
		{
			Debug.Log("Invalid template file!");
			return;
		}
		Debug.Log("Writing localization content at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
		list.Add("Writing localization content at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
		int num5;
		using (StreamReader streamReader2 = File.OpenText(streamingAssetsPath + "/Strings/Localization/" + this.localisationInputFile))
		{
			int num2 = 0;
			for (int i2 = 0; i2 < 1; i2++)
			{
				streamReader2.ReadLine();
				num2++;
			}
			List<string> list5 = new List<string>();
			int num3 = 0;
			while (!streamReader2.EndOfStream)
			{
				string text2 = streamReader2.ReadLine();
				List<string> list6 = Enumerable.ToList<string>(new Regex(','.ToString() + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(text2));
				bool flag = false;
				int num4 = list6.Count;
				if (this.inputFeaturesLastColumnLineNumbers)
				{
					num4--;
				}
				for (int j = 0; j < num4; j++)
				{
					if (list6[j].Length > 0)
					{
						flag = true;
					}
				}
				if (flag && list6.Count > 0)
				{
					for (int k = 0; k < list6.Count; k++)
					{
						if (list6[k].Length >= 2 && list6[k].get_Chars(0) == '"' && list6[k].get_Chars(list6[k].Length - 1) == '"')
						{
							list6[k] = list6[k].Substring(1, list6[k].Length - 2);
						}
					}
					string text3 = list6[0].ToLower();
					List<string> list7 = new List<string>();
					string text4 = string.Empty;
					bool flag2 = false;
					for (int l = 0; l < text3.Length; l++)
					{
						char c = text3.get_Chars(l);
						if (c == '<' && !flag2)
						{
							flag2 = true;
						}
						else if (c == '>' && flag2)
						{
							list7.Add(text4);
							text4 = string.Empty;
							flag2 = false;
						}
						else if (flag2)
						{
							text4 += c.ToString();
						}
					}
					if (list7.Count > 0)
					{
						list.Add("New set of keys found on line " + text2);
						num3 = 0;
						list5.Clear();
						list5.AddRange(list7);
					}
					else
					{
						num3++;
						list.Add("Gender variation ++ : " + num3.ToString());
					}
					using (List<string>.Enumerator enumerator = list5.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text5 = enumerator.Current;
							string text6 = string.Empty;
							string text7 = string.Empty;
							int m = 0;
							while (m < text5.Length)
							{
								if (text5.get_Chars(m) == ':')
								{
									text6 = text5.Substring(0, m);
									text7 = text5.Substring(m + 1);
									if (num3 == 1)
									{
										text7 += "_F";
										break;
									}
									if (num3 == 2)
									{
										text7 += "_NB";
										break;
									}
									break;
								}
								else
								{
									m++;
								}
							}
							if (text7.Length > 0)
							{
								int i;
								for (i = 2; i <= LanguageConfigLoader.Instance.fileInputConfig.Count; i = num5 + 1)
								{
									if (list6[i] != null && list6[i].Length > 0)
									{
										LanguageConfigLoader.LocInput locInput = LanguageConfigLoader.Instance.fileInputConfig.Find((LanguageConfigLoader.LocInput item) => item.documentColumn == i);
										if (locInput != null)
										{
											if (dictionary.ContainsKey(locInput.languageCode) && dictionary[locInput.languageCode].ContainsKey(text6) && dictionary[locInput.languageCode][text6].ContainsKey(text7))
											{
												string text8;
												string text9;
												string text10;
												string text11;
												bool flag3;
												string text12;
												Strings.ParseLine(dictionary[locInput.languageCode][text6][text7], out text8, out text9, out text10, out text11, out num5, out flag3, out text12, true);
												text12 = text12.Trim();
												if (text12 != null && text12.Length > 0)
												{
													DateTime dateTime2;
													if (DateTime.TryParse(text12, ref dateTime2))
													{
														if (dateTime2 > dateTime)
														{
															Debug.Log(string.Concat(new string[]
															{
																"Existing key ",
																locInput.languageCode,
																", ",
																text6,
																", ",
																text7,
																" is newer (",
																dateTime2.ToString(),
																") than the input date (",
																dateTime.ToString(),
																"), skipping..."
															}));
															list.Add(string.Concat(new string[]
															{
																"Existing key ",
																locInput.languageCode,
																", ",
																text6,
																", ",
																text7,
																" is newer (",
																dateTime2.ToString(),
																") than the input date (",
																dateTime.ToString(),
																"), skipping..."
															}));
															goto IL_985;
														}
														if (dateTime2 < dateTime)
														{
															list.Add(string.Concat(new string[]
															{
																"Existing key ",
																locInput.languageCode,
																", ",
																text6,
																", ",
																text7,
																" is older (",
																dateTime2.ToString(),
																") than the input date (",
																dateTime.ToString(),
																"), overwriting with new content..."
															}));
														}
													}
													else
													{
														Debug.Log(string.Concat(new string[]
														{
															"Unable to parse date: ",
															text12,
															" (Key: ",
															locInput.languageCode,
															", ",
															text6,
															", ",
															text7,
															")"
														}));
														list.Add(string.Concat(new string[]
														{
															"Unable to parse date: ",
															text12,
															" (Key: ",
															locInput.languageCode,
															", ",
															text6,
															", ",
															text7,
															")"
														}));
													}
												}
											}
											if (!dictionary.ContainsKey(locInput.languageCode))
											{
												dictionary.Add(locInput.languageCode, new Dictionary<string, Dictionary<string, string>>());
											}
											if (!dictionary[locInput.languageCode].ContainsKey(text6))
											{
												dictionary[locInput.languageCode].Add(text6, new Dictionary<string, string>());
											}
											if (!dictionary[locInput.languageCode][text6].ContainsKey(text7))
											{
												dictionary[locInput.languageCode][text6].Add(text7, string.Empty);
											}
											string text13 = list6[i];
											if (text13 == null || text13.Length <= 0)
											{
												text13 = Strings.missingString;
												list.Add(string.Concat(new string[]
												{
													"Missing: ",
													locInput.languageCode,
													", ",
													text6,
													", ",
													text7,
													" - Replacing with missing string - ",
													Strings.missingString
												}));
											}
											string text14 = string.Concat(new string[]
											{
												"\"",
												text7,
												"\",\"",
												string.Empty,
												"\",\"",
												text13,
												"\",\"",
												string.Empty,
												"\",",
												string.Empty,
												",\"",
												string.Empty,
												"\",\"",
												this.inputDate,
												"\""
											});
											if (num3 > 0)
											{
												list3.Add(string.Concat(new string[]
												{
													text6,
													":",
													text7,
													" (",
													locInput.languageCode,
													")"
												}));
												if (!list4.Contains(text6))
												{
													list4.Add(text6);
												}
											}
											dictionary[locInput.languageCode][text6][text7] = text14;
											num++;
										}
										else
										{
											string[] array = new string[5];
											array[0] = "Skipping column ";
											array[1] = i.ToString();
											array[2] = "/";
											int num6 = 3;
											num5 = list6.Count;
											array[num6] = num5.ToString();
											array[4] = " as there is no configuration...";
											Debug.Log(string.Concat(array));
											List<string> list8 = list;
											string[] array2 = new string[5];
											array2[0] = "Skipping column ";
											array2[1] = i.ToString();
											array2[2] = "/";
											int num7 = 3;
											num5 = list6.Count;
											array2[num7] = num5.ToString();
											array2[4] = " as there is no configuration...";
											list8.Add(string.Concat(array2));
										}
									}
									IL_985:
									num5 = i;
								}
							}
						}
						goto IL_9E7;
					}
					goto IL_9D0;
				}
				goto IL_9D0;
				IL_9E7:
				num2++;
				continue;
				IL_9D0:
				list.Add("No contents found on line " + num2.ToString());
				goto IL_9E7;
			}
		}
		foreach (KeyValuePair<string, Dictionary<string, Dictionary<string, string>>> keyValuePair in dictionary)
		{
			foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair2 in keyValuePair.Value)
			{
				string text15 = string.Concat(new string[]
				{
					streamingAssetsPath,
					"/Strings/",
					keyValuePair.Key,
					"/",
					keyValuePair2.Key,
					".csv"
				});
				if (keyValuePair2.Key != "names.rooms" && keyValuePair2.Key.Length >= 5 && keyValuePair2.Key.Substring(0, 5) == "names")
				{
					string[] array3 = new string[5];
					array3[0] = "Skipping file: ";
					array3[1] = text15;
					array3[2] = " (";
					int num8 = 3;
					num5 = keyValuePair2.Value.Count;
					array3[num8] = num5.ToString();
					array3[4] = " keys)...";
					Debug.Log(string.Concat(array3));
					List<string> list9 = list;
					string[] array4 = new string[5];
					array4[0] = "Skipping file: ";
					array4[1] = text15;
					array4[2] = " (";
					int num9 = 3;
					num5 = keyValuePair2.Value.Count;
					array4[num9] = num5.ToString();
					array4[4] = " keys)...";
					list9.Add(string.Concat(array4));
				}
				else
				{
					List<string> list10 = new List<string>(list2);
					foreach (KeyValuePair<string, string> keyValuePair3 in keyValuePair2.Value)
					{
						list10.Add(keyValuePair3.Value);
					}
					string[] array5 = new string[5];
					array5[0] = "Writing: ";
					array5[1] = text15;
					array5[2] = " (";
					int num10 = 3;
					num5 = keyValuePair2.Value.Count;
					array5[num10] = num5.ToString();
					array5[4] = " keys)...";
					Debug.Log(string.Concat(array5));
					List<string> list11 = list;
					string[] array6 = new string[5];
					array6[0] = "Writing: ";
					array6[1] = text15;
					array6[2] = " (";
					int num11 = 3;
					num5 = keyValuePair2.Value.Count;
					array6[num11] = num5.ToString();
					array6[4] = " keys)...";
					list11.Add(string.Concat(array6));
					File.WriteAllLines(text15, list10);
				}
			}
		}
		string[] array7 = new string[5];
		array7[0] = "Written ";
		array7[1] = num.ToString();
		array7[2] = " keys across ";
		int num12 = 3;
		num5 = dictionary.Count;
		array7[num12] = num5.ToString();
		array7[4] = " lanuages (exluding ENG)...";
		Debug.Log(string.Concat(array7));
		List<string> list12 = list;
		string[] array8 = new string[5];
		array8[0] = "Written ";
		array8[1] = num.ToString();
		array8[2] = " keys across ";
		int num13 = 3;
		num5 = dictionary.Count;
		array8[num13] = num5.ToString();
		array8[4] = " lanuages (exluding ENG)...";
		list12.Add(string.Concat(array8));
		string[] array9 = new string[5];
		array9[0] = "Written ";
		int num14 = 1;
		num5 = list3.Count;
		array9[num14] = num5.ToString();
		array9[2] = " gender variations of keys across ";
		int num15 = 3;
		num5 = list4.Count;
		array9[num15] = num5.ToString();
		array9[4] = " files...";
		Debug.Log(string.Concat(array9));
		List<string> list13 = list;
		string[] array10 = new string[5];
		array10[0] = "Written ";
		int num16 = 1;
		num5 = list3.Count;
		array10[num16] = num5.ToString();
		array10[2] = " gender variations of keys across ";
		int num17 = 3;
		num5 = list4.Count;
		array10[num17] = num5.ToString();
		array10[4] = " files...";
		list13.Add(string.Concat(array10));
		foreach (string text16 in list4)
		{
			list.Add(text16);
		}
		Debug.Log("Localization update complete!");
		list.Add("Localization update complete!");
		StreamWriter streamWriter = new StreamWriter(streamingAssetsPath + "/Strings/Localization/Input/OutputDebug.txt", false);
		for (int n = 0; n < list.Count; n++)
		{
			streamWriter.WriteLine(list[n]);
		}
		streamWriter.Close();
		StreamWriter streamWriter2 = new StreamWriter(streamingAssetsPath + "/Strings/Localization/Input/GenderVariationKeys.txt", false);
		for (int num18 = 0; num18 < list3.Count; num18++)
		{
			streamWriter2.WriteLine(list3[num18]);
		}
		streamWriter2.Close();
	}

	// Token: 0x0600263E RID: 9790 RVA: 0x001ED318 File Offset: 0x001EB518
	public static Human GetVmailSender(StateSaveData.MessageThreadSave msgThread, int msgIndex, out string nameString)
	{
		nameString = string.Empty;
		Human human = null;
		if (CityData.Instance.GetHuman(msgThread.senders[msgIndex], out human, true))
		{
			nameString = human.GetCitizenName();
			Game.Log("Got vmail sender " + nameString + " from ID " + msgThread.senders[msgIndex].ToString(), 2);
		}
		else
		{
			Game.Log("Failed to get vmail sender from ID " + msgThread.senders[msgIndex].ToString(), 2);
		}
		DDSSaveClasses.DDSTreeSave ddstreeSave = Toolbox.Instance.allDDSTrees[msgThread.treeID];
		DDSSaveClasses.DDSMessageSettings ddsmessageSettings = ddstreeSave.messageRef[msgThread.messages[msgIndex]];
		DDSSaveClasses.DDSParticipant ddsparticipant = ddstreeSave.participantA;
		if (ddsmessageSettings.saidBy == 1)
		{
			ddsparticipant = ddstreeSave.participantB;
		}
		else if (ddsmessageSettings.saidBy == 2)
		{
			ddsparticipant = ddstreeSave.participantC;
		}
		else if (ddsmessageSettings.saidBy == 3)
		{
			ddsparticipant = ddstreeSave.participantD;
		}
		if (ddsparticipant.connection == Acquaintance.ConnectionType.storyPartner)
		{
			nameString = Strings.ComposeText("|story.partnerfullname|", Player.Instance, Strings.LinkSetting.forceNoLinks, null, null, false);
		}
		else if (ddsparticipant.connection >= Acquaintance.ConnectionType.corpDove && ddsparticipant.connection <= Acquaintance.ConnectionType.pestControl)
		{
			nameString = Strings.Get("computer", ddsparticipant.connection.ToString(), Strings.Casing.asIs, false, false, false, null);
		}
		if (nameString.Length <= 0)
		{
			nameString = Strings.Get("computer", "Unknown", Strings.Casing.asIs, false, false, false, null);
		}
		return human;
	}

	// Token: 0x0600263F RID: 9791 RVA: 0x001ED488 File Offset: 0x001EB688
	public static Human GetVmailReciever(StateSaveData.MessageThreadSave msgThread, int msgIndex, out string nameString)
	{
		nameString = string.Empty;
		Human human = null;
		if (CityData.Instance.GetHuman(msgThread.recievers[msgIndex], out human, true))
		{
			nameString = human.GetCitizenName();
			Game.Log("Got vmail receiver " + nameString + " from ID " + msgThread.recievers[msgIndex].ToString(), 2);
		}
		else
		{
			Game.Log("Failed to get vmail receiver from ID " + msgThread.senders[msgIndex].ToString(), 2);
		}
		DDSSaveClasses.DDSTreeSave ddstreeSave = Toolbox.Instance.allDDSTrees[msgThread.treeID];
		DDSSaveClasses.DDSMessageSettings ddsmessageSettings = ddstreeSave.messageRef[msgThread.messages[msgIndex]];
		DDSSaveClasses.DDSParticipant ddsparticipant = ddstreeSave.participantA;
		if (ddsmessageSettings.saidTo == 1)
		{
			ddsparticipant = ddstreeSave.participantB;
		}
		else if (ddsmessageSettings.saidTo == 2)
		{
			ddsparticipant = ddstreeSave.participantC;
		}
		else if (ddsmessageSettings.saidTo == 3)
		{
			ddsparticipant = ddstreeSave.participantD;
		}
		if (ddsparticipant.connection == Acquaintance.ConnectionType.storyPartner)
		{
			nameString = Strings.ComposeText("|story.partnerfullname|", Player.Instance, Strings.LinkSetting.forceNoLinks, null, null, false);
		}
		else if (ddsparticipant.connection >= Acquaintance.ConnectionType.corpDove && ddsparticipant.connection <= Acquaintance.ConnectionType.pestControl)
		{
			nameString = Strings.Get("computer", ddsparticipant.connection.ToString(), Strings.Casing.asIs, false, false, false, null);
		}
		if (nameString.Length <= 0)
		{
			nameString = Strings.Get("computer", "Unknown", Strings.Casing.asIs, false, false, false, null);
		}
		return human;
	}

	// Token: 0x06002640 RID: 9792 RVA: 0x001ED5F8 File Offset: 0x001EB7F8
	public static string ComposeText(string input, object baseObject, Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic, List<Evidence.DataKey> evidenceKeys = null, object additionalObject = null, bool forceKnownCitizenGender = false)
	{
		if (!Strings.textFilesLoaded)
		{
			Strings.Instance.LoadTextFiles();
		}
		List<string> list = Enumerable.ToList<string>(input.Split(new char[]
		{
			'|'
		}, 0));
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		if (baseObject != null)
		{
			VMailApp.VmailParsingData thread = baseObject as VMailApp.VmailParsingData;
			if (thread != null && thread.thread.dsID > -1 && thread.thread.ds == StateSaveData.CustomDataSource.groupID)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Override with group ID " + thread.thread.dsID.ToString(), 2);
				}
				GroupsController.SocialGroup socialGroup = GroupsController.Instance.groups.Find((GroupsController.SocialGroup item) => item.id == thread.thread.dsID);
				if (socialGroup != null)
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Overwrite group as input object: " + socialGroup.id.ToString(), 2);
					}
					baseObject = socialGroup;
				}
				else if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Unable to find group " + socialGroup.id.ToString(), 2);
				}
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			string text = list[i];
			if (flag && text.Length > 0)
			{
				bool flag2 = false;
				bool flag3 = false;
				if (char.IsUpper(text.get_Chars(0)))
				{
					flag2 = true;
				}
				if (flag2 && char.IsUpper(text.get_Chars(text.Length - 1)))
				{
					flag3 = true;
				}
				DDSScope baseScope = null;
				Object @object = baseObject as Human;
				VMailApp.VmailParsingData vmailParsingData = baseObject as VMailApp.VmailParsingData;
				Interactable interactable = baseObject as Interactable;
				MurderController.Murder murder = baseObject as MurderController.Murder;
				NewGameLocation newGameLocation = baseObject as NewGameLocation;
				Evidence evidence = baseObject as Evidence;
				SideJob sideJob = baseObject as SideJob;
				SyncDiskPreset c = baseObject as SyncDiskPreset;
				GroupsController.SocialGroup socialGroup2 = baseObject as GroupsController.SocialGroup;
				bool knowCitizenGender = false;
				if (@object != null || vmailParsingData != null)
				{
					baseScope = GameplayControls.Instance.humanScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'human'...", 2);
					}
					knowCitizenGender = true;
				}
				else if (interactable != null)
				{
					baseScope = GameplayControls.Instance.itemScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'item'...", 2);
					}
					knowCitizenGender = true;
				}
				else if (murder != null)
				{
					baseScope = GameplayControls.Instance.murderScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'murder'...", 2);
					}
				}
				else if (newGameLocation != null)
				{
					baseScope = GameplayControls.Instance.locationScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'location'...", 2);
					}
				}
				else if (sideJob != null)
				{
					baseScope = GameplayControls.Instance.sideJobScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'side job'...", 2);
					}
				}
				else if (c != null)
				{
					baseScope = GameplayControls.Instance.syncDiskScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'sync disk'...", 2);
					}
				}
				else if (socialGroup2 != null)
				{
					baseScope = GameplayControls.Instance.groupScope;
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Base scope is 'group'...", 2);
					}
				}
				else if (evidence != null)
				{
					if (evidence.meta != null)
					{
						baseScope = GameplayControls.Instance.itemScope;
						if (Game.Instance.printDebug)
						{
							Game.Log("DDS: Base scope is 'item' (meta)...", 2);
						}
						knowCitizenGender = true;
					}
					else
					{
						baseScope = GameplayControls.Instance.evidenceScope;
						if (Game.Instance.printDebug)
						{
							Game.Log("DDS: Base scope is 'evidence'...", 2);
						}
					}
				}
				else if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Base scope is 'null'...", 2);
				}
				if (forceKnownCitizenGender)
				{
					knowCitizenGender = true;
				}
				try
				{
					text = Strings.ScopeParser(text, baseScope, baseObject, linkSetting, evidenceKeys, additionalObject, knowCitizenGender);
				}
				catch
				{
					Game.LogError("Unable to parse text", 2);
				}
				if (flag3 || (flag2 && text.Length <= 1))
				{
					text = text.ToUpper();
				}
				else if (flag2)
				{
					text = text.Substring(0, 1).ToUpper() + text.Substring(1, text.Length - 1);
				}
			}
			flag = !flag;
			stringBuilder.Append(text);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06002641 RID: 9793 RVA: 0x001EDA44 File Offset: 0x001EBC44
	public static string ScopeParser(string input, DDSScope baseScope, object baseObject, Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic, List<Evidence.DataKey> evidenceKeys = null, object additionalObject = null, bool knowCitizenGender = false)
	{
		string text = string.Empty;
		if (Game.Instance.printDebug)
		{
			Game.Log(string.Concat(new string[]
			{
				"DDS: Scope parser: ",
				input,
				" in base object ",
				(baseObject != null) ? baseObject.ToString() : null,
				"..."
			}), 2);
		}
		string[] array = input.Split('.', 0);
		DDSScope ddsscope = baseScope;
		object inputObject = baseObject;
		Evidence baseEvidence = null;
		if (linkSetting == Strings.LinkSetting.automatic && baseScope != null)
		{
			if (baseScope.name == "object")
			{
				linkSetting = Strings.LinkSetting.forceLinks;
				baseEvidence = Strings.GetEvidenceFromBaseScope(baseObject);
			}
			else if (baseScope.name == "citizen")
			{
				if (baseObject as Human == null)
				{
					if (!(baseObject is VMailApp.VmailParsingData))
					{
						if (baseObject is Interactable)
						{
							linkSetting = Strings.LinkSetting.forceLinks;
							baseEvidence = Strings.GetEvidenceFromBaseScope(baseObject);
						}
					}
					else
					{
						linkSetting = Strings.LinkSetting.forceLinks;
						baseEvidence = Strings.GetEvidenceFromBaseScope(baseObject);
					}
				}
				else
				{
					linkSetting = Strings.LinkSetting.forceNoLinks;
				}
			}
			else if (baseScope.name == "murder")
			{
				linkSetting = Strings.LinkSetting.forceLinks;
			}
			else if (baseScope.name == "evidence")
			{
				linkSetting = Strings.LinkSetting.forceLinks;
				baseEvidence = Strings.GetEvidenceFromBaseScope(baseObject);
			}
			else if (baseScope.name == "location")
			{
				linkSetting = Strings.LinkSetting.forceLinks;
			}
			else if (baseScope.name == "company")
			{
				linkSetting = Strings.LinkSetting.forceLinks;
			}
			else
			{
				linkSetting = Strings.LinkSetting.forceNoLinks;
			}
		}
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i];
			if (i == array.Length - 1)
			{
				string withinScope = string.Empty;
				if (ddsscope != null)
				{
					withinScope = ddsscope.name;
				}
				text = Strings.GetContainedValue(baseObject, withinScope, text2, inputObject, baseEvidence, linkSetting, evidenceKeys, additionalObject, knowCitizenGender);
			}
			else
			{
				ddsscope = Strings.GetContainedScope(baseScope, ddsscope, text2, inputObject, out inputObject, additionalObject);
			}
		}
		if (Game.Instance.printDebug)
		{
			Game.Log("DDS: Scope parser return: " + text, 2);
		}
		return text;
	}

	// Token: 0x06002642 RID: 9794 RVA: 0x001EDC1C File Offset: 0x001EBE1C
	public static DDSScope GetContainedScope(DDSScope baseScope, DDSScope currentScope, string newScope, object inputObject, out object outputObject, object additionalObject)
	{
		outputObject = inputObject;
		DDSScope ddsscope = null;
		newScope = newScope.ToLower();
		if (currentScope != null)
		{
			DDSScope.ContainedScope containedScope = currentScope.containedScopes.Find((DDSScope.ContainedScope item) => item.name == newScope);
			if (containedScope != null)
			{
				ddsscope = containedScope.type;
				outputObject = Strings.GetScopeObject(baseScope, outputObject, currentScope.name, containedScope.name, null, additionalObject);
			}
			else if (newScope == currentScope.name)
			{
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Required scope " + newScope + " is same as current scope " + currentScope.name, 2);
				}
				ddsscope = currentScope;
				outputObject = inputObject;
			}
		}
		if (ddsscope == null && Toolbox.Instance.globalScopeDictionary.TryGetValue(newScope, ref ddsscope))
		{
			outputObject = Strings.GetScopeObject(baseScope, outputObject, ddsscope.name, newScope, null, additionalObject);
		}
		if (ddsscope == null)
		{
			if (currentScope != null)
			{
				Game.LogError("Unable to get contained scope " + newScope + " within scope " + currentScope.name, 2);
			}
			else
			{
				Game.LogError("Unable to get contained scope " + newScope + " without a preceeding scope!", 2);
			}
		}
		if (ddsscope != null)
		{
			if (Game.Instance.printDebug)
			{
				string text = "DDS: Switch scope > ";
				string name = ddsscope.name;
				string text2 = ", object: ";
				object obj = outputObject;
				Game.Log(text + name + text2 + ((obj != null) ? obj.ToString() : null), 2);
			}
		}
		else if (Game.Instance.printDebug)
		{
			string text3 = "DDS: Switch scope > null, object: ";
			object obj2 = outputObject;
			Game.Log(text3 + ((obj2 != null) ? obj2.ToString() : null), 2);
		}
		return ddsscope;
	}

	// Token: 0x06002643 RID: 9795 RVA: 0x001EDDD8 File Offset: 0x001EBFD8
	public static object GetScopeObject(DDSScope baseScope, object inputObject, string withinScope, string newType, List<Evidence.DataKey> evidenceKeys = null, object additionalObject = null)
	{
		object obj = null;
		withinScope = withinScope.ToLower();
		newType = newType.ToLower();
		if (newType == withinScope)
		{
			return inputObject;
		}
		try
		{
			if (withinScope == "citizen")
			{
				Human human = inputObject as Human;
				VMailApp.VmailParsingData vmailParsingData = null;
				Interactable interactable = null;
				if (human == null)
				{
					vmailParsingData = (inputObject as VMailApp.VmailParsingData);
					if (vmailParsingData == null)
					{
						interactable = (inputObject as Interactable);
						if (interactable == null)
						{
							Game.LogError("Value error: Unable to convert input object to Human: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
							return obj;
						}
						human = interactable.belongsTo;
					}
					else
					{
						string text;
						human = Strings.GetVmailSender(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
					}
				}
				if (newType == "job")
				{
					obj = human.job;
				}
				else if (newType == "home")
				{
					obj = human.home;
				}
				else if (newType == "den")
				{
					obj = human.den;
				}
				else if (newType == "receiver")
				{
					if (vmailParsingData != null)
					{
						string text;
						obj = Strings.GetVmailReciever(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
					}
					else if (interactable != null)
					{
						obj = interactable.reciever;
					}
					else if (additionalObject != null && additionalObject as Human != null)
					{
						obj = additionalObject;
					}
					else if (human.isPlayer && InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor != null)
					{
						obj = InteractionController.Instance.talkingTo.isActor;
					}
					else if (!human.isPlayer && InteractionController.Instance.talkingTo != null && InteractionController.Instance.talkingTo.isActor == human)
					{
						obj = Player.Instance;
					}
					else if (human.inConversation && human.currentConversation != null)
					{
						obj = human.currentConversation.currentlyTalkingTo;
					}
				}
				else if (newType == "location")
				{
					obj = human.currentGameLocation;
				}
				else if (newType == "sightinglocation")
				{
					Human human2 = null;
					if (vmailParsingData != null)
					{
						string text;
						human2 = Strings.GetVmailReciever(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
					}
					else if (interactable != null)
					{
						human2 = interactable.reciever;
					}
					else if (additionalObject != null && additionalObject as Human != null)
					{
						human2 = (additionalObject as Human);
					}
					else if (human.inConversation && human.currentConversation != null)
					{
						human2 = human.currentConversation.currentlyTalkingTo;
					}
					if (human2 != null && human.lastSightings.ContainsKey(human2))
					{
						NewNode newNode = null;
						if (PathFinder.Instance.nodeMap.TryGetValue(human.lastSightings[human2].node, ref newNode))
						{
							obj = newNode.gameLocation;
						}
					}
				}
				else if (newType == "sightingtime")
				{
					Human human3 = null;
					if (vmailParsingData != null)
					{
						string text;
						human3 = Strings.GetVmailReciever(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
					}
					else if (interactable != null)
					{
						human3 = interactable.reciever;
					}
					else if (additionalObject != null && additionalObject as Human != null)
					{
						human3 = (additionalObject as Human);
					}
					else if (human.inConversation && human.currentConversation != null)
					{
						human3 = human.currentConversation.currentlyTalkingTo;
					}
					if (human3 != null && human.lastSightings != null && human.lastSightings.ContainsKey(human3))
					{
						obj = human.lastSightings[human3].time;
					}
				}
				else
				{
					if (newType == "sightingclosest")
					{
						Human human4 = null;
						if (vmailParsingData != null)
						{
							string text;
							human4 = Strings.GetVmailReciever(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
						}
						else if (interactable != null)
						{
							human4 = interactable.reciever;
						}
						else if (additionalObject != null && additionalObject as Human != null)
						{
							human4 = (additionalObject as Human);
						}
						else if (human.inConversation && human.currentConversation != null)
						{
							human4 = human.currentConversation.currentlyTalkingTo;
						}
						if (!(human4 != null) || !human.lastSightings.ContainsKey(human4))
						{
							goto IL_2C29;
						}
						NewNode newNode2 = null;
						if (!PathFinder.Instance.nodeMap.TryGetValue(human.lastSightings[human4].node, ref newNode2))
						{
							goto IL_2C29;
						}
						obj = null;
						float num = float.PositiveInfinity;
						foreach (NewNode.NodeAccess nodeAccess in newNode2.gameLocation.entrances)
						{
							if (nodeAccess.walkingAccess)
							{
								if (nodeAccess.toNode.gameLocation != newNode2.gameLocation)
								{
									if (nodeAccess.toNode.gameLocation.thisAsStreet != null || nodeAccess.toNode.gameLocation.thisAsAddress.company != null || nodeAccess.toNode.gameLocation.thisAsAddress.residence != null)
									{
										float num2 = Vector3.Distance(nodeAccess.worldAccessPoint, newNode2.position);
										if (num2 < num)
										{
											obj = nodeAccess.toNode.gameLocation;
											num = num2;
										}
									}
								}
								else if (nodeAccess.fromNode.gameLocation != newNode2.gameLocation && (nodeAccess.fromNode.gameLocation.thisAsStreet != null || nodeAccess.fromNode.gameLocation.thisAsAddress.company != null || nodeAccess.fromNode.gameLocation.thisAsAddress.residence != null))
								{
									float num3 = Vector3.Distance(nodeAccess.worldAccessPoint, newNode2.position);
									if (num3 < num)
									{
										obj = nodeAccess.fromNode.gameLocation;
										num = num3;
									}
								}
							}
						}
						if (obj != null)
						{
							goto IL_2C29;
						}
						using (List<Company>.Enumerator enumerator2 = CityData.Instance.companyDirectory.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Company company2 = enumerator2.Current;
								if (company2.placeOfBusiness != null)
								{
									foreach (NewNode.NodeAccess nodeAccess2 in company2.placeOfBusiness.entrances)
									{
										if (nodeAccess2.walkingAccess && nodeAccess2.toNode.nodeCoord.z == newNode2.nodeCoord.z)
										{
											float num4 = Vector3.Distance(nodeAccess2.worldAccessPoint, newNode2.position);
											if (num4 < num)
											{
												obj = nodeAccess2.toNode.gameLocation;
												num = num4;
											}
										}
									}
								}
							}
							goto IL_2C29;
						}
					}
					if (newType == "a")
					{
						if (human.inConversation && human.currentConversation != null)
						{
							obj = human.currentConversation.participantA;
						}
					}
					else if (newType == "b")
					{
						if (human.inConversation && human.currentConversation != null)
						{
							obj = human.currentConversation.participantB;
						}
					}
					else if (newType == "c")
					{
						if (human.inConversation && human.currentConversation != null)
						{
							obj = human.currentConversation.participantC;
						}
					}
					else if (newType == "d")
					{
						if (human.inConversation && human.currentConversation != null)
						{
							obj = human.currentConversation.participantD;
						}
					}
					else if (newType == "partner")
					{
						obj = human.partner;
					}
					else if (newType == "paramour")
					{
						if (human.paramour != null)
						{
							obj = human.paramour;
						}
						else
						{
							obj = human.partner;
						}
					}
					else if (newType == "doctor")
					{
						obj = human.GetDoctor();
					}
					else
					{
						if (newType == "friend")
						{
							float num5 = -99999f;
							using (List<Acquaintance>.Enumerator enumerator3 = human.acquaintances.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									Acquaintance acquaintance = enumerator3.Current;
									if (acquaintance.known >= 0.33f)
									{
										float num6 = acquaintance.known + acquaintance.like;
										if (num6 > num5)
										{
											obj = acquaintance.with;
											num5 = num6;
										}
									}
								}
								goto IL_2C29;
							}
						}
						if (newType == "enemy")
						{
							float num7 = 99999f;
							using (List<Acquaintance>.Enumerator enumerator3 = human.acquaintances.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									Acquaintance acquaintance2 = enumerator3.Current;
									if (acquaintance2.known >= 0.2f)
									{
										float num8 = acquaintance2.known + acquaintance2.like;
										if (num8 < num7)
										{
											obj = acquaintance2.with;
											num7 = num8;
										}
									}
								}
								goto IL_2C29;
							}
						}
						if (newType == "neighbour")
						{
							float num9 = -99999f;
							using (List<Acquaintance>.Enumerator enumerator3 = human.acquaintances.GetEnumerator())
							{
								while (enumerator3.MoveNext())
								{
									Acquaintance acquaintance3 = enumerator3.Current;
									if (acquaintance3.connections.Contains(Acquaintance.ConnectionType.neighbor))
									{
										float num10 = acquaintance3.known + acquaintance3.like;
										if (num10 > num9)
										{
											obj = acquaintance3.with;
											num9 = num10;
										}
									}
								}
								goto IL_2C29;
							}
						}
						if (newType == "landlord")
						{
							obj = human.GetLandlord();
						}
						else if (newType == "faveatery")
						{
							NewAddress newAddress = null;
							if (human.favouritePlaces.TryGetValue(CompanyPreset.CompanyCategory.meal, ref newAddress))
							{
								obj = newAddress;
							}
						}
						else if (newType == "favrecreation")
						{
							NewAddress newAddress2 = null;
							if (human.favouritePlaces.TryGetValue(CompanyPreset.CompanyCategory.recreational, ref newAddress2))
							{
								obj = newAddress2;
							}
						}
						else if (newType == "group1")
						{
							if (human.groups.Count > 0)
							{
								List<GroupsController.SocialGroup> list = human.groups.FindAll((GroupsController.SocialGroup item) => Toolbox.Instance.groupsDictionary.ContainsKey(item.preset) && Toolbox.Instance.groupsDictionary[item.preset].groupType == GroupPreset.GroupType.interestGroup);
								if (list.Count > 0)
								{
									list.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
									obj = list[0];
								}
							}
						}
						else if (newType == "group2")
						{
							if (human.groups.Count > 1)
							{
								List<GroupsController.SocialGroup> list2 = human.groups.FindAll((GroupsController.SocialGroup item) => Toolbox.Instance.groupsDictionary.ContainsKey(item.preset) && Toolbox.Instance.groupsDictionary[item.preset].groupType == GroupPreset.GroupType.interestGroup);
								if (list2.Count > 1)
								{
									list2.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
									obj = list2[1];
								}
							}
						}
						else if (newType == "murder")
						{
							if (human.death != null && human.death.isDead)
							{
								obj = human.death.GetMurder();
							}
						}
						else if (newType == "sidejob" && additionalObject != null)
						{
							SideJob sideJob = additionalObject as SideJob;
							if (sideJob != null)
							{
								obj = sideJob;
							}
						}
					}
				}
			}
			else if (withinScope == "city")
			{
				if (newType == "time")
				{
					obj = SessionData.Instance.gameTime;
				}
				else if (newType == "player")
				{
					obj = Player.Instance;
				}
			}
			else if (withinScope == "company")
			{
				Company company = inputObject as Company;
				if (company == null)
				{
					Game.LogError("Value error: Unable to convert input object to Company: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
					return obj;
				}
				if (newType == "location")
				{
					obj = company.address;
				}
				else if (newType == "owner")
				{
					obj = company.director;
				}
				else if (newType == "receptionist")
				{
					obj = company.receptionist;
				}
				else if (newType == "security")
				{
					obj = company.security;
				}
				else if (newType == "janitor")
				{
					obj = company.janitor;
				}
				else if (newType == "employee")
				{
					List<Occupation> list3 = company.companyRoster.FindAll((Occupation item) => item.employee != null && item.employee != company.director);
					if (list3.Count > 0)
					{
						string str = company.companyID.ToString() + company.name;
						obj = list3[Toolbox.Instance.GetPsuedoRandomNumber(0, list3.Count, str, false)].employee;
					}
				}
			}
			else if (!(withinScope == "controls"))
			{
				if (withinScope == "group")
				{
					GroupsController.SocialGroup socialGroup = inputObject as GroupsController.SocialGroup;
					if (socialGroup == null)
					{
						Game.LogError("Value error: Unable to convert input object to Group: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
						return obj;
					}
					if (newType == "leader")
					{
						if (socialGroup.members.Count > 0)
						{
							Human human5 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[0], out human5, true))
							{
								obj = human5;
							}
						}
					}
					else if (newType == "member2")
					{
						if (socialGroup.members.Count > 1)
						{
							Human human6 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[1], out human6, true))
							{
								obj = human6;
							}
						}
					}
					else if (newType == "member3")
					{
						if (socialGroup.members.Count > 2)
						{
							Human human7 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[2], out human7, true))
							{
								obj = human7;
							}
						}
					}
					else if (newType == "member4")
					{
						if (socialGroup.members.Count > 3)
						{
							Human human8 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[3], out human8, true))
							{
								obj = human8;
							}
						}
					}
					else if (newType == "member5")
					{
						if (socialGroup.members.Count > 4)
						{
							Human human9 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[4], out human9, true))
							{
								obj = human9;
							}
						}
					}
					else if (newType == "member6")
					{
						if (socialGroup.members.Count > 5)
						{
							Human human10 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[5], out human10, true))
							{
								obj = human10;
							}
						}
					}
					else if (newType == "member7")
					{
						if (socialGroup.members.Count > 6)
						{
							Human human11 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[6], out human11, true))
							{
								obj = human11;
							}
						}
					}
					else if (newType == "member8")
					{
						if (socialGroup.members.Count > 6)
						{
							Human human12 = null;
							if (CityData.Instance.GetHuman(socialGroup.members[7], out human12, true))
							{
								obj = human12;
							}
						}
					}
					else if (newType == "meetingplace")
					{
						obj = socialGroup.GetMeetingPlace();
					}
					else if (newType == "nextmeetingtime")
					{
						obj = socialGroup.GetNextMeetingTime();
					}
				}
				else if (withinScope == "job")
				{
					Occupation occupation = inputObject as Occupation;
					if (occupation == null)
					{
						Game.LogError("Value error: Unable to convert input object to Job: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
						return obj;
					}
					if (newType == "employee")
					{
						obj = occupation.employee;
					}
					else if (newType == "employer")
					{
						obj = occupation.employer;
					}
				}
				else if (withinScope == "killer")
				{
					if (newType == "lastmurder")
					{
						float gameTime = SessionData.Instance.gameTime;
						Interactable interactable2 = inputObject as Interactable;
						if (interactable2 != null)
						{
							if (interactable2.TryGetCreationTime(out gameTime))
							{
								Game.Log("DDS: Getting last murder using an object's creation time: " + gameTime.ToString(), 2);
							}
							else
							{
								gameTime = SessionData.Instance.gameTime;
							}
						}
						obj = Strings.GetPreviousMurder(gameTime);
					}
					else if (newType == "nextmurder")
					{
						float gameTime2 = SessionData.Instance.gameTime;
						Interactable interactable3 = inputObject as Interactable;
						if (interactable3 != null)
						{
							if (interactable3.TryGetCreationTime(out gameTime2))
							{
								Game.Log("DDS: Getting next murder using an object's creation time: " + gameTime2.ToString(), 2);
							}
							else
							{
								gameTime2 = SessionData.Instance.gameTime;
							}
						}
						obj = Strings.GetNextMurder(gameTime2);
					}
					else if (newType == "location" && MurderController.Instance.currentMurderer != null)
					{
						obj = MurderController.Instance.currentMurderer.currentGameLocation;
					}
				}
				else if (withinScope == "location")
				{
					NewGameLocation newGameLocation = inputObject as NewGameLocation;
					NewAddress thisAsAddress = newGameLocation.thisAsAddress;
					StreetController thisAsStreet = newGameLocation.thisAsStreet;
					if (newGameLocation == null && thisAsAddress == null && thisAsStreet == null)
					{
						Game.LogError("Value error: Unable to convert input object to Location: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
						return obj;
					}
					if (newType == "company")
					{
						if (thisAsAddress != null)
						{
							obj = thisAsAddress.company;
						}
					}
					else if (newType == "owner")
					{
						if (thisAsAddress != null)
						{
							if (thisAsAddress.company != null)
							{
								obj = thisAsAddress.company.director;
							}
							if (obj == null && thisAsAddress.owners.Count > 0)
							{
								obj = thisAsAddress.owners[0];
							}
							if (obj == null && thisAsAddress.inhabitants.Count > 0)
							{
								obj = thisAsAddress.inhabitants[0];
							}
						}
					}
					else if (newType == "landlord")
					{
						if (thisAsAddress != null && thisAsAddress.residence != null && thisAsAddress.inhabitants.Count > 0)
						{
							obj = thisAsAddress.inhabitants[0].GetLandlord();
						}
					}
					else if (newType == "group1")
					{
						List<GroupsController.SocialGroup> list4 = new List<GroupsController.SocialGroup>();
						foreach (GroupsController.SocialGroup socialGroup2 in GroupsController.Instance.groups)
						{
							if ((!Toolbox.Instance.groupsDictionary.ContainsKey(socialGroup2.preset) || Toolbox.Instance.groupsDictionary[socialGroup2.preset].groupType == GroupPreset.GroupType.interestGroup) && socialGroup2.GetMeetingPlace() == newGameLocation)
							{
								list4.Add(socialGroup2);
							}
						}
						if (list4.Count > 0)
						{
							list4.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
							obj = list4[0];
						}
					}
					else if (newType == "group2")
					{
						List<GroupsController.SocialGroup> list5 = new List<GroupsController.SocialGroup>();
						foreach (GroupsController.SocialGroup socialGroup3 in GroupsController.Instance.groups)
						{
							if ((!Toolbox.Instance.groupsDictionary.ContainsKey(socialGroup3.preset) || Toolbox.Instance.groupsDictionary[socialGroup3.preset].groupType == GroupPreset.GroupType.interestGroup) && socialGroup3.GetMeetingPlace() == newGameLocation)
							{
								list5.Add(socialGroup3);
							}
						}
						if (list5.Count > 1)
						{
							list5.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
							obj = list5[1];
						}
					}
					else if (newType == "group3")
					{
						List<GroupsController.SocialGroup> list6 = new List<GroupsController.SocialGroup>();
						foreach (GroupsController.SocialGroup socialGroup4 in GroupsController.Instance.groups)
						{
							if ((!Toolbox.Instance.groupsDictionary.ContainsKey(socialGroup4.preset) || Toolbox.Instance.groupsDictionary[socialGroup4.preset].groupType == GroupPreset.GroupType.interestGroup) && socialGroup4.GetMeetingPlace() == newGameLocation)
							{
								list6.Add(socialGroup4);
							}
						}
						if (list6.Count > 2)
						{
							list6.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
							obj = list6[2];
						}
					}
					else if (newType == "group4")
					{
						List<GroupsController.SocialGroup> list7 = new List<GroupsController.SocialGroup>();
						foreach (GroupsController.SocialGroup socialGroup5 in GroupsController.Instance.groups)
						{
							if ((!Toolbox.Instance.groupsDictionary.ContainsKey(socialGroup5.preset) || Toolbox.Instance.groupsDictionary[socialGroup5.preset].groupType == GroupPreset.GroupType.interestGroup) && socialGroup5.GetMeetingPlace() == newGameLocation)
							{
								list7.Add(socialGroup5);
							}
						}
						if (list7.Count > 3)
						{
							list7.Sort((GroupsController.SocialGroup p1, GroupsController.SocialGroup p2) => p1.id.CompareTo(p2.id));
							obj = list7[3];
						}
					}
				}
				else if (withinScope == "murder")
				{
					MurderController.Murder murder = inputObject as MurderController.Murder;
					if (murder == null)
					{
						Game.LogError("Value error: Unable to convert input object to Murder: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
						return obj;
					}
					Human.Death death = murder.death;
					if (newType == "victim")
					{
						obj = murder.victim;
					}
					else if (newType == "killer")
					{
						obj = murder.murderer;
					}
					else if (newType == "weapon")
					{
						if (murder.weapon != null)
						{
							obj = murder.weapon;
						}
					}
					else if (newType == "location")
					{
						if (death != null)
						{
							obj = death.GetDeathLocation();
						}
					}
					else if (newType == "time")
					{
						if (death != null)
						{
							obj = death.time;
						}
					}
					else if (newType == "discovercitizen")
					{
						if (death != null)
						{
							obj = death.GetDiscoverer();
						}
					}
					else if (newType == "discovertime")
					{
						if (death != null)
						{
							obj = death.discoveredAt;
						}
					}
					else if (newType == "timefrom")
					{
						if (death != null)
						{
							obj = death.timeOfDeathRange.x;
						}
					}
					else if (newType == "timeto")
					{
						if (death != null)
						{
							obj = death.timeOfDeathRange.y;
						}
					}
					else if (newType == "callingcard")
					{
						obj = murder.callingCard;
					}
					else if (newType == "kidnapkilltime")
					{
						obj = murder.killTime;
					}
				}
				else if (withinScope == "object")
				{
					Interactable interactable4 = inputObject as Interactable;
					if (interactable4 != null)
					{
						if (newType == "owner")
						{
							obj = interactable4.belongsTo;
						}
						else if (newType == "writer")
						{
							obj = interactable4.writer;
						}
						else if (newType == "receiver")
						{
							obj = interactable4.reciever;
						}
						else if (newType == "other")
						{
							obj = interactable4.reciever;
						}
						else if (newType == "location")
						{
							if (interactable4.node != null)
							{
								obj = interactable4.node.gameLocation;
							}
						}
						else if (newType == "purchasedfrom")
						{
							EvidenceReceipt evidenceReceipt = interactable4.evidence as EvidenceReceipt;
							if (evidenceReceipt != null)
							{
								obj = evidenceReceipt.soldHere;
							}
							else
							{
								Game.LogError("Scope error: Trying to get purchased place from non-receipt evidence", 2);
							}
						}
						else if (newType == "purchasedtime")
						{
							EvidenceReceipt evidenceReceipt2 = interactable4.evidence as EvidenceReceipt;
							if (evidenceReceipt2 != null)
							{
								obj = evidenceReceipt2.purchasedTime;
							}
							else
							{
								Game.LogError("Scope error: Trying to get purchased time from non-receipt evidence", 2);
							}
						}
						else if (newType == "sidejob")
						{
							obj = interactable4.jobParent;
							if (obj == null && additionalObject != null)
							{
								SideJob sideJob2 = additionalObject as SideJob;
								if (sideJob2 != null)
								{
									obj = sideJob2;
								}
							}
						}
						else if (newType == "lastcall")
						{
							if (interactable4.t != null && interactable4.t.location != null && interactable4.t.location.building != null)
							{
								TelephoneController.PhoneCall phoneCall = null;
								float num11 = -999999f;
								foreach (TelephoneController.PhoneCall phoneCall2 in interactable4.t.location.building.callLog)
								{
									if (phoneCall2.to == interactable4.t.number && phoneCall2.time > num11)
									{
										phoneCall = phoneCall2;
										num11 = phoneCall2.time;
									}
								}
								if (phoneCall != null)
								{
									obj = phoneCall.fromNS.interactable;
								}
							}
						}
						else if (newType == "lastcalltime")
						{
							if (interactable4.t != null && interactable4.t.location != null && interactable4.t.location.building != null)
							{
								TelephoneController.PhoneCall phoneCall3 = null;
								float num12 = -999999f;
								foreach (TelephoneController.PhoneCall phoneCall4 in interactable4.t.location.building.callLog)
								{
									if (phoneCall4.to == interactable4.t.number && phoneCall4.time > num12)
									{
										phoneCall3 = phoneCall4;
										num12 = phoneCall4.time;
									}
								}
								if (phoneCall3 != null)
								{
									obj = num12;
								}
							}
						}
						else if (newType == "group")
						{
							obj = interactable4.group;
						}
						else if (newType == "forsale")
						{
							obj = interactable4.forSale;
						}
					}
					else
					{
						Evidence evidence = inputObject as Evidence;
						if (evidence.meta == null)
						{
							Game.LogError("Value error: Unable to convert input object to Object/MetaObject: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
							return obj;
						}
						if (newType == "owner")
						{
							if (evidence.meta.owner > -1)
							{
								Human human13 = null;
								if (CityData.Instance.GetHuman(evidence.meta.owner, out human13, true))
								{
									obj = human13;
								}
							}
						}
						else if (newType == "writer")
						{
							if (evidence.meta.writer > -1)
							{
								Human human14 = null;
								if (CityData.Instance.GetHuman(evidence.meta.writer, out human14, true))
								{
									obj = human14;
								}
							}
						}
						else if (newType == "receiver")
						{
							if (evidence.meta.reciever > -1)
							{
								Human human15 = null;
								if (CityData.Instance.GetHuman(evidence.meta.reciever, out human15, true))
								{
									obj = human15;
								}
							}
						}
						else if (newType == "other")
						{
							if (evidence.meta.reciever > -1)
							{
								Human human16 = null;
								if (CityData.Instance.GetHuman(evidence.meta.reciever, out human16, true))
								{
									obj = human16;
								}
							}
						}
						else if (newType == "location")
						{
							NewNode newNode3 = null;
							if (PathFinder.Instance.nodeMap.TryGetValue(evidence.meta.n, ref newNode3))
							{
								obj = newNode3.gameLocation;
							}
						}
						else if (newType == "purchasedfrom")
						{
							EvidenceReceipt evidenceReceipt3 = evidence as EvidenceReceipt;
							if (evidenceReceipt3 != null)
							{
								obj = evidenceReceipt3.soldHere;
							}
							else
							{
								Game.LogError("Scope error: Trying to get purchased place from non-receipt evidence", 2);
							}
						}
						else if (newType == "purchasedtime")
						{
							EvidenceReceipt evidenceReceipt4 = evidence as EvidenceReceipt;
							if (evidenceReceipt4 != null)
							{
								obj = evidenceReceipt4.purchasedTime;
							}
							else
							{
								Game.LogError("Scope error: Trying to get purchased time from non-receipt evidence", 2);
							}
						}
					}
				}
				else if (withinScope == "random")
				{
					string text2 = CityData.Instance.seed;
					if (baseScope != null)
					{
						text2 += baseScope.name;
					}
					if (inputObject != null)
					{
						Human human17 = inputObject as Human;
						if (human17 != null)
						{
							text2 = human17.humanID.ToString();
						}
						else
						{
							Interactable interactable5 = inputObject as Interactable;
							if (interactable5 != null)
							{
								text2 = interactable5.id.ToString();
							}
						}
					}
					if (newType == "citizen")
					{
						obj = CityData.Instance.citizenDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.citizenDirectory.Count, text2, false)];
					}
					else if (newType == "address")
					{
						obj = CityData.Instance.addressDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.addressDirectory.Count, text2, false)];
					}
					else if (newType == "street")
					{
						obj = CityData.Instance.streetDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.streetDirectory.Count, text2, false)];
					}
					else if (newType == "residence")
					{
						obj = CityData.Instance.residenceDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.residenceDirectory.Count, text2, false)];
					}
					else if (newType == "park")
					{
						List<NewAddress> list8 = CityData.Instance.addressDirectory.FindAll((NewAddress item) => item.addressPreset.name == "Park");
						if (list8.Count > 0)
						{
							obj = list8[Toolbox.Instance.GetPsuedoRandomNumber(0, list8.Count, text2, false)];
						}
					}
					else if (newType == "company")
					{
						obj = CityData.Instance.companyDirectory[Toolbox.Instance.GetPsuedoRandomNumber(0, CityData.Instance.companyDirectory.Count, text2, false)];
					}
					else if (newType == "eatery")
					{
						List<Company> list9 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.meal));
						if (list9.Count > 0)
						{
							obj = list9[Toolbox.Instance.GetPsuedoRandomNumber(0, list9.Count, text2, false)];
						}
					}
					else if (newType == "shop")
					{
						List<Company> list10 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.companyCategories.Contains(CompanyPreset.CompanyCategory.retail));
						if (list10.Count > 0)
						{
							obj = list10[Toolbox.Instance.GetPsuedoRandomNumber(0, list10.Count, text2, false)];
						}
					}
					else if (newType == "diner")
					{
						List<Company> list11 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "AmericanDiner");
						if (list11.Count > 0)
						{
							obj = list11[Toolbox.Instance.GetPsuedoRandomNumber(0, list11.Count, text2, false)];
						}
					}
					else if (newType == "bar")
					{
						List<Company> list12 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "Bar");
						if (list12.Count > 0)
						{
							obj = list12[Toolbox.Instance.GetPsuedoRandomNumber(0, list12.Count, text2, false)];
						}
					}
					else if (newType == "launderette")
					{
						List<Company> list13 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "Launderette");
						if (list13.Count > 0)
						{
							obj = list13[Toolbox.Instance.GetPsuedoRandomNumber(0, list13.Count, text2, false)];
						}
					}
					else if (newType == "syncclinic")
					{
						List<Company> list14 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "SyncClinic");
						if (list14.Count > 0)
						{
							obj = list14[Toolbox.Instance.GetPsuedoRandomNumber(0, list14.Count, text2, false)];
						}
					}
					else if (newType == "police")
					{
						List<Company> list15 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "EnforcerBranch");
						if (list15.Count > 0)
						{
							obj = list15[Toolbox.Instance.GetPsuedoRandomNumber(0, list15.Count, text2, false)];
						}
					}
					else if (newType == "hospital")
					{
						List<Company> list16 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "HospitalWing");
						if (list16.Count > 0)
						{
							obj = list16[Toolbox.Instance.GetPsuedoRandomNumber(0, list16.Count, text2, false)];
						}
					}
					else if (newType == "weaponsdealer")
					{
						List<Company> list17 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "WeaponsDealer");
						if (list17.Count > 0)
						{
							obj = list17[Toolbox.Instance.GetPsuedoRandomNumber(0, list17.Count, text2, false)];
						}
					}
					else if (newType == "blackmarkettrader")
					{
						List<Company> list18 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "BlackmarketTrader");
						if (list18.Count > 0)
						{
							obj = list18[Toolbox.Instance.GetPsuedoRandomNumber(0, list18.Count, text2, false)];
						}
					}
					else if (newType == "loanshark")
					{
						List<Company> list19 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "LoanShark");
						if (list19.Count > 0)
						{
							obj = list19[Toolbox.Instance.GetPsuedoRandomNumber(0, list19.Count, text2, false)];
						}
					}
					else if (newType == "blackmarketsyncclinic")
					{
						List<Company> list20 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "BlackmarketSyncClinic");
						if (list20.Count > 0)
						{
							obj = list20[Toolbox.Instance.GetPsuedoRandomNumber(0, list20.Count, text2, false)];
						}
					}
					else if (newType == "gamblingden")
					{
						List<Company> list21 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "GamblingDen");
						if (list21.Count > 0)
						{
							obj = list21[Toolbox.Instance.GetPsuedoRandomNumber(0, list21.Count, text2, false)];
						}
					}
					else if (newType == "hotel")
					{
						List<Company> list22 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "Ballroom");
						if (list22.Count > 0)
						{
							obj = list22[Toolbox.Instance.GetPsuedoRandomNumber(0, list22.Count, text2, false)];
						}
					}
					else if (newType == "industrial")
					{
						List<Company> list23 = CityData.Instance.companyDirectory.FindAll((Company item) => item.preset.name == "IndustrialPlant");
						if (list23.Count > 0)
						{
							obj = list23[Toolbox.Instance.GetPsuedoRandomNumber(0, list23.Count, text2, false)];
						}
					}
				}
				else if (withinScope == "sidejob")
				{
					SideJob sideJob3 = inputObject as SideJob;
					if (sideJob3 == null)
					{
						Game.LogError("Value error: Unable to convert input object to Side Job: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
						return obj;
					}
					if (newType == "sidejob")
					{
						obj = sideJob3;
					}
					else if (newType == "poster")
					{
						obj = sideJob3.poster;
					}
					else if (newType == "purp")
					{
						obj = sideJob3.purp;
					}
					else if (!(newType == "posttime"))
					{
						if (newType == "post")
						{
							obj = sideJob3.post;
						}
						else if (newType == "callphone")
						{
							if (sideJob3.chosenGooseChasePhone != null)
							{
								obj = sideJob3.chosenGooseChasePhone;
							}
							else
							{
								Game.LogError("Job: Unable to get goose chace phone interactable " + sideJob3.gooseChasePhone.ToString() + " from job " + sideJob3.jobID.ToString(), 2);
							}
						}
						else if (newType == "meet")
						{
							if (sideJob3.chosenMeetingPoint != null)
							{
								obj = sideJob3.chosenMeetingPoint;
							}
							else
							{
								Game.LogError("Job: Unable to get meeting point interactable " + sideJob3.meetingPoint.ToString() + " from job " + sideJob3.jobID.ToString(), 2);
							}
						}
						else if (newType == "calltime")
						{
							obj = sideJob3.gooseChaseCallTime;
						}
						else if (newType == "stolenitem")
						{
							if (sideJob3.activeJobItems.ContainsKey(JobPreset.JobTag.A))
							{
								if (Game.Instance.printDebug)
								{
									Game.Log("DDS: Returning item A from job " + sideJob3.jobID.ToString(), 2);
								}
								obj = sideJob3.activeJobItems[JobPreset.JobTag.A];
							}
							else if (Game.Instance.printDebug)
							{
								Game.Log("DDS: Unable to retrieve stolen item from sidejob " + sideJob3.jobID.ToString() + "! Cannot find item A within items " + sideJob3.activeJobItems.Count.ToString(), 2);
							}
						}
						else if (newType == "stolentimefrom")
						{
							SideJobStolenItem sideJobStolenItem = sideJob3 as SideJobStolenItem;
							if (sideJobStolenItem != null)
							{
								obj = sideJobStolenItem.theftTimeFrom;
							}
						}
						else if (newType == "stolentimeto")
						{
							SideJobStolenItem sideJobStolenItem2 = sideJob3 as SideJobStolenItem;
							if (sideJobStolenItem2 != null)
							{
								obj = sideJobStolenItem2.theftTimeTo;
							}
						}
						else if (newType == "submission")
						{
							if (sideJob3.thisCase != null && sideJob3.thisCase.handIn.Count > 0)
							{
								Interactable closestHandIn = sideJob3.thisCase.GetClosestHandIn();
								if (closestHandIn != null)
								{
									obj = closestHandIn.node.gameLocation;
								}
							}
						}
						else if (newType == "extraperson1")
						{
							obj = sideJob3.GetExtraPerson1();
						}
					}
				}
				else if (withinScope == "story")
				{
					if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
					{
						ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
						if (chapterIntro == null)
						{
							Game.LogError("Value error: Unable to convert input object to Chapter: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
							return obj;
						}
						if (chapterIntro != null)
						{
							if (newType == "associate")
							{
								obj = chapterIntro.killer;
							}
							else if (newType == "kidnapper")
							{
								obj = chapterIntro.kidnapper;
							}
							else if (newType == "notewriter")
							{
								obj = chapterIntro.noteWriter;
							}
							else if (newType == "restaurant")
							{
								obj = chapterIntro.restaurant;
							}
							else if (newType == "workplace")
							{
								obj = chapterIntro.kidnapper.job.employer.address;
							}
							else if (newType == "meettime")
							{
								obj = chapterIntro.meetTime;
							}
							else if (newType == "lockpicksneeded")
							{
								obj = Toolbox.Instance.GetLockpicksNeeded(chapterIntro.playersStorageBox.val);
							}
							else if (newType == "routeraddress")
							{
								obj = chapterIntro.chosenRouterAddress;
							}
							else if (newType == "bar")
							{
								obj = chapterIntro.killerBar;
							}
							else if (newType == "redgummeet")
							{
								obj = chapterIntro.killerBar;
							}
							else if (newType == "weaponsdealer")
							{
								obj = chapterIntro.weaponSeller.company;
							}
							else if (newType == "playersapartment")
							{
								obj = chapterIntro.apartment;
							}
							else if (newType == "flophouse")
							{
								obj = chapterIntro.slophouse;
							}
							else if (newType == "flophouseowner")
							{
								obj = chapterIntro.slophouseOwner;
							}
						}
					}
				}
				else if (withinScope == "evidence")
				{
					Evidence evidence2 = inputObject as Evidence;
					if (evidence2 != null)
					{
						if (newType == "object")
						{
							obj = evidence2.interactable;
						}
						else if (newType == "timefrom")
						{
							EvidenceTime evidenceTime = evidence2 as EvidenceTime;
							if (evidenceTime != null)
							{
								obj = evidenceTime.timeFrom;
							}
						}
						else if (newType == "timeto")
						{
							EvidenceTime evidenceTime2 = evidence2 as EvidenceTime;
							if (evidenceTime2 != null)
							{
								obj = evidenceTime2.timeTo;
							}
						}
						else if (newType == "writer")
						{
							obj = evidence2.writer;
						}
						else if (newType == "receiver")
						{
							obj = evidence2.reciever;
						}
						else if (newType == "belongsto")
						{
							obj = evidence2.belongsTo;
						}
						else if (newType == "telephonefrom")
						{
							EvidenceTelephoneCall evidenceTelephoneCall = evidence2 as EvidenceTelephoneCall;
							if (evidenceTelephoneCall != null && evidenceTelephoneCall.callFrom != null)
							{
								obj = evidenceTelephoneCall.callFrom.interactable;
							}
						}
						else if (newType == "telephoneto")
						{
							EvidenceTelephoneCall evidenceTelephoneCall2 = evidence2 as EvidenceTelephoneCall;
							if (evidenceTelephoneCall2 != null && evidenceTelephoneCall2.callTo != null)
							{
								obj = evidenceTelephoneCall2.callTo.interactable;
							}
						}
					}
				}
				else if (!(withinScope == "time"))
				{
					Game.LogError("Scope not found: " + withinScope, 2);
				}
			}
			IL_2C29:;
		}
		catch
		{
			Game.LogError("Error while getting scope object!", 2);
			return obj;
		}
		if (obj == null)
		{
			Game.LogError("GetScopeObject: Unable to retrieve scope " + newType + " within scope " + withinScope, 2);
		}
		return obj;
	}

	// Token: 0x06002644 RID: 9796 RVA: 0x001F0B7C File Offset: 0x001EED7C
	public static string GetContainedValue(object baseObject, string withinScope, string newValue, object inputObject, Evidence baseEvidence, Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic, List<Evidence.DataKey> evidenceKeys = null, object additionalObject = null, bool knowCitizenGender = false)
	{
		string text = string.Empty;
		withinScope = withinScope.ToLower();
		string text2 = newValue.ToLower();
		Strings.LinkData linkData = null;
		if (withinScope == "citizen")
		{
			Human human = inputObject as Human;
			if (human == null)
			{
				VMailApp.VmailParsingData vmailParsingData = inputObject as VMailApp.VmailParsingData;
				if (vmailParsingData == null)
				{
					Interactable interactable = inputObject as Interactable;
					if (interactable != null)
					{
						human = interactable.belongsTo;
					}
				}
				else
				{
					string text3;
					human = Strings.GetVmailSender(vmailParsingData.thread, vmailParsingData.messageIndex, out text3);
				}
				if (human == null)
				{
					Game.LogError("Value error: Unable to convert input object to Human: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
					return text;
				}
			}
			if (text2 == "fullname" || text2 == "name")
			{
				text = human.GetCitizenName();
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.name));
				}
			}
			else if (text2 == "firstname")
			{
				text = human.GetFirstName();
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.firstName));
				}
			}
			else if (text2 == "surname")
			{
				text = human.GetSurName();
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.surname));
				}
			}
			else if (text2 == "casualname")
			{
				text = human.GetCasualName();
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.firstName));
				}
			}
			else if (text2 == "formalname")
			{
				string text4 = string.Empty;
				if (human.gender == Human.Gender.male)
				{
					text4 = Strings.Get("descriptors", "Mr", Strings.Casing.asIs, false, false, false, null);
				}
				else if (human.gender == Human.Gender.female)
				{
					text4 = Strings.Get("descriptors", "Ms", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text4 = Strings.Get("descriptors", "Mx", Strings.Casing.asIs, false, false, false, null);
				}
				text = text4 + " " + human.GetInitialledName();
				if (Strings.loadedLanguage != null && Strings.loadedLanguage.swapCitizenTitleOrder)
				{
					text = human.GetInitialledName() + " " + text4;
				}
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.initialedName));
				}
			}
			else if (text2 == "signature")
			{
				string text5 = string.Empty;
				if (human.handwriting != null)
				{
					text5 = "<font=\"" + human.handwriting.fontAsset.name + "\">";
				}
				text = text5 + human.GetInitialledName() + "</font>";
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.initialedName));
				}
			}
			else if (text2 == "initial")
			{
				string firstName = human.GetFirstName();
				if (firstName != null && firstName.Length > 0)
				{
					text = firstName.Substring(0, 1);
				}
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.firstNameInitial));
				}
			}
			else if (text2 == "initials")
			{
				text = human.GetInitials();
				if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
				{
					linkData = Strings.AddOrGetLink(human.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(human.evidenceEntry, Evidence.DataKey.initials));
				}
			}
			else if (text2 == "gender")
			{
				text = Strings.Get("descriptors", human.gender.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "build")
			{
				text = Strings.Get("descriptors", human.descriptors.build.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "haircolour")
			{
				text = Strings.Get("descriptors", human.descriptors.hairColourCategory.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "hairtype")
			{
				text = Strings.Get("descriptors", human.descriptors.hairType.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "eyecolour")
			{
				text = Strings.Get("descriptors", human.descriptors.eyeColour.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "age")
			{
				int num = human.GetAge();
				text = num.ToString();
			}
			else if (text2 == "agegroup")
			{
				text = Strings.Get("descriptors", human.GetAgeGroup().ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "height")
			{
				text = Strings.Get("descriptors", human.descriptors.height.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "passcode")
			{
				List<int> digits = human.passcode.GetDigits();
				for (int i = 0; i < digits.Count; i++)
				{
					string text6 = text;
					int num = digits[i];
					text = text6 + num.ToString();
				}
				if (linkSetting == Strings.LinkSetting.forceLinks)
				{
					linkData = Strings.AddOrGetLink(digits);
				}
			}
			else if (text2 == "passcode1")
			{
				int num = human.passcode.GetDigit(0);
				text = num.ToString();
			}
			else if (text2 == "passcode2")
			{
				int num = human.passcode.GetDigit(1);
				text = num.ToString();
			}
			else if (text2 == "passcode3")
			{
				int num = human.passcode.GetDigit(2);
				text = num.ToString();
			}
			else if (text2 == "passcode4")
			{
				int num = human.passcode.GetDigit(3);
				text = num.ToString();
			}
			else if (text2 == "relnoun")
			{
				Human human5 = baseObject as Human;
				float num2 = 0f;
				if (human5 != null)
				{
					Acquaintance acquaintance = null;
					human5.FindAcquaintanceExists(human, out acquaintance);
					if (acquaintance != null)
					{
						if (acquaintance.connections.Contains(Acquaintance.ConnectionType.lover) || acquaintance.secretConnection == Acquaintance.ConnectionType.paramour)
						{
							text = Strings.GetTextForComponent("143675c5-4f0b-411b-8881-04df117c3c35", human5, human5, null, "\n", false, null, Strings.LinkSetting.automatic, null);
						}
						else if (acquaintance.connections.Contains(Acquaintance.ConnectionType.boss))
						{
							text = Strings.Get("misc", "boss", Strings.Casing.asIs, false, false, false, null);
						}
						else
						{
							num2 = acquaintance.known;
						}
					}
				}
				if (text == null || text.Length <= 0)
				{
					float num3 = Mathf.Clamp01((num2 + human5.slangUsage) * (human5.slangUsage * 2f));
					if (num3 <= 0.33f && human.gender == Human.Gender.male)
					{
						text = Strings.Get("misc", "sir", Strings.Casing.asIs, false, false, false, null);
					}
					else if (num3 <= 0.33f && human.gender == Human.Gender.female)
					{
						text = Strings.Get("misc", "ma'am", Strings.Casing.asIs, false, false, false, null);
					}
					else
					{
						text = Strings.GetTextForComponent("ace5b902-65ef-4bde-ae3c-62c788870304", human5, human5, null, "\n", false, null, Strings.LinkSetting.automatic, null);
					}
				}
			}
			else if (text2 == "noungeneral")
			{
				text = Strings.GetTextForComponent("ace5b902-65ef-4bde-ae3c-62c788870304", human, human, null, "\n", false, null, Strings.LinkSetting.automatic, null);
			}
			else if (text2 == "nounlover")
			{
				text = Strings.GetTextForComponent("143675c5-4f0b-411b-8881-04df117c3c35", human, human, null, "\n", false, null, Strings.LinkSetting.automatic, null);
			}
			else if (text2 == "curse")
			{
				text = Strings.GetTextForComponent("cbb38590-25bc-430d-89b8-03cf3ede9946", human, human, null, "\n", false, null, Strings.LinkSetting.automatic, null);
			}
			else if (text2 == "bloodtype")
			{
				text = Strings.Get("descriptors", human.bloodType.ToString(), Strings.Casing.asIs, false, false, knowCitizenGender, human);
			}
			else if (text2 == "heshe")
			{
				if (human.gender == Human.Gender.male)
				{
					text = Strings.Get("descriptors", "he", Strings.Casing.asIs, false, false, false, null);
				}
				else if (human.gender == Human.Gender.female)
				{
					text = Strings.Get("descriptors", "she", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text = Strings.Get("descriptors", "they", Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "himher")
			{
				if (human.gender == Human.Gender.male)
				{
					text = Strings.Get("descriptors", "him", Strings.Casing.asIs, false, false, false, null);
				}
				else if (human.gender == Human.Gender.female)
				{
					text = Strings.Get("descriptors", "her", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text = Strings.Get("descriptors", "them", Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "hishers")
			{
				if (human.gender == Human.Gender.male)
				{
					text = Strings.Get("descriptors", "his", Strings.Casing.asIs, false, false, false, null);
				}
				else if (human.gender == Human.Gender.female)
				{
					text = Strings.Get("descriptors", "hers", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text = Strings.Get("descriptors", "their", Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "birthgender")
			{
				text = Strings.Get("descriptors", human.birthGender.ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			else if (text2 == "dateofbirth")
			{
				text = human.birthday;
			}
			else if (text2 == "birthweight")
			{
				float psuedoRandomNumber = Toolbox.Instance.GetPsuedoRandomNumber(5f, 11f, human.citizenName + human.humanID.ToString(), false);
				text = Toolbox.Instance.RoundToPlaces(psuedoRandomNumber, 1).ToString() + Strings.Get("descriptors", "lb", Strings.Casing.lowerCase, false, false, false, null);
			}
			else if (text2 == "favitem1")
			{
				RetailItemPreset retailItemPreset = null;
				int num4 = -999999;
				RetailItemPreset c = null;
				int num5 = -999999;
				foreach (KeyValuePair<RetailItemPreset, int> keyValuePair in human.itemRanking)
				{
					if (retailItemPreset == null || keyValuePair.Value > num4)
					{
						retailItemPreset = keyValuePair.Key;
						num4 = keyValuePair.Value;
					}
					else if (c == null || keyValuePair.Value > num5)
					{
						c = keyValuePair.Key;
						num5 = keyValuePair.Value;
					}
				}
				if (retailItemPreset != null)
				{
					text = Strings.Get("evidence.names", retailItemPreset.itemPreset.name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "favitem2")
			{
				RetailItemPreset c2 = null;
				int num6 = -999999;
				RetailItemPreset retailItemPreset2 = null;
				int num7 = -999999;
				foreach (KeyValuePair<RetailItemPreset, int> keyValuePair2 in human.itemRanking)
				{
					if (c2 == null || keyValuePair2.Value > num6)
					{
						c2 = keyValuePair2.Key;
						num6 = keyValuePair2.Value;
					}
					else if (retailItemPreset2 == null || keyValuePair2.Value > num7)
					{
						retailItemPreset2 = keyValuePair2.Key;
						num7 = keyValuePair2.Value;
					}
				}
				if (retailItemPreset2 != null)
				{
					text = Strings.Get("evidence.names", retailItemPreset2.itemPreset.name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "acquaintance")
			{
				Human human2 = baseObject as Human;
				if (human2 != null)
				{
					Acquaintance acquaintance2 = null;
					human2.FindAcquaintanceExists(human, out acquaintance2);
					if (acquaintance2 != null)
					{
						text = Strings.Get("evidence.generic", acquaintance2.connections[0].ToString(), Strings.Casing.asIs, false, false, false, null);
					}
					else
					{
						text = Strings.Get("evidence.generic", "stranger", Strings.Casing.asIs, false, false, false, null);
					}
				}
			}
			else if (text2 == "namecipher")
			{
				if (Toolbox.Instance.GetPsuedoRandomNumber(0, 1, human.humanID.ToString() + human.citizenName, false) == 0 && human.GetFirstName().Length > 0 && human.GetSurName().Length > 0)
				{
					string text7 = human.GetFirstName();
					if (text7 != null && text7.Length > 0)
					{
						text7 = text7.Substring(0, 1);
					}
					text7 += human.GetSurName();
					string[] array = text7.Split(' ', 0);
					string text8 = string.Empty;
					for (int j = 0; j < array.Length; j++)
					{
						text8 += array[j];
					}
					int num8 = Mathf.Max(Mathf.CeilToInt((float)text8.Length * 0.33f), 1);
					List<int> list = new List<int>();
					List<char> list2 = new List<char>(text8);
					for (int k = 0; k < num8; k++)
					{
						Toolbox instance = Toolbox.Instance;
						int lowerRange = 0;
						int length = text8.Length;
						string text9 = human.humanID.ToString();
						string citizenName = human.citizenName;
						int num = list.Count;
						int num9 = instance.GetPsuedoRandomNumber(lowerRange, length, text9 + citizenName + num.ToString(), false);
						int num10 = 99;
						while (list.Contains(num9) && num10 > 0)
						{
							num9++;
							if (num9 >= text8.Length)
							{
								num9 = 0;
							}
							num10--;
						}
						if (!list.Contains(num9))
						{
							list.Add(num9);
							char c3 = text8.get_Chars(num9);
							list2.Remove(c3);
						}
					}
					string text10 = string.Empty;
					string text11 = string.Empty;
					for (int l = 0; l < text8.Length; l++)
					{
						if (l == 1)
						{
							text10 += ". ";
						}
						if (list.Contains(l))
						{
							text10 += text8.get_Chars(l).ToString();
						}
						else
						{
							text10 += "_ ";
							Toolbox instance2 = Toolbox.Instance;
							int lowerRange2 = 0;
							int count = list2.Count;
							string text12 = human.humanID.ToString();
							string citizenName2 = human.citizenName;
							int num = list2.Count;
							int psuedoRandomNumber2 = instance2.GetPsuedoRandomNumber(lowerRange2, count, text12 + citizenName2 + num.ToString(), false);
							int num11 = 99;
							while (psuedoRandomNumber2 == l && num11 > 0)
							{
								Toolbox instance3 = Toolbox.Instance;
								int lowerRange3 = 0;
								int count2 = list2.Count;
								string text13 = human.humanID.ToString();
								string citizenName3 = human.citizenName;
								num = list2.Count;
								psuedoRandomNumber2 = instance3.GetPsuedoRandomNumber(lowerRange3, count2, text13 + citizenName3 + num.ToString(), false);
								num11--;
							}
							char c4 = list2[psuedoRandomNumber2];
							text11 += c4.ToString();
							text11 += " ";
							list2.Remove(c4);
						}
					}
					text = text11.ToUpper() + "\n" + text10.ToUpper();
				}
			}
			else if (text2 == "killernamecipher")
			{
				if (Toolbox.Instance.GetPsuedoRandomNumber(0, 1, human.humanID.ToString() + human.citizenName, false) == 0 && human.GetFirstName().Length > 0 && human.GetSurName().Length > 0)
				{
					string text14 = human.GetFirstName();
					if (text14 != null && text14.Length > 0)
					{
						text14 = text14.Substring(0, 1);
					}
					text14 += human.GetSurName();
					string[] array2 = text14.Split(' ', 0);
					string text15 = string.Empty;
					for (int m2 = 0; m2 < array2.Length; m2++)
					{
						text15 += array2[m2];
					}
					List<MurderController.Murder> list3 = new List<MurderController.Murder>();
					List<MurderController.Murder> list4 = new List<MurderController.Murder>();
					if (baseObject != null)
					{
						Interactable objParent = baseObject as Interactable;
						if (objParent != null)
						{
							Game.Log("Killer name cipher: Found base object interactable", 2);
							if (objParent.murderParent != null)
							{
								Game.Log("Killer name cipher: Found murder parent on object", 2);
								if (objParent.murderParent.time > 0f)
								{
									list3 = MurderController.Instance.activeMurders.FindAll((MurderController.Murder item) => item.murdererID == human.humanID && item.time < objParent.murderParent.time);
									list4 = MurderController.Instance.inactiveMurders.FindAll((MurderController.Murder item) => item.murdererID == human.humanID && item.time < objParent.murderParent.time);
									string text16 = "Killer name cipher: Found valid murder time & ";
									int num = list3.Count + list4.Count;
									Game.Log(text16 + num.ToString() + " murders that happened prior to this one", 2);
								}
							}
						}
					}
					int num12 = Mathf.Clamp(list3.Count + list4.Count, 0, text15.Length - 1);
					List<int> list5 = new List<int>();
					List<char> list6 = new List<char>(text15);
					for (int n = 0; n < num12; n++)
					{
						Toolbox instance4 = Toolbox.Instance;
						int lowerRange4 = 0;
						int length2 = text15.Length;
						string text17 = human.humanID.ToString();
						string citizenName4 = human.citizenName;
						int num = list5.Count;
						int num13 = instance4.GetPsuedoRandomNumber(lowerRange4, length2, text17 + citizenName4 + num.ToString(), false);
						int num14 = 99;
						while (list5.Contains(num13) && num14 > 0)
						{
							num13++;
							if (num13 >= text15.Length)
							{
								num13 = 0;
							}
							num14--;
						}
						if (!list5.Contains(num13))
						{
							list5.Add(num13);
							char c5 = text15.get_Chars(num13);
							list6.Remove(c5);
						}
					}
					string text18 = string.Empty;
					string text19 = string.Empty;
					for (int num15 = 0; num15 < text15.Length; num15++)
					{
						if (num15 == 1)
						{
							text18 += ". ";
						}
						if (list5.Contains(num15))
						{
							text18 += text15.get_Chars(num15).ToString();
						}
						else
						{
							text18 += "_ ";
							Toolbox instance5 = Toolbox.Instance;
							int lowerRange5 = 0;
							int count3 = list6.Count;
							string text20 = human.humanID.ToString();
							string citizenName5 = human.citizenName;
							int num = list6.Count;
							int psuedoRandomNumber3 = instance5.GetPsuedoRandomNumber(lowerRange5, count3, text20 + citizenName5 + num.ToString(), false);
							int num16 = 99;
							while (psuedoRandomNumber3 == num15 && num16 > 0)
							{
								Toolbox instance6 = Toolbox.Instance;
								int lowerRange6 = 0;
								int count4 = list6.Count;
								string text21 = human.humanID.ToString();
								string citizenName6 = human.citizenName;
								num = list6.Count;
								psuedoRandomNumber3 = instance6.GetPsuedoRandomNumber(lowerRange6, count4, text21 + citizenName6 + num.ToString(), false);
								num16--;
							}
							char c6 = list6[psuedoRandomNumber3];
							text19 += c6.ToString();
							text19 += " ";
							list6.Remove(c6);
						}
					}
					text = text19.ToUpper() + "\n" + text18.ToUpper();
				}
			}
			else if (text2 == "investigateroom")
			{
				if (human.ai != null)
				{
					if (human.ai.investigateLocation != null)
					{
						text = Strings.Get("names.rooms", human.ai.investigateLocation.room.preset.name, Strings.Casing.lowerCase, false, false, false, null);
					}
					else if (human.currentRoom != null)
					{
						text = Strings.Get("names.rooms", human.currentRoom.preset.name, Strings.Casing.lowerCase, false, false, false, null);
					}
				}
			}
			else if (text2 == "investigateobject")
			{
				if (human.ai != null && human.ai.investigateObject != null)
				{
					text = Strings.Get("evidence.names", human.ai.investigateObject.preset.name, Strings.Casing.lowerCase, false, false, false, null);
				}
			}
			else if (text2 == "tamperedobject")
			{
				if (human.ai != null && human.ai.tamperedObject != null)
				{
					text = Strings.Get("evidence.names", human.ai.tamperedObject.preset.name, Strings.Casing.lowerCase, false, false, false, null);
				}
			}
			else if (text2 == "summary" && human.evidenceEntry != null)
			{
				text = human.evidenceEntry.GetSummary(evidenceKeys);
			}
			else if (text2 == "weightkg")
			{
				text = human.descriptors.weightKG.ToString();
			}
			else if (text2 == "heightcm")
			{
				text = human.descriptors.heightCM.ToString();
			}
			else if (text2 == "shoesize")
			{
				text = human.descriptors.shoeSize.ToString();
			}
			else if (text2 == "shoetype")
			{
				text = Strings.Get("evidence.generic", human.descriptors.footwear.ToString(), Strings.Casing.lowerCase, false, false, knowCitizenGender, human);
			}
			else if (text2 == "shoetype")
			{
				text = human.GetBloodTypeString();
			}
			else if (text2 == "interest")
			{
				List<Human.Trait> list7 = human.characterTraits.FindAll((Human.Trait item) => item.trait.featureInInterestPool);
				if (list7.Count > 0)
				{
					Human.Trait trait = list7[Toolbox.Instance.GetPsuedoRandomNumber(0, list7.Count, CityData.Instance.seed + human.humanID.ToString() + human.citizenName, false)];
					text = Strings.Get("descriptors", trait.trait.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
				}
			}
			else if (text2 == "affliction")
			{
				List<Human.Trait> list8 = human.characterTraits.FindAll((Human.Trait item) => item.trait.featureInAfflictionPool);
				if (list8.Count > 0)
				{
					Human.Trait trait2 = list8[Toolbox.Instance.GetPsuedoRandomNumber(0, list8.Count, CityData.Instance.seed + human.humanID.ToString() + human.citizenName, false)];
					text = Strings.Get("descriptors", trait2.trait.name, Strings.Casing.firstLetterCaptial, false, false, false, null);
				}
			}
			else if (text2 == "sightingdirection")
			{
				Human human3 = null;
				if (additionalObject != null && additionalObject as Human != null)
				{
					human3 = (additionalObject as Human);
				}
				else if (human.inConversation && human.currentConversation != null)
				{
					human3 = human.currentConversation.currentlyTalkingTo;
				}
				if (human3 != null && human.lastSightings.ContainsKey(human3))
				{
					NewGameLocation newGameLocation = null;
					Vector2 sightingDirection = human.GetSightingDirection(human.lastSightings[human3], out newGameLocation);
					if (newGameLocation != null)
					{
						text = newGameLocation.name;
					}
					else if (sightingDirection.x < 0f)
					{
						text = Strings.Get("misc", "west", Strings.Casing.asIs, false, false, false, null);
					}
					else if (sightingDirection.x > 0f)
					{
						text = Strings.Get("misc", "east", Strings.Casing.asIs, false, false, false, null);
					}
					else if (sightingDirection.y < 0f)
					{
						text = Strings.Get("misc", "south", Strings.Casing.asIs, false, false, false, null);
					}
					else if (sightingDirection.y > 0f)
					{
						text = Strings.Get("misc", "north", Strings.Casing.asIs, false, false, false, null);
					}
				}
			}
			else if (text2 == "hotelroombuilding")
			{
				GameplayController.HotelGuest hotelRoom = Toolbox.Instance.GetHotelRoom(human);
				if (hotelRoom != null)
				{
					NewAddress address = hotelRoom.GetAddress();
					if (address != null && address.building != null)
					{
						text = address.building.name;
					}
				}
			}
			else if (text2 == "hotelroomnumber")
			{
				GameplayController.HotelGuest hotelRoom2 = Toolbox.Instance.GetHotelRoom(human);
				if (hotelRoom2 != null)
				{
					NewAddress address2 = hotelRoom2.GetAddress();
					if (address2 != null && address2.residence != null)
					{
						text = address2.residence.GetResidenceString();
					}
				}
			}
			else if (text2 == "hotelroomcost")
			{
				GameplayController.HotelGuest hotelRoom3 = Toolbox.Instance.GetHotelRoom(human);
				if (hotelRoom3 != null && hotelRoom3.GetAddress() != null)
				{
					text = hotelRoom3.roomCost.ToString();
				}
			}
			else if (text2 == "hotelbill")
			{
				GameplayController.HotelGuest hotelRoom4 = Toolbox.Instance.GetHotelRoom(human);
				if (hotelRoom4 != null)
				{
					hotelRoom4.bill.ToString();
				}
			}
		}
		else if (withinScope == "city")
		{
			if (text2 == "name")
			{
				text = CityData.Instance.cityName;
			}
			else if (text2 == "population")
			{
				int num = CityData.Instance.citizenDirectory.Count;
				text = num.ToString();
			}
			else if (text2 == "unemployment")
			{
				int num = Mathf.RoundToInt((float)CityData.Instance.unemployedDirectory.Count / (float)CityData.Instance.citizenDirectory.Count);
				text = num.ToString();
			}
			else if (text2 == "homeless")
			{
				int num = Mathf.RoundToInt((float)CityData.Instance.homelessDirectory.Count / (float)CityData.Instance.citizenDirectory.Count);
				text = num.ToString();
			}
			else if (text2 == "currency")
			{
				text = CityControls.Instance.cityCurrency;
			}
			else if (text2 == "hotelcostupper")
			{
				text = CityControls.Instance.hotelCostUpper.ToString();
			}
			else if (text2 == "hotelcostlower")
			{
				text = CityControls.Instance.hotelCostLower.ToString();
			}
		}
		else if (withinScope == "company")
		{
			Company company = inputObject as Company;
			if (company == null)
			{
				Game.LogError("Value error: Unable to convert input object to Company: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (text2 == "name")
			{
				text = company.name;
				if (linkSetting == Strings.LinkSetting.forceLinks && company.address != null)
				{
					linkData = Strings.AddOrGetLink(company.address.evidenceEntry, null);
				}
			}
			else if (text2 == "openhours")
			{
				if (company.retailOpenHours.x == 0f && company.retailOpenHours.y == 24f)
				{
					text += Strings.Get("evidence.generic", "24 hours", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text = text + SessionData.Instance.DecimalToClockString(company.retailOpenHours.x, false) + " - " + SessionData.Instance.DecimalToClockString(company.retailOpenHours.y, false);
				}
			}
			else if (text2 == "opendays")
			{
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
					for (int num17 = 0; num17 < company.daysClosed.Count; num17++)
					{
						text += Strings.Get("ui.interface", company.daysClosed[num17].ToString(), Strings.Casing.asIs, false, false, false, null);
						if (num17 < company.daysClosed.Count - 1)
						{
							text += "& ";
						}
					}
				}
				else
				{
					for (int num18 = 0; num18 < company.daysClosed.Count; num18++)
					{
						text += Strings.Get("ui.interface", company.daysClosed[num18].ToString(), Strings.Casing.asIs, false, false, false, null);
						if (num18 < company.daysClosed.Count - 1)
						{
							text += ", ";
						}
					}
				}
			}
			else if (text2 == "item1")
			{
				List<InteractablePreset> list9 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list9.Count > 0)
				{
					text = Strings.Get("evidence.names", list9[0].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item2")
			{
				List<InteractablePreset> list10 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list10.Count > 1)
				{
					text = Strings.Get("evidence.names", list10[1].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item3")
			{
				List<InteractablePreset> list11 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list11.Count > 2)
				{
					text = Strings.Get("evidence.names", list11[2].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item4")
			{
				List<InteractablePreset> list12 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list12.Count > 3)
				{
					text = Strings.Get("evidence.names", list12[3].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item5")
			{
				List<InteractablePreset> list13 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list13.Count > 4)
				{
					text = Strings.Get("evidence.names", list13[4].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item6")
			{
				List<InteractablePreset> list14 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list14.Count > 5)
				{
					text = Strings.Get("evidence.names", list14[5].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item7")
			{
				List<InteractablePreset> list15 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list15.Count > 6)
				{
					text = Strings.Get("evidence.names", list15[6].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item8")
			{
				List<InteractablePreset> list16 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list16.Count > 7)
				{
					text = Strings.Get("evidence.names", list16[7].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item9")
			{
				List<InteractablePreset> list17 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list17.Count > 8)
				{
					text = Strings.Get("evidence.names", list17[8].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "item10")
			{
				List<InteractablePreset> list18 = Enumerable.ToList<InteractablePreset>(company.prices.Keys);
				if (list18.Count > 9)
				{
					text = Strings.Get("evidence.names", list18[9].name, Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "loansharkloan")
			{
				text = GameplayControls.Instance.defaultLoanAmount.ToString();
			}
			else if (text2 == "loansharkextra")
			{
				text = GameplayControls.Instance.defaultLoanExtra.ToString();
			}
			else if (text2 == "loansharkpayment")
			{
				GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
				if (loanDebt != null)
				{
					int num = loanDebt.GetRepaymentAmount();
					text = num.ToString();
				}
				else
				{
					text = GameplayControls.Instance.defaultLoanRepayment.ToString();
				}
			}
			else if (text2 == "loansharkdebt")
			{
				GameplayController.LoanDebt loanDebt2 = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
				if (loanDebt2 != null)
				{
					text = loanDebt2.debt.ToString();
				}
				else
				{
					text = "0";
				}
			}
			else if (text2 == "loansharknextdue")
			{
				GameplayController.LoanDebt loanDebt3 = GameplayController.Instance.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
				if (loanDebt3 != null)
				{
					int num19 = 0;
					int num;
					float num20;
					int num21;
					int num22;
					SessionData.Instance.ParseTimeData(loanDebt3.nextPaymentDueBy, out num20, out num19, out num, out num21, out num22);
					num19--;
					if (num19 < 0)
					{
						num19 = 6;
					}
					string dictionary = "ui.interface";
					SessionData.WeekDay weekDay = (SessionData.WeekDay)num19;
					text = Strings.Get(dictionary, weekDay.ToString(), Strings.Casing.asIs, false, false, false, null);
				}
			}
		}
		else if (withinScope == "controls")
		{
			string[] array3 = text2.Split('_', 0);
			bool flag = false;
			if (array3.Length > 1)
			{
				flag = true;
			}
			string text22 = array3[0];
			if (flag)
			{
				ActionElementMap firstElementMapWithAction = InputController.Instance.player.controllers.maps.GetFirstElementMapWithAction(text22, true);
				if (firstElementMapWithAction != null)
				{
					string text23 = firstElementMapWithAction.elementIdentifierName;
					string text24 = string.Empty;
					string text25;
					if (firstElementMapWithAction.controllerMap.controllerType == null)
					{
						text25 = "desktop";
						int num22;
						if (text23.Length <= 1)
						{
							text23 = "Keyboard Key";
							text24 = firstElementMapWithAction.elementIdentifierName.ToUpper() + "  ";
						}
						else if (text23.Length <= 3 && text23.Substring(0, 1) == "F" && int.TryParse(text23.Substring(1, text23.Length - 1), ref num22))
						{
							text23 = "Keyboard Key";
							text24 = firstElementMapWithAction.elementIdentifierName.ToUpper() + "  ";
						}
					}
					else if (firstElementMapWithAction.controllerMap.controllerType == 1)
					{
						text25 = "desktop";
					}
					else
					{
						text25 = "controller";
						if (text23 == "Left Stick" || text23 == "Right Stick")
						{
							text23 += " Button";
						}
						if (text23 == "Left Stick X" || text23 == "Left Stick Y")
						{
							text23 = "Left Stick";
						}
						else if (text23 == "Right Stick X" || text23 == "Right Stick Y")
						{
							text23 = "Right Stick";
						}
						if (firstElementMapWithAction.controllerMap.controller.name == "Pro Controller" && (text23 == "X" || text23 == "Y" || text23 == "A" || text23 == "B"))
						{
							text23 += "_Nintendo";
						}
					}
					text = string.Concat(new string[]
					{
						"<sprite=\"",
						text25,
						"\" name=\"",
						text23,
						"\">",
						text24
					});
				}
				else
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Unable to find control icon " + text22, 2);
					}
					text = string.Empty;
				}
			}
			else
			{
				ActionElementMap firstElementMapWithAction2 = InputController.Instance.player.controllers.maps.GetFirstElementMapWithAction(text22, true);
				if (firstElementMapWithAction2 != null)
				{
					text = firstElementMapWithAction2.elementIdentifierName.ToUpper();
				}
				else
				{
					if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Unable to find control icon " + text22, 2);
					}
					text = string.Empty;
				}
			}
		}
		else if (withinScope == "group")
		{
			GroupsController.SocialGroup socialGroup = inputObject as GroupsController.SocialGroup;
			if (socialGroup == null)
			{
				Game.LogError("Value error: Unable to convert input object to Group: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (text2 == "name")
			{
				text = Strings.Get("misc", socialGroup.preset, Strings.Casing.asIs, false, false, false, null);
			}
			else if (text2 == "time")
			{
				text = SessionData.Instance.DecimalToClockString(socialGroup.decimalStartTime, false);
			}
			else if (text2 == "days")
			{
				for (int num23 = 0; num23 < socialGroup.weekDays.Count; num23++)
				{
					text += Strings.Get("ui.interface", socialGroup.weekDays[num23].ToString(), Strings.Casing.asIs, false, false, false, null);
					if (num23 < socialGroup.weekDays.Count - 1)
					{
						text += ", ";
					}
				}
			}
		}
		else if (withinScope == "job")
		{
			Occupation occupation = inputObject as Occupation;
			if (occupation == null)
			{
				Game.LogError("Value error: Unable to convert input object to Job: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (text2 == "title")
			{
				text = Strings.Get("jobs", occupation.preset.name, Strings.Casing.asIs, false, false, knowCitizenGender, occupation.employee);
			}
			else if (text2 == "salary")
			{
				string cityCurrency = CityControls.Instance.cityCurrency;
				int num22 = Mathf.RoundToInt(occupation.salary * 1000f);
				text = cityCurrency + num22.ToString();
			}
			else if (text2 == "hours")
			{
				text = occupation.GetWorkingHoursString();
			}
			else if (text2 == "start")
			{
				text = SessionData.Instance.DecimalToClockString(occupation.startTimeDecimalHour, false);
			}
			else if (text2 == "end")
			{
				text = SessionData.Instance.DecimalToClockString(occupation.startTimeDecimalHour + occupation.workHours, false);
			}
		}
		else if (withinScope == "killer")
		{
			if (text2 == "moniker")
			{
				text = Strings.Get("misc", "static_killer_ref_1", Strings.Casing.asIs, false, false, false, null);
				if (MurderController.Instance.currentMurderer != null)
				{
					MurderController.Murder murder = MurderController.Instance.activeMurders.Find((MurderController.Murder item) => item.murderer == MurderController.Instance.currentMurderer);
					if (murder == null)
					{
						murder = MurderController.Instance.inactiveMurders.Find((MurderController.Murder item) => item.murderer == MurderController.Instance.currentMurderer);
					}
					if (murder != null)
					{
						text = murder.GetMonkier();
					}
				}
			}
			else if (text2 == "cleanupreward")
			{
				text = GameplayControls.Instance.coverUpReward.ToString();
			}
		}
		else if (withinScope == "location")
		{
			NewGameLocation newGameLocation2 = inputObject as NewGameLocation;
			if (newGameLocation2 == null)
			{
				Game.LogError("Value error: Unable to convert input object to Location: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (newGameLocation2 != null)
			{
				NewAddress thisAsAddress = newGameLocation2.thisAsAddress;
				StreetController thisAsStreet = newGameLocation2.thisAsStreet;
				if (text2 == "name")
				{
					text = newGameLocation2.name;
					if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
					{
						linkData = Strings.AddOrGetLink(newGameLocation2.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(newGameLocation2.evidenceEntry, Evidence.DataKey.name));
					}
				}
				else if (text2 == "building")
				{
					if (thisAsAddress != null)
					{
						text = thisAsAddress.building.name;
						if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
						{
							linkData = Strings.AddOrGetLink(newGameLocation2.building.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(newGameLocation2.building.evidenceEntry, Evidence.DataKey.name));
						}
					}
					else if (thisAsStreet != null)
					{
						text = " ";
					}
				}
				else if (text2 == "district")
				{
					if (newGameLocation2.district != null)
					{
						text = newGameLocation2.district.name;
					}
				}
				else if (text2 == "street")
				{
					if (thisAsStreet != null)
					{
						text = thisAsStreet.name;
						if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
						{
							linkData = Strings.AddOrGetLink(thisAsStreet.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(thisAsStreet.evidenceEntry, Evidence.DataKey.name));
						}
					}
					else if (thisAsAddress != null && thisAsAddress.building != null && thisAsAddress.building.street != null)
					{
						text = thisAsAddress.building.street.name;
						if (linkSetting == Strings.LinkSetting.forceLinks && baseEvidence != null)
						{
							linkData = Strings.AddOrGetLink(thisAsAddress.building.street.evidenceEntry, baseEvidence.GetMergedDiscoveryLinkKeysFor(thisAsAddress.building.street.evidenceEntry, Evidence.DataKey.name));
						}
					}
				}
				else if (text2 == "passcode")
				{
					if (thisAsAddress != null && thisAsAddress.passcode != null)
					{
						List<int> digits2 = thisAsAddress.passcode.GetDigits();
						for (int num24 = 0; num24 < digits2.Count; num24++)
						{
							string text26 = text;
							int num22 = digits2[num24];
							text = text26 + num22.ToString();
						}
						if (linkSetting == Strings.LinkSetting.forceLinks)
						{
							linkData = Strings.AddOrGetLink(digits2);
						}
					}
				}
				else if (text2 == "telephone")
				{
					if (newGameLocation2.telephones.Count > 0)
					{
						text = newGameLocation2.telephones[0].numberString;
						if (linkSetting == Strings.LinkSetting.forceLinks)
						{
							linkData = Strings.AddOrGetLink(newGameLocation2.telephones[0]);
						}
					}
				}
				else if (text2 == "type")
				{
					if (thisAsAddress != null)
					{
						text = Strings.Get("names.rooms", thisAsAddress.addressPreset.name, Strings.Casing.asIs, false, false, false, null);
					}
					else
					{
						text = Strings.Get("names.rooms", "street", Strings.Casing.asIs, false, false, false, null);
					}
				}
				else if (text2 == "price")
				{
					string cityCurrency2 = CityControls.Instance.cityCurrency;
					int num22 = newGameLocation2.GetPrice(false);
					text = cityCurrency2 + num22.ToString();
				}
				else if (text2 == "bedrooms")
				{
					if (thisAsAddress != null)
					{
						List<NewRoom> list19 = thisAsAddress.rooms.FindAll((NewRoom item) => item.preset.roomType == InteriorControls.Instance.bedroomType);
						if (list19.Count <= 0)
						{
							text = Strings.Get("names.rooms", "Studio", Strings.Casing.asIs, false, false, false, null);
						}
						else
						{
							string text27 = Strings.Get("names.rooms", "Bedrooms", Strings.Casing.asIs, false, false, false, null);
							string text28 = ": ";
							int num22 = list19.Count;
							text = text27 + text28 + num22.ToString();
						}
					}
				}
				else if (text2 == "floor")
				{
					if (thisAsAddress != null && thisAsAddress.floor != null)
					{
						text = Strings.Get("names.rooms", "floor_" + thisAsAddress.floor.floor.ToString(), Strings.Casing.asIs, false, false, false, null);
					}
				}
				else if (text2 == "sqm")
				{
					int num22 = newGameLocation2.GetSQM(false);
					text = num22.ToString();
				}
				else if (text2 == "password")
				{
					if (newGameLocation2.thisAsAddress != null)
					{
						text = newGameLocation2.thisAsAddress.GetPassword();
					}
					else
					{
						text = string.Empty;
					}
				}
			}
		}
		else if (withinScope == "murder")
		{
			MurderController.Murder m = inputObject as MurderController.Murder;
			if (text2 == "moniker")
			{
				if (m != null)
				{
					text = m.GetMonkier();
				}
			}
			else if (text2 == "method")
			{
				if (m != null && m.weaponPreset != null)
				{
					MurderController.MurderMethod murderMethod = MurderController.Instance.methodTypes.Find((MurderController.MurderMethod item) => item.type == m.weaponPreset.weapon.type);
					if (murderMethod != null)
					{
						text = Strings.ComposeText(Strings.Get("dds.blocks", murderMethod.blockDDS, Strings.Casing.asIs, false, false, false, null), m, Strings.LinkSetting.forceNoLinks, null, null, false);
					}
				}
			}
			else if (text2 == "methodnoadverb")
			{
				if (m != null)
				{
					if (m.weaponPreset != null)
					{
						MurderController.MurderMethod murderMethod2 = MurderController.Instance.methodTypes.Find((MurderController.MurderMethod item) => item.type == m.weaponPreset.weapon.type);
						if (murderMethod2 != null)
						{
							text = Strings.ComposeText(Strings.Get("dds.blocks", murderMethod2.blockDDS, Strings.Casing.asIs, false, false, false, null).Replace("|adverb|", "").Trim(), m, Strings.LinkSetting.forceNoLinks, null, null, false);
						}
					}
					else if (Game.Instance.printDebug)
					{
						Game.Log("DDS: Unable to get murder weapon preset", 2);
					}
				}
				else if (Game.Instance.printDebug)
				{
					string text29 = "DDS: Unable to get murder class with input object ";
					Type type = inputObject.GetType();
					Game.Log(text29 + ((type != null) ? type.ToString() : null), 2);
				}
			}
			else if (text2 == "adverb")
			{
				if (m != null)
				{
					text = Strings.GetTextForComponent("7ab5765e-a2a6-4c08-a1de-370c26da9794", m, null, null, "\n", false, null, Strings.LinkSetting.automatic, null);
				}
			}
			else if (text2 == "message")
			{
				if (m != null)
				{
					text = m.graffitiMsg;
				}
			}
			else if (text2 == "ransom")
			{
				if (m != null)
				{
					int num22 = m.ransomAmount * 1000;
					text = num22.ToString();
				}
			}
			else if (text2 == "ransomlocation")
			{
				if (m != null)
				{
					text = m.ransomSite.name;
				}
			}
			else if (text2 == "kidnaphoursgiven")
			{
				if (m != null)
				{
					text = m.preset.kidnapperTimeUntilKill.ToString();
				}
			}
			else if (text2 == "ransomphone" && m != null)
			{
				text = m.fakeNumberStr;
			}
		}
		else if (withinScope == "object")
		{
			Interactable interactable2 = inputObject as Interactable;
			if (interactable2 != null)
			{
				if (text2 == "name")
				{
					text = interactable2.GetName().ToLower();
				}
				else if (text2 == "purchaseditems")
				{
					EvidenceReceipt evidenceReceipt = interactable2.evidence as EvidenceReceipt;
					if (evidenceReceipt != null)
					{
						for (int num25 = 0; num25 < evidenceReceipt.purchased.Count; num25++)
						{
							float input = (float)evidenceReceipt.soldHere.prices[evidenceReceipt.purchased[num25]];
							text = string.Concat(new string[]
							{
								text,
								"\n\n",
								Strings.Get("evidence.names", evidenceReceipt.purchased[num25].name, Strings.Casing.asIs, false, false, false, null),
								"<pos=70%>",
								CityControls.Instance.cityCurrency,
								Toolbox.Instance.AddZeros(Toolbox.Instance.RoundToPlaces(input, 2), 2)
							});
						}
					}
					else
					{
						Game.LogError("Value error: Trying to get purchased items from non-receipt evidence", 2);
					}
				}
				else if (text2 == "purchasedprice")
				{
					text = CityControls.Instance.cityCurrency;
				}
				else if (text2 == "telephone")
				{
					if (interactable2.t != null)
					{
						text = interactable2.t.numberString;
						if (linkSetting == Strings.LinkSetting.forceLinks)
						{
							linkData = Strings.AddOrGetLink(interactable2.t);
						}
					}
				}
				else if (text2 == "summary" && interactable2.evidence != null)
				{
					text = interactable2.evidence.GetSummary(evidenceKeys);
				}
				else if (text2 == "lostitemtype")
				{
					if (interactable2.pv != null)
					{
						Interactable.Passed passed5 = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemPreset);
						if (passed5 != null)
						{
							text = Strings.Get("evidence.names", passed5.str, Strings.Casing.asIs, false, false, false, null);
						}
					}
				}
				else if (text2 == "lostitembuilding")
				{
					if (interactable2.pv != null)
					{
						Interactable.Passed passed = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemBuilding);
						if (passed != null)
						{
							NewBuilding newBuilding = HighlanderSingleton<CityBuildings>.Instance.buildingDirectory.Find((NewBuilding item) => item.buildingID == (int)passed.value);
							if (newBuilding != null)
							{
								text = newBuilding.name;
							}
						}
					}
				}
				else if (text2 == "lostitemreward")
				{
					if (interactable2.pv != null)
					{
						Interactable.Passed passed2 = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemReward);
						if (passed2 != null)
						{
							int num22 = Mathf.RoundToInt(passed2.value);
							text = num22.ToString();
						}
					}
				}
				else if (text2 == "lostitemfloorx")
				{
					if (interactable2.pv != null)
					{
						Interactable.Passed passed3 = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemFloorX);
						if (passed3 != null)
						{
							int num22 = Mathf.RoundToInt(passed3.value);
							text = num22.ToString();
						}
					}
				}
				else if (text2 == "lostitemfloory" && interactable2.pv != null)
				{
					Interactable.Passed passed4 = interactable2.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.lostItemFloorY);
					if (passed4 != null)
					{
						int num22 = Mathf.RoundToInt(passed4.value);
						text = num22.ToString();
					}
				}
				if (text == null || text.Length <= 0)
				{
					return Strings.GetContainedValue(baseObject, "citizen", newValue, interactable2.writer, baseEvidence, linkSetting, evidenceKeys, additionalObject, knowCitizenGender);
				}
			}
			else
			{
				Evidence evidence = inputObject as Evidence;
				if (evidence == null || evidence.meta == null)
				{
					if (inputObject == null)
					{
						Game.LogError("Value error: Unable to convert input object to Interactable: Null", 2);
					}
					else
					{
						Game.LogError("Value error: Unable to convert input object to Interactable: " + inputObject.GetType().ToString(), 2);
					}
					return text;
				}
				if (text2 == "name")
				{
					text = Strings.Get("evidence.names", evidence.meta.preset, Strings.Casing.asIs, false, false, false, null);
				}
				else if (text2 == "purchaseditems")
				{
					EvidenceReceipt evidenceReceipt2 = evidence as EvidenceReceipt;
					if (evidenceReceipt2 != null)
					{
						for (int num26 = 0; num26 < evidenceReceipt2.purchased.Count; num26++)
						{
							float input2 = (float)evidenceReceipt2.soldHere.prices[evidenceReceipt2.purchased[num26]];
							text = string.Concat(new string[]
							{
								text,
								"\n\n",
								Strings.Get("evidence.names", evidenceReceipt2.purchased[num26].name, Strings.Casing.asIs, false, false, false, null),
								"<pos=70%>",
								CityControls.Instance.cityCurrency,
								Toolbox.Instance.AddZeros(Toolbox.Instance.RoundToPlaces(input2, 2), 2)
							});
						}
					}
					else
					{
						Game.LogError("Value error: Trying to get purchased items from non-receipt evidence", 2);
					}
				}
				else if (text2 == "purchasedprice")
				{
					text = CityControls.Instance.cityCurrency;
				}
				else if (text2 == "summary")
				{
					text = evidence.GetSummary(evidenceKeys);
				}
			}
		}
		else if (withinScope == "random")
		{
			string text30 = CityData.Instance.seed;
			Interactable interactable3 = inputObject as Interactable;
			if (interactable3 != null)
			{
				text30 += interactable3.id.ToString();
			}
			else
			{
				SideJob sideJob = inputObject as SideJob;
				if (sideJob != null)
				{
					text30 += sideJob.jobID.ToString();
				}
				else
				{
					Human human4 = inputObject as Human;
					if (human4 != null)
					{
						text30 += human4.humanID.ToString();
					}
					else if (inputObject != null)
					{
						string text31 = text30;
						int num22 = inputObject.GetHashCode();
						text30 = text31 + num22.ToString();
					}
				}
			}
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Random seed: " + text30, 2);
			}
			if (text2 == "fullname")
			{
				List<Descriptors.EthnicGroup> list20 = new List<Descriptors.EthnicGroup>();
				foreach (SocialStatistics.EthnicityFrequency ethnicityFrequency in SocialStatistics.Instance.ethnicityFrequencies)
				{
					for (int num27 = 0; num27 < ethnicityFrequency.frequency; num27++)
					{
						list20.Add(ethnicityFrequency.ethnicity);
					}
				}
				Descriptors.EthnicGroup ethnicGroup = list20[Toolbox.Instance.GetPsuedoRandomNumber(0, list20.Count, text30, false)];
				string text32 = "male";
				if (Toolbox.Instance.GetPsuedoRandomNumber(0f, 1f, text30, false) > 0.5f)
				{
					text32 = "female";
				}
				string text3;
				string text33;
				string text34;
				bool flag2;
				string text35;
				text = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup.ToString() + ".first." + text32, 1f, null, 0f, out text3, out text33, out text34, out flag2, out text35, text30);
			}
			else if (text2 == "casualname")
			{
				List<Descriptors.EthnicGroup> list21 = new List<Descriptors.EthnicGroup>();
				foreach (SocialStatistics.EthnicityFrequency ethnicityFrequency2 in SocialStatistics.Instance.ethnicityFrequencies)
				{
					for (int num28 = 0; num28 < ethnicityFrequency2.frequency; num28++)
					{
						list21.Add(ethnicityFrequency2.ethnicity);
					}
				}
				Descriptors.EthnicGroup ethnicGroup2 = list21[Toolbox.Instance.GetPsuedoRandomNumber(0, list21.Count, text30, false)];
				string text3;
				string text33;
				string text34;
				bool flag2;
				string text35;
				text = NameGenerator.Instance.GenerateName(null, 0f, "names." + ethnicGroup2.ToString() + ".sur", 1f, null, 0f, out text35, out text34, out text33, out flag2, out text3, text30);
			}
			else if (text2 == "number5")
			{
				int num22 = Toolbox.Instance.GetPsuedoRandomNumber(1, 6, text30, false);
				text = num22.ToString();
			}
			else if (text2 == "number10")
			{
				int num22 = Toolbox.Instance.GetPsuedoRandomNumber(1, 11, text30, false);
				text = num22.ToString();
			}
			else if (text2 == "number20")
			{
				int num22 = Toolbox.Instance.GetPsuedoRandomNumber(1, 21, text30, false);
				text = num22.ToString();
			}
			else if (text2 == "number50")
			{
				int num22 = Toolbox.Instance.GetPsuedoRandomNumber(1, 51, text30, false);
				text = num22.ToString();
			}
			else if (text2 == "number100")
			{
				int num22 = Toolbox.Instance.GetPsuedoRandomNumber(1, 101, text30, false);
				text = num22.ToString();
			}
		}
		else if (withinScope == "sidejob")
		{
			SideJob job = inputObject as SideJob;
			if (job == null)
			{
				Game.LogError("Value error: Unable to convert input object to Side Job: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (text2 == "reward")
			{
				if (job.rewardSyncDisk != null && job.rewardSyncDisk.Length > 0)
				{
					text = Strings.Get("missions.postings", "a rare sync disk", Strings.Casing.asIs, false, false, false, null) + "- " + Strings.Get("evidence.syncdisks", job.rewardSyncDisk, Strings.Casing.asIs, false, false, false, null);
					if (job.reward > 0)
					{
						text += " & ";
					}
				}
				if (job.reward > 0)
				{
					text = text + CityControls.Instance.cityCurrency + job.reward.ToString();
				}
			}
			else if (text2 == "type")
			{
				text = Strings.Get("missions.postings", job.presetStr, Strings.Casing.asIs, false, false, false, null);
			}
			else if (text2 == "motive")
			{
				text = Strings.Get("missions.postings", job.motiveStr, Strings.Casing.asIs, false, false, false, null);
			}
			else if (text2 == "fakenumber")
			{
				text = job.fakeNumberStr;
				List<int> list22 = new List<int>();
				for (int num29 = 0; num29 < job.fakeNumberStr.Length; num29++)
				{
					int num30 = 0;
					if (int.TryParse(job.fakeNumberStr.get_Chars(num29).ToString(), ref num30))
					{
						list22.Add(num30);
					}
				}
				if (linkSetting == Strings.LinkSetting.forceLinks)
				{
					linkData = Strings.AddOrGetLink(list22);
				}
			}
			else if (text2 == "revengeamount")
			{
				if (job.thisCase != null)
				{
					Case.ResolveQuestion resolveQuestion = job.thisCase.resolveQuestions.Find((Case.ResolveQuestion item) => item.inputType == Case.InputType.revengeObjective && item.revengeObjective != null && item.revengeObjective.Length > 0);
					if (resolveQuestion != null)
					{
						int num22 = Mathf.RoundToInt(resolveQuestion.revengeObjPassed);
						text = num22.ToString();
					}
					else if (Game.Instance.printDebug)
					{
						string text36 = "DDS: ";
						int num22 = job.resolveQuestions.Count;
						Game.Log(text36 + num22.ToString() + " questions exist...", 2);
					}
				}
			}
			else
			{
				if (text2 == "meetitem")
				{
					if (job == null)
					{
						goto IL_4A8E;
					}
					try
					{
						InteractablePreset interactablePreset = InteriorControls.Instance.meetupConsumables[job.meetingConsumableIndex];
						if (interactablePreset != null)
						{
							text = Strings.Get("evidence.names", interactablePreset.name, Strings.Casing.asIs, false, false, false, null);
						}
						goto IL_4A8E;
					}
					catch
					{
						goto IL_4A8E;
					}
				}
				if (text2 == "stolenitemroom")
				{
					if (job != null)
					{
						SideJobStolenItem sideJobStolenItem = job as SideJobStolenItem;
						if (sideJobStolenItem != null)
						{
							NewRoom newRoom = null;
							if (CityData.Instance.roomDictionary.TryGetValue(sideJobStolenItem.stolenItemRoom, ref newRoom))
							{
								text = Strings.Get("names.rooms", newRoom.preset.name, Strings.Casing.lowerCase, false, false, false, null);
							}
						}
					}
				}
				else if (text2 == "secretlocation" && job != null && job.secretLocationFurniture != 0)
				{
					NewNode newNode = null;
					if (PathFinder.Instance.nodeMap.TryGetValue(job.secretLocationNode, ref newNode))
					{
						FurnitureLocation furnitureLocation = newNode.room.individualFurniture.Find((FurnitureLocation item) => item.id == job.secretLocationFurniture);
						if (furnitureLocation != null)
						{
							if (Strings.loadedLanguage != null && Strings.loadedLanguage.systemLanguage != 10)
							{
								text = string.Concat(new string[]
								{
									Strings.Get("evidence.names", furnitureLocation.furniture.name, Strings.Casing.asIs, false, false, false, null),
									" (",
									Strings.Get("names.rooms", newNode.room.preset.name, Strings.Casing.lowerCase, false, false, false, null),
									", ",
									newNode.gameLocation.name,
									")"
								});
							}
							else
							{
								text = string.Concat(new string[]
								{
									Strings.Get("evidence.names", furnitureLocation.furniture.name, Strings.Casing.asIs, false, false, false, null),
									" ",
									Strings.Get("missions.postings", "in the", Strings.Casing.asIs, false, false, false, null),
									" ",
									Strings.Get("names.rooms", newNode.room.preset.name, Strings.Casing.lowerCase, false, false, false, null),
									", ",
									newNode.gameLocation.name
								});
							}
						}
					}
				}
			}
		}
		else if (withinScope == "story")
		{
			if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
			{
				ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
				if (chapterIntro != null)
				{
					if (text2 == "lockpicksneeded")
					{
						int num22 = chapterIntro.lockpicksNeeded - 2;
						text = num22.ToString();
					}
					else if (text2 == "partnerfirstname")
					{
						text = "Sam";
					}
					else if (text2 == "partnerfullname")
					{
						text = "Sam Merriweather";
					}
				}
			}
		}
		else if (withinScope == "time")
		{
			float num31 = Convert.ToSingle(inputObject);
			float num32 = 0f;
			int num33 = 0;
			int num34 = 0;
			int num35 = 0;
			int num36 = 0;
			SessionData.Instance.ParseTimeData(num31, out num32, out num33, out num34, out num35, out num36);
			if (text2 == "timeofday")
			{
				if (num32 > 3f && num32 < 12f)
				{
					text = Strings.Get("misc", "evening", Strings.Casing.asIs, false, false, false, null);
				}
				else if (num32 > 3f && num32 < 17f)
				{
					text = Strings.Get("misc", "afternoon", Strings.Casing.asIs, false, false, false, null);
				}
				else
				{
					text = Strings.Get("misc", "evening", Strings.Casing.asIs, false, false, false, null);
				}
			}
			else if (text2 == "time")
			{
				text = SessionData.Instance.DecimalToClockString(num32, false);
			}
			else if (text2 == "time24")
			{
				text = SessionData.Instance.DecimalToClockString(num32, false);
			}
			else if (text2 == "time12")
			{
				float formatted = SessionData.Instance.FloatMinutes12H(num32);
				string text37 = Strings.Get("ui.interface", "AM", Strings.Casing.asIs, false, false, false, null);
				if (num32 > 12f)
				{
					text37 = Strings.Get("ui.interface", "PM", Strings.Casing.asIs, false, false, false, null);
				}
				text = SessionData.Instance.MinutesToClockString(formatted, false) + text37;
			}
			else if (text2 == "time12rounded15")
			{
				num32 = (float)Mathf.RoundToInt(num32 * 100f / 25f) * 25f / 100f;
				float formatted2 = SessionData.Instance.FloatMinutes12H(num32);
				string text38 = Strings.Get("ui.interface", "AM", Strings.Casing.asIs, false, false, false, null);
				if (num32 > 12f)
				{
					text38 = Strings.Get("ui.interface", "PM", Strings.Casing.asIs, false, false, false, null);
				}
				text = SessionData.Instance.MinutesToClockString(formatted2, false) + text38;
			}
			else if (text2 == "day")
			{
				string dictionary2 = "ui.interface";
				SessionData.WeekDay weekDay = (SessionData.WeekDay)num33;
				text = Strings.Get(dictionary2, weekDay.ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			else if (text2 == "date")
			{
				text = SessionData.Instance.ShortDateString(num31, true);
			}
			else if (text2 == "datelong")
			{
				text = SessionData.Instance.LongDateString(num31, true, false, true, false, true, true, false, true);
			}
			else if (text2 == "year")
			{
				text = num36.ToString();
			}
		}
		else if (withinScope == "syncdisk")
		{
			SyncDiskPreset syncDiskPreset = inputObject as SyncDiskPreset;
			int num37 = 0;
			int num38 = 0;
			if (additionalObject != null)
			{
				int[] array4 = additionalObject as int[];
				if (array4 != null)
				{
					if (array4.Length != 0)
					{
						num37 = array4[0];
					}
					if (array4.Length > 1)
					{
						num38 = array4[1];
					}
				}
			}
			if (syncDiskPreset != null)
			{
				if (text2 == "value")
				{
					if (num37 == 0)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect1Value);
						text = num22.ToString();
					}
					else if (num37 == 1)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect2Value);
						text = num22.ToString();
					}
					else if (num37 == 2)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect3Value);
						text = num22.ToString();
					}
				}
				else if (text2 == "upgradevalue")
				{
					if (num37 == 0)
					{
						if (num38 == 0 && syncDiskPreset.option1UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[0]);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option1UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[1]);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option1UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[2]);
							text = num22.ToString();
						}
					}
					else if (num37 == 1)
					{
						if (num38 == 0 && syncDiskPreset.option2UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[0]);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option2UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[1]);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option2UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[2]);
							text = num22.ToString();
						}
					}
					else if (num37 == 2)
					{
						if (num38 == 0 && syncDiskPreset.option3UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[0]);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option3UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[1]);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option3UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[2]);
							text = num22.ToString();
						}
					}
				}
				else if (text2 == "value%")
				{
					if (num37 == 0)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect1Value * 100f);
						text = num22.ToString();
					}
					else if (num37 == 1)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect2Value * 100f);
						text = num22.ToString();
					}
					else if (num37 == 2)
					{
						int num22 = Mathf.RoundToInt(syncDiskPreset.mainEffect3Value * 100f);
						text = num22.ToString();
					}
				}
				else if (text2 == "upgradevalue%")
				{
					if (num37 == 0)
					{
						if (num38 == 0 && syncDiskPreset.option1UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[0] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option1UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[1] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option1UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option1UpgradeValues[2] * 100f);
							text = num22.ToString();
						}
					}
					else if (num37 == 1)
					{
						if (num38 == 0 && syncDiskPreset.option2UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[0] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option2UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[1] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option2UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option2UpgradeValues[2] * 100f);
							text = num22.ToString();
						}
					}
					else if (num37 == 2)
					{
						if (num38 == 0 && syncDiskPreset.option3UpgradeValues.Count > 0)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[0] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 1 && syncDiskPreset.option3UpgradeValues.Count > 1)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[1] * 100f);
							text = num22.ToString();
						}
						else if (num38 == 2 && syncDiskPreset.option3UpgradeValues.Count > 2)
						{
							int num22 = Mathf.RoundToInt(syncDiskPreset.option3UpgradeValues[2] * 100f);
							text = num22.ToString();
						}
					}
				}
				else if (text2 == "uninstallcost")
				{
					int num22 = Mathf.RoundToInt((float)syncDiskPreset.uninstallCost);
					text = num22.ToString();
				}
			}
		}
		else if (withinScope == "evidence")
		{
			Evidence evidence2 = inputObject as Evidence;
			if (evidence2 == null)
			{
				Game.LogError("Value error: Unable to convert input object to Evidence: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (text2 == "name")
			{
				text = evidence2.name;
			}
			else if (text2 == "note")
			{
				List<Evidence.DataKey> list23 = new List<Evidence.DataKey>();
				list23.Add(Evidence.DataKey.name);
				text = evidence2.GetNoteComposed(list23, true);
			}
			else if (text2 == "date")
			{
				EvidenceDate evidenceDate = evidence2 as EvidenceDate;
				if (evidenceDate != null)
				{
					text = evidenceDate.date;
				}
			}
			else if (text2 == "duration")
			{
				EvidenceTime evidenceTime = evidence2 as EvidenceTime;
				if (evidenceTime != null)
				{
					text = evidenceTime.duration;
				}
			}
		}
		else
		{
			Evidence evidence3 = inputObject as Evidence;
			if (evidence3 == null)
			{
				Game.LogError("Value error: Unable to convert input object to Evidence: " + ((inputObject != null) ? inputObject.ToString() : null), 2);
				return text;
			}
			if (evidence3 != null && text2 == "summary")
			{
				text = evidence3.GetSummary(evidenceKeys);
			}
			else
			{
				Game.LogError("Scope not found: '" + withinScope + "'", 2);
			}
		}
		IL_4A8E:
		if (text == null || text.Length <= 0)
		{
			Game.LogError("Unable to retrieve value '" + text2 + "' in scope: " + withinScope, 2);
		}
		else if (linkSetting == Strings.LinkSetting.forceLinks && linkData != null)
		{
			text = string.Concat(new string[]
			{
				"<link=",
				linkData.id.ToString(),
				">",
				text,
				"</link>"
			});
		}
		return text;
	}

	// Token: 0x06002645 RID: 9797 RVA: 0x001F56BC File Offset: 0x001F38BC
	public static MurderController.Murder GetPreviousMurder(float specificTime)
	{
		MurderController.Murder murder = null;
		if (MurderController.Instance.activeMurders.Count > 0)
		{
			for (int i = 0; i < MurderController.Instance.activeMurders.Count; i++)
			{
				MurderController.Murder murder2 = MurderController.Instance.activeMurders[i];
				if (murder2.creationTime < specificTime && murder2.death != null && murder2.death.time <= specificTime && (murder == null || murder2.death.time > murder.death.time))
				{
					murder = murder2;
				}
			}
		}
		if (MurderController.Instance.inactiveMurders.Count > 0)
		{
			for (int j = 0; j < MurderController.Instance.inactiveMurders.Count; j++)
			{
				MurderController.Murder murder3 = MurderController.Instance.inactiveMurders[j];
				if (murder3.creationTime < specificTime && murder3.death != null && murder3.death.time <= specificTime && (murder == null || murder3.death.time > murder.death.time))
				{
					murder = murder3;
				}
			}
		}
		return murder;
	}

	// Token: 0x06002646 RID: 9798 RVA: 0x001F57C4 File Offset: 0x001F39C4
	public static MurderController.Murder GetNextMurder(float specificTime)
	{
		MurderController.Murder murder = null;
		if (MurderController.Instance.activeMurders.Count > 0)
		{
			for (int i = 0; i < MurderController.Instance.activeMurders.Count; i++)
			{
				MurderController.Murder murder2 = MurderController.Instance.activeMurders[i];
				if (murder2.creationTime < specificTime && (murder == null || murder2.creationTime > murder.creationTime))
				{
					murder = murder2;
				}
			}
		}
		if (murder == null && MurderController.Instance.inactiveMurders.Count > 0)
		{
			for (int j = 0; j < MurderController.Instance.inactiveMurders.Count; j++)
			{
				MurderController.Murder murder3 = MurderController.Instance.inactiveMurders[j];
				if (murder3.creationTime < specificTime && (murder == null || murder3.creationTime > murder.creationTime))
				{
					murder = murder3;
				}
			}
		}
		return murder;
	}

	// Token: 0x06002647 RID: 9799 RVA: 0x001F5890 File Offset: 0x001F3A90
	public static Evidence GetEvidenceFromBaseScope(object baseObject)
	{
		Evidence result = null;
		Human human = baseObject as Human;
		VMailApp.VmailParsingData vmailParsingData = baseObject as VMailApp.VmailParsingData;
		Interactable interactable = baseObject as Interactable;
		NewGameLocation newGameLocation = baseObject as NewGameLocation;
		Evidence evidence = baseObject as Evidence;
		if (human != null)
		{
			result = human.evidenceEntry;
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Base evidence is 'human'...", 2);
			}
		}
		else if (vmailParsingData != null)
		{
			string text;
			Human vmailSender = Strings.GetVmailSender(vmailParsingData.thread, vmailParsingData.messageIndex, out text);
			if (vmailSender != null)
			{
				result = vmailSender.evidenceEntry;
			}
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Base evidence is 'human' (from vmail)...", 2);
			}
		}
		else if (interactable != null)
		{
			result = interactable.evidence;
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Base evidence is 'item'...", 2);
			}
		}
		else if (newGameLocation != null)
		{
			result = newGameLocation.evidenceEntry;
			if (Game.Instance.printDebug)
			{
				Game.Log("DDS: Base evidence is 'location'...", 2);
			}
		}
		else if (evidence != null)
		{
			if (evidence.meta != null)
			{
				result = evidence;
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Base evidence is 'item' (meta)...", 2);
				}
			}
			else
			{
				result = evidence;
				if (Game.Instance.printDebug)
				{
					Game.Log("DDS: Base evidence is 'evidence'...", 2);
				}
			}
		}
		else if (Game.Instance.printDebug)
		{
			Game.Log("DDS: Base evidence is 'null'...", 2);
		}
		return result;
	}

	// Token: 0x06002648 RID: 9800 RVA: 0x001F59F0 File Offset: 0x001F3BF0
	public static Strings.LinkData AddOrGetLink(Evidence newEvidence, List<Evidence.DataKey> overrideKeys = null)
	{
		List<Strings.LinkData> list = null;
		if (newEvidence == null)
		{
			Game.LogError("Trying to create link from null evidence!", 2);
			return null;
		}
		if (overrideKeys == null)
		{
			overrideKeys = new List<Evidence.DataKey>();
			overrideKeys.Add(Evidence.DataKey.name);
		}
		if (!Strings.Instance.evidenceLinkDictionary.TryGetValue(newEvidence, ref list) && list != null)
		{
			foreach (Strings.LinkData linkData in list)
			{
				if (Enumerable.SequenceEqual<Evidence.DataKey>(linkData.dataKeys, overrideKeys))
				{
					return linkData;
				}
			}
		}
		return new Strings.LinkData(newEvidence, overrideKeys);
	}

	// Token: 0x06002649 RID: 9801 RVA: 0x001F5A8C File Offset: 0x001F3C8C
	public static Strings.LinkData AddOrGetLink(Telephone newTelephone)
	{
		Strings.LinkData result = null;
		if (!Strings.Instance.linkDictionary.TryGetValue(newTelephone, ref result))
		{
			result = new Strings.LinkData(newTelephone);
		}
		return result;
	}

	// Token: 0x0600264A RID: 9802 RVA: 0x001F5AB8 File Offset: 0x001F3CB8
	public static Strings.LinkData AddOrGetLink(List<int> newInputCode)
	{
		foreach (KeyValuePair<object, Strings.LinkData> keyValuePair in Strings.Instance.linkDictionary)
		{
			List<int> list = keyValuePair.Key as List<int>;
			if (list != null && Enumerable.SequenceEqual<int>(list, newInputCode))
			{
				return keyValuePair.Value;
			}
		}
		return new Strings.LinkData(newInputCode);
	}

	// Token: 0x0600264B RID: 9803 RVA: 0x001F5B34 File Offset: 0x001F3D34
	public static string GetMainTextFromInteractable(Interactable interactable, Strings.LinkSetting linkSetting = Strings.LinkSetting.automatic)
	{
		string result = string.Empty;
		if (interactable.evidence != null)
		{
			string text = interactable.evidence.preset.ddsDocumentID;
			if (interactable.evidence.overrideDDS != null && interactable.evidence.overrideDDS.Length > 0)
			{
				text = interactable.evidence.overrideDDS;
			}
			DDSSaveClasses.DDSTreeSave ddstreeSave;
			if (!Toolbox.Instance.allDDSTrees.TryGetValue(text, ref ddstreeSave))
			{
				Game.Log(string.Concat(new string[]
				{
					"Misc Error: Cannot find content for  tree ",
					text,
					" (",
					interactable.evidence.preset.name,
					"). Original doc ID: ",
					interactable.evidence.preset.ddsDocumentID
				}), 2);
				return result;
			}
			int num = 0;
			foreach (DDSSaveClasses.DDSMessageSettings ddsmessageSettings in ddstreeSave.messages)
			{
				if (ddsmessageSettings.msgID != null && ddsmessageSettings.msgID.Length > 0 && ddsmessageSettings.usePages)
				{
					object obj = interactable.evidence;
					if (interactable != null)
					{
						obj = interactable;
					}
					result = Strings.GetTextForComponent(ddsmessageSettings.msgID, obj, interactable.evidence.writer, interactable.evidence.reciever, "\n", false, null, linkSetting, null);
					num++;
					break;
				}
			}
			if (num > 0)
			{
				return result;
			}
			using (List<DDSSaveClasses.DDSMessageSettings>.Enumerator enumerator = ddstreeSave.messages.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DDSSaveClasses.DDSMessageSettings ddsmessageSettings2 = enumerator.Current;
					if (ddsmessageSettings2.msgID != null && ddsmessageSettings2.msgID.Length > 0)
					{
						object obj2 = interactable.evidence;
						if (interactable != null)
						{
							obj2 = interactable;
						}
						result = Strings.GetTextForComponent(ddsmessageSettings2.msgID, obj2, interactable.evidence.writer, interactable.evidence.reciever, "\n", false, null, linkSetting, null);
						num++;
						break;
					}
				}
				return result;
			}
		}
		Game.Log("Misc Error: Interactable " + interactable.name + " has no evidence, cannot get the main text...", 2);
		return result;
	}

	// Token: 0x0600264C RID: 9804 RVA: 0x001F5D58 File Offset: 0x001F3F58
	public static string FilterInputtedText(string input, bool useCensor = true, int maxCharacters = 100)
	{
		input = Strings.RemoveCharacters(input, true, false, true, false);
		if (input.Length > 100)
		{
			input = input.Substring(0, maxCharacters);
		}
		if (input.Length < 1)
		{
			return "null";
		}
		return input.Trim();
	}

	// Token: 0x0600264D RID: 9805 RVA: 0x001F5D90 File Offset: 0x001F3F90
	public static string RemoveCharacters(string input, bool removeSpecialCharacters, bool removeNumbers, bool removeDots, bool removeSpaces)
	{
		string text = input;
		if (removeSpecialCharacters)
		{
			text = new string(Array.FindAll<char>(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || c == '-' || c == '.'));
		}
		if (removeNumbers)
		{
			text = Regex.Replace(text, "[\\d-]", string.Empty, 8);
		}
		if (removeDots)
		{
			text = Regex.Replace(text, "\\.+", string.Empty, 8);
		}
		if (removeSpaces)
		{
			text = Regex.Replace(text, "\\s+", string.Empty, 8);
		}
		return text;
	}

	// Token: 0x0600264E RID: 9806 RVA: 0x001F5E14 File Offset: 0x001F4014
	[Button(null, 0)]
	public void FindBlockInMessages()
	{
		if (this.findBlock != null && this.findBlock.Length > 0)
		{
			foreach (KeyValuePair<string, DDSSaveClasses.DDSMessageSave> keyValuePair in Toolbox.Instance.allDDSMessages)
			{
				if (keyValuePair.Value.blocks.Exists((DDSSaveClasses.DDSBlockCondition item) => item.blockID == this.findBlock))
				{
					Game.Log(string.Concat(new string[]
					{
						"Block ",
						this.findBlock,
						" exists in message: ",
						keyValuePair.Value.name,
						" (",
						keyValuePair.Value.id,
						"). Other blocks in this message include: "
					}), 2);
					foreach (DDSSaveClasses.DDSBlockCondition ddsblockCondition in keyValuePair.Value.blocks)
					{
						Game.Log("..." + ddsblockCondition.blockID, 2);
					}
				}
			}
		}
	}

	// Token: 0x0600264F RID: 9807 RVA: 0x001F5F58 File Offset: 0x001F4158
	[Button(null, 0)]
	public void OutputAllCharacters()
	{
		HashSet<char> hashSet = new HashSet<char>();
		HashSet<char> hashSet2 = new HashSet<char>();
		HashSet<char> hashSet3 = new HashSet<char>();
		string text = Application.streamingAssetsPath + "/Strings/Localization/UniqueCharacters_ALL.txt";
		Application.streamingAssetsPath + "/Strings/Localization/UniqueCharacters_LATIN.txt";
		string text2 = Application.streamingAssetsPath + "/Strings/Localization/UniqueCharacters_NON-LATIN.txt";
		string text6;
		foreach (LanguageConfigLoader.LocInput locInput in LanguageConfigLoader.Instance.fileInputConfig)
		{
			foreach (FileInfo fileInfo in Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Strings/" + locInput.languageCode + "/").GetFiles("*.csv", 1)))
			{
				using (StreamReader streamReader = File.OpenText(fileInfo.FullName))
				{
					int num = 0;
					for (int i = 0; i < 3; i++)
					{
						streamReader.ReadLine();
						num++;
					}
					while (!streamReader.EndOfStream)
					{
						string input = streamReader.ReadLine();
						string empty = string.Empty;
						string text3 = string.Empty;
						string text4 = string.Empty;
						int num2 = 0;
						bool flag = false;
						string text5;
						Strings.ParseLine(input, out empty, out text5, out text3, out text4, out num2, out flag, out text6, true);
						text3 = Strings.ConvertLineBreaksToDisplay(text3);
						text4 = Strings.ConvertLineBreaksToDisplay(text4);
						if (text3.Length > 0)
						{
							text6 = text3;
							for (int j = 0; j < text6.Length; j++)
							{
								char c = text6.get_Chars(j);
								if (!hashSet.Contains(c))
								{
									hashSet.Add(c);
								}
								if (this.CheckForNotLatin(c))
								{
									if (!hashSet3.Contains(c))
									{
										hashSet3.Add(c);
									}
								}
								else if (!hashSet2.Contains(c))
								{
									hashSet2.Add(c);
								}
							}
						}
						if (text4.Length > 0)
						{
							text6 = text4;
							for (int j = 0; j < text6.Length; j++)
							{
								char c2 = text6.get_Chars(j);
								if (!hashSet.Contains(c2))
								{
									hashSet.Add(c2);
								}
								if (this.CheckForNotLatin(c2))
								{
									if (!hashSet3.Contains(c2))
									{
										hashSet3.Add(c2);
									}
								}
								else if (!hashSet2.Contains(c2))
								{
									hashSet2.Add(c2);
								}
							}
						}
						num++;
					}
				}
			}
		}
		string text7 = string.Empty;
		string text8 = string.Empty;
		string text9 = string.Empty;
		if (this.includeDefaultAsciiCharacters)
		{
			foreach (char c3 in Enumerable.ToArray<int>(Enumerable.Range(1, 127)))
			{
				if (c3 != '\n' && c3 != '\r' && c3 != ' ')
				{
					if (!text7.Contains(c3))
					{
						text7 += c3.ToString();
					}
					if (this.CheckForNotLatin(c3))
					{
						if (!text9.Contains(c3))
						{
							text9 += c3.ToString();
						}
					}
					else if (!text8.Contains(c3))
					{
						text8 += c3.ToString();
					}
				}
			}
		}
		foreach (char c4 in hashSet)
		{
			if (c4 != '\n' && c4 != '\r' && c4 != ' ')
			{
				if (!text7.Contains(c4))
				{
					text7 += c4.ToString();
				}
				if (this.CheckForNotLatin(c4))
				{
					if (!text9.Contains(c4))
					{
						text9 += c4.ToString();
					}
				}
				else if (!text8.Contains(c4))
				{
					text8 += c4.ToString();
				}
			}
		}
		text6 = this.customUsedCharacters;
		for (int j = 0; j < text6.Length; j++)
		{
			char c5 = text6.get_Chars(j);
			if (!text7.Contains(c5))
			{
				text7 += c5.ToString();
			}
			if (this.CheckForNotLatin(c5))
			{
				if (!text9.Contains(c5))
				{
					text9 += c5.ToString();
				}
			}
			else if (!text8.Contains(c5))
			{
				text8 += c5.ToString();
			}
		}
		List<string> list = new List<string>();
		list.Add(text7);
		File.WriteAllLines(text, list);
		Debug.Log(text7.Length.ToString() + " unique characters file saved in " + text);
		File.WriteAllLines(text, list);
		Debug.Log(text7.Length.ToString() + " unique characters file saved in " + text);
		list = new List<string>();
		list.Add(text8);
		list = new List<string>();
		list.Add(text9);
		File.WriteAllLines(text2, list);
		Debug.Log(text9.Length.ToString() + " unique characters file saved in " + text2);
	}

	// Token: 0x06002650 RID: 9808 RVA: 0x001F64B4 File Offset: 0x001F46B4
	private bool CheckForNotLatin(char c)
	{
		bool flag = false;
		if ((c > '`' && c < '{') || (c > '@' && c < '['))
		{
			flag = true;
		}
		return !flag;
	}

	// Token: 0x06002651 RID: 9809 RVA: 0x001F64E0 File Offset: 0x001F46E0
	[Button(null, 0)]
	public void ImportCorrections()
	{
		Debug.Log("Importing localization corrections input file...");
		List<string> list = new List<string>();
		DateTime dateTime;
		if (!DateTime.TryParse(this.correctionsInputDate, ref dateTime))
		{
			Debug.Log("Unable to parse input date!");
			list.Add("Unable to parse input date!");
			return;
		}
		List<string> list2 = new List<string>();
		new List<string>();
		new List<string>();
		using (StreamReader streamReader = File.OpenText(Application.streamingAssetsPath + "/Strings/Localization/" + this.templateInputFile))
		{
			while (!streamReader.EndOfStream)
			{
				string text = streamReader.ReadLine();
				list2.Add(text);
			}
		}
		if (list2.Count <= 0)
		{
			Debug.Log("Invalid template file!");
			return;
		}
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		foreach (FileInfo fileInfo in Enumerable.ToList<FileInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Strings/" + this.correctionsLanguage + "/").GetFiles("*.csv", 1)))
		{
			Debug.Log("Loading file: " + fileInfo.FullName);
			string text2 = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
			if (!dictionary.ContainsKey(text2))
			{
				dictionary.Add(text2, new Dictionary<string, string>());
			}
			Debug.Log("Reading file: " + text2);
			using (StreamReader streamReader2 = File.OpenText(fileInfo.FullName))
			{
				int num = 0;
				for (int i = 0; i < 3; i++)
				{
					streamReader2.ReadLine();
					num++;
				}
				while (!streamReader2.EndOfStream)
				{
					string text3 = streamReader2.ReadLine();
					string empty = string.Empty;
					string input = string.Empty;
					string input2 = string.Empty;
					int num2 = 0;
					bool flag = false;
					string text4;
					string text5;
					Strings.ParseLine(text3, out empty, out text4, out input, out input2, out num2, out flag, out text5, true);
					input = Strings.ConvertLineBreaksToDisplay(input);
					input2 = Strings.ConvertLineBreaksToDisplay(input2);
					if (empty.Length > 0)
					{
						if (dictionary[text2].ContainsKey(empty))
						{
							Debug.Log(string.Concat(new string[]
							{
								"Dictionary in ",
								text2,
								" already contains key: ",
								empty,
								", skipping..."
							}));
						}
						else
						{
							dictionary[text2].Add(empty, text3);
						}
					}
					else
					{
						Debug.Log("Found key of length 0 in " + text2 + ", skipping this line...");
					}
					num++;
				}
			}
		}
		Debug.Log("Loaded " + this.correctionsLanguage + " files. Loading corrections input file...");
		string streamingAssetsPath = Application.streamingAssetsPath;
		string text6 = streamingAssetsPath + "/Strings/Localization/" + this.localisationCorrectionsInputFile;
		List<string> list3 = new List<string>();
		using (StreamReader streamReader3 = File.OpenText(text6))
		{
			int num3 = 0;
			List<string> list4 = new List<string>();
			int num4 = 0;
			while (!streamReader3.EndOfStream)
			{
				string text7 = streamReader3.ReadLine();
				List<string> list5 = Enumerable.ToList<string>(new Regex(','.ToString() + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))").Split(text7));
				bool flag2 = false;
				int count = list5.Count;
				for (int j = 0; j < count; j++)
				{
					if (list5[j].Length > 0)
					{
						flag2 = true;
					}
				}
				if (flag2 && list5.Count > 0)
				{
					for (int k = 0; k < list5.Count; k++)
					{
						if (list5[k].Length >= 2 && list5[k].get_Chars(0) == '"' && list5[k].get_Chars(list5[k].Length - 1) == '"')
						{
							list5[k] = list5[k].Substring(1, list5[k].Length - 2);
						}
					}
					string text8 = list5[0].ToLower();
					List<string> list6 = new List<string>();
					string text9 = string.Empty;
					bool flag3 = false;
					for (int l = 0; l < text8.Length; l++)
					{
						char c = text8.get_Chars(l);
						if (c == '<' && !flag3)
						{
							flag3 = true;
						}
						else if (c == '>' && flag3)
						{
							list6.Add(text9);
							text9 = string.Empty;
							flag3 = false;
						}
						else if (flag3)
						{
							text9 += c.ToString();
						}
					}
					if (list6.Count > 0)
					{
						list.Add("Set of keys found on line " + text7);
						num4 = 0;
						list4.Clear();
						list4.AddRange(list6);
					}
					else if (this.useGenderVariationFormatting)
					{
						num4++;
						list.Add("Gender variation ++ : " + num4.ToString());
					}
					using (List<string>.Enumerator enumerator2 = list4.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string text10 = enumerator2.Current;
							string text11 = string.Empty;
							string text12 = string.Empty;
							int m = 0;
							while (m < text10.Length)
							{
								if (text10.get_Chars(m) == ':')
								{
									text11 = text10.Substring(0, m);
									text12 = text10.Substring(m + 1);
									if (!this.useGenderVariationFormatting)
									{
										break;
									}
									if (num4 == 1)
									{
										text12 += "_F";
										break;
									}
									if (num4 == 2)
									{
										text12 += "_NB";
										break;
									}
									break;
								}
								else
								{
									m++;
								}
							}
							if (text12.Length > 0)
							{
								if (dictionary.ContainsKey(text11))
								{
									if (dictionary[text11].ContainsKey(text12))
									{
										string text4;
										string text5;
										string text13;
										string text14;
										int count2;
										bool flag4;
										string text15;
										Strings.ParseLine(dictionary[text11][text12], out text5, out text4, out text13, out text14, out count2, out flag4, out text15, true);
										text15 = text15.Trim();
										if (text15 == null || text15.Length <= 0)
										{
											Debug.Log(string.Concat(new string[]
											{
												"Unable to parse date: ",
												text15,
												" (Key: ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												")"
											}));
											list.Add(string.Concat(new string[]
											{
												"Unable to parse date: ",
												text15,
												" (Key: ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												")"
											}));
											continue;
										}
										DateTime dateTime2;
										if (!DateTime.TryParse(text15, ref dateTime2))
										{
											Debug.Log(string.Concat(new string[]
											{
												"Unable to parse date: ",
												text15,
												" (Key: ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												")"
											}));
											list.Add(string.Concat(new string[]
											{
												"Unable to parse date: ",
												text15,
												" (Key: ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												")"
											}));
											continue;
										}
										if (dateTime2 >= dateTime)
										{
											Debug.Log(string.Concat(new string[]
											{
												"Existing key ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												" is newer (",
												dateTime2.ToString(),
												") than the input date (",
												dateTime.ToString(),
												"), skipping..."
											}));
											list.Add(string.Concat(new string[]
											{
												"Existing key ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												" is newer (",
												dateTime2.ToString(),
												") than the input date (",
												dateTime.ToString(),
												"), skipping..."
											}));
											continue;
										}
										if (dateTime2 < dateTime)
										{
											Debug.Log(string.Concat(new string[]
											{
												"Existing key ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												" is older (",
												dateTime2.ToString(),
												") than the input date (",
												dateTime.ToString(),
												"), overwriting with new content: ",
												list5[this.columnContent]
											}));
											list.Add(string.Concat(new string[]
											{
												"Existing key ",
												this.correctionsLanguage,
												", ",
												text11,
												", ",
												text12,
												" is older (",
												dateTime2.ToString(),
												") than the input date (",
												dateTime.ToString(),
												"), overwriting with new content: ",
												list5[this.columnContent]
											}));
										}
									}
									else
									{
										if (!this.createMissingKey)
										{
											Debug.Log(string.Concat(new string[]
											{
												"Missing key: ",
												text11,
												": ",
												text12,
												", this will be skipped"
											}));
											list.Add(string.Concat(new string[]
											{
												"Missing key: ",
												text11,
												": ",
												text12,
												", this will be skipped"
											}));
											continue;
										}
										Debug.Log(string.Concat(new string[]
										{
											"Missing key: ",
											text11,
											": ",
											text12,
											", this will be appended to the file"
										}));
										list.Add(string.Concat(new string[]
										{
											"Missing key: ",
											text11,
											": ",
											text12,
											", this will be appended to the file"
										}));
									}
								}
								else
								{
									if (!this.createMissingFiles)
									{
										Debug.Log("Missing file: " + text11 + ", this will be skipped");
										list.Add("Missing file: " + text11 + ", this will be skipped");
										continue;
									}
									Debug.Log("Missing file: " + text11 + ", this will be created");
									list.Add("Missing file: " + text11 + ", this will be created");
								}
								if (!dictionary.ContainsKey(text11))
								{
									dictionary.Add(text11, new Dictionary<string, string>());
								}
								if (!dictionary[text11].ContainsKey(text12))
								{
									dictionary[text11].Add(text12, string.Empty);
								}
								string text16 = list5[this.columnContent];
								if (text16 != null && text16.Length > 0)
								{
									if (!list3.Contains(text11))
									{
										list3.Add(text11);
									}
									string text17 = string.Concat(new string[]
									{
										"\"",
										text12,
										"\",\"",
										string.Empty,
										"\",\"",
										text16,
										"\",\"",
										string.Empty,
										"\",",
										string.Empty,
										",\"",
										string.Empty,
										"\",\"",
										this.correctionsInputDate,
										"\""
									});
									dictionary[text11][text12] = text17;
								}
							}
						}
						goto IL_B73;
					}
					goto IL_B5C;
				}
				goto IL_B5C;
				IL_B73:
				num3++;
				continue;
				IL_B5C:
				list.Add("No contents found on line " + num3.ToString());
				goto IL_B73;
			}
		}
		foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in dictionary)
		{
			if (list3.Contains(keyValuePair.Key))
			{
				string text18 = string.Concat(new string[]
				{
					streamingAssetsPath,
					"/Strings/",
					this.correctionsLanguage,
					"/",
					keyValuePair.Key,
					".csv"
				});
				if (keyValuePair.Key != "names.rooms" && keyValuePair.Key.Length >= 5 && keyValuePair.Key.Substring(0, 5) == "names")
				{
					string[] array = new string[5];
					array[0] = "Skipping file: ";
					array[1] = text18;
					array[2] = " (";
					int num5 = 3;
					int count2 = keyValuePair.Value.Count;
					array[num5] = count2.ToString();
					array[4] = " keys)...";
					Debug.Log(string.Concat(array));
					List<string> list7 = list;
					string[] array2 = new string[5];
					array2[0] = "Skipping file: ";
					array2[1] = text18;
					array2[2] = " (";
					int num6 = 3;
					count2 = keyValuePair.Value.Count;
					array2[num6] = count2.ToString();
					array2[4] = " keys)...";
					list7.Add(string.Concat(array2));
				}
				else
				{
					List<string> list8 = new List<string>(list2);
					foreach (KeyValuePair<string, string> keyValuePair2 in keyValuePair.Value)
					{
						list8.Add(keyValuePair2.Value);
					}
					string[] array3 = new string[5];
					array3[0] = "Writing: ";
					array3[1] = text18;
					array3[2] = " (";
					int num7 = 3;
					int count2 = keyValuePair.Value.Count;
					array3[num7] = count2.ToString();
					array3[4] = " keys)...";
					Debug.Log(string.Concat(array3));
					List<string> list9 = list;
					string[] array4 = new string[5];
					array4[0] = "Writing: ";
					array4[1] = text18;
					array4[2] = " (";
					int num8 = 3;
					count2 = keyValuePair.Value.Count;
					array4[num8] = count2.ToString();
					array4[4] = " keys)...";
					list9.Add(string.Concat(array4));
					File.WriteAllLines(text18, list8);
				}
			}
		}
		Debug.Log("Localization update complete!");
		list.Add("Localization update complete!");
		StreamWriter streamWriter = new StreamWriter(streamingAssetsPath + "/Strings/Localization/Input/OutputDebug.txt", false);
		for (int n = 0; n < list.Count; n++)
		{
			streamWriter.WriteLine(list[n]);
		}
		streamWriter.Close();
	}

	// Token: 0x06002652 RID: 9810 RVA: 0x001F73B4 File Offset: 0x001F55B4
	[Button(null, 0)]
	public void OutputSerializedLanguageConfig()
	{
		foreach (LanguageConfigLoader.LocInput locInput in LanguageConfigLoader.Instance.fileInputConfig)
		{
			string text = Path.Combine(new string[]
			{
				Application.streamingAssetsPath + "/Strings/" + locInput.languageCode + "/LanguageSettings.txt"
			});
			string text2 = JsonUtility.ToJson(locInput, true);
			using (StreamWriter streamWriter = File.CreateText(text))
			{
				streamWriter.Write(text2);
			}
		}
	}

	// Token: 0x040044BF RID: 17599
	[Header("Setup")]
	public GameObject languageLoaderPrefab;

	// Token: 0x040044C0 RID: 17600
	public static bool textFilesLoaded = false;

	// Token: 0x040044C1 RID: 17601
	public static bool backupENGLoaded = false;

	// Token: 0x040044C2 RID: 17602
	public static LanguageConfigLoader.LocInput loadedLanguage;

	// Token: 0x040044C3 RID: 17603
	public static Dictionary<string, Dictionary<string, Strings.DisplayString>> stringTable = new Dictionary<string, Dictionary<string, Strings.DisplayString>>();

	// Token: 0x040044C4 RID: 17604
	public static Dictionary<string, string> dictionaryPathnames = new Dictionary<string, string>();

	// Token: 0x040044C5 RID: 17605
	public static Dictionary<string, Dictionary<string, Strings.DisplayString>> stringTableENG = new Dictionary<string, Dictionary<string, Strings.DisplayString>>();

	// Token: 0x040044C6 RID: 17606
	private static FileInfo templateFile;

	// Token: 0x040044C7 RID: 17607
	public static Dictionary<string, List<Strings.RandomDisplayString>> randomEntryLists = new Dictionary<string, List<Strings.RandomDisplayString>>();

	// Token: 0x040044C8 RID: 17608
	public static Dictionary<string, List<Strings.RandomDisplayString>> randomEntryListsENG = new Dictionary<string, List<Strings.RandomDisplayString>>();

	// Token: 0x040044C9 RID: 17609
	[Header("Localisation Output")]
	public List<string> localisationIgnoreFileList = new List<string>();

	// Token: 0x040044CA RID: 17610
	public List<string> localisationIgnoreDirectoryList = new List<string>();

	// Token: 0x040044CB RID: 17611
	[Tooltip("If the below string is present in the notes section then skip the line")]
	public bool useIgnoreFlagInNotes = true;

	// Token: 0x040044CC RID: 17612
	[EnableIf("useIgnoreFlagInNotes")]
	public string ignoreFlag = "LOC_IGNORE";

	// Token: 0x040044CD RID: 17613
	[Range(0f, 5f)]
	[Tooltip("Add this many extra line breaks between entries in the output")]
	public int extraLineBreaks = 3;

	// Token: 0x040044CE RID: 17614
	[Space(7f)]
	[Tooltip("If two or more identical content strings are detected in english, write them to a single entry for output")]
	public bool condenseIdenticalEnglishContentIntoOneKey = true;

	// Token: 0x040044CF RID: 17615
	[Space(7f)]
	[Tooltip("If true: Only output changes since this the below date...")]
	public bool onlyOuputChangesSince;

	// Token: 0x040044D0 RID: 17616
	[EnableIf("onlyOuputChangesSince")]
	public string outputSinceDate;

	// Token: 0x040044D1 RID: 17617
	[Header("Localisation Input")]
	public string localisationInputFile;

	// Token: 0x040044D2 RID: 17618
	public string inputDate;

	// Token: 0x040044D3 RID: 17619
	public string templateInputFile;

	// Token: 0x040044D4 RID: 17620
	[Tooltip("The last column should be ignored when checking for content")]
	public bool inputFeaturesLastColumnLineNumbers = true;

	// Token: 0x040044D5 RID: 17621
	[Tooltip("Write this if the localized text is missing. If detected, this will revert to the ENG string if english string")]
	public static string missingString = "<empty>";

	// Token: 0x040044D6 RID: 17622
	[Tooltip("These will be added to the character output")]
	public string customUsedCharacters = string.Empty;

	// Token: 0x040044D7 RID: 17623
	[Tooltip("ASCII characters will be added to the character output")]
	public bool includeDefaultAsciiCharacters = true;

	// Token: 0x040044D8 RID: 17624
	[Header("Localisation Corrections")]
	public string localisationCorrectionsInputFile;

	// Token: 0x040044D9 RID: 17625
	public string correctionsInputDate;

	// Token: 0x040044DA RID: 17626
	public string correctionsLanguage;

	// Token: 0x040044DB RID: 17627
	[Tooltip("If true this will detect lines immediately below keys as gender variants of the keys with the following format: Main = M, 1 = Female, 2 = NB")]
	public bool useGenderVariationFormatting = true;

	// Token: 0x040044DC RID: 17628
	[Tooltip("The column index in which the correct new translation is found")]
	public int columnContent = 3;

	// Token: 0x040044DD RID: 17629
	[Tooltip("If the corrections file features a file that doesn't exist in the existing text, create it")]
	public bool createMissingFiles = true;

	// Token: 0x040044DE RID: 17630
	[Tooltip("If the corrections file features a key that doesn't exist in the existing text, create it")]
	public bool createMissingKey = true;

	// Token: 0x040044DF RID: 17631
	[Header("Debug")]
	public string findBlock;

	// Token: 0x040044E0 RID: 17632
	private Dictionary<object, Strings.LinkData> linkDictionary = new Dictionary<object, Strings.LinkData>();

	// Token: 0x040044E1 RID: 17633
	private Dictionary<Evidence, List<Strings.LinkData>> evidenceLinkDictionary = new Dictionary<Evidence, List<Strings.LinkData>>();

	// Token: 0x040044E2 RID: 17634
	public Dictionary<int, Strings.LinkData> linkIDReference = new Dictionary<int, Strings.LinkData>();

	// Token: 0x040044E3 RID: 17635
	private static Strings _instance;

	// Token: 0x0200080B RID: 2059
	public class DisplayString
	{
		// Token: 0x040044E4 RID: 17636
		public string displayStr;

		// Token: 0x040044E5 RID: 17637
		public string alternateStr;
	}

	// Token: 0x0200080C RID: 2060
	public class RandomDisplayString
	{
		// Token: 0x040044E6 RID: 17638
		public string displayStr;

		// Token: 0x040044E7 RID: 17639
		public string alternateStr;

		// Token: 0x040044E8 RID: 17640
		public bool needsSuffixForShortName;
	}

	// Token: 0x0200080D RID: 2061
	public enum Casing
	{
		// Token: 0x040044EA RID: 17642
		asIs,
		// Token: 0x040044EB RID: 17643
		firstLetterCaptial,
		// Token: 0x040044EC RID: 17644
		pascalCase,
		// Token: 0x040044ED RID: 17645
		upperCase,
		// Token: 0x040044EE RID: 17646
		lowerCase
	}

	// Token: 0x0200080E RID: 2062
	public enum LinkSetting
	{
		// Token: 0x040044F0 RID: 17648
		automatic,
		// Token: 0x040044F1 RID: 17649
		forceLinks,
		// Token: 0x040044F2 RID: 17650
		forceNoLinks
	}

	// Token: 0x0200080F RID: 2063
	public class LinkData
	{
		// Token: 0x06002658 RID: 9816 RVA: 0x001F7564 File Offset: 0x001F5764
		public LinkData(Evidence newEvidence, List<Evidence.DataKey> overrideKeys)
		{
			this.id = Strings.LinkData.assignID;
			Strings.LinkData.assignID++;
			this.evidence = newEvidence;
			this.dataKeys = overrideKeys;
			if (!Strings.Instance.evidenceLinkDictionary.ContainsKey(this.evidence))
			{
				Strings.Instance.evidenceLinkDictionary.Add(this.evidence, new List<Strings.LinkData>());
			}
			Strings.Instance.evidenceLinkDictionary[this.evidence].Add(this);
			Strings.Instance.linkIDReference.Add(this.id, this);
		}

		// Token: 0x06002659 RID: 9817 RVA: 0x001F7600 File Offset: 0x001F5800
		public LinkData(Telephone newTelephone = null)
		{
			this.id = Strings.LinkData.assignID;
			Strings.LinkData.assignID++;
			this.evidence = newTelephone.telephoneEntry;
			this.inputCode = newTelephone.GetInputCode();
			Strings.Instance.linkDictionary.Add(newTelephone, this);
			Strings.Instance.linkIDReference.Add(this.id, this);
		}

		// Token: 0x0600265A RID: 9818 RVA: 0x001F766C File Offset: 0x001F586C
		public LinkData(List<int> newInputCode = null)
		{
			this.id = Strings.LinkData.assignID;
			Strings.LinkData.assignID++;
			this.inputCode = newInputCode;
			Strings.Instance.linkDictionary.Add(this.inputCode, this);
			Strings.Instance.linkIDReference.Add(this.id, this);
		}

		// Token: 0x0600265B RID: 9819 RVA: 0x001F76C9 File Offset: 0x001F58C9
		public LinkData(string newHelpPage)
		{
			this.id = Strings.LinkData.assignID;
			Strings.LinkData.assignID++;
			Strings.Instance.linkIDReference.Add(this.id, this);
			this.helpPage = newHelpPage;
		}

		// Token: 0x0600265C RID: 9820 RVA: 0x001F7708 File Offset: 0x001F5908
		public void OnLink()
		{
			AudioController.Instance.Play2DSound(AudioControls.Instance.inLineLink, null, 1f);
			if (this.inputCode != null && InterfaceController.Instance.activeCodeInput != null && InterfaceController.Instance.activeCodeInput.digits == this.inputCode.Count)
			{
				InterfaceController.Instance.activeCodeInput.parentWindow.SetActiveContent(InterfaceController.Instance.activeCodeInput.windowContent);
				InterfaceController.Instance.activeCodeInput.OnInputCode(this.inputCode);
				return;
			}
			if (this.evidence != null)
			{
				if (Game.Instance.devMode)
				{
					string text = string.Empty;
					if (this.dataKeys != null)
					{
						foreach (Evidence.DataKey dataKey in this.dataKeys)
						{
							text = text + ", " + dataKey.ToString();
						}
					}
					string text2 = "OnLink: ";
					Evidence evidence = this.evidence;
					Game.Log(text2 + ((evidence != null) ? evidence.ToString() : null) + " keys: " + text, 2);
				}
				InterfaceController.Instance.SpawnWindow(this.evidence, Evidence.DataKey.name, this.dataKeys, "", false, true, default(Vector2), null, null, null, true);
				return;
			}
			if (this.helpPage != null && this.helpPage.Length > 0)
			{
				if (HelpController.Instance == null)
				{
					InterfaceController.Instance.ToggleNotebook(this.helpPage, true);
					return;
				}
				HelpController.Instance.DisplayHelpPage(this.helpPage);
			}
		}

		// Token: 0x040044F3 RID: 17651
		public int id;

		// Token: 0x040044F4 RID: 17652
		public static int assignID = 1;

		// Token: 0x040044F5 RID: 17653
		public Evidence evidence;

		// Token: 0x040044F6 RID: 17654
		public List<Evidence.DataKey> dataKeys;

		// Token: 0x040044F7 RID: 17655
		public List<int> inputCode;

		// Token: 0x040044F8 RID: 17656
		public string helpPage;
	}
}
