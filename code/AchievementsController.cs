using System;
using System.Collections.Generic;
using NaughtyAttributes;
using Steamworks;
using UnityEngine;

// Token: 0x0200039E RID: 926
public class AchievementsController : MonoBehaviour
{
	// Token: 0x1700009B RID: 155
	// (get) Token: 0x06001515 RID: 5397 RVA: 0x00133DF1 File Offset: 0x00131FF1
	public static AchievementsController Instance
	{
		get
		{
			return AchievementsController._instance;
		}
	}

	// Token: 0x06001516 RID: 5398 RVA: 0x00133DF8 File Offset: 0x00131FF8
	private void Awake()
	{
		if (AchievementsController._instance != null && AchievementsController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		AchievementsController._instance = this;
	}

	// Token: 0x06001517 RID: 5399 RVA: 0x00133E28 File Offset: 0x00132028
	public bool GetAchievementStatus(string id)
	{
		bool result;
		try
		{
			bool flag;
			SteamUserStats.GetAchievement(id, ref flag);
			result = flag;
		}
		catch
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06001518 RID: 5400 RVA: 0x00133E58 File Offset: 0x00132058
	public void UnlockAchievement(string nameReference, string id)
	{
		try
		{
			bool flag = SteamUserStats.RequestCurrentStats();
			Game.Log("ACHIEVEMENT TRIGGER: " + nameReference + ": " + id, 2);
			if (!this.GetAchievementStatus(id) && flag)
			{
				try
				{
					SteamUserStats.SetAchievement(id);
				}
				catch
				{
					Game.LogError("Could not unlock achievement: Is steamworks connected?", 2);
				}
			}
		}
		catch
		{
			Game.LogError("Could not unlock achievement: Is steamworks connected?", 2);
		}
	}

	// Token: 0x06001519 RID: 5401 RVA: 0x00133ED4 File Offset: 0x001320D4
	public void AddToStat(string nameReference, string id, int add)
	{
		try
		{
			if (SteamUserStats.RequestCurrentStats())
			{
				Game.Log(string.Concat(new string[]
				{
					"STAT TRIGGER: ",
					nameReference,
					": ",
					id,
					" +",
					add.ToString()
				}), 2);
				try
				{
					int num;
					if (SteamUserStats.GetStat(id, ref num))
					{
						num += add;
						if (SteamUserStats.SetStat(id, num))
						{
							SteamUserStats.StoreStats();
						}
						else
						{
							Game.LogError("Cannot set stat: " + nameReference + ": " + id, 2);
						}
					}
					else
					{
						Game.LogError("Cannot get stat: " + nameReference + ": " + id, 2);
					}
				}
				catch
				{
					Game.LogError("Could not add stat: Is steamworks connected?", 2);
				}
			}
		}
		catch
		{
			Game.LogError("Could not add stat: Is steamworks connected?", 2);
		}
	}

	// Token: 0x0600151A RID: 5402 RVA: 0x00133FB4 File Offset: 0x001321B4
	public void ClearAchievement(string id)
	{
		try
		{
			if (SteamUserStats.RequestCurrentStats())
			{
				try
				{
					SteamUserStats.SetAchievement(id);
				}
				catch
				{
					Game.LogError("Could not clear achivement: Is steamworks connected?", 2);
				}
			}
		}
		catch
		{
			Game.LogError("Could not clear achivement: Is steamworks connected?", 2);
		}
	}

	// Token: 0x0600151B RID: 5403 RVA: 0x0013400C File Offset: 0x0013220C
	public void LoadTrackingDataFromSave(ref StateSaveData data)
	{
		this.freeHealthCareFlag = data.freeHealthCareFlag;
		this.notTheAnswerFlag = data.notTheAnswerFlag;
		this.privateSlyFlag = data.privateSlyFlag;
		this.allConnectedReference = data.allConnectedReference;
		this.pacifistFlag = data.pacifistFlag;
		this.notAScratchFlag = data.notAScratchFlag;
		this.spareNoOneReference = data.spareNoOneReference;
	}

	// Token: 0x0600151C RID: 5404 RVA: 0x00134074 File Offset: 0x00132274
	[Button("Testing: KO Everybody", 0)]
	public void TestKOEverybody()
	{
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			citizen.RecieveDamage(99999f, Player.Instance, Vector2.zero, Vector2.zero, null, null, SpatterSimulation.EraseMode.useDespawnTime, true, false, 0f, 1f, false, true, 1f);
		}
	}

	// Token: 0x04001A06 RID: 6662
	[Header("Achievement Flags")]
	[InfoBox("Flags/data that is used to help detect achievements", 0)]
	[Tooltip("Used to help detect whether we are escaping from hospital without paying...")]
	public bool freeHealthCareFlag;

	// Token: 0x04001A07 RID: 6663
	[Tooltip("Used to help detect whether we are participating in a murder case without using violence. The number is a reference to a case ID, while active the player is not allowed to use violence...")]
	public int notTheAnswerFlag = -1;

	// Token: 0x04001A08 RID: 6664
	[Tooltip("As above but seen trespassing instead of violence caused")]
	public int privateSlyFlag = -1;

	// Token: 0x04001A09 RID: 6665
	[Tooltip("Records how many unique things we have pinned in a single game")]
	public List<string> allConnectedReference = new List<string>();

	// Token: 0x04001A0A RID: 6666
	[Tooltip("If at any point the player KO's anyone in their game, this is set to true and the achievement cannot complete")]
	public bool pacifistFlag;

	// Token: 0x04001A0B RID: 6667
	[Tooltip("If at any point the player is KO'd, this is set to true and the achievement cannot complete")]
	public bool notAScratchFlag;

	// Token: 0x04001A0C RID: 6668
	[Tooltip("Tracks who the player has KO'd in this game by citizen ID. Used for the achivement for KO'ing everybody")]
	public List<int> spareNoOneReference = new List<int>();

	// Token: 0x04001A0D RID: 6669
	private static AchievementsController _instance;
}
