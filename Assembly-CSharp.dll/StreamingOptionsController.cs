using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.CompilerServices;
using NaughtyAttributes;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000492 RID: 1170
public class StreamingOptionsController : MonoBehaviour
{
	// Token: 0x170000AE RID: 174
	// (get) Token: 0x06001903 RID: 6403 RVA: 0x0017279F File Offset: 0x0017099F
	public static StreamingOptionsController Instance
	{
		get
		{
			return StreamingOptionsController._instance;
		}
	}

	// Token: 0x06001904 RID: 6404 RVA: 0x001727A6 File Offset: 0x001709A6
	private void Awake()
	{
		if (StreamingOptionsController._instance != null && StreamingOptionsController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		StreamingOptionsController._instance = this;
	}

	// Token: 0x06001905 RID: 6405 RVA: 0x001727D4 File Offset: 0x001709D4
	private void Start()
	{
		this.OnAuthChange();
		this.audienceData = new TwitchAudienceData();
		base.StartCoroutine(this.GrabKnownOnlineBots());
	}

	// Token: 0x06001906 RID: 6406 RVA: 0x001727F4 File Offset: 0x001709F4
	private void OnDisable()
	{
		base.StopAllCoroutines();
	}

	// Token: 0x06001907 RID: 6407 RVA: 0x001727FC File Offset: 0x001709FC
	private void Update()
	{
		if (TwitchOAuthController.Instance._hasAuth != this._hasAuth)
		{
			this._hasAuth = TwitchOAuthController.Instance._hasAuth;
			this.OnAuthChange();
		}
		if (!TwitchOAuthController.Instance._hasAuth)
		{
			return;
		}
		if (this.enableTwitchAudienceCitizens)
		{
			if (this.autoUpdateTime >= (float)this.updateFrequency)
			{
				this.UpdateTwitchCitizens();
				this.autoUpdateTime = 0f;
				return;
			}
			this.autoUpdateTime += Time.deltaTime;
		}
	}

	// Token: 0x06001908 RID: 6408 RVA: 0x0017287C File Offset: 0x00170A7C
	public void OnConnectButton()
	{
		this.connectToTwitchButton.SetInteractable(false);
		if (!TwitchOAuthController.Instance._hasAuth)
		{
			StreamingOptionsController.Instance.SetStatusText(Strings.Get("ui.interface", "Connecting", Strings.Casing.asIs, false, false, false, null));
			TwitchOAuthController.Instance.TryTwitchAuthorization();
			return;
		}
		this.ResetTwitchAuthFlushGeneratedData();
	}

	// Token: 0x06001909 RID: 6409 RVA: 0x001728D0 File Offset: 0x00170AD0
	public void OnAuthChange()
	{
		StreamingOptionsController.<OnAuthChange>d__35 <OnAuthChange>d__;
		<OnAuthChange>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<OnAuthChange>d__.<>4__this = this;
		<OnAuthChange>d__.<>1__state = -1;
		<OnAuthChange>d__.<>t__builder.Start<StreamingOptionsController.<OnAuthChange>d__35>(ref <OnAuthChange>d__);
	}

	// Token: 0x0600190A RID: 6410 RVA: 0x00172907 File Offset: 0x00170B07
	public void ResetTwitchAuthFlushGeneratedData()
	{
		TwitchOAuthController.Instance._hasAuth = false;
		this.grabbedAudience = false;
		this.audienceData = new TwitchAudienceData();
		this.customNames = new List<CitizenReplacement>();
		this.customNamesReserves = new List<CitizenReplacement>();
	}

	// Token: 0x0600190B RID: 6411 RVA: 0x0017293C File Offset: 0x00170B3C
	private void ForceTwitchAudienceCitizensToOff()
	{
		try
		{
			PlayerPrefs.SetInt("twitchAudienceCitizens", 0);
			PlayerPrefsController.Instance.gameSettingControls.Find((PlayerPrefsController.GameSetting item) => item.identifier == "twitchAudienceCitizens").intValue = 0;
			this.enableTwitchAudienceToggle.SetOff();
			PlayerPrefsController.Instance.OnToggleChanged("twitchAudienceCitizens", false, null);
		}
		catch
		{
		}
	}

	// Token: 0x0600190C RID: 6412 RVA: 0x001729BC File Offset: 0x00170BBC
	public void SetEnableTwitchAudienceCitizens(bool val)
	{
		this.enableTwitchAudienceCitizens = val;
		Game.Log("Streaming: Enable twitch audience citizens: " + this.enableTwitchAudienceCitizens.ToString(), 2);
		if (this.enableTwitchAudienceCitizens)
		{
			this.autoUpdateTime = (float)this.updateFrequency - 0.25f;
			if (this.grabbedAudience)
			{
				this.citizenUpdateText.text = Strings.Get("ui.interface", "Last updated with audience names: ", Strings.Casing.asIs, false, false, false, null) + this.customNames.Count.ToString();
			}
			else
			{
				this.citizenUpdateText.text = Strings.Get("ui.interface", "Press the update button to gather Twitch audience names...", Strings.Casing.asIs, false, false, false, null);
			}
		}
		else
		{
			this.autoUpdateTime = 0f;
			this.citizenUpdateText.text = Strings.Get("ui.interface", "Audience citizen names disabled", Strings.Casing.asIs, false, false, false, null);
		}
		if (SessionData.Instance.startedGame)
		{
			CityData.Instance.CreateCityDirectory();
		}
	}

	// Token: 0x0600190D RID: 6413 RVA: 0x00172AA8 File Offset: 0x00170CA8
	public void SetUpdateFrequency(int val)
	{
		this.updateFrequency = val;
	}

	// Token: 0x0600190E RID: 6414 RVA: 0x00172AB4 File Offset: 0x00170CB4
	public void UpdateTwitchCitizens()
	{
		StreamingOptionsController.<UpdateTwitchCitizens>d__40 <UpdateTwitchCitizens>d__;
		<UpdateTwitchCitizens>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<UpdateTwitchCitizens>d__.<>4__this = this;
		<UpdateTwitchCitizens>d__.<>1__state = -1;
		<UpdateTwitchCitizens>d__.<>t__builder.Start<StreamingOptionsController.<UpdateTwitchCitizens>d__40>(ref <UpdateTwitchCitizens>d__);
	}

	// Token: 0x0600190F RID: 6415 RVA: 0x00172AEC File Offset: 0x00170CEC
	private void AddUsersDataToNamePool(TwitchRootObject userData)
	{
		foreach (TwitchUserData twitchUserData in userData.data)
		{
			this.namePool.Add(twitchUserData.user_name);
		}
	}

	// Token: 0x06001910 RID: 6416 RVA: 0x00172B4C File Offset: 0x00170D4C
	private void FinalizeNamePool()
	{
		this.AddUsersDataToNamePool(this.audienceData.chattersNew);
		this.AddUsersDataToNamePool(this.audienceData.moderatorsNew);
		this.AddUsersDataToNamePool(this.audienceData.vipsNew);
		List<string> list = Enumerable.ToList<string>(Enumerable.Except<string>(this.namePool, this.activeKnownBots, StringComparer.OrdinalIgnoreCase));
		this.namePool = list;
	}

	// Token: 0x06001911 RID: 6417 RVA: 0x00172BB0 File Offset: 0x00170DB0
	private void ProcessNamePool()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		if (this.audienceData != null)
		{
			num = this.namePool.Count;
			while (this.customNames.Count < this.maxListCount && this.namePool.Count > 0)
			{
				int num4 = Toolbox.Instance.Rand(0, this.namePool.Count, false);
				this.TryAddCustomName(this.namePool[num4]);
				this.namePool.RemoveAt(num4);
				if (this.namePool.Count <= 0)
				{
					break;
				}
			}
			num2 = this.customNames.Count;
			while (this.customNames.Count < this.maxListCount && this.customNamesReserves.Count > 0)
			{
				int num5 = Toolbox.Instance.Rand(0, this.customNamesReserves.Count, false);
				this.customNames.Add(this.customNamesReserves[num5]);
				this.customNamesReserves.RemoveAt(num5);
				num3++;
			}
			this.customNamesReserves = new List<CitizenReplacement>();
		}
		this.grabbedAudience = true;
		this.grabbingAudenceInProgress = false;
		this.autoUpdateTime = 0f;
		Game.Log(string.Concat(new string[]
		{
			"Streaming: Created name list of ",
			this.customNames.Count.ToString(),
			" with ",
			num2.ToString(),
			" successful full names and ",
			num3.ToString(),
			" reserves"
		}), 2);
		if (this.enableTwitchAudienceCitizens)
		{
			if (SessionData.Instance.startedGame)
			{
				CityData.Instance.CreateCityDirectory();
			}
			this.citizenUpdateText.text = string.Concat(new string[]
			{
				Strings.Get("ui.interface", "Last updated with audience names: ", Strings.Casing.asIs, false, false, false, null),
				this.customNames.Count.ToString(),
				"/",
				num.ToString(),
				" "
			});
		}
	}

