using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

// Token: 0x02000256 RID: 598
public class Chapter : MonoBehaviour
{
	// Token: 0x06000D44 RID: 3396 RVA: 0x000BB930 File Offset: 0x000B9B30
	public void Awake()
	{
		if (CityConstructor.Instance.saveState == null)
		{
			this.loadedFromSave = false;
		}
		else
		{
			Game.Log("Chapter: Loading chapter save data...", 2);
			this.LoadStateSaveData(CityConstructor.Instance.saveState.chapterSaveState);
			this.loadedFromSave = true;
		}
		this.OnLoaded();
		if (SessionData.Instance.startedGame && (CityConstructor.Instance == null || !CityConstructor.Instance.preSimActive))
		{
			this.OnGameStart();
			return;
		}
		CityConstructor.Instance.OnLoadFinalize += this.OnLoadFinalize;
		CityConstructor.Instance.OnGameStarted += this.OnGameStart;
		ChapterController.Instance.OnNewPart += this.OnNewChapterPart;
	}

	// Token: 0x06000D45 RID: 3397 RVA: 0x000BB9F0 File Offset: 0x000B9BF0
	public virtual void OnLoaded()
	{
		string text = "Chapter: OnLoad: ";
		ChapterPreset loadedChapter = ChapterController.Instance.loadedChapter;
		Game.Log(text + ((loadedChapter != null) ? loadedChapter.ToString() : null), 2);
		this.preset = ChapterController.Instance.loadedChapter;
		ChapterController.Instance.chapterScript = this;
		this.thisCase = CasePanelController.Instance.activeCases.Find((Case item) => item.caseType == Case.CaseType.mainStory && item.mainStoryChapter == this.preset.name);
		if (this.thisCase == null)
		{
			Game.Log("Chapter: Creating new case for chapter " + this.preset.name, 2);
			this.thisCase = CasePanelController.Instance.CreateNewCase(Case.CaseType.mainStory, Case.CaseStatus.forced, true, Strings.Get("ui.chapters", this.preset.name, Strings.Casing.asIs, false, false, false, null));
			this.thisCase.mainStoryChapter = this.preset.name;
			MurderController.Instance.currentActiveCase = this.thisCase;
		}
		if (!this.loadedFromSave)
		{
			SessionData.Instance.SetWeather(ChapterController.Instance.loadedChapter.rainAmount, ChapterController.Instance.loadedChapter.windAmount, ChapterController.Instance.loadedChapter.snowAmount, ChapterController.Instance.loadedChapter.lightningAmount, ChapterController.Instance.loadedChapter.fogAmount, ChapterController.Instance.loadedChapter.transitionSpeed, false);
		}
		Game.Log(string.Concat(new string[]
		{
			"CityGen: Chapter ",
			this.preset.name,
			" (",
			this.preset.chapterNumber.ToString(),
			") loaded..."
		}), 2);
	}

	// Token: 0x06000D46 RID: 3398 RVA: 0x000BBB8C File Offset: 0x000B9D8C
	public virtual void OnLoadFinalize()
	{
		Game.Log("Chapter: OnLoadFinalize", 2);
	}

	// Token: 0x06000D47 RID: 3399 RVA: 0x000BBB99 File Offset: 0x000B9D99
	public virtual void OnObjectsCreated()
	{
		Game.Log("Chapter: OnObjectsCreated", 2);
	}

	// Token: 0x06000D48 RID: 3400 RVA: 0x000BBBA8 File Offset: 0x000B9DA8
	public virtual void OnGameStart()
	{
		Game.Log("Chapter: OnGameStart", 2);
		CityConstructor.Instance.OnGameStarted -= this.OnGameStart;
		this.gameStart = true;
		if (ChapterController.Instance.loadFirstPartOnStart)
		{
			if (Game.Instance.demoMode && Game.Instance.demoChapterSkip)
			{
				ChapterController.Instance.SkipToChapterPart(21, true, false);
			}
			else
			{
				ChapterController.Instance.SkipToChapterPart(this.preset.startingPart, true, false);
			}
		}
		Player.Instance.nourishment = 0.7f;
		Player.Instance.hydration = 0.7f;
		Player.Instance.energy = 0.7f;
		CitizenBehaviour.Instance.OnGameWorldLoop += this.OnGameWorldLoop;
	}

