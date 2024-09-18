using System;
using System.Collections.Generic;
using ModIO;
using ModIOBrowser;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000331 RID: 817
public class ModController : MonoBehaviour
{
	// Token: 0x17000090 RID: 144
	// (get) Token: 0x06001260 RID: 4704 RVA: 0x00105311 File Offset: 0x00103511
	private bool hasSpawned
	{
		get
		{
			return this.spawnedBrowser != null;
		}
	}

	// Token: 0x17000091 RID: 145
	// (get) Token: 0x06001261 RID: 4705 RVA: 0x0010531F File Offset: 0x0010351F
	public static ModController Instance
	{
		get
		{
			return ModController._instance;
		}
	}

	// Token: 0x06001262 RID: 4706 RVA: 0x00105328 File Offset: 0x00103528
	private void Awake()
	{
		if (ModController._instance != null && ModController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ModController._instance = this;
		}
		if (ModLoader.Instance == null)
		{
			Object.Instantiate<GameObject>(this.modLoaderPrefab);
		}
	}

	// Token: 0x06001263 RID: 4707 RVA: 0x0010537B File Offset: 0x0010357B
	private void Start()
	{
		this.SetModConfigChanged(false);
	}

	// Token: 0x06001264 RID: 4708 RVA: 0x00105384 File Offset: 0x00103584
	public void OpenModBrowser()
	{
		if (!this.hasSpawned)
		{
			this.spawnedBrowser = Object.Instantiate<GameObject>(this.browserPrefab);
		}
		Game.Log("Mods: Opening Mod.io browser...", 2);
		Browser.Open(new Action(this.OnBrowserClose));
		this.SetModConfigChanged(true);
	}

	// Token: 0x06001265 RID: 4709 RVA: 0x001053C2 File Offset: 0x001035C2
	public void OnBrowserClose()
	{
		Game.Log("Mods: User has closed Mod.io browser...", 2);
		this.UpdateModEntries();
	}

	// Token: 0x06001266 RID: 4710 RVA: 0x001053D8 File Offset: 0x001035D8
	public void UpdateModEntries()
	{
		if (ModLoader.Instance == null)
		{
			Game.Log("Mods: Instantiating new mod loader object...", 2);
			Object.Instantiate<GameObject>(this.modLoaderPrefab);
		}
		ModLoader.Instance.GetMods();
		while (this.spawnedModElements.Count > 0)
		{
			if (this.spawnedModElements[0] != null)
			{
				Object.Destroy(this.spawnedModElements[0].gameObject);
			}
			this.spawnedModElements.RemoveAt(0);
		}
		Game.Log("Mods: Updating mod entries for " + ModLoader.Instance.sortedModsList.Count.ToString() + " loaded mods...", 2);
		for (int i = 0; i < ModLoader.Instance.sortedModsList.Count; i++)
		{
			ModSettingsData newMod = ModLoader.Instance.sortedModsList[i];
			ModEntryController component = Object.Instantiate<GameObject>(this.modElementPrefab, this.modContentRect).GetComponent<ModEntryController>();
			component.Setup(newMod);
			this.spawnedModElements.Add(component);
		}
	}

	// Token: 0x06001267 RID: 4711 RVA: 0x001054DA File Offset: 0x001036DA
	public void SetModConfigChanged(bool val)
	{
		this.modConfigChanged = val;
		this.applyButton.SetInteractable(this.modConfigChanged);
	}

	// Token: 0x06001268 RID: 4712 RVA: 0x001054F4 File Offset: 0x001036F4
	public void OnApplyRestartButton()
	{
		if (this.modConfigChanged)
		{
			PopupMessageController.Instance.PopupMessage("ModRestart", true, true, "Cancel", "Confirm", true, PopupMessageController.AffectPauseState.automatic, false, "", false, false, false, false, "", "", false, "", false, "", "");
			PopupMessageController.Instance.OnRightButton += this.OnRestartConfirm;
			PopupMessageController.Instance.OnLeftButton += this.OnRestartCancel;
		}
	}

	// Token: 0x06001269 RID: 4713 RVA: 0x00105578 File Offset: 0x00103778
	public void OnRestartConfirm()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnRestartConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.OnRestartCancel;
		AudioController.Instance.StopAllSounds();
		Object.Destroy(ModLoader.Instance.gameObject);
		Object.Destroy(LanguageConfigLoader.Instance.gameObject);
		SceneManager.LoadScene("ControllerDetect");
	}

	// Token: 0x0600126A RID: 4714 RVA: 0x001055E3 File Offset: 0x001037E3
	public void OnRestartCancel()
	{
		PopupMessageController.Instance.OnRightButton -= this.OnRestartConfirm;
		PopupMessageController.Instance.OnLeftButton -= this.OnRestartCancel;
		this.SetModConfigChanged(false);
	}

	// Token: 0x04001699 RID: 5785
	[Header("Setup")]
	public GameObject browserPrefab;

	// Token: 0x0400169A RID: 5786
	public GameObject modElementPrefab;

	// Token: 0x0400169B RID: 5787
	public GameObject modLoaderPrefab;

	// Token: 0x0400169C RID: 5788
	[Header("Status")]
	[Tooltip("If true this will prompt a restart of the game to properly load-in mods")]
	public bool modConfigChanged;

	// Token: 0x0400169D RID: 5789
	[Header("Components")]
	public RectTransform modContentRect;

	// Token: 0x0400169E RID: 5790
	public GameObject spawnedBrowser;

	// Token: 0x0400169F RID: 5791
	public List<ModEntryController> spawnedModElements = new List<ModEntryController>();

	// Token: 0x040016A0 RID: 5792
	public ButtonController applyButton;

	// Token: 0x040016A1 RID: 5793
	private static ModController _instance;

	// Token: 0x02000332 RID: 818
	[Serializable]
	public class ModIconSetup
	{
		// Token: 0x040016A2 RID: 5794
		public SubscribedModStatus state;

		// Token: 0x040016A3 RID: 5795
		public Sprite spriteEnabled;

		// Token: 0x040016A4 RID: 5796
		public Sprite spriteDisabled;
	}
}
