using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020000AD RID: 173
public class CitizenBehaviour : MonoBehaviour
{
	// Token: 0x14000007 RID: 7
	// (add) Token: 0x06000547 RID: 1351 RVA: 0x000516C8 File Offset: 0x0004F8C8
	// (remove) Token: 0x06000548 RID: 1352 RVA: 0x00051700 File Offset: 0x0004F900
	public event CitizenBehaviour.GameWorldLoop OnGameWorldLoop;

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000549 RID: 1353 RVA: 0x00051735 File Offset: 0x0004F935
	public static CitizenBehaviour Instance
	{
		get
		{
			return CitizenBehaviour._instance;
		}
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x0005173C File Offset: 0x0004F93C
	private void Awake()
	{
		if (CitizenBehaviour._instance != null && CitizenBehaviour._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CitizenBehaviour._instance = this;
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x0005176A File Offset: 0x0004F96A
	public void StartGame()
	{
		this.GameSpeedChange();
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x00051774 File Offset: 0x0004F974
	public void GameSpeedChange()
	{
		if (SessionData.Instance.startedGame)
		{
			base.CancelInvoke();
			if (SessionData.Instance.play)
			{
				this.timeOnLastGameWorldUpdate = SessionData.Instance.gameTime;
				if (!SessionData.Instance.isFloorEdit)
				{
					base.InvokeRepeating("RoutineCheck", 0f, GameplayControls.Instance.routineUpdateFrequency / SessionData.Instance.currentTimeMultiplier);
					base.InvokeRepeating("LightLevelLoop", 0f, GameplayControls.Instance.stealthModeLoopUpdateFrequency / SessionData.Instance.currentTimeMultiplier);
				}
				base.InvokeRepeating("GameWorldCheck", 0f, GameplayControls.Instance.gameWorldUpdateFrequency / SessionData.Instance.currentTimeMultiplier);
			}
		}
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00051830 File Offset: 0x0004FA30
	public void RoutineCheck()
	{
		for (int i = 0; i < this.veryHighTickRate.Count; i++)
		{
			if (!this.veryHighTickRate[i].dueUpdate)
			{
				this.updateList.Insert(0, this.veryHighTickRate[i]);
				this.veryHighTickRate[i].dueUpdate = true;
			}
		}
		if (this.tickCounter % 5 == 0)
		{
			for (int j = 0; j < this.highTickRate.Count; j++)
			{
				if (!this.highTickRate[j].dueUpdate)
				{
					this.updateList.Add(this.highTickRate[j]);
					this.highTickRate[j].dueUpdate = true;
				}
			}
		}
		if (this.tickCounter % 10 == 0)
		{
			for (int k = 0; k < this.mediumTickRate.Count; k++)
			{
				if (!this.mediumTickRate[k].dueUpdate)
				{
					this.updateList.Add(this.mediumTickRate[k]);
					this.mediumTickRate[k].dueUpdate = true;
				}
			}
		}
		if (this.tickCounter % 15 == 0)
		{
			for (int l = 0; l < this.lowTickRate.Count; l++)
			{
				if (!this.lowTickRate[l].dueUpdate)
				{
					this.updateList.Add(this.lowTickRate[l]);
					this.lowTickRate[l].dueUpdate = true;
				}
			}
		}
		if (this.tickCounter % 20 == 0)
		{
			for (int m = 0; m < this.veryLowTickRate.Count; m++)
			{
				if (!this.veryLowTickRate[m].dueUpdate)
				{
					this.updateList.Add(this.veryLowTickRate[m]);
					this.veryLowTickRate[m].dueUpdate = true;
				}
			}
		}
		this.tickCounter++;
		if (this.tickCounter > 20)
		{
			this.tickCounter = 1;
		}
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00051A29 File Offset: 0x0004FC29
	public void AddToCitizenRenderQueue(Human human)
	{
		if (!this.citizensRenderQueue.Contains(human))
		{
			this.citizensRenderQueue.Add(human);
		}
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x00051A46 File Offset: 0x0004FC46
	public void RemoveFromCitizenRenderQueue(Human human)
	{
		this.citizensRenderQueue.Remove(human);
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x00051A58 File Offset: 0x0004FC58
	private void Update()
	{
		if (SessionData.Instance.play)
		{
			this.executionsThisFrame = 0;
			while (this.updateList.Count > 0 && this.executionsThisFrame < this.AITicksPerFrame)
			{
				if (this.updateList[0] != null)
				{
					this.updateList[0].AITick(false, false);
				}
				this.updateList.RemoveAt(0);
				this.executionsThisFrame++;
			}
			this.aiTickBacklog = this.updateList.Count;
			if (this.buildingEmissionTexturesToUpdate.Count > 0)
			{
				this.buildingEmissionTexturesToUpdate[0].emissionTextureInstanced.Apply();
				this.buildingEmissionTexturesToUpdate.RemoveAt(0);
			}
			for (int i = 0; i < GameplayController.Instance.burningBarrels.Count; i++)
			{
				Interactable interactable = GameplayController.Instance.burningBarrels[i];
				if (interactable != null && interactable.sw0 && interactable.controller != null)
				{
					foreach (AudioController.LoopingSoundInfo loopingSoundInfo in interactable.loopingAudio)
					{
						if (loopingSoundInfo != null)
						{
							loopingSoundInfo.audioEvent.setParameterByName("Wind", SessionData.Instance.currentWind, false);
						}
					}
				}
			}
			if (GameplayController.Instance.switchRessetingObjects.Count > 0)
			{
				this.toRemove.Clear();
				foreach (KeyValuePair<Interactable, float> keyValuePair in GameplayController.Instance.switchRessetingObjects)
				{
					if (SessionData.Instance.gameTime >= keyValuePair.Value + keyValuePair.Key.preset.resetTimer)
					{
						keyValuePair.Key.ResetToDefaultSwitchState();
						this.toRemove.Add(keyValuePair.Key);
					}
				}
				if (this.toRemove.Count > 0)
				{
					foreach (Interactable interactable2 in this.toRemove)
					{
						if (interactable2 != null)
						{
							GameplayController.Instance.switchRessetingObjects.Remove(interactable2);
						}
					}
				}
			}
			for (int j = 0; j < GameplayController.Instance.activeGrenades.Count; j++)
			{
				Interactable interactable3 = GameplayController.Instance.activeGrenades[j];
				if (interactable3.val > 0f)
				{
					interactable3.SetValue(interactable3.val - Time.deltaTime);
					if (interactable3.recentCallCheck <= 0f)
					{
						interactable3.UpdateWorldPositionAndNode(true);
						AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.grenadeBeep, null, interactable3.node, interactable3.wPos, interactable3, null, 1f, null, false, null, false);
						interactable3.recentCallCheck = interactable3.val * 0.2f;
					}
					else
					{
						interactable3.recentCallCheck -= Time.deltaTime;
					}
				}
				else
				{
					Toolbox.Instance.ExplodeGrenade(interactable3);
					j--;
				}
			}
			for (int k = 0; k < GameplayController.Instance.activeGadgets.Count; k++)
			{
				Interactable interactable4 = GameplayController.Instance.activeGadgets[k];
				if (interactable4 != null && interactable4.preset.specialCaseFlag == InteractablePreset.SpecialCase.codebreaker)
				{
					Interactable interactable5 = interactable4.objectRef as Interactable;
					List<int> list = new List<int>();
					ComputerLogin computerLogin = null;
					string text = string.Empty;
					float num = 0f;
					if (interactable5.preset.isComputer)
					{
						num = -1f;
						computerLogin = interactable5.controller.computer.GetComponentInChildren<ComputerLogin>();
						if (computerLogin != null && computerLogin.loginSelection.selected != null)
						{
							list = computerLogin.loginSelection.selected.option.human.passcode.GetDigits();
						}
					}
					else
					{
						List<string> list2;
						list = interactable5.GetPasswordFromSource(out list2);
					}
					int num2 = 0;
					if (list.Count >= 4)
					{
						text = list[0].ToString() + list[1].ToString() + list[2].ToString() + list[3].ToString();
						int.TryParse(text, ref num2);
					}
					for (int l = 0; l < 8; l++)
					{
						int num3 = Mathf.RoundToInt(interactable4.cs);
						if (interactable4.cs < 10000f)
						{
							if (num3 >= num2 && list.Count >= 4)
							{
								if (interactable5.preset.isComputer)
								{
									AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.codebreakerSuccess, null, interactable4.node, interactable4.GetWorldPosition(true), interactable4, null, 1f, null, false, null, false);
									GameplayController.Instance.AddPasscode(computerLogin.loginSelection.selected.option.human.passcode, true);
									computerLogin.OnInputCode(list, 0f);
								}
								else
								{
									AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.codebreakerSuccess, null, interactable4.node, interactable4.GetWorldPosition(true), interactable4, null, 1f, null, false, null, false);
									interactable5.SetLockedState(false, Player.Instance, true, false);
									GameplayController.Instance.AddPasscode(interactable5.GetPasswordSource(), true);
								}
								interactable4.SetSwitchState(false, null, true, true, false);
								interactable4.UpdateCurrentActions();
								InteractionController.Instance.UpdateInteractionText();
								interactable4.cs = 10000f;
								if (interactable4.controller != null)
								{
									interactable4.controller.transform.GetComponentInChildren<ActiveCodebreakerController>().OnCrack(text);
								}
							}
							else
							{
								interactable4.cs += Mathf.Min(Time.deltaTime * 120f, 1f);
							}
						}
						else
						{
							interactable4.cs += Time.deltaTime;
							if (interactable4.cs > 10008f)
							{
								if (interactable4.controller != null)
								{
									InteractableCreator.Instance.CreateWorldInteractable(InteriorControls.Instance.codebreakerUsed, Player.Instance, null, null, interactable4.wPos, interactable4.wEuler, null, null, "").ForcePhysicsActive(false, true, interactable4.controller.transform.up * 0.5f + interactable4.controller.transform.right * num, 2, false);
								}
								interactable4.Delete();
								break;
							}
						}
					}
				}
			}
			if (this.lightUpdateQueue.Count <= 0)
			{
				for (int m = 0; m < CityData.Instance.dynamicShadowSystemLights.Count; m++)
				{
					LightController lightController = CityData.Instance.dynamicShadowSystemLights[m];
					if (lightController == null)
					{
						CityData.Instance.dynamicShadowSystemLights.RemoveAt(m);
						m--;
					}
					else if (lightController.hdrpLightData != null && lightController.isOn && lightController.useShadows && lightController.lightComponent.enabled && Vector3.Distance(CameraController.Instance.cam.transform.position, lightController.lightComponent.transform.position) <= lightController.preset.shadowFadeDistance * Game.Instance.shadowFadeDistanceMultiplier)
					{
						this.lightUpdateQueue.Add(lightController);
					}
				}
			}
			else
			{
				int num4 = 0;
				while (num4 < Game.Instance.maxUpdateDynamicShadowsPerFrame && this.lightUpdateQueue.Count > 0)
				{
					LightController lightController2 = this.lightUpdateQueue[0];
					if (lightController2.hdrpLightData != null && lightController2.useShadows && lightController2.isOn && lightController2.lightComponent.enabled)
					{
						lightController2.hdrpLightData.RequestShadowMapRendering();
					}
					this.lightUpdateQueue.RemoveAt(0);
					num4++;
				}
			}
		}
		if (this.citizensRenderQueue.Count > 0)
		{
			for (int n = 0; n < Mathf.Min(this.loadCitizensPerFrame, this.citizensRenderQueue.Count); n++)
			{
				Human human = Enumerable.FirstOrDefault<Human>(this.citizensRenderQueue);
				if (human != null)
				{
					human.outfitController.LoadCurrentOutfit(false, false);
					if (human.updateMeshList)
					{
						human.UpdateMeshList();
					}
					this.citizensRenderQueue.Remove(human);
					if (this.citizensRenderQueue.Count <= 0)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x00052320 File Offset: 0x00050520
	private void LateUpdate()
	{
		for (int i = 0; i < CityData.Instance.citizenDirectory.Count; i++)
		{
			Citizen citizen = CityData.Instance.citizenDirectory[i];
			if (!(citizen.ai == null) && citizen.ai.enabled)
			{
				if (citizen.ai.tickRate == NewAIController.AITickRate.veryHigh || citizen.visible)
				{
					citizen.ai.FrequentUpdate();
				}
				else if (citizen.ai.tickRate == NewAIController.AITickRate.high)
				{
					if (this.tickCounter % 2 == 0)
					{
						citizen.ai.FrequentUpdate();
					}
				}
				else if (citizen.ai.tickRate == NewAIController.AITickRate.medium)
				{
					if (this.tickCounter % 4 == 0)
					{
						citizen.ai.FrequentUpdate();
					}
				}
				else if (citizen.ai.tickRate == NewAIController.AITickRate.low)
				{
					if (this.tickCounter % 6 == 0)
					{
						citizen.ai.FrequentUpdate();
					}
				}
				else if (citizen.ai.tickRate == NewAIController.AITickRate.veryLow && this.tickCounter % 8 == 0)
				{
					citizen.ai.FrequentUpdate();
				}
				this.frequentTickCounter++;
				if (this.frequentTickCounter > 8)
				{
					this.frequentTickCounter = 1;
				}
			}
		}
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00052454 File Offset: 0x00050654
	private void GameWorldCheck()
	{
		if (this.OnGameWorldLoop != null)
		{
			this.OnGameWorldLoop();
		}
		GameplayController.Instance.UpdateConversationDelays();
		int num = 0;
		for (int i = 0; i < this.sceneRecorders.Count; i++)
		{
			if (num >= GameplayControls.Instance.maxCapturesPerFrame)
			{
				this.sceneRecorders.Sort((SceneRecorder p1, SceneRecorder p2) => p1.lastCaptureAt.CompareTo(p2.lastCaptureAt));
				break;
			}
			if (this.sceneRecorders[i] != null && SessionData.Instance.gameTime >= this.sceneRecorders[i].lastCaptureAt + GameplayControls.Instance.captureInterval)
			{
				this.sceneRecorders[i].ExecuteCapture(true, false, true);
				num++;
			}
		}
		for (int j = 0; j < CityData.Instance.companyDirectory.Count; j++)
		{
			if (CityData.Instance.companyDirectory[j] != null)
			{
				CityData.Instance.companyDirectory[j].OpenCloseCheck();
			}
		}
		float num2 = SessionData.Instance.gameTime - this.timeOnLastGameWorldUpdate;
		for (int k = 0; k < GameplayController.Instance.turnedOffSecurity.Count; k++)
		{
			Interactable interactable = GameplayController.Instance.turnedOffSecurity[k];
			if (interactable != null)
			{
				float securityStrength = interactable.GetSecurityStrength();
				interactable.SetValue(interactable.val + num2 / GameplayControls.Instance.securityResetTime * securityStrength);
				if (interactable.val >= securityStrength)
				{
					interactable.val = securityStrength;
					if (interactable.node != null && (interactable.node.gameLocation.thisAsAddress == null || (interactable.node.gameLocation.thisAsAddress.GetBreakerSecurity() != null && interactable.node.gameLocation.thisAsAddress.GetBreakerSecurity().sw0)))
					{
						interactable.SetSwitchState(true, null, true, false, false);
					}
				}
			}
		}
		for (int l = 0; l < GameplayController.Instance.activeAlarmsBuildings.Count; l++)
		{
			NewBuilding newBuilding = GameplayController.Instance.activeAlarmsBuildings[l];
			if (!(newBuilding == null))
			{
				foreach (Interactable interactable2 in newBuilding.otherSecurity)
				{
					if (interactable2 != null && interactable2.preset.specialCaseFlag == InteractablePreset.SpecialCase.gasReleaseSystem && SessionData.Instance.gameTime > interactable2.node.room.lastRoomGassed + 0.75f && interactable2.sw0)
					{
						Dictionary<NewRoom, float> dictionary = new Dictionary<NewRoom, float>();
						HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
						dictionary.Add(interactable2.node.room, 1f);
						int num3 = 32;
						while (dictionary.Count > 0 && num3 > 0)
						{
							NewRoom newRoom = null;
							float num4 = -1f;
							foreach (KeyValuePair<NewRoom, float> keyValuePair in dictionary)
							{
								if (keyValuePair.Value > num4)
								{
									newRoom = keyValuePair.Key;
									num4 = keyValuePair.Value;
								}
							}
							if (newRoom == null)
							{
								break;
							}
							newRoom.AddGas(num2 / GameplayControls.Instance.gasFillTime * num4);
							foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
							{
								if (nodeAccess.walkingAccess)
								{
									NewRoom otherRoom = nodeAccess.GetOtherRoom(newRoom);
									if (!hashSet.Contains(otherRoom) && !otherRoom.IsOutside())
									{
										if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.adjacent || nodeAccess.accessType == NewNode.NodeAccess.AccessType.bannister || nodeAccess.accessType == NewNode.NodeAccess.AccessType.verticalSpace || nodeAccess.accessType == NewNode.NodeAccess.AccessType.openDoorway)
										{
											float num5 = num4 - 0.4f;
											if (!dictionary.ContainsKey(otherRoom))
											{
												dictionary.Add(otherRoom, num5);
											}
											else
											{
												Dictionary<NewRoom, float> dictionary2 = dictionary;
												NewRoom newRoom2 = otherRoom;
												dictionary2[newRoom2] += num5;
											}
										}
										else if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.door)
										{
											float num6 = num4 - 0.6f;
											if (!dictionary.ContainsKey(otherRoom))
											{
												dictionary.Add(otherRoom, num6);
											}
											else
											{
												Dictionary<NewRoom, float> dictionary2 = dictionary;
												NewRoom newRoom2 = otherRoom;
												dictionary2[newRoom2] += num6;
											}
										}
									}
								}
							}
							hashSet.Add(newRoom);
							dictionary.Remove(newRoom);
							num3--;
						}
					}
				}
				bool flag = true;
				foreach (Interactable interactable3 in newBuilding.securityCameras)
				{
					if (interactable3 != null && interactable3.controller != null && interactable3.controller.securitySystem != null && interactable3.controller.securitySystem.seenIllegalThisCheck.Count > 0)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					newBuilding.alarmTimer -= num2;
					if (newBuilding.alarmTimer <= 0f)
					{
						newBuilding.SetAlarm(false, null, null);
					}
				}
			}
		}
		for (int m = 0; m < GameplayController.Instance.activeAlarmsLocations.Count; m++)
		{
			NewAddress newAddress = GameplayController.Instance.activeAlarmsLocations[m];
			if (!(newAddress == null))
			{
				foreach (Interactable interactable4 in newAddress.otherSecurity)
				{
					if (interactable4 != null && interactable4.preset.specialCaseFlag == InteractablePreset.SpecialCase.gasReleaseSystem)
					{
						Game.Log("Gas release system found", 2);
						if (interactable4.sw0)
						{
							Dictionary<NewRoom, float> dictionary3 = new Dictionary<NewRoom, float>();
							HashSet<NewRoom> hashSet2 = new HashSet<NewRoom>();
							dictionary3.Add(interactable4.node.room, 1f);
							int num7 = 32;
							while (dictionary3.Count > 0 && num7 > 0)
							{
								NewRoom newRoom3 = null;
								float num8 = -1f;
								foreach (KeyValuePair<NewRoom, float> keyValuePair2 in dictionary3)
								{
									if (keyValuePair2.Value > num8)
									{
										newRoom3 = keyValuePair2.Key;
										num8 = keyValuePair2.Value;
									}
								}
								if (newRoom3 == null)
								{
									break;
								}
								newRoom3.AddGas(num2 / GameplayControls.Instance.gasFillTime * num8);
								foreach (NewNode.NodeAccess nodeAccess2 in newRoom3.entrances)
								{
									if (nodeAccess2.walkingAccess)
									{
										NewRoom otherRoom2 = nodeAccess2.GetOtherRoom(newRoom3);
										if (!hashSet2.Contains(otherRoom2) && !otherRoom2.IsOutside())
										{
											if (nodeAccess2.accessType == NewNode.NodeAccess.AccessType.adjacent || nodeAccess2.accessType == NewNode.NodeAccess.AccessType.bannister || nodeAccess2.accessType == NewNode.NodeAccess.AccessType.verticalSpace || nodeAccess2.accessType == NewNode.NodeAccess.AccessType.openDoorway)
											{
												float num9 = num8 - 0.4f;
												if (!dictionary3.ContainsKey(otherRoom2))
												{
													dictionary3.Add(otherRoom2, num9);
												}
												else
												{
													Dictionary<NewRoom, float> dictionary2 = dictionary3;
													NewRoom newRoom2 = otherRoom2;
													dictionary2[newRoom2] += num9;
												}
											}
											else if (nodeAccess2.accessType == NewNode.NodeAccess.AccessType.door)
											{
												float num10 = num8 - 0.6f;
												if (!dictionary3.ContainsKey(otherRoom2))
												{
													dictionary3.Add(otherRoom2, num10);
												}
												else
												{
													Dictionary<NewRoom, float> dictionary2 = dictionary3;
													NewRoom newRoom2 = otherRoom2;
													dictionary2[newRoom2] += num10;
												}
											}
										}
									}
								}
								hashSet2.Add(newRoom3);
								dictionary3.Remove(newRoom3);
								num7--;
							}
						}
					}
				}
				bool flag2 = true;
				foreach (Interactable interactable5 in newAddress.securityCameras)
				{
					if (interactable5 != null && interactable5.controller != null && interactable5.controller.securitySystem != null && interactable5.sw0 && interactable5.controller.securitySystem.seenIllegalThisCheck.Count > 0)
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					newAddress.alarmTimer -= num2;
					if (newAddress.alarmTimer <= 0f)
					{
						newAddress.SetAlarm(false, null);
					}
				}
			}
		}
		for (int n = 0; n < GameplayController.Instance.gasRooms.Count; n++)
		{
			NewRoom newRoom4 = GameplayController.Instance.gasRooms[n];
			if (!(newRoom4 == null))
			{
				newRoom4.AddGas(num2 / -GameplayControls.Instance.gasEmptyTime);
			}
		}
		for (int num11 = 0; num11 < GameplayController.Instance.alteredSecurityTargetsLocations.Count; num11++)
		{
			NewAddress newAddress2 = GameplayController.Instance.alteredSecurityTargetsLocations[num11];
			if (!(newAddress2 == null) && SessionData.Instance.gameTime >= newAddress2.targetModeSetAt + GameplayControls.Instance.securityResetTime)
			{
				newAddress2.SetTargetMode(NewBuilding.AlarmTargetMode.illegalActivities, false);
				GameplayController.Instance.alteredSecurityTargetsLocations.RemoveAt(num11);
				num11--;
			}
		}
		for (int num12 = 0; num12 < GameplayController.Instance.alteredSecurityTargetsBuildings.Count; num12++)
		{
			NewBuilding newBuilding2 = GameplayController.Instance.alteredSecurityTargetsBuildings[num12];
			if (!(newBuilding2 == null) && SessionData.Instance.gameTime >= newBuilding2.targetModeSetAt + GameplayControls.Instance.securityResetTime)
			{
				newBuilding2.SetTargetMode(NewBuilding.AlarmTargetMode.illegalActivities, false);
				GameplayController.Instance.alteredSecurityTargetsBuildings.RemoveAt(num12);
				num12--;
			}
		}
		foreach (KeyValuePair<Interactable, float> keyValuePair3 in GameplayController.Instance.proxyTrackers)
		{
			List<NewRoom> list = new List<NewRoom>();
			list.Add(keyValuePair3.Key.node.room);
			list.AddRange(keyValuePair3.Key.node.room.adjacentRooms);
			bool flag3 = false;
			foreach (NewRoom newRoom5 in list)
			{
				foreach (Actor actor in newRoom5.currentOccupants)
				{
					Human human = actor as Human;
					List<DataRaycastController.NodeRaycastHit> list2;
					if (human != null && Vector3.Distance(human.transform.position, keyValuePair3.Key.wPos) < keyValuePair3.Value && DataRaycastController.Instance.NodeRaycast(keyValuePair3.Key.node, human.currentNode, out list2, null, false))
					{
						flag3 = true;
						break;
					}
				}
			}
			if (flag3)
			{
				if (!keyValuePair3.Key.sw2)
				{
					keyValuePair3.Key.SetCustomState2(true, null, true, true, false);
				}
			}
			else if (keyValuePair3.Key.sw2)
			{
				keyValuePair3.Key.SetCustomState2(false, null, true, true, false);
			}
		}
		List<Interactable> list3 = new List<Interactable>();
		using (HashSet<Interactable>.Enumerator enumerator7 = GameplayController.Instance.objectsWithDynamicPrints.GetEnumerator())
		{
			while (enumerator7.MoveNext())
			{
				Interactable interactable6 = enumerator7.Current;
				if (interactable6 != null && !(interactable6.controller != null))
				{
					for (int num13 = 0; num13 < interactable6.df.Count; num13++)
					{
						Interactable.DynamicFingerprint dynamicFingerprint = interactable6.df[num13];
						if (dynamicFingerprint.life == Interactable.PrintLife.timed && SessionData.Instance.gameTime > dynamicFingerprint.created + GameplayControls.Instance.fingerprintLife)
						{
							interactable6.RemoveDynamicPrint(dynamicFingerprint);
						}
					}
					if (interactable6.df.Count <= 0)
					{
						list3.Add(interactable6);
					}
				}
			}
			goto IL_C3E;
		}
		IL_C20:
		GameplayController.Instance.objectsWithDynamicPrints.Remove(list3[0]);
		list3.RemoveAt(0);
		IL_C3E:
		if (list3.Count <= 0)
		{
			for (int num14 = 0; num14 < this.smokestacks.Count; num14++)
			{
				CitizenBehaviour.Smokestack smokestack = this.smokestacks[num14];
				if (smokestack == null)
				{
					this.smokestacks.RemoveAt(num14);
				}
				else
				{
					smokestack.timer -= num2;
					if (smokestack.timer <= 0f && smokestack.building != null && smokestack.building.buildingModelBase != null)
					{
						if (smokestack.building.displayBuildingModel || Player.Instance.currentBuilding == smokestack.building)
						{
							Object.Instantiate<GameObject>(smokestack.building.preset.spritePrefab, smokestack.building.buildingModelBase.transform).transform.localPosition = smokestack.building.preset.spawnOffset;
						}
						smokestack.timer = Toolbox.Instance.Rand(smokestack.building.preset.spawnInterval.x, smokestack.building.preset.spawnInterval.y, false);
						if (AudioDebugging.Instance.overrideSmokeStackEmissionFrequency && Game.Instance.devMode)
						{
							smokestack.timer = AudioDebugging.Instance.chemSmokeStackEmissionFrequency / 60f;
						}
					}
				}
			}
			for (int num15 = 0; num15 < GameplayController.Instance.spatter.Count; num15++)
			{
				SpatterSimulation spatterSimulation = GameplayController.Instance.spatter[num15];
				if (spatterSimulation.eraseMode != SpatterSimulation.EraseMode.neverOrManual)
				{
					if (spatterSimulation.eraseMode == SpatterSimulation.EraseMode.useDespawnTime)
					{
						if (SessionData.Instance.gameTime - spatterSimulation.createdAt >= GameplayControls.Instance.spatterRemovalTime)
						{
							spatterSimulation.Remove();
							num15--;
						}
					}
					else if (spatterSimulation.eraseMode == SpatterSimulation.EraseMode.useDespawnTimeOnceExecuted)
					{
						if (spatterSimulation.isExecuted && SessionData.Instance.gameTime - spatterSimulation.executedAt >= GameplayControls.Instance.spatterRemovalTime)
						{
							spatterSimulation.Remove();
							num15--;
						}
					}
					else if (spatterSimulation.eraseMode == SpatterSimulation.EraseMode.onceExecutedAndOutOfAddressPlusDespawnTime)
					{
						if (spatterSimulation.isExecuted)
						{
							if (spatterSimulation.room != null && Player.Instance.currentGameLocation != spatterSimulation.room.gameLocation)
							{
								if (spatterSimulation.eraseModeTimeStamp < 0f)
								{
									spatterSimulation.eraseModeTimeStamp = SessionData.Instance.gameTime;
								}
								else if (SessionData.Instance.gameTime - spatterSimulation.eraseModeTimeStamp >= GameplayControls.Instance.spatterRemovalTime)
								{
									spatterSimulation.Remove();
									num15--;
								}
							}
							else if (spatterSimulation.eraseModeTimeStamp < 0f)
							{
								spatterSimulation.eraseModeTimeStamp = SessionData.Instance.gameTime;
							}
							else if (SessionData.Instance.gameTime - spatterSimulation.eraseModeTimeStamp >= GameplayControls.Instance.spatterRemovalTime)
							{
								spatterSimulation.Remove();
								num15--;
							}
						}
					}
					else if (spatterSimulation.eraseMode == SpatterSimulation.EraseMode.onceExecutedAndOutOfBuildingPlusDespawnTime && spatterSimulation.isExecuted)
					{
						if (spatterSimulation.room != null && Player.Instance.currentBuilding != spatterSimulation.room.gameLocation.building)
						{
							if (spatterSimulation.eraseModeTimeStamp < 0f)
							{
								spatterSimulation.eraseModeTimeStamp = SessionData.Instance.gameTime;
							}
							else if (SessionData.Instance.gameTime - spatterSimulation.eraseModeTimeStamp >= GameplayControls.Instance.spatterRemovalTime)
							{
								spatterSimulation.Remove();
								num15--;
							}
						}
						else if (spatterSimulation.eraseModeTimeStamp < 0f)
						{
							spatterSimulation.eraseModeTimeStamp = SessionData.Instance.gameTime;
						}
						else if (SessionData.Instance.gameTime - spatterSimulation.eraseModeTimeStamp >= GameplayControls.Instance.spatterRemovalTime)
						{
							spatterSimulation.Remove();
							num15--;
						}
					}
				}
			}
			for (int num16 = 0; num16 < GameplayController.Instance.interactablesMoved.Count; num16++)
			{
				Interactable interactable7 = GameplayController.Instance.interactablesMoved[num16];
				if (interactable7.node.gameLocation == Player.Instance.home)
				{
					GameplayController.Instance.interactablesMoved.RemoveAt(num16);
					num16--;
				}
				else if (!interactable7.spR)
				{
					GameplayController.Instance.interactablesMoved.RemoveAt(num16);
					num16--;
				}
				else if ((InteractionController.Instance.carryingObject == null || InteractionController.Instance.carryingObject.interactable != interactable7) && (interactable7.node == null || Player.Instance.currentGameLocation != interactable7.node.gameLocation) && (interactable7.spawnNode == null || Player.Instance.currentGameLocation != interactable7.spawnNode.gameLocation) && SessionData.Instance.gameTime >= interactable7.lma + GameplayControls.Instance.objectPositionResetTime && interactable7.inInventory == null && (interactable7.controller == null || interactable7.node == null || !interactable7.node.room.isVisible))
				{
					if (interactable7.preset.GetPhysicsProfile().removeOnReset)
					{
						interactable7.SafeDelete(false);
					}
					else
					{
						interactable7.originalPosition = false;
						interactable7.SetOriginalPosition(true, true);
					}
					num16--;
				}
			}
			for (int num17 = 0; num17 < this.tempEscalationBoost.Count; num17++)
			{
				NewGameLocation newGameLocation = this.tempEscalationBoost[num17];
				List<Actor> list4 = new List<Actor>();
				List<Actor> list5 = new List<Actor>();
				foreach (KeyValuePair<Actor, NewGameLocation.TrespassEscalation> keyValuePair4 in newGameLocation.escalation)
				{
					bool flag4 = false;
					if (keyValuePair4.Key.ai != null && keyValuePair4.Key.ai.currentGoal != null)
					{
						flag4 = keyValuePair4.Key.ai.currentGoal.preset.allowEnforcersEverywhere;
					}
					keyValuePair4.Key.UpdateTrespassing(flag4);
					int num18;
					string text;
					if (keyValuePair4.Key.currentGameLocation != newGameLocation)
					{
						list4.Add(keyValuePair4.Key);
					}
					else if (!keyValuePair4.Key.IsTrespassing(keyValuePair4.Key.currentRoom, out num18, out text, flag4))
					{
						list4.Add(keyValuePair4.Key);
					}
				}
				foreach (Actor actor2 in list4)
				{
					newGameLocation.RemoveEscalation(actor2, false);
				}
				foreach (Actor actor3 in list5)
				{
					newGameLocation.RemoveEscalation(actor3, true);
				}
			}
			if (Player.Instance.hygiene < 0.99f && Player.Instance.isTrespassing && Player.Instance.currentRoom != null)
			{
				foreach (Actor actor4 in Player.Instance.currentRoom.currentOccupants)
				{
					if (!(actor4 == null) && !actor4.isDead && !actor4.isAsleep && !actor4.isStunned && actor4.ai != null && !actor4.ai.investigationGoal.isActive && Toolbox.Instance.Rand(0f, 1f, false) >= Player.Instance.hygiene)
					{
						actor4.speechController.TriggerBark(SpeechController.Bark.stench);
						actor4.ai.Investigate(Player.Instance.currentNode, Player.Instance.transform.position, Player.Instance, NewAIController.ReactionState.investigatingSound, 0.25f, 0, false, 1f, null);
					}
				}
			}
			if (Game.Instance.allowLoitering && Player.Instance.currentGameLocation != null && Player.Instance.currentGameLocation.thisAsAddress != null && Player.Instance.currentGameLocation.thisAsAddress.company != null && Player.Instance.currentGameLocation.thisAsAddress.company.preset.enableLoiteringBehaviour && !GameplayController.Instance.guestPasses.ContainsKey(Player.Instance.currentGameLocation.thisAsAddress) && UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.disableLoitering) < 1f)
			{
				Player.Instance.currentGameLocation.playerLoiteringTimer += num2;
				if (Player.Instance.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringConfrontThreshold)
				{
					Human human2 = null;
					bool flag5 = false;
					float num19 = -99999f;
					foreach (Actor actor5 in Player.Instance.currentRoom.currentOccupants)
					{
						Human human3 = (Human)actor5;
						if (human3 != null && human3.ai != null && !human3.isPlayer && human3.locationsOfAuthority.Contains(Player.Instance.currentGameLocation))
						{
							if (human3.ai.currentAction != null && human3.ai.currentAction.preset == RoutineControls.Instance.loiterConfront)
							{
								flag5 = true;
								break;
							}
							float num20 = 1000f - Vector3.Distance(human3.transform.position, Player.Instance.transform.position);
							if (num20 > num19)
							{
								num19 = num20;
								human2 = human3;
							}
						}
					}
					if (!flag5 && human2 != null && human2.ai.currentGoal != null)
					{
						if (!human2.ai.currentGoal.actions.Exists((NewAIAction item) => item.preset == RoutineControls.Instance.loiterConfront) && !human2.inConversation && human2.ai.currentGoal.TryInsertInteractableAction(Player.Instance.interactable, RoutineControls.Instance.loiterConfront, 8, null, true))
						{
							Game.Log("Triggered loitering confrontation: " + human2.GetCitizenName(), 2);
						}
					}
				}
				if (Player.Instance.currentGameLocation.playerLoiteringTimer >= GameplayControls.Instance.loiteringTrespassThreshold)
				{
					Player.Instance.UpdateIllegalStatus();
				}
			}
			if (GameplayController.Instance.guestPasses.Count > 0)
			{
				List<NewAddress> list6 = new List<NewAddress>();
				foreach (KeyValuePair<NewAddress, Vector2> keyValuePair5 in GameplayController.Instance.guestPasses)
				{
					if (SessionData.Instance.gameTime > keyValuePair5.Value.x)
					{
						list6.Add(keyValuePair5.Key);
					}
				}
				foreach (NewAddress newAddress3 in list6)
				{
					GameplayController.Instance.guestPasses.Remove(newAddress3);
					if (Player.Instance.currentRoom.gameLocation == newAddress3)
					{
						Player.Instance.OnRoomChange();
					}
				}
			}
			if (GameplayController.Instance.caseProcessing.Count > 0)
			{
				List<Case> list7 = new List<Case>();
				foreach (KeyValuePair<Case, float> keyValuePair6 in GameplayController.Instance.caseProcessing)
				{
					if (keyValuePair6.Key.caseType == Case.CaseType.murder || keyValuePair6.Key.caseType == Case.CaseType.mainStory)
					{
						Case.ResolveQuestion resolveQuestion = keyValuePair6.Key.resolveQuestions.Find((Case.ResolveQuestion item) => item.tag == JobPreset.JobTag.A);
						Case.ResolveQuestion detainQuestion = keyValuePair6.Key.resolveQuestions.Find((Case.ResolveQuestion item) => item.tag == JobPreset.JobTag.B);
						if (detainQuestion != null && resolveQuestion != null && detainQuestion.isValid && detainQuestion.inputType == Case.InputType.location)
						{
							Game.Log(string.Concat(new string[]
							{
								"Checking for detained suspect ",
								resolveQuestion.input,
								" at ",
								detainQuestion.input,
								"..."
							}), 2);
							foreach (NewGameLocation newGameLocation2 in CityData.Instance.gameLocationDirectory.FindAll((NewGameLocation item) => item.name.ToLower() == detainQuestion.input.ToLower()))
							{
								string[] array = new string[5];
								array[0] = "...Scanning occupants of ";
								array[1] = newGameLocation2.name;
								array[2] = " (";
								int num21 = 3;
								int num18 = newGameLocation2.currentOccupants.Count;
								array[num21] = num18.ToString();
								array[4] = ") ...";
								Game.Log(string.Concat(array), 2);
								foreach (Actor actor6 in newGameLocation2.currentOccupants)
								{
									if (!actor6.isPlayer && !actor6.isDead)
									{
										Citizen citizen = actor6 as Citizen;
										if (!(citizen == null) && !(actor6.ai == null) && actor6.ai.restrained && citizen.GetCitizenName().ToLower() == resolveQuestion.input.ToLower())
										{
											Game.Log("...Submit " + citizen.GetCitizenName() + " for suspect processing...", 2);
											if (!keyValuePair6.Key.suspectsDetained.Contains(citizen.humanID))
											{
												keyValuePair6.Key.suspectsDetained.Add(citizen.humanID);
											}
											citizen.RemoveFromWorld(true);
										}
									}
								}
							}
						}
					}
					Objective objective = keyValuePair6.Key.currentActiveObjectives.Find((Objective item) => item.queueElement.entryRef == "case processing");
					float num22 = keyValuePair6.Value + GameplayControls.Instance.caseResultProcessTime;
					float num23 = Mathf.Clamp01((SessionData.Instance.gameTime - keyValuePair6.Value) / (num22 - keyValuePair6.Value));
					if (objective != null)
					{
						objective.SetProgress(num23);
					}
					if (num23 >= 1f)
					{
						keyValuePair6.Key.Resolve();
						if (objective != null)
						{
							objective.Complete();
						}
						list7.Add(keyValuePair6.Key);
					}
				}
				foreach (Case @case in list7)
				{
					GameplayController.Instance.caseProcessing.Remove(@case);
				}
			}
			for (int num24 = 0; num24 < GameplayController.Instance.timeEvidence.Count; num24++)
			{
				EvidenceTime t = GameplayController.Instance.timeEvidence[num24];
				bool flag6 = false;
				Predicate<Case.CaseElement> <>9__8;
				foreach (Case case2 in CasePanelController.Instance.activeCases)
				{
					List<Case.CaseElement> caseElements = case2.caseElements;
					Predicate<Case.CaseElement> predicate;
					if ((predicate = <>9__8) == null)
					{
						predicate = (<>9__8 = ((Case.CaseElement item) => item.id == t.evID));
					}
					if (caseElements.Exists(predicate))
					{
						flag6 = true;
						break;
					}
				}
				if (!flag6)
				{
					Predicate<Case.CaseElement> <>9__9;
					foreach (Case case3 in CasePanelController.Instance.archivedCases)
					{
						List<Case.CaseElement> caseElements2 = case3.caseElements;
						Predicate<Case.CaseElement> predicate2;
						if ((predicate2 = <>9__9) == null)
						{
							predicate2 = (<>9__9 = ((Case.CaseElement item) => item.id == t.evID));
						}
						if (caseElements2.Exists(predicate2))
						{
							flag6 = true;
							break;
						}
					}
					if (!flag6)
					{
						if (GameplayController.Instance.history.Exists((GameplayController.History item) => item.evID == t.evID))
						{
							flag6 = true;
						}
						else if (InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence == t))
						{
							flag6 = true;
						}
						else if (!flag6)
						{
							GameplayController.Instance.evidenceDictionary.Remove(t.evID);
							GameplayController.Instance.timeEvidence.RemoveAt(num24);
							num24--;
						}
					}
				}
			}
			for (int num25 = 0; num25 < GameplayController.Instance.debt.Count; num25++)
			{
				GameplayController.LoanDebt loanDebt = GameplayController.Instance.debt[num25];
				if (SessionData.Instance.gameTime > loanDebt.nextPaymentDueBy && SessionData.Instance.gameTime > loanDebt.dueCheck && loanDebt.debt > 0)
				{
					loanDebt.missedPayments += Mathf.Min(loanDebt.payments, loanDebt.debt);
					string text2 = "Gameplay: Added a missed payment of ";
					int num18 = Mathf.Min(loanDebt.payments, loanDebt.debt);
					Game.Log(text2 + num18.ToString(), 2);
					float num26 = 24f - SessionData.Instance.decimalClock;
					num26 += 24.01f;
					loanDebt.dueCheck = (SessionData.Instance.gameTime += num26);
				}
			}
			foreach (NewGameLocation newGameLocation3 in GameplayController.Instance.crimeScenes)
			{
				if (newGameLocation3.thisAsAddress != null)
				{
					foreach (NewNode.NodeAccess nodeAccess3 in newGameLocation3.entrances)
					{
						if (nodeAccess3.walkingAccess && nodeAccess3.door != null && !nodeAccess3.door.forbiddenForPublic && !CityData.Instance.visibleRooms.Contains(nodeAccess3.door.wall.node.room) && !CityData.Instance.visibleRooms.Contains(nodeAccess3.door.wall.otherWall.node.room))
						{
							nodeAccess3.door.SetForbidden(true);
						}
					}
				}
			}
			for (int num27 = 0; num27 < GameplayController.Instance.policeTapeDoors.Count; num27++)
			{
				NewDoor newDoor = GameplayController.Instance.policeTapeDoors[num27];
				if (!GameplayController.Instance.crimeScenes.Contains(newDoor.wall.node.gameLocation) && !GameplayController.Instance.crimeScenes.Contains(newDoor.wall.otherWall.node.gameLocation))
				{
					if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null && !Game.Instance.sandboxMode)
					{
						ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
						if (chapterIntro != null && (chapterIntro.apartment == newDoor.wall.node.gameLocation || chapterIntro.apartment == newDoor.wall.otherWall.node.gameLocation))
						{
							goto IL_209F;
						}
					}
					newDoor.SetForbidden(false);
				}
				IL_209F:;
			}
			for (int num28 = 0; num28 < GameplayController.Instance.dateEvidence.Count; num28++)
			{
				EvidenceDate t = GameplayController.Instance.dateEvidence[num28];
				bool flag7 = false;
				Predicate<Case.CaseElement> <>9__12;
				foreach (Case case4 in CasePanelController.Instance.activeCases)
				{
					List<Case.CaseElement> caseElements3 = case4.caseElements;
					Predicate<Case.CaseElement> predicate3;
					if ((predicate3 = <>9__12) == null)
					{
						predicate3 = (<>9__12 = ((Case.CaseElement item) => item.id == t.evID));
					}
					if (caseElements3.Exists(predicate3))
					{
						flag7 = true;
						break;
					}
				}
				if (!flag7)
				{
					Predicate<Case.CaseElement> <>9__13;
					foreach (Case case5 in CasePanelController.Instance.archivedCases)
					{
						List<Case.CaseElement> caseElements4 = case5.caseElements;
						Predicate<Case.CaseElement> predicate4;
						if ((predicate4 = <>9__13) == null)
						{
							predicate4 = (<>9__13 = ((Case.CaseElement item) => item.id == t.evID));
						}
						if (caseElements4.Exists(predicate4))
						{
							flag7 = true;
							break;
						}
					}
					if (!flag7)
					{
						if (GameplayController.Instance.history.Exists((GameplayController.History item) => item.evID == t.evID))
						{
							flag7 = true;
						}
						else if (InterfaceController.Instance.activeWindows.Exists((InfoWindow item) => item.passedEvidence == t))
						{
							flag7 = true;
						}
						else if (!flag7)
						{
							GameplayController.Instance.evidenceDictionary.Remove(t.evID);
							GameplayController.Instance.dateEvidence.RemoveAt(num28);
							num28--;
						}
					}
				}
			}
			if (Player.Instance.drunk > 0f)
			{
				if (Player.Instance.drunk > 0.5f)
				{
					this.triggerHeadache = Mathf.Max(this.triggerHeadache, Player.Instance.drunk);
				}
				Player.Instance.AddDrunk(num2 * -0.4f);
			}
			if (Player.Instance.drunk < 0.15f && CitizenBehaviour.Instance.triggerHeadache > 0f)
			{
				float num29 = Mathf.Min(CitizenBehaviour.Instance.triggerHeadache * (num2 / 0.01f), CitizenBehaviour.Instance.triggerHeadache);
				Player.Instance.AddHeadache(num29);
				Player.Instance.AddHygiene(num29 * -0.7f);
				Player.Instance.AddHydration(num29 * -0.5f);
				CitizenBehaviour.Instance.triggerHeadache -= num29;
			}
			if (Player.Instance.bruised > 0f)
			{
				Player.Instance.AddBruised(num2 * -0.2f);
			}
			if (Player.Instance.blackEye > 0f)
			{
				Player.Instance.AddBlackEye(num2 * -0.2f);
			}
			if (Player.Instance.sick > 0f)
			{
				Player.Instance.AddSick(num2 * -0.5f);
			}
			if (Player.Instance.numb > 0f)
			{
				Player.Instance.AddNumb(num2 * -2f);
			}
			if (Player.Instance.poisoned > 0f)
			{
				Player.Instance.AddHealth(Player.Instance.poisoned * -10f * num2, true, false);
				Player.Instance.AddPoisoned(-num2, null);
			}
			if (Player.Instance.gasLevel > 0f && !Player.Instance.playerKOInProgress)
			{
				Player.Instance.AddHealth(Player.Instance.gasLevel * -10f * num2, true, false);
			}
			if (Player.Instance.bleeding > 0f)
			{
				Player.Instance.AddBleeding(num2 * -0.5f);
			}
			float upgradeEffect = UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.starchAddiction);
			if (upgradeEffect != 0f)
			{
				Player.Instance.AddStarchAddiction(num2 * upgradeEffect);
			}
			if (Player.Instance.spendingTimeMode && InteractionController.Instance.lockedInInteraction != null && InteractionController.Instance.lockedInInteraction.preset.specialCaseFlag == InteractablePreset.SpecialCase.sleepPosition)
			{
				if (Player.Instance.energy >= 0.8f)
				{
					Player.Instance.AddWellRested(num2 / 2f);
				}
				Player.Instance.AddNourishment(GameplayControls.Instance.playerHungerRate * 0.5f * -num2);
				Player.Instance.AddHydration(GameplayControls.Instance.playerThirstRate * 0.5f * -num2);
				Player.Instance.AddEnergy(num2 / 4f);
			}
			else
			{
				int num18;
				Chapter chapter;
				if (!Game.Instance.disableSurvivalStatusesInStory || !Toolbox.Instance.IsStoryMissionActive(out chapter, out num18))
				{
					Player.Instance.AddNourishment(GameplayControls.Instance.playerHungerRate * -num2);
					Player.Instance.AddHydration(GameplayControls.Instance.playerThirstRate * -num2);
					Player.Instance.AddAlertness(GameplayControls.Instance.playerTirednessRate * -num2);
				}
				Player.Instance.AddWellRested(num2 * -0.5f);
			}
			Player.Instance.AddAlertness(GameplayControls.Instance.playerTirednessRate * -num2);
			if (Player.Instance.wet > 0f)
			{
				Player.Instance.AddWet(num2 * (-0.25f + Player.Instance.heat) * -3f);
			}
			if (Player.Instance.blackedOut > 0f)
			{
				Player.Instance.AddBlackedOut(num2 * -5f);
			}
			if (Player.Instance.brokenLeg > 0f)
			{
				Player.Instance.AddBrokenLeg(num2 * -0.1f);
			}
			if (Player.Instance.headache > 0f)
			{
				if (!Player.Instance.isRunning)
				{
					Player.Instance.AddHeadache(num2 * -0.75f);
				}
				else if (Player.Instance.isRunning && Player.Instance.headache >= 0.1f)
				{
					Player.Instance.AddHeadache(num2 * 2f);
				}
			}
			if (Player.Instance.currentNode != null)
			{
				foreach (Interactable interactable8 in Player.Instance.currentNode.interactables)
				{
					if (interactable8.preset.specialCaseFlag == InteractablePreset.SpecialCase.shower && interactable8.sw0)
					{
						bool flag8 = false;
						if (Player.Instance.hygiene < 1f)
						{
							flag8 = true;
						}
						Player.Instance.AddHygiene(num2 * 35f);
						Player.Instance.AddHeat(num2 * 42f);
						Player.Instance.AddWet(num2 * 50f);
						if (AchievementsController.Instance != null && flag8 && Player.Instance.hygiene >= 1f)
						{
							AchievementsController.Instance.UnlockAchievement("Fresh as a Daisy", "shower");
							break;
						}
						break;
					}
				}
			}
			if (!CutSceneController.Instance.cutSceneActive)
			{
				if (Player.Instance.isOnStreet)
				{
					Player.Instance.AddHeat(num2 * SessionData.Instance.temperature * 1.5f);
					Player.Instance.AddWet(num2 * SessionData.Instance.currentRain * 8f);
				}
				else if (Player.Instance.inAirVent)
				{
					Player.Instance.AddHeat(num2 * GameplayControls.Instance.airDuctTemperature * 1.75f);
				}
				else
				{
					Player.Instance.AddHeat(num2 * GameplayControls.Instance.indoorTemperature * 2.1f);
				}
			}
			if (Player.Instance.heat < 1f && Player.Instance.currentRoom != null)
			{
				foreach (Interactable interactable9 in Player.Instance.currentRoom.heatSources)
				{
					float num30 = Vector3.Distance(interactable9.wPos, Player.Instance.transform.position);
					if (num30 <= 5f)
					{
						Player.Instance.AddHeat(num2 * (GameplayControls.Instance.heatSourceTemperature * (1f - num30 / 5f)));
					}
				}
			}
			if (Player.Instance.heat < 0.5f)
			{
				Player.Instance.AddHealth(-0.1f * num2, true, false);
			}
			if (Player.Instance.bleeding > 0f)
			{
				Player.Instance.AddHealth(Player.Instance.bleeding * 3f * -num2, true, false);
				new SpatterSimulation(Player.Instance, new Vector3(0f, 1f, 0f), Vector3.down, GameplayControls.Instance.bleedingSpatter, SpatterSimulation.EraseMode.onceExecutedAndOutOfAddressPlusDespawnTime, Player.Instance.bleeding, false);
			}
			int num31 = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.passiveIncome));
			if ((float)num31 > 0f)
			{
				this.passiveIncomeTimer += num2;
				if (this.passiveIncomeTimer >= 2f)
				{
					GameplayController.Instance.AddMoney(Mathf.Max(num31, 1), true, "passiveincome");
					this.passiveIncomeTimer = 0f;
				}
			}
			List<NewDoor> list8 = new List<NewDoor>();
			foreach (NewDoor newDoor2 in GameplayController.Instance.damagedDoors)
			{
				if (newDoor2.wall.currentDoorStrength < newDoor2.wall.baseDoorStrength)
				{
					if (newDoor2.wall.node.gameLocation != Player.Instance.currentGameLocation && newDoor2.wall.otherWall.node.gameLocation != Player.Instance.currentGameLocation && newDoor2.wall.node.building != Player.Instance.currentBuilding && newDoor2.wall.otherWall.node.building != Player.Instance.currentBuilding)
					{
						newDoor2.wall.SetDoorStrength(Mathf.Min(newDoor2.wall.currentDoorStrength + 0.01f, newDoor2.wall.baseDoorStrength));
					}
				}
				else
				{
					list8.Add(newDoor2);
				}
			}
			foreach (NewDoor newDoor3 in list8)
			{
				GameplayController.Instance.damagedDoors.Remove(newDoor3);
			}
			SideJobController.Instance.JobCreationCheck();
			List<NewGameLocation> list9 = new List<NewGameLocation>();
			using (List<Human>.Enumerator enumerator18 = CityData.Instance.deadCitizensDirectory.GetEnumerator())
			{
				while (enumerator18.MoveNext())
				{
					Human h = enumerator18.Current;
					if (h.death != null && h.death.isDead)
					{
						h.death.smell += num2 / GameplayControls.Instance.smellTime;
						if (h.death.smell > 0.1f)
						{
							foreach (Human.Wound wound in h.currentWounds)
							{
								wound.ProcessBloodPoolForWound();
							}
						}
						if (!h.death.reported && h.currentRoom != null && !h.unreportable && !MurderController.Instance.playerAcceptedCoverUp)
						{
							MurderController.Murder murder = h.death.GetMurder();
							if (murder == null || murder.preset.caseType != MurderPreset.CaseType.kidnap)
							{
								int num32 = Mathf.FloorToInt(h.death.smell);
								HashSet<NewRoom> hashSet3 = new HashSet<NewRoom>();
								hashSet3.Add(h.currentRoom);
								HashSet<NewRoom> hashSet4 = new HashSet<NewRoom>();
								Game.Log(string.Concat(new string[]
								{
									"Murder: Checking dead body smell (",
									h.death.smell.ToString(),
									") of ",
									h.citizenName,
									" at ",
									h.currentRoom.GetName(),
									", scanning room extents: ",
									num32.ToString(),
									"..."
								}), 2);
								Predicate<NewAIGoal> <>9__14;
								while (hashSet3.Count > 0 && num32 > 0)
								{
									NewRoom newRoom6 = Enumerable.First<NewRoom>(hashSet3);
									string[] array2 = new string[7];
									array2[0] = "Murder: Smell scanning: ";
									array2[1] = newRoom6.name;
									array2[2] = " with ";
									int num33 = 3;
									int num18 = newRoom6.currentOccupants.Count;
									array2[num33] = num18.ToString();
									array2[4] = " occupants and ";
									int num34 = 5;
									num18 = newRoom6.entrances.Count;
									array2[num34] = num18.ToString();
									array2[6] = " entrances...";
									Game.Log(string.Concat(array2), 2);
									foreach (Actor actor7 in new List<Actor>(newRoom6.currentOccupants))
									{
										Human human4 = actor7 as Human;
										if (human4 != null && !human4.isPlayer && human4.ai != null && !human4.isDead && human4 != h.death.GetKiller())
										{
											List<NewAIGoal> goals = human4.ai.goals;
											Predicate<NewAIGoal> predicate5;
											if ((predicate5 = <>9__14) == null)
											{
												predicate5 = (<>9__14 = ((NewAIGoal item) => (item.preset == RoutineControls.Instance.smellDeadBody || item.preset == RoutineControls.Instance.findDeadBody) && item.passedInteractable == h.interactable));
											}
											if (goals.Exists(predicate5))
											{
												Game.Log("Murder: " + human4.GetCitizenName() + " already smells dead body...", 2);
												break;
											}
											if (!human4.isAsleep && !human4.isStunned)
											{
												Game.Log("Murder: " + human4.GetCitizenName() + " smells dead body...", 2);
												human4.ai.CreateNewGoal(RoutineControls.Instance.smellDeadBody, 0f, 0f, null, h.interactable, null, null, null, 1);
											}
										}
									}
									foreach (NewNode.NodeAccess nodeAccess4 in newRoom6.entrances)
									{
										if (nodeAccess4.walkingAccess)
										{
											NewRoom otherRoom3 = nodeAccess4.GetOtherRoom(newRoom6);
											if (!hashSet4.Contains(otherRoom3) && !hashSet3.Contains(otherRoom3))
											{
												hashSet3.Add(otherRoom3);
											}
										}
									}
									hashSet3.Remove(newRoom6);
									hashSet4.Add(newRoom6);
									num32--;
								}
							}
						}
					}
				}
			}
			foreach (KeyValuePair<NewGameLocation, GameplayController.EnforcerCall> keyValuePair7 in GameplayController.Instance.enforcerCalls)
			{
				if (keyValuePair7.Value.state == GameplayController.EnforcerCallState.logged)
				{
					List<Human> list10 = new List<Human>();
					List<Human> list11 = new List<Human>();
					List<float> list12 = new List<float>();
					foreach (Human human5 in GameplayController.Instance.enforcers)
					{
						if (!human5.isStunned && !human5.isDead && (human5.ai.currentGoal == null || !(human5.ai.currentGoal.preset == RoutineControls.Instance.enforcerResponse)))
						{
							bool flag9 = false;
							foreach (KeyValuePair<NewGameLocation, GameplayController.EnforcerCall> keyValuePair8 in GameplayController.Instance.enforcerCalls)
							{
								if (keyValuePair8.Value.response != null && keyValuePair8.Value.response.Contains(human5.humanID))
								{
									flag9 = true;
									break;
								}
							}
							if (!flag9)
							{
								float num35 = -Vector3.Distance(human5.transform.position, keyValuePair7.Key.entrances[0].worldAccessPoint);
								if (human5.ai.currentGoal != null && human5.ai.currentGoal.preset == RoutineControls.Instance.workGoal)
								{
									num35 += 1000f;
								}
								if (human5.isStunned)
								{
									num35 -= 10000f;
								}
								if (human5.isAsleep)
								{
									num35 -= 100f;
								}
								list11.Add(human5);
								list12.Add(num35);
							}
						}
					}
					if (list11.Count >= 2)
					{
						while (list10.Count < 2 && list11.Count > 0)
						{
							int num36 = -1;
							float num37 = -1E+09f;
							for (int num38 = 0; num38 < list12.Count; num38++)
							{
								if (list12[num38] > num37)
								{
									num36 = num38;
									num37 = list12[num38];
								}
							}
							if (num36 <= -1)
							{
								break;
							}
							list10.Add(list11[num36]);
							list11.RemoveAt(num36);
							list12.RemoveAt(num36);
						}
						if (keyValuePair7.Value.response == null)
						{
							keyValuePair7.Value.response = new List<int>();
						}
						foreach (Human human6 in list10)
						{
							Game.Log("Gameplay: Enforcer responding to call at " + keyValuePair7.Key.name + ": " + human6.GetCitizenName(), 2);
							keyValuePair7.Value.response.Add(human6.humanID);
							if (keyValuePair7.Value.immedaiteTeleport && keyValuePair7.Key.entrances.Count > 0)
							{
								NewNode.NodeAccess mainEntrance = keyValuePair7.Key.GetMainEntrance();
								if (mainEntrance != null)
								{
									NewRoom otherRoom4 = mainEntrance.GetOtherRoom(keyValuePair7.Key);
									if (otherRoom4 != null)
									{
										NewNode newNode = null;
										float num39 = 0f;
										foreach (NewNode newNode2 in otherRoom4.nodes)
										{
											if (!newNode2.noPassThrough && !newNode2.noAccess && newNode2.accessToOtherNodes.Count > 0)
											{
												float num40 = Vector3.Distance(newNode2.position, mainEntrance.worldAccessPoint);
												if (newNode == null || num40 > num39)
												{
													newNode = newNode2;
													num39 = num40;
												}
											}
										}
										string[] array3 = new string[6];
										array3[0] = "Gameplay: Teleporting enforcer ";
										array3[1] = human6.GetCitizenName();
										array3[2] = " to ";
										array3[3] = otherRoom4.name;
										array3[4] = ", pos: ";
										int num41 = 5;
										Vector3 vector = newNode.position;
										array3[num41] = vector.ToString();
										Game.Log(string.Concat(array3), 2);
										human6.Teleport(newNode, null, true, false);
									}
								}
							}
							if (human6.ai.currentGoal != null && human6.ai.currentGoal.preset != RoutineControls.Instance.enforcerResponse)
							{
								if (human6.ai.currentAction != null)
								{
									human6.ai.currentAction.Remove(human6.ai.currentAction.preset.repeatDelayOnActionFail);
								}
								Game.Log(string.Concat(new string[]
								{
									"Gameplay: Enforcer ",
									human6.GetCitizenName(),
									" deactivating goal ",
									human6.ai.currentGoal.preset.name,
									" to enable enforcer response"
								}), 2);
								human6.ai.currentGoal.OnDeactivate(1f);
							}
							human6.ai.CreateNewGoal(RoutineControls.Instance.enforcerResponse, 0f, 0f, null, null, keyValuePair7.Key, null, null, -2).OnActivate();
							human6.ai.AITick(true, false);
						}
						keyValuePair7.Value.state = GameplayController.EnforcerCallState.responding;
					}
				}
				else
				{
					if (keyValuePair7.Value.state == GameplayController.EnforcerCallState.responding)
					{
						using (List<int>.Enumerator enumerator23 = keyValuePair7.Value.response.GetEnumerator())
						{
							while (enumerator23.MoveNext())
							{
								int id = enumerator23.Current;
								Human human7 = null;
								if (CityData.Instance.GetHuman(id, out human7, true) && keyValuePair7.Key.currentOccupants.Contains(human7))
								{
									keyValuePair7.Value.arrivalTime = SessionData.Instance.gameTime;
									keyValuePair7.Value.state = GameplayController.EnforcerCallState.arrived;
									break;
								}
							}
							continue;
						}
					}
					if (keyValuePair7.Value.state == GameplayController.EnforcerCallState.arrived)
					{
						foreach (Actor actor8 in keyValuePair7.Key.currentOccupants)
						{
							if ((actor8.isDead || (actor8.ai != null && actor8.ai.ko)) && !actor8.unreportable)
							{
								keyValuePair7.Value.isCrimeScene = true;
								Human human8 = actor8 as Human;
								if (!(human8 != null) || human8.death == null || !human8.death.isDead || human8.death.reported)
								{
									continue;
								}
								using (List<int>.Enumerator enumerator23 = keyValuePair7.Value.response.GetEnumerator())
								{
									while (enumerator23.MoveNext())
									{
										int id2 = enumerator23.Current;
										Human human9 = null;
										if (CityData.Instance.GetHuman(id2, out human9, true) && keyValuePair7.Key.currentOccupants.Contains(human9))
										{
											human8.death.SetReported(human9, Human.Death.ReportType.visual);
											break;
										}
									}
									continue;
								}
							}
							Human human10 = actor8 as Human;
							if (human10 != null && human10.ai != null && human10.home == keyValuePair7.Key)
							{
								if (!actor8.ai.goals.Exists((NewAIGoal item) => item.preset == RoutineControls.Instance.layLow))
								{
									float newDuration = 1f;
									if (keyValuePair7.Value.isCrimeScene)
									{
										newDuration = 12f;
									}
									NewGameLocation newGameLocation4 = human10.home;
									if (newGameLocation4 == null || newGameLocation4.isCrimeScene)
									{
										Company company = CityData.Instance.companyDirectory.Find((Company item) => item.publicFacing && item.placeOfBusiness != null && !item.placeOfBusiness.isCrimeScene && item.IsOpenAtThisTime(SessionData.Instance.gameTime));
										if (company != null)
										{
											newGameLocation4 = company.placeOfBusiness;
										}
									}
									if (newGameLocation4 != null)
									{
										actor8.ai.CreateNewGoal(RoutineControls.Instance.layLow, 0f, newDuration, null, null, newGameLocation4, null, null, -2);
									}
								}
							}
						}
						if (!keyValuePair7.Value.isCrimeScene)
						{
							if (SessionData.Instance.gameTime > keyValuePair7.Value.arrivalTime + GameplayControls.Instance.crimeSceneSearchLength)
							{
								bool flag10 = false;
								foreach (int id3 in keyValuePair7.Value.response)
								{
									Human human11 = null;
									if (CityData.Instance.GetHuman(id3, out human11, true))
									{
										human11.ai.dontEverCloseDoors = true;
										if (keyValuePair7.Key.currentOccupants.Contains(human11))
										{
											flag10 = true;
										}
									}
								}
								if (flag10)
								{
									keyValuePair7.Value.state = GameplayController.EnforcerCallState.completed;
								}
							}
						}
						else if (SessionData.Instance.gameTime > keyValuePair7.Value.arrivalTime + GameplayControls.Instance.crimeSceneSearchLength)
						{
							keyValuePair7.Key.SetAsCrimeScene(true);
							keyValuePair7.Key.loggedAsCrimeScene = keyValuePair7.Value.logTime;
							List<Human> list13 = new List<Human>();
							foreach (int id4 in keyValuePair7.Value.response)
							{
								Human human12 = null;
								if (CityData.Instance.GetHuman(id4, out human12, true))
								{
									human12.ai.dontEverCloseDoors = true;
									if (keyValuePair7.Key.currentOccupants.Contains(human12) && !list13.Contains(human12))
									{
										list13.Add(human12);
									}
								}
							}
							if (list13.Count > 0 && keyValuePair7.Value.guard <= -1)
							{
								Human human13 = list13[0];
								keyValuePair7.Value.guard = human13.humanID;
								Game.Log("Gameplay: Set " + human13.GetCitizenName() + " as guard for crime scene", 2);
								NewGameLocation key = keyValuePair7.Key;
								List<NewGameLocation> list14 = new List<NewGameLocation>();
								foreach (NewNode.NodeAccess nodeAccess5 in key.entrances)
								{
									if (nodeAccess5.walkingAccess)
									{
										list14.Add(nodeAccess5.GetOtherGameLocation(keyValuePair7.Key));
									}
								}
								human13.ai.CreateNewGoal(RoutineControls.Instance.enforcerGuardDuty, 0f, 24f, null, null, list14[Toolbox.Instance.Rand(0, list14.Count, false)], null, null, -2);
								NewAIGoal newAIGoal = human13.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.enforcerResponse);
								if (newAIGoal != null)
								{
									newAIGoal.Complete();
								}
							}
							else if (keyValuePair7.Value.guard > -1)
							{
								for (int num42 = 0; num42 < keyValuePair7.Value.response.Count; num42++)
								{
									int num43 = keyValuePair7.Value.response[num42];
									if (num43 != keyValuePair7.Value.guard)
									{
										Human human14 = null;
										if (CityData.Instance.GetHuman(num43, out human14, true))
										{
											NewAIGoal newAIGoal2 = human14.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.enforcerResponse);
											human14.ai.CloseDoorsNormallyAfterLeavingGamelocation(keyValuePair7.Key);
											if (newAIGoal2 != null)
											{
												newAIGoal2.Complete();
											}
										}
										keyValuePair7.Value.response.RemoveAt(num42);
										num42--;
									}
								}
								if (SessionData.Instance.gameTime >= keyValuePair7.Value.arrivalTime + GameplayControls.Instance.crimeSceneSearchLength + GameplayControls.Instance.crimeSceneLength)
								{
									keyValuePair7.Value.state = GameplayController.EnforcerCallState.completed;
								}
							}
						}
					}
					else if (keyValuePair7.Value.state == GameplayController.EnforcerCallState.completed)
					{
						Game.Log("Gameplay: Enforcer call to " + keyValuePair7.Key.name + " is completed...", 2);
						if (keyValuePair7.Value.isCrimeScene && !GameplayController.Instance.crimeSceneCleanups.Contains(keyValuePair7.Key))
						{
							GameplayController.Instance.crimeSceneCleanups.Add(keyValuePair7.Key);
						}
						keyValuePair7.Value.isCrimeScene = false;
						keyValuePair7.Key.SetAsCrimeScene(false);
						foreach (int id5 in keyValuePair7.Value.response)
						{
							Human human15 = null;
							if (CityData.Instance.GetHuman(id5, out human15, true))
							{
								NewAIGoal newAIGoal3 = human15.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.enforcerResponse);
								if (newAIGoal3 != null)
								{
									newAIGoal3.Complete();
								}
								NewAIGoal newAIGoal4 = human15.ai.goals.Find((NewAIGoal item) => item.preset == RoutineControls.Instance.enforcerGuardDuty);
								if (newAIGoal4 != null)
								{
									newAIGoal4.Complete();
								}
								human15.ai.CloseDoorsNormallyAfterLeavingGamelocation(keyValuePair7.Key);
							}
						}
						if (!list9.Contains(keyValuePair7.Key))
						{
							list9.Add(keyValuePair7.Key);
						}
					}
				}
			}
			foreach (NewGameLocation newGameLocation5 in list9)
			{
				GameplayController.Instance.enforcerCalls.Remove(newGameLocation5);
			}
			if (GameplayController.Instance.brokenWindows.Count > 0)
			{
				List<Vector3> list15 = new List<Vector3>();
				foreach (KeyValuePair<Vector3, float> keyValuePair9 in GameplayController.Instance.brokenWindows)
				{
					if (SessionData.Instance.gameTime >= keyValuePair9.Value + GameplayControls.Instance.brokenWindowResetTime)
					{
						list15.Add(keyValuePair9.Key);
					}
				}
				foreach (Vector3 vector2 in list15)
				{
					string text3 = "Fixing window at ";
					Vector3 vector = vector2;
					Game.Log(text3 + vector.ToString(), 2);
					GameplayController.Instance.brokenWindows.Remove(vector2);
				}
			}
			if (GameplayController.Instance.activeKettles.Count > 0)
			{
				for (int num44 = 0; num44 < GameplayController.Instance.activeKettles.Count; num44++)
				{
					Interactable interactable10 = GameplayController.Instance.activeKettles[num44];
					if (interactable10.sw0)
					{
						if (!interactable10.sw2)
						{
							interactable10.SetCustomState2(true, null, true, false, false);
						}
						if (interactable10.cs < 1f)
						{
							interactable10.cs += num2 * 7f;
							interactable10.cs = Mathf.Clamp01(interactable10.cs);
							interactable10.UpdateLoopingAudioParams();
						}
					}
					else if (interactable10.cs > 0f)
					{
						interactable10.cs -= num2 * 30f;
						interactable10.cs = Mathf.Clamp01(interactable10.cs);
						interactable10.UpdateLoopingAudioParams();
						if (interactable10.cs <= 0f)
						{
							if (interactable10.sw2)
							{
								interactable10.SetCustomState2(false, null, true, false, false);
							}
							GameplayController.Instance.activeKettles.RemoveAt(num44);
							num44--;
						}
					}
				}
			}
			if (GameplayController.Instance.hotelGuests.Count > 0)
			{
				for (int num45 = 0; num45 < GameplayController.Instance.hotelGuests.Count; num45++)
				{
					GameplayController.HotelGuest hotelGuest = GameplayController.Instance.hotelGuests[num45];
					if (SessionData.Instance.gameTime >= hotelGuest.nextPayment)
					{
						hotelGuest.bill += hotelGuest.roomCost;
						hotelGuest.nextPayment += 24f;
					}
					if (SessionData.Instance.gameTime >= hotelGuest.lastPayment + 24f + CityControls.Instance.kickoutTime)
					{
						Human human16 = hotelGuest.GetHuman();
						if (human16 != null && human16.isPlayer)
						{
							NewAddress address = hotelGuest.GetAddress();
							if (address != null && human16.currentBuilding != address.building)
							{
								List<Interactable> list16 = new List<Interactable>();
								foreach (NewRoom newRoom7 in address.rooms)
								{
									foreach (NewNode newNode3 in newRoom7.nodes)
									{
										for (int num46 = 0; num46 < newNode3.interactables.Count; num46++)
										{
											Interactable interactable11 = newNode3.interactables[num46];
											if (interactable11 != null && interactable11.preset.spawnable && interactable11.preset.prefab != null && interactable11.belongsTo != null)
											{
												list16.Add(interactable11);
											}
										}
									}
								}
								foreach (Interactable interactable12 in list16)
								{
									Game.Log("Gameplay: Repossessing " + interactable12.GetName(), 2);
									interactable12.SafeDelete(false);
								}
								InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "hotel_kickout1", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								InterfaceController.Instance.NewGameMessage(InterfaceController.GameMessageType.notification, 0, Strings.Get("ui.gamemessage", "hotel_kickout2", Strings.Casing.asIs, false, false, false, null), InterfaceControls.Icon.money, null, true, InterfaceControls.Instance.messageRed, -1, 0f, null, GameMessageController.PingOnComplete.none, null, null, null);
								GameplayController.Instance.RemoveHotelGuest(address, human16, false);
								StatusController.Instance.SetWantedInBuilding(address.building, GameplayControls.Instance.buildingWantedTime);
							}
						}
					}
				}
			}
			if (GameplayController.Instance.activeMusicPlayers.Count > 0)
			{
				for (int num47 = 0; num47 < GameplayController.Instance.activeMusicPlayers.Count; num47++)
				{
					GameplayController.Instance.activeMusicPlayers[num47].UpdateMusicPlayer();
				}
			}
			if (GameplayController.Instance.doorKnockAttempts.Count > 0)
			{
				List<NewDoor> list17 = new List<NewDoor>();
				foreach (KeyValuePair<NewDoor, List<GameplayController.DoorKnockAttempt>> keyValuePair10 in GameplayController.Instance.doorKnockAttempts)
				{
					for (int num48 = 0; num48 < keyValuePair10.Value.Count; num48++)
					{
						GameplayController.DoorKnockAttempt doorKnockAttempt = keyValuePair10.Value[num48];
						doorKnockAttempt.value -= num2 / 0.5f;
						if (doorKnockAttempt.value <= 0f)
						{
							keyValuePair10.Value.RemoveAt(num48);
							num48--;
						}
					}
					if (keyValuePair10.Value.Count <= 0)
					{
						list17.Add(keyValuePair10.Key);
					}
				}
				foreach (NewDoor newDoor4 in list17)
				{
					GameplayController.Instance.doorKnockAttempts.Remove(newDoor4);
				}
			}
			if (GameplayController.Instance.crimeSceneCleanups.Count > 0)
			{
				for (int num49 = 0; num49 < GameplayController.Instance.crimeSceneCleanups.Count; num49++)
				{
					NewGameLocation newGameLocation6 = GameplayController.Instance.crimeSceneCleanups[num49];
					if ((!(Player.Instance.currentGameLocation != null) || !(Player.Instance.currentGameLocation == newGameLocation6)) && (!(Player.Instance.previousGameLocation != null) || !(Player.Instance.previousGameLocation == newGameLocation6)) && (!(newGameLocation6.building != null) || !(Player.Instance.currentBuilding == newGameLocation6.building)) && SessionData.Instance.gameTime >= newGameLocation6.loggedAsCrimeScene + GameplayControls.Instance.crimeSceneCleanupDelay && Game.Instance.sandboxMode)
					{
						for (int num50 = 0; num50 < newGameLocation6.currentOccupants.Count; num50++)
						{
							Actor actor9 = newGameLocation6.currentOccupants[num50];
							if (actor9.isDead)
							{
								Human human17 = actor9 as Human;
								foreach (MurderController.Murder murder2 in MurderController.Instance.activeMurders)
								{
									if (murder2.victim == human17)
									{
										murder2.OnCleanCrimeScene();
									}
								}
								foreach (MurderController.Murder murder3 in MurderController.Instance.inactiveMurders)
								{
									if (murder3.victim == human17)
									{
										murder3.OnCleanCrimeScene();
									}
								}
								if (human17 != null)
								{
									human17.RemoveFromWorld(true);
								}
							}
						}
						if (newGameLocation6.thisAsAddress != null)
						{
							foreach (NewNode.NodeAccess nodeAccess6 in newGameLocation6.entrances)
							{
								if (nodeAccess6.door != null)
								{
									if (nodeAccess6.door.forbiddenForPublic)
									{
										nodeAccess6.door.SetForbidden(false);
									}
									nodeAccess6.door.SetOpen(0f, null, true, 1f);
								}
							}
							if (newGameLocation6.thisAsAddress.residence != null && (newGameLocation6.thisAsAddress.company == null || newGameLocation6.thisAsAddress.company.preset.isSelfEmployed))
							{
								if (newGameLocation6.thisAsAddress.owners.FindAll((Human item) => !item.isDead).Count <= 0)
								{
									newGameLocation6.RemoveAllInhabitantFurniture(false, FurnitureClusterLocation.RemoveInteractablesOption.remove);
									newGameLocation6.thisAsAddress.inhabitants.Clear();
									newGameLocation6.thisAsAddress.owners.Clear();
								}
							}
						}
						GameplayController.Instance.crimeSceneCleanups.RemoveAt(num49);
						num49--;
					}
				}
			}
			MurderController.Instance.Tick(num2);
			this.timeOnLastGameWorldUpdate = SessionData.Instance.gameTime;
			return;
		}
		goto IL_C20;
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x000570F4 File Offset: 0x000552F4
	private void LightLevelLoop()
	{
		if (!this.actorsInStealthMode.Contains(Player.Instance))
		{
			Player.Instance.StealthModeLoop();
		}
		foreach (Actor actor in this.actorsInStealthMode)
		{
			actor.StealthModeLoop();
		}
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x00057160 File Offset: 0x00055360
	public void OnHourChange()
	{
		float num = 1f / GameplayControls.Instance.maximumFootprintTime;
		foreach (KeyValuePair<NewRoom, List<GameplayController.Footprint>> keyValuePair in GameplayController.Instance.activeFootprints)
		{
			for (int i = 0; i < keyValuePair.Value.Count; i++)
			{
				GameplayController.Footprint footprint = keyValuePair.Value[i];
				footprint.str -= num;
				footprint.bl -= num;
				if (Mathf.Max(footprint.str, footprint.bl) <= 0.1f)
				{
					GameplayController.Instance.footprintsList.Remove(footprint);
					keyValuePair.Value.RemoveAt(i);
					i--;
				}
			}
		}
		CleanupController.Instance.TrashUpdate();
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00057254 File Offset: 0x00055454
	public void OnDayChange()
	{
		this.UpdateForSale();
		Player.Instance.claimedAccidentCover = false;
		Player.Instance.foodHygeinePhotos.Clear();
		Player.Instance.sanitaryHygeinePhotos.Clear();
		Player.Instance.illegalOpsPhotos.Clear();
		foreach (Citizen citizen in CityData.Instance.citizenDirectory)
		{
			using (List<DialogPreset>.Enumerator enumerator2 = Toolbox.Instance.defaultDialogOptions.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					DialogPreset dia = enumerator2.Current;
					if (dia.dailyReplenish)
					{
						if (citizen.evidenceEntry.dialogOptions.ContainsKey(dia.tiedToKey))
						{
							if (!citizen.evidenceEntry.dialogOptions[dia.tiedToKey].Exists((EvidenceWitness.DialogOption item) => item.preset == dia))
							{
								citizen.evidenceEntry.AddDialogOption(dia.tiedToKey, dia, null, null, true);
							}
						}
						else
						{
							citizen.evidenceEntry.AddDialogOption(dia.tiedToKey, dia, null, null, true);
						}
					}
				}
			}
			citizen.WalletItemCheck(1);
		}
		foreach (NewAddress newAddress in CityData.Instance.addressDirectory)
		{
			if (newAddress.vandalism.Count > 0)
			{
				for (int i = 0; i < newAddress.vandalism.Count; i++)
				{
					NewAddress.Vandalism vandalism = newAddress.vandalism[i];
					if (SessionData.Instance.gameTime >= vandalism.time + GameplayControls.Instance.vandalismTimeout)
					{
						newAddress.vandalism.RemoveAt(i);
						i--;
					}
				}
			}
		}
		foreach (NewBuilding newBuilding in HighlanderSingleton<CityBuildings>.Instance.buildingDirectory)
		{
			int num = newBuilding.preset.maxLostAndFound - newBuilding.lostAndFound.Count;
			for (int j = 0; j < num; j++)
			{
				newBuilding.TriggerNewLostAndFound();
			}
		}
	}

	// Token: 0x06000556 RID: 1366 RVA: 0x00057530 File Offset: 0x00055730
	public void UpdateForSale()
	{
		foreach (ResidenceController residenceController in CityData.Instance.residenceDirectory)
		{
			if (residenceController.address.inhabitants.Count <= 0 && Player.Instance.residence != residenceController && residenceController.preset != null && residenceController.preset.enableForSale)
			{
				if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
				{
					ChapterIntro chapterIntro = ChapterController.Instance.chapterScript as ChapterIntro;
					if (chapterIntro != null && residenceController.address == chapterIntro.slophouse)
					{
						if (residenceController.address.saleNote != null)
						{
							residenceController.address.saleNote.Delete();
						}
						GameplayController.Instance.forSale.Remove(residenceController.address);
						continue;
					}
				}
				if (!GameplayController.Instance.forSale.Contains(residenceController.address))
				{
					GameplayController.Instance.forSale.Add(residenceController.address);
				}
			}
			else if (GameplayController.Instance.forSale.Contains(residenceController.address))
			{
				if (residenceController.address.saleNote != null)
				{
					residenceController.address.saleNote.Delete();
				}
				GameplayController.Instance.forSale.Remove(residenceController.address);
			}
		}
		foreach (NewAddress newAddress in GameplayController.Instance.forSale)
		{
			if (newAddress.saleNote == null)
			{
				FurnitureLocation furnitureLocation = null;
				if (ChapterController.Instance != null && ChapterController.Instance.chapterScript != null)
				{
					ChapterIntro chapterIntro2 = ChapterController.Instance.chapterScript as ChapterIntro;
					if (chapterIntro2 != null && newAddress == chapterIntro2.slophouse)
					{
						if (newAddress.saleNote != null)
						{
							newAddress.saleNote.Delete();
							continue;
						}
						continue;
					}
				}
				if (CityData.Instance.jobBoardsDirectory.Count > 0)
				{
					furnitureLocation = CityData.Instance.jobBoardsDirectory[Toolbox.Instance.Rand(0, CityData.Instance.jobBoardsDirectory.Count, false)];
				}
				List<FurniturePreset.SubObject> list = new List<FurniturePreset.SubObject>();
				for (int i = 0; i < furnitureLocation.furniture.subObjects.Count; i++)
				{
					FurniturePreset.SubObject so = furnitureLocation.furniture.subObjects[i];
					if (InteriorControls.Instance.saleNote.subObjectClasses.Contains(so.preset) && !furnitureLocation.spawnedInteractables.Exists((Interactable item) => item.subObject == so))
					{
						list.Add(so);
					}
				}
				if (list.Count > 0)
				{
					FurniturePreset.SubObject subObject = list[Toolbox.Instance.Rand(0, list.Count, false)];
					List<Interactable.Passed> list2 = new List<Interactable.Passed>();
					list2.Add(new Interactable.Passed(Interactable.PassedVarType.addressID, (float)newAddress.id, null));
					Game.Log("Decor: Placing sale note for " + newAddress.name + " at " + furnitureLocation.anchorNode.gameLocation.name, 2);
					newAddress.saleNote = InteractableCreator.Instance.CreateFurnitureSpawnedInteractable(InteriorControls.Instance.saleNote, furnitureLocation, subObject, null, null, null, list2, null, null, "");
				}
				else
				{
					Game.Log("Decor: Unable to place sale note for " + newAddress.name, 2);
				}
			}
		}
	}

	// Token: 0x04000505 RID: 1285
	public List<NewAIController> veryLowTickRate = new List<NewAIController>();

	// Token: 0x04000506 RID: 1286
	public List<NewAIController> lowTickRate = new List<NewAIController>();

	// Token: 0x04000507 RID: 1287
	public List<NewAIController> mediumTickRate = new List<NewAIController>();

	// Token: 0x04000508 RID: 1288
	public List<NewAIController> highTickRate = new List<NewAIController>();

	// Token: 0x04000509 RID: 1289
	public List<NewAIController> veryHighTickRate = new List<NewAIController>();

	// Token: 0x0400050A RID: 1290
	[NonSerialized]
	public List<NewAIController> updateList = new List<NewAIController>();

	// Token: 0x0400050B RID: 1291
	public int tickCounter = 1;

	// Token: 0x0400050C RID: 1292
	public int AITicksPerFrame = 20;

	// Token: 0x0400050D RID: 1293
	public int executionsThisFrame;

	// Token: 0x0400050E RID: 1294
	public int aiTickBacklog;

	// Token: 0x0400050F RID: 1295
	public int visibleHumans;

	// Token: 0x04000510 RID: 1296
	public int frequentTickCounter = 1;

	// Token: 0x04000511 RID: 1297
	private List<LightController> lightUpdateQueue = new List<LightController>();

	// Token: 0x04000512 RID: 1298
	public HashSet<Actor> actorsInStealthMode = new HashSet<Actor>();

	// Token: 0x04000513 RID: 1299
	private float passiveIncomeTimer;

	// Token: 0x04000514 RID: 1300
	public float currentCursorGameTime;

	// Token: 0x04000515 RID: 1301
	public int maxRoutineInitsPerFrame = 30;

	// Token: 0x04000516 RID: 1302
	public int lastUpdateInitCount;

	// Token: 0x04000517 RID: 1303
	public bool initialPositioning;

	// Token: 0x04000518 RID: 1304
	public float triggerHeadache;

	// Token: 0x04000519 RID: 1305
	public List<NewBuilding> buildingEmissionTexturesToUpdate = new List<NewBuilding>();

	// Token: 0x0400051A RID: 1306
	private List<ResidenceController> residenceLightPool = new List<ResidenceController>();

	// Token: 0x0400051B RID: 1307
	private List<NewBuilding> buildingPool = new List<NewBuilding>();

	// Token: 0x0400051C RID: 1308
	private List<Interactable> streetLightPool = new List<Interactable>();

	// Token: 0x0400051D RID: 1309
	public float timeOnLastGameWorldUpdate;

	// Token: 0x0400051E RID: 1310
	[Header("Citizen Rendering")]
	public int loadCitizensPerFrame = 1;

	// Token: 0x0400051F RID: 1311
	public HashSet<Human> citizensRenderQueue = new HashSet<Human>();

	// Token: 0x04000520 RID: 1312
	[Header("Smokestacks")]
	public List<CitizenBehaviour.Smokestack> smokestacks = new List<CitizenBehaviour.Smokestack>();

	// Token: 0x04000521 RID: 1313
	[Header("Scene Captures")]
	public List<SceneRecorder> sceneRecorders = new List<SceneRecorder>();

	// Token: 0x04000522 RID: 1314
	public List<NewGameLocation> tempEscalationBoost = new List<NewGameLocation>();

	// Token: 0x04000524 RID: 1316
	private static CitizenBehaviour _instance;

	// Token: 0x04000525 RID: 1317
	private List<Interactable> toRemove = new List<Interactable>();

	// Token: 0x020000AE RID: 174
	[Serializable]
	public class Smokestack
	{
		// Token: 0x04000526 RID: 1318
		public NewBuilding building;

		// Token: 0x04000527 RID: 1319
		public float timer;
	}

	// Token: 0x020000AF RID: 175
	// (Invoke) Token: 0x0600055A RID: 1370
	public delegate void GameWorldLoop();
}