	// Token: 0x06000D49 RID: 3401 RVA: 0x000BBC70 File Offset: 0x000B9E70
	public void ClearAllObjectives()
	{
		if (this.gameStart)
		{
			Game.Log("Chapter: Clearing all objectives for " + this.preset.name + "...", 2);
			Player.Instance.speechController.speechQueue.Clear();
			if (Player.Instance.speechController.activeSpeechBubble != null)
			{
				Object.Destroy(Player.Instance.speechController.activeSpeechBubble.gameObject);
				Player.Instance.speechController.activeSpeechBubble = null;
			}
			Player.Instance.speechController.speechDelay = 0f;
			for (int i = 0; i < this.thisCase.currentActiveObjectives.Count; i++)
			{
				if (!this.thisCase.currentActiveObjectives[i].isCancelled)
				{
					this.thisCase.currentActiveObjectives[i].Cancel();
				}
			}
		}
	}

	// Token: 0x06000D4A RID: 3402 RVA: 0x000BBD5C File Offset: 0x000B9F5C
	public void ClearObjective(string clearThis)
	{
		Game.Log("Chapter: Clearing objectives named " + clearThis + "...", 2);
		List<SpeechController.QueueElement> list = Player.Instance.speechController.speechQueue.FindAll((SpeechController.QueueElement item) => item.isObjective && item.entryRef.ToLower() == clearThis.ToLower());
		foreach (SpeechController.QueueElement queueElement in list)
		{
			Player.Instance.speechController.speechQueue.Remove(queueElement);
		}
		if (Player.Instance.speechController.activeSpeechBubble != null && list.Contains(Player.Instance.speechController.activeSpeechBubble.speech))
		{
			Object.Destroy(Player.Instance.speechController.activeSpeechBubble.gameObject);
			Player.Instance.speechController.activeSpeechBubble = null;
		}
		for (int i = 0; i < this.thisCase.currentActiveObjectives.Count; i++)
		{
			if (this.thisCase.currentActiveObjectives[i].queueElement.entryRef.ToLower() == clearThis.ToLower() && !this.thisCase.currentActiveObjectives[i].isCancelled)
			{
				this.thisCase.currentActiveObjectives[i].Cancel();
			}
		}
	}

	// Token: 0x06000D4B RID: 3403 RVA: 0x000BBEE0 File Offset: 0x000BA0E0
	public virtual void OnNewChapterPart(bool delay = false, bool teleportPlayer = false)
	{
		Game.Log("Chapter: OnNewChapterPart: " + ChapterController.Instance.currentPartName, 2);
		Game.Log(string.Concat(new string[]
		{
			"CityGen: Chapter part ",
			ChapterController.Instance.currentPartName,
			" (",
			ChapterController.Instance.currentPart.ToString(),
			") loaded..."
		}), 2);
		this.teleportPlayerToChapter = teleportPlayer;
		this.chapterFrameDelay = delay;
		base.StopCoroutine("ChapterActivationDelay");
		base.StartCoroutine("ChapterActivationDelay");
	}

	// Token: 0x06000D4C RID: 3404 RVA: 0x000BBF74 File Offset: 0x000BA174
	private IEnumerator ChapterActivationDelay()
	{
		int frameDelay = 3;
		if (!this.chapterFrameDelay)
		{
			frameDelay = 0;
		}
		MethodInfo method = base.GetType().GetMethod(ChapterController.Instance.currentPartName);
		object[] passed = new object[]
		{
			0
		};
		while (frameDelay > 0)
		{
			int num = frameDelay;
			frameDelay = num - 1;
			yield return null;
		}
		method.Invoke(this, passed);
		if (this.teleportPlayerToChapter)
		{
			if (InteractionController.Instance.lockedInInteraction == null)
			{
				Player.Instance.Teleport(this.currentPartLocation, null, true, false);
			}
			else
			{
				string text = "Chapter: Cancelled teleport as player is using ";
				Interactable lockedInInteraction = InteractionController.Instance.lockedInInteraction;
				Game.Log(text + ((lockedInInteraction != null) ? lockedInInteraction.ToString() : null), 2);
			}
		}
		yield break;
	}

