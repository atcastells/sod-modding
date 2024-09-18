using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000084 RID: 132
[Serializable]
public class NewAIAction
{
	// Token: 0x0600040F RID: 1039 RVA: 0x0002ECC4 File Offset: 0x0002CEC4
	public NewAIAction(NewAIGoal newGoal, AIActionPreset newPreset, bool newInsertedAction = false, NewRoom newPassedRoom = null, Interactable newPassedInteractable = null, NewNode newForcedNode = null, GroupsController.SocialGroup newPassedGroup = null, List<InteractablePreset> newPassedAcquireItems = null, bool newForceRun = false, int newInsertedActionPriority = 3, string newDebug = "")
	{
		this.goal = newGoal;
		this.preset = newPreset;
		this.name = this.preset.name;
		this.passedRoom = newPassedRoom;
		if (newPassedInteractable != null)
		{
			this.passedInteractable = newPassedInteractable;
		}
		else
		{
			this.passedInteractable = null;
		}
		this.forcedNode = newForcedNode;
		this.passedGroup = newPassedGroup;
		this.passedAcquireItems = newPassedAcquireItems;
		this.debug = newDebug;
		this.repeat = this.preset.repeatOnComplete;
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
			{
				"Create new action: ",
				this.preset.name,
				" for goal ",
				this.goal.name,
				" actor position: ",
				this.goal.aiController.transform.position.ToString()
			}), Actor.HumanDebug.actions);
		}
		this.createdAt = SessionData.Instance.gameTime;
		this.forceRun = newForceRun;
		this.insertedAction = newInsertedAction;
		this.insertedActionPriority = newInsertedActionPriority;
		if (!this.insertedAction)
		{
			this.goal.actions.Add(this);
		}
		else
		{
			bool flag = false;
			for (int i = 0; i < this.goal.actions.Count; i++)
			{
				NewAIAction newAIAction = this.goal.actions[i];
				if (!newAIAction.insertedAction)
				{
					this.goal.actions.Insert(i, this);
					flag = true;
					break;
				}
				if (this.forcedNode != null && this.forcedNode.room == this.goal.aiController.human.currentRoom)
				{
					this.goal.actions.Insert(i, this);
					flag = true;
					break;
				}
				if (this.passedRoom != null && this.passedRoom == this.goal.aiController.human.currentRoom)
				{
					this.goal.actions.Insert(i, this);
					flag = true;
					break;
				}
				if (this.insertedActionPriority >= newAIAction.insertedActionPriority)
				{
					this.goal.actions.Insert(i, this);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				this.goal.actions.Add(this);
			}
		}
		float num = Toolbox.Instance.Rand(this.preset.minutesTakenRange.x, this.preset.minutesTakenRange.y, false);
		if (num > 0f)
		{
			this.timeThisWillTake = num / 60f;
		}
		else
		{
			this.timeThisWillTake = 0f;
		}
		if (this.passedInteractable != null)
		{
			this.debugPassedInteractable = this.passedInteractable.controller;
		}
		this.debugPassedRoom = this.passedRoom;
		this.debugForcedNode = false;
		this.debugForcedNodeWorldPos = Vector3.zero;
		if (this.forcedNode != null)
		{
			this.debugForcedNode = true;
			this.debugForcedNodeWorldPos = this.forcedNode.position;
		}
	}

	// Token: 0x06000410 RID: 1040 RVA: 0x0002F008 File Offset: 0x0002D208
	public void OnActivate()
	{
		if (!this.isActive)
		{
			if (this.preset.exitConversationOnActivate && this.goal.aiController.human.inConversation && this.goal.aiController.human.currentConversation != null)
			{
				this.goal.aiController.human.currentConversation.EndConversation();
			}
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.DebugDestinationPosition("Activate action: " + this.preset.name);
			}
			this.completed = false;
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.DebugDestinationPosition("New action is activated: " + this.preset.name);
			}
			if (this.goal.aiController.human.animationController.armsBoolAnimationState != CitizenAnimationController.ArmsBoolSate.armsOneShotUse && this.goal.aiController.human.animationController.armsBoolAnimationState != CitizenAnimationController.ArmsBoolSate.none && !this.goal.aiController.restrained)
			{
				this.goal.aiController.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
			}
			if (this.goal.aiController.human.animationController.idleAnimationState != CitizenAnimationController.IdleAnimationState.none && !this.goal.aiController.restrained)
			{
				this.goal.aiController.human.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
			}
			if (!this.preset.sleepOnArrival && this.goal.aiController.human.isAsleep)
			{
				this.goal.aiController.human.WakeUp(false);
				return;
			}
			if (!this.preset.lying && this.goal.aiController.human.isInBed)
			{
				this.goal.aiController.human.SetInBed(false);
			}
			if (this.preset.activationRequiresConsumable && (this.goal.aiController.human.currentConsumables == null || this.goal.aiController.human.currentConsumables.Count <= 0))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Removing action as human has no consumables...", Actor.HumanDebug.actions);
				}
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			this.goal.aiController.currentAction = this;
			this.goal.aiController.pathCursor = -1;
			this.isActive = true;
			this.goal.aiController.UpdateHeldItems(AIActionPreset.ActionStateFlag.onActivation);
			if (this.passedInteractable != null)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"Activated action: ",
						this.preset.name,
						" actor position: ",
						this.goal.aiController.transform.position.ToString(),
						" passed interactable: ",
						this.passedInteractable.GetName()
					}), Actor.HumanDebug.actions);
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
				{
					"Activated action: ",
					this.preset.name,
					" actor position: ",
					this.goal.aiController.transform.position.ToString(),
					" passed interactable: null"
				}), Actor.HumanDebug.actions);
			}
			if (this.preset.actionLocation == AIActionPreset.ActionLocation.pause)
			{
				this.node = this.goal.aiController.human.currentNode;
				this.goal.aiController.AddDebugAction("New Action is pause at (" + this.node.name + ")");
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.investigate)
			{
				this.node = this.goal.aiController.investigateLocation;
				if (this.node != null)
				{
					this.goal.aiController.AddDebugAction("New Action is investigate location (" + this.node.name + ")");
				}
				this.goal.searchProgress = 0;
				this.goal.searchedNodes.Clear();
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.putDownInteractable)
			{
				FurnitureLocation furnitureLocation = null;
				if (this.goal.aiController.human.currentGameLocation != null && this.goal.aiController.human.currentNode != null && this.passedInteractable != null)
				{
					this.bestPlacement = this.goal.aiController.human.currentGameLocation.GetBestSpawnLocation(this.passedInteractable.preset, false, this.goal.aiController.human, null, null, out furnitureLocation, null, true, 0, InteractablePreset.OwnedPlacementRule.both, 0, null, false, null, null, this.goal.aiController.human.currentNode, "", false, true);
				}
				else if (this.passedInteractable == null)
				{
					Game.Log("AI Error: " + this.goal.aiController.human.name + " Cannot execute 'put down interactable' action without a passed interactable...", 2);
				}
				if (this.bestPlacement != null)
				{
					this.node = this.bestPlacement.furnParent.anchorNode;
				}
				else if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					string text = string.Empty;
					foreach (SubObjectClassPreset subObjectClassPreset in this.passedInteractable.preset.putDownPositions)
					{
						text = text + subObjectClassPreset.name + ", ";
					}
					text += " - backup: ";
					foreach (SubObjectClassPreset subObjectClassPreset2 in this.passedInteractable.preset.backupPutDownPositions)
					{
						text = text + subObjectClassPreset2.name + ", ";
					}
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							"Unable to find put down interactable (",
							this.passedInteractable.preset.name,
							") furniture location in ",
							this.goal.aiController.human.currentGameLocation.name,
							": ",
							text
						}), Actor.HumanDebug.actions);
					}
					Game.Log(string.Concat(new string[]
					{
						"AI Error: Unable to find put down interactable (",
						this.passedInteractable.preset.name,
						") furniture location in ",
						this.goal.aiController.human.currentGameLocation.name,
						": ",
						text
					}), 2);
				}
				this.interactable = null;
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.pickUpInteractable)
			{
				this.node = this.passedInteractable.node;
				if (Game.Instance.devMode && Game.Instance.collectDebugData)
				{
					this.debugPickupInteractable.Add(this.passedInteractable);
				}
				this.interactable = null;
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.randomNodeWithinLocation || this.preset.actionLocation == AIActionPreset.ActionLocation.randomNodeWithinLocationPrioritiseWindows)
			{
				NewGameLocation newGameLocation = this.goal.aiController.CheckConfinedLocation(this.goal.gameLocation);
				if (newGameLocation != null)
				{
					bool prioritiseWindows = false;
					if (this.preset.actionLocation == AIActionPreset.ActionLocation.randomNodeWithinLocationPrioritiseWindows)
					{
						prioritiseWindows = true;
					}
					this.node = this.goal.aiController.human.FindSafeTeleport(newGameLocation, prioritiseWindows, true);
				}
				if (this.node != null)
				{
					this.goal.aiController.AddDebugAction("New Action is random node within location (" + this.node.name + ")");
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.proximityToMusic)
			{
				List<NewNode> list = new List<NewNode>();
				foreach (NewRoom newRoom in this.goal.gameLocation.rooms)
				{
					int i;
					string text2;
					if (!(newRoom.roomType == InteriorControls.Instance.nullRoomType) && newRoom.entrances.Count > 0 && newRoom.nodes.Count > 1 && newRoom.musicPlaying && (this.preset.bannedRooms != AIActionPreset.SourceOfBannedRooms.jobPreset || this.goal.aiController.human.job == null || !(this.goal.aiController.human.job.preset != null) || !this.goal.aiController.human.job.preset.bannedRooms.Contains(newRoom.preset)) && (this.goal.preset.allowTrespass || !this.goal.aiController.human.IsTrespassing(newRoom, out i, out text2, this.goal.preset.allowEnforcersEverywhere)))
					{
						foreach (NewNode newNode in newRoom.nodes)
						{
							if ((!(newNode.room.gameLocation.thisAsStreet == null) || newNode.floorType == NewNode.FloorTileType.floorAndCeiling || newNode.floorType == NewNode.FloorTileType.floorOnly) && !newNode.isIndoorsEntrance && !newNode.noAccess && !newNode.noPassThrough)
							{
								list.Add(newNode);
							}
						}
					}
				}
				if (list.Count <= 0)
				{
					using (List<NewNode.NodeAccess>.Enumerator enumerator4 = this.goal.gameLocation.entrances.GetEnumerator())
					{
						while (enumerator4.MoveNext())
						{
							NewNode.NodeAccess nodeAccess = enumerator4.Current;
							if (!nodeAccess.fromNode.room.isNullRoom)
							{
								this.node = nodeAccess.fromNode;
								break;
							}
						}
						goto IL_2C6F;
					}
				}
				this.node = list[Toolbox.Instance.Rand(0, list.Count, false)];
				this.goal.aiController.AddDebugAction("New Action is random node within location (" + this.node.name + ")");
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.randomNodeWithinHome && this.goal.aiController.human.home != null)
			{
				this.node = this.goal.aiController.human.FindSafeTeleport(this.goal.aiController.human.home, false, true);
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.randomNodeWithinDen)
			{
				if (this.goal.aiController.human.den != null)
				{
					this.node = this.goal.aiController.human.FindSafeTeleport(this.goal.aiController.human.den, false, true);
				}
				else
				{
					Game.Log("AIError: " + this.goal.aiController.human.GetCitizenName() + " does not have a den. Using home instead...", 2);
				}
				if (this.node == null)
				{
					this.node = this.goal.aiController.human.FindSafeTeleport(this.goal.aiController.human.home, false, true);
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.tailAndConfrontPlayer)
			{
				if (this.goal.aiController.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor != null && item.actor.isPlayer))
				{
					this.interactable = Player.Instance.interactable;
					this.node = Player.Instance.currentNode;
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Action location update with player node", Actor.HumanDebug.actions);
					}
				}
				else
				{
					this.interactable = null;
					this.node = this.goal.aiController.human.FindSafeTeleport(Player.Instance.currentGameLocation, false, true);
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.victimApartmentDoor)
			{
				if (this.goal.murderRef != null)
				{
					if (this.goal.murderRef.victim != null && this.goal.murderRef.victim.home != null)
					{
						NewNode.NodeAccess nodeAccess2 = this.goal.murderRef.victim.home.entrances.Find((NewNode.NodeAccess item) => item.accessType == NewNode.NodeAccess.AccessType.door);
						NewNode fromNode = nodeAccess2.fromNode;
						NewNode newNode2 = nodeAccess2.toNode;
						if (nodeAccess2.toNode.gameLocation == this.goal.murderRef.victim.home)
						{
							NewNode toNode = nodeAccess2.toNode;
							newNode2 = nodeAccess2.fromNode;
						}
						this.node = newNode2;
						if (nodeAccess2.door != null)
						{
							this.interactable = nodeAccess2.door.doorInteractable;
						}
					}
					else if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Action " + this.preset.presetName + " requires a murder reference with a victim", Actor.HumanDebug.actions);
					}
				}
				else if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Action " + this.preset.presetName + " requires a goal with a passed murder reference", Actor.HumanDebug.actions);
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.sniperVantagePoint)
			{
				if (this.goal.murderRef != null)
				{
					if (this.goal.murderRef.victim != null)
					{
						float num;
						if (this.goal.murderRef.mo.requiresSniperVantageAtHome && this.goal.aiController.human.home != null)
						{
							List<NewGameLocation> list2;
							if (Toolbox.Instance.TryGetSniperVantagePoint(this.goal.aiController.human.home, out this.vantagePoint, out num, out list2, this.goal.murderRef.sniperVictimSite))
							{
								this.node = this.vantagePoint.node;
								this.interactable = null;
							}
							else if (Game.Instance.collectDebugData)
							{
								Actor human = this.goal.aiController.human;
								string text3 = "Action ";
								string presetName = this.preset.presetName;
								string text4 = " unable to find a valid sniper vantage point: ";
								NewGameLocation sniperVictimSite = this.goal.murderRef.sniperVictimSite;
								human.SelectedDebug(text3 + presetName + text4 + ((sniperVictimSite != null) ? sniperVictimSite.ToString() : null), Actor.HumanDebug.actions);
							}
						}
						else if (Toolbox.Instance.TryGetSniperVantagePoint(this.goal.aiController.human, this.goal.murderRef.sniperVictimSite, out this.vantagePoint, out num, null))
						{
							this.node = this.vantagePoint.node;
							this.interactable = null;
						}
						else if (Game.Instance.collectDebugData)
						{
							Actor human2 = this.goal.aiController.human;
							string text5 = "Action ";
							string presetName2 = this.preset.presetName;
							string text6 = " unable to find a valid sniper vantage point: ";
							NewGameLocation sniperVictimSite2 = this.goal.murderRef.sniperVictimSite;
							human2.SelectedDebug(text5 + presetName2 + text6 + ((sniperVictimSite2 != null) ? sniperVictimSite2.ToString() : null), Actor.HumanDebug.actions);
						}
					}
					else if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Action " + this.preset.presetName + " requires a murder reference with a victim", Actor.HumanDebug.actions);
					}
				}
				else if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Action " + this.preset.presetName + " requires a goal with a passed murder reference", Actor.HumanDebug.actions);
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.NearbyStreetRandomNode)
			{
				StreetController streetController = CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)];
				float num2 = float.PositiveInfinity;
				foreach (StreetController streetController2 in CityData.Instance.streetDirectory)
				{
					if (streetController2 != this.goal.aiController.human.currentGameLocation && streetController2.rooms.Count > 0)
					{
						float num3 = Vector3.Distance(this.goal.aiController.human.currentNodeCoord, Enumerable.FirstOrDefault<NewNode>(streetController2.rooms[0].nodes).nodeCoord);
						if (num3 < num2)
						{
							streetController = streetController2;
							num2 = num3;
						}
					}
				}
				List<NewNode> list3 = new List<NewNode>();
				foreach (NewRoom newRoom2 in streetController.rooms)
				{
					int i;
					string text2;
					if (!(newRoom2.roomType == InteriorControls.Instance.nullRoomType) && newRoom2.entrances.Count > 0 && newRoom2.nodes.Count > 1 && (this.preset.bannedRooms != AIActionPreset.SourceOfBannedRooms.jobPreset || this.goal.aiController.human.job == null || !(this.goal.aiController.human.job.preset != null) || !this.goal.aiController.human.job.preset.bannedRooms.Contains(newRoom2.preset)) && (this.goal.preset.allowTrespass || !this.goal.aiController.human.IsTrespassing(newRoom2, out i, out text2, this.goal.preset.allowEnforcersEverywhere)))
					{
						foreach (NewNode newNode3 in newRoom2.nodes)
						{
							if (!newNode3.isIndoorsEntrance && !newNode3.noAccess && !newNode3.noPassThrough && (!(newNode3.gameLocation.thisAsStreet == null) || newNode3.floorType == NewNode.FloorTileType.floorOnly || newNode3.floorType == NewNode.FloorTileType.floorAndCeiling))
							{
								list3.Add(newNode3);
							}
						}
					}
				}
				if (list3.Count > 0)
				{
					this.node = list3[Toolbox.Instance.Rand(0, list3.Count, false)];
					this.goal.aiController.AddDebugAction("New Action is random node within location (" + this.node.name + ")");
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.flee)
			{
				List<NewNode> list4 = new List<NewNode>();
				Actor actor = Player.Instance;
				if (this.goal.aiController.human.lastScaredBy != null)
				{
					actor = this.goal.aiController.human.lastScaredBy;
				}
				bool flag = false;
				NewGameLocation newGameLocation2 = null;
				if (this.goal.aiController.confineLocation != null)
				{
					flag = true;
					newGameLocation2 = this.goal.aiController.confineLocation;
				}
				if (this.goal.aiController.victimsForMurders.Count > 0)
				{
					foreach (MurderController.Murder murder in this.goal.aiController.victimsForMurders)
					{
						if (!(murder.location == null) && murder.state >= MurderController.MurderState.travellingTo && murder.preset.blockVictimFromLeavingLocation)
						{
							flag = true;
							newGameLocation2 = murder.location;
							break;
						}
					}
				}
				if (flag && newGameLocation2 != null)
				{
					using (List<NewRoom>.Enumerator enumerator2 = newGameLocation2.rooms.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NewRoom newRoom3 = enumerator2.Current;
							if (!(newRoom3.roomType == InteriorControls.Instance.nullRoomType) && !(actor.currentRoom == newRoom3))
							{
								foreach (NewNode newNode4 in newRoom3.nodes)
								{
									if ((!(newNode4.gameLocation.thisAsStreet == null) || newNode4.floorType == NewNode.FloorTileType.floorAndCeiling || newNode4.floorType == NewNode.FloorTileType.floorOnly) && !newNode4.isIndoorsEntrance && !newNode4.noAccess && !newNode4.noPassThrough)
									{
										list4.Add(newNode4);
									}
								}
							}
						}
						goto IL_192B;
					}
				}
				if (this.goal.aiController.human.home != actor.currentGameLocation || !actor.isPlayer)
				{
					if (this.goal.aiController.human.home != null && !this.goal.aiController.avoidLocations.Contains(this.goal.aiController.human.home))
					{
						using (List<NewRoom>.Enumerator enumerator2 = this.goal.aiController.human.home.rooms.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								NewRoom newRoom4 = enumerator2.Current;
								if (!(newRoom4.roomType == InteriorControls.Instance.nullRoomType) && !(actor.currentRoom == newRoom4))
								{
									foreach (NewNode newNode5 in newRoom4.nodes)
									{
										if ((!(newNode5.gameLocation.thisAsStreet == null) || newNode5.floorType == NewNode.FloorTileType.floorAndCeiling || newNode5.floorType == NewNode.FloorTileType.floorOnly) && !newNode5.isIndoorsEntrance && !newNode5.noAccess && !newNode5.noPassThrough)
										{
											list4.Add(newNode5);
										}
									}
								}
							}
							goto IL_192B;
						}
					}
					using (List<NewRoom>.Enumerator enumerator2 = CityData.Instance.streetDirectory[Toolbox.Instance.Rand(0, CityData.Instance.streetDirectory.Count, false)].rooms.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							NewRoom newRoom5 = enumerator2.Current;
							if (!(actor.currentRoom == newRoom5))
							{
								foreach (NewNode newNode6 in newRoom5.nodes)
								{
									if ((!(newNode6.gameLocation.thisAsStreet == null) || newNode6.floorType == NewNode.FloorTileType.floorAndCeiling || newNode6.floorType == NewNode.FloorTileType.floorOnly) && !newNode6.isIndoorsEntrance && !newNode6.noAccess && !newNode6.noPassThrough)
									{
										list4.Add(newNode6);
									}
								}
							}
						}
						goto IL_192B;
					}
				}
				foreach (NewNode newNode7 in Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.answerTelephone, this.goal.aiController.human.currentRoom, this.goal.aiController.human, AIActionPreset.FindSetting.onlyPublic, true, null, null, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, false).node.room.nodes)
				{
					if ((!(newNode7.gameLocation.thisAsStreet == null) || newNode7.floorType == NewNode.FloorTileType.floorAndCeiling || newNode7.floorType == NewNode.FloorTileType.floorOnly) && !newNode7.isIndoorsEntrance && !newNode7.noAccess)
					{
						list4.Add(newNode7);
					}
				}
				IL_192B:
				if (list4.Count > 0)
				{
					this.node = list4[Toolbox.Instance.Rand(0, list4.Count, false)];
				}
			}
			else if (this.preset.actionLocation == AIActionPreset.ActionLocation.nearbyInvestigate)
			{
				List<NewNode> list5 = new List<NewNode>();
				List<NewNode> list6 = new List<NewNode>();
				list6.Add(this.goal.aiController.investigateLocation);
				int num4 = 50;
				this.goal.searchProgress++;
				while (list6.Count > 0 && num4 > 0)
				{
					NewNode newNode8 = list6[0];
					list5.Add(newNode8);
					try
					{
						if (newNode8.room != this.goal.aiController.investigateLocation.room)
						{
							list5.Add(newNode8);
						}
						foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair in newNode8.accessToOtherNodes)
						{
							if (!(keyValuePair.Key.room.roomType == InteriorControls.Instance.nullRoomType) && keyValuePair.Value.walkingAccess && !keyValuePair.Key.isIndoorsEntrance && keyValuePair.Key.room.nodes.Count > 1 && (!(keyValuePair.Key.gameLocation.thisAsStreet == null) || keyValuePair.Key.floorType == NewNode.FloorTileType.floorAndCeiling || keyValuePair.Key.floorType == NewNode.FloorTileType.floorOnly))
							{
								if (this.goal.aiController.human.escalationLevel <= 0)
								{
									if (keyValuePair.Key.room != newNode8.room)
									{
										continue;
									}
								}
								else if (this.goal.aiController.human.escalationLevel <= 1 && keyValuePair.Key.gameLocation != newNode8.gameLocation)
								{
									continue;
								}
								int i;
								string text2;
								if ((this.goal.preset.allowTrespass || !this.goal.aiController.human.IsTrespassing(keyValuePair.Key.room, out i, out text2, this.goal.preset.allowEnforcersEverywhere)) && !list5.Contains(keyValuePair.Key) && !list6.Contains(keyValuePair.Key) && !this.goal.searchedNodes.Contains(keyValuePair.Key) && Vector3.Distance(this.goal.aiController.investigateLocation.nodeCoord, keyValuePair.Key.nodeCoord) <= 3f + (float)this.goal.searchProgress)
								{
									list6.Add(keyValuePair.Key);
								}
							}
						}
					}
					catch
					{
					}
					list6.RemoveAt(0);
					num4--;
				}
				this.node = list5[Toolbox.Instance.Rand(0, list5.Count, false)];
				if (this.node != null)
				{
					this.goal.searchedNodes.Add(this.node);
					if (Game.Instance.collectDebugData)
					{
						Actor human3 = this.goal.aiController.human;
						string text7 = "New Action is random nearby investigate location ";
						Vector3 position = this.node.position;
						human3.SelectedDebug(text7 + position.ToString(), Actor.HumanDebug.actions);
					}
					if (Game.Instance.collectDebugData)
					{
						NewAIController aiController = this.goal.aiController;
						string text8 = "New Action is random nearby investigate location ";
						Vector3 position = this.node.position;
						aiController.AddDebugAction(text8 + position.ToString());
					}
				}
			}
			else
			{
				if (this.preset.actionLocation == AIActionPreset.ActionLocation.interactable)
				{
					if (Game.Instance.collectDebugData)
					{
						if (this.passedInteractable != null)
						{
							this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
							{
								"Received passed interactable: ",
								this.passedInteractable.name,
								" (",
								this.passedInteractable.id.ToString(),
								")"
							}), Actor.HumanDebug.actions);
						}
						else
						{
							this.goal.aiController.human.SelectedDebug("Haven't received a passed interactable", Actor.HumanDebug.actions);
						}
					}
					if (this.passedInteractable != null && this.passedInteractable.isActor != null)
					{
						this.interactable = this.passedInteractable;
						this.node = this.passedInteractable.isActor.currentNode;
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug("Received passed interactable: " + this.passedInteractable.name, Actor.HumanDebug.actions);
						}
					}
					else if (this.passedInteractable != null && (!this.preset.confirmActionLocation || (this.passedInteractable.originalPosition && this.passedInteractable.node != null && this.passedInteractable.node.room.actionReference.ContainsKey(this.preset) && this.passedInteractable.node.room.actionReference[this.preset].Contains(this.passedInteractable))))
					{
						this.interactable = this.passedInteractable;
						this.node = this.passedInteractable.node;
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug("New Action in passed interactable (" + this.passedInteractable.name + ")", Actor.HumanDebug.actions);
						}
						if (this.node != null && this.node.accessToOtherNodes.Count <= 0)
						{
							Game.Log("AI Error: Action picked node with no access points.", 2);
						}
					}
					else if (this.passedRoom != null)
					{
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug("Received passed room (" + this.passedRoom.GetName() + "), searching for interactables...", Actor.HumanDebug.actions);
						}
						if (!this.preset.confirmActionLocation || this.passedRoom.actionReference.ContainsKey(this.preset))
						{
							List<Interactable> list7 = new List<Interactable>();
							if (this.passedRoom.actionReference.ContainsKey(this.preset))
							{
								list7 = this.passedRoom.actionReference[this.preset];
							}
							Interactable.UsagePoint usagePoint = null;
							this.interactable = this.InteractablePicker(ref list7, this.goal.aiController.human.transform.position, this.preset.socialRules, out this.node, out usagePoint, this.passedGroup, false, true, null);
							if (usagePoint != null)
							{
								this.SetUsagePoint(usagePoint, this.preset.usageSlot);
							}
							if (Game.Instance.collectDebugData)
							{
								Actor human4 = this.goal.aiController.human;
								string text9 = "New Action in passed room (";
								string text10 = this.passedRoom.GetName();
								string text11 = ") : ";
								Interactable interactable = this.interactable;
								human4.SelectedDebug(text9 + text10 + text11 + ((interactable != null) ? interactable.ToString() : null), Actor.HumanDebug.actions);
							}
						}
						else if (this.preset.confirmActionLocation && Game.Instance.collectDebugData)
						{
							Actor human5 = this.goal.aiController.human;
							string[] array = new string[9];
							array[0] = "Room ";
							array[1] = this.passedRoom.GetName();
							array[2] = " ";
							array[3] = this.passedRoom.roomID.ToString();
							array[4] = " does not contain action reference ";
							array[5] = this.preset.name;
							array[6] = " (";
							int num5 = 7;
							int i = this.passedRoom.actionReference.Count;
							array[num5] = i.ToString();
							array[8] = ") This will default to the nearest...";
							human5.SelectedDebug(string.Concat(array), Actor.HumanDebug.actions);
						}
						if (this.node != null && this.node.accessToOtherNodes.Count <= 0)
						{
							Game.Log("AI Error: Action picked node with no access points.", 2);
						}
					}
					else if (this.goal.roomLocation != null && this.goal.roomLocation.actionReference.ContainsKey(this.preset))
					{
						List<Interactable> list8 = this.goal.roomLocation.actionReference[this.preset];
						Interactable.UsagePoint usagePoint2 = null;
						this.interactable = this.InteractablePicker(ref list8, this.goal.aiController.human.transform.position, this.preset.socialRules, out this.node, out usagePoint2, this.passedGroup, false, true, null);
						if (usagePoint2 != null)
						{
							this.SetUsagePoint(usagePoint2, this.preset.usageSlot);
						}
						if (Game.Instance.collectDebugData)
						{
							Actor human6 = this.goal.aiController.human;
							string text12 = "New Action in selected room (";
							string text13 = this.goal.roomLocation.name;
							string text14 = ") : ";
							Interactable interactable2 = this.interactable;
							human6.SelectedDebug(text12 + text13 + text14 + ((interactable2 != null) ? interactable2.ToString() : null), Actor.HumanDebug.actions);
						}
						if (this.node != null && this.node.accessToOtherNodes.Count <= 0)
						{
							Game.Log("AI Error: Action picked node with no access points.", 2);
						}
					}
					else if (this.goal.gameLocation.actionReference.ContainsKey(this.preset))
					{
						List<Interactable> list9 = this.goal.gameLocation.actionReference[this.preset];
						Interactable.UsagePoint usagePoint3 = null;
						this.interactable = this.InteractablePicker(ref list9, this.goal.aiController.human.transform.position, this.preset.socialRules, out this.node, out usagePoint3, this.passedGroup, false, true, null);
						if (usagePoint3 != null)
						{
							this.SetUsagePoint(usagePoint3, this.preset.usageSlot);
						}
						if (Game.Instance.collectDebugData)
						{
							Actor human7 = this.goal.aiController.human;
							string text15 = "New Action in selected gamelocation (";
							string text16 = this.goal.gameLocation.name;
							string text17 = ") : ";
							Interactable interactable3 = this.interactable;
							human7.SelectedDebug(text15 + text16 + text17 + ((interactable3 != null) ? interactable3.ToString() : null), Actor.HumanDebug.actions);
						}
						if (this.node != null && this.node.accessToOtherNodes.Count <= 0)
						{
							Game.Log("AI Error: Action picked node with no access points.", 2);
						}
					}
				}
				else if (this.preset.actionLocation == AIActionPreset.ActionLocation.interactableSpawn)
				{
					if (this.passedInteractable != null)
					{
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
							{
								"Received passed interactable: ",
								this.passedInteractable.name,
								" (",
								this.passedInteractable.id.ToString(),
								")"
							}), Actor.HumanDebug.actions);
						}
						this.interactable = this.passedInteractable;
						this.node = this.interactable.spawnNode;
					}
				}
				else if (this.preset.actionLocation == AIActionPreset.ActionLocation.player)
				{
					this.interactable = Player.Instance.interactable;
					this.node = Player.Instance.currentNode;
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Action location update with player node", Actor.HumanDebug.actions);
					}
				}
				if (this.preset.actionLocation == AIActionPreset.ActionLocation.interactableLOS)
				{
					if (this.passedInteractable != null)
					{
						List<NewNode> list10 = new List<NewNode>();
						NewNode currentNode = this.passedInteractable.node;
						if (this.passedInteractable.isActor != null)
						{
							currentNode = this.passedInteractable.isActor.currentNode;
						}
						list10.Add(currentNode);
						List<NewNode> list11 = new List<NewNode>();
						HashSet<NewNode> hashSet = new HashSet<NewNode>();
						int num6 = 50;
						while (list11.Count > 0 && num6 > 0)
						{
							NewNode newNode9 = list11[0];
							list10.Add(newNode9);
							foreach (Vector2Int vector2Int in CityData.Instance.offsetArrayX4)
							{
								Vector3 vector = newNode9.nodeCoord + new Vector3Int(vector2Int.x, vector2Int.y, 0);
								NewNode newNode10 = null;
								if (PathFinder.Instance.nodeMap.TryGetValue(vector, ref newNode10) && !newNode10.noAccess && !newNode10.noPassThrough && !list11.Contains(newNode10) && !hashSet.Contains(newNode10))
								{
									Vector3 position2 = newNode10.position;
									position2.y = this.goal.aiController.human.lookAtThisTransform.position.y;
									RaycastHit raycastHit;
									if (Toolbox.Instance.RaycastCheck(position2, this.passedInteractable.controller.transform, 10f, out raycastHit))
									{
										list11.Add(newNode10);
									}
								}
							}
							hashSet.Add(newNode9);
							list11.RemoveAt(0);
							num6--;
						}
						if (list10.Count > 1)
						{
							list10.Remove(currentNode);
						}
						this.node = list10[0];
						float num7 = float.PositiveInfinity;
						using (List<NewNode>.Enumerator enumerator8 = list10.GetEnumerator())
						{
							while (enumerator8.MoveNext())
							{
								NewNode newNode11 = enumerator8.Current;
								float num8 = Vector3.Distance(newNode11.nodeCoord, this.goal.aiController.human.currentNodeCoord);
								if (num8 < num7)
								{
									this.node = newNode11;
									num7 = num8;
								}
							}
							goto IL_2700;
						}
					}
					Game.Log("AI Error: No passed interactable for interactable LOS", 2);
				}
				IL_2700:
				if (this.preset.actionLocation == AIActionPreset.ActionLocation.findNearest || (this.node == null && this.forcedNode == null && this.preset.onUnableToFindLocation == AIActionPreset.ActionFinding.findNearest))
				{
					if (this.preset.actionLocation != AIActionPreset.ActionLocation.findNearest && Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							this.goal.aiController.human.name,
							" unable to locate node for ",
							this.preset.name,
							"(",
							this.goal.gameLocation.name,
							"), finding nearest (expensive)..."
						}), Actor.HumanDebug.actions);
					}
					HashSet<NewRoom> hashSet2 = new HashSet<NewRoom>();
					if (this.goal.aiController.human.job != null && this.goal.aiController.human.job.preset != null && this.goal.gameLocation != null)
					{
						foreach (NewRoom newRoom6 in this.goal.gameLocation.rooms)
						{
							if (this.goal.aiController.human.job.preset.bannedRooms.Contains(newRoom6.preset))
							{
								hashSet2.Add(newRoom6);
							}
						}
					}
					NewGameLocation newGameLocation3 = null;
					if (this.preset.limitSearchToGoalLocation || (this.goal != null && this.goal.preset.passedGamelocationIsImportant))
					{
						newGameLocation3 = this.goal.gameLocation;
						if (newGameLocation3 != null && Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug("Limit to action seach location: " + newGameLocation3.name, Actor.HumanDebug.actions);
						}
					}
					List<Interactable> avoidInteractables = null;
					if (this.preset.avoidRepeatingInteractables)
					{
						avoidInteractables = this.goal.chosenInteractablesThisGoal;
					}
					bool mustContainDesireCategory = false;
					if (this.goal.preset.locationOption == AIGoalPreset.LocationOption.commercialDecision)
					{
						mustContainDesireCategory = true;
					}
					Toolbox instance = Toolbox.Instance;
					AIActionPreset action = this.preset;
					NewRoom currentRoom = this.goal.aiController.human.currentRoom;
					Human human8 = this.goal.aiController.human;
					AIActionPreset.FindSetting searchSetting = this.preset.searchSetting;
					bool findOverrideWithHome = this.preset.findOverrideWithHome;
					HashSet<NewRoom> ignore = hashSet2;
					bool filterSearchUsingRoomType = this.preset.filterSearchUsingRoomType;
					List<RoomTypePreset> searchRoomType = this.preset.searchRoomType;
					this.interactable = instance.FindNearestWithAction(action, currentRoom, human8, searchSetting, findOverrideWithHome, ignore, newGameLocation3, null, false, InteractablePreset.SpecialCase.none, filterSearchUsingRoomType, searchRoomType, true, this.goal.preset.allowEnforcersEverywhere, this.preset.robberyPriorityMultiplier, avoidInteractables, this.passedAcquireItems, false, mustContainDesireCategory, this.goal.preset.desireCategory, false);
					if (this.interactable != null)
					{
						if (!this.goal.chosenInteractablesThisGoal.Contains(this.interactable))
						{
							this.goal.chosenInteractablesThisGoal.Add(this.interactable);
						}
						this.node = this.interactable.node;
					}
					else if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Unable to find nearest interactable with action " + this.preset.name, Actor.HumanDebug.actions);
					}
					this.goal.aiController.AddDebugAction("New Action, find nearest " + this.preset.name);
				}
				if (this.node == null && this.forcedNode == null && this.preset.onUnableToFindLocation == AIActionPreset.ActionFinding.removeAction)
				{
					Game.Log(string.Concat(new string[]
					{
						"AI Error: ",
						this.goal.aiController.human.name,
						" unable to locate node for ",
						this.preset.name,
						", removing as per action setting..."
					}), 2);
					this.goal.aiController.AddDebugAction(this.goal.aiController.human.name + " unable to locate node for " + this.preset.name + ", removing as per action setting...");
					this.Remove(this.preset.repeatDelayOnActionFail);
					return;
				}
				if (this.node == null && this.forcedNode == null && this.preset.onUnableToFindLocation == AIActionPreset.ActionFinding.removeGoal)
				{
					try
					{
						Game.Log(string.Concat(new string[]
						{
							"AI Error: ",
							this.goal.aiController.human.name,
							" unable to locate node for ",
							this.preset.name,
							", removing goal ",
							this.goal.preset.name,
							" as per action setting..."
						}), 2);
						this.goal.aiController.AddDebugAction(string.Concat(new string[]
						{
							this.goal.aiController.human.name,
							" unable to locate node for ",
							this.preset.name,
							", removing goal ",
							this.goal.preset.name,
							" as per action setting..."
						}));
					}
					catch
					{
					}
					this.Remove(this.preset.repeatDelayOnActionFail);
					this.goal.Remove();
					return;
				}
			}
			IL_2C6F:
			if (this.interactable != null)
			{
				this.SetUsagePoint(this.interactable.usagePoint, this.preset.usageSlot);
				if (this.usagePoint != null)
				{
					Human human9 = null;
					if (this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human9) && human9 != null && human9 != this.goal.aiController.human)
					{
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug(human9.name + " is already using furniture slot " + this.preset.usageSlot.ToString(), Actor.HumanDebug.actions);
						}
						this.UsingFurnitureCheck();
					}
					if (this.interactable != null && this.usagePoint != null)
					{
						this.node = this.usagePoint.node;
						Vector3 usageWorldPosition = this.usagePoint.GetUsageWorldPosition(this.goal.aiController.human.transform.position, this.goal.aiController.human);
						this.debugInteractionUsagePosition = usageWorldPosition;
						Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(usageWorldPosition);
						NewNode newNode12 = null;
						if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode12))
						{
							this.node = newNode12;
						}
						if (this.interactable.isActor != null)
						{
							this.node = this.interactable.isActor.currentNode;
						}
						if (this.node != null && Game.Instance.collectDebugData)
						{
							Actor human10 = this.goal.aiController.human;
							string text18 = "Automatic selection of interactable's use point uses node: ";
							Vector3Int nodeCoord = this.node.nodeCoord;
							string text19 = nodeCoord.ToString();
							string text20 = " world pos: ";
							Vector3 position = this.node.position;
							human10.SelectedDebug(text18 + text19 + text20 + position.ToString(), Actor.HumanDebug.actions);
						}
					}
				}
				this.debugInteractableController = this.interactable.controller;
			}
			this.debugForcedNode = false;
			this.debugForcedNodeWorldPos = Vector3.zero;
			if (this.forcedNode != null)
			{
				this.node = this.forcedNode;
				this.debugForcedNode = true;
				this.debugForcedNodeWorldPos = this.node.position;
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Node overridden with forced node! This may distrupt any selected use points: " + this.preset.name, Actor.HumanDebug.actions);
				}
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.DebugDestinationPosition("Node overridden with forced node! This may distrupt any selected use points: " + this.preset.name);
				}
			}
			else if (this.node == null)
			{
				string[] array2 = new string[7];
				array2[0] = "AI Error: ";
				array2[1] = this.goal.aiController.human.name;
				array2[2] = "Cannot execute ";
				array2[3] = this.preset.name;
				array2[4] = " at this location: ";
				int num9 = 5;
				NewGameLocation gameLocation = this.goal.gameLocation;
				array2[num9] = ((gameLocation != null) ? gameLocation.ToString() : null);
				array2[6] = " Error in goal choosing location?";
				Game.Log(string.Concat(array2), 2);
				if (Game.Instance.collectDebugData)
				{
					Actor human11 = this.goal.aiController.human;
					string[] array3 = new string[5];
					array3[0] = "Cannot execute ";
					array3[1] = this.preset.name;
					array3[2] = " at this location: ";
					int num10 = 3;
					NewGameLocation gameLocation2 = this.goal.gameLocation;
					array3[num10] = ((gameLocation2 != null) ? gameLocation2.ToString() : null);
					array3[4] = " Error in goal choosing location?";
					human11.SelectedDebug(string.Concat(array3), Actor.HumanDebug.actions);
				}
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			if (this.goal.aiController.confineLocation != null && this.goal.aiController.confineLocation != this.node.gameLocation)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"AI is confined to location ",
						this.goal.aiController.confineLocation.name,
						", removing action for ",
						this.node.gameLocation.name,
						" ",
						this.preset.name,
						"..."
					}), Actor.HumanDebug.actions);
				}
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.DebugDestinationPosition(string.Concat(new string[]
					{
						"AI is confined to location ",
						this.goal.aiController.confineLocation.name,
						", removing action for ",
						this.node.gameLocation.name,
						" ",
						this.preset.name,
						"..."
					}));
				}
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			if (this.goal.aiController.avoidLocations != null && this.goal.aiController.avoidLocations.Contains(this.node.gameLocation))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"AI is avoiding location ",
						this.node.gameLocation.name,
						", removing action for ",
						this.node.gameLocation.name,
						" ",
						this.preset.name,
						"..."
					}), Actor.HumanDebug.actions);
				}
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.DebugDestinationPosition(string.Concat(new string[]
					{
						"AI is avoiding location ",
						this.node.gameLocation.name,
						", removing action for ",
						this.node.gameLocation.name,
						" ",
						this.preset.name,
						"..."
					}));
				}
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			if (this.goal.aiController.victimsForMurders.Count > 0)
			{
				foreach (MurderController.Murder murder2 in this.goal.aiController.victimsForMurders)
				{
					if (!(murder2.location == null) && murder2.state >= MurderController.MurderState.travellingTo && murder2.preset.blockVictimFromLeavingLocation && murder2.location != this.node.gameLocation)
					{
						if (Game.Instance.collectDebugData)
						{
							Game.Log(string.Concat(new string[]
							{
								"Murder: AI ",
								this.goal.aiController.human.name,
								" is confined to location ",
								murder2.location.name,
								", removing action for ",
								this.node.gameLocation.name,
								" ",
								this.preset.name,
								"..."
							}), 2);
							this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
							{
								"AI ",
								this.goal.aiController.human.name,
								" is confined to location ",
								murder2.location.name,
								", removing action for ",
								this.node.gameLocation.name,
								" ",
								this.preset.name,
								"..."
							}), Actor.HumanDebug.actions);
							this.goal.aiController.DebugDestinationPosition(string.Concat(new string[]
							{
								"AI ",
								this.goal.aiController.human.name,
								" is confined to location ",
								murder2.location.name,
								", removing action for ",
								this.node.gameLocation.name,
								" ",
								this.preset.name,
								"..."
							}));
						}
						this.Remove(this.preset.repeatDelayOnActionFail);
						return;
					}
				}
			}
			if (this.usagePoint != null)
			{
				this.debugInteractionUsagePosition = this.usagePoint.GetUsageWorldPosition(this.node.position, this.goal.aiController.human);
			}
			else
			{
				this.debugInteractionUsagePosition = Vector3.zero;
			}
			if (this.preset.forceReactionState)
			{
				this.goal.aiController.SetReactionState(this.preset.setReactionState);
			}
			if (this.preset.onTriggerBark.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfOnTrigger && InterfaceController.Instance.activeSpeechBubbles.Count <= CitizenControls.Instance.maxSpeechBubbles)
			{
				this.goal.aiController.human.speechController.TriggerBark(this.preset.onTriggerBark[Toolbox.Instance.Rand(0, this.preset.onTriggerBark.Count, false)]);
			}
			if (this.preset.specificOutfitOnActivate)
			{
				this.goal.aiController.human.outfitController.SetCurrentOutfit(this.preset.allowedOutfitOnActivate, false, false, true);
			}
			else if (this.preset.makeClothedOnActivate)
			{
				this.goal.aiController.human.outfitController.MakeClothed();
			}
			if (this.preset.setExpressionOnActivate)
			{
				this.goal.aiController.SetExpression(this.preset.activateExpression);
			}
			if (!this.DestinationCheck("OnActionStart: ", false))
			{
				if (this.preset.changeIdleOnActivate && !this.goal.aiController.restrained)
				{
					if (this.interactable != null && this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.forceStanding)
					{
						this.goal.aiController.human.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
					}
					else
					{
						this.goal.aiController.human.animationController.SetIdleAnimationState(this.preset.idleAnimationOnActivate);
					}
				}
				if (this.preset.changeArmsOnActivate && !this.goal.aiController.restrained)
				{
					this.goal.aiController.human.animationController.SetArmsBoolState(this.preset.armsAnimationOnActivate);
				}
				this.SetupPath(true);
			}
			this.goal.aiController.human.SpeechTriggerPoint(DDSSaveClasses.TriggerPoint.onNewAction, null, this.preset);
			this.UpdateCombatPose();
			if (this.goal.preset.actionFoundRoomBecomesPassedRoom && !this.insertedAction && this.node != null)
			{
				this.goal.roomLocation = this.node.room;
				foreach (NewAIAction newAIAction in this.goal.actions)
				{
					newAIAction.passedRoom = this.node.room;
				}
			}
			if (this.interactable != null && this.preset.tamperAction && this.interactable.preset.entertainmentSource)
			{
				foreach (NewNode newNode13 in this.interactable.node.room.nodes)
				{
					foreach (Interactable interactable4 in newNode13.interactables)
					{
						if (interactable4 != this.interactable && interactable4.preset.entertainmentSource && interactable4.sw0)
						{
							if (Game.Instance.collectDebugData)
							{
								this.goal.aiController.DebugDestinationPosition(string.Concat(new string[]
								{
									"AI avoiding turning on ",
									this.interactable.GetName(),
									" as there is another entertainment source present in this room (",
									interactable4.GetName(),
									")"
								}));
							}
							this.Remove(this.preset.repeatDelayOnActionFail);
							return;
						}
					}
				}
			}
		}
	}

	// Token: 0x06000411 RID: 1041 RVA: 0x00032BD4 File Offset: 0x00030DD4
	public bool DestinationCheck(string debug = "", bool overflowLoop = false)
	{
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Destination check...", Actor.HumanDebug.movement);
		}
		if (this.preset.cancelIfNonValidMugging && this.passedInteractable != null && this.passedInteractable.isActor != null)
		{
			Human human = this.passedInteractable.isActor as Human;
			if (human != null)
			{
				string empty = string.Empty;
				if (!this.goal.aiController.IsMuggingValid(human, out empty))
				{
					Game.Log(string.Concat(new string[]
					{
						"Failed mugging of ",
						human.name,
						" by ",
						this.goal.aiController.human.name,
						": ",
						empty
					}), 2);
					this.Remove(this.preset.repeatDelayOnActionFail);
					return false;
				}
			}
		}
		if (this.preset.cancelIfPlayerNotLoitering && this.passedInteractable != null && this.passedInteractable.isActor != null && (!Game.Instance.allowLoitering || Player.Instance.currentGameLocation == null || !this.goal.aiController.human.locationsOfAuthority.Contains(Player.Instance.currentGameLocation) || Player.Instance.currentGameLocation.playerLoiteringTimer < GameplayControls.Instance.loiteringConfrontThreshold || UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.disableLoitering) >= 1f))
		{
			Human human2 = this.passedInteractable.isActor as Human;
			if (human2 != null)
			{
				Game.Log(string.Concat(new string[]
				{
					"Canelled loitering confrontation of ",
					human2.name,
					" by ",
					this.goal.aiController.human.name,
					" (",
					GameplayControls.Instance.loiteringConfrontThreshold.ToString(),
					")"
				}), 2);
				this.Remove(this.preset.repeatDelayOnActionFail);
				return false;
			}
		}
		if (this.preset.skipIfGuestPass)
		{
			NewAddress newAddress = null;
			if (this.forcedNode != null)
			{
				newAddress = this.forcedNode.gameLocation.thisAsAddress;
			}
			if (newAddress == null && this.passedInteractable != null && this.passedInteractable.node != null)
			{
				newAddress = this.passedInteractable.node.gameLocation.thisAsAddress;
			}
			if (newAddress != null && GameplayController.Instance.guestPasses.ContainsKey(newAddress))
			{
				this.Remove(this.preset.repeatDelayOnActionFail);
				return false;
			}
		}
		if (!this.goal.preset.allowTrespass && !this.insertedAction)
		{
			if (this.goal.aiController.killerForMurders.Count > 0)
			{
				if (this.goal.aiController.killerForMurders.Exists((MurderController.Murder item) => item.state == MurderController.MurderState.executing || item.state == MurderController.MurderState.post))
				{
					goto IL_3A5;
				}
			}
			int num;
			string text;
			if (this.goal.aiController.human.IsTrespassing(this.node.room, out num, out text, this.goal.preset.allowEnforcersEverywhere))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Would be trespassing at this location (" + this.node.room.GetName() + ")", Actor.HumanDebug.actions);
				}
				this.OnUsePointBusy();
				return false;
			}
		}
		IL_3A5:
		bool flag = false;
		if (this.preset.useCloseEnoughSetting && Vector3.Distance(this.goal.aiController.human.transform.position, this.usagePoint.GetUsageWorldPosition(this.node.position, this.goal.aiController.human)) <= 1.425f)
		{
			this.SetAtDestination(true, false);
			flag = true;
		}
		if (this.goal.aiController.human.currentNode == this.node)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.DebugDestinationPosition(debug + "Reached the correct node...");
			}
			if (!this.InteractableUsePointCheck())
			{
				this.OnUsePointBusy();
			}
			else
			{
				this.goal.aiController.SetDestinationNode(this.node, false);
				if (this.usagePoint == null)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.DebugDestinationPosition(debug + "Reached the correct node, and use point is null so setting at desitnation");
					}
					this.SetAtDestination(true, false);
					flag = true;
				}
				else
				{
					Vector3 usageWorldPosition = this.usagePoint.GetUsageWorldPosition(this.node.position, this.goal.aiController.human);
					this.debugInteractionUsagePosition = usageWorldPosition;
					if (Vector3.Distance(this.goal.aiController.human.transform.position, usageWorldPosition) <= 0.05f)
					{
						Human human3 = null;
						this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human3);
						if (human3 == null || human3 == this.goal.aiController.human)
						{
							if (Game.Instance.collectDebugData)
							{
								Actor human4 = this.goal.aiController.human;
								string[] array = new string[5];
								array[0] = debug;
								array[1] = "Reached the correct node and the correct use position: ";
								array[2] = this.goal.aiController.human.transform.position.ToString();
								array[3] = ", existing: ";
								int num2 = 4;
								Human human5 = human3;
								array[num2] = ((human5 != null) ? human5.ToString() : null);
								human4.SelectedDebug(string.Concat(array), Actor.HumanDebug.actions);
								NewAIController aiController = this.goal.aiController;
								string[] array2 = new string[5];
								array2[0] = debug;
								array2[1] = "Reached the correct node and the correct use position: ";
								array2[2] = this.goal.aiController.human.transform.position.ToString();
								array2[3] = ", existing: ";
								int num3 = 4;
								Human human6 = human3;
								array2[num3] = ((human6 != null) ? human6.ToString() : null);
								aiController.DebugDestinationPosition(string.Concat(array2));
							}
							this.SetAtDestination(true, false);
							flag = true;
						}
						else if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug(debug + "Reached the correct node but the use point is busy", Actor.HumanDebug.actions);
							this.goal.aiController.DebugDestinationPosition(debug + "Reached the correct node but the use point is busy");
						}
					}
					else if (this.node != this.usagePoint.node && this.node == this.forcedNode)
					{
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.DebugDestinationPosition(debug + "Reached the correct node, but it is not the same node as evident on the usage point");
						}
						this.SetAtDestination(true, false);
						flag = true;
					}
					else if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.DebugDestinationPosition(debug + "Reached the correct node, but not at desitnation position");
					}
				}
			}
			if (!flag && this.preset.attackPersuitTargetOnProximity && this.goal.aiController.persuitTarget != null && !this.IsCloseEnoughForAttack() && Game.Instance.collectDebugData)
			{
				if (this.goal.aiController.persuitTarget.isDead)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Target is dead", Actor.HumanDebug.attacks);
				}
				if (this.goal.aiController.persuitTarget.isStunned)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Target is stunned", Actor.HumanDebug.attacks);
				}
				if (!this.goal.aiController.seesOnPersuit)
				{
					this.goal.aiController.human.SelectedDebug(debug + " SeesOnPersuit is not true", Actor.HumanDebug.attacks);
				}
				if (this.goal.aiController.human.escalationLevel < 2)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Escalation level is too low: " + this.goal.aiController.human.escalationLevel.ToString(), Actor.HumanDebug.attacks);
				}
			}
		}
		else if (this.usagePoint != null && !this.usagePoint.useSetting.useDoorBehaviour)
		{
			Vector3 usageWorldPosition2 = this.usagePoint.GetUsageWorldPosition(this.goal.aiController.human.transform.position, this.goal.aiController.human);
			this.debugInteractionUsagePosition = usageWorldPosition2;
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(usageWorldPosition2);
			NewNode newNode = null;
			if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode))
			{
				if (newNode != this.node)
				{
					this.node = newNode;
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Updated node position", Actor.HumanDebug.actions);
					}
					if (!overflowLoop)
					{
						return this.DestinationCheck("Recheck destination after use point update...", true);
					}
				}
				else
				{
					this.MovementDestinationCheck(newNode);
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				Actor human7 = this.goal.aiController.human;
				string text2 = "Unable to find node pos: ";
				Vector3 vector = usageWorldPosition2;
				human7.SelectedDebug(text2 + vector.ToString(), Actor.HumanDebug.actions);
			}
		}
		else if (this.preset.attackPersuitTargetOnProximity && this.goal.aiController.persuitTarget != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug(debug + " attempt attack...", Actor.HumanDebug.attacks);
			}
			if (!this.IsCloseEnoughForAttack() && Game.Instance.collectDebugData)
			{
				if (this.goal.aiController.persuitTarget.isDead)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Target is dead", Actor.HumanDebug.attacks);
				}
				if (this.goal.aiController.persuitTarget.isStunned)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Target is stunned", Actor.HumanDebug.attacks);
				}
				if (!this.goal.aiController.seesOnPersuit)
				{
					this.goal.aiController.human.SelectedDebug(debug + " SeesOnPersuit is not true", Actor.HumanDebug.attacks);
				}
				if (this.goal.aiController.human.escalationLevel < 2)
				{
					this.goal.aiController.human.SelectedDebug(debug + " Escalation level is too low: " + this.goal.aiController.human.escalationLevel.ToString(), Actor.HumanDebug.attacks);
				}
			}
		}
		else if (this.node == null)
		{
			this.goal.aiController.DebugDestinationPosition(debug + ": Null node detected!");
			this.SetAtDestination(true, false);
			flag = true;
		}
		else
		{
			this.SetAtDestination(false, false);
			this.MovementDestinationCheck(this.node);
			if (this.preset.useLOSCheck)
			{
				this.LOSCheck();
			}
		}
		return flag;
	}

	// Token: 0x06000412 RID: 1042 RVA: 0x00033730 File Offset: 0x00031930
	private bool IsCloseEnoughForAttack()
	{
		bool result = false;
		if (this.goal.aiController.seesOnPersuit && !this.goal.aiController.persuitTarget.isDead && !this.goal.aiController.persuitTarget.isStunned && this.goal.aiController.human.escalationLevel >= 2)
		{
			float num = Vector3.Distance(this.goal.aiController.persuitTarget.transform.position, this.goal.aiController.transform.position);
			if (num <= this.goal.aiController.weaponRangeMax && num >= this.goal.aiController.currentWeaponPreset.minimumRange)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(this.debug + " within weapon range, setting at destination to trigger attack...", Actor.HumanDebug.attacks);
				}
				result = true;
				this.goal.aiController.SetDestinationNode(this.node, false);
				this.SetAtDestination(true, true);
			}
			else
			{
				if (Game.Instance.collectDebugData)
				{
					if (num > this.goal.aiController.weaponRangeMax)
					{
						this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							this.debug,
							" too far to use weapon (",
							num.ToString(),
							"/",
							this.goal.aiController.weaponRangeMax.ToString(),
							")"
						}), Actor.HumanDebug.attacks);
					}
					else if (num < this.goal.aiController.currentWeaponPreset.minimumRange)
					{
						this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							this.debug,
							" too close to use weapon (",
							num.ToString(),
							"/",
							this.goal.aiController.currentWeaponPreset.minimumRange.ToString(),
							")"
						}), Actor.HumanDebug.attacks);
					}
				}
				if (this.preset.throwObjectsAtTarget && !this.goal.preset.disableThrowing && this.goal.aiController.killerForMurders.Count <= 0 && num <= CitizenControls.Instance.throwMaxRange && num >= CitizenControls.Instance.throwMinRange && (this.goal.aiController.human.currentConsumables.Count > 0 || this.goal.aiController.human.trash.Count > 0) && this.goal.aiController.attackDelay <= 0f && !this.goal.aiController.attackActive && this.goal.aiController.throwDelay <= 0f)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " within throwing range, setting at destination to trigger attack...", Actor.HumanDebug.attacks);
					}
					result = true;
					this.goal.aiController.SetDestinationNode(this.node, false);
					this.SetAtDestination(true, true);
				}
			}
		}
		return result;
	}

	// Token: 0x06000413 RID: 1043 RVA: 0x00033A98 File Offset: 0x00031C98
	public void MovementDestinationCheck(NewNode resetNode)
	{
		if (this.goal.aiController.currentDestinationPositon == this.goal.aiController.transform.position)
		{
			this.goal.aiController.SetDestinationNode(resetNode, true);
		}
	}

	// Token: 0x06000414 RID: 1044 RVA: 0x00033AD8 File Offset: 0x00031CD8
	public void SetUsagePoint(Interactable.UsagePoint newUsagePoint, Interactable.UsePointSlot newSlot)
	{
		if (this.preset.actionLocation == AIActionPreset.ActionLocation.interactableSpawn)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Action location is interactable spawn, so no usage point should be set...", Actor.HumanDebug.actions);
			}
			this.usagePoint = null;
			return;
		}
		if (Game.Instance.collectDebugData)
		{
			if (newUsagePoint != null)
			{
				this.goal.aiController.human.SelectedDebug("Setting usage point... " + newUsagePoint.interactable.name, Actor.HumanDebug.actions);
			}
			else
			{
				this.goal.aiController.human.SelectedDebug("Setting usage point to null", Actor.HumanDebug.actions);
			}
		}
		if (newUsagePoint == this.usagePoint)
		{
			return;
		}
		if (this.usagePoint != null)
		{
			this.usagePoint.RemoveUserFromAllSlots(this.goal.aiController.human);
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Set new usage point slot " + newSlot.ToString(), Actor.HumanDebug.actions);
		}
		this.usagePoint = newUsagePoint;
	}

	// Token: 0x06000415 RID: 1045 RVA: 0x00033BE8 File Offset: 0x00031DE8
	public bool InteractableUsePointCheck()
	{
		Human human = null;
		if (this.usagePoint != null)
		{
			try
			{
				if (Game.Instance.collectDebugData)
				{
					Actor human2 = this.goal.aiController.human;
					string[] array = new string[6];
					array[0] = "Interactable use point check for: ";
					array[1] = this.usagePoint.interactable.name;
					array[2] = " ";
					array[3] = this.usagePoint.interactable.id.ToString();
					array[4] = " at ";
					int num = 5;
					Vector3 position = this.usagePoint.interactable.node.position;
					array[num] = position.ToString();
					human2.SelectedDebug(string.Concat(array), Actor.HumanDebug.actions);
				}
			}
			catch
			{
			}
			this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human);
		}
		if (this.usagePoint == null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("The currently set usage point is null...", Actor.HumanDebug.actions);
			}
			return true;
		}
		if (human == null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("The currently set usage point is empty...", Actor.HumanDebug.actions);
			}
			return true;
		}
		if (!(human != null))
		{
			return this.interactable == null;
		}
		if (human == this.goal.aiController.human)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("The existing usage point user is me: " + human.name, Actor.HumanDebug.actions);
			}
			return true;
		}
		if (human.ai != null && human.ai.currentAction != null && human.ai.currentAction.usagePoint != this.usagePoint && Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Invalid usage point detected for " + human.name + ", their usage point is elsewhere and this did not unassign correctly", Actor.HumanDebug.actions);
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("The currently set usage point is full (" + human.name + "), searching for other use points at the same furniture...", Actor.HumanDebug.actions);
		}
		return false;
	}

	// Token: 0x06000416 RID: 1046 RVA: 0x00033E34 File Offset: 0x00032034
	public void OnUsePointBusy()
	{
		if (this.preset.armsAnimationOnArrival != CitizenAnimationController.ArmsBoolSate.none)
		{
			this.goal.aiController.human.animationController.SetArmsBoolState(CitizenAnimationController.ArmsBoolSate.none);
		}
		if (this.preset.idleAnimationOnArrival != CitizenAnimationController.IdleAnimationState.none)
		{
			this.goal.aiController.human.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
		}
		if (!this.actionCheckRecursion && this.preset.onUsePointBusy == AIActionPreset.ActionBusy.findAlternate)
		{
			if (this.UsingFurnitureCheck())
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("...Successfully found an alternate interactable solution for current action.", Actor.HumanDebug.actions);
				}
				this.actionCheckRecursion = true;
				this.SetupPath(true);
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("...Failed to find alternate interactable solution for current action, postponing goal...", Actor.HumanDebug.actions);
			}
			if (this.goal != null)
			{
				this.actionCheckRecursion = true;
				this.goal.OnDeactivate(this.goal.preset.repeatDelayOnBusy);
				return;
			}
		}
		else
		{
			if (this.actionCheckRecursion || this.preset.onUsePointBusy == AIActionPreset.ActionBusy.skipAction)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("...Skipping action only...", Actor.HumanDebug.actions);
				}
				this.actionCheckRecursion = true;
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			if (this.preset.onUsePointBusy == AIActionPreset.ActionBusy.skipGoal || (this.preset.onUsePointBusy == AIActionPreset.ActionBusy.standGuardIfEnforcerSkipGoalNot && (!this.goal.aiController.human.isEnforcer || !this.goal.aiController.human.isOnDuty)))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("...Skipping goal...", Actor.HumanDebug.actions);
				}
				if (this.goal != null)
				{
					this.actionCheckRecursion = true;
					this.goal.OnDeactivate(this.goal.preset.repeatDelayOnBusy);
					return;
				}
			}
			else if (this.preset.onUsePointBusy == AIActionPreset.ActionBusy.standGuard || (this.preset.onUsePointBusy == AIActionPreset.ActionBusy.standGuardIfEnforcerSkipGoalNot && this.goal.aiController.human.isEnforcer && this.goal.aiController.human.isOnDuty))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("...Skipping action, standing guard...", Actor.HumanDebug.actions);
				}
				Interactable interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.standAgainstWall, this.goal.aiController.human.currentRoom, this.goal.aiController.human, AIActionPreset.FindSetting.onlyPublic, true, null, this.goal.aiController.human.currentGameLocation, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, true);
				if (interactable != null)
				{
					if (!this.goal.TryInsertInteractableAction(interactable, RoutineControls.Instance.standAgainstWall, 99, null, true))
					{
						this.actionCheckRecursion = true;
						this.Remove(this.preset.repeatDelayOnActionFail);
						return;
					}
				}
				else
				{
					interactable = Toolbox.Instance.FindNearestWithAction(RoutineControls.Instance.standGuard, this.goal.aiController.human.currentRoom, this.goal.aiController.human, AIActionPreset.FindSetting.onlyPublic, true, null, this.goal.aiController.human.currentGameLocation, null, false, InteractablePreset.SpecialCase.none, false, null, true, false, 0f, null, null, false, false, CompanyPreset.CompanyCategory.meal, true);
					if (!this.goal.TryInsertInteractableAction(interactable, RoutineControls.Instance.standGuard, 99, null, true))
					{
						this.actionCheckRecursion = true;
						this.Remove(this.preset.repeatDelayOnActionFail);
						return;
					}
				}
				this.actionCheckRecursion = true;
			}
		}
	}

	// Token: 0x06000417 RID: 1047 RVA: 0x000341F0 File Offset: 0x000323F0
	public void UpdateCombatPose()
	{
		if (this.preset.useCombatPose == AIActionPreset.CombatPose.always)
		{
			if (!this.preset.onlyUseCombatPoseWithEscalationOne || (this.preset.onlyUseCombatPoseWithEscalationOne && this.goal.aiController.human.escalationLevel >= 2))
			{
				this.goal.aiController.SetInCombat(true, false);
				return;
			}
			this.goal.aiController.SetInCombat(false, false);
			return;
		}
		else
		{
			if (this.preset.useCombatPose == AIActionPreset.CombatPose.never)
			{
				this.goal.aiController.SetInCombat(false, false);
				return;
			}
			if (this.preset.useCombatPose != AIActionPreset.CombatPose.onlyWhenPreviouslyPersuing)
			{
				if (this.preset.useCombatPose == AIActionPreset.CombatPose.onlyWhenAtDestination)
				{
					if (this.isAtLocation)
					{
						this.goal.aiController.SetInCombat(true, false);
						return;
					}
					if (this.goal.aiController.persuitTarget != null && (!this.preset.onlyUseCombatPoseWithEscalationOne || (this.preset.onlyUseCombatPoseWithEscalationOne && this.goal.aiController.human.escalationLevel >= 2)))
					{
						this.goal.aiController.SetInCombat(true, false);
						return;
					}
					this.goal.aiController.SetInCombat(false, false);
				}
				return;
			}
			if (this.goal.aiController.persuitTarget != null && (!this.preset.onlyUseCombatPoseWithEscalationOne || (this.preset.onlyUseCombatPoseWithEscalationOne && this.goal.aiController.human.escalationLevel >= 2)))
			{
				this.goal.aiController.SetInCombat(true, false);
				return;
			}
			this.goal.aiController.SetInCombat(false, false);
			return;
		}
	}

	// Token: 0x06000418 RID: 1048 RVA: 0x000343A0 File Offset: 0x000325A0
	public void SetupPath(bool scanForNextNodeFurniture = true)
	{
		try
		{
			if (Game.Instance.collectDebugData)
			{
				Actor human = this.goal.aiController.human;
				string[] array = new string[6];
				array[0] = "Setting up path to node ";
				int num = 1;
				Vector3 position = this.node.position;
				array[num] = position.ToString();
				array[2] = " from ";
				int num2 = 3;
				position = this.goal.aiController.human.currentNode.position;
				array[num2] = position.ToString();
				array[4] = " , scan for next node furniture: ";
				array[5] = scanForNextNodeFurniture.ToString();
				human.SelectedDebug(string.Concat(array), Actor.HumanDebug.movement);
			}
			if (Game.Instance.collectRoutineTimingInfo)
			{
				this.estimatedArrival = SessionData.Instance.gameTime + Toolbox.Instance.TravelTimeEstimate(this.goal.aiController.human, this.goal.aiController.human.currentNode, this.node);
			}
			this.path = PathFinder.Instance.GetPath(this.goal.aiController.human.currentNode, this.node, this.goal.aiController.human, null);
		}
		catch
		{
		}
		if (this.path == null)
		{
			this.Remove(this.preset.repeatDelayOnActionFail);
			return;
		}
		this.goal.aiController.pathCursor = -1;
		this.goal.aiController.ReachNewPathNode("Start action", scanForNextNodeFurniture);
		if (this.path != null && Game.Instance.collectDebugData)
		{
			this.goal.aiController.DebugDestinationPosition(string.Concat(new string[]
			{
				"Set up new path with ",
				this.path.accessList.Count.ToString(),
				" nodes for action ",
				this.preset.name,
				", set destination to false..."
			}));
		}
		this.SetAtDestination(false, false);
		this.goal.aiController.SetUpdateEnabled(true);
		if (this.path != null && this.path.accessList.Count <= 0)
		{
			this.DestinationCheck("Checking for destination as path list is 0", false);
		}
	}

	// Token: 0x06000419 RID: 1049 RVA: 0x000345E8 File Offset: 0x000327E8
	public bool UsingFurnitureCheck()
	{
		if (this.interactable == null)
		{
			return false;
		}
		List<Interactable> list = new List<Interactable>();
		list.Add(this.interactable);
		if (this.interactable.node == null)
		{
			Game.Log("Attempting update update node for interactable " + this.interactable.name + " as node is missing...", 2);
			this.interactable.UpdateWorldPositionAndNode(false);
		}
		List<Interactable> list2 = new List<Interactable>();
		if (this.interactable != null && this.interactable.node != null && this.interactable.node.room.actionReference.ContainsKey(this.preset))
		{
			list2 = this.interactable.node.room.actionReference[this.preset];
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("...Scanning " + list2.Count.ToString() + " entries in same room...", Actor.HumanDebug.actions);
		}
		Interactable interactable = this.InteractablePicker(ref list2, this.goal.aiController.human.transform.position, this.preset.socialRules, out this.node, out this.usagePoint, this.passedGroup, false, true, list);
		if (interactable != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Found another interactable within the same room: " + interactable.name, Actor.HumanDebug.actions);
			}
			this.interactable = interactable;
			this.SetUsagePoint(this.interactable.usagePoint, this.preset.usageSlot);
			this.node = this.usagePoint.node;
			return true;
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Couldn't find another node on the same furniture, checking gamelocation for other interactables...", Actor.HumanDebug.actions);
		}
		if (this.interactable.node.gameLocation.actionReference.ContainsKey(this.preset))
		{
			list2 = this.interactable.node.gameLocation.actionReference[this.preset];
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("...Scanning " + list2.Count.ToString() + " entries...", Actor.HumanDebug.actions);
		}
		interactable = this.InteractablePicker(ref list2, this.goal.aiController.human.transform.position, this.preset.socialRules, out this.node, out this.usagePoint, this.passedGroup, false, true, list);
		if (interactable != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Found another interactable within the same gamelocation: " + interactable.name, Actor.HumanDebug.actions);
			}
			this.interactable = interactable;
			this.SetUsagePoint(this.interactable.usagePoint, this.preset.usageSlot);
			this.node = this.usagePoint.node;
			return true;
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Couldn't find other furniture within the same address, checking for nearest public interactables...", Actor.HumanDebug.actions);
		}
		if (!this.preset.limitSearchToGoalLocation && (this.goal == null || !this.goal.preset.passedGamelocationIsImportant))
		{
			HashSet<NewRoom> ignore = new HashSet<NewRoom>(this.interactable.node.gameLocation.rooms);
			bool mustContainDesireCategory = false;
			if (this.goal.preset.locationOption == AIGoalPreset.LocationOption.commercialDecision)
			{
				mustContainDesireCategory = true;
			}
			Interactable interactable2 = Toolbox.Instance.FindNearestWithAction(this.preset, this.goal.aiController.human.currentRoom, this.goal.aiController.human, AIActionPreset.FindSetting.nonTrespassing, true, ignore, null, null, false, InteractablePreset.SpecialCase.none, this.preset.filterSearchUsingRoomType, this.preset.searchRoomType, true, this.goal.preset.allowEnforcersEverywhere, 0f, null, this.passedAcquireItems, false, mustContainDesireCategory, this.goal.preset.desireCategory, false);
			if (interactable2 != null)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Found another interactable - nearest public", Actor.HumanDebug.actions);
				}
				this.SetUsagePoint(interactable2.usagePoint, this.preset.usageSlot);
				this.node = this.usagePoint.node;
				this.interactable = interactable2;
				return true;
			}
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("...Failed to find any valid interactable interaction spaces on this or any other furniture :(", Actor.HumanDebug.actions);
		}
		return false;
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00034A84 File Offset: 0x00032C84
	public void OnDeactivate(bool executeDeactivateAnimation = true)
	{
		this.actionCheckRecursion = false;
		this.EndSoundLoop();
		if (this.preset.executeCompleteActionsOnEnd && !this.completed && (!this.preset.executeCompleteActionsOnEndIfArrived || this.isAtLocation))
		{
			this.ExecuteAdditionalActions(ref this.preset.forcedActionsOnComplete);
		}
		if (!this.completed && this.isAtLocation)
		{
			this.ExecuteEndSwitchChanges();
			this.DropItemAtEnd();
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.DebugDestinationPosition("Deactivate action: " + this.preset.name);
			this.goal.aiController.human.SelectedDebug("Deactivated action: " + this.preset.name + " actor position: " + this.goal.aiController.transform.position.ToString(), Actor.HumanDebug.actions);
		}
		if (this.usagePoint != null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Deactivate: Setting usage point to null...", Actor.HumanDebug.actions);
			}
			Human human = null;
			this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human);
			if (human == this.goal.aiController.human)
			{
				this.usagePoint.TrySetUser(this.preset.usageSlot, null, "OnDeactivate action " + this.preset.name);
			}
		}
		else if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Deactivate: Usage point is already null...", Actor.HumanDebug.actions);
		}
		this.SetUsagePoint(null, this.preset.usageSlot);
		this.CancelNextAIInteraction();
		if (this.preset.specificOutfitOnDeactivate && !this.goal.aiController.restrained)
		{
			this.goal.aiController.human.outfitController.SetCurrentOutfit(this.preset.allowedOutfitOnDeactivate, false, false, true);
		}
		else if (this.preset.makeClothedOnDeactivate && !this.goal.aiController.restrained)
		{
			this.goal.aiController.human.outfitController.MakeClothed();
		}
		if (this.preset.setExpressionOnDeactivate && !this.goal.aiController.restrained)
		{
			this.goal.aiController.SetExpression(this.preset.deactivateExpression);
		}
		if (this.isActive)
		{
			this.goal.aiController.currentAction = null;
			this.isActive = false;
			this.node = null;
			this.path = null;
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Deactivated previously-active action: " + this.preset.name, Actor.HumanDebug.actions);
				this.goal.aiController.DebugDestinationPosition("Set at destination to false on deactivate of new action");
			}
			if (this.preset.limitTickRate)
			{
				this.goal.aiController.UpdateTickRate(false);
			}
			this.SetAtDestination(false, false);
		}
		this.goal.aiController.UpdateHeldItems(AIActionPreset.ActionStateFlag.onDeactivation);
		if (executeDeactivateAnimation)
		{
			if (this.preset.changeIdleOnDeactivate && !this.goal.aiController.restrained)
			{
				this.goal.aiController.human.animationController.SetIdleAnimationState(this.preset.idleAnimationOnDeactivate);
			}
			if (this.preset.changeArmsOnDeactivate && !this.goal.aiController.restrained)
			{
				this.goal.aiController.human.animationController.SetArmsBoolState(this.preset.armsAnimationOnDeactivate);
			}
		}
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00034E4C File Offset: 0x0003304C
	public void CancelNextAIInteraction()
	{
		if (this.passedInteractable != null && this.passedInteractable.nextAIInteraction == this)
		{
			this.passedInteractable.SetNextAIInteraction(null, this.goal.aiController);
		}
		if (this.interactable != null && this.interactable.nextAIInteraction == this)
		{
			this.interactable.SetNextAIInteraction(null, this.goal.aiController);
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00034EB4 File Offset: 0x000330B4
	public void Complete()
	{
		this.completed = true;
		this.EndSoundLoop();
		if (this.preset.specificOutfitOnComplete)
		{
			this.goal.aiController.human.outfitController.SetCurrentOutfit(this.preset.allowedOutfitOnComplete, false, false, true);
		}
		else if (this.preset.makeClothedOnActivate)
		{
			this.goal.aiController.human.outfitController.MakeClothed();
		}
		if (this.preset.setExpressionOnComplete)
		{
			this.goal.aiController.SetExpression(this.preset.completeExpression);
		}
		this.CancelNextAIInteraction();
		if (this.preset.executeThisOnComplete)
		{
			if (this.interactable != null)
			{
				InteractablePreset.InteractionAction action = null;
				if (this.interactable.aiActionReference.TryGetValue(this.preset, ref action))
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
						{
							"Executing action ",
							this.preset.name,
							" in interactable ",
							this.interactable.name,
							" ",
							this.interactable.id.ToString()
						}), Actor.HumanDebug.actions);
					}
					this.interactable.OnInteraction(action, this.goal.aiController.human, true, 0f);
				}
				else if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"Could not find action ",
						this.preset.name,
						" in interactable ",
						this.interactable.name,
						" ",
						this.interactable.id.ToString()
					}), Actor.HumanDebug.actions);
				}
			}
			else if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Could not find action " + this.preset.name + " because interactable isn't part of this action", Actor.HumanDebug.actions);
			}
		}
		this.ExecuteAdditionalActions(ref this.preset.forcedActionsOnComplete);
		this.ExecuteEndSwitchChanges();
		this.DropItemAtEnd();
		if (this.preset.onCompleteBark.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfOnComplete && InterfaceController.Instance.activeSpeechBubbles.Count <= CitizenControls.Instance.maxSpeechBubbles)
		{
			this.goal.aiController.human.speechController.TriggerBark(this.preset.onCompleteBark[Toolbox.Instance.Rand(0, this.preset.onCompleteBark.Count, false)]);
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Completed action: " + this.preset.name, Actor.HumanDebug.actions);
		}
		this.goal.OnCompletedAction(this);
		this.Remove(this.preset.repeatDelayOnActionSuccess);
		this.goal.aiController.UpdateHeldItems(AIActionPreset.ActionStateFlag.onDeactivation);
		if (this.preset.changeIdleOnComplete && !this.goal.aiController.restrained)
		{
			this.goal.aiController.human.animationController.SetIdleAnimationState(this.preset.idleAnimationOnComplete);
		}
		if (this.preset.changeArmsOnComplete && !this.goal.aiController.restrained)
		{
			this.goal.aiController.human.animationController.SetArmsBoolState(this.preset.armsAnimationOnComplete);
		}
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00035288 File Offset: 0x00033488
	private void ExecuteAdditionalActions(ref List<AIActionPreset.AutomaticAction> actionPresets)
	{
		if (this.interactable == null)
		{
			return;
		}
		foreach (AIActionPreset.AutomaticAction automaticAction in actionPresets)
		{
			InteractablePreset.InteractionAction interactionAction = null;
			if (automaticAction.proximityCheck)
			{
				if (this.interactable == null)
				{
					continue;
				}
				float num;
				if (this.interactable.isActor != null)
				{
					if (!this.goal.aiController.trackedTargets.Exists((NewAIController.TrackingTarget item) => item.actor != null && item.actor == this.interactable.isActor))
					{
						continue;
					}
					num = Vector3.Distance(this.interactable.isActor.transform.position, this.goal.aiController.human.transform.position);
				}
				else
				{
					num = Vector3.Distance(this.interactable.wPos, this.goal.aiController.human.transform.position);
				}
				if (num > 3f)
				{
					continue;
				}
			}
			if (this.interactable.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"Force action: ",
						interactionAction.interactionName,
						" (",
						interactionAction.action.presetName,
						")"
					}), Actor.HumanDebug.actions);
				}
				this.interactable.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
			}
			else if (this.interactable.lockInteractable != null && this.interactable.lockInteractable.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Force action found on lock: " + interactionAction.interactionName, Actor.HumanDebug.actions);
				}
				this.interactable.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
			}
			else
			{
				bool flag = false;
				if (this.interactable.furnitureParent != null)
				{
					if (this.preset.forcedActionsSearchLevel >= AIActionPreset.ForcedActionsSearchLevel.otherIntegratedInteractables)
					{
						foreach (Interactable interactable in this.interactable.furnitureParent.integratedInteractables)
						{
							if (interactable != this.interactable && interactable.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction))
							{
								if (Game.Instance.collectDebugData)
								{
									this.goal.aiController.human.SelectedDebug("Force action: " + interactionAction.interactionName, Actor.HumanDebug.actions);
								}
								interactable.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
								flag = true;
								break;
							}
						}
					}
					if (!flag && this.preset.forcedActionsSearchLevel == AIActionPreset.ForcedActionsSearchLevel.spawnInteractablesChildren)
					{
						InteractableController interactableController = Enumerable.ToList<InteractableController>(this.interactable.furnitureParent.furniture.prefab.GetComponentsInChildren<InteractableController>(true)).Find((InteractableController item) => item.id == (InteractableController.InteractableID)this.interactable.pt);
						if (!(interactableController == null))
						{
							goto IL_4E8;
						}
						string text = interactableController.gameObject.name;
						using (List<Interactable>.Enumerator enumerator2 = this.interactable.furnitureParent.spawnedInteractables.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								Interactable interactable2 = enumerator2.Current;
								if (interactable2 != this.interactable && interactable2.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction) && interactable2.subObject.parent == text)
								{
									if (Game.Instance.collectDebugData)
									{
										this.goal.aiController.human.SelectedDebug("Force action: " + interactionAction.interactionName, Actor.HumanDebug.actions);
									}
									interactable2.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
									flag = true;
									break;
								}
							}
							goto IL_4E8;
						}
					}
					if (!flag && this.preset.forcedActionsSearchLevel >= AIActionPreset.ForcedActionsSearchLevel.spawnedInteractablesAll)
					{
						foreach (Interactable interactable3 in this.interactable.furnitureParent.spawnedInteractables)
						{
							if (interactable3 != this.interactable && interactable3.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction))
							{
								if (Game.Instance.collectDebugData)
								{
									this.goal.aiController.human.SelectedDebug("Force action: " + interactionAction.interactionName, Actor.HumanDebug.actions);
								}
								interactable3.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
								flag = true;
								break;
							}
						}
					}
				}
				IL_4E8:
				if (!flag && this.preset.forcedActionsSearchLevel == AIActionPreset.ForcedActionsSearchLevel.InteractablesOnNode)
				{
					foreach (Interactable interactable4 in this.interactable.node.interactables)
					{
						if (interactable4 != this.interactable && interactable4.aiActionReference.TryGetValue(automaticAction.forcedAction, ref interactionAction))
						{
							if (Game.Instance.collectDebugData)
							{
								this.goal.aiController.human.SelectedDebug("Force action: " + interactionAction.interactionName, Actor.HumanDebug.actions);
							}
							interactable4.OnInteraction(interactionAction, this.goal.aiController.human, true, automaticAction.additionalDelay);
							break;
						}
					}
				}
			}
		}
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x000358E0 File Offset: 0x00033AE0
	public void ExecuteEndSwitchChanges()
	{
		if (this.preset.switchStatesOnEnd.Count > 0 && this.interactable != null)
		{
			foreach (InteractablePreset.SwitchState switchState in this.preset.switchStatesOnEnd)
			{
				this.interactable.SetSwtichByType(switchState.switchState, switchState.boolIs, this.goal.aiController.human, true, false);
			}
		}
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x00035978 File Offset: 0x00033B78
	public void DropItemAtEnd()
	{
		if (this.preset.dropItemOnEnd != null)
		{
			Vector2 vector = Random.insideUnitCircle * 0.33f;
			Vector3 vector2 = this.goal.aiController.human.transform.position;
			if (this.goal.aiController.human.currentNode.walkableNodeSpace.Count > 0)
			{
				List<NewNode.NodeSpace> list = new List<NewNode.NodeSpace>(this.goal.aiController.human.currentNode.walkableNodeSpace.Values);
				vector2 = list[Toolbox.Instance.Rand(0, list.Count, false)].position + new Vector3(vector.x, 0f, vector.y);
			}
			else
			{
				vector2 += new Vector3(vector.x, 0f, vector.y);
			}
			Vector3 vector3;
			vector3..ctor(0f, Toolbox.Instance.Rand(0f, 360f, false));
			if (Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.dropItemOnEnd.chanceOfDroppedAngle)
			{
				vector3 += new Vector3(-90f, 0f, 0f);
				vector2.y += this.preset.dropItemOnEnd.droppedAngleHeightBoost;
			}
			Interactable interactable = InteractableCreator.Instance.CreateWorldInteractable(this.preset.dropItemOnEnd, this.goal.aiController.human, this.goal.aiController.human, this.goal.aiController.human, vector2, vector3, null, null, "");
			interactable.MarkAsTrash(true, false, 0f);
			interactable.ft = true;
		}
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x00035B44 File Offset: 0x00033D44
	public void Remove(float delayReactivationTime = 0f)
	{
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
			{
				"Removing action ",
				this.preset.name,
				" (repeat delay of ",
				delayReactivationTime.ToString(),
				")"
			}), Actor.HumanDebug.actions);
		}
		this.EndSoundLoop();
		if (delayReactivationTime > 0f)
		{
			if (!this.goal.aiController.delayedActionsForTime.ContainsKey(this.preset))
			{
				this.goal.aiController.delayedActionsForTime.Add(this.preset, 0f);
			}
			this.goal.aiController.delayedActionsForTime[this.preset] = SessionData.Instance.gameTime + delayReactivationTime;
		}
		if (this.usagePoint != null)
		{
			Human human = null;
			this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human);
			if (human == this.goal.aiController.human)
			{
				this.usagePoint.TrySetUser(this.preset.usageSlot, null, "OnRemove action: " + this.preset.name);
			}
			this.SetUsagePoint(null, this.preset.usageSlot);
		}
		this.CancelNextAIInteraction();
		if (this.isActive)
		{
			this.goal.aiController.currentAction = null;
			this.isActive = false;
			if (this.preset.limitTickRate)
			{
				this.goal.aiController.UpdateTickRate(false);
			}
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Remove action: " + this.preset.name, Actor.HumanDebug.actions);
		}
		this.goal.actions.Remove(this);
	}

	// Token: 0x06000421 RID: 1057 RVA: 0x00035D28 File Offset: 0x00033F28
	public void TriggerArrivalSound()
	{
		if (this.preset.onArrivalSound != null)
		{
			Vector3 worldPosition = this.goal.aiController.human.transform.position;
			if (this.interactable != null)
			{
				worldPosition = this.interactable.GetWorldPosition(true);
			}
			if (this.preset.isLoop)
			{
				if (this.audioLoop != null)
				{
					AudioController.Instance.StopSound(this.audioLoop, AudioController.StopType.immediate, "Start of new action requires new loop");
					this.audioLoop = null;
				}
				this.audioLoop = AudioController.Instance.PlayWorldLooping(this.preset.onArrivalSound, this.goal.aiController.human, this.interactable, null, 1f, false, false, null, null);
				return;
			}
			if (this.preset.soundDelay > 0f)
			{
				AudioController.Instance.PlayOneShotDelayed(this.preset.soundDelay, this.preset.onArrivalSound, this.goal.aiController.human, this.goal.aiController.human.currentNode, worldPosition, null, 1f, null, false);
				return;
			}
			AudioController.Instance.PlayWorldOneShot(this.preset.onArrivalSound, this.goal.aiController.human, this.goal.aiController.human.currentNode, worldPosition, this.interactable, null, 1f, null, false, null, false);
		}
	}

	// Token: 0x06000422 RID: 1058 RVA: 0x00035E96 File Offset: 0x00034096
	public void EndSoundLoop()
	{
		if (this.audioLoop != null)
		{
			AudioController.Instance.StopSound(this.audioLoop, AudioController.StopType.fade, "End of action");
			this.audioLoop = null;
		}
	}

	// Token: 0x06000423 RID: 1059 RVA: 0x00035EC0 File Offset: 0x000340C0
	public void AITick()
	{
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Action AITick: " + this.preset.name, Actor.HumanDebug.updates);
		}
		Human human = null;
		if (this.usagePoint != null)
		{
			if (this.interactable != null && this.interactable.isActor != null)
			{
				this.usagePoint.node = this.interactable.isActor.currentNode;
				this.usagePoint.PositionUpdate();
				this.node = this.usagePoint.node;
			}
			this.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human);
		}
		if (this.preset.completableAction && this.interactable != null && this.usagePoint != null && human != this.goal.aiController.human && this.isActive && !this.completed)
		{
			foreach (AIActionPreset.CheckActionAgainstState checkActionAgainstState in this.preset.checkActionAgainstState)
			{
				if (this.InteractableStateCheck(checkActionAgainstState))
				{
					if (checkActionAgainstState.outcome == AIActionPreset.CheckActionOutcome.cancelAction)
					{
						this.Remove(this.preset.repeatDelayOnActionFail);
						return;
					}
					if (checkActionAgainstState.outcome == AIActionPreset.CheckActionOutcome.cancelGoal)
					{
						this.goal.Remove();
						return;
					}
				}
			}
		}
		if (this.preset.useLOSCheck)
		{
			this.LOSCheck();
		}
		if (this.isAtLocation)
		{
			float num = SessionData.Instance.gameTime - this.lastRecordedTickWhileAtDesitnation;
			this.lastRecordedTickWhileAtDesitnation = SessionData.Instance.gameTime;
			if (this.dontUpdateGoalPriorityForExtraTime > 0f)
			{
				this.dontUpdateGoalPriorityForExtraTime -= num;
				this.dontUpdateGoalPriorityForExtraTime = Mathf.Max(this.dontUpdateGoalPriorityForExtraTime, 0f);
			}
			if (this.preset.forcedActive.Count > 0 && this.interactable != null && this.interactable.furnitureParent != null)
			{
				foreach (Interactable interactable in this.interactable.furnitureParent.integratedInteractables)
				{
					if (this.preset.forcedActive.Contains(interactable.preset) && !interactable.sw0)
					{
						interactable.SetSwitchState(true, this.goal.aiController.human, true, false, false);
					}
				}
			}
			bool canFallAsleep = this.preset.canFallAsleep;
			if (this.preset.attackPersuitTargetOnProximity && this.goal.aiController.persuitTarget != null && !this.goal.aiController.attackActive)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(this.debug + " attempt attack...", Actor.HumanDebug.attacks);
				}
				if (this.goal.aiController.seesOnPersuit && !this.goal.aiController.persuitTarget.isDead && !this.goal.aiController.persuitTarget.isStunned && this.goal.aiController.human.escalationLevel >= 2)
				{
					float num2 = Vector3.Distance(this.goal.aiController.persuitTarget.transform.position, this.goal.aiController.transform.position);
					if (num2 <= this.goal.aiController.weaponRangeMax && num2 >= this.goal.aiController.currentWeaponPreset.minimumRange)
					{
						this.goal.aiController.StartAttack(this.goal.aiController.persuitTarget);
					}
					else
					{
						if (Game.Instance.collectDebugData)
						{
							if (num2 < this.goal.aiController.weaponRangeMax)
							{
								this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
								{
									this.debug,
									" too far to use weapon (",
									num2.ToString(),
									"/",
									this.goal.aiController.weaponRangeMax.ToString(),
									")"
								}), Actor.HumanDebug.attacks);
							}
							else if (num2 > this.goal.aiController.currentWeaponPreset.minimumRange)
							{
								this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
								{
									this.debug,
									" to close too use weapon (",
									num2.ToString(),
									"/",
									this.goal.aiController.currentWeaponPreset.minimumRange.ToString(),
									")"
								}), Actor.HumanDebug.attacks);
							}
						}
						if (this.preset.throwObjectsAtTarget && !this.goal.preset.disableThrowing && this.goal.aiController.killerForMurders.Count <= 0 && num2 <= CitizenControls.Instance.throwMaxRange && num2 >= CitizenControls.Instance.throwMinRange && (this.goal.aiController.human.currentConsumables.Count > 0 || this.goal.aiController.human.trash.Count > 0))
						{
							this.goal.aiController.ThrowObject(this.goal.aiController.persuitTarget);
						}
					}
				}
				else if (Game.Instance.collectDebugData)
				{
					if (this.goal.aiController.persuitTarget.isDead)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " Target is dead", Actor.HumanDebug.attacks);
					}
					if (this.goal.aiController.persuitTarget.isStunned)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " Target is stunned", Actor.HumanDebug.attacks);
					}
					if (!this.goal.aiController.seesOnPersuit)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " SeesOnPersuit is not true", Actor.HumanDebug.attacks);
					}
					if (this.goal.aiController.human.escalationLevel < 2)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " Escalation level is too low: " + this.goal.aiController.human.escalationLevel.ToString(), Actor.HumanDebug.attacks);
					}
				}
			}
			if (this.preset.cancelIfPersuitTargetNotInRange && !this.goal.aiController.throwActive && !this.IsPersuitTargetCatchable())
			{
				this.Remove(this.preset.repeatDelayOnActionFail);
				return;
			}
			this.goal.aiController.human.AddNourishment(num * this.preset.overtimeNourishment);
			this.goal.aiController.human.AddHydration(num * this.preset.overtimeHydration);
			this.goal.aiController.human.AddAlertness(num * this.preset.overtimeAlertness);
			this.goal.aiController.human.AddEnergy(num * this.preset.overtimeEnergy);
			this.goal.aiController.human.AddExcitement(num * this.preset.overtimeExcitement);
			this.goal.aiController.human.AddChores(num * this.preset.overtimeChores);
			this.goal.aiController.human.AddHygiene(num * this.preset.overtimeHygiene);
			this.goal.aiController.human.AddBladder(num * this.preset.overtimeBladder);
			this.goal.aiController.human.AddHeat(num * this.preset.overtimeHeat);
			this.goal.aiController.human.AddDrunk(num * this.preset.overtimeDrunk);
			this.goal.aiController.human.AddBreath(num * this.preset.overtimeBreath);
			this.goal.aiController.human.AddPoisoned(num * this.preset.overtimePoison, null);
			if (this.preset.completableAction)
			{
				float num3 = 1f;
				if (this.timeThisWillTake > 0f)
				{
					num3 = num / this.timeThisWillTake;
				}
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
					{
						"Add progress ",
						num3.ToString(),
						" to action ",
						this.preset.name,
						"(",
						this.progress.ToString(),
						") time since last update: ",
						num.ToString(),
						" and time this will take: ",
						this.timeThisWillTake.ToString()
					}), Actor.HumanDebug.actions);
				}
				this.progress += num3;
				if (this.preset.completeOnSeeIllegal && this.goal.aiController.persuitTarget != null)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Persuit target " + this.goal.aiController.persuit.ToString() + " active...", Actor.HumanDebug.attacks);
					}
					if (this.goal.aiController.human.seesIllegal.ContainsKey(this.goal.aiController.persuitTarget))
					{
						if (this.goal.aiController.human.seesIllegal[this.goal.aiController.persuitTarget] >= 1f)
						{
							this.progress = 1f;
						}
						else if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug("Persuit target sight progress: " + this.goal.aiController.human.seesIllegal[this.goal.aiController.persuitTarget].ToString(), Actor.HumanDebug.sight);
						}
					}
				}
				this.progress = Mathf.Clamp01(this.progress);
				if (this.preset.useCurrentConsumable && this.goal.aiController.human.currentConsumables.Count > 0 && this.goal.aiController.human.currentConsumables[0].retailItem != null)
				{
					using (List<InteractablePreset>.Enumerator enumerator3 = this.goal.aiController.human.currentConsumables.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							InteractablePreset interactablePreset = enumerator3.Current;
							if (interactablePreset.retailItem != null)
							{
								this.goal.aiController.human.AddNourishment(num3 * interactablePreset.retailItem.nourishment);
								this.goal.aiController.human.AddHydration(num3 * interactablePreset.retailItem.hydration);
								this.goal.aiController.human.AddAlertness(num3 * interactablePreset.retailItem.alertness);
								this.goal.aiController.human.AddEnergy(num3 * interactablePreset.retailItem.energy);
								this.goal.aiController.human.AddExcitement(num3 * interactablePreset.retailItem.excitement);
								this.goal.aiController.human.AddChores(num3 * interactablePreset.retailItem.chores);
								this.goal.aiController.human.AddHygiene(num3 * interactablePreset.retailItem.hygiene);
								this.goal.aiController.human.AddBladder(num3 * interactablePreset.retailItem.bladder);
								this.goal.aiController.human.AddHeat(num3 * interactablePreset.retailItem.heat);
								this.goal.aiController.human.AddDrunk(num3 * interactablePreset.retailItem.drunk);
								this.goal.aiController.human.AddBreath(num3 * interactablePreset.retailItem.breath);
								this.goal.aiController.human.AddPoisoned(num3 * interactablePreset.retailItem.poisoned, null);
								this.goal.aiController.human.AddHealth(num3 * interactablePreset.retailItem.health, true, false);
							}
						}
						goto IL_F04;
					}
				}
				this.goal.aiController.human.AddNourishment(num3 * this.preset.progressNourishment);
				this.goal.aiController.human.AddHydration(num3 * this.preset.progressHydration);
				this.goal.aiController.human.AddAlertness(num3 * this.preset.progressAlertness);
				this.goal.aiController.human.AddEnergy(num3 * this.preset.progressEnergy);
				this.goal.aiController.human.AddExcitement(num3 * this.preset.progressExcitement);
				this.goal.aiController.human.AddChores(num3 * this.preset.progressChores);
				this.goal.aiController.human.AddHygiene(num3 * this.preset.progressHygeiene);
				this.goal.aiController.human.AddBladder(num3 * this.preset.progressBladder);
				this.goal.aiController.human.AddHeat(num3 * this.preset.progressHeat);
				this.goal.aiController.human.AddDrunk(num3 * this.preset.progressDrunk);
				this.goal.aiController.human.AddBreath(num3 * this.preset.progressBreath);
				this.goal.aiController.human.AddPoisoned(num3 * this.preset.progressPoisoned, null);
			}
			IL_F04:
			if (this.preset.whileArrivedBark.Count > 0 && SessionData.Instance.gameTime - this.goal.aiController.human.speechController.lastSpeech > 0.045f && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfWhileArrived && this.goal.aiController.human.speechController.speechQueue.Count <= 0 && (!this.preset.mustSeeOtherCitizen || (this.goal.aiController.currentTrackTarget != null && this.goal.aiController.currentTrackTarget.actor != null && this.goal.aiController.currentTrackTarget.active)) && InterfaceController.Instance.activeSpeechBubbles.Count <= CitizenControls.Instance.maxSpeechBubbles)
			{
				this.goal.aiController.human.speechController.TriggerBark(this.preset.whileArrivedBark[Toolbox.Instance.Rand(0, this.preset.whileArrivedBark.Count, false)]);
			}
			if (this.preset.progressVmailThreads && num > 0.1f)
			{
				bool flag = false;
				foreach (StateSaveData.MessageThreadSave messageThreadSave in this.goal.aiController.human.messageThreadFeatures)
				{
					DDSSaveClasses.DDSTreeSave ddstreeSave = null;
					if (!Toolbox.Instance.allDDSTrees.TryGetValue(messageThreadSave.treeID, ref ddstreeSave))
					{
						Game.LogError("Cannot find vmail tree " + messageThreadSave.treeID, 2);
					}
					else
					{
						string text = ddstreeSave.startingMessage;
						float num4 = -999999f;
						for (int i = 0; i < messageThreadSave.timestamps.Count; i++)
						{
							if (messageThreadSave.timestamps[i] > num4)
							{
								num4 = messageThreadSave.timestamps[i];
								text = messageThreadSave.messages[i];
							}
						}
						DDSSaveClasses.DDSMessageSettings ddsmessageSettings = null;
						if (!ddstreeSave.messageRef.TryGetValue(text, ref ddsmessageSettings))
						{
							Game.LogError("Cannot find message instance ID " + text, 2);
						}
						else
						{
							Human human2 = null;
							CityData.Instance.GetHuman(messageThreadSave.participantA, out human2, true);
							foreach (Human.DDSRank ddsrank in Toolbox.Instance.GetMessageTreeLinkRankings(messageThreadSave, ddsmessageSettings))
							{
								if (ddstreeSave.messageRef.TryGetValue(ddsrank.linkRef.to, ref ddsmessageSettings))
								{
									bool flag2 = false;
									if (ddsmessageSettings.saidBy <= 0 && this.goal.aiController.human.humanID == messageThreadSave.participantA)
									{
										flag2 = true;
									}
									else if (ddsmessageSettings.saidBy == 1 && this.goal.aiController.human.humanID == messageThreadSave.participantB)
									{
										flag2 = true;
									}
									else if (ddsmessageSettings.saidBy == 2 && this.goal.aiController.human.humanID == messageThreadSave.participantC)
									{
										flag2 = true;
									}
									else if (ddsmessageSettings.saidBy == 3 && this.goal.aiController.human.humanID == messageThreadSave.participantD)
									{
										flag2 = true;
									}
									if (flag2)
									{
										num4 = Mathf.Lerp(num4, Mathf.Max(SessionData.Instance.gameTime - 4f, num4), Toolbox.Instance.Rand(0f, 0.5f, false));
										messageThreadSave.messages.Add(ddsrank.linkRef.to);
										messageThreadSave.timestamps.Add(num4);
										if (ddsmessageSettings.saidBy <= 0)
										{
											messageThreadSave.senders.Add(human2.humanID);
										}
										else if (ddsmessageSettings.saidBy == 1)
										{
											messageThreadSave.senders.Add(messageThreadSave.participantB);
										}
										else if (ddsmessageSettings.saidBy == 2)
										{
											messageThreadSave.senders.Add(messageThreadSave.participantC);
										}
										else if (ddsmessageSettings.saidBy == 3)
										{
											messageThreadSave.senders.Add(messageThreadSave.participantD);
										}
										if (ddsmessageSettings.saidTo <= 0)
										{
											messageThreadSave.recievers.Add(human2.humanID);
										}
										else if (ddsmessageSettings.saidTo == 1)
										{
											messageThreadSave.recievers.Add(messageThreadSave.participantB);
										}
										else if (ddsmessageSettings.saidTo == 2)
										{
											messageThreadSave.recievers.Add(messageThreadSave.participantC);
										}
										else if (ddsmessageSettings.saidTo == 3)
										{
											messageThreadSave.recievers.Add(messageThreadSave.participantD);
										}
										Game.Log("Progressed vmail thread!", 2);
										flag = true;
										break;
									}
								}
							}
							if (flag)
							{
								break;
							}
						}
					}
				}
			}
			if (this.progress >= 1f)
			{
				this.Complete();
				return;
			}
		}
		else
		{
			this.DestinationCheck("Tick check", false);
			if (this.goal.aiController.human.isInBed)
			{
				this.goal.aiController.human.SetInBed(false);
			}
			if (this.preset.whileJourneyBark.Count > 0 && SessionData.Instance.gameTime - this.goal.aiController.human.speechController.lastSpeech > 0.03f && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfWhileJourney && InterfaceController.Instance.activeSpeechBubbles.Count <= CitizenControls.Instance.maxSpeechBubbles)
			{
				this.goal.aiController.human.speechController.TriggerBark(this.preset.whileJourneyBark[Toolbox.Instance.Rand(0, this.preset.whileJourneyBark.Count, false)]);
			}
		}
	}

	// Token: 0x06000424 RID: 1060 RVA: 0x0003746C File Offset: 0x0003566C
	public bool InteractableStateCheck(AIActionPreset.CheckActionAgainstState stateCheck)
	{
		if (this.interactable == null)
		{
			return false;
		}
		bool flag = false;
		if (stateCheck.switchState == InteractablePreset.Switch.switchState && this.interactable.sw0 == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.custom1 && this.interactable.sw1 == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.custom2 && this.interactable.sw2 == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.custom3 && this.interactable.sw3 == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.lockState && this.interactable.locked == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.carryPhysicsObject && this.interactable.phy == stateCheck.switchIs)
		{
			flag = true;
		}
		else if (stateCheck.switchState == InteractablePreset.Switch.enforcersInside && this.goal.aiController.human.currentGameLocation != null && this.goal.aiController.human.isHome && this.goal.aiController.human.currentGameLocation.currentOccupants.Exists((Actor item) => item != this.goal.aiController.human && item.isEnforcer && item.isOnDuty))
		{
			flag = true;
		}
		if (flag && Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
			{
				this.interactable.name,
				"'s state ",
				stateCheck.switchState.ToString(),
				" matches the actions: ",
				stateCheck.switchIs.ToString(),
				" outcome: ",
				stateCheck.outcome.ToString()
			}), Actor.HumanDebug.attacks);
		}
		return flag;
	}

	// Token: 0x06000425 RID: 1061 RVA: 0x00037650 File Offset: 0x00035850
	public void LOSCheck()
	{
		if (this.passedInteractable != null && this.passedInteractable.controller != null)
		{
			RaycastHit raycastHit;
			bool flag = (this.passedInteractable.isActor != null && this.goal.aiController.human.currentNode == this.passedInteractable.isActor.currentNode) || this.goal.aiController.human.currentNode == this.passedInteractable.node || Toolbox.Instance.RaycastCheck(this.goal.aiController.human.lookAtThisTransform, this.passedInteractable.controller.transform, 10f, out raycastHit);
			if (flag)
			{
				this.node = this.goal.aiController.human.currentNode;
				this.goal.aiController.DebugDestinationPosition("Set at destination due to LOS");
				this.SetAtDestination(true, false);
			}
		}
	}

	// Token: 0x06000426 RID: 1062 RVA: 0x00037758 File Offset: 0x00035958
	private bool IsPersuitTargetCatchable()
	{
		if (this.goal.aiController.persuitTarget == null)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("...Cancelling as target isn't here (no persuit target)", Actor.HumanDebug.attacks);
			}
			return false;
		}
		if (this.goal.aiController.persuitTarget.isDead || this.goal.aiController.persuitTarget.isStunned)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("...Cancelling as target is dead or stunned", Actor.HumanDebug.attacks);
			}
			return false;
		}
		if (!this.goal.aiController.human.seenIllegalThisCheck.Contains(this.goal.aiController.persuitTarget) && this.goal.aiController.persuitChaseLogicUses <= 0f)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("...Cancelling as target isn't here (unseen this check)", Actor.HumanDebug.attacks);
			}
			return false;
		}
		float num = Vector3.Distance(this.goal.aiController.persuitTarget.transform.position, this.goal.aiController.transform.position);
		if (num > this.goal.aiController.weaponRangeMax || num < this.goal.aiController.currentWeaponPreset.minimumRange)
		{
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("...Cancelling as target isn't here (too near/far away: " + num.ToString() + ")", Actor.HumanDebug.attacks);
			}
			return false;
		}
		if (this.goal.aiController.persuitTarget.isHiding && this.goal.aiController.persuitTarget.isPlayer && Player.Instance.spottedWhileHiding.Contains(this.goal.aiController.human))
		{
			this.goal.InsertPlayerHidingPlaceRemoval();
			return false;
		}
		return true;
	}

	// Token: 0x06000427 RID: 1063 RVA: 0x00037960 File Offset: 0x00035B60
	public void SetAtDestination(bool val, bool forceUpdate = false)
	{
		if (this.isAtLocation != val || forceUpdate)
		{
			this.isAtLocation = val;
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Set at destination: " + this.isAtLocation.ToString(), Actor.HumanDebug.actions);
				this.goal.aiController.DebugDestinationPosition("Set at destination: " + this.isAtLocation.ToString());
			}
			if (this.preset.limitTickRate)
			{
				this.goal.aiController.UpdateTickRate(false);
			}
			if (!this.isAtLocation)
			{
				this.goal.aiController.SetUpdateEnabled(true);
			}
			else
			{
				this.TriggerArrivalSound();
				if (this.preset.specificOutfitOnArrive)
				{
					this.goal.aiController.human.outfitController.SetCurrentOutfit(this.preset.allowedOutfitOnArrive, false, false, true);
				}
				else if (this.preset.makeClothedOnActivate)
				{
					this.goal.aiController.human.outfitController.MakeClothed();
				}
				if (this.preset.setExpressionOnArrive)
				{
					this.goal.aiController.SetExpression(this.preset.arriveExpression);
				}
				this.goal.aiController.UpdateHeldItems(AIActionPreset.ActionStateFlag.onArrival);
				if (this.preset.changeIdleOnArrival && !this.goal.aiController.restrained)
				{
					if (this.interactable != null && this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.forceStanding)
					{
						this.goal.aiController.human.animationController.SetIdleAnimationState(CitizenAnimationController.IdleAnimationState.none);
					}
					else
					{
						this.goal.aiController.human.animationController.SetIdleAnimationState(this.preset.idleAnimationOnArrival);
					}
				}
				if (this.preset.changeArmsOnArrival && !this.goal.aiController.restrained)
				{
					this.goal.aiController.human.animationController.SetArmsBoolState(this.preset.armsAnimationOnArrival);
				}
				this.goal.aiController.human.UpdateCurrentNodeSpace();
				this.dontUpdateGoalPriorityForExtraTime = (float)this.preset.dontUpdateGoalPriorityFor / 60f;
				this.lastRecordedTickWhileAtDesitnation = SessionData.Instance.gameTime;
				this.arrivedAtDestination = SessionData.Instance.gameTime;
				if (this.preset.lying)
				{
					this.goal.aiController.human.SetInBed(true);
				}
				if (this.estimatedArrival > -1f)
				{
					Toolbox.Instance.AddToTravelTimeRecords(this.goal.aiController.human, this.estimatedArrival - this.arrivedAtDestination);
				}
				if (this.preset.attackPersuitTargetOnProximity && this.goal.aiController.persuitTarget != null && !this.goal.aiController.attackActive)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(this.debug + " attempt attack at destination...", Actor.HumanDebug.attacks);
					}
					if (this.goal.aiController.seesOnPersuit && !this.goal.aiController.persuitTarget.isDead && !this.goal.aiController.persuitTarget.isStunned && this.goal.aiController.human.escalationLevel >= 2)
					{
						float num = Vector3.Distance(this.goal.aiController.persuitTarget.transform.position, this.goal.aiController.transform.position);
						if (num <= this.goal.aiController.weaponRangeMax && num >= this.goal.aiController.currentWeaponPreset.minimumRange)
						{
							this.goal.aiController.StartAttack(this.goal.aiController.persuitTarget);
						}
						else
						{
							if (Game.Instance.collectDebugData)
							{
								if (num > this.goal.aiController.weaponRangeMax)
								{
									this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
									{
										this.debug,
										" too far to use weapon (",
										num.ToString(),
										"/",
										this.goal.aiController.weaponRangeMax.ToString(),
										")"
									}), Actor.HumanDebug.attacks);
								}
								else if (num < this.goal.aiController.currentWeaponPreset.minimumRange)
								{
									this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
									{
										this.debug,
										" too close to use weapon (",
										num.ToString(),
										"/",
										this.goal.aiController.currentWeaponPreset.minimumRange.ToString(),
										")"
									}), Actor.HumanDebug.attacks);
								}
							}
							if (this.preset.throwObjectsAtTarget && !this.goal.preset.disableThrowing && this.goal.aiController.killerForMurders.Count <= 0 && num <= CitizenControls.Instance.throwMaxRange && num >= CitizenControls.Instance.throwMinRange && (this.goal.aiController.human.currentConsumables.Count > 0 || this.goal.aiController.human.trash.Count > 0))
							{
								this.goal.aiController.ThrowObject(this.goal.aiController.persuitTarget);
							}
						}
					}
					else if (Game.Instance.collectDebugData)
					{
						if (this.goal.aiController.persuitTarget.isDead)
						{
							this.goal.aiController.human.SelectedDebug(this.debug + " Target is dead", Actor.HumanDebug.attacks);
						}
						if (this.goal.aiController.persuitTarget.isStunned)
						{
							this.goal.aiController.human.SelectedDebug(this.debug + " Target is stunned", Actor.HumanDebug.attacks);
						}
						if (!this.goal.aiController.seesOnPersuit)
						{
							this.goal.aiController.human.SelectedDebug(this.debug + " SeesOnPersuit is not true", Actor.HumanDebug.attacks);
						}
						if (this.goal.aiController.human.escalationLevel < 2)
						{
							this.goal.aiController.human.SelectedDebug(this.debug + " Escalation level is too low: " + this.goal.aiController.human.escalationLevel.ToString(), Actor.HumanDebug.attacks);
						}
					}
				}
				if (this.preset.cancelIfPersuitTargetNotInRange && !this.goal.aiController.throwActive && !this.IsPersuitTargetCatchable())
				{
					this.Remove(this.preset.repeatDelayOnActionFail);
					return;
				}
				if (this.preset.sleepOnArrival)
				{
					this.goal.aiController.human.GoToSleep();
				}
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug("Attempting to set arrival direction: " + this.preset.facing.ToString(), Actor.HumanDebug.movement);
				}
				if (this.preset.facing == AIActionPreset.ActionFacingDirection.towardsDestination)
				{
					if (this.goal.aiController.currentDestinationNode != null)
					{
						this.goal.aiController.SetFaceTravelDirection();
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.awayFromDestination)
				{
					if (this.goal.aiController.currentDestinationNode != null)
					{
						this.goal.aiController.SetFacingPosition(-this.goal.aiController.currentDestinationPositon);
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.interactable)
				{
					if (this.interactable != null)
					{
						this.goal.aiController.SetFacingPosition(this.interactable.usagePoint.GetUsageWorldPosition(this.node.position, this.goal.aiController.human));
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.InverseInteractable)
				{
					if (this.interactable != null)
					{
						this.goal.aiController.SetFacingPosition(-this.interactable.usagePoint.GetUsageWorldPosition(this.node.position, this.goal.aiController.human));
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.player)
				{
					this.goal.aiController.SetFacingPosition(Player.Instance.transform.position);
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.sniperVantagePoint)
				{
					if (this.vantagePoint != null)
					{
						this.goal.aiController.SetFacingPosition(this.vantagePoint.position);
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.awayFromSniperVantagePoint)
				{
					if (this.vantagePoint != null)
					{
						this.goal.aiController.SetFacingPosition(-this.vantagePoint.position);
					}
				}
				else if (this.preset.facing == AIActionPreset.ActionFacingDirection.victim)
				{
					if (this.goal.murderRef != null && this.goal.murderRef.victim != null)
					{
						this.goal.aiController.SetFacingPosition(this.goal.murderRef.victim.lookAtThisTransform.position);
					}
				}
				else
				{
					if (this.preset.facing == AIActionPreset.ActionFacingDirection.door)
					{
						using (Dictionary<NewNode, NewNode.NodeAccess>.Enumerator enumerator = this.node.accessToOtherNodes.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair = enumerator.Current;
								if (keyValuePair.Value.accessType == NewNode.NodeAccess.AccessType.door && keyValuePair.Value.door != null && (keyValuePair.Value.door.doorInteractable == this.passedInteractable || keyValuePair.Value.door.handleInteractable == this.passedInteractable))
								{
									NewNode key = keyValuePair.Key;
									this.goal.aiController.SetFacingPosition(key.position);
									break;
								}
							}
							goto IL_C9A;
						}
					}
					if (this.preset.facing == AIActionPreset.ActionFacingDirection.accessableDirection)
					{
						List<NewNode> list = new List<NewNode>();
						if (this.node != null)
						{
							foreach (KeyValuePair<NewNode, NewNode.NodeAccess> keyValuePair2 in this.node.accessToOtherNodes)
							{
								if (keyValuePair2.Value.walkingAccess && !keyValuePair2.Value.toNode.noPassThrough && (keyValuePair2.Value.accessType != NewNode.NodeAccess.AccessType.door || (keyValuePair2.Value.door != null && !keyValuePair2.Value.door.isClosed)))
								{
									list.Add(keyValuePair2.Key);
								}
							}
						}
						if (list.Count > 0)
						{
							this.goal.aiController.SetFacingPosition(list[Toolbox.Instance.Rand(0, list.Count, false)].position);
						}
					}
					else if (this.preset.facing == AIActionPreset.ActionFacingDirection.investigate)
					{
						if (this.goal.aiController.seesOnPersuit && this.goal.aiController.persuitTarget != null)
						{
							this.goal.aiController.SetFacingPosition(this.goal.aiController.persuitTarget.transform.position);
						}
						else
						{
							this.goal.aiController.SetFacingPosition(this.goal.aiController.investigatePositionProjection);
						}
					}
					else if (this.preset.facing == AIActionPreset.ActionFacingDirection.interactableSetting)
					{
						if (this.usagePoint != null)
						{
							this.goal.aiController.SetFacingPosition(this.usagePoint.worldLookAtPoint);
						}
					}
					else if (this.preset.facing == AIActionPreset.ActionFacingDirection.inverseInteractableSetting && this.usagePoint != null)
					{
						this.goal.aiController.SetFacingPosition(-this.usagePoint.worldLookAtPoint);
					}
				}
				IL_C9A:
				if (this.preset.onArrivalBark.Count > 0 && Toolbox.Instance.Rand(0f, 1f, false) <= this.preset.chanceOfOnArrival && InterfaceController.Instance.activeSpeechBubbles.Count <= CitizenControls.Instance.maxSpeechBubbles)
				{
					this.goal.aiController.human.speechController.TriggerBark(this.preset.onArrivalBark[Toolbox.Instance.Rand(0, this.preset.onArrivalBark.Count, false)]);
				}
				if (this.usagePoint != null)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug("Setting usage point to me " + this.goal.aiController.human.name + "...", Actor.HumanDebug.actions);
					}
					this.usagePoint.TrySetUser(this.preset.usageSlot, this.goal.aiController.human, "Set on destination: " + this.preset.name);
				}
				if (this.interactable != null)
				{
					this.ExecuteAdditionalActions(ref this.preset.forcedActionsOnArrival);
				}
				if (this.passedInteractable != null && this.preset.actionLocation == AIActionPreset.ActionLocation.putDownInteractable && this.bestPlacement != null)
				{
					if (this.bestPlacement != null)
					{
						this.passedInteractable.SetAsNotInventory(this.node);
						bool relocateAuthority = Toolbox.Instance.GetRelocateAuthority(this.goal.aiController.human, this.passedInteractable);
						this.passedInteractable.ConvertToFurnitureSpawnedObject(this.bestPlacement.furnParent, this.bestPlacement.location, true, relocateAuthority);
						if (!this.goal.aiController.putDownItems.Contains(this.passedInteractable))
						{
							this.goal.aiController.putDownItems.Add(this.passedInteractable);
						}
					}
					else
					{
						Game.Log("AI Error: No best placement for putting down " + this.passedInteractable.name, 2);
					}
				}
				if (this.passedInteractable != null && this.preset.actionLocation == AIActionPreset.ActionLocation.pickUpInteractable)
				{
					if (this.passedInteractable.furnitureParent != null)
					{
						foreach (Interactable interactable in this.passedInteractable.furnitureParent.integratedInteractables)
						{
							interactable.AddNewDynamicFingerprint(this.goal.aiController.human, Interactable.PrintLife.timed);
							interactable.AddNewDynamicFingerprint(this.goal.aiController.human, Interactable.PrintLife.timed);
							interactable.AddNewDynamicFingerprint(this.goal.aiController.human, Interactable.PrintLife.timed);
						}
					}
					this.passedInteractable.SetInInventory(this.goal.aiController.human);
					this.goal.aiController.putDownItems.Remove(this.passedInteractable);
				}
			}
			this.UpdateCombatPose();
		}
	}

	// Token: 0x06000428 RID: 1064 RVA: 0x00038934 File Offset: 0x00036B34
	public void OnInvalidMovement(int attemptNumber)
	{
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("Action: OnInvalidMovement", Actor.HumanDebug.movement);
		}
	}

	// Token: 0x06000429 RID: 1065 RVA: 0x0003895D File Offset: 0x00036B5D
	public void ImmediateComplete()
	{
		if (this.preset.completableAction)
		{
			this.Complete();
		}
	}

	// Token: 0x0600042A RID: 1066 RVA: 0x00038974 File Offset: 0x00036B74
	public Interactable InteractablePicker(ref List<Interactable> opt, Vector3 currentWorldPosition, bool useSocialRules, out NewNode useNode, out Interactable.UsagePoint usePoint, GroupsController.SocialGroup meetingGroup = null, bool useDistance = false, bool useDistanceIfInSameAddress = true, List<Interactable> ignore = null)
	{
		useNode = null;
		usePoint = null;
		if (Game.Instance.collectDebugData)
		{
			Actor human = this.goal.aiController.human;
			string text = "Picking interactable from pool of ";
			int count = opt.Count;
			human.SelectedDebug(text + count.ToString() + "...", Actor.HumanDebug.actions);
		}
		Interactable.UsagePoint usagePoint = null;
		float num = -99999f;
		foreach (Interactable interactable in opt)
		{
			if (!interactable.originalPosition && interactable.wo)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(interactable.name + " not at original postion...", Actor.HumanDebug.actions);
				}
			}
			else if (ignore != null && ignore.Contains(interactable))
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(interactable.name + " on ignore list...", Actor.HumanDebug.actions);
				}
			}
			else if (interactable.node == null)
			{
				if (Game.Instance.collectDebugData)
				{
					this.goal.aiController.human.SelectedDebug(interactable.name + " no valid node location...", Actor.HumanDebug.actions);
				}
			}
			else if (interactable.node.accessToOtherNodes.Count <= 0)
			{
				string[] array = new string[5];
				array[0] = "Interactable ";
				array[1] = interactable.name;
				array[2] = " at ";
				int num2 = 3;
				Vector3 wPos = interactable.wPos;
				array[num2] = wPos.ToString();
				array[4] = " has no access...";
				Game.LogError(string.Concat(array), 2);
			}
			else
			{
				Human human2 = null;
				interactable.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human2);
				if (human2 != null && human2 != this.goal.aiController.human)
				{
					if (Game.Instance.collectDebugData)
					{
						this.goal.aiController.human.SelectedDebug(interactable.name + " being used already by " + human2.name, Actor.HumanDebug.actions);
					}
				}
				else
				{
					if (interactable.usagePoint.reserved != null)
					{
						if (SessionData.Instance.decimalClock >= interactable.usagePoint.reserved.decimalStartTime + Toolbox.Instance.groupsDictionary[interactable.usagePoint.reserved.preset].meetUpLength)
						{
							interactable.usagePoint.SetReserved(null);
						}
						else if (!this.goal.aiController.human.groups.Contains(interactable.usagePoint.reserved) && meetingGroup != interactable.usagePoint.reserved)
						{
							if (Game.Instance.collectDebugData)
							{
								Actor human3 = this.goal.aiController.human;
								string text2 = interactable.name;
								string text3 = " reserved by ";
								GroupsController.SocialGroup reserved = interactable.usagePoint.reserved;
								human3.SelectedDebug(text2 + text3 + ((reserved != null) ? reserved.ToString() : null), Actor.HumanDebug.actions);
								continue;
							}
							continue;
						}
					}
					string empty = string.Empty;
					int count;
					if (!this.goal.preset.allowTrespass && this.goal.aiController.human.IsTrespassing(interactable.node.room, out count, out empty, this.goal.preset.allowEnforcersEverywhere))
					{
						if (Game.Instance.collectDebugData)
						{
							this.goal.aiController.human.SelectedDebug(interactable.name + " no valid authority: " + empty, Actor.HumanDebug.actions);
						}
					}
					else
					{
						List<Human> list = new List<Human>();
						int num3 = 0;
						int num4 = 0;
						foreach (Interactable interactable2 in interactable.node.interactables)
						{
							if (!interactable2.preset.disableForSocialGroups && opt.Contains(interactable2))
							{
								Human human4 = null;
								if (interactable2.usagePoint.TryGetUserAtSlot(this.preset.usageSlot, out human4))
								{
									list.Add(human4);
								}
								else
								{
									num4++;
								}
								num3++;
							}
						}
						float num5 = (float)interactable.preset.AIPriority;
						float num6 = interactable.preset.pickDistanceMultiplier;
						if (interactable.preset.perActionPrioritySettings.Count > 0)
						{
							InteractablePreset.AIUsePriority aiusePriority = interactable.preset.perActionPrioritySettings.Find((InteractablePreset.AIUsePriority item) => item.actions.Contains(this.preset));
							if (aiusePriority != null)
							{
								num5 = aiusePriority.AIPriority;
								num6 = aiusePriority.pickDistanceMultiplier;
							}
						}
						float num7 = num5 + Toolbox.Instance.Rand(0f, 1f, false);
						if (useSocialRules)
						{
							if (meetingGroup != null)
							{
								GroupPreset groupPreset = Toolbox.Instance.groupsDictionary[meetingGroup.preset];
								num6 += groupPreset.useDistanceMultiplierModifier;
								if (interactable.usagePoint.reserved == meetingGroup)
								{
									num7 += 200f;
									if (Game.Instance.collectDebugData)
									{
										this.goal.aiController.human.SelectedDebug("Found reservation for group at " + interactable.name + " id " + interactable.id.ToString(), Actor.HumanDebug.actions);
									}
								}
								int num8 = 0;
								foreach (int id in meetingGroup.members)
								{
									Human human5 = null;
									if (CityData.Instance.GetHuman(id, out human5, true) && list.Contains(human5) && human5.ai != null && human5.ai.currentAction != null && human5.ai.currentAction.interactable != interactable)
									{
										num7 += 200f;
										if (Game.Instance.collectDebugData)
										{
											this.goal.aiController.human.SelectedDebug(string.Concat(new string[]
											{
												"Found matching group memeber ",
												human5.name,
												" using ",
												human5.ai.currentAction.interactable.name,
												" id ",
												human5.ai.currentAction.interactable.id.ToString(),
												" local to ",
												interactable.name,
												" id ",
												interactable.id.ToString()
											}), Actor.HumanDebug.actions);
										}
										num8++;
									}
								}
								if (num8 <= 0 && !interactable.preset.disableForSocialGroups)
								{
									if (list.Count <= 0)
									{
										if (Game.Instance.collectDebugData)
										{
											this.goal.aiController.human.SelectedDebug(interactable.name + " " + interactable.id.ToString() + " has no other users...", Actor.HumanDebug.actions);
										}
										num7 += (float)Mathf.Min(num3, meetingGroup.members.Count) * 5f;
									}
									if (num4 >= meetingGroup.members.Count)
									{
										if (Game.Instance.collectDebugData)
										{
											Actor human6 = this.goal.aiController.human;
											string[] array2 = new string[5];
											array2[0] = interactable.name;
											array2[1] = " ";
											array2[2] = interactable.id.ToString();
											array2[3] = " has enough free seats for my group of ";
											int num9 = 4;
											count = meetingGroup.members.Count;
											array2[num9] = count.ToString();
											human6.SelectedDebug(string.Concat(array2), Actor.HumanDebug.actions);
										}
										num7 += (float)Mathf.Min(num4, meetingGroup.members.Count) * 5f;
									}
								}
							}
							else
							{
								foreach (Human findC in list)
								{
									Acquaintance acquaintance = null;
									if (this.goal.aiController.human.FindAcquaintanceExists(findC, out acquaintance))
									{
										num7 += (acquaintance.known - 0.2f + (acquaintance.like - 0.2f)) * 10f;
									}
									num7 += (this.goal.aiController.human.extraversion - 0.75f) * 5f;
								}
							}
						}
						if (useDistance || (useDistanceIfInSameAddress && this.goal.aiController.human.currentGameLocation == interactable.node.gameLocation))
						{
							num7 -= Vector3.Distance(interactable.wPos, currentWorldPosition) * 0.2f * num6;
						}
						if (usagePoint == null || num7 > num)
						{
							if (Game.Instance.collectDebugData)
							{
								Actor human7 = this.goal.aiController.human;
								string[] array3 = new string[11];
								array3[0] = "BEST YET: Evaluate interactable ";
								array3[1] = interactable.name;
								array3[2] = " ";
								array3[3] = interactable.id.ToString();
								array3[4] = " on ";
								int num10 = 5;
								Vector3Int nodeCoord = interactable.node.nodeCoord;
								array3[num10] = nodeCoord.ToString();
								array3[6] = " in room ";
								array3[7] = interactable.node.room.name;
								array3[8] = ": ";
								array3[9] = num7.ToString();
								array3[10] = "...";
								human7.SelectedDebug(string.Concat(array3), Actor.HumanDebug.actions);
							}
							usagePoint = interactable.usagePoint;
							num = num7;
						}
						else if (Game.Instance.collectDebugData)
						{
							Actor human8 = this.goal.aiController.human;
							string[] array4 = new string[11];
							array4[0] = "Evaluate interactable ";
							array4[1] = interactable.name;
							array4[2] = " ";
							array4[3] = interactable.id.ToString();
							array4[4] = " on ";
							int num11 = 5;
							Vector3Int nodeCoord = interactable.node.nodeCoord;
							array4[num11] = nodeCoord.ToString();
							array4[6] = " in room ";
							array4[7] = interactable.node.room.name;
							array4[8] = ": ";
							array4[9] = num7.ToString();
							array4[10] = "...";
							human8.SelectedDebug(string.Concat(array4), Actor.HumanDebug.actions);
						}
					}
				}
			}
		}
		if (usagePoint != null)
		{
			if (usagePoint.interactable.node.accessToOtherNodes.Count <= 0)
			{
				Game.LogError("Picked a node with no access. Something is wrong here...", 2);
			}
			if (Game.Instance.collectDebugData)
			{
				this.goal.aiController.human.SelectedDebug("Chosen interactable " + usagePoint.interactable.name + " id " + usagePoint.interactable.id.ToString(), Actor.HumanDebug.actions);
			}
			usePoint = usagePoint;
			useNode = usePoint.node;
			if (useSocialRules && meetingGroup != null && Toolbox.Instance.groupsDictionary[meetingGroup.preset].reserveSeats)
			{
				foreach (Interactable interactable3 in meetingGroup.reserved)
				{
					interactable3.usagePoint.reserved = null;
				}
				foreach (Interactable interactable4 in usePoint.node.interactables)
				{
					if (opt.Contains(interactable4))
					{
						interactable4.usagePoint.SetReserved(meetingGroup);
						if (meetingGroup.reserved == null)
						{
							meetingGroup.reserved = new List<Interactable>();
						}
						meetingGroup.reserved.Add(interactable4);
					}
				}
			}
			return usePoint.interactable;
		}
		if (Game.Instance.collectDebugData)
		{
			this.goal.aiController.human.SelectedDebug("No valid options for interactable", Actor.HumanDebug.actions);
		}
		return null;
	}

	// Token: 0x0400038E RID: 910
	public string name = "Action";

	// Token: 0x0400038F RID: 911
	[Header("Action Variables")]
	[NonSerialized]
	public NewAIGoal goal;

	// Token: 0x04000390 RID: 912
	public AIActionPreset preset;

	// Token: 0x04000391 RID: 913
	[Tooltip("Is this action currently active?")]
	public bool isActive;

	// Token: 0x04000392 RID: 914
	public bool completed;

	// Token: 0x04000393 RID: 915
	public bool repeat;

	// Token: 0x04000394 RID: 916
	[NonSerialized]
	public bool checkedForInsertions;

	// Token: 0x04000395 RID: 917
	public bool insertedAction;

	// Token: 0x04000396 RID: 918
	[ReadOnly]
	public int insertedActionPriority = 3;

	// Token: 0x04000397 RID: 919
	[Header("Location")]
	public NewNode node;

	// Token: 0x04000398 RID: 920
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x04000399 RID: 921
	[NonSerialized]
	public Interactable.UsagePoint usagePoint;

	// Token: 0x0400039A RID: 922
	[Tooltip("Is the citizen at the correct location?")]
	public bool isAtLocation;

	// Token: 0x0400039B RID: 923
	public PathFinder.PathData path;

	// Token: 0x0400039C RID: 924
	[NonSerialized]
	public Interactable passedInteractable;

	// Token: 0x0400039D RID: 925
	public NewRoom passedRoom;

	// Token: 0x0400039E RID: 926
	public NewNode forcedNode;

	// Token: 0x0400039F RID: 927
	[NonSerialized]
	public GroupsController.SocialGroup passedGroup;

	// Token: 0x040003A0 RID: 928
	[NonSerialized]
	public bool forceRun;

	// Token: 0x040003A1 RID: 929
	public float estimatedArrival = -1f;

	// Token: 0x040003A2 RID: 930
	public float arrivedAtDestination;

	// Token: 0x040003A3 RID: 931
	private bool actionCheckRecursion;

	// Token: 0x040003A4 RID: 932
	private NewGameLocation.ObjectPlacement bestPlacement;

	// Token: 0x040003A5 RID: 933
	public List<InteractablePreset> passedAcquireItems;

	// Token: 0x040003A6 RID: 934
	public NewWall vantagePoint;

	// Token: 0x040003A7 RID: 935
	[Header("Audio")]
	[NonSerialized]
	public AudioController.LoopingSoundInfo audioLoop;

	// Token: 0x040003A8 RID: 936
	[Header("Progress")]
	[NonSerialized]
	public float lastRecordedTickWhileAtDesitnation;

	// Token: 0x040003A9 RID: 937
	public float timeThisWillTake;

	// Token: 0x040003AA RID: 938
	public float progress;

	// Token: 0x040003AB RID: 939
	[NonSerialized]
	public float dontUpdateGoalPriorityForExtraTime;

	// Token: 0x040003AC RID: 940
	public float createdAt;

	// Token: 0x040003AD RID: 941
	[Header("Debug")]
	public string debug;

	// Token: 0x040003AE RID: 942
	[Space(7f)]
	public InteractableController debugPassedInteractable;

	// Token: 0x040003AF RID: 943
	public NewRoom debugPassedRoom;

	// Token: 0x040003B0 RID: 944
	public bool debugForcedNode;

	// Token: 0x040003B1 RID: 945
	public Vector3 debugForcedNodeWorldPos = Vector3.zero;

	// Token: 0x040003B2 RID: 946
	public List<Interactable> debugPickupInteractable = new List<Interactable>();

	// Token: 0x040003B3 RID: 947
	[Space(7f)]
	public InteractableController debugInteractableController;

	// Token: 0x040003B4 RID: 948
	public Vector3 debugInteractionUsagePosition;
}
