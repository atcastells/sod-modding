using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020002AC RID: 684
public class SurveillanceApp : CruncherAppContent
{
	// Token: 0x06000F2D RID: 3885 RVA: 0x000D8694 File Offset: 0x000D6894
	public override void OnSetup()
	{
		base.OnSetup();
		this.titleText.text = Strings.Get("computer", "Surveillance", Strings.Casing.asIs, false, false, false, null);
		this.printButtonText.text = Strings.Get("computer", "Print", Strings.Casing.asIs, false, false, false, null);
		this.yesterdayText.text = Strings.Get("computer", "Yesterday", Strings.Casing.upperCase, false, false, false, null);
		this.todayText.text = Strings.Get("computer", "Today", Strings.Casing.upperCase, false, false, false, null);
		this.cameras.AddRange(this.controller.ic.interactable.node.gameLocation.securityCameras);
		if (this.controller.ic.interactable.node.gameLocation.thisAsAddress != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company != null && this.controller.ic.interactable.node.gameLocation.thisAsAddress.company.preset.controlsBuildingSurveillance)
		{
			foreach (NewAddress newAddress in this.controller.ic.interactable.node.building.lobbies)
			{
				this.cameras.AddRange(newAddress.securityCameras);
			}
		}
		this.timestampText.text = string.Empty;
		this.timestampTextShadow.text = this.timestampText.text;
		this.locationstampText.text = string.Empty;
		this.locationstampTextShadow.text = this.locationstampText.text;
		this.SetScene(null);
		if (this.cameras.Count > 0)
		{
			this.SetCamera(this.cameras[0]);
		}
	}