	// Token: 0x06001912 RID: 6418 RVA: 0x00172DB0 File Offset: 0x00170FB0
	public UniTask<bool> ValidateTokenUpdated()
	{
		StreamingOptionsController.<ValidateTokenUpdated>d__44 <ValidateTokenUpdated>d__;
		<ValidateTokenUpdated>d__.<>t__builder = AsyncUniTaskMethodBuilder<bool>.Create();
		<ValidateTokenUpdated>d__.<>4__this = this;
		<ValidateTokenUpdated>d__.<>1__state = -1;
		<ValidateTokenUpdated>d__.<>t__builder.Start<StreamingOptionsController.<ValidateTokenUpdated>d__44>(ref <ValidateTokenUpdated>d__);
		return <ValidateTokenUpdated>d__.<>t__builder.Task;
	}

	// Token: 0x06001913 RID: 6419 RVA: 0x00172DF3 File Offset: 0x00170FF3
	public void SetStatusText(string newText)
	{
		Game.Log("Streaming: Change status text: " + newText, 2);
		this.twitchConnectStatusText.text = newText;
	}

	// Token: 0x06001914 RID: 6420 RVA: 0x00172E14 File Offset: 0x00171014
	public UniTask GetChattersUpdated()
	{
		StreamingOptionsController.<GetChattersUpdated>d__46 <GetChattersUpdated>d__;
		<GetChattersUpdated>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GetChattersUpdated>d__.<>4__this = this;
		<GetChattersUpdated>d__.<>1__state = -1;
		<GetChattersUpdated>d__.<>t__builder.Start<StreamingOptionsController.<GetChattersUpdated>d__46>(ref <GetChattersUpdated>d__);
		return <GetChattersUpdated>d__.<>t__builder.Task;
	}

