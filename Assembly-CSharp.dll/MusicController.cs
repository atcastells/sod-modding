using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000396 RID: 918
public class MusicController : MonoBehaviour
{
	// Token: 0x17000099 RID: 153
	// (get) Token: 0x060014E4 RID: 5348 RVA: 0x00131A80 File Offset: 0x0012FC80
	public static MusicController Instance
	{
		get
		{
			return MusicController._instance;
		}
	}

	// Token: 0x060014E5 RID: 5349 RVA: 0x00131A88 File Offset: 0x0012FC88
	private void Awake()
	{
		if (MusicController._instance != null && MusicController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			MusicController._instance = this;
		}
		this.cues = AssetLoader.Instance.GetAllMusicCues();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x060014E6 RID: 5350 RVA: 0x00131ADD File Offset: 0x0012FCDD
	public void SetGameState(MusicCue.MusicTriggerGameState newGameState)
	{
		if (this.currentGameState != newGameState)
		{
			Game.Log("Audio: Music: Switch game state to " + newGameState.ToString(), 2);
			this.currentGameState = newGameState;
			this.MusicTriggerCheck(MusicCue.MusicTriggerEvent.none);
		}
	}

	// Token: 0x060014E7 RID: 5351 RVA: 0x00131B13 File Offset: 0x0012FD13
	public void SetPlayerState(MusicCue.MusicTriggerPlayerState newPlayerState)
	{
		if (this.currentPlayerSate != newPlayerState)
		{
			Game.Log("Audio: Music: Switch player state to " + newPlayerState.ToString(), 2);
			this.currentPlayerSate = newPlayerState;
			this.MusicTriggerCheck(MusicCue.MusicTriggerEvent.none);
		}
	}

	// Token: 0x060014E8 RID: 5352 RVA: 0x00131B49 File Offset: 0x0012FD49
	public void SetPlayerLocation(MusicCue.MusicTriggerPlayerLocation newPlayerLocation)
	{
		if (this.currentPlayerLocation != newPlayerLocation)
		{
			Game.Log("Audio: Music: Switch player location to " + newPlayerLocation.ToString(), 2);
			this.currentPlayerLocation = newPlayerLocation;
			this.MusicTriggerCheck(MusicCue.MusicTriggerEvent.none);
		}
	}

	// Token: 0x060014E9 RID: 5353 RVA: 0x00131B80 File Offset: 0x0012FD80
	public void MusicTriggerCheck(MusicCue.MusicTriggerEvent passEvent = MusicCue.MusicTriggerEvent.none)
	{
		if (Game.Instance != null)
		{
			Game.Log("Audio: Music: Executing music check...", 2);
		}
		this.currentValidCues = new List<MusicCue>();
		foreach (MusicCue musicCue in this.cues)
		{
			if (!musicCue.disabled && (!musicCue.playOnce || !this.playedOnceTracks.Contains(musicCue)))
			{
				foreach (MusicCue.MusicTrigger trigger in musicCue.triggers)
				{
					if (musicCue.debug && Game.Instance.collectDebugData)
					{
						Game.Log("Audio: Checking trigger for " + musicCue.presetName + "...", 2);
					}
					if (this.IsTriggerValid(trigger, passEvent, musicCue.debug) && !this.currentValidCues.Contains(musicCue))
					{
						this.currentValidCues.Add(musicCue);
					}
				}
			}
		}
		this.currentValidCues.Sort((MusicCue p1, MusicCue p2) => ((float)p2.ambientPriority + Toolbox.Instance.Rand(-0.05f, 0.05f, false) + this.GetPreviouslyPlayedBias(p2) * 0.5f).CompareTo((float)p1.ambientPriority + Toolbox.Instance.Rand(-0.05f, 0.05f, false) + this.GetPreviouslyPlayedBias(p1) * 0.5f));
		List<MusicCue> list = new List<MusicCue>();
		foreach (KeyValuePair<MusicCue, EventInstance> keyValuePair in this.activeTracks)
		{
			PLAYBACK_STATE playback_STATE = 2;
			keyValuePair.Value.getPlaybackState(ref playback_STATE);
			string text = "Audio: Playback state of ";
			MusicCue key = keyValuePair.Key;
			Game.Log(text + ((key != null) ? key.ToString() : null) + " is " + playback_STATE.ToString(), 2);
			if ((playback_STATE == null || playback_STATE == 3 || playback_STATE == 1) && keyValuePair.Key.stopOnIncompatibleStateSwitch && !this.currentValidCues.Contains(keyValuePair.Key))
			{
				Game.Log("Audio: " + keyValuePair.Key.name + " is incompatible, stopping...", 2);
				keyValuePair.Value.stop(0);
				keyValuePair.Value.release();
				list.Add(keyValuePair.Key);
				this.isPlaying = false;
			}
		}
		foreach (MusicCue musicCue2 in list)
		{
			this.activeTracks.Remove(musicCue2);
			this.activeCuePresets.Remove(musicCue2);
		}
		for (int i = 0; i < this.currentValidCues.Count; i++)
		{
			MusicCue musicCue3 = this.currentValidCues[i];
			if (!this.isPlaying || musicCue3.interrupt)
			{
				this.PlayNewTrack(musicCue3, true);
				return;
			}
		}
	}

	// Token: 0x060014EA RID: 5354 RVA: 0x00131E7C File Offset: 0x0013007C
	public bool IsTriggerValid(MusicCue.MusicTrigger trigger, MusicCue.MusicTriggerEvent passEvent, bool debug)
	{
		if (passEvent != MusicCue.MusicTriggerEvent.none)
		{
			if (passEvent != trigger.onEvent)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Music cue not valid: Wrong event", 2);
				}
				return false;
			}
			if (Toolbox.Instance.Rand(0f, 1f, false) > trigger.eventTriggerChance)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Event chance failed", 2);
				}
				return false;
			}
		}
		else if (trigger.triggerOnlyOnEvents)
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Cue will only trigger on events", 2);
			}
			return false;
		}
		if (!trigger.ignoreSilentTimeBetweenTracks && this.nextTrackTriggerTime > 0f)
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Waiting for delay between tracks", 2);
			}
			return false;
		}
		if (trigger.onGameState != MusicCue.MusicTriggerGameState.any && trigger.onGameState != this.currentGameState)
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Game state " + this.currentGameState.ToString() + " does not match " + trigger.onGameState.ToString(), 2);
			}
			return false;
		}
		if (trigger.onPlayerSate != MusicCue.MusicTriggerPlayerState.any && trigger.onPlayerSate != this.currentPlayerSate)
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Player state " + this.currentPlayerSate.ToString() + " does not match " + trigger.onPlayerSate.ToString(), 2);
			}
			return false;
		}
		if (trigger.onPlayerLocation != MusicCue.MusicTriggerPlayerLocation.any && trigger.onPlayerLocation != this.currentPlayerLocation)
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Player location state " + this.currentPlayerLocation.ToString() + " does not match " + trigger.onPlayerLocation.ToString(), 2);
			}
			return false;
		}
		if (trigger.onlyInDistricts)
		{
			if (Player.Instance.currentGameLocation == null || Player.Instance.currentGameLocation.district == null || !trigger.compatibleDistricts.Contains(Player.Instance.currentGameLocation.district.preset))
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid district", 2);
				}
				return false;
			}
		}
		else if (trigger.excludeDistricts && Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.district != null && trigger.excludedDistricts.Contains(Player.Instance.currentGameLocation.district.preset))
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Invalid district", 2);
			}
			return false;
		}
		if (trigger.onlyInBuildings)
		{
			if (Player.Instance.currentGameLocation == null || Player.Instance.currentGameLocation.building == null || !trigger.compatibleBuildings.Contains(Player.Instance.currentGameLocation.building.preset))
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid building", 2);
				}
				return false;
			}
		}
		else if (trigger.excludeBuildings && Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.building != null && trigger.excludedBuildings.Contains(Player.Instance.currentGameLocation.building.preset))
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Invalid building", 2);
			}
			return false;
		}
		if (trigger.onlyInLocations)
		{
			if (Player.Instance.currentGameLocation == null || Player.Instance.currentGameLocation.thisAsAddress == null || Player.Instance.currentGameLocation.thisAsAddress.addressPreset == null || !trigger.compatibleAddressTypes.Contains(Player.Instance.currentGameLocation.thisAsAddress.addressPreset))
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid location", 2);
				}
				return false;
			}
		}
		else if (trigger.excludeLocations && Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.addressPreset != null && trigger.excludedAddressTypes.Contains(Player.Instance.currentGameLocation.thisAsAddress.addressPreset))
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Invalid location", 2);
			}
			return false;
		}
		if (trigger.onlyDuringStatuses)
		{
			bool flag = false;
			foreach (StatusPreset statusPreset in StatusController.Instance.activeStatuses)
			{
				if (trigger.compatibleStatuses.Contains(statusPreset))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid status", 2);
				}
				return false;
			}
		}
		else if (trigger.excludeStatuses)
		{
			bool flag2 = true;
			foreach (StatusPreset statusPreset2 in StatusController.Instance.activeStatuses)
			{
				if (trigger.compatibleStatuses.Contains(statusPreset2))
				{
					flag2 = false;
					break;
				}
			}
			if (!flag2)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid status", 2);
				}
				return false;
			}
		}
		if (trigger.floorRanges.Count > 0 && Player.Instance.currentNode != null)
		{
			bool flag3 = false;
			foreach (Vector2 vector in trigger.floorRanges)
			{
				if (Player.Instance.currentNode.nodeCoord.z >= Mathf.RoundToInt(vector.x) && Player.Instance.currentNode.nodeCoord.z <= Mathf.RoundToInt(vector.y))
				{
					flag3 = true;
					break;
				}
			}
			if (!flag3)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid floor range", 2);
				}
				return false;
			}
		}
		if (trigger.timeRanges.Count > 0)
		{
			bool flag4 = false;
			foreach (Vector2 vector2 in trigger.timeRanges)
			{
				if (SessionData.Instance.decimalClock >= vector2.x && SessionData.Instance.decimalClock <= vector2.y)
				{
					flag4 = true;
					break;
				}
			}
			if (!flag4)
			{
				if (debug && Game.Instance.collectDebugData)
				{
					Game.Log("Audio: Music cue not valid: Invalid time range", 2);
				}
				return false;
			}
		}
		if (trigger.useDecorGrimeRange && Player.Instance.currentRoom != null && !Player.Instance.currentRoom.isNullRoom && Player.Instance.currentRoom.wallMat != null && (Player.Instance.currentRoom.defaultWallKey.grubiness < trigger.grimeRange.x || Player.Instance.currentRoom.defaultWallKey.grubiness > trigger.grimeRange.y))
		{
			if (debug && Game.Instance.collectDebugData)
			{
				Game.Log("Audio: Music cue not valid: Invalid grime range", 2);
			}
			return false;
		}
		if (debug && Game.Instance.collectDebugData)
		{
			Game.Log("Audio: Music cue is valid", 2);
		}
		return true;
	}

	// Token: 0x060014EB RID: 5355 RVA: 0x00132654 File Offset: 0x00130854
	private float GetPreviouslyPlayedBias(MusicCue cue)
	{
		float result = 1f;
		if (this.previousTracks.Contains(cue))
		{
			int num = this.previousTracks.IndexOf(cue);
			if (num > 0)
			{
				result = 1f - (float)num / (float)(this.previousTracks.Count - 1);
			}
		}
		return result;
	}

	// Token: 0x060014EC RID: 5356 RVA: 0x001326A0 File Offset: 0x001308A0
	public void PlayNewTrack(MusicCue newTrack, bool interupt = false)
	{
		if (!this.enableMusic)
		{
			return;
		}
		this.nextTrackTriggerTime = Toolbox.Instance.Rand(this.silenceBetweenTracks.x, this.silenceBetweenTracks.y, false);
		if (interupt)
		{
			this.StopCurrentTrack();
		}
		EventInstance eventInstance = RuntimeManager.CreateInstance(GUID.Parse(newTrack.fmodGUID));
		eventInstance.start();
		this.activeTracks.Add(newTrack, eventInstance);
		this.activeCuePresets.Add(newTrack);
		this.previousTracks.Remove(newTrack);
		this.previousTracks.Add(newTrack);
		Game.Log("Audio: Play new ambient track: " + newTrack.name, 2);
		if (newTrack.playOnce)
		{
			this.playedOnceTracks.Add(newTrack);
		}
	}

	// Token: 0x060014ED RID: 5357 RVA: 0x0013275C File Offset: 0x0013095C
	private void Update()
	{
		if (!this.enableMusic)
		{
			return;
		}
		this.isPlaying = false;
		List<MusicCue> list = new List<MusicCue>();
		foreach (KeyValuePair<MusicCue, EventInstance> keyValuePair in this.activeTracks)
		{
			EventInstance value = keyValuePair.Value;
			PLAYBACK_STATE playback_STATE = 2;
			value.getPlaybackState(ref playback_STATE);
			if (playback_STATE != 2)
			{
				this.isPlaying = true;
				break;
			}
			keyValuePair.Value.release();
			list.Add(keyValuePair.Key);
		}
		foreach (MusicCue musicCue in list)
		{
			this.activeTracks.Remove(musicCue);
			this.activeCuePresets.Remove(musicCue);
		}
		if (!this.isPlaying)
		{
			if (this.nextTrackTriggerTime > 0f)
			{
				this.nextTrackTriggerTime -= Time.deltaTime;
				return;
			}
			this.MusicTriggerCheck(MusicCue.MusicTriggerEvent.none);
		}
	}

	// Token: 0x060014EE RID: 5358 RVA: 0x00132884 File Offset: 0x00130A84
	[Button(null, 0)]
	public void StopCurrentTrack()
	{
		List<MusicCue> list = new List<MusicCue>();
		foreach (KeyValuePair<MusicCue, EventInstance> keyValuePair in this.activeTracks)
		{
			keyValuePair.Value.stop(0);
			keyValuePair.Value.release();
			list.Add(keyValuePair.Key);
		}
		foreach (MusicCue musicCue in list)
		{
			this.activeTracks.Remove(musicCue);
			this.activeCuePresets.Remove(musicCue);
		}
	}

	// Token: 0x060014EF RID: 5359 RVA: 0x00132958 File Offset: 0x00130B58
	[Button(null, 0)]
	public void ForceNextTrack()
	{
		this.nextTrackTriggerTime = 0f;
	}

	// Token: 0x040019AC RID: 6572
	[NonSerialized]
	public List<MusicCue> cues;

	// Token: 0x040019AD RID: 6573
	[Header("Settings")]
	public bool enableMusic = true;

	// Token: 0x040019AE RID: 6574
	[Header("Seconds Between Tracks")]
	public Vector2 silenceBetweenTracks = new Vector2(120f, 360f);

	// Token: 0x040019AF RID: 6575
	[Header("State")]
	[InfoBox("Current valid cues that the game will select from", 0)]
	public List<MusicCue> currentValidCues = new List<MusicCue>();

	// Token: 0x040019B0 RID: 6576
	[InfoBox("List of tracks that have already been played and are marked to play only once", 0)]
	public List<MusicCue> playedOnceTracks = new List<MusicCue>();

	// Token: 0x040019B1 RID: 6577
	public bool isPlaying;

	// Token: 0x040019B2 RID: 6578
	public float nextTrackTriggerTime;

	// Token: 0x040019B3 RID: 6579
	[Space(7f)]
	public MusicCue.MusicTriggerGameState currentGameState;

	// Token: 0x040019B4 RID: 6580
	public MusicCue.MusicTriggerPlayerState currentPlayerSate;

	// Token: 0x040019B5 RID: 6581
	public MusicCue.MusicTriggerPlayerLocation currentPlayerLocation;

	// Token: 0x040019B6 RID: 6582
	[Space(7f)]
	[InfoBox("Used to determine priorities when avoiding repeating tracks", 0)]
	public List<MusicCue> previousTracks = new List<MusicCue>();

	// Token: 0x040019B7 RID: 6583
	private Dictionary<MusicCue, EventInstance> activeTracks = new Dictionary<MusicCue, EventInstance>();

	// Token: 0x040019B8 RID: 6584
	public List<MusicCue> activeCuePresets = new List<MusicCue>();

	// Token: 0x040019B9 RID: 6585
	private static MusicController _instance;
}
