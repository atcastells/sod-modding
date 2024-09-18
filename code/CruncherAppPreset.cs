using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x020006F6 RID: 1782
[CreateAssetMenu(fileName = "cruncherapp_data", menuName = "Database/Cruncher App")]
public class CruncherAppPreset : SoCustomComparison
{
	// Token: 0x0400331A RID: 13082
	[Header("App Settings")]
	[Tooltip("The background image to display on load")]
	public Material loadBackground;

	// Token: 0x0400331B RID: 13083
	[Tooltip("The background image to display once app has loaded")]
	public Material loadedBackground;

	// Token: 0x0400331C RID: 13084
	[Tooltip("Use the cursor if the player is controlling the computer")]
	public bool useCursor = true;

	// Token: 0x0400331D RID: 13085
	[Tooltip("The cursor to use")]
	public Sprite cursorSprite;

	// Token: 0x0400331E RID: 13086
	[Tooltip("Use timer to exit: The app will exit on this timer")]
	public bool useTimer;

	// Token: 0x0400331F RID: 13087
	[Tooltip("Timer length in seconds")]
	[EnableIf("useTimer")]
	public float timerLength = 3f;

	// Token: 0x04003320 RID: 13088
	[Tooltip("Take this time to load in")]
	public float loadTime = 1f;

	// Token: 0x04003321 RID: 13089
	[Tooltip("How heavy this is on loading the machine (1 = constant)")]
	[Range(0f, 1f)]
	public float loadDemand = 0.33f;

	// Token: 0x04003322 RID: 13090
	[Tooltip("Always load during the duration of this app")]
	public bool alwaysLoad;

	// Token: 0x04003323 RID: 13091
	[Range(0f, 1f)]
	[Tooltip("How heavy this is on loading the machine (1 = constant)")]
	public float alwaysLoadDemand = 0.33f;

	// Token: 0x04003324 RID: 13092
	[Tooltip("App Icon displayed on desktop")]
	public Sprite desktopIcon;

	// Token: 0x04003325 RID: 13093
	[Tooltip("Computer light emmits this colour")]
	public Color screenLightColourOnLoad = Color.white;

	// Token: 0x04003326 RID: 13094
	[Tooltip("Computer light emmits this colour")]
	public Color screenLightColourOnFinishLoad = Color.white;

	// Token: 0x04003327 RID: 13095
	[Header("Access")]
	public bool alwaysInstalled;

	// Token: 0x04003328 RID: 13096
	[DisableIf("alwaysInstalled")]
	public bool onlyIfCorporateSabotageSkill;

	// Token: 0x04003329 RID: 13097
	[DisableIf("alwaysInstalled")]
	public bool companyOnly;

	// Token: 0x0400332A RID: 13098
	[DisableIf("alwaysInstalled")]
	public bool salesRecordsOnly;

	// Token: 0x0400332B RID: 13099
	[Tooltip("Only installed if the login is an owner of the address")]
	[DisableIf("alwaysInstalled")]
	public bool onlyIfOwner;

	// Token: 0x0400332C RID: 13100
	[ReorderableList]
	[DisableIf("alwaysInstalled")]
	public List<CruncherAppPreset.AppAccess> installationConditions = new List<CruncherAppPreset.AppAccess>();

	// Token: 0x0400332D RID: 13101
	[DisableIf("alwaysInstalled")]
	public List<AddressPreset> onlyInAddresses = new List<AddressPreset>();

	// Token: 0x0400332E RID: 13102
	[DisableIf("alwaysInstalled")]
	public bool onlyIfResidential;

	// Token: 0x0400332F RID: 13103
	[Header("Content")]
	[ReorderableList]
	public List<GameObject> appContent = new List<GameObject>();

	// Token: 0x04003330 RID: 13104
	[Tooltip("Played when the app is started")]
	[Header("Audio")]
	public AudioEvent onStartSound;

	// Token: 0x04003331 RID: 13105
	[Tooltip("Played when the app is ended")]
	public AudioEvent onExitSound;

	// Token: 0x04003332 RID: 13106
	[Tooltip("Played when the app has finished loading")]
	public AudioEvent onFinishedLoadingSound;

	// Token: 0x04003333 RID: 13107
	[Header("On Exit")]
	[Tooltip("Open this app on end")]
	public CruncherAppPreset openOnEnd;

	// Token: 0x020006F7 RID: 1783
	[Serializable]
	public class AppAccess
	{
		// Token: 0x04003334 RID: 13108
		public CharacterTrait.RuleType rule;

		// Token: 0x04003335 RID: 13109
		public List<CharacterTrait> traitList = new List<CharacterTrait>();
	}
}
