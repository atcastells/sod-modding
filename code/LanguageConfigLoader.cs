using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// Token: 0x02000324 RID: 804
public class LanguageConfigLoader : MonoBehaviour
{
	// Token: 0x1700008D RID: 141
	// (get) Token: 0x06001236 RID: 4662 RVA: 0x00102E65 File Offset: 0x00101065
	public static LanguageConfigLoader Instance
	{
		get
		{
			return LanguageConfigLoader._instance;
		}
	}

	// Token: 0x06001237 RID: 4663 RVA: 0x00102E6C File Offset: 0x0010106C
	private void Awake()
	{
		if (LanguageConfigLoader._instance != null && LanguageConfigLoader._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			LanguageConfigLoader._instance = this;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		this.LoadLanguageConfig();
	}

	// Token: 0x06001238 RID: 4664 RVA: 0x00102EAC File Offset: 0x001010AC
	public void LoadLanguageConfig()
	{
		if (this.loadedLanguageConfig)
		{
			return;
		}
		List<DirectoryInfo> list = Enumerable.ToList<DirectoryInfo>(new DirectoryInfo(Application.streamingAssetsPath + "/Strings/").GetDirectories());
		list.AddRange(ModLoader.Instance.GetActiveLanguageModDirectories());
		foreach (DirectoryInfo directoryInfo in list)
		{
			Game.Log("Menu: Language directory found: " + directoryInfo.FullName, 2);
			string text = directoryInfo.FullName + "/LanguageSettings.txt";
			if (File.Exists(text))
			{
				string text2 = string.Empty;
				using (StreamReader streamReader = File.OpenText(text))
				{
					text2 = streamReader.ReadToEnd();
				}
				LanguageConfigLoader.LocInput locSettings = JsonUtility.FromJson<LanguageConfigLoader.LocInput>(text2);
				if (locSettings != null)
				{
					Game.Log("Menu: Successfully parsed a language info file: " + locSettings.languageCode, 2);
					LanguageConfigLoader.LocInput locInput = this.fileInputConfig.Find((LanguageConfigLoader.LocInput item) => item.languageCode.ToLower() == locSettings.languageCode.ToLower());
					if (locInput != null)
					{
						using (List<FileInfo>.Enumerator enumerator2 = Enumerable.ToList<FileInfo>(new DirectoryInfo(directoryInfo.FullName).GetFiles("*.csv", 1)).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								FileInfo fileInfo = enumerator2.Current;
								Game.Log("Menu: " + locSettings.languageCode + ": Found a file override: " + fileInfo.FullName, 2);
								string text3 = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
								if (!locInput.modOverrideFiles.ContainsKey(text3))
								{
									locInput.modOverrideFiles.Add(text3, fileInfo);
								}
								else
								{
									locInput.modOverrideFiles[text3] = fileInfo;
								}
								locInput.debugOverrideFiles.Add(fileInfo.FullName);
							}
							continue;
						}
					}
					locSettings.path = directoryInfo.FullName;
					this.fileInputConfig.Add(locSettings);
				}
			}
			else
			{
				Game.Log("Menu: No language config exists at " + text, 2);
			}
		}
		this.loadedLanguageConfig = true;
	}

	// Token: 0x0400163C RID: 5692
	public bool loadedLanguageConfig;

	// Token: 0x0400163D RID: 5693
	public List<LanguageConfigLoader.LocInput> fileInputConfig = new List<LanguageConfigLoader.LocInput>();

	// Token: 0x0400163E RID: 5694
	private static LanguageConfigLoader _instance;

	// Token: 0x02000325 RID: 805
	[Serializable]
	public class LocInput
	{
		// Token: 0x0400163F RID: 5695
		public string languageCode;

		// Token: 0x04001640 RID: 5696
		public string displayName;

		// Token: 0x04001641 RID: 5697
		public int documentColumn;

		// Token: 0x04001642 RID: 5698
		public SystemLanguage systemLanguage;

		// Token: 0x04001643 RID: 5699
		[Tooltip("Display 'Mr' etc after the name")]
		public bool swapCitizenTitleOrder;

		// Token: 0x04001644 RID: 5700
		public bool staticKillerMoniker = true;

		// Token: 0x04001645 RID: 5701
		public string startText;

		// Token: 0x04001646 RID: 5702
		[NonSerialized]
		public string path;

		// Token: 0x04001647 RID: 5703
		[NonSerialized]
		public Dictionary<string, FileInfo> modOverrideFiles = new Dictionary<string, FileInfo>();

		// Token: 0x04001648 RID: 5704
		public List<string> debugOverrideFiles = new List<string>();
	}
}
