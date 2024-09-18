using System;
using System.Collections.Generic;
using System.IO;
using ModIO;
using UnityEngine;

// Token: 0x02000334 RID: 820
public class ModLoader : MonoBehaviour
{
	// Token: 0x17000092 RID: 146
	// (get) Token: 0x06001274 RID: 4724 RVA: 0x00105C40 File Offset: 0x00103E40
	public static ModLoader Instance
	{
		get
		{
			return ModLoader._instance;
		}
	}

	// Token: 0x06001275 RID: 4725 RVA: 0x00105C47 File Offset: 0x00103E47
	private void Awake()
	{
		if (ModLoader._instance != null && ModLoader._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ModLoader._instance = this;
		}
		Object.DontDestroyOnLoad(base.gameObject);
		this.GetMods();
	}

	// Token: 0x06001276 RID: 4726 RVA: 0x00105C88 File Offset: 0x00103E88
	public void GetMods()
	{
		Game.Log("Mods: Refreshing mods...", 2);
		this.sortedModsList.Clear();
		Result result;
		UserInstalledMod[] installedModsForUser = ModIOUnity.GetInstalledModsForUser(ref result, true);
		if (result.Succeeded())
		{
			foreach (UserInstalledMod userInstalledMod in installedModsForUser)
			{
				Game.Log("Menu: Found installed mod: " + userInstalledMod.modProfile.name + " at " + userInstalledMod.directory, 2);
				ModSettingsData modSettingsData = null;
				string text = userInstalledMod.directory + "/ModSettings.txt";
				if (File.Exists(text))
				{
					string text2 = string.Empty;
					using (StreamReader streamReader = File.OpenText(text))
					{
						text2 = streamReader.ReadToEnd();
					}
					modSettingsData = JsonUtility.FromJson<ModSettingsData>(text2);
					Game.Log("Menu: Parsed an existing mod settings file from " + text, 2);
				}
				if (modSettingsData == null)
				{
					Game.Log("Menu: Cannot find an existing mod settings file at " + text + ", creating a new one...", 2);
					modSettingsData = new ModSettingsData();
					modSettingsData.name = userInstalledMod.modProfile.name;
					string text3 = JsonUtility.ToJson(modSettingsData, true);
					using (StreamWriter streamWriter = File.CreateText(text))
					{
						streamWriter.Write(text3);
					}
				}
				modSettingsData.modData = userInstalledMod;
				this.sortedModsList.Add(modSettingsData);
			}
		}
		this.sortedModsList.Sort((ModSettingsData p1, ModSettingsData p2) => p1.loadOrderValue.CompareTo(p2.loadOrderValue));
		int num = -999999999;
		for (int j = 0; j < this.sortedModsList.Count; j++)
		{
			ModSettingsData modSettingsData2 = this.sortedModsList[j];
			if (j > 0 && modSettingsData2.loadOrderValue <= num)
			{
				modSettingsData2.loadOrderValue = num + 1;
				Game.Log("Mods: Writing new load order for " + modSettingsData2.modData.modProfile.name + ": " + modSettingsData2.loadOrderValue.ToString(), 2);
				string text4 = modSettingsData2.modData.directory + "/ModSettings.txt";
				string text5 = JsonUtility.ToJson(modSettingsData2, true);
				using (StreamWriter streamWriter2 = File.CreateText(text4))
				{
					streamWriter2.Write(text5);
				}
			}
			num = modSettingsData2.loadOrderValue;
		}
		this.modsLoaded = true;
	}