	// Token: 0x06001915 RID: 6421 RVA: 0x00172E58 File Offset: 0x00171058
	public UniTask GetModeratorsUpdated()
	{
		StreamingOptionsController.<GetModeratorsUpdated>d__47 <GetModeratorsUpdated>d__;
		<GetModeratorsUpdated>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GetModeratorsUpdated>d__.<>4__this = this;
		<GetModeratorsUpdated>d__.<>1__state = -1;
		<GetModeratorsUpdated>d__.<>t__builder.Start<StreamingOptionsController.<GetModeratorsUpdated>d__47>(ref <GetModeratorsUpdated>d__);
		return <GetModeratorsUpdated>d__.<>t__builder.Task;
	}

	// Token: 0x06001916 RID: 6422 RVA: 0x00172E9C File Offset: 0x0017109C
	public UniTask GetVipsUpdated()
	{
		StreamingOptionsController.<GetVipsUpdated>d__48 <GetVipsUpdated>d__;
		<GetVipsUpdated>d__.<>t__builder = AsyncUniTaskMethodBuilder.Create();
		<GetVipsUpdated>d__.<>4__this = this;
		<GetVipsUpdated>d__.<>1__state = -1;
		<GetVipsUpdated>d__.<>t__builder.Start<StreamingOptionsController.<GetVipsUpdated>d__48>(ref <GetVipsUpdated>d__);
		return <GetVipsUpdated>d__.<>t__builder.Task;
	}

