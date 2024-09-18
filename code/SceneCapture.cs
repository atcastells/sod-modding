using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020004C4 RID: 1220
public class SceneCapture : MonoBehaviour
{
	// Token: 0x170000BE RID: 190
	// (get) Token: 0x06001A62 RID: 6754 RVA: 0x00183E6A File Offset: 0x0018206A
	public static SceneCapture Instance
	{
		get
		{
			return SceneCapture._instance;
		}
	}

	// Token: 0x06001A63 RID: 6755 RVA: 0x00183E71 File Offset: 0x00182071
	private void Awake()
	{
		if (SceneCapture._instance != null && SceneCapture._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SceneCapture._instance = this;
	}

	// Token: 0x06001A64 RID: 6756 RVA: 0x00183E9F File Offset: 0x0018209F
	private void Start()
	{
		this.photoRoomParent.SetActive(false);
	}

	// Token: 0x06001A65 RID: 6757 RVA: 0x00183EB0 File Offset: 0x001820B0
	public Texture2D CapturePhotoOfEvidence(Evidence ev, bool forceNew = false)
	{
		if (!SessionData.Instance.startedGame)
		{
			return null;
		}
		Game.Log("Interface: Request capture for " + ev.GetNameForDataKey(Evidence.DataKey.name) + "...", 2);
		if (this.cachedRenders.ContainsKey(ev) && !forceNew)
		{
			Game.Log("Interface: Capture is cached already...", 2);
			this.cachedRenders[ev].lastUsed = SessionData.Instance.gameTime;
			return this.cachedRenders[ev].img;
		}
		if (this.cachedRenders.Count >= this.maxEvidenceCache)
		{
			Game.Log("Interface: Max cache exceeded, removing oldest render...", 2);
			Evidence evidence = null;
			float num = float.PositiveInfinity;
			foreach (KeyValuePair<Evidence, SceneCapture.PhotoCache> keyValuePair in this.cachedRenders)
			{
				if (keyValuePair.Value.lastUsed < num)
				{
					evidence = keyValuePair.Key;
					num = keyValuePair.Value.lastUsed;
				}
			}
			if (evidence != null)
			{
				this.cachedRenders.Remove(evidence);
				this.cachedEvidencePhotos = this.cachedRenders.Count;
			}
		}
		Vector3 pos = Vector3.zero;
		Vector3 euler = Vector3.zero;
		NewNode passNode = null;
		Transform transform = null;
		Vector3 position = Vector3.zero;
		Quaternion rotation = Quaternion.identity;
		bool active = true;
		int layer = 0;
		Transform transform2 = null;
		bool outlineActive = false;
		Human human = ev.controller as Human;
		List<Transform> list = new List<Transform>();
		bool basicMode = false;
		if (ev.preset.captureRules == EvidencePreset.CaptureRules.building)
		{
			NewBuilding newBuilding = ev.controller as NewBuilding;
			if (newBuilding != null)
			{
				newBuilding.SetDisplayBuildingModel(true, false, null);
				Transform transform3 = newBuilding.transform;
				if (newBuilding.buildingModelBase != null)
				{
					transform3 = newBuilding.buildingModelBase.transform;
				}
				if (newBuilding.preset.overrideEvidencePhotoSettings)
				{
					string text = "Interface: Render using overriden cam pos ";
					Vector3 vector = newBuilding.preset.relativeCamPhotoPos;
					string text2 = vector.ToString();
					vector = newBuilding.preset.relativeCamPhotoEuler;
					Game.Log(text + text2 + vector.ToString(), 2);
					pos = transform3.TransformPoint(newBuilding.preset.relativeCamPhotoPos);
					euler = transform3.eulerAngles + newBuilding.preset.relativeCamPhotoEuler;
				}
				else
				{
					pos = transform3.TransformPoint(ev.preset.relativeCamPhotoPos);
					euler = transform3.eulerAngles + ev.preset.relativeCamPhotoEuler;
				}
			}
		}
		else if (ev.preset.captureRules == EvidencePreset.CaptureRules.location)
		{
			NewGameLocation newGameLocation = ev.controller as NewGameLocation;
			if (newGameLocation != null)
			{
				NewAddress thisAsAddress = newGameLocation.thisAsAddress;
				if (thisAsAddress != null && thisAsAddress.building != null)
				{
					thisAsAddress.building.SetDisplayBuildingModel(true, false, null);
					Transform transform4 = thisAsAddress.building.transform;
					if (thisAsAddress.building.buildingModelBase != null)
					{
						transform4 = thisAsAddress.building.buildingModelBase.transform;
					}
					if (thisAsAddress.building.preset.overrideEvidencePhotoSettings)
					{
						string text3 = "Interface: Render using overriden cam pos ";
						Vector3 vector = thisAsAddress.building.preset.relativeCamPhotoPos;
						string text4 = vector.ToString();
						vector = thisAsAddress.building.preset.relativeCamPhotoEuler;
						Game.Log(text3 + text4 + vector.ToString(), 2);
						pos = transform4.TransformPoint(thisAsAddress.building.preset.relativeCamPhotoPos);
						euler = transform4.eulerAngles + thisAsAddress.building.preset.relativeCamPhotoEuler;
					}
					else
					{
						pos = transform4.TransformPoint(ev.preset.relativeCamPhotoPos);
						euler = transform4.eulerAngles + ev.preset.relativeCamPhotoEuler;
					}
				}
			}
		}
		else if (ev.preset.captureRules == EvidencePreset.CaptureRules.item)
		{
			if (ev.interactable != null)
			{
				passNode = ev.interactable.node;
				if (ev.interactable.controller != null)
				{
					if (!ev.interactable.preset.overrideEvidencePhotoSettings)
					{
						pos = ev.interactable.controller.transform.TransformPoint(ev.preset.relativeCamPhotoPos);
						euler = ev.interactable.controller.transform.eulerAngles + ev.preset.relativeCamPhotoEuler;
					}
					else
					{
						pos = ev.interactable.controller.transform.TransformPoint(ev.interactable.preset.relativeCamPhotoPos);
						euler = ev.interactable.controller.transform.eulerAngles + ev.interactable.preset.relativeCamPhotoEuler;
					}
				}
				else
				{
					Vector3 vector2 = Vector3.one;
					if (ev.interactable.preset.prefab != null)
					{
						vector2 = ev.interactable.preset.prefab.transform.localScale;
					}
					if (ev.interactable.furnitureParent != null)
					{
						Vector3 localScale = ev.interactable.furnitureParent.furniture.prefab.transform.localScale;
						vector2..ctor(ev.interactable.furnitureParent.scaleMultiplier.x * localScale.x, ev.interactable.furnitureParent.scaleMultiplier.y * localScale.y, ev.interactable.furnitureParent.scaleMultiplier.z * localScale.z);
					}
					Matrix4x4 matrix4x = Matrix4x4.TRS(ev.interactable.wPos, Quaternion.Euler(ev.interactable.wEuler), vector2);
					if (!ev.interactable.preset.overrideEvidencePhotoSettings)
					{
						pos = matrix4x.MultiplyPoint3x4(ev.preset.relativeCamPhotoPos);
						euler = ev.interactable.wEuler + ev.preset.relativeCamPhotoEuler;
					}
					else
					{
						pos = matrix4x.MultiplyPoint3x4(ev.interactable.preset.relativeCamPhotoPos);
						euler = ev.interactable.wEuler + ev.interactable.preset.relativeCamPhotoEuler;
					}
				}
			}
		}
		else if (ev.preset.captureRules == EvidencePreset.CaptureRules.citizen)
		{
			basicMode = true;
			this.photoRoomParent.SetActive(true);
			if (human != null)
			{
				if (human.outline != null)
				{
					outlineActive = human.outline.outlineActive;
					human.outline.SetOutlineActive(false);
				}
				pos = this.cameraTransform.position;
				euler = this.cameraTransform.eulerAngles;
				passNode = human.currentNode;
				if (human.outfitController == null)
				{
					Game.LogError("Unable to find character's outfit controller (" + human.name + ")", 2);
				}
				transform = human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head);
				if (human.outfitController != null && human.outfitController.lod != null)
				{
					human.outfitController.lod.ForceLOD(0);
				}
				if (transform != null)
				{
					Game.Log("Found head anchor (" + human.name + ") present on layer " + transform.gameObject.layer.ToString(), 2);
					layer = transform.gameObject.layer;
					active = transform.gameObject.activeSelf;
					rotation = transform.transform.rotation;
					position = transform.transform.position;
					transform2 = transform.transform.parent;
					transform.SetParent(null);
					transform.gameObject.layer = 0;
					transform.gameObject.SetActive(true);
					transform.rotation = this.itemTransform.rotation * Quaternion.Euler(ev.preset.relativeCamPhotoEuler);
					transform.position = this.itemTransform.position - ev.preset.relativeCamPhotoPos;
				}
				if (human.outfitController != null)
				{
					human.outfitController.rightPupil.localScale = new Vector3(0.02f, 0.02f, 0.02f);
					human.outfitController.leftPupil.localScale = new Vector3(0.02f, 0.02f, 0.02f);
					human.outfitController.rightPupil.localPosition = new Vector3(0.045f, 0f, 0f);
					human.outfitController.leftPupil.localPosition = new Vector3(-0.045f, 0f, 0f);
				}
				list.AddRange(Toolbox.Instance.GetTagsWithinTransform(transform.transform, "Interactable"));
				CameraController.Instance.cam.orthographic = true;
			}
		}
		if (transform != null)
		{
			list.AddRange(Toolbox.Instance.GetTagsWithinTransform(transform.transform, "SceneCapHide"));
			list.Remove(transform);
			for (int i = 0; i < list.Count; i++)
			{
				Transform transform5 = list[i];
				if (transform5.gameObject.activeSelf)
				{
					transform5.gameObject.SetActive(false);
				}
				else
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		SceneCapture.PhotoCache photoCache = new SceneCapture.PhotoCache();
		photoCache.img = this.CaptureScene(pos, euler, Toolbox.Instance.mugShotCaptureLayerMask, ev.preset.changeTimeOfDay, ev.preset.captureTimeOfDay, this.evidenceRenderTexturePrefab, this.evidenceFoV, null, passNode, ev.preset.useCaptureLight, basicMode, false, SceneCapture.PostProcessingProfile.captureNormal);
		photoCache.lastUsed = SessionData.Instance.gameTime;
		this.cachedRenders.Add(ev, photoCache);
		this.cachedEvidencePhotos++;
		if (this.photoRoomParent.activeSelf)
		{
			this.photoRoomParent.SetActive(false);
		}
		if (CameraController.Instance.cam.orthographic)
		{
			CameraController.Instance.cam.orthographic = false;
		}
		if (list != null)
		{
			foreach (Transform transform6 in list)
			{
				transform6.gameObject.SetActive(true);
			}
		}
		if (human != null)
		{
			if (human.outline != null)
			{
				human.outline.SetOutlineActive(outlineActive);
			}
			if (human.outfitController != null && human.outfitController.lod != null)
			{
				human.outfitController.lod.ForceLOD(-1);
			}
		}
		if (transform != null)
		{
			transform.gameObject.SetActive(active);
			if (transform2 != null)
			{
				transform.SetParent(transform2);
			}
			transform.gameObject.layer = layer;
			transform.position = position;
			transform.rotation = rotation;
		}
		Player.Instance.UpdateCurrentBuildingModelVisibility();
		return photoCache.img;
	}

	// Token: 0x06001A66 RID: 6758 RVA: 0x001849C8 File Offset: 0x00182BC8
	public Texture2D GetSurveillanceScene(SceneRecorder.SceneCapture scene, bool saveToCache = true)
	{
		if (scene == null)
		{
			Game.LogError("Gameplay: Scene is null for GetSurveillanceScene!", 2);
			return null;
		}
		if (scene.recorder == null)
		{
			Game.LogError("Gameplay: Recorder is null for GetSurveillanceScene!", 2);
			return null;
		}
		if (this.cachedSurveillance.ContainsKey(scene))
		{
			Game.Log(string.Concat(new string[]
			{
				"Gameplay: Capture is cached already for ",
				(scene != null) ? scene.ToString() : null,
				" ",
				SessionData.Instance.TimeStringOnDay(scene.t, true, true),
				"..."
			}), 2);
			this.cachedSurveillance[scene].lastUsed = SessionData.Instance.gameTime;
			foreach (SceneCapture.ActorScreenPosition actorScreenPosition in this.cachedSurveillance[scene].actorSP)
			{
				actorScreenPosition.human.lastUsedCCTVScreenPoint = actorScreenPosition.screenPoint;
			}
			return this.cachedSurveillance[scene].img;
		}
		if (this.cachedSurveillance.Count >= this.maxSurveillanceCache)
		{
			Game.Log("Gameplay: Max cache exceeded (" + this.maxSurveillanceCache.ToString() + "), removing oldest render...", 2);
			SceneRecorder.SceneCapture sceneCapture = null;
			float num = float.PositiveInfinity;
			foreach (KeyValuePair<SceneRecorder.SceneCapture, SceneCapture.PhotoCache> keyValuePair in this.cachedSurveillance)
			{
				if (keyValuePair.Value.lastUsed < num)
				{
					sceneCapture = keyValuePair.Key;
					num = keyValuePair.Value.lastUsed;
				}
			}
			if (sceneCapture != null)
			{
				this.cachedSurveillance.Remove(sceneCapture);
				this.cachedSurveillancePhotos = this.cachedSurveillance.Count;
			}
		}
		Game.Log("Gameplay: Grabbing capture for surveillance scene...", 2);
		Game.Log("Gameplay: Loading data from scene capture...", 2);
		List<SceneRecorder.RoomCapture> list = new List<SceneRecorder.RoomCapture>();
		foreach (SceneRecorder.RoomCapture roomCapture in scene.rCap)
		{
			NewRoom room = roomCapture.GetRoom();
			if (room != null && room.mainLightStatus != roomCapture.light)
			{
				SceneRecorder.RoomCapture roomCapture2 = new SceneRecorder.RoomCapture
				{
					id = roomCapture.id,
					light = room.mainLightStatus
				};
				list.Add(roomCapture2);
				room.SetMainLights(roomCapture.light, "Capture", null, true, false);
			}
		}
		List<SceneRecorder.DoorCapture> list2 = new List<SceneRecorder.DoorCapture>();
		foreach (SceneRecorder.DoorCapture doorCapture in scene.dCap)
		{
			NewDoor door = doorCapture.GetDoor();
			if (door != null)
			{
				list2.Add(new SceneRecorder.DoorCapture(door));
				door.SetOpen((float)doorCapture.a, null, true, 1f);
			}
		}
		foreach (SceneRecorder.InteractableCapture interactableCapture in scene.oCap)
		{
			interactableCapture.Load();
		}
		List<SceneRecorder.InteractableStateCapture> list3 = new List<SceneRecorder.InteractableStateCapture>();
		foreach (SceneRecorder.InteractableStateCapture interactableStateCapture in scene.oSCap)
		{
			Interactable interactable = interactableStateCapture.GetInteractable();
			if (interactable != null && (Player.Instance.computerInteractable == null || Player.Instance.computerInteractable != interactable) && interactable.sw0 != interactableStateCapture.sw)
			{
				list3.Add(new SceneRecorder.InteractableStateCapture(interactable));
				interactable.SetSwitchState(interactableStateCapture.sw, null, false, false, true);
			}
		}
		SceneCapture.PhotoCache photoCache = new SceneCapture.PhotoCache();
		Game.Log("Gameplay: Scene capture execution...", 2);
		photoCache.img = this.CaptureScene(scene.GetCaptureWorldPosition(), scene.GetCaptureWorldRotation(), Toolbox.Instance.sceneCaptureLayerMask, true, scene.GetDecimalClock(), this.surveillanceRenderTexturePrefab, ref scene.aCap, out photoCache.actorSP, this.surveillanceFov, null, null, false, false, false, SceneCapture.PostProcessingProfile.captureCCTV);
		photoCache.lastUsed = SessionData.Instance.gameTime;
		if (saveToCache)
		{
			this.cachedSurveillance.Add(scene, photoCache);
			this.cachedSurveillancePhotos++;
		}
		Game.Log("Gameplay: Resetting data from screen capture...", 2);
		foreach (SceneRecorder.RoomCapture roomCapture3 in list)
		{
			roomCapture3.GetRoom().SetMainLights(roomCapture3.light, "Capture Reset", null, true, false);
		}
		foreach (SceneRecorder.DoorCapture doorCapture2 in list2)
		{
			NewDoor door2 = doorCapture2.GetDoor();
			if (door2 != null)
			{
				door2.SetOpen((float)doorCapture2.a, null, true, 1f);
			}
		}
		foreach (SceneRecorder.InteractableStateCapture interactableStateCapture2 in list3)
		{
			interactableStateCapture2.Load();
		}
		foreach (SceneRecorder.InteractableCapture interactableCapture2 in scene.oCap)
		{
			interactableCapture2.Unload();
		}
		Game.Log("Gameplay: Successfully captured scene " + SessionData.Instance.TimeStringOnDay(scene.t, true, true), 2);
		Player.Instance.UpdateCulling();
		return photoCache.img;
	}

	// Token: 0x06001A67 RID: 6759 RVA: 0x00184FAC File Offset: 0x001831AC
	public Texture2D CaptureScene(Vector3 pos, Vector3 euler, int layerMask, bool changeTimeOfDay, float decimalClock, RenderTexture renderPrefab, float fov = 70f, List<Interactable> forceHide = null, NewNode passNode = null, bool useCaptureLight = true, bool basicMode = false, bool ignoreEarlyCapError = false, SceneCapture.PostProcessingProfile captureProfile = SceneCapture.PostProcessingProfile.captureNormal)
	{
		List<SceneRecorder.ActorCapture> list = new List<SceneRecorder.ActorCapture>();
		List<SceneCapture.ActorScreenPosition> list2;
		return this.CaptureScene(pos, euler, layerMask, changeTimeOfDay, decimalClock, renderPrefab, ref list, out list2, fov, forceHide, passNode, useCaptureLight, basicMode, ignoreEarlyCapError, captureProfile);
	}

	// Token: 0x06001A68 RID: 6760 RVA: 0x00184FE0 File Offset: 0x001831E0
	public Texture2D CaptureScene(Vector3 pos, Vector3 euler, int layerMask, bool changeTimeOfDay, float decimalClock, RenderTexture renderPrefab, ref List<SceneRecorder.ActorCapture> humanRef, out List<SceneCapture.ActorScreenPosition> actorScreenPointCapture, float fov = 70f, List<Interactable> forceHide = null, NewNode passNode = null, bool useCaptureLight = true, bool basicMode = false, bool ignoreEarlyCapError = false, SceneCapture.PostProcessingProfile captureProfile = SceneCapture.PostProcessingProfile.captureNormal)
	{
		if (!SessionData.Instance.startedGame && !ignoreEarlyCapError)
		{
			Game.LogError("Requesting scene capture before game has started: This could result in some blank captures...", 2);
		}
		string[] array = new string[6];
		array[0] = "Gameplay: Capturing scene with profile ";
		array[1] = captureProfile.ToString();
		array[2] = " Pos: ";
		int num = 3;
		Vector3 vector = pos;
		array[num] = vector.ToString();
		array[4] = " euler: ";
		int num2 = 5;
		vector = euler;
		array[num2] = vector.ToString();
		Game.Log(string.Concat(array), 2);
		RenderTexture renderTexture = Object.Instantiate<RenderTexture>(renderPrefab);
		actorScreenPointCapture = new List<SceneCapture.ActorScreenPosition>();
		int cullingMask = CameraController.Instance.cam.cullingMask;
		CameraController.Instance.cam.cullingMask = layerMask;
		float fieldOfView = CameraController.Instance.cam.fieldOfView;
		CameraController.Instance.cam.fieldOfView = fov;
		Vector3 position = CameraController.Instance.cam.transform.position;
		Vector3 eulerAngles = CameraController.Instance.cam.transform.eulerAngles;
		if (captureProfile == SceneCapture.PostProcessingProfile.captureNormal)
		{
			CityControls.Instance.captureSceneNormal.volume.priority = 999f;
			CityControls.Instance.captureSceneNormal.volume.weight = 1f;
			CityControls.Instance.captureSceneNormal.objectRef.SetActive(true);
		}
		else if (captureProfile == SceneCapture.PostProcessingProfile.captureCCTV)
		{
			CityControls.Instance.captureSceneCCTV.volume.priority = 999f;
			CityControls.Instance.captureSceneCCTV.volume.weight = 1f;
			CityControls.Instance.captureSceneCCTV.objectRef.SetActive(true);
		}
		bool flashlight = FirstPersonItemController.Instance.flashlight;
		FirstPersonItemController.Instance.flashLightObject.SetActive(false);
		if (useCaptureLight)
		{
			FirstPersonItemController.Instance.captureLightObject.SetActive(true);
		}
		CameraController.Instance.cam.transform.position = pos;
		CameraController.Instance.cam.transform.eulerAngles = euler;
		float decimalClock2 = SessionData.Instance.decimalClock;
		if (!basicMode)
		{
			if (passNode != null)
			{
				string[] array2 = new string[6];
				array2[0] = "Gameplay: Updating culling for ";
				array2[1] = passNode.room.name;
				array2[2] = " (capture) ";
				int num3 = 3;
				vector = passNode.position;
				array2[num3] = vector.ToString();
				array2[4] = ", ";
				int num4 = 5;
				vector = pos;
				array2[num4] = vector.ToString();
				Game.Log(string.Concat(array2), 2);
				GeometryCullingController.Instance.UpdateCullingForRoom(passNode.room, false, false, null, true);
			}
			else
			{
				passNode = Toolbox.Instance.FindClosestValidNodeToWorldPosition(pos, false, true, false, default(Vector3Int), false, 0, false, 200);
				if (passNode != null && passNode.room != null)
				{
					string[] array3 = new string[6];
					array3[0] = "Gameplay: Updating culling for ";
					array3[1] = passNode.room.name;
					array3[2] = " (capture)";
					int num5 = 3;
					vector = passNode.position;
					array3[num5] = vector.ToString();
					array3[4] = ", ";
					int num6 = 5;
					vector = pos;
					array3[num6] = vector.ToString();
					Game.Log(string.Concat(array3), 2);
					GeometryCullingController.Instance.UpdateCullingForRoom(passNode.room, false, false, null, true);
				}
			}
			SessionData.Instance.ExecuteSyncPhysics(SessionData.PhysicsSyncType.both);
			ObjectPoolingController.Instance.ExecuteUpdateObjectRanges(true);
			if (changeTimeOfDay)
			{
				SessionData.Instance.SetSceneVisuals(decimalClock);
			}
			if (forceHide != null)
			{
				foreach (Interactable interactable in forceHide)
				{
					if (interactable.controller != null)
					{
						interactable.controller.gameObject.SetActive(false);
					}
				}
			}
			if (humanRef != null)
			{
				foreach (SceneRecorder.ActorCapture actorCapture in humanRef)
				{
					Game.Log("Gameplay: Loading actor capture " + actorCapture.id.ToString(), 2);
					actorCapture.Load();
					if (!(actorCapture.poser == null))
					{
						Vector3 vector2 = CameraController.Instance.cam.WorldToScreenPoint(actorCapture.poser.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.Head).transform.position);
						Vector2 vector3;
						vector3..ctor(vector2.x / (float)CameraController.Instance.cam.pixelWidth, vector2.y / (float)CameraController.Instance.cam.pixelHeight);
						Human human = actorCapture.GetHuman();
						actorScreenPointCapture.Add(new SceneCapture.ActorScreenPosition
						{
							human = human,
							screenPoint = vector3
						});
						human.lastUsedCCTVScreenPoint = vector3;
					}
				}
			}
		}
		foreach (CustomPassVolume customPassVolume in SessionData.Instance.customPasses)
		{
			if (!(customPassVolume == null))
			{
				foreach (CustomPass customPass in customPassVolume.customPasses)
				{
					customPass.enabled = false;
				}
			}
		}
		MapController.Instance.directionalArrowContainer.gameObject.SetActive(false);
		RenderTexture.active = renderTexture;
		CameraController.Instance.cam.targetTexture = renderTexture;
		CameraController.Instance.cam.Render();
		CameraController.Instance.cam.transform.position = position;
		CameraController.Instance.cam.transform.eulerAngles = eulerAngles;
		CameraController.Instance.cam.targetTexture = null;
		CameraController.Instance.cam.fieldOfView = fieldOfView;
		CameraController.Instance.cam.cullingMask = cullingMask;
		if (forceHide != null)
		{
			foreach (Interactable interactable2 in forceHide)
			{
				if (interactable2.controller != null)
				{
					interactable2.controller.gameObject.SetActive(true);
				}
			}
		}
		if (captureProfile == SceneCapture.PostProcessingProfile.captureNormal)
		{
			CityControls.Instance.captureSceneNormal.objectRef.SetActive(false);
		}
		else if (captureProfile == SceneCapture.PostProcessingProfile.captureCCTV)
		{
			CityControls.Instance.captureSceneCCTV.objectRef.SetActive(false);
		}
		if (!basicMode)
		{
			if (changeTimeOfDay)
			{
				SessionData.Instance.SetSceneVisuals(decimalClock2);
			}
			Player.Instance.OnRoomChange();
		}
		FirstPersonItemController.Instance.flashLightObject.SetActive(flashlight);
		FirstPersonItemController.Instance.captureLightObject.SetActive(false);
		if (humanRef != null)
		{
			foreach (SceneRecorder.ActorCapture actorCapture2 in humanRef)
			{
				actorCapture2.Unload();
			}
		}
		foreach (CustomPassVolume customPassVolume2 in SessionData.Instance.customPasses)
		{
			if (!(customPassVolume2 == null))
			{
				foreach (CustomPass customPass2 in customPassVolume2.customPasses)
				{
					customPass2.enabled = true;
				}
			}
		}
		MapController.Instance.directionalArrowContainer.gameObject.SetActive(MapController.Instance.displayDirectionArrow);
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, 3, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
		texture2D.name = "New Capture";
		texture2D.Apply();
		RenderTexture.active = null;
		string text = "Gameplay: Captured scene for ";
		vector = pos;
		string text2 = vector.ToString();
		string text3 = " and euler ";
		vector = euler;
		Game.Log(text + text2 + text3 + vector.ToString(), 2);
		renderTexture.Release();
		Object.Destroy(renderTexture);
		CameraController.Instance.cam.Render();
		return texture2D;
	}

	// Token: 0x06001A69 RID: 6761 RVA: 0x0018583C File Offset: 0x00183A3C
	[Button(null, 0)]
	public void ClearRenderCache()
	{
		this.cachedRenders.Clear();
		this.cachedEvidencePhotos = 0;
		this.cachedSurveillance.Clear();
		this.cachedSurveillancePhotos = 0;
	}

	// Token: 0x04002304 RID: 8964
	[Header("Capture")]
	public RenderTexture evidenceRenderTexturePrefab;

	// Token: 0x04002305 RID: 8965
	public RenderTexture surveillanceRenderTexturePrefab;

	// Token: 0x04002306 RID: 8966
	public float evidenceFoV = 70f;

	// Token: 0x04002307 RID: 8967
	public float surveillanceFov = 90f;

	// Token: 0x04002308 RID: 8968
	[NonSerialized]
	public SceneRecorder.SceneCapture currrentlyViewing;

	// Token: 0x04002309 RID: 8969
	[Tooltip("The max number of cached evidence photos")]
	[Header("Cache")]
	public int maxEvidenceCache = 32;

	// Token: 0x0400230A RID: 8970
	[ReadOnly]
	public int cachedEvidencePhotos;

	// Token: 0x0400230B RID: 8971
	public Dictionary<Evidence, SceneCapture.PhotoCache> cachedRenders = new Dictionary<Evidence, SceneCapture.PhotoCache>();

	// Token: 0x0400230C RID: 8972
	[Space(7f)]
	public int maxSurveillanceCache = 16;

	// Token: 0x0400230D RID: 8973
	[ReadOnly]
	public int cachedSurveillancePhotos;

	// Token: 0x0400230E RID: 8974
	public Dictionary<SceneRecorder.SceneCapture, SceneCapture.PhotoCache> cachedSurveillance = new Dictionary<SceneRecorder.SceneCapture, SceneCapture.PhotoCache>();

	// Token: 0x0400230F RID: 8975
	[Header("Photo Room")]
	public GameObject photoRoomParent;

	// Token: 0x04002310 RID: 8976
	public Transform cameraTransform;

	// Token: 0x04002311 RID: 8977
	public Transform itemTransform;

	// Token: 0x04002312 RID: 8978
	private static SceneCapture _instance;

	// Token: 0x020004C5 RID: 1221
	[Serializable]
	public class PhotoCache
	{
		// Token: 0x04002313 RID: 8979
		public Texture2D img;

		// Token: 0x04002314 RID: 8980
		public float lastUsed;

		// Token: 0x04002315 RID: 8981
		public List<SceneCapture.ActorScreenPosition> actorSP = new List<SceneCapture.ActorScreenPosition>();
	}

	// Token: 0x020004C6 RID: 1222
	public struct ActorScreenPosition
	{
		// Token: 0x04002316 RID: 8982
		public Human human;

		// Token: 0x04002317 RID: 8983
		public Vector2 screenPoint;
	}

	// Token: 0x020004C7 RID: 1223
	public enum PostProcessingProfile
	{
		// Token: 0x04002319 RID: 8985
		captureNormal,
		// Token: 0x0400231A RID: 8986
		captureCCTV
	}
}