	// Token: 0x06001277 RID: 4727 RVA: 0x00105F04 File Offset: 0x00104104
	public List<FileInfo> GetFilesWithinActiveMods(string subPath, params string[] fileExtensions)
	{
		List<FileInfo> list = new List<FileInfo>();
		if (Game.Instance != null && !Game.Instance.allowMods)
		{
			return list;
		}
		if (!this.modsLoaded)
		{
			Game.LogError("Attempting to retrieve mod info before mods are loaded...", 2);
			return list;
		}
		for (int i = 0; i < this.sortedModsList.Count; i++)
		{
			ModSettingsData modSettingsData = this.sortedModsList[i];
			if (modSettingsData.modData.enabled)
			{
				string text = modSettingsData.modData.directory + subPath;
				if (Directory.Exists(text))
				{
					Debug.Log("Mods: Directory " + text + " exists within mod " + modSettingsData.modData.modProfile.name);
					DirectoryInfo directoryInfo = new DirectoryInfo(text);
					foreach (string text2 in fileExtensions)
					{
						FileInfo[] files = directoryInfo.GetFiles("*." + text2, 1);
						Debug.Log(string.Concat(new string[]
						{
							"Mods: Found ",
							files.Length.ToString(),
							" with file ext ",
							text2,
							"   within ",
							text
						}));
						list.AddRange(files);
					}
				}
			}
		}
		return list;
	}

	// Token: 0x06001278 RID: 4728 RVA: 0x00106048 File Offset: 0x00104248
	public List<DirectoryInfo> GetActiveLanguageModDirectories()
	{
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		if (Game.Instance != null && !Game.Instance.allowMods)
		{
			return list;
		}
		if (!this.modsLoaded)
		{
			Game.LogError("Attempting to retrieve mod info before mods are loaded...", 2);
			return list;
		}
		for (int i = 0; i < this.sortedModsList.Count; i++)
		{
			ModSettingsData modSettingsData = this.sortedModsList[i];
			if (modSettingsData.modData.enabled)
			{
				string text = modSettingsData.modData.directory + "/StreamingAssets/Strings";
				if (Directory.Exists(text))
				{
					Debug.Log("Mods: Directory " + text + " exists within mod " + modSettingsData.modData.modProfile.name);
					DirectoryInfo[] directories = new DirectoryInfo(text).GetDirectories();
					Debug.Log("Mods: Found " + directories.Length.ToString() + " within " + text);
					list.AddRange(directories);
				}
			}
		}
		return list;
	}

	// Token: 0x06001279 RID: 4729 RVA: 0x0010613C File Offset: 0x0010433C
	public List<FileInfo> GetActiveCities()
	{
		return this.GetFilesWithinActiveMods("/StreamingAssets/Cities", new string[]
		{
			"cit",
			"citb",
			"txt"
		});
	}

	// Token: 0x0600127A RID: 4730 RVA: 0x00106167 File Offset: 0x00104367
	public List<FileInfo> GetActiveSaves()
	{
		return this.GetFilesWithinActiveMods("/StreamingAssets/Save", new string[]
		{
			"sod",
			"sodb"
		});
	}

	// Token: 0x0600127B RID: 4731 RVA: 0x0010618C File Offset: 0x0010438C
	public List<DirectoryInfo> GetActiveDDSModDirectories()
	{
		List<DirectoryInfo> list = new List<DirectoryInfo>();
		if (Game.Instance != null && !Game.Instance.allowMods)
		{
			return list;
		}
		if (!this.modsLoaded)
		{
			Game.LogError("Attempting to retrieve mod info before mods are loaded...", 2);
			return list;
		}
		for (int i = 0; i < this.sortedModsList.Count; i++)
		{
			ModSettingsData modSettingsData = this.sortedModsList[i];
			if (modSettingsData.modData.enabled)
			{
				string text = modSettingsData.modData.directory + "/StreamingAssets/DDS";
				if (Directory.Exists(text))
				{
					Debug.Log("Mods: Directory " + text + " exists within mod " + modSettingsData.modData.modProfile.name);
					DirectoryInfo[] directories = new DirectoryInfo(text).GetDirectories();
					Debug.Log("Mods: Found " + directories.Length.ToString() + " within " + text);
					list.AddRange(directories);
				}
			}
		}
		return list;
	}

	// Token: 0x040016B0 RID: 5808
	[Header("Status")]
	public bool modsLoaded;

	// Token: 0x040016B1 RID: 5809
	public List<ModSettingsData> sortedModsList = new List<ModSettingsData>();

	// Token: 0x040016B2 RID: 5810
	private static ModLoader _instance;
}