	// Token: 0x06000D4D RID: 3405 RVA: 0x000BBF84 File Offset: 0x000BA184
	private void OnDestroy()
	{
		CityConstructor.Instance.OnLoadFinalize -= this.OnLoadFinalize;
		CityConstructor.Instance.OnGameStarted -= this.OnGameStart;
		ChapterController.Instance.OnNewPart -= this.OnNewChapterPart;
		CitizenBehaviour.Instance.OnGameWorldLoop -= this.OnGameWorldLoop;
		this.invokeOnDelay.Clear();
	}

	// Token: 0x06000D4E RID: 3406 RVA: 0x000BBFF8 File Offset: 0x000BA1F8
	public virtual void OnGameWorldLoop()
	{
		if (SessionData.Instance.play && this.invokeOnDelay.Count > 0)
		{
			float num = SessionData.Instance.gameTime - CitizenBehaviour.Instance.timeOnLastGameWorldUpdate;
			List<string> list = new List<string>();
			foreach (string text in Enumerable.ToList<string>(this.invokeOnDelay.Keys))
			{
				Dictionary<string, float> dictionary = this.invokeOnDelay;
				string text2 = text;
				dictionary[text2] -= num;
				if (this.invokeOnDelay[text] <= 0f)
				{
					base.Invoke(text, 0f);
					list.Add(text);
				}
			}
			foreach (string text3 in list)
			{
				this.invokeOnDelay.Remove(text3);
			}
		}
	}

	// Token: 0x06000D4F RID: 3407 RVA: 0x000BC114 File Offset: 0x000BA314
	public void InvokeAfterDelay(string command, float delayRealSeconds)
	{
		if (!this.invokeOnDelay.ContainsKey(command))
		{
			float num = delayRealSeconds / 0.016667f / 60f / 60f * 0.6f;
			this.invokeOnDelay.Add(command, num);
		}
	}

	// Token: 0x06000D50 RID: 3408 RVA: 0x000BC156 File Offset: 0x000BA356
	public virtual void SetCurrentPartLocation(NewNode newNode)
	{
		if (newNode != null && newNode.gameLocation != null)
		{
			Game.Log("Chapter: Set current part location to " + newNode.gameLocation.name, 2);
		}
		this.currentPartLocation = newNode;
	}

	// Token: 0x06000D51 RID: 3409 RVA: 0x000BC18C File Offset: 0x000BA38C
	public virtual void PlayerVO(string entryRef, float delay = 0f, bool useParsing = true, bool shouting = false, bool interupt = false, bool forceColour = false, Color color = default(Color))
	{
		if (!Player.Instance.speechController.speechQueue.Exists((SpeechController.QueueElement item) => item.dictRef == ChapterController.Instance.loadedChapter.dictionary && item.entryRef == entryRef) && Strings.stringTable.ContainsKey("dds.blocks"))
		{
			if (Strings.stringTable["dds.blocks"].ContainsKey(entryRef))
			{
				Player.Instance.speechController.Speak("dds.blocks", entryRef, useParsing, shouting, interupt, delay, forceColour, color, null, false, false, null, null, null, null);
				return;
			}
			Game.Log("Chapter: Entry " + entryRef + " not found! This may have been deleted...", 2);
		}
	}