	// Token: 0x06000F2E RID: 3886 RVA: 0x000D88AC File Offset: 0x000D6AAC
	public void SetCamera(Interactable newSelection)
	{
		if (this.selectedCamera != null)
		{
			this.selectedCamera.sceneRecorder.OnNewCapture -= this.OnSelectedCameraNewCapture;
		}
		this.selectedCamera = newSelection;
		if (this.dispayYesterday)
		{
			int num = SessionData.Instance.dayInt - 1;
			if (num < 0)
			{
				num = 6;
			}
			num = Mathf.Clamp(num, 0, 6);
			TMP_Text tmp_Text = this.yesterdayText;
			string text = ">";
			string dictionary = "ui.interface";
			SessionData.WeekDay weekDay = (SessionData.WeekDay)num;
			tmp_Text.text = text + Strings.Get(dictionary, weekDay.ToString(), Strings.Casing.asIs, false, false, false, null) + "<";
			this.todayText.text = Strings.Get("ui.interface", SessionData.Instance.day.ToString(), Strings.Casing.asIs, false, false, false, null);
		}
		else
		{
			int num2 = SessionData.Instance.dayInt - 1;
			if (num2 < 0)
			{
				num2 = 6;
			}
			num2 = Mathf.Clamp(num2, 0, 6);
			TMP_Text tmp_Text2 = this.yesterdayText;
			string dictionary2 = "ui.interface";
			SessionData.WeekDay weekDay = (SessionData.WeekDay)num2;
			tmp_Text2.text = Strings.Get(dictionary2, weekDay.ToString(), Strings.Casing.asIs, false, false, false, null);
			this.todayText.text = ">" + Strings.Get("ui.interface", SessionData.Instance.day.ToString(), Strings.Casing.asIs, false, false, false, null) + "<";
		}
		while (this.spawnedTimelineEntries.Count > 0)
		{
			Object.Destroy(this.spawnedTimelineEntries[0].gameObject);
			this.spawnedTimelineEntries.RemoveAt(0);
		}
		this.loadedCaptures.Clear();
		if (this.selectedCamera != null)
		{
			Game.Log("Gameplay: Surveillance set new camera: " + this.selectedCamera.node.room.name, 2);
			this.selectedCamera.sceneRecorder.OnNewCapture += this.OnSelectedCameraNewCapture;
			this.selectedSentries.Clear();
			if (this.selectedCamera.node.gameLocation.thisAsAddress != null)
			{
				this.selectedSentries.AddRange(this.selectedCamera.node.gameLocation.thisAsAddress.sentryGuns.FindAll((Interactable item) => item.node.room == this.selectedCamera.node.room));
			}
			string text2 = string.Empty;
			if (this.selectedCamera.node.floor != null)
			{
				text2 = Strings.Get("names.rooms", "floor_" + this.selectedCamera.node.floor.floor.ToString(), Strings.Casing.asIs, false, false, false, null);
			}
			this.cameraSelectionText.text = Strings.Get("names.rooms", this.selectedCamera.node.room.preset.name, Strings.Casing.asIs, false, false, false, null) + " " + text2;
			this.titleText.text = Strings.Get("computer", "Surveillance", Strings.Casing.asIs, false, false, false, null);
			float num3 = SessionData.Instance.gameTime - SessionData.Instance.decimalClock;
			for (int i = 0; i < this.selectedCamera.sceneRecorder.interactable.cap.Count; i++)
			{
				if (this.selectedCamera.sceneRecorder.interactable.cap[i].t >= num3)
				{
					if (!this.dispayYesterday)
					{
						this.loadedCaptures.Add(this.selectedCamera.sceneRecorder.interactable.cap[i]);
					}
				}
				else if (this.dispayYesterday)
				{
					this.loadedCaptures.Add(this.selectedCamera.sceneRecorder.interactable.cap[i]);
				}
			}
			if (this.loadedCaptures.Count > 0)
			{
				this.timelineScale = new Vector2(this.loadedCaptures[0].t, SessionData.Instance.gameTime);
			}
			this.timelineCovers = this.timelineScale.y - this.timelineScale.x;
			foreach (SceneRecorder.SceneCapture sceneCapture in this.loadedCaptures)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(this.timelineEntryPrefab, this.timelineRect);
				RectTransform component = gameObject.GetComponent<RectTransform>();
				float num4 = (sceneCapture.t - this.timelineScale.x) / this.timelineCovers;
				component.anchoredPosition = new Vector2(num4 * this.timelineRect.rect.width, 0f);
				CruncherTimelineEntry component2 = gameObject.GetComponent<CruncherTimelineEntry>();
				component2.Setup(this, sceneCapture);
				this.spawnedTimelineEntries.Add(component2);
			}
			this.timelineScrub.anchoredPosition = new Vector2((this.scrubTime - this.timelineScale.x) / this.timelineCovers * this.timelineRect.rect.width - this.timelineRect.rect.width * 0.5f, 0f);
			this.timelineScrub.anchoredPosition = new Vector2(Mathf.Clamp(this.timelineScrub.anchoredPosition.x, this.timelineRect.rect.width * -0.5f, this.timelineRect.rect.width * 0.5f), 0f);
			this.timelineScrub.SetAsLastSibling();
			this.yesterdayButton.interactable = true;
			this.todayButton.interactable = true;
			this.UpdateCamStatus();
			this.UpdateTimelineFlagging();
		}
		else
		{
			Game.Log("Gameplay: Surveillance set null camera", 2);
			this.SetScene(null);
			this.cameraSelectionText.text = "-";
			this.selectedSentries.Clear();
			this.yesterdayButton.interactable = false;
			this.todayButton.interactable = false;
		}
		this.UpdateScrub(true, this.currentSceneGametime);
	}

	// Token: 0x06000F2F RID: 3887 RVA: 0x000D8EAC File Offset: 0x000D70AC
	private void OnSelectedCameraNewCapture()
	{
		Game.Log("Gameplay: Selected camera has a new capture...", 2);
		this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.1f, 0.3f, false), 0.33f);
		this.SetCamera(this.selectedCamera);
	}

	// Token: 0x06000F30 RID: 3888 RVA: 0x000D8EEA File Offset: 0x000D70EA
	private void OnDestroy()
	{
		SceneCapture.Instance.currrentlyViewing = null;
		if (this.selectedCamera != null)
		{
			this.selectedCamera.sceneRecorder.OnNewCapture -= this.OnSelectedCameraNewCapture;
		}
	}

	// Token: 0x06000F31 RID: 3889 RVA: 0x000D8F1C File Offset: 0x000D711C
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			if (Player.Instance.computerInteractable == this.controller.ic.interactable)
			{
				if (this.controller.currentHover == this.scrubUI && !this.scrubMove && Input.GetMouseButtonDown(0))
				{
					this.scrubOffset = this.controller.cursorRect.position - this.timelineScrub.position;
					this.scrubMove = true;
				}
				if (this.scrubMove)
				{
					if (!Input.GetMouseButton(0))
					{
						this.scrubMove = false;
					}
					else
					{
						this.timelineScrub.position = this.controller.cursorRect.position - this.scrubOffset;
						this.timelineScrub.anchoredPosition = new Vector2(Mathf.Clamp(this.timelineScrub.anchoredPosition.x, this.timelineRect.rect.width * -0.5f, this.timelineRect.rect.width * 0.5f), 0f);
						this.UpdateScrub(false, 0f);
					}
				}
				else if (!InterfaceController.Instance.playerTextInputActive)
				{
					if (InputController.Instance.player.GetButtonDown("NavigateLeft"))
					{
						this.NextCaptureButtion(-1);
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
					}
					else if (InputController.Instance.player.GetButtonDown("NavigateRight"))
					{
						this.NextCaptureButtion(1);
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
					}
					else if (InputController.Instance.player.GetButtonDown("NavigateUp"))
					{
						this.CameraSelection(-1);
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
					}
					else if (InputController.Instance.player.GetButtonDown("NavigateDown"))
					{
						this.CameraSelection(1);
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerKeyboardKey, Player.Instance, Player.Instance.currentNode, base.transform.position, null, null, 1f, null, false, null, false);
					}
				}
			}
			this.hoveredActor = null;
			foreach (CruncherSurveillanceActorEntry cruncherSurveillanceActorEntry in this.spawnedActorEntries)
			{
				if (this.controller.currentHover != null && cruncherSurveillanceActorEntry.component == this.controller.currentHover)
				{
					this.hoveredActor = cruncherSurveillanceActorEntry;
					cruncherSurveillanceActorEntry.SetOnOver(true, false);
				}
				else
				{
					cruncherSurveillanceActorEntry.SetOnOver(false, false);
				}
			}
			if ((this.hoveredActor != null && this.currentScene != null) || this.actorPageActive)
			{
				if (!this.controller.ic.interactable.sw3)
				{
					this.controller.ic.interactable.SetCustomState3(true, this.hoveredActor.human, false, false, false);
					return;
				}
			}
			else
			{
				if (this.controller.ic.interactable.sw3)
				{
					this.controller.ic.interactable.SetCustomState3(false, null, false, false, false);
				}
				if (this.locator.gameObject.activeSelf)
				{
					this.locator.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x06000F32 RID: 3890 RVA: 0x000D9310 File Offset: 0x000D7510
	public void UpdateScrub(bool forceTime = false, float newScrub = 0f)
	{
		float num = Mathf.Clamp01((this.timelineScrub.anchoredPosition.x + this.timelineRect.rect.width * 0.5f) / this.timelineRect.rect.width);
		this.scrubTime = Mathf.Lerp(this.timelineScale.x, this.timelineScale.y, num);
		if (forceTime)
		{
			this.scrubTime = Mathf.Clamp(newScrub, this.timelineScale.x, this.timelineScale.y);
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Force scrub time of ",
				newScrub.ToString(),
				" (game time: ",
				SessionData.Instance.gameTime.ToString(),
				")"
			}), 2);
			float num2 = Mathf.InverseLerp(this.timelineScale.x, this.timelineScale.y, this.scrubTime);
			Game.Log("Gameplay: Force scrub inverse lerp: " + num2.ToString(), 2);
			this.timelineScrub.anchoredPosition = new Vector2(num2 * this.timelineRect.rect.width - 0.5f * this.timelineRect.rect.width, 0f);
		}
		SceneRecorder.SceneCapture sceneCapture = null;
		foreach (SceneRecorder.SceneCapture sceneCapture2 in this.loadedCaptures)
		{
			if (sceneCapture == null || (sceneCapture2.t >= sceneCapture.t && sceneCapture2.t <= this.scrubTime))
			{
				sceneCapture = sceneCapture2;
			}
		}
		this.SetScene(sceneCapture);
	}

	// Token: 0x06000F33 RID: 3891 RVA: 0x000D94DC File Offset: 0x000D76DC
	public void SetScene(SceneRecorder.SceneCapture newScene)
	{
		if (this.currentScene != newScene && Player.Instance.computerInteractable == this.controller.ic.interactable)
		{
			this.currentScene = newScene;
			this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.2f, 0.5f, false), 0.33f);
			if (this.currentScene != null)
			{
				this.captureDisplay.color = Color.white;
				this.currentSceneGametime = newScene.t;
				this.timestampText.text = SessionData.Instance.TimeAndDate(this.currentSceneGametime, true, true, true);
				this.timestampTextShadow.text = this.timestampText.text;
				if (this.selectedCamera.node.room.isNullRoom)
				{
					this.locationstampText.text = this.selectedCamera.node.gameLocation.name;
				}
				else
				{
					this.locationstampText.text = this.selectedCamera.node.room.GetName();
				}
				this.locationstampTextShadow.text = this.locationstampText.text;
				Game.Log(string.Concat(new string[]
				{
					"Gameplay: Set scene ",
					this.selectedCamera.node.room.name,
					" scene time: ",
					SessionData.Instance.TimeStringOnDay(this.currentScene.t, true, true),
					" (time: ",
					this.timestampText.text,
					")"
				}), 2);
				this.captureDisplay.texture = SceneCapture.Instance.GetSurveillanceScene(this.currentScene, true);
				this.captureDisplay.color = Color.white;
				SceneCapture.Instance.currrentlyViewing = this.currentScene;
			}
			else
			{
				Game.Log("Gameplay: Set scene null", 2);
				this.captureDisplay.color = Color.black;
				SceneCapture.Instance.currrentlyViewing = null;
				this.timestampText.text = string.Empty;
				this.timestampTextShadow.text = this.timestampText.text;
				this.locationstampText.text = string.Empty;
				this.locationstampTextShadow.text = this.locationstampText.text;
				this.currentSceneGametime = -9999f;
			}
			this.actorPage = 0;
			this.UpdateActorList();
		}
	}

	// Token: 0x06000F34 RID: 3892 RVA: 0x000D9744 File Offset: 0x000D7944
	public void CameraSelection(int addSelection)
	{
		int num = this.cameras.IndexOf(this.selectedCamera);
		num += addSelection;
		if (num < 0)
		{
			num += this.cameras.Count;
		}
		else if (num >= this.cameras.Count)
		{
			num -= this.cameras.Count;
		}
		if (this.cameras.Count > 0)
		{
			this.SetCamera(this.cameras[num]);
		}
	}

	// Token: 0x06000F35 RID: 3893 RVA: 0x000D6564 File Offset: 0x000D4764
	public void ExitButton()
	{
		this.controller.OnAppExit();
	}

	// Token: 0x06000F36 RID: 3894 RVA: 0x000D97B8 File Offset: 0x000D79B8
	public void NextCaptureButtion(int val)
	{
		if (this.loadedCaptures.Count > 1)
		{
			int num = this.loadedCaptures.IndexOf(this.currentScene);
			num += val;
			num = Mathf.Clamp(num, 0, this.loadedCaptures.Count - 1);
			this.timelineScrub.anchoredPosition = new Vector2((this.loadedCaptures[num].t - this.timelineScale.x) / this.timelineCovers * this.timelineRect.rect.width - this.timelineRect.rect.width * 0.5f, 0f);
			this.timelineScrub.anchoredPosition = new Vector2(Mathf.Clamp(this.timelineScrub.anchoredPosition.x, this.timelineRect.rect.width * -0.5f, this.timelineRect.rect.width * 0.5f), 0f);
			this.SetScene(this.loadedCaptures[num]);
		}
	}

	// Token: 0x06000F37 RID: 3895 RVA: 0x000D98D8 File Offset: 0x000D7AD8
	public override void PrintButton()
	{
		this.controller.SetTimedLoading(Toolbox.Instance.Rand(0.5f, 1f, false), 0.33f);
		if (this.controller.printedDocument == null && this.currentScene != null && this.controller.printTimer <= 0f)
		{
			this.controller.printTimer = 1f;
			this.controller.printerParent.localPosition = new Vector3(this.controller.printerParent.localPosition.x, this.controller.printerParent.localPosition.y, -0.05f);
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerPrint, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
			this.currentScene.recorder.interactable.sCap.Add(this.currentScene);
			Interactable.Passed passed = new Interactable.Passed(Interactable.PassedVarType.savedSceneCapID, (float)this.currentScene.id, null);
			List<Interactable.Passed> list = new List<Interactable.Passed>();
			list.Add(passed);
			this.controller.printedDocument = InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.surveillancePrintout, Player.Instance, Player.Instance, null, this.controller.printerParent.position, this.controller.ic.transform.eulerAngles, list, null, "");
			if (this.controller.printedDocument != null)
			{
				this.controller.printedDocument.MarkAsTrash(true, false, 0f);
			}
			this.controller.printedDocument.OnRemovedFromWorld += base.OnPlayerTakePrint;
			return;
		}
		AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.computerInvalidPasscode, Player.Instance, this.controller.ic.interactable.node, this.controller.ic.interactable.wPos, null, null, 1f, null, false, null, false);
	}

	// Token: 0x06000F38 RID: 3896 RVA: 0x000D9B04 File Offset: 0x000D7D04
	public void SetCamActiveButton(bool val)
	{
		if (this.selectedCamera != null)
		{
			this.selectedCamera.SetSwitchState(val, Player.Instance, true, false, false);
			if (val)
			{
				this.selectedCamera.SetValue(this.selectedCamera.GetSecurityStrength());
			}
			else
			{
				this.selectedCamera.SetValue(0f);
			}
		}
		this.UpdateCamStatus();
	}

	// Token: 0x06000F39 RID: 3897 RVA: 0x000D9B5E File Offset: 0x000D7D5E
	private void UpdateCamStatus()
	{
		Interactable interactable = this.selectedCamera;
	}

	// Token: 0x06000F3A RID: 3898 RVA: 0x000D9B68 File Offset: 0x000D7D68
	private void UpdateActorList()
	{
		while (this.spawnedActorEntries.Count > 0)
		{
			Object.Destroy(this.spawnedActorEntries[0].gameObject);
			this.spawnedActorEntries.RemoveAt(0);
		}
		this.loadInHeadshots = 0f;
		this.controller.SetTimedLoading(1f, 0.33f);
		List<Human> list = new List<Human>();
		if (this.currentScene != null)
		{
			foreach (SceneRecorder.ActorCapture actorCapture in this.currentScene.aCap)
			{
				Human human = actorCapture.GetHuman();
				if (!human.isPlayer && !list.Contains(human))
				{
					list.Add(human);
				}
			}
		}
		float num = 0.002f;
		int num2 = Mathf.Max(Mathf.FloorToInt((float)(list.Count - 1) / 5f), 0);
		this.actorPage = Mathf.Clamp(this.actorPage, 0, num2);
		int num3 = this.actorPage * 5;
		int num4 = this.actorPage * 5 + 5;
		this.actorPageText.text = (this.actorPage + 1).ToString() + "/" + (num2 + 1).ToString();
		for (int i = Mathf.Min(num3, list.Count); i < Mathf.Min(list.Count, num4); i++)
		{
			Human newHuman = list[i];
			CruncherSurveillanceActorEntry component = Object.Instantiate<GameObject>(this.actorListPrefab, this.actorListRect).GetComponent<CruncherSurveillanceActorEntry>();
			component.Setup(this, newHuman);
			this.spawnedActorEntries.Add(component);
			component.rect.anchoredPosition = new Vector2(num, 0f);
			num += component.rect.sizeDelta.x + 0.002f;
		}
		if (this.actorPage > 0)
		{
			this.actorPrevButton.interactable = true;
		}
		else
		{
			this.actorPrevButton.interactable = false;
		}
		if (this.actorPage < num2)
		{
			this.actorNextButton.interactable = true;
			return;
		}
		this.actorNextButton.interactable = false;
	}

	// Token: 0x06000F3B RID: 3899 RVA: 0x000D9D88 File Offset: 0x000D7F88
	public void SetActorPage(int val)
	{
		this.actorPage += val;
		this.UpdateActorList();
	}

	// Token: 0x06000F3C RID: 3900 RVA: 0x000D9D9E File Offset: 0x000D7F9E
	public void SelectActor(CruncherSurveillanceActorEntry actorButton)
	{
		this.selectedActor = actorButton;
		this.SetActorPage(true, true);
	}

	// Token: 0x06000F3D RID: 3901 RVA: 0x000D9DB0 File Offset: 0x000D7FB0
	public void SetActorPage(bool val, bool forceUpdate = false)
	{
		if (val != this.actorPageActive || forceUpdate)
		{
			this.actorPageActive = val;
			this.selectedActor.UpdateText();
			this.actorImage.texture = this.selectedActor.headshotImg.texture;
			this.actorNameText.text = this.selectedActor.popupText.text;
			this.actorPageRect.gameObject.SetActive(this.actorPageActive);
			this.camDisplayPageRect.gameObject.SetActive(!this.actorPageActive);
			if (this.selectedActor.human.evidenceEntry.GetTiedKeys(Evidence.DataKey.photo).Contains(Evidence.DataKey.name) || GameplayController.Instance.money < 50)
			{
				this.acquireNameButton.interactable = false;
				return;
			}
			if (GameplayController.Instance.money >= 50)
			{
				this.acquireNameButton.interactable = true;
			}
		}
	}

	// Token: 0x06000F3E RID: 3902 RVA: 0x000D9E9A File Offset: 0x000D809A
	public void ActorBackButton()
	{
		this.SetActorPage(false, false);
	}

	// Token: 0x06000F3F RID: 3903 RVA: 0x000D9EA4 File Offset: 0x000D80A4
	public void AcquireNameButton()
	{
		if (GameplayController.Instance.money >= 50)
		{
			this.selectedActor.human.evidenceEntry.MergeDataKeys(Evidence.DataKey.photo, Evidence.DataKey.name);
			GameplayController.Instance.AddMoney(-50, true, "Acquire name");
			this.SetActorPage(true, true);
		}
	}

	// Token: 0x06000F40 RID: 3904 RVA: 0x000D9EF0 File Offset: 0x000D80F0
	public void ToggleFlagOnFootage()
	{
		if (this.selectedActor != null && this.selectedActor != this.flaggedActor)
		{
			this.SetFlaggedActor(this.selectedActor.human);
			return;
		}
		this.SetFlaggedActor(null);
	}

	// Token: 0x06000F41 RID: 3905 RVA: 0x000D9F2C File Offset: 0x000D812C
	public void SetFlaggedActor(Human h)
	{
		if (this.flaggedActor != h)
		{
			this.flaggedActor = h;
			this.UpdateTimelineFlagging();
		}
	}

	// Token: 0x06000F42 RID: 3906 RVA: 0x000D9F4C File Offset: 0x000D814C
	public void UpdateTimelineFlagging()
	{
		foreach (CruncherTimelineEntry cruncherTimelineEntry in this.spawnedTimelineEntries)
		{
			if (this.hoveredActor != null)
			{
				if (cruncherTimelineEntry.sceneReference.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.hoveredActor.human.humanID))
				{
					cruncherTimelineEntry.SetMouseOver(true);
				}
				else
				{
					cruncherTimelineEntry.SetMouseOver(false);
				}
			}
			else
			{
				cruncherTimelineEntry.SetMouseOver(false);
			}
			if (this.flaggedActor != null)
			{
				if (cruncherTimelineEntry.sceneReference.aCap.Exists((SceneRecorder.ActorCapture item) => item.id == this.flaggedActor.humanID))
				{
					cruncherTimelineEntry.SetFlagged(true);
				}
				else
				{
					cruncherTimelineEntry.SetFlagged(false);
				}
			}
			else
			{
				cruncherTimelineEntry.SetFlagged(false);
			}
		}
	}

	// Token: 0x06000F43 RID: 3907 RVA: 0x00002265 File Offset: 0x00000465
	public void SaveToTapeButton()
	{
	}

	// Token: 0x06000F44 RID: 3908 RVA: 0x000DA028 File Offset: 0x000D8228
	public void YesterdayButton()
	{
		if (!this.dispayYesterday)
		{
			this.dispayYesterday = true;
			this.SetCamera(this.selectedCamera);
		}
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x000DA045 File Offset: 0x000D8245
	public void TodayButton()
	{
		if (this.dispayYesterday)
		{
			this.dispayYesterday = false;
			this.SetCamera(this.selectedCamera);
		}
	}

	// Token: 0x0400125F RID: 4703
	[Header("Components")]
	public TextMeshProUGUI titleText;

	// Token: 0x04001260 RID: 4704
	public TextMeshProUGUI cameraSelectionText;

	// Token: 0x04001261 RID: 4705
	public RectTransform timelineRect;

	// Token: 0x04001262 RID: 4706
	public RectTransform timelineScrub;

	// Token: 0x04001263 RID: 4707
	public ComputerOSUIComponent scrubUI;

	// Token: 0x04001264 RID: 4708
	public GameObject timelineEntryPrefab;

	// Token: 0x04001265 RID: 4709
	public TextMeshProUGUI timestampText;

	// Token: 0x04001266 RID: 4710
	public TextMeshProUGUI timestampTextShadow;

	// Token: 0x04001267 RID: 4711
	public TextMeshProUGUI locationstampText;

	// Token: 0x04001268 RID: 4712
	public TextMeshProUGUI locationstampTextShadow;

	// Token: 0x04001269 RID: 4713
	public RenderTexture renderTexturePrefab;

	// Token: 0x0400126A RID: 4714
	public RawImage captureDisplay;

	// Token: 0x0400126B RID: 4715
	public RectTransform captureRect;

	// Token: 0x0400126C RID: 4716
	public Button yesterdayButton;

	// Token: 0x0400126D RID: 4717
	public Button todayButton;

	// Token: 0x0400126E RID: 4718
	public Button actorNextButton;

	// Token: 0x0400126F RID: 4719
	public Button actorPrevButton;

	// Token: 0x04001270 RID: 4720
	public TextMeshProUGUI printButtonText;

	// Token: 0x04001271 RID: 4721
	public RectTransform actorPageRect;

	// Token: 0x04001272 RID: 4722
	public RectTransform camDisplayPageRect;

	// Token: 0x04001273 RID: 4723
	public RawImage actorImage;

	// Token: 0x04001274 RID: 4724
	public TextMeshProUGUI actorNameText;

	// Token: 0x04001275 RID: 4725
	public Button acquireNameButton;

	// Token: 0x04001276 RID: 4726
	public Button actorBackButton;

	// Token: 0x04001277 RID: 4727
	public TextMeshProUGUI actorPageText;

	// Token: 0x04001278 RID: 4728
	public TextMeshProUGUI yesterdayText;

	// Token: 0x04001279 RID: 4729
	public TextMeshProUGUI todayText;

	// Token: 0x0400127A RID: 4730
	[Space(5f)]
	public RectTransform actorListRect;

	// Token: 0x0400127B RID: 4731
	public GameObject actorListPrefab;

	// Token: 0x0400127C RID: 4732
	public RectTransform locator;

	// Token: 0x0400127D RID: 4733
	[Space(7f)]
	public Color timelineColor;

	// Token: 0x0400127E RID: 4734
	public Color timelineMOColor;

	// Token: 0x0400127F RID: 4735
	public Color timelineFlagColor;

	// Token: 0x04001280 RID: 4736
	[Header("State")]
	public List<Interactable> cameras = new List<Interactable>();

	// Token: 0x04001281 RID: 4737
	[NonSerialized]
	public Interactable selectedCamera;

	// Token: 0x04001282 RID: 4738
	public List<Interactable> selectedSentries = new List<Interactable>();

	// Token: 0x04001283 RID: 4739
	public List<SceneRecorder.SceneCapture> loadedCaptures = new List<SceneRecorder.SceneCapture>();

	// Token: 0x04001284 RID: 4740
	private List<CruncherTimelineEntry> spawnedTimelineEntries = new List<CruncherTimelineEntry>();

	// Token: 0x04001285 RID: 4741
	public List<CruncherSurveillanceActorEntry> spawnedActorEntries = new List<CruncherSurveillanceActorEntry>();

	// Token: 0x04001286 RID: 4742
	public CruncherSurveillanceActorEntry hoveredActor;

	// Token: 0x04001287 RID: 4743
	public CruncherSurveillanceActorEntry selectedActor;

	// Token: 0x04001288 RID: 4744
	public Human flaggedActor;

	// Token: 0x04001289 RID: 4745
	public bool dispayYesterday;

	// Token: 0x0400128A RID: 4746
	public Vector2 timelineScale;

	// Token: 0x0400128B RID: 4747
	public float timelineCovers;

	// Token: 0x0400128C RID: 4748
	public SceneRecorder.SceneCapture currentScene;

	// Token: 0x0400128D RID: 4749
	public float currentSceneGametime;

	// Token: 0x0400128E RID: 4750
	public float scrubTime;

	// Token: 0x0400128F RID: 4751
	public int actorPage;

	// Token: 0x04001290 RID: 4752
	public float loadInHeadshots;

	// Token: 0x04001291 RID: 4753
	public bool scrubMove;

	// Token: 0x04001292 RID: 4754
	private Vector3 scrubOffset;

	// Token: 0x04001293 RID: 4755
	public bool actorPageActive;
}
