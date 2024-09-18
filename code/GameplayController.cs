using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200067A RID: 1658
public class GameplayController : MonoBehaviour
{
	// Token: 0x1400005E RID: 94
	// (add) Token: 0x06002461 RID: 9313 RVA: 0x001DEEB8 File Offset: 0x001DD0B8
	// (remove) Token: 0x06002462 RID: 9314 RVA: 0x001DEEF0 File Offset: 0x001DD0F0
	public event GameplayController.MatchesChange OnMatchesChanged;

	// Token: 0x1400005F RID: 95
	// (add) Token: 0x06002463 RID: 9315 RVA: 0x001DEF28 File Offset: 0x001DD128
	// (remove) Token: 0x06002464 RID: 9316 RVA: 0x001DEF60 File Offset: 0x001DD160
	public event GameplayController.NewEvidenceHistory OnNewEvidenceHistory;

	// Token: 0x14000060 RID: 96
	// (add) Token: 0x06002465 RID: 9317 RVA: 0x001DEF98 File Offset: 0x001DD198
	// (remove) Token: 0x06002466 RID: 9318 RVA: 0x001DEFD0 File Offset: 0x001DD1D0
	public event GameplayController.NewPhoneData OnNewPhoneData;

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x06002467 RID: 9319 RVA: 0x001DF005 File Offset: 0x001DD205
	public static GameplayController Instance
	{
		get
		{
			return GameplayController._instance;
		}
	}

	// Token: 0x06002468 RID: 9320 RVA: 0x001DF00C File Offset: 0x001DD20C
	private void Awake()
	{
		if (GameplayController._instance != null && GameplayController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			GameplayController._instance = this;
		}
		this.UpdateMatch = (Action)Delegate.Combine(this.UpdateMatch, new Action(this.UpdateMatches));
	}

	// Token: 0x06002469 RID: 9321 RVA: 0x001DF068 File Offset: 0x001DD268
	private void Start()
	{
		this.defaultItemButton = this.setDefaultItemButton;
	}

	// Token: 0x0600246A RID: 9322 RVA: 0x001DF076 File Offset: 0x001DD276
	public void DestroySelf()
	{
		Object.Destroy(base.gameObject);
		GameplayController._instance = null;
	}