	// Token: 0x06000D52 RID: 3410 RVA: 0x000BC23C File Offset: 0x000BA43C
	public virtual void AddObjective(string entryRef, List<Objective.ObjectiveTrigger> triggers, bool usePointer = false, Vector3 pointerPosition = default(Vector3), InterfaceControls.Icon useIcon = InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction onCompleteAction = Objective.OnCompleteAction.nextChapterPart, float delay = 0f, bool removePrevious = false, string chapterString = "", bool isSilent = false, bool allowCrouchPromt = false, bool useParsing = true)
	{
		if (this.thisCase != null)
		{
			if (this.thisCase.currentActiveObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef))
			{
				Game.Log("Chapter: Objective " + entryRef + " already exists in case active objectives", 2);
				return;
			}
			if (!this.thisCase.inactiveCurrentObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef))
			{
				if (!this.thisCase.endedObjectives.Exists((Objective item) => item.queueElement.entryRef == entryRef))
				{
					Player.Instance.speechController.speechQueue.Add(new SpeechController.QueueElement(this.thisCase.id, entryRef, usePointer, pointerPosition, useIcon, triggers, onCompleteAction, delay, removePrevious, chapterString, isSilent, allowCrouchPromt, null, false, useParsing));
					Player.Instance.speechController.enabled = true;
					using (List<SpeechController.QueueElement>.Enumerator enumerator = Player.Instance.speechController.speechQueue.FindAll((SpeechController.QueueElement item) => item.forceBottom).GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							SpeechController.QueueElement queueElement = enumerator.Current;
							Player.Instance.speechController.speechQueue.Remove(queueElement);
							Player.Instance.speechController.speechQueue.Add(queueElement);
						}
						return;
					}
				}
				Game.Log("Chapter: Objective " + entryRef + " already exists in case ended objectives", 2);
				return;
			}
			Game.Log("Chapter: Objective " + entryRef + " already exists in case inactive objectives", 2);
			return;
		}
		else
		{
			Game.LogError("Chapter: Trying to create objective " + entryRef + " but no case is assigned to the job!", 2);
		}
	}

	// Token: 0x06000D53 RID: 3411 RVA: 0x000BC41C File Offset: 0x000BA61C
	public virtual void AddObjective(string entryRef, Objective.ObjectiveTrigger trigger, bool usePointer = false, Vector3 pointerPosition = default(Vector3), InterfaceControls.Icon useIcon = InterfaceControls.Icon.lookingGlass, Objective.OnCompleteAction onCompleteAction = Objective.OnCompleteAction.nextChapterPart, float delay = 0f, bool removePrevious = false, string chapterString = "", bool isSilent = false, bool allowCrouchPromt = false)
	{
		if (this.thisCase != null)
		{
			this.thisCase.AddObjective(entryRef, trigger, usePointer, pointerPosition, useIcon, onCompleteAction, delay, removePrevious, chapterString, isSilent, allowCrouchPromt, null, false, false, true);
			return;
		}
		Game.LogError("Chapter: Trying to create objective " + entryRef + " but no case is assigned to the job!", 2);
	}

	// Token: 0x06000D54 RID: 3412 RVA: 0x00003EEE File Offset: 0x000020EE
	public virtual StateSaveData.ChaperStateSave GetChapterSaveData()
	{
		return null;
	}

	// Token: 0x06000D55 RID: 3413 RVA: 0x00002265 File Offset: 0x00000465
	public virtual void LoadStateSaveData(StateSaveData.ChaperStateSave newData)
	{
	}

	// Token: 0x06000D56 RID: 3414 RVA: 0x000BC46C File Offset: 0x000BA66C
	public Interactable LoadInteractableFromData(string reference, ref StateSaveData.ChaperStateSave saveData)
	{
		Interactable interactable = null;
		int id = saveData.GetDataInt(reference);
		if (id > -1 && !CityData.Instance.savableInteractableDictionary.TryGetValue(id, ref interactable))
		{
			interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == id);
			if (interactable == null)
			{
				Game.LogError(string.Concat(new string[]
				{
					"Chapter: Unable to load object ",
					reference,
					" for chapter, ID ",
					id.ToString(),
					" out of ",
					CityData.Instance.savableInteractableDictionary.Count.ToString(),
					" interactables"
				}), 2);
			}
		}
		return interactable;
	}

	// Token: 0x04000F83 RID: 3971
	public ChapterPreset preset;

	// Token: 0x04000F84 RID: 3972
	[NonSerialized]
	public Case thisCase;

	// Token: 0x04000F85 RID: 3973
	public bool loadedFromSave;

	// Token: 0x04000F86 RID: 3974
	public bool gameStart;

	// Token: 0x04000F87 RID: 3975
	public float blackTimer;

	// Token: 0x04000F88 RID: 3976
	public float blurTimer;

	// Token: 0x04000F89 RID: 3977
	public float blackFade = 1f;

	// Token: 0x04000F8A RID: 3978
	public float blurFade = 1f;

	// Token: 0x04000F8B RID: 3979
	public NewNode currentPartLocation;

	// Token: 0x04000F8C RID: 3980
	private bool teleportPlayerToChapter;

	// Token: 0x04000F8D RID: 3981
	private bool chapterFrameDelay;

	// Token: 0x04000F8E RID: 3982
	public Dictionary<string, float> invokeOnDelay = new Dictionary<string, float>();
}