	// Token: 0x06001917 RID: 6423 RVA: 0x00172EDF File Offset: 0x001710DF
	private IEnumerator GrabKnownOnlineBots()
	{
		string text = "https://api.twitchinsights.net/v1/bots/online";
		using (UnityWebRequest webRequest = UnityWebRequest.Get(text))
		{
			yield return webRequest.SendWebRequest();
			if (webRequest.error == null)
			{
				using (IEnumerator<JToken> enumerator = JObject.Parse(webRequest.downloadHandler.text)["bots"].GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						JToken jtoken = enumerator.Current;
						this.activeKnownBots.Add(jtoken[0].ToString());
					}
					goto IL_E7;
				}
			}
			Game.Log(webRequest.error, 2);
			IL_E7:;
		}
		UnityWebRequest webRequest = null;
		yield break;
		yield break;
	}

	// Token: 0x06001918 RID: 6424 RVA: 0x00172EF0 File Offset: 0x001710F0
	private bool TryAddCustomName(string input)
	{
		string text = input.Trim();
		string text2 = string.Empty;
		string text3 = string.Empty;
		input = Regex.Replace(input, "[\\d-]", "_", 8);
		input = string.Join("_", Regex.Split(input, "(?<!^)(?=[A-Z](?![A-Z]|$))"));
		string[] array = input.Split(new char[]
		{
			' ',
			'_',
			'.',
			',',
			'-'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string text4 = array[i].Trim();
			if (!(text4.ToLower() == "the"))
			{
				if (text2.Length <= 0)
				{
					if (text4.Length >= 2)
					{
						text2 = text4;
					}
				}
				else if (text3.Length <= 0 && text4.Length >= 2)
				{
					text3 = text4;
				}
				if (text2.Length >= 2 && text3.Length >= 2)
				{
					break;
				}
			}
		}
		int num;
		if (text2.Length > 0 && text3.Length > 0)
		{
			num = 2;
		}
		else
		{
			if (text2.Length <= 0 && text3.Length <= 0)
			{
				return false;
			}
			num = 1;
		}
		text2 = Strings.ApplyCasing(text2, Strings.Casing.firstLetterCaptial);
		text3 = Strings.ApplyCasing(text3, Strings.Casing.firstLetterCaptial);
		if (num == 2 && this.customNames.Count < this.maxListCount)
		{
			CitizenReplacement newRep = new CitizenReplacement();
			newRep.name = text2 + " " + text3;
			newRep.firstName = text2;
			newRep.surName = text3;
			if (Enumerable.ToList<CitizenReplacement>(Enumerable.Where<CitizenReplacement>(this.customNames, (CitizenReplacement p) => newRep.firstName == p.firstName && newRep.surName == p.surName)).Count <= 0)
			{
				Game.Log(string.Concat(new string[]
				{
					"Streaming: Writing parsed ",
					text,
					" = ",
					newRep.name,
					" to StreamerAudience list..."
				}), 2);
				this.customNames.Add(newRep);
			}
		}
		else if (num == 1 && this.customNamesReserves.Count < this.maxListCount)
		{
			CitizenReplacement newRep = new CitizenReplacement();
			if (text2.Length > 0 && text3.Length <= 0)
			{
				newRep.surName = text2;
			}
			else if (text3.Length > 0 && text2.Length <= 0)
			{
				newRep.surName = text3;
			}
			newRep.name = text2 + " " + text3;
			if (Enumerable.ToList<CitizenReplacement>(Enumerable.Where<CitizenReplacement>(this.customNamesReserves, (CitizenReplacement p) => newRep.firstName == p.firstName && newRep.surName == p.surName)).Count <= 0 && Enumerable.ToList<CitizenReplacement>(Enumerable.Where<CitizenReplacement>(this.customNames, (CitizenReplacement p) => newRep.firstName == p.firstName && newRep.surName == p.surName)).Count <= 0)
			{
				Game.Log(string.Concat(new string[]
				{
					"Streaming: Writing parsed ",
					text,
					" = ",
					newRep.name,
					" to StreamerAudienceReserves list..."
				}), 2);
				this.customNamesReserves.Add(newRep);
			}
		}
		return true;
	}

	// Token: 0x06001919 RID: 6425 RVA: 0x00173204 File Offset: 0x00171404
	[Button(null, 0)]
	public void ParseNamesFromNameList()
	{
		if (this.customNameList != null)
		{
			this.namePool.Clear();
			foreach (string text in this.customNameList.text.Split('\n', 0))
			{
				this.namePool.Add(text);
			}
			this.ProcessNamePool();
		}
	}

	// Token: 0x040021A8 RID: 8616
	[Header("Settings")]
	public bool enableTwitchAudienceCitizens;

	// Token: 0x040021A9 RID: 8617
	public int updateFrequency = 1600;

	// Token: 0x040021AA RID: 8618
	public int maxListCount = 2000;

	// Token: 0x040021AB RID: 8619
	[Header("Components")]
	public TextMeshProUGUI twitchConnectStatusText;

	// Token: 0x040021AC RID: 8620
	public ButtonController connectToTwitchButton;

	// Token: 0x040021AD RID: 8621
	public ToggleController enableTwitchAudienceToggle;

	// Token: 0x040021AE RID: 8622
	public TextMeshProUGUI citizenUpdateText;

	// Token: 0x040021AF RID: 8623
	public List<ButtonController> disabledIfNoConnection = new List<ButtonController>();

	// Token: 0x040021B0 RID: 8624
	[Header("State")]
	public bool grabbedAudience;

	// Token: 0x040021B1 RID: 8625
	public bool grabbingAudenceInProgress;

	// Token: 0x040021B2 RID: 8626
	public bool loginNameSet;

	// Token: 0x040021B3 RID: 8627
	public TwitchAudienceData audienceData;

	// Token: 0x040021B4 RID: 8628
	public float autoUpdateTime;

	// Token: 0x040021B5 RID: 8629
	public List<CitizenReplacement> customNames = new List<CitizenReplacement>();

	// Token: 0x040021B6 RID: 8630
	public List<CitizenReplacement> customNamesReserves = new List<CitizenReplacement>();

	// Token: 0x040021B7 RID: 8631
	private bool _hasAuth;

	// Token: 0x040021B8 RID: 8632
	private bool _hasValidToken;

	// Token: 0x040021B9 RID: 8633
	private bool _fetchingDataInProgress;

	// Token: 0x040021BA RID: 8634
	private List<string> namePool = new List<string>();

	// Token: 0x040021BB RID: 8635
	private List<string> activeKnownBots = new List<string>();

	// Token: 0x040021BC RID: 8636
	private HashSet<string> finalNamePool = new HashSet<string>();

	// Token: 0x040021BD RID: 8637
	private const string twitchValidationEndpoint = "https://id.twitch.tv/oauth2/validate";

	// Token: 0x040021BE RID: 8638
	private const string twitchChatterEndpoint = "https://api.twitch.tv/helix/chat/chatters?broadcaster_id=";

	// Token: 0x040021BF RID: 8639
	private const string twitchModeratorEndpoint = "https://api.twitch.tv/helix/moderation/moderators?broadcaster_id=";

	// Token: 0x040021C0 RID: 8640
	private const string twitchVipEndpoint = "https://api.twitch.tv/helix/channels/vips?broadcaster_id=";

	// Token: 0x040021C1 RID: 8641
	private const string knownBotsEndpoints = "https://api.twitchinsights.net/v1/bots/online";

	// Token: 0x040021C2 RID: 8642
	[Header("Custom Name List")]
	[InfoBox("Can be used for debugging to test the name parsing functionality", 0)]
	public TextAsset customNameList;

	// Token: 0x040021C3 RID: 8643
	private static StreamingOptionsController _instance;
}