	// Token: 0x0600246B RID: 9323 RVA: 0x001DF08C File Offset: 0x001DD28C
	public void UpdateConversationDelays()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, float> keyValuePair in this.globalConversationDelay)
		{
			if (SessionData.Instance.gameTime >= keyValuePair.Value + 1.25f)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (string text in list)
		{
			this.globalConversationDelay.Remove(text);
		}
	}

	// Token: 0x0600246C RID: 9324 RVA: 0x001DF14C File Offset: 0x001DD34C
	public void AddNewMatch(MatchPreset match, Evidence newEntry)
	{
		if (SessionData.Instance.startedGame)
		{
			Game.Log("Evidence: Add match: " + newEntry.name + " to " + match.name, 2);
		}
		if (!this.parentMatches.ContainsKey(match))
		{
			this.parentMatches.Add(match, new List<Evidence>());
		}
		this.parentMatches[match].Add(newEntry);
		if (!newEntry.isFound)
		{
			newEntry.OnDiscovered += this.OnDiscoverMatchEvidence;
		}
		else
		{
			this.UpdateMatchesEndOfFrame();
		}
		if (this.OnMatchesChanged != null)
		{
			this.OnMatchesChanged();
		}
	}

	// Token: 0x0600246D RID: 9325 RVA: 0x001DF1EC File Offset: 0x001DD3EC
	private void OnDiscoverMatchEvidence(Evidence ev)
	{
		this.UpdateMatchesEndOfFrame();
		ev.OnDiscovered -= this.OnDiscoverMatchEvidence;
	}

	// Token: 0x0600246E RID: 9326 RVA: 0x001DF206 File Offset: 0x001DD406
	public void UpdateMatchesEndOfFrame()
	{
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		Toolbox.Instance.InvokeEndOfFrame(this.UpdateMatch, "Check matches");
	}

	// Token: 0x0600246F RID: 9327 RVA: 0x001DF22C File Offset: 0x001DD42C
	public void RemoveMatch(MatchPreset match, Evidence newEntry)
	{
		if (this.parentMatches.ContainsKey(match))
		{
			if (this.parentMatches[match].Contains(newEntry))
			{
				this.parentMatches[match].Remove(newEntry);
				if (this.parentMatches[match].Count <= 0)
				{
					this.parentMatches.Remove(match);
				}
				Toolbox.Instance.InvokeEndOfFrame(this.UpdateMatch, "Check matches");
			}
			if (this.OnMatchesChanged != null)
			{
				this.OnMatchesChanged();
			}
		}
	}

	// Token: 0x06002470 RID: 9328 RVA: 0x001DF2B8 File Offset: 0x001DD4B8
	public void UpdateMatches()
	{
		foreach (KeyValuePair<MatchPreset, List<Evidence>> keyValuePair in this.parentMatches)
		{
			MatchPreset key = keyValuePair.Key;
			if (!key.canOnlyBeMatchedWith)
			{
				Game.Log("Evidence: Update matches for " + key.name + "...", 2);
				MatchPreset matchPreset = key;
				List<Evidence> list = null;
				if (key.onlyMatchWithThis != null)
				{
					matchPreset = key.onlyMatchWithThis;
				}
				if (this.parentMatches.ContainsKey(key))
				{
					list = this.parentMatches[key].FindAll((Evidence item) => item.isFound);
				}
				List<Evidence> list2 = new List<Evidence>();
				if (this.parentMatches.ContainsKey(matchPreset))
				{
					list2 = this.parentMatches[matchPreset].FindAll((Evidence item) => item.isFound);
					if (key.onlyMatchWithThis == null)
					{
						list = list2;
					}
				}
				else
				{
					Game.Log("Evidence: ...ParentMatches does not contain key " + matchPreset.name, 2);
				}
				Game.Log(string.Concat(new string[]
				{
					"Evidence: ...Found ",
					list.Count.ToString(),
					">",
					list2.Count.ToString(),
					" entries in this category."
				}), 2);
				List<Evidence> list3 = new List<Evidence>();
				List<Evidence> list4 = new List<Evidence>();
				for (int i = 0; i < list.Count; i++)
				{
					Evidence evidence = list[i];
					for (int j = 0; j < list2.Count; j++)
					{
						Evidence evidence2 = list2[j];
						if ((key.canMatchWithItself || evidence != evidence2) && (!key.onlyMatchWithMatchParents || evidence.preset.isMatchParent || evidence2.preset.isMatchParent) && FactMatches.MatchCheck(key, evidence, evidence2))
						{
							bool flag = true;
							for (int k = 0; k < list3.Count; k++)
							{
								Evidence evidence3 = list3[k];
								if (evidence3 == evidence && list4[k] == evidence2)
								{
									flag = false;
									break;
								}
								if (evidence3 == evidence2 && list4[k] == evidence)
								{
									flag = false;
									break;
								}
							}
							if (flag)
							{
								list3.Add(evidence);
								list4.Add(evidence2);
							}
						}
					}
				}
				Game.Log("Evidence: Matches required for " + key.name + " = " + list3.Count.ToString(), 2);
				if (!this.matchesDetails.ContainsKey(key))
				{
					this.matchesDetails.Add(key, new List<FactMatches>());
				}
				for (int l = 0; l < this.matchesDetails[key].Count; l++)
				{
					FactMatches factMatches = this.matchesDetails[key][l];
					if (factMatches == null)
					{
						this.matchesDetails[key].RemoveAt(l);
						l--;
					}
					else if ((list3.Contains(factMatches.fromEvidence[0]) && list4.Contains(factMatches.toEvidence[0])) || (list3.Contains(factMatches.toEvidence[0]) && list4.Contains(factMatches.fromEvidence[0])))
					{
						list3.Remove(factMatches.fromEvidence[0]);
						list4.Remove(factMatches.toEvidence[0]);
					}
				}
				for (int m = 0; m < list3.Count; m++)
				{
					Game.Log("Evidence: Create remaining; " + list3[m].name + " to " + list4[m].name, 2);
					Evidence fromEvidenceSingular = list3[m];
					Evidence toEvidenceSingular = list4[m];
					Game.Log("Evidence: Spawn new match from " + list3[m].name + " to " + list4[m].name, 2);
					List<object> list5 = new List<object>();
					list5.Add(key);
					FactMatches factMatches2 = EvidenceCreator.Instance.CreateFact("Matches", fromEvidenceSingular, toEvidenceSingular, null, null, true, list5, key.linkFromKeys, key.linkToKeys, false) as FactMatches;
					if (factMatches2 != null)
					{
						this.matchesDetails[key].Add(factMatches2);
					}
					else
					{
						Game.Log("Evidence: New match was not created, it must exist already in reverse form", 2);
					}
				}
			}
		}
	}

	// Token: 0x06002471 RID: 9329 RVA: 0x001DF784 File Offset: 0x001DD984
	public void AddHistory(Evidence entry, List<Evidence.DataKey> keys)
	{
		if (entry == null)
		{
			return;
		}
		List<Evidence.DataKey> tiedKeys = entry.GetTiedKeys(keys);
		foreach (GameplayController.History history in this.history.FindAll((GameplayController.History item) => item.evID == entry.evID))
		{
			Evidence evidence = null;
			if (this.evidenceDictionary.TryGetValue(history.evID, ref evidence))
			{
				List<Evidence.DataKey> tiedKeys2 = evidence.GetTiedKeys(keys);
				foreach (Evidence.DataKey dataKey in tiedKeys)
				{
					if (tiedKeys2.Contains(dataKey))
					{
						Game.Log("Interface: " + history.evID + " already existings in history, ignoring...", 2);
						history.lastAccess = SessionData.Instance.gameTime;
						this.history.Sort((GameplayController.History p2, GameplayController.History p1) => p1.lastAccess.CompareTo(p2.lastAccess));
						this.itemOnlyHistory.Sort((GameplayController.History p2, GameplayController.History p1) => p1.lastAccess.CompareTo(p2.lastAccess));
						if (this.OnNewEvidenceHistory != null)
						{
							this.OnNewEvidenceHistory();
						}
						return;
					}
				}
			}
		}
		bool flag = false;
		if (entry.interactable != null && entry.interactable.preset.spawnable)
		{
			flag = true;
		}
		if (flag && this.itemOnlyHistory.Count >= InterfaceControls.Instance.maximumEvidenceItemHistory)
		{
			GameplayController.History history2 = this.itemOnlyHistory[this.itemOnlyHistory.Count - 1];
			this.history.Remove(history2);
			this.itemOnlyHistory.RemoveAt(this.itemOnlyHistory.Count - 1);
		}
		GameplayController.History history3 = new GameplayController.History();
		history3.evID = entry.evID;
		history3.keys = tiedKeys;
		history3.lastAccess = SessionData.Instance.gameTime;
		if (Player.Instance.currentGameLocation.thisAsAddress != null)
		{
			history3.locationID = Player.Instance.currentGameLocation.thisAsAddress.id;
		}
		this.history.Add(history3);
		if (flag)
		{
			this.itemOnlyHistory.Add(history3);
		}
		if (this.OnNewEvidenceHistory != null)
		{
			this.OnNewEvidenceHistory();
		}
		Game.Log(string.Concat(new string[]
		{
			"Interface: Added history: ",
			entry.evID,
			" total: ",
			this.history.Count.ToString(),
			" items: ",
			this.itemOnlyHistory.Count.ToString(),
			"/",
			InterfaceControls.Instance.maximumEvidenceItemHistory.ToString()
		}), 2);
	}

	// Token: 0x06002472 RID: 9330 RVA: 0x001DFAC0 File Offset: 0x001DDCC0
	public void AddMoney(int addVal, bool displayMessage, string reason)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (addVal == 0)
		{
			return;
		}
		this.money += addVal;
		this.money = Mathf.Max(this.money, 0);
		Game.Log("Gameplay: Add money: " + addVal.ToString(), 2);
		if (displayMessage && addVal != 0)
		{
			string text = string.Empty;
			if (addVal >= 0)
			{
				text = "+";
			}
			SessionData.Instance.TutorialTrigger("money", false);
			InterfaceController instance = InterfaceController.Instance;
			InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
			int newNumerical = 0;
			string newMessage = text + CityControls.Instance.cityCurrency + addVal.ToString();
			InterfaceControls.Icon newIcon = InterfaceControls.Icon.money;
			AudioEvent additionalSFX = null;
			bool colourOverride = false;
			RectTransform moneyNotificationIcon = InterfaceController.Instance.moneyNotificationIcon;
			instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, moneyNotificationIcon, GameMessageController.PingOnComplete.money, null, null, null);
		}
		FirstPersonItemController.Instance.PlayerMoneyCheck();
		if (InterfaceControls.Instance.cashText != null)
		{
			InterfaceControls.Instance.cashText.text = CityControls.Instance.cityCurrency + this.money.ToString();
			if (BioScreenController.Instance.cashText != null)
			{
				BioScreenController.Instance.cashText.text = InterfaceControls.Instance.cashText.text;
			}
		}
	}

	// Token: 0x06002473 RID: 9331 RVA: 0x001DFC04 File Offset: 0x001DDE04
	public void SetMoney(int newVal)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		this.money = Mathf.Max(newVal, 0);
		InterfaceController.Instance.lastMoney = this.money;
		Game.Log("Gameplay: Set money: " + this.money.ToString(), 2);
		FirstPersonItemController.Instance.PlayerMoneyCheck();
		if (InterfaceControls.Instance.cashText != null)
		{
			InterfaceControls.Instance.cashText.text = CityControls.Instance.cityCurrency + this.money.ToString();
			if (BioScreenController.Instance.cashText != null)
			{
				BioScreenController.Instance.cashText.text = InterfaceControls.Instance.cashText.text;
			}
		}
	}

	// Token: 0x06002474 RID: 9332 RVA: 0x001DFCD8 File Offset: 0x001DDED8
	public void AddSocialCredit(int addVal, bool displayMessage, string reason)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (addVal == 0)
		{
			return;
		}
		int num = this.socialCredit;
		this.socialCredit += addVal;
		this.socialCredit = Mathf.Max(this.socialCredit, 0);
		Game.Log("Gameplay: Add social credit: " + this.socialCredit.ToString(), 2);
		if (displayMessage && addVal != 0)
		{
			string empty = string.Empty;
			InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, addVal, Strings.Get("ui.gamemessage", "Social credit", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.resolve, AudioControls.Instance.gainSocialCredit, false, default(Color), -1, 3f, null, GameMessageController.PingOnComplete.none, null, null, null);
		}
		if (addVal > 0 && displayMessage)
		{
			SessionData.Instance.TutorialTrigger("socialcredit", false);
		}
		BioScreenController.Instance.OnChangePoints(displayMessage);
	}

	// Token: 0x06002475 RID: 9333 RVA: 0x001DFDBC File Offset: 0x001DDFBC
	public void SetSocialCredit(int newVal)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		this.socialCredit = Mathf.Max(newVal, 0);
		Game.Log("Gameplay: Set social credit: " + this.socialCredit.ToString(), 2);
		BioScreenController.Instance.OnChangePoints(false);
	}

	// Token: 0x06002476 RID: 9334 RVA: 0x001DFE16 File Offset: 0x001DE016
	public int GetCurrentSocialCreditLevel()
	{
		return this.GetSocialCreditLevel(this.socialCredit);
	}

	// Token: 0x06002477 RID: 9335 RVA: 0x001DFE24 File Offset: 0x001DE024
	public int GetNextSocialCreditLevelThreshold()
	{
		return this.GetSocialCreditThresholdForLevel(this.GetCurrentSocialCreditLevel() + 1);
	}

	// Token: 0x06002478 RID: 9336 RVA: 0x001DFE34 File Offset: 0x001DE034
	public int GetSocialCreditLevel(int points)
	{
		int num = 1;
		int i = points;
		while (i > 0)
		{
			i -= Mathf.RoundToInt(GameplayControls.Instance.socialCreditLevelCurve.Evaluate((float)(num + 1)));
			if (i < 0)
			{
				break;
			}
			num++;
		}
		int result;
		try
		{
			result = Mathf.Min(num, Game.Instance.gameLengthMaxLevels[Game.Instance.gameLength]);
		}
		catch
		{
			result = num;
		}
		return result;
	}

	// Token: 0x06002479 RID: 9337 RVA: 0x001DFEA8 File Offset: 0x001DE0A8
	public int GetSocialCreditThreshold(int points)
	{
		return this.GetSocialCreditThresholdForLevel(this.GetSocialCreditLevel(points));
	}

	// Token: 0x0600247A RID: 9338 RVA: 0x001DFEB8 File Offset: 0x001DE0B8
	public int GetSocialCreditThresholdForLevel(int level)
	{
		int num = 0;
		while (level > 0)
		{
			num += Mathf.RoundToInt(GameplayControls.Instance.socialCreditLevelCurve.Evaluate((float)level));
			level--;
		}
		return num;
	}

	// Token: 0x0600247B RID: 9339 RVA: 0x001DFEEC File Offset: 0x001DE0EC
	public void AddLockpicks(int addVal, bool displayMessage)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		if (addVal == 0)
		{
			return;
		}
		this.lockPicks += addVal;
		if (displayMessage)
		{
			string text = string.Empty;
			if (addVal > 0)
			{
				text = "+";
			}
			InterfaceController instance = InterfaceController.Instance;
			InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
			int newNumerical = 0;
			string newMessage = text + addVal.ToString() + " " + Strings.Get("ui.gamemessage", "lockpicks", Strings.Casing.asIs, false, false, false, null);
			InterfaceControls.Icon newIcon = InterfaceControls.Icon.lockpick;
			AudioEvent additionalSFX = null;
			bool colourOverride = false;
			RectTransform lockpicksNotificationIcon = InterfaceController.Instance.lockpicksNotificationIcon;
			instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, lockpicksNotificationIcon, GameMessageController.PingOnComplete.lockpicks, null, null, null);
		}
		InterfaceControls.Instance.lockpicksText.text = this.lockPicks.ToString();
	}

	// Token: 0x0600247C RID: 9340 RVA: 0x001DFFA8 File Offset: 0x001DE1A8
	public void SetLockpicks(int newVal)
	{
		if (SessionData.Instance.isFloorEdit)
		{
			return;
		}
		if (SessionData.Instance.isDialogEdit)
		{
			return;
		}
		this.lockPicks = newVal;
		InterfaceController.Instance.lastLockpicks = this.lockPicks;
		InterfaceControls.Instance.lockpicksText.text = this.lockPicks.ToString();
	}

	// Token: 0x0600247D RID: 9341 RVA: 0x001E0000 File Offset: 0x001DE200
	public void UseLockpick(float val)
	{
		this.currentLockpickStrength -= val / Mathf.LerpUnclamped(GameplayControls.Instance.lockpickEffectivenessRange.x, GameplayControls.Instance.lockpickEffectivenessRange.y, UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.lockpickingEfficiencyModifier));
		this.currentLockpickStrength = Mathf.Clamp01(this.currentLockpickStrength);
		if (this.currentLockpickStrength <= 0f)
		{
			this.DepleteLockpick();
		}
	}

	// Token: 0x0600247E RID: 9342 RVA: 0x001E006F File Offset: 0x001DE26F
	public void DepleteLockpick()
	{
		this.AddLockpicks(-1, false);
		this.currentLockpickStrength = 1f;
	}

	// Token: 0x0600247F RID: 9343 RVA: 0x001E0084 File Offset: 0x001DE284
	public void AddGuestPass(NewAddress loc, float forHours)
	{
		if (loc == null)
		{
			return;
		}
		if (!this.guestPasses.ContainsKey(loc))
		{
			this.guestPasses.Add(loc, Vector2.zero);
		}
		loc.ResetLoiteringTimer();
		this.guestPasses[loc] = new Vector2(SessionData.Instance.gameTime + forHours, forHours);
		Game.Log(string.Concat(new string[]
		{
			"Player: Adding guest pass for ",
			loc.name,
			" for ",
			forHours.ToString(),
			" hours"
		}), 2);
		if (Player.Instance.currentRoom.gameLocation == loc)
		{
			Player.Instance.OnRoomChange();
		}
	}

	// Token: 0x06002480 RID: 9344 RVA: 0x001E013C File Offset: 0x001DE33C
	public void AddGuestPass(NewAddress loc, Vector2 directData)
	{
		if (loc == null)
		{
			return;
		}
		if (!this.guestPasses.ContainsKey(loc))
		{
			this.guestPasses.Add(loc, Vector2.zero);
		}
		loc.ResetLoiteringTimer();
		this.guestPasses[loc] = directData;
		Game.Log("Player: Adding guest pass for " + loc.name, 2);
		if (Player.Instance.currentRoom.gameLocation == loc)
		{
			Player.Instance.OnRoomChange();
		}
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x001E01BC File Offset: 0x001DE3BC
	public void CallEnforcers(NewGameLocation newLocation, bool forceCrimeScene = false, bool immediateTeleport = false)
	{
		if (newLocation == null)
		{
			return;
		}
		if (!this.enforcerCalls.ContainsKey(newLocation))
		{
			GameplayController.EnforcerCall enforcerCall = new GameplayController.EnforcerCall();
			enforcerCall.isCrimeScene = forceCrimeScene;
			enforcerCall.immedaiteTeleport = immediateTeleport;
			if (newLocation.thisAsAddress != null)
			{
				enforcerCall.isStreet = false;
				enforcerCall.id = newLocation.thisAsAddress.id;
				newLocation.thisAsAddress.floor.SetAlarmLockdown(true, null);
			}
			else
			{
				if (!(newLocation.thisAsStreet != null))
				{
					return;
				}
				enforcerCall.isStreet = true;
				enforcerCall.id = newLocation.thisAsStreet.streetID;
			}
			enforcerCall.logTime = SessionData.Instance.gameTime;
			Game.Log("Gameplay: New enforcer call added at " + newLocation.name, 2);
			this.enforcerCalls.Add(newLocation, enforcerCall);
			if (InteractionController.Instance.dialogMode)
			{
				base.StartCoroutine(this.WaitForEndCall(newLocation));
				return;
			}
			this.NewMurderCaseNotify(newLocation);
		}
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x001E02B0 File Offset: 0x001DE4B0
	private IEnumerator WaitForEndCall(NewGameLocation newLocation)
	{
		while (InteractionController.Instance.dialogMode)
		{
			yield return null;
		}
		this.NewMurderCaseNotify(newLocation);
		yield break;
	}

	// Token: 0x06002483 RID: 9347 RVA: 0x001E02C8 File Offset: 0x001DE4C8
	private void NewMurderCaseNotify(NewGameLocation newLocation)
	{
		using (List<MurderController.Murder>.Enumerator enumerator = MurderController.Instance.activeMurders.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				MurderController.Murder m = enumerator.Current;
				if ((Game.Instance.sandboxMode || (ChapterController.Instance != null && ChapterController.Instance.chapterScript as ChapterIntro != null && (ChapterController.Instance.chapterScript as ChapterIntro).completed)) && newLocation.currentOccupants.Exists((Actor item) => item == m.victim && item.isDead))
				{
					InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "New Enforcer Call", Strings.Casing.asIs, false, false, false, null) + ": " + newLocation.name, InterfaceControls.Icon.skull, AudioControls.Instance.enforcerScannerMsg, false, default(Color), -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
					MurderController.Instance.OnVictimDiscovery();
					if (MurderController.Instance.currentActiveCase != null)
					{
						Game.Log("Murder: Adding next crime scene objective...", 2);
						Objective.ObjectiveTrigger trigger = new Objective.ObjectiveTrigger(Objective.ObjectiveTriggerType.exploreCrimeScene, "", false, 0f, null, null, null, null, null, newLocation, null, "", false, default(Vector3));
						string entryRef = Strings.Get("missions.postings", "Explore Reported Crime Scene", Strings.Casing.asIs, false, false, false, null) + newLocation.name;
						MurderController.Instance.currentActiveCase.AddObjective(entryRef, trigger, false, default(Vector3), InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction.nothing, 0f, false, "", false, false, null, false, true, false);
					}
				}
			}
		}
	}

	// Token: 0x06002484 RID: 9348 RVA: 0x001E0490 File Offset: 0x001DE690
	public void AddPasscode(GameplayController.Passcode newCode, bool displayMessage = true)
	{
		if (newCode == null || newCode.GetDigits().Count < 4)
		{
			return;
		}
		if (!this.acquiredPasscodes.Contains(newCode))
		{
			this.acquiredPasscodes.Add(newCode);
			if (displayMessage)
			{
				string text = string.Empty;
				if (newCode.type == GameplayController.PasscodeType.room)
				{
					NewRoom newRoom = null;
					if (CityData.Instance.roomDictionary.TryGetValue(newCode.id, ref newRoom))
					{
						text = newRoom.GetName() + ", " + newRoom.gameLocation.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
					}
				}
				else if (newCode.type == GameplayController.PasscodeType.address)
				{
					NewAddress newAddress = null;
					if (CityData.Instance.addressDictionary.TryGetValue(newCode.id, ref newAddress))
					{
						text = newAddress.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
					}
				}
				else if (newCode.type == GameplayController.PasscodeType.citizen)
				{
					Human human = null;
					if (CityData.Instance.GetHuman(newCode.id, out human, true))
					{
						text = human.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
					}
				}
				else if (newCode.type == GameplayController.PasscodeType.interactable)
				{
					Interactable interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == newCode.id);
					if (interactable != null)
					{
						text = interactable.GetName() + ", " + interactable.node.gameLocation.evidenceEntry.GetNameForDataKey(Evidence.DataKey.name);
					}
				}
				text = string.Concat(new string[]
				{
					text,
					" ",
					newCode.GetDigit(0).ToString(),
					newCode.GetDigit(1).ToString(),
					newCode.GetDigit(2).ToString(),
					newCode.GetDigit(3).ToString()
				});
				InterfaceController instance = InterfaceController.Instance;
				InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
				int newNumerical = 0;
				string newMessage = Strings.Get("ui.gamemessage", "passcode", Strings.Casing.asIs, false, false, false, null) + ": " + text;
				InterfaceControls.Icon newIcon = InterfaceControls.Icon.agent;
				AudioEvent additionalSFX = null;
				bool colourOverride = false;
				RectTransform notebookNotificationIcon = InterfaceController.Instance.notebookNotificationIcon;
				instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, notebookNotificationIcon, GameMessageController.PingOnComplete.none, null, null, null);
			}
			int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.moneyForPasscodes));
			if (num > 0)
			{
				GameplayController.Instance.AddMoney(num, true, "moneyforpasscodes");
			}
		}
	}

	// Token: 0x06002485 RID: 9349 RVA: 0x001E0718 File Offset: 0x001DE918
	public void AddOrMergePhoneNumberData(int newNumber, bool knowLocation, List<Human> knowCitizens, string textOverride = "", bool displayMessage = true)
	{
		List<int> list = new List<int>();
		if (knowCitizens != null)
		{
			foreach (Human human in knowCitizens)
			{
				list.Add(human.humanID);
			}
		}
		GameplayController.PhoneNumber phoneNumber = this.acquiredNumbers.Find((GameplayController.PhoneNumber item) => item.number == newNumber);
		if (phoneNumber == null)
		{
			GameplayController.PhoneNumber phoneNumber2 = new GameplayController.PhoneNumber
			{
				number = newNumber,
				loc = knowLocation,
				p = list,
				textOverride = textOverride
			};
			this.acquiredNumbers.Add(phoneNumber2);
			if (displayMessage)
			{
				string empty = string.Empty;
				InterfaceController instance = InterfaceController.Instance;
				InterfaceController.GameMessageType newType = InterfaceController.GameMessageType.notification;
				int newNumerical = 0;
				string newMessage = Strings.Get("ui.gamemessage", "telephone", Strings.Casing.asIs, false, false, false, null) + ": " + Toolbox.Instance.GetTelephoneNumberString(newNumber);
				InterfaceControls.Icon newIcon = InterfaceControls.Icon.telephone;
				AudioEvent additionalSFX = null;
				bool colourOverride = false;
				RectTransform notebookNotificationIcon = InterfaceController.Instance.notebookNotificationIcon;
				instance.NewGameMessage(newType, newNumerical, newMessage, newIcon, additionalSFX, colourOverride, default(Color), -1, 0f, notebookNotificationIcon, GameMessageController.PingOnComplete.none, null, null, null);
			}
		}
		else
		{
			phoneNumber.loc = knowLocation;
			phoneNumber.p = list;
			phoneNumber.textOverride = textOverride;
		}
		if (this.OnNewPhoneData != null)
		{
			this.OnNewPhoneData();
		}
	}

	// Token: 0x06002486 RID: 9350 RVA: 0x001E086C File Offset: 0x001DEA6C
	public void AddDoorKnockAttempt(NewDoor door, Actor human)
	{
		GameplayController.DoorKnockAttempt doorKnockAttempt = null;
		if (this.doorKnockAttempts.ContainsKey(door))
		{
			doorKnockAttempt = this.doorKnockAttempts[door].Find((GameplayController.DoorKnockAttempt item) => item.human == human);
		}
		else
		{
			this.doorKnockAttempts.Add(door, new List<GameplayController.DoorKnockAttempt>());
		}
		if (doorKnockAttempt != null)
		{
			doorKnockAttempt.value += 1f;
			return;
		}
		GameplayController.DoorKnockAttempt doorKnockAttempt2 = new GameplayController.DoorKnockAttempt
		{
			human = human,
			value = 1f
		};
		this.doorKnockAttempts[door].Add(doorKnockAttempt2);
	}

	// Token: 0x06002487 RID: 9351 RVA: 0x001E090C File Offset: 0x001DEB0C
	public float GetDoorKnockAttemptValue(NewDoor door, Actor human)
	{
		GameplayController.DoorKnockAttempt doorKnockAttempt = null;
		if (this.doorKnockAttempts.ContainsKey(door))
		{
			doorKnockAttempt = this.doorKnockAttempts[door].Find((GameplayController.DoorKnockAttempt item) => item.human == human);
		}
		if (doorKnockAttempt == null)
		{
			return 0f;
		}
		if (human.isEnforcer && human.isOnDuty)
		{
			return doorKnockAttempt.value * 2f;
		}
		return doorKnockAttempt.value;
	}

	// Token: 0x06002488 RID: 9352 RVA: 0x001E098C File Offset: 0x001DEB8C
	public void KnockOnDoor(NewDoor door, Actor actor, int knocks = 2, float forceAdditionalUrgency = 0f)
	{
		float nextUrgency = this.GetDoorKnockAttemptValue(door, actor) + forceAdditionalUrgency;
		if (Game.Instance.devMode && Game.Instance.collectDebugData && actor != null)
		{
			string text = string.Empty;
			if (actor.ai != null && actor.ai.currentAction != null)
			{
				text = actor.ai.currentAction.preset.name + " (" + actor.ai.currentAction.goal.preset.name + ")";
			}
			string text2 = string.Empty;
			if (actor.ai != null && actor.ai.currentAction != null)
			{
				if (actor.ai.currentAction.goal.passedInteractable != null && actor.ai.currentAction.goal.passedInteractable.node != null)
				{
					int num;
					text2 = "Trespass goal location (interactable): " + actor.IsTrespassing(actor.ai.currentAction.goal.passedInteractable.node.room, out num, out text2, true).ToString() + " " + text2;
				}
				else if (actor.ai.currentAction.goal.roomLocation != null)
				{
					int num;
					text2 = "Trespass goal location (room): " + actor.IsTrespassing(actor.ai.currentAction.goal.roomLocation, out num, out text2, true).ToString() + " " + text2;
				}
				else if (actor.ai.currentAction.goal.passedNode != null)
				{
					int num;
					text2 = "Trespass goal location (node): " + actor.IsTrespassing(actor.ai.currentAction.goal.passedNode.room, out num, out text2, true).ToString() + " " + text2;
				}
				for (int i = 0; i < actor.ai.currentAction.goal.actions.Count; i++)
				{
					NewAIAction newAIAction = actor.ai.currentAction.goal.actions[i];
					if (!newAIAction.insertedAction && newAIAction.node != null)
					{
						string empty = string.Empty;
						int num;
						bool flag = actor.IsTrespassing(newAIAction.node.room, out num, out empty, true);
						text2 = string.Concat(new string[]
						{
							text2,
							", Trespass action ",
							newAIAction.preset.name,
							" location (node): ",
							flag.ToString(),
							" ",
							empty
						});
						break;
					}
				}
			}
			Game.Log(string.Concat(new string[]
			{
				"Debug: Knock by ",
				actor.name,
				" with urgency: ",
				nextUrgency.ToString(),
				" at ",
				actor.currentRoom.name,
				", ",
				actor.currentGameLocation.name,
				" - door to ",
				door.wall.node.gameLocation.name,
				" action: ",
				text,
				" trespass: ",
				text2
			}), 2);
		}
		base.StartCoroutine(this.DoorKnockSounds(door, actor, nextUrgency, knocks));
		this.AddDoorKnockAttempt(door, actor as Human);
	}

	// Token: 0x06002489 RID: 9353 RVA: 0x001E0D03 File Offset: 0x001DEF03
	private IEnumerator DoorKnockSounds(NewDoor door, Actor actor, float nextUrgency, int knocks = 2)
	{
		door.knockingInProgress = true;
		if (nextUrgency <= 1.5f)
		{
			door.doorInteractable.actionAudioEventOverrides[RoutineControls.Instance.knockOnDoor] = door.preset.audioKnockLight;
		}
		else if (nextUrgency > 1.5f && nextUrgency < 3.5f)
		{
			door.doorInteractable.actionAudioEventOverrides[RoutineControls.Instance.knockOnDoor] = door.preset.audioKnockMed;
		}
		else
		{
			door.doorInteractable.actionAudioEventOverrides[RoutineControls.Instance.knockOnDoor] = door.preset.audioKnockHeavy;
		}
		if (actor.isPlayer)
		{
			InteractionController.Instance.UpdateInteractionText();
		}
		float num = Mathf.Clamp01(nextUrgency / 8f);
		float vol = Mathf.Lerp(0.9f, 1f, num);
		float delay = Mathf.Lerp(0.26f, 0.22f, num);
		AudioEvent knockAudio = door.preset.audioKnockLight;
		if (num > 0.33f)
		{
			knockAudio = door.preset.audioKnockMed;
			if (num > 0.66f)
			{
				knockAudio = door.preset.audioKnockHeavy;
			}
		}
		if (door.wall.otherWall.node.gameLocation.thisAsAddress != null)
		{
			if (actor.currentRoom == door.wall.node.room)
			{
				door.wall.otherWall.node.gameLocation.thisAsAddress.OnDoorKnockByActor(door, num, actor);
			}
			else
			{
				door.wall.node.gameLocation.thisAsAddress.OnDoorKnockByActor(door, num, actor);
			}
		}
		while (knocks > 0 && door.isClosed)
		{
			AudioController.Instance.PlayWorldOneShot(knockAudio, actor, door.parentedWall.node, door.doorInteractable.wPos, null, null, vol, door.bothNodesForAudioSource, false, null, false);
			int num2 = knocks;
			knocks = num2 - 1;
			yield return new WaitForSeconds(delay);
		}
		door.knockingInProgress = false;
		yield break;
	}

	// Token: 0x0600248A RID: 9354 RVA: 0x001E0D28 File Offset: 0x001DEF28
	public void SetJobDifficultyLevel(int newInt)
	{
		this.jobDifficultyLevel = newInt;
	}

	// Token: 0x0600248B RID: 9355 RVA: 0x001E0D34 File Offset: 0x001DEF34
	public void AddToGraffitiCache(string obj, Material mat)
	{
		if (!this.graffitiCache.ContainsKey(obj))
		{
			if (this.graffitiCache.Count > 50)
			{
				try
				{
					List<string> list = new List<string>(this.graffitiCache.Keys);
					this.graffitiCache.Remove(Enumerable.FirstOrDefault<string>(list));
				}
				catch
				{
				}
			}
			this.graffitiCache.Add(obj, mat);
		}
	}

	// Token: 0x0600248C RID: 9356 RVA: 0x001E0DA4 File Offset: 0x001DEFA4
	public void AddMotionTracker(Interactable newTracker, int range)
	{
		if (newTracker == null)
		{
			return;
		}
		if (!this.activeTrackers.ContainsKey(newTracker))
		{
			this.activeTrackers.Add(newTracker, new List<NewNode>());
		}
		else
		{
			this.activeTrackers[newTracker].Clear();
		}
		float num = float.PositiveInfinity;
		Vector3 vector;
		vector..ctor(0f, 0f, -1f);
		foreach (NewWall newWall in newTracker.node.walls)
		{
			float num2 = Vector3.Distance(newWall.position, newTracker.wPos);
			if (num2 < num)
			{
				num = num2;
				vector = -new Vector3(newWall.wallOffset.x, 0f, newWall.wallOffset.y).normalized;
			}
		}
		this.activeTrackers[newTracker].Add(newTracker.node);
		Vector3 vector2 = newTracker.node.nodeCoord;
		NewNode newNode = newTracker.node;
		for (int i = 0; i < range; i++)
		{
			vector2 += new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z), 0);
			NewNode newNode2;
			if (!PathFinder.Instance.nodeMap.TryGetValue(vector2, ref newNode2) || !newNode.accessToOtherNodes.ContainsKey(newNode2) || (newNode.accessToOtherNodes[newNode2].accessType != NewNode.NodeAccess.AccessType.adjacent && newNode.accessToOtherNodes[newNode2].accessType != NewNode.NodeAccess.AccessType.openDoorway && newNode.accessToOtherNodes[newNode2].accessType != NewNode.NodeAccess.AccessType.streetToStreet))
			{
				break;
			}
			if (!this.activeTrackers[newTracker].Contains(newNode2))
			{
				this.activeTrackers[newTracker].Add(newNode2);
			}
			newNode = newNode2;
		}
		foreach (NewNode newNode3 in this.activeTrackers[newTracker])
		{
			if (!this.trackedNodes.Contains(newNode3))
			{
				this.trackedNodes.Add(newNode3);
			}
		}
		string[] array = new string[11];
		array[0] = "Gameplay: Adding motion tracker to node ";
		int num3 = 1;
		NewNode node = newTracker.node;
		array[num3] = ((node != null) ? node.ToString() : null);
		array[2] = " at ";
		array[3] = newTracker.node.gameLocation.name;
		array[4] = " in the direction of ";
		int num4 = 5;
		Vector3 vector3 = vector;
		array[num4] = vector3.ToString();
		array[6] = " : ";
		array[7] = new Vector3(vector.x, vector.z, 0f).ToString();
		array[8] = " covering ";
		array[9] = this.activeTrackers[newTracker].Count.ToString();
		array[10] = " nodes";
		Game.Log(string.Concat(array), 2);
	}

	// Token: 0x0600248D RID: 9357 RVA: 0x001E10B8 File Offset: 0x001DF2B8
	public void RemoveMotionTracker(Interactable newTracker)
	{
		if (newTracker == null)
		{
			return;
		}
		if (this.activeTrackers.ContainsKey(newTracker))
		{
			foreach (NewNode newNode in this.activeTrackers[newTracker])
			{
				bool flag = true;
				foreach (KeyValuePair<Interactable, List<NewNode>> keyValuePair in this.activeTrackers)
				{
					if (keyValuePair.Key != newTracker && keyValuePair.Value.Contains(newNode))
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					this.trackedNodes.Remove(newNode);
				}
			}
			this.activeTrackers.Remove(newTracker);
		}
	}

	// Token: 0x0600248E RID: 9358 RVA: 0x001E1198 File Offset: 0x001DF398
	public void AddProxyDetonator(Interactable newTracker, float range)
	{
		if (newTracker == null)
		{
			return;
		}
		Game.Log("Gameplay: Adding new proxy detonator " + newTracker.name + " with range " + range.ToString(), 2);
		newTracker.SetValue(GameplayControls.Instance.proxyGrenadeFuse);
		if (!this.proxyTrackers.ContainsKey(newTracker))
		{
			this.proxyTrackers.Add(newTracker, range);
			return;
		}
		this.proxyTrackers[newTracker] = range;
	}

	// Token: 0x0600248F RID: 9359 RVA: 0x001E1204 File Offset: 0x001DF404
	public void RemoveProxyDetonator(Interactable newTracker)
	{
		newTracker.SetValue(GameplayControls.Instance.proxyGrenadeFuse);
		if (this.proxyTrackers.ContainsKey(newTracker))
		{
			this.proxyTrackers.Remove(newTracker);
		}
	}

	// Token: 0x06002490 RID: 9360 RVA: 0x001E1231 File Offset: 0x001DF431
	public void SetPlayerKnowsPassword(NewAddress newAddress)
	{
		if (newAddress == null)
		{
			return;
		}
		if (!this.playerKnowsPasswords.Contains(newAddress.id))
		{
			this.playerKnowsPasswords.Add(newAddress.id);
			MapController.Instance.AddUpdateCall(newAddress.mapButton);
		}
	}

	// Token: 0x06002491 RID: 9361 RVA: 0x001E1274 File Offset: 0x001DF474
	public void ProcessDynamicTextImages()
	{
		foreach (ArtPreset artPreset in Toolbox.Instance.allArt)
		{
			if (artPreset.useDynamicText)
			{
				Texture2D texture2D = artPreset.material.GetTexture("_BaseColorMap") as Texture2D;
				TextToImageController.TextToImageSettings textToImageSettings = new TextToImageController.TextToImageSettings();
				textToImageSettings.color = artPreset.textColour;
				textToImageSettings.textSize = artPreset.textSize;
				textToImageSettings.font = artPreset.textFont;
				textToImageSettings.enableProcessing = true;
				textToImageSettings.useAlpha = true;
				textToImageSettings.trim = true;
				textToImageSettings.trimPadding = 4;
				if (artPreset.dynamicTextSource == ArtPreset.DynamicTextSouce.blackMarketTraderPassword)
				{
					NewAddress newAddress = CityData.Instance.addressDirectory.Find((NewAddress item) => item.addressPreset != null && item.addressPreset.name == "BlackmarketTrader");
					if (newAddress != null)
					{
						textToImageSettings.textString = newAddress.GetPassword();
					}
				}
				else if (artPreset.dynamicTextSource == ArtPreset.DynamicTextSouce.weaponsDealerPassword)
				{
					NewAddress newAddress2 = CityData.Instance.addressDirectory.Find((NewAddress item) => item.addressPreset != null && item.addressPreset.name == "WeaponsDealer");
					if (newAddress2 != null)
					{
						textToImageSettings.textString = newAddress2.GetPassword();
					}
				}
				Texture2D texture2D2 = TextToImageController.Instance.CaptureTextToImage(textToImageSettings);
				int num = Mathf.Max(texture2D.width, texture2D2.width);
				int num2 = texture2D.height + texture2D2.height;
				Vector2 vector;
				vector..ctor(0f, (float)texture2D2.height);
				Vector2 zero = Vector2.zero;
				if (texture2D.width > texture2D2.width)
				{
					vector.x = 0f;
					zero.x = (float)Mathf.FloorToInt((float)(texture2D.width - texture2D2.width) * 0.5f);
				}
				else if (texture2D.width < texture2D2.width)
				{
					zero.x = 0f;
					vector.x = (float)Mathf.FloorToInt((float)(texture2D2.width - texture2D.width) * 0.5f);
				}
				Texture2D texture2D3 = new Texture2D(num, num2);
				texture2D3.filterMode = 0;
				for (int i = 0; i < texture2D3.width; i++)
				{
					for (int j = 0; j < texture2D3.height; j++)
					{
						texture2D3.SetPixel(i, j, Color.clear);
					}
				}
				for (int k = 0; k < texture2D.width; k++)
				{
					for (int l = 0; l < texture2D.height; l++)
					{
						Color pixel = texture2D.GetPixel(k, l);
						texture2D3.SetPixel((int)vector.x + k, (int)vector.y + l, pixel);
					}
				}
				for (int m = 0; m < texture2D2.width; m++)
				{
					for (int n = 0; n < texture2D2.height; n++)
					{
						Color pixel2 = texture2D2.GetPixel(m, n);
						texture2D3.SetPixel((int)zero.x + m, (int)zero.y + n, pixel2);
					}
				}
				texture2D3.Apply();
				Material material = Object.Instantiate<Material>(artPreset.material);
				material.SetTexture("_BaseColorMap", texture2D3);
				this.dynamicTextImages.Add(artPreset, material);
			}
		}
	}

	// Token: 0x06002492 RID: 9362 RVA: 0x001E15D0 File Offset: 0x001DF7D0
	public void AddNewDebt(Company company, int amount, int paymentExtra, int repayments)
	{
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: Adding new player debt for ",
			company.name,
			" to the tune of ",
			amount.ToString(),
			"..."
		}), 2);
		GameplayController.LoanDebt loanDebt = this.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
		GameplayController.Instance.AddMoney(amount, true, "Loan");
		if (AchievementsController.Instance != null)
		{
			AchievementsController.Instance.UnlockAchievement("Shark Bait", "loan_shark");
		}
		if (loanDebt != null)
		{
			loanDebt.debt += amount + paymentExtra;
			loanDebt.payments += repayments;
			return;
		}
		loanDebt = new GameplayController.LoanDebt
		{
			companyID = company.companyID,
			debt = amount + paymentExtra,
			payments = repayments
		};
		float num = 24f - SessionData.Instance.decimalClock;
		num += 24.01f;
		loanDebt.nextPaymentDueBy = (SessionData.Instance.gameTime += num);
		this.debt.Add(loanDebt);
	}

	// Token: 0x06002493 RID: 9363 RVA: 0x001E1700 File Offset: 0x001DF900
	public void DebtPayment(Company company)
	{
		if (company == null)
		{
			return;
		}
		GameplayController.LoanDebt loanDebt = this.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
		if (loanDebt != null)
		{
			int repaymentAmount = loanDebt.GetRepaymentAmount();
			loanDebt.debt -= repaymentAmount;
			GameplayController.Instance.AddMoney(-repaymentAmount, true, "Debt payoff");
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Paying player debt for ",
				company.name,
				": ",
				repaymentAmount.ToString(),
				" (",
				loanDebt.debt.ToString(),
				" remaining)"
			}), 2);
			if (loanDebt.debt <= 0)
			{
				this.debt.Remove(loanDebt);
				return;
			}
			float num = 24f - SessionData.Instance.decimalClock;
			num += 24.01f;
			loanDebt.nextPaymentDueBy = (SessionData.Instance.gameTime += num);
		}
	}

	// Token: 0x06002494 RID: 9364 RVA: 0x001E180C File Offset: 0x001DFA0C
	public void ShortDebtPayment(Company company, int amount)
	{
		if (company == null)
		{
			return;
		}
		GameplayController.LoanDebt loanDebt = this.debt.Find((GameplayController.LoanDebt item) => item.companyID == company.companyID);
		if (loanDebt != null)
		{
			loanDebt.debt -= amount;
			GameplayController.Instance.AddMoney(-amount, true, "Debt payoff");
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Paying player debt for ",
				company.name,
				": ",
				amount.ToString(),
				" (",
				loanDebt.debt.ToString(),
				" remaining)"
			}), 2);
			if (loanDebt.debt <= 0)
			{
				this.debt.Remove(loanDebt);
			}
		}
	}

	// Token: 0x06002495 RID: 9365 RVA: 0x001E18DC File Offset: 0x001DFADC
	public void AddHotelGuest(Human human, bool expensiveRoom)
	{
		if (human == null)
		{
			return;
		}
		if (human.currentBuilding == null)
		{
			return;
		}
		List<NewAddress> list = CityData.Instance.addressDirectory.FindAll((NewAddress item) => item.building == human.currentBuilding && item.residence != null && item.residence.preset != null && item.residence.preset.isHotelRoom);
		using (List<GameplayController.HotelGuest>.Enumerator enumerator = this.hotelGuests.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GameplayController.HotelGuest g = enumerator.Current;
				list.RemoveAll((NewAddress item) => item.id == g.addID);
			}
		}
		List<NewAddress> list2 = new List<NewAddress>();
		List<NewAddress> list3 = new List<NewAddress>();
		float num = 0f;
		foreach (NewAddress newAddress in list)
		{
			num += newAddress.normalizedLandValue;
		}
		num /= (float)list.Count;
		foreach (NewAddress newAddress2 in list)
		{
			if (newAddress2.normalizedLandValue <= num)
			{
				list2.Add(newAddress2);
			}
			else
			{
				list3.Add(newAddress2);
			}
		}
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: There are ",
			list2.Count.ToString(),
			" cheap rooms and ",
			list3.Count.ToString(),
			" expensive rooms..."
		}), 2);
		if (expensiveRoom)
		{
			if (list3.Count > 0)
			{
				this.AddHotelGuest(list3[Toolbox.Instance.Rand(0, list3.Count, false)], human, CityControls.Instance.hotelCostUpper);
				return;
			}
			if (list2.Count > 0)
			{
				this.AddHotelGuest(list2[Toolbox.Instance.Rand(0, list2.Count, false)], human, CityControls.Instance.hotelCostUpper);
				return;
			}
		}
		else
		{
			if (list2.Count > 0)
			{
				this.AddHotelGuest(list2[Toolbox.Instance.Rand(0, list2.Count, false)], human, CityControls.Instance.hotelCostLower);
				return;
			}
			if (list3.Count > 0)
			{
				this.AddHotelGuest(list3[Toolbox.Instance.Rand(0, list3.Count, false)], human, CityControls.Instance.hotelCostLower);
			}
		}
	}

	// Token: 0x06002496 RID: 9366 RVA: 0x001E1B80 File Offset: 0x001DFD80
	public void AddHotelGuest(NewAddress address, Human human, int cost)
	{
		if (address == null)
		{
			return;
		}
		if (human == null)
		{
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Gameplay: Adding new hotel geust; ",
			human.GetCitizenName(),
			" at room ",
			address.name,
			" for ",
			cost.ToString(),
			" a night..."
		}), 2);
		GameplayController.HotelGuest hotelGuest = new GameplayController.HotelGuest
		{
			addID = address.id,
			humanID = human.humanID,
			roomCost = cost,
			bill = cost,
			lastPayment = SessionData.Instance.gameTime,
			nextPayment = SessionData.Instance.gameTime + 24f
		};
		this.hotelGuests.Add(hotelGuest);
		human.AddLocationOfAuthorty(address);
		if (human.isPlayer)
		{
			Player.Instance.AddToKeyring(address, true);
			if (MapController.Instance.playerRoute == null)
			{
				MapController.Instance.PlotPlayerRoute(address);
			}
			if (AchievementsController.Instance != null)
			{
				AchievementsController.Instance.UnlockAchievement("Room 237", "hire_hotel");
			}
		}
	}

	// Token: 0x06002497 RID: 9367 RVA: 0x001E1CA0 File Offset: 0x001DFEA0
	public void RemoveHotelGuest(NewAddress address, Human human, bool removeKey = true)
	{
		if (address == null)
		{
			return;
		}
		if (human == null)
		{
			return;
		}
		Game.Log("Gameplay: Removing all hotel guests at; " + human.GetCitizenName() + " room " + address.name, 2);
		this.hotelGuests.RemoveAll((GameplayController.HotelGuest item) => item.addID == address.id && item.humanID == human.humanID);
		human.RemoveLocationOfAuthority(address);
		if (human.isPlayer && removeKey)
		{
			Player.Instance.RemoveFromKeyring(address);
		}
	}

	// Token: 0x04002E1B RID: 11803
	public Dictionary<string, Evidence> evidenceDictionary = new Dictionary<string, Evidence>();

	// Token: 0x04002E1C RID: 11804
	public List<Fact> factList = new List<Fact>();

	// Token: 0x04002E1D RID: 11805
	public List<Evidence> singletonEvidence = new List<Evidence>();

	// Token: 0x04002E1E RID: 11806
	public List<EvidenceDate> dateEvidence = new List<EvidenceDate>();

	// Token: 0x04002E1F RID: 11807
	public List<EvidenceTime> timeEvidence = new List<EvidenceTime>();

	// Token: 0x04002E20 RID: 11808
	public List<EvidenceMultiPage> multiPageEvidence = new List<EvidenceMultiPage>();

	// Token: 0x04002E21 RID: 11809
	public List<GameplayController.History> history = new List<GameplayController.History>();

	// Token: 0x04002E22 RID: 11810
	[NonSerialized]
	public List<GameplayController.History> itemOnlyHistory = new List<GameplayController.History>();

	// Token: 0x04002E23 RID: 11811
	public Dictionary<Vector3, Interactable> confirmedPrints = new Dictionary<Vector3, Interactable>();

	// Token: 0x04002E24 RID: 11812
	public int printsLetterLoop;

	// Token: 0x04002E25 RID: 11813
	public HashSet<Interactable> objectsWithDynamicPrints = new HashSet<Interactable>();

	// Token: 0x04002E26 RID: 11814
	public Dictionary<MatchPreset, List<Evidence>> parentMatches = new Dictionary<MatchPreset, List<Evidence>>();

	// Token: 0x04002E27 RID: 11815
	public Dictionary<MatchPreset, List<FactMatches>> matchesDetails = new Dictionary<MatchPreset, List<FactMatches>>();

	// Token: 0x04002E28 RID: 11816
	public Dictionary<NewAddress, Vector2> guestPasses = new Dictionary<NewAddress, Vector2>();

	// Token: 0x04002E29 RID: 11817
	public Dictionary<int, StateSaveData.MessageThreadSave> messageThreads = new Dictionary<int, StateSaveData.MessageThreadSave>();

	// Token: 0x04002E2A RID: 11818
	public int assignMessageThreadID = 1;

	// Token: 0x04002E2B RID: 11819
	public List<Human> enforcers = new List<Human>();

	// Token: 0x04002E2C RID: 11820
	public Dictionary<NewGameLocation, GameplayController.EnforcerCall> enforcerCalls = new Dictionary<NewGameLocation, GameplayController.EnforcerCall>();

	// Token: 0x04002E2D RID: 11821
	public Dictionary<Case, float> caseProcessing = new Dictionary<Case, float>();

	// Token: 0x04002E2E RID: 11822
	public List<Interactable> hospitalBeds = new List<Interactable>();

	// Token: 0x04002E2F RID: 11823
	public Dictionary<Vector3, float> brokenWindows = new Dictionary<Vector3, float>();

	// Token: 0x04002E30 RID: 11824
	public Dictionary<NewDoor, List<GameplayController.DoorKnockAttempt>> doorKnockAttempts = new Dictionary<NewDoor, List<GameplayController.DoorKnockAttempt>>();

	// Token: 0x04002E31 RID: 11825
	public List<Interactable> activeGadgets = new List<Interactable>();

	// Token: 0x04002E32 RID: 11826
	public HashSet<NewGameLocation> crimeScenes = new HashSet<NewGameLocation>();

	// Token: 0x04002E33 RID: 11827
	public List<NewDoor> policeTapeDoors = new List<NewDoor>();

	// Token: 0x04002E34 RID: 11828
	public List<NewGameLocation> crimeSceneCleanups = new List<NewGameLocation>();

	// Token: 0x04002E35 RID: 11829
	public List<Interactable> closedBreakers = new List<Interactable>();

	// Token: 0x04002E36 RID: 11830
	public List<Interactable> turnedOffSecurity = new List<Interactable>();

	// Token: 0x04002E37 RID: 11831
	public List<Interactable> burningBarrels = new List<Interactable>();

	// Token: 0x04002E38 RID: 11832
	public Dictionary<Interactable, float> switchRessetingObjects = new Dictionary<Interactable, float>();

	// Token: 0x04002E39 RID: 11833
	public List<int> playerKnowsPasswords = new List<int>();

	// Token: 0x04002E3A RID: 11834
	public List<NewRoom> gasRooms = new List<NewRoom>();

	// Token: 0x04002E3B RID: 11835
	public List<string> companiesSabotaged = new List<string>();

	// Token: 0x04002E3C RID: 11836
	public Dictionary<string, float> globalConversationDelay = new Dictionary<string, float>();

	// Token: 0x04002E3D RID: 11837
	public List<string> booksRead = new List<string>();

	// Token: 0x04002E3E RID: 11838
	public List<Interactable> activeKettles = new List<Interactable>();

	// Token: 0x04002E3F RID: 11839
	public List<Interactable> activeMusicPlayers = new List<Interactable>();

	// Token: 0x04002E40 RID: 11840
	public Dictionary<string, Material> graffitiCache = new Dictionary<string, Material>();

	// Token: 0x04002E41 RID: 11841
	public Dictionary<Interactable, List<NewNode>> activeTrackers = new Dictionary<Interactable, List<NewNode>>();

	// Token: 0x04002E42 RID: 11842
	public HashSet<NewNode> trackedNodes = new HashSet<NewNode>();

	// Token: 0x04002E43 RID: 11843
	public Dictionary<Interactable, float> proxyTrackers = new Dictionary<Interactable, float>();

	// Token: 0x04002E44 RID: 11844
	public List<Interactable> activeGrenades = new List<Interactable>();

	// Token: 0x04002E45 RID: 11845
	public GameObject setDefaultItemButton;

	// Token: 0x04002E46 RID: 11846
	public GameObject defaultItemButton;

	// Token: 0x04002E47 RID: 11847
	[Header("Player Stats")]
	public int money;

	// Token: 0x04002E48 RID: 11848
	public int lockPicks;

	// Token: 0x04002E49 RID: 11849
	public int socialCredit;

	// Token: 0x04002E4A RID: 11850
	public List<SocialControls.SocialCreditBuff> socialCreditPerks = new List<SocialControls.SocialCreditBuff>();

	// Token: 0x04002E4B RID: 11851
	public float currentLockpickStrength = 1f;

	// Token: 0x04002E4C RID: 11852
	public int perilFine;

	// Token: 0x04002E4D RID: 11853
	public string[] doeLetters = new string[]
	{
		"A",
		"B",
		"C",
		"D",
		"E",
		"F",
		"G",
		"H",
		"I",
		"J",
		"K",
		"L",
		"M",
		"N",
		"O",
		"P",
		"Q",
		"R",
		"S",
		"T",
		"U",
		"V",
		"W",
		"X",
		"Y",
		"Z"
	};

	// Token: 0x04002E4E RID: 11854
	public float timeSinceLastUpdateLoop;

	// Token: 0x04002E4F RID: 11855
	public float lastUpdateLoop;

	// Token: 0x04002E50 RID: 11856
	[Header("Passcodes")]
	public List<GameplayController.Passcode> acquiredPasscodes = new List<GameplayController.Passcode>();

	// Token: 0x04002E51 RID: 11857
	[Header("Phone Numbers")]
	public List<GameplayController.PhoneNumber> acquiredNumbers = new List<GameplayController.PhoneNumber>();

	// Token: 0x04002E52 RID: 11858
	[Header("Apartments for Sale")]
	public List<NewAddress> forSale = new List<NewAddress>();

	// Token: 0x04002E53 RID: 11859
	[Header("Hotel")]
	public List<GameplayController.HotelGuest> hotelGuests = new List<GameplayController.HotelGuest>();

	// Token: 0x04002E54 RID: 11860
	[Header("Culling State")]
	public List<NewRoom> roomsVicinity = new List<NewRoom>();

	// Token: 0x04002E55 RID: 11861
	public List<AirDuctGroup> ductsVicinity = new List<AirDuctGroup>();

	// Token: 0x04002E56 RID: 11862
	public HashSet<Human> activeRagdolls = new HashSet<Human>();

	// Token: 0x04002E57 RID: 11863
	public HashSet<Interactable> activePhysics = new HashSet<Interactable>();

	// Token: 0x04002E58 RID: 11864
	[Header("Clean-up")]
	public List<SpatterSimulation> spatter = new List<SpatterSimulation>();

	// Token: 0x04002E59 RID: 11865
	public List<Interactable> interactablesMoved = new List<Interactable>();

	// Token: 0x04002E5A RID: 11866
	public HashSet<NewDoor> damagedDoors = new HashSet<NewDoor>();

	// Token: 0x04002E5B RID: 11867
	[Header("Security")]
	public List<NewBuilding> activeAlarmsBuildings = new List<NewBuilding>();

	// Token: 0x04002E5C RID: 11868
	public List<NewAddress> activeAlarmsLocations = new List<NewAddress>();

	// Token: 0x04002E5D RID: 11869
	public List<NewBuilding> alteredSecurityTargetsBuildings = new List<NewBuilding>();

	// Token: 0x04002E5E RID: 11870
	public List<NewAddress> alteredSecurityTargetsLocations = new List<NewAddress>();

	// Token: 0x04002E5F RID: 11871
	[Header("Footprints")]
	public List<GameplayController.Footprint> footprintsList = new List<GameplayController.Footprint>();

	// Token: 0x04002E60 RID: 11872
	public Dictionary<NewRoom, List<GameplayController.Footprint>> activeFootprints = new Dictionary<NewRoom, List<GameplayController.Footprint>>();

	// Token: 0x04002E61 RID: 11873
	public Dictionary<Vector3, Interactable> confirmedFootprints = new Dictionary<Vector3, Interactable>();

	// Token: 0x04002E62 RID: 11874
	public Dictionary<ArtPreset, Material> dynamicTextImages = new Dictionary<ArtPreset, Material>();

	// Token: 0x04002E63 RID: 11875
	public List<GameplayController.LoanDebt> debt = new List<GameplayController.LoanDebt>();

	// Token: 0x04002E64 RID: 11876
	[Tooltip("Dictates which missions can spawn...")]
	[Header("Difficulty")]
	public int jobDifficultyLevel;

	// Token: 0x04002E68 RID: 11880
	private Action UpdateMatch;

	// Token: 0x04002E69 RID: 11881
	private static GameplayController _instance;

	// Token: 0x0200067B RID: 1659
	[Serializable]
	public class History
	{
		// Token: 0x04002E6A RID: 11882
		public string evID;

		// Token: 0x04002E6B RID: 11883
		public List<Evidence.DataKey> keys;

		// Token: 0x04002E6C RID: 11884
		public float lastAccess;

		// Token: 0x04002E6D RID: 11885
		public int locationID;
	}

	// Token: 0x0200067C RID: 1660
	[Serializable]
	public class LostAndFound
	{
		// Token: 0x04002E6E RID: 11886
		public string preset;

		// Token: 0x04002E6F RID: 11887
		public int ownerID;

		// Token: 0x04002E70 RID: 11888
		public int buildingID;

		// Token: 0x04002E71 RID: 11889
		public int spawnedItem;

		// Token: 0x04002E72 RID: 11890
		public int spawnedNote;

		// Token: 0x04002E73 RID: 11891
		public int rewardMoney;

		// Token: 0x04002E74 RID: 11892
		public int rewardSC;
	}

	// Token: 0x0200067D RID: 1661
	public class DoorKnockAttempt
	{
		// Token: 0x04002E75 RID: 11893
		public Actor human;

		// Token: 0x04002E76 RID: 11894
		public float value;
	}

	// Token: 0x0200067E RID: 1662
	[Serializable]
	public class Passcode
	{
		// Token: 0x0600249C RID: 9372 RVA: 0x001E2106 File Offset: 0x001E0306
		public Passcode(GameplayController.PasscodeType newType)
		{
			this.type = newType;
		}

		// Token: 0x0600249D RID: 9373 RVA: 0x001E212C File Offset: 0x001E032C
		public string GetNotePlacements()
		{
			string text = string.Empty;
			foreach (int num in this.notes)
			{
				Interactable interactable = null;
				if (CityData.Instance.savableInteractableDictionary.TryGetValue(num, ref interactable))
				{
					text = string.Concat(new string[]
					{
						text,
						"\n",
						interactable.id.ToString(),
						" ",
						interactable.name,
						" at ",
						interactable.GetWorldPosition(true).ToString(),
						", ",
						interactable.node.room.name,
						", ",
						interactable.node.gameLocation.name
					});
				}
			}
			Game.Log(text, 2);
			return text;
		}

		// Token: 0x0600249E RID: 9374 RVA: 0x001E2238 File Offset: 0x001E0438
		public List<int> GetDigits()
		{
			if (Game.Instance.overridePasscodes)
			{
				List<int> list = new List<int>();
				string text = Game.Instance.overriddenPasscode.ToString();
				while (text != null && text.Length < 4)
				{
					text = "0" + text;
				}
				try
				{
					for (int i = 0; i < 4; i++)
					{
						int num = 0;
						int.TryParse(text.Substring(i, 1), ref num);
						list.Add(num);
					}
					return list;
				}
				catch
				{
					return this.digits;
				}
			}
			return this.digits;
		}

		// Token: 0x0600249F RID: 9375 RVA: 0x001E22D0 File Offset: 0x001E04D0
		public int GetDigit(int index)
		{
			index = Mathf.Clamp(index, 0, 3);
			if (Game.Instance.overridePasscodes)
			{
				List<int> list = new List<int>();
				string text = Game.Instance.overriddenPasscode.ToString();
				while (text != null && text.Length < 4)
				{
					text = "0" + text;
				}
				try
				{
					for (int i = 0; i < 4; i++)
					{
						int num = 0;
						int.TryParse(text.Substring(i, 1), ref num);
						list.Add(num);
					}
					return list[index];
				}
				catch
				{
					return this.digits[index];
				}
			}
			return this.digits[index];
		}

		// Token: 0x04002E77 RID: 11895
		public List<int> digits = new List<int>();

		// Token: 0x04002E78 RID: 11896
		public GameplayController.PasscodeType type;

		// Token: 0x04002E79 RID: 11897
		public int id;

		// Token: 0x04002E7A RID: 11898
		public bool used;

		// Token: 0x04002E7B RID: 11899
		public List<int> notes = new List<int>();
	}

	// Token: 0x0200067F RID: 1663
	public enum PasscodeType
	{
		// Token: 0x04002E7D RID: 11901
		citizen,
		// Token: 0x04002E7E RID: 11902
		room,
		// Token: 0x04002E7F RID: 11903
		address,
		// Token: 0x04002E80 RID: 11904
		interactable
	}

	// Token: 0x02000680 RID: 1664
	[Serializable]
	public class PhoneNumber
	{
		// Token: 0x04002E81 RID: 11905
		public int number;

		// Token: 0x04002E82 RID: 11906
		public string textOverride;

		// Token: 0x04002E83 RID: 11907
		public bool loc;

		// Token: 0x04002E84 RID: 11908
		public List<int> p = new List<int>();
	}

	// Token: 0x02000681 RID: 1665
	[Serializable]
	public class HotelGuest
	{
		// Token: 0x060024A1 RID: 9377 RVA: 0x001E2398 File Offset: 0x001E0598
		public Human GetHuman()
		{
			Human result = null;
			CityData.Instance.GetHuman(this.humanID, out result, true);
			return result;
		}

		// Token: 0x060024A2 RID: 9378 RVA: 0x001E23BC File Offset: 0x001E05BC
		public NewAddress GetAddress()
		{
			NewAddress result = null;
			CityData.Instance.addressDictionary.TryGetValue(this.addID, ref result);
			return result;
		}

		// Token: 0x060024A3 RID: 9379 RVA: 0x001E23E4 File Offset: 0x001E05E4
		public void PayBill(int amount)
		{
			this.bill = 0;
			this.lastPayment = SessionData.Instance.gameTime;
		}

		// Token: 0x060024A4 RID: 9380 RVA: 0x001E2400 File Offset: 0x001E0600
		public void FromLoadGame()
		{
			Human human = this.GetHuman();
			if (human != null)
			{
				NewAddress address = this.GetAddress();
				if (address != null)
				{
					human.AddLocationOfAuthorty(address);
				}
			}
		}

		// Token: 0x04002E85 RID: 11909
		public int addID;

		// Token: 0x04002E86 RID: 11910
		public int humanID;

		// Token: 0x04002E87 RID: 11911
		public int roomCost;

		// Token: 0x04002E88 RID: 11912
		public int bill;

		// Token: 0x04002E89 RID: 11913
		public float lastPayment;

		// Token: 0x04002E8A RID: 11914
		public float nextPayment;
	}

	// Token: 0x02000682 RID: 1666
	[Serializable]
	public class EnforcerCall
	{
		// Token: 0x04002E8B RID: 11915
		public bool isStreet;

		// Token: 0x04002E8C RID: 11916
		public int id;

		// Token: 0x04002E8D RID: 11917
		public float logTime;

		// Token: 0x04002E8E RID: 11918
		public GameplayController.EnforcerCallState state;

		// Token: 0x04002E8F RID: 11919
		public List<int> response;

		// Token: 0x04002E90 RID: 11920
		public float arrivalTime;

		// Token: 0x04002E91 RID: 11921
		public bool isCrimeScene;

		// Token: 0x04002E92 RID: 11922
		public bool immedaiteTeleport;

		// Token: 0x04002E93 RID: 11923
		public int guard = -1;
	}

	// Token: 0x02000683 RID: 1667
	public enum EnforcerCallState
	{
		// Token: 0x04002E95 RID: 11925
		logged,
		// Token: 0x04002E96 RID: 11926
		responding,
		// Token: 0x04002E97 RID: 11927
		arrived,
		// Token: 0x04002E98 RID: 11928
		completed
	}

	// Token: 0x02000684 RID: 1668
	[Serializable]
	public class Footprint
	{
		// Token: 0x060024A7 RID: 9383 RVA: 0x001E2444 File Offset: 0x001E0644
		public Footprint(Human human, Vector3 position, Vector3 euler, float dirt, float blood, NewRoom forceRoom = null)
		{
			this.hID = human.humanID;
			this.wP = position;
			this.eU = euler;
			this.str = Toolbox.Instance.RoundToPlaces(dirt * Toolbox.Instance.Rand(0.75f, 1f, false), 2);
			this.bl = Toolbox.Instance.RoundToPlaces(blood * Toolbox.Instance.Rand(0.75f, 1f, false), 2);
			this.t = SessionData.Instance.gameTime;
			if (forceRoom == null)
			{
				forceRoom = human.currentRoom;
			}
			this.rID = forceRoom.roomID;
			if (!GameplayController.Instance.activeFootprints.ContainsKey(forceRoom))
			{
				GameplayController.Instance.activeFootprints.Add(forceRoom, new List<GameplayController.Footprint>());
			}
			GameplayController.Instance.activeFootprints[forceRoom].Add(this);
			GameplayController.Instance.footprintsList.Add(this);
			forceRoom.QueueFootprintUpdate();
		}

		// Token: 0x04002E99 RID: 11929
		public int hID;

		// Token: 0x04002E9A RID: 11930
		public int rID;

		// Token: 0x04002E9B RID: 11931
		public Vector3 wP;

		// Token: 0x04002E9C RID: 11932
		public Vector3 eU;

		// Token: 0x04002E9D RID: 11933
		public float str;

		// Token: 0x04002E9E RID: 11934
		public float bl;

		// Token: 0x04002E9F RID: 11935
		public float t;
	}

	// Token: 0x02000685 RID: 1669
	[Serializable]
	public class LoanDebt
	{
		// Token: 0x060024A8 RID: 9384 RVA: 0x001E2549 File Offset: 0x001E0749
		public int GetRepaymentAmount()
		{
			return Mathf.Min(this.payments + this.missedPayments, this.debt);
		}

		// Token: 0x04002EA0 RID: 11936
		public int companyID;

		// Token: 0x04002EA1 RID: 11937
		public int debt;

		// Token: 0x04002EA2 RID: 11938
		public int payments;

		// Token: 0x04002EA3 RID: 11939
		public int missedPayments;

		// Token: 0x04002EA4 RID: 11940
		public float nextPaymentDueBy;

		// Token: 0x04002EA5 RID: 11941
		public float dueCheck;
	}

	// Token: 0x02000686 RID: 1670
	// (Invoke) Token: 0x060024AB RID: 9387
	public delegate void MatchesChange();

	// Token: 0x02000687 RID: 1671
	// (Invoke) Token: 0x060024AF RID: 9391
	public delegate void NewEvidenceHistory();

	// Token: 0x02000688 RID: 1672
	// (Invoke) Token: 0x060024B3 RID: 9395
	public delegate void NewPhoneData();
}
