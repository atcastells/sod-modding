using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000231 RID: 561
public class AudioController : MonoBehaviour
{
	// Token: 0x1700006C RID: 108
	// (get) Token: 0x06000CAB RID: 3243 RVA: 0x000B2786 File Offset: 0x000B0986
	public static AudioController Instance
	{
		get
		{
			return AudioController._instance;
		}
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x000B2790 File Offset: 0x000B0990
	private void Awake()
	{
		if (AudioController._instance != null && AudioController._instance != this)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			AudioController._instance = this;
		}
		this.updateAmbientZonesAction = (Action)Delegate.Combine(this.updateAmbientZonesAction, new Action(this.UpdateAmbientZones));
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x000B27F8 File Offset: 0x000B09F8
	private void Start()
	{
		this.footstepLayerMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.castAllExcept, new int[]
		{
			2,
			18,
			20,
			24,
			30
		});
		foreach (AmbientZone newPreset in Toolbox.Instance.allAmbientZones)
		{
			this.ambientZones.Add(new AudioController.AmbientZoneInstance(newPreset));
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x000B287C File Offset: 0x000B0A7C
	public void UpdateMixing()
	{
		if (Player.Instance.inAirVent)
		{
			this.UpdateVentIndoorOutdoor();
			this.UpdateDistanceToVent();
		}
		foreach (AudioController.AmbientZoneInstance ambientZoneInstance in this.ambientZones)
		{
			bool flag = false;
			if (ambientZoneInstance.actualVolume < ambientZoneInstance.desiredVolume)
			{
				ambientZoneInstance.actualVolume += Time.deltaTime * (float)this.updateMixing;
				ambientZoneInstance.actualVolume = Mathf.Min(ambientZoneInstance.actualVolume, ambientZoneInstance.desiredVolume);
				flag = true;
			}
			else if (ambientZoneInstance.actualVolume > ambientZoneInstance.desiredVolume)
			{
				ambientZoneInstance.actualVolume -= Time.deltaTime * (float)this.updateMixing;
				ambientZoneInstance.actualVolume = Mathf.Max(ambientZoneInstance.actualVolume, ambientZoneInstance.desiredVolume);
				flag = true;
			}
			if (flag)
			{
				if (ambientZoneInstance.actualVolume <= 0.01f)
				{
					if (ambientZoneInstance.eventData != null)
					{
						ambientZoneInstance.eventData.UpdatePlayState();
						if (ambientZoneInstance.eventData.state == null || ambientZoneInstance.eventData.state == 3 || ambientZoneInstance.eventData.state == 1)
						{
							this.StopSound(ambientZoneInstance.eventData.audioEvent, AudioController.StopType.immediate);
						}
					}
				}
				else
				{
					bool flag2 = false;
					if (ambientZoneInstance.eventData != null)
					{
						ambientZoneInstance.eventData.UpdatePlayState();
						if (ambientZoneInstance.eventData.state == 2 || ambientZoneInstance.eventData.state == 4)
						{
							this.StopSound(ambientZoneInstance.eventData.audioEvent, AudioController.StopType.immediate);
							ambientZoneInstance.eventData = null;
							flag2 = true;
						}
						else
						{
							ambientZoneInstance.eventData.audioEvent.setVolume(ambientZoneInstance.actualVolume);
						}
					}
					if (ambientZoneInstance.eventData == null || flag2)
					{
						ambientZoneInstance.eventData = this.Play2DLooping(ambientZoneInstance.preset.mainEvent, null, ambientZoneInstance.actualVolume);
					}
				}
			}
			if (ambientZoneInstance.eventData != null && (ambientZoneInstance.eventData.state == null || ambientZoneInstance.eventData.state == 3 || ambientZoneInstance.eventData.state == 1))
			{
				bool flag3 = false;
				if (ambientZoneInstance.preset.passWalla)
				{
					if (ambientZoneInstance.audibleRoom != null)
					{
						int num = 0;
						foreach (Actor actor in CityData.Instance.visibleActors)
						{
							if (!actor.isDead && !actor.isAsleep && !actor.isStunned && !actor.isPlayer && !(actor.lookAtThisTransform == null) && actor.isOnStreet == Player.Instance.isOnStreet && Vector3.Distance(CameraController.Instance.cam.transform.position, actor.lookAtThisTransform.position) <= ambientZoneInstance.preset.maxWallaRange)
							{
								num++;
							}
						}
						ambientZoneInstance.desiredWalla = Mathf.Clamp01((float)num / ambientZoneInstance.preset.maxWallaCrowd);
					}
					else
					{
						ambientZoneInstance.desiredWalla = 0f;
					}
					if (ambientZoneInstance.actualWalla < ambientZoneInstance.desiredWalla)
					{
						ambientZoneInstance.actualWalla += Time.deltaTime * (float)this.updateMixing * 0.33f;
						ambientZoneInstance.actualWalla = Mathf.Min(ambientZoneInstance.actualWalla, ambientZoneInstance.desiredWalla);
						flag3 = true;
					}
					else if (ambientZoneInstance.actualWalla > ambientZoneInstance.desiredWalla)
					{
						ambientZoneInstance.actualWalla -= Time.deltaTime * (float)this.updateMixing * 0.33f;
						ambientZoneInstance.actualWalla = Mathf.Max(ambientZoneInstance.actualWalla, ambientZoneInstance.desiredWalla);
						flag3 = true;
					}
				}
				if (flag3 || ambientZoneInstance.preset.passTimeOfDay)
				{
					if (ambientZoneInstance.preset.passTimeOfDay)
					{
						ambientZoneInstance.eventData.audioEvent.setParameterByName("TimeOfDay", SessionData.Instance.dayProgress, false);
					}
					if (ambientZoneInstance.preset.passWalla)
					{
						ambientZoneInstance.eventData.audioEvent.setParameterByName("Walla", ambientZoneInstance.actualWalla, false);
					}
				}
				if (ambientZoneInstance.preset.passPlayerInVent)
				{
					float num2 = 0f;
					if (Player.Instance.inAirVent)
					{
						num2 = 1f;
					}
					ambientZoneInstance.eventData.audioEvent.setParameterByName("PlayerInVent", num2, false);
				}
				if (ambientZoneInstance.preset.passPlayerVentExtInt)
				{
					ambientZoneInstance.eventData.audioEvent.setParameterByName("PlayerVentExtInt", AudioController.Instance.ventOutdoorsIndoors, false);
				}
				if (ambientZoneInstance.preset.passDistanceToVent)
				{
					ambientZoneInstance.eventData.audioEvent.setParameterByName("DistanceToVent", AudioController.Instance.nearbyVent, false);
				}
				if (ambientZoneInstance.preset.passRain)
				{
					ambientZoneInstance.eventData.audioEvent.setParameterByName("Rain", SessionData.Instance.currentRain, false);
				}
				if (ambientZoneInstance.preset.passBasement)
				{
					float num3 = 0f;
					if (Player.Instance.currentNode != null && Player.Instance.currentNode.nodeCoord.z < 0)
					{
						num3 = Mathf.Clamp01(Mathf.Abs(Player.Instance.transform.position.y) / 9.9f);
					}
					ambientZoneInstance.eventData.audioEvent.setParameterByName("Basement", num3, false);
				}
				if (ambientZoneInstance.preset.passHeightWindSpeed)
				{
					float num4 = Mathf.Clamp01(Player.Instance.transform.position.y * 0.04f);
					float num5 = Mathf.Clamp01(Mathf.Max(SessionData.Instance.currentWind, num4));
					ambientZoneInstance.eventData.audioEvent.setParameterByName("HeightWindSpeed", num5, false);
				}
				if (ambientZoneInstance.preset.passEdgeDistance)
				{
					ambientZoneInstance.eventData.audioEvent.setParameterByName("EdgeDistance", this.edgeDistanceNormalized, false);
				}
			}
		}
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x000B2E90 File Offset: 0x000B1090
	public void StartAmbienceTracks()
	{
		if (this.ambienceWind != null)
		{
			this.StopSound(this.ambienceWind, AudioController.StopType.immediate, "Init ambience");
		}
		if (this.ambienceRain != null)
		{
			this.StopSound(this.ambienceRain, AudioController.StopType.immediate, "Init ambience");
		}
		if (this.ambiencePA != null)
		{
			this.StopSound(this.ambiencePA, AudioController.StopType.immediate, "Init ambience");
		}
		this.ambienceWind = this.Play2DLooping(AudioControls.Instance.ambienceWind, null, 1f);
		this.ambienceRain = this.Play2DLooping(AudioControls.Instance.ambienceRain, null, 1f);
		this.ambiencePA = this.Play2DLooping(AudioControls.Instance.ambiencePA, null, 1f);
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x000B2F40 File Offset: 0x000B1140
	public bool PlayWorldFootstep(AudioEvent eventPreset, Actor actor, bool rightFoot = false)
	{
		if (eventPreset == null)
		{
			return false;
		}
		if (!SessionData.Instance.play)
		{
			return false;
		}
		if (SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.normal)
		{
			return false;
		}
		if (actor.currentCityTile == null)
		{
			return false;
		}
		if (!actor.currentCityTile.isInPlayerVicinity)
		{
			return false;
		}
		if (!SessionData.Instance.startedGame)
		{
			return false;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return false;
		}
		if (eventPreset.disabled)
		{
			return false;
		}
		if (eventPreset.isLicensed && !Game.Instance.allowLicensedMusic)
		{
			return false;
		}
		if (actor != null && actor.transform.position.y < CityControls.Instance.basementWaterLevel)
		{
			if (actor.isPlayer)
			{
				eventPreset = AudioControls.Instance.playerWaterWade;
				Player.Instance.AddHygiene(-0.05f);
			}
			else
			{
				eventPreset = AudioControls.Instance.footstepWaterWade;
			}
		}
		if (eventPreset.guid.Length <= 0)
		{
			Game.LogError("Invalid footstep GUID for " + eventPreset.name, 2);
			return false;
		}
		GUID guid = GUID.Parse(eventPreset.guid);
		EventDescription eventDescription;
		RuntimeManager.StudioSystem.getEventByID(guid, ref eventDescription);
		float num = 0f;
		float num2;
		eventDescription.getMinMaxDistance(ref num2, ref num);
		float num3 = 1f;
		int num4 = 0;
		if (!actor.isPlayer && Vector3.Distance(Player.Instance.transform.position, actor.transform.position) > num)
		{
			return false;
		}
		AudioController.SoundMaterialOverride soundMaterialOverride = null;
		if (actor.isPlayer || (actor.currentNode != null && actor.currentNode.individualFurniture.Count > 0 && actor.visible))
		{
			Vector3 vector = Vector3.zero;
			if (rightFoot)
			{
				vector = actor.transform.TransformPoint(new Vector3(0.13f, 0.1f, 0f));
			}
			else
			{
				vector = actor.transform.TransformPoint(new Vector3(-0.13f, 0.1f, 0f));
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(vector, Vector3.down, ref raycastHit, 2.6f, this.footstepLayerMask, 1))
			{
				MaterialOverrideController component = raycastHit.transform.GetComponent<MaterialOverrideController>();
				if (component != null)
				{
					soundMaterialOverride = new AudioController.SoundMaterialOverride(component.concrete, component.wood, component.carpet, component.tile, component.plaster, component.fabric, component.metal, component.glass);
				}
				else
				{
					InteractableController component2 = raycastHit.transform.GetComponent<InteractableController>();
					if (component2 != null && component2.interactable != null && component2.interactable.preset.useMaterialOverride)
					{
						soundMaterialOverride = component2.interactable.preset.materialOverride;
					}
					else
					{
						MeshFilter component3 = raycastHit.transform.GetComponent<MeshFilter>();
						if (component3 != null)
						{
							FurniturePreset furnitureFromMesh = Toolbox.Instance.GetFurnitureFromMesh(component3.sharedMesh);
							if (furnitureFromMesh != null)
							{
								soundMaterialOverride = new AudioController.SoundMaterialOverride(furnitureFromMesh.concrete, furnitureFromMesh.wood, furnitureFromMesh.carpet, furnitureFromMesh.tile, furnitureFromMesh.plaster, furnitureFromMesh.fabric, furnitureFromMesh.metal, furnitureFromMesh.glass);
							}
						}
					}
				}
			}
		}
		if (soundMaterialOverride == null)
		{
			soundMaterialOverride = new AudioController.SoundMaterialOverride(actor.currentRoom.floorMaterial.concrete, actor.currentRoom.floorMaterial.wood, actor.currentRoom.floorMaterial.carpet, actor.currentRoom.floorMaterial.tile, actor.currentRoom.floorMaterial.plaster, actor.currentRoom.floorMaterial.fabric, actor.currentRoom.floorMaterial.metal, actor.currentRoom.floorMaterial.glass);
		}
		if (!eventPreset.disableOcclusion)
		{
			List<AudioController.ActiveListener> list = null;
			bool flag = false;
			List<NewRoom> list2;
			num3 = this.GetOcculusion(Player.Instance.currentNode, actor.currentNode, eventPreset, num3, actor, soundMaterialOverride, out num4, out list, out flag, out list2, out num2, null, false);
			if (flag && actor != null)
			{
				foreach (AudioController.ActiveListener activeListener in list)
				{
					if (activeListener.listener != actor)
					{
						activeListener.listener.HearIllegal(eventPreset, actor.currentNode, actor.currentNode.position, actor, activeListener.escalationLevel);
					}
				}
			}
		}
		float num5 = 0f;
		float num6 = 0f;
		float num7 = 0f;
		float num8 = 0f;
		float num9 = 0f;
		if (actor.isOnStreet)
		{
			num7 = SessionData.Instance.citySnow;
		}
		if (actor.isRunning)
		{
			num9 = 1f;
		}
		if (actor.isPlayer)
		{
			num3 = 1f;
			num4 = 0;
			if (Toolbox.Instance.Rand(0f, 1f, false) < StatusController.Instance.tripChanceDrunk * 0.02f)
			{
				Player.Instance.Trip(Toolbox.Instance.Rand(0.05f, 0.1f, false), false, true);
			}
			else if (Player.Instance.isRunning && (soundMaterialOverride.tile >= 0.8f || soundMaterialOverride.metal >= 0.8f || soundMaterialOverride.glass >= 0.8f) && Toolbox.Instance.Rand(0f, 1f, false) < StatusController.Instance.tripChanceWet * 0.03f)
			{
				Player.Instance.Trip(Toolbox.Instance.Rand(0.05f, 0.1f, false), false, true);
			}
			if (Player.Instance.isOnStreet)
			{
				num5 = SessionData.Instance.cityWetness;
			}
			num6 = Player.Instance.wet;
			if (Player.Instance.isCrouched)
			{
				num8 = 1f;
			}
		}
		if (num3 <= 0.01f)
		{
			return false;
		}
		EventInstance eventInstance = RuntimeManager.CreateInstance(guid);
		eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(actor.footstepSoundTransform.gameObject));
		eventInstance.setParameterByName("Occlusion", (float)num4, false);
		eventInstance.setParameterByName("Concrete", soundMaterialOverride.concrete, false);
		eventInstance.setParameterByName("Wood", soundMaterialOverride.wood, false);
		eventInstance.setParameterByName("Carpet", soundMaterialOverride.carpet, false);
		eventInstance.setParameterByName("Tile", soundMaterialOverride.tile, false);
		eventInstance.setParameterByName("Plaster", soundMaterialOverride.plaster, false);
		eventInstance.setParameterByName("Fabric", soundMaterialOverride.fabric, false);
		eventInstance.setParameterByName("Metal", soundMaterialOverride.metal, false);
		eventInstance.setParameterByName("Glass", soundMaterialOverride.glass, false);
		eventInstance.setParameterByName("CityWetness", num5, false);
		eventInstance.setParameterByName("CharacterWetness", num6, false);
		eventInstance.setParameterByName("CitySnow", num7, false);
		eventInstance.setParameterByName("Crouching", num8, false);
		eventInstance.setParameterByName("Running", num9, false);
		if (actor.isRunning)
		{
			eventInstance.setParameterByName("Stealth", -1f, false);
		}
		else if (!actor.stealthMode)
		{
			eventInstance.setParameterByName("Stealth", 0f, false);
		}
		else
		{
			eventInstance.setParameterByName("Stealth", 1f, false);
		}
		eventInstance.start();
		eventInstance.setVolume(num3);
		if (!actor.isPlayer && (Player.Instance.illegalStatus || Player.Instance.witnessesToIllegalActivity.Contains(actor)))
		{
			bool flag2 = false;
			eventInstance.isVirtual(ref flag2);
			if (!flag2 && this.GetPlayersSoundLevel(actor.currentNode, eventPreset, num3, soundMaterialOverride) > this.playerHearingThreshold)
			{
				actor.HeardByPlayer();
			}
		}
		eventInstance.release();
		if (soundMaterialOverride != null)
		{
			soundMaterialOverride = null;
		}
		return true;
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x000B3710 File Offset: 0x000B1910
	public void PlayerPlayerImpactSound(float fallCount)
	{
		Game.Log("Audio: Playing player impact sound with param; " + fallCount.ToString(), 2);
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.normal)
		{
			return;
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return;
		}
		AudioController.SoundMaterialOverride soundMaterialOverride = null;
		RaycastHit raycastHit;
		if (Physics.Raycast(Player.Instance.transform.TransformPoint(new Vector3(0f, 0.1f, 0f)), Vector3.down, ref raycastHit, 2.6f, this.footstepLayerMask, 1))
		{
			Game.Log("Audio: Player impact sound raycast hits: " + raycastHit.transform.name, 2);
			MaterialOverrideController component = raycastHit.transform.GetComponent<MaterialOverrideController>();
			if (component != null)
			{
				soundMaterialOverride = new AudioController.SoundMaterialOverride(component.concrete, component.wood, component.carpet, component.tile, component.plaster, component.fabric, component.metal, component.glass);
			}
			else
			{
				InteractableController component2 = raycastHit.transform.GetComponent<InteractableController>();
				if (component2 != null && component2.interactable != null && component2.interactable.preset.useMaterialOverride)
				{
					soundMaterialOverride = component2.interactable.preset.materialOverride;
				}
				else
				{
					MeshFilter component3 = raycastHit.transform.GetComponent<MeshFilter>();
					if (component3 != null)
					{
						FurniturePreset furnitureFromMesh = Toolbox.Instance.GetFurnitureFromMesh(component3.sharedMesh);
						if (furnitureFromMesh != null)
						{
							soundMaterialOverride = new AudioController.SoundMaterialOverride(furnitureFromMesh.concrete, furnitureFromMesh.wood, furnitureFromMesh.carpet, furnitureFromMesh.tile, furnitureFromMesh.plaster, furnitureFromMesh.fabric, furnitureFromMesh.metal, furnitureFromMesh.glass);
						}
					}
				}
			}
		}
		else
		{
			Game.Log("Audio: Player impact sound raycast does not hit anything, using room materials instead...", 2);
		}
		if (soundMaterialOverride == null)
		{
			soundMaterialOverride = new AudioController.SoundMaterialOverride(Player.Instance.currentRoom.floorMaterial.concrete, Player.Instance.currentRoom.floorMaterial.wood, Player.Instance.currentRoom.floorMaterial.carpet, Player.Instance.currentRoom.floorMaterial.tile, Player.Instance.currentRoom.floorMaterial.plaster, Player.Instance.currentRoom.floorMaterial.fabric, Player.Instance.currentRoom.floorMaterial.metal, Player.Instance.currentRoom.floorMaterial.glass);
		}
		AudioEvent eventPreset;
		if (soundMaterialOverride.concrete >= 0.5f)
		{
			eventPreset = AudioControls.Instance.playerLandImpactConcrete;
		}
		else if (soundMaterialOverride.metal >= 0.5f)
		{
			eventPreset = AudioControls.Instance.playerLandImpactMetal;
		}
		else
		{
			if (soundMaterialOverride.wood < 0.5f)
			{
				return;
			}
			eventPreset = AudioControls.Instance.playerLandImpactWood;
		}
		AudioController.FMODParam fmodparam = new AudioController.FMODParam
		{
			name = "Fall",
			value = fallCount
		};
		List<AudioController.FMODParam> list = new List<AudioController.FMODParam>();
		list.Add(fmodparam);
		this.PlayWorldOneShot(eventPreset, Player.Instance, Player.Instance.currentNode, Player.Instance.footstepSoundTransform.position + new Vector3(0f, 0.1f, 0f), null, list, 1f, null, false, soundMaterialOverride, false);
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x000B3A70 File Offset: 0x000B1C70
	public EventInstance PlayWorldOneShot(AudioEvent eventPreset, Actor who, NewNode location, Vector3 worldPosition, Interactable interactable = null, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f, List<NewNode> additionalSources = null, bool forceIgnoreOcclusion = false, AudioController.SoundMaterialOverride surfaceData = null, bool forceSuspicious = false)
	{
		if (eventPreset == null || (eventPreset.guid.Length <= 0 && !eventPreset.isDummyEvent) || SessionData.Instance.currentTimeSpeed != SessionData.TimeSpeed.normal || !SessionData.Instance.startedGame || (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive))
		{
			EventInstance result = default(EventInstance);
			result.release();
			return result;
		}
		if (eventPreset.disabled)
		{
			EventInstance result2 = default(EventInstance);
			result2.release();
			return result2;
		}
		if (eventPreset.isLicensed && !Game.Instance.allowLicensedMusic)
		{
			EventInstance result3 = default(EventInstance);
			result3.release();
			return result3;
		}
		if (location == null)
		{
			if (PathFinder.Instance.nodeMap.TryGetValue(CityData.Instance.RealPosToNodeInt(worldPosition), ref location))
			{
				if (eventPreset.debug && location.room != null)
				{
					string[] array = new string[7];
					array[0] = "Audio: Node was not passed to audio controller, but one was found using world position (";
					int num = 1;
					Vector3 vector = worldPosition;
					array[num] = vector.ToString();
					array[2] = " = ";
					array[3] = location.room.GetName();
					array[4] = ": ";
					int num2 = 5;
					vector = location.position;
					array[num2] = vector.ToString();
					array[6] = ")";
					Game.Log(string.Concat(array), 2);
				}
			}
			else if (eventPreset.debug)
			{
				Game.Log("Audio: Node was not passed to audio controller, and unable to find one using world position so continuing without...", 2);
			}
		}
		if (location != null && location.tile != null && location.tile.cityTile != null && !location.tile.cityTile.isInPlayerVicinity)
		{
			return default(EventInstance);
		}
		GUID guid = default(GUID);
		if (!eventPreset.isDummyEvent)
		{
			guid = GUID.Parse(eventPreset.guid);
		}
		float num3 = volumeOverride;
		int num4 = 0;
		if (location == null)
		{
			Game.Log("Audio: Ignoring occlusion for " + eventPreset.name + " as no location was passed.", 2);
		}
		List<AudioController.ActiveListener> list = new List<AudioController.ActiveListener>();
		if (!forceIgnoreOcclusion && !eventPreset.disableOcclusion && location != null)
		{
			bool flag = false;
			List<NewRoom> list2;
			float num5;
			num3 = this.GetOcculusion(Player.Instance.currentNode, location, eventPreset, num3, who, surfaceData, out num4, out list, out flag, out list2, out num5, additionalSources, forceSuspicious);
			if (who != null && flag)
			{
				foreach (AudioController.ActiveListener activeListener in list)
				{
					if (activeListener.listener != who)
					{
						activeListener.listener.HearIllegal(eventPreset, location, location.position, who, activeListener.escalationLevel);
					}
				}
			}
			if (eventPreset.debug)
			{
				Game.Log(string.Concat(new string[]
				{
					"Audio: Occlusion value for ",
					eventPreset.name,
					" is ",
					num3.ToString(),
					" with penetration ",
					num4.ToString()
				}), 2);
			}
		}
		if (num3 <= 0.01f)
		{
			EventInstance result4 = default(EventInstance);
			result4.release();
			return result4;
		}
		EventInstance result5;
		if (!eventPreset.isDummyEvent)
		{
			result5 = RuntimeManager.CreateInstance(guid);
		}
		else
		{
			result5 = default(EventInstance);
		}
		result5.set3DAttributes(RuntimeUtils.To3DAttributes(worldPosition));
		result5.setParameterByName("Occlusion", (float)num4, false);
		if (who != null)
		{
			if (who.isRunning)
			{
				result5.setParameterByName("Stealth", -1f, false);
			}
			else if (!who.stealthMode)
			{
				result5.setParameterByName("Stealth", 0f, false);
			}
			else
			{
				result5.setParameterByName("Stealth", 1f, false);
			}
		}
		if (parameters != null)
		{
			foreach (AudioController.FMODParam fmodparam in parameters)
			{
				result5.setParameterByName(fmodparam.name, fmodparam.value, false);
			}
		}
		if (surfaceData != null)
		{
			result5.setParameterByName("Concrete", surfaceData.concrete, false);
			result5.setParameterByName("Wood", surfaceData.wood, false);
			result5.setParameterByName("Carpet", surfaceData.carpet, false);
			result5.setParameterByName("Tile", surfaceData.tile, false);
			result5.setParameterByName("Plaster", surfaceData.plaster, false);
			result5.setParameterByName("Fabric", surfaceData.fabric, false);
			result5.setParameterByName("Metal", surfaceData.metal, false);
			result5.setParameterByName("Glass", surfaceData.glass, false);
		}
		if (!eventPreset.isDummyEvent)
		{
			result5.start();
			result5.setVolume(Mathf.Clamp01(num3 * eventPreset.masterVolumeScale));
		}
		if (eventPreset.debug)
		{
			string[] array2 = new string[7];
			array2[0] = "Audio: Playing ";
			array2[1] = eventPreset.name;
			array2[2] = " with occluded volume of ";
			int num6 = 3;
			float num5 = Mathf.Clamp01(num3 * eventPreset.masterVolumeScale);
			array2[num6] = num5.ToString();
			array2[4] = " and ";
			array2[5] = list.Count.ToString();
			array2[6] = " active listeners";
			Game.Log(string.Concat(array2), 2);
		}
		if (who != null && !who.isPlayer && location != null && (Player.Instance.illegalStatus || Player.Instance.witnessesToIllegalActivity.Contains(who)))
		{
			bool flag2 = false;
			result5.isVirtual(ref flag2);
			if (!flag2)
			{
				float playersSoundLevel = this.GetPlayersSoundLevel(location, eventPreset, num3, null);
				if (playersSoundLevel > this.playerHearingThreshold)
				{
					Game.Log(string.Concat(new string[]
					{
						"Audio: Heard by player: ",
						who.name,
						"(",
						eventPreset.name,
						") passing through ",
						num4.ToString(),
						" occ units, sound level of ",
						playersSoundLevel.ToString()
					}), 2);
					who.HeardByPlayer();
				}
			}
		}
		result5.release();
		if (surfaceData != null)
		{
			surfaceData = null;
		}
		return result5;
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x000B4080 File Offset: 0x000B2280
	public void PlayOneShotDelayed(float delay, AudioEvent eventPreset, Actor who, NewNode location, Vector3 worldPosition, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f, List<NewNode> additionalSources = null, bool forceIgnoreOcclusion = false)
	{
		if (!SessionData.Instance.startedGame || (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive))
		{
			return;
		}
		this.delayedSound.Add(new AudioController.DelayedSoundInfo(delay, eventPreset, who, location, worldPosition, parameters, volumeOverride, additionalSources, forceIgnoreOcclusion, false));
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x000B40D4 File Offset: 0x000B22D4
	public AudioController.LoopingSoundInfo PlayWorldLooping(AudioEvent eventPreset, Actor who, Interactable interactable, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f, bool forceSuspicious = false, bool pauseWhenGamePaused = false, SessionData.TelevisionChannel isBroadcast = null, InteractablePreset.IfSwitchStateSFX newSwitchInfo = null)
	{
		return this.PlayWorldLooping(eventPreset, who, interactable.node, interactable.wPos, interactable, parameters, volumeOverride, forceSuspicious, pauseWhenGamePaused, isBroadcast, newSwitchInfo);
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x000B4104 File Offset: 0x000B2304
	public AudioController.LoopingSoundInfo PlayWorldLoopingStatic(AudioEvent eventPreset, Actor who, NewNode worldNode, Vector3 worldPos, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f, bool forceSuspicious = false, bool pauseWhenGamePaused = false, SessionData.TelevisionChannel isBroadcast = null, InteractablePreset.IfSwitchStateSFX newSwitchInfo = null)
	{
		return this.PlayWorldLooping(eventPreset, who, worldNode, worldPos, null, parameters, volumeOverride, forceSuspicious, pauseWhenGamePaused, isBroadcast, newSwitchInfo);
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x000B412C File Offset: 0x000B232C
	public AudioController.LoopingSoundInfo PlayWorldLooping(AudioEvent eventPreset, Actor who, NewNode worldNode, Vector3 worldPosition, Interactable interactable = null, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f, bool forceSuspicious = false, bool pauseWhenGamePaused = false, SessionData.TelevisionChannel isBroadcast = null, InteractablePreset.IfSwitchStateSFX newSwitchInfo = null)
	{
		if (eventPreset == null || eventPreset.guid.Length <= 0)
		{
			string text = "Audio: PlayWorldLooping: Null or invalid preset: ";
			AudioEvent audioEvent = eventPreset;
			Game.Log(text + ((audioEvent != null) ? audioEvent.ToString() : null), 2);
			return null;
		}
		if (eventPreset.disabled)
		{
			return null;
		}
		if (eventPreset.isLicensed && !Game.Instance.allowLicensedMusic)
		{
			return null;
		}
		if (eventPreset.debug)
		{
			Game.Log("Audio: Playing new world looping sound " + eventPreset.name + "...", 2);
		}
		if (isBroadcast != null && isBroadcast.currentShow != null)
		{
			eventPreset = isBroadcast.currentShow.audioEvent;
		}
		if (worldNode == null)
		{
			Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(worldPosition);
			PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref worldNode);
		}
		string text2 = string.Empty;
		if (Game.Instance.devMode)
		{
			text2 = eventPreset.name;
			if (worldNode != null)
			{
				string[] array = new string[7];
				array[0] = text2;
				array[1] = " ";
				array[2] = worldNode.room.name;
				array[3] = ", ";
				array[4] = worldNode.room.gameLocation.name;
				array[5] = " ";
				int num = 6;
				Vector3 vector = worldPosition;
				array[num] = vector.ToString();
				text2 = string.Concat(array);
			}
		}
		AudioController.LoopingSoundInfo loopingSoundInfo = new AudioController.LoopingSoundInfo
		{
			name = text2,
			volumeOverride = volumeOverride,
			eventPreset = eventPreset,
			sourceLocation = worldNode,
			who = who,
			forceSuspicious = forceSuspicious,
			init = true,
			worldPos = worldPosition,
			isBroadcast = isBroadcast,
			interactableLoopInfo = newSwitchInfo,
			interactable = interactable
		};
		this.loopingSounds.Add(loopingSoundInfo);
		loopingSoundInfo.UpdateOcclusion(true);
		return loopingSoundInfo;
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x000B42E0 File Offset: 0x000B24E0
	public AudioController.LoopingSoundInfo Play2DLooping(AudioEvent eventPreset, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f)
	{
		if (eventPreset == null || eventPreset.guid.Length <= 0)
		{
			Game.Log("Audio: Play2DLooping: No preset", 2);
			return null;
		}
		if (eventPreset.disabled)
		{
			return null;
		}
		if (eventPreset.isLicensed && !Game.Instance.allowLicensedMusic)
		{
			return null;
		}
		if (eventPreset.debug)
		{
			Game.Log("Audio: Playing new 2D looping sound: " + eventPreset.name, 2);
		}
		GUID guid = GUID.Parse(eventPreset.guid);
		EventDescription description;
		RuntimeManager.StudioSystem.getEventByID(guid, ref description);
		EventInstance audioEvent = RuntimeManager.CreateInstance(guid);
		if (parameters != null)
		{
			foreach (AudioController.FMODParam fmodparam in parameters)
			{
				audioEvent.setParameterByName(fmodparam.name, fmodparam.value, false);
			}
		}
		audioEvent.start();
		audioEvent.setVolume(Mathf.Clamp01(eventPreset.masterVolumeScale * volumeOverride));
		AudioController.LoopingSoundInfo loopingSoundInfo = new AudioController.LoopingSoundInfo
		{
			name = eventPreset.name,
			audioEvent = audioEvent,
			description = description,
			eventPreset = eventPreset,
			init = true
		};
		this.loopingSounds.Add(loopingSoundInfo);
		return loopingSoundInfo;
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x000B4424 File Offset: 0x000B2624
	public void UpdateAllLoopingSoundOcclusion()
	{
		if (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive)
		{
			return;
		}
		for (int i = 0; i < this.loopingSounds.Count; i++)
		{
			AudioController.LoopingSoundInfo loopingSoundInfo = this.loopingSounds[i];
			if (!loopingSoundInfo.isActive)
			{
				loopingSoundInfo.isValid = loopingSoundInfo.audioEvent.isValid();
				loopingSoundInfo.UpdatePlayState();
				if (loopingSoundInfo.state == 2 || !loopingSoundInfo.isValid)
				{
					if (loopingSoundInfo.eventPreset.forceOutlineForLoopIfPlayerTrespassing)
					{
						this.ForceOutlineCheck(loopingSoundInfo.eventPreset, loopingSoundInfo.interactable, true);
					}
					if (loopingSoundInfo.isValid)
					{
						loopingSoundInfo.audioEvent.release();
					}
					this.loopingSounds.RemoveAt(i);
					i--;
				}
			}
			else
			{
				this.loopingSounds[i].UpdateOcclusion(false);
			}
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x000B4500 File Offset: 0x000B2700
	public void UpdateClosestWindowAndDoor(bool doorCheckOnly = false)
	{
		NewWall newWall = null;
		if (!doorCheckOnly)
		{
			this.windowAudioPosition = Vector3.zero;
			this.closestWindowDistance = 9999f;
		}
		NewNode.NodeAccess nodeAccess = null;
		this.doorAudioPosition = Vector3.zero;
		this.closestDoorDistance = 9999f;
		if (SessionData.Instance.startedGame)
		{
			foreach (NewRoom newRoom in CityData.Instance.visibleRooms)
			{
				if (!doorCheckOnly)
				{
					foreach (NewWall newWall2 in newRoom.windows)
					{
						if (newWall2.node.gameLocation == Player.Instance.currentGameLocation || newWall2.otherWall.node.gameLocation == Player.Instance.currentGameLocation)
						{
							float num = Vector3.Distance(Player.Instance.transform.position, newWall2.position);
							if (newWall == null || num < this.closestWindowDistance)
							{
								newWall = newWall2;
								this.windowAudioPosition = newWall2.position;
								this.closestWindowDistance = num;
							}
						}
					}
				}
				if (Player.Instance.currentNode != null && !Player.Instance.currentNode.isOutside)
				{
					using (List<NewNode.NodeAccess>.Enumerator enumerator3 = newRoom.entrances.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							NewNode.NodeAccess nodeAccess2 = enumerator3.Current;
							if (nodeAccess2.accessType == NewNode.NodeAccess.AccessType.door && nodeAccess2.door != null && !nodeAccess2.door.isClosed && (nodeAccess2.fromNode.isOutside || nodeAccess2.toNode.isOutside) && (nodeAccess2.fromNode.gameLocation == Player.Instance.currentGameLocation || nodeAccess2.toNode.gameLocation == Player.Instance.currentGameLocation))
							{
								float num2 = Vector3.Distance(Player.Instance.transform.position, nodeAccess2.worldAccessPoint) * (2f - this.openMultiplierCurve.Evaluate(Mathf.Abs(nodeAccess2.door.ajarProgress)));
								if (nodeAccess == null || num2 < this.closestDoorDistance)
								{
									nodeAccess = nodeAccess2;
									this.doorAudioPosition = nodeAccess2.worldAccessPoint;
									this.closestDoorDistance = num2;
								}
							}
						}
						goto IL_270;
					}
					goto IL_265;
				}
				goto IL_265;
				IL_270:
				if (newRoom.footprintUpdateQueued)
				{
					newRoom.UpdateFootprints(false);
					continue;
				}
				continue;
				IL_265:
				this.closestDoorDistance = 0f;
				goto IL_270;
			}
		}
		if (!doorCheckOnly)
		{
			this.PassWindowDistance();
		}
		this.PassDistanceFromExternalDoor();
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x000B4804 File Offset: 0x000B2A04
	public void UpdateDistanceFromEdge()
	{
		this.edgeDistance = 9999f;
		if (SessionData.Instance.startedGame && CityData.Instance.borderBlock != null)
		{
			foreach (CityTile cityTile in CityData.Instance.borderBlock.cityTiles)
			{
				float num = Mathf.Max(Vector3.Distance(Player.Instance.transform.position, cityTile.transform.position) - 25f, 0f);
				if (num < this.edgeDistance)
				{
					this.edgeDistance = num;
				}
			}
		}
		this.edgeDistanceNormalized = Mathf.Clamp01(this.edgeDistance / this.edgeDistanceMultiplier);
		this.PassEdgeDistance();
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x000B48E4 File Offset: 0x000B2AE4
	public void PassWindowDistance()
	{
		this.closestWindowDistanceNormalized = Mathf.Clamp01(this.closestWindowDistance / this.closestWindowDistanceMultiplier);
		if (this.ambienceRain != null)
		{
			this.ambienceRain.audioEvent.setParameterByName("WindowDistance", this.closestWindowDistanceNormalized, false);
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.audioEvent.setParameterByName("WindowDistance", this.closestWindowDistanceNormalized, false);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.audioEvent.setParameterByName("WindowDistance", this.closestWindowDistanceNormalized, false);
		}
		foreach (NewBuilding newBuilding in GameplayController.Instance.activeAlarmsBuildings)
		{
			newBuilding.UpdateAlarmPAWindowDistance(this.closestWindowDistanceNormalized);
		}
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x000B49C8 File Offset: 0x000B2BC8
	public void PassDistanceFromExternalDoor()
	{
		this.closestDoorDistanceNormalized = Mathf.Clamp01(this.closestDoorDistance / this.closestDoorDistanceMultiplier);
		if (this.ambienceRain != null)
		{
			this.ambienceRain.audioEvent.setParameterByName("ExternalDoorDistance", this.closestDoorDistanceNormalized, false);
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.audioEvent.setParameterByName("ExternalDoorDistance", this.closestDoorDistanceNormalized, false);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.audioEvent.setParameterByName("ExternalDoorDistance", this.closestDoorDistanceNormalized, false);
		}
		foreach (NewBuilding newBuilding in GameplayController.Instance.activeAlarmsBuildings)
		{
			newBuilding.UpdateAlarmPAExternalDoorDistance(this.closestDoorDistanceNormalized);
		}
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x000B4AAC File Offset: 0x000B2CAC
	[Button(null, 0)]
	public void PassWeatherParams()
	{
		if (this.ambienceRain != null)
		{
			this.passedRain = SessionData.Instance.currentRain;
			if (this.passedRain > 0f)
			{
				this.ambienceRain.UpdatePlayState();
				if (this.ambienceRain.state == 2 || this.ambienceRain.state == 4)
				{
					this.ambienceRain.audioEvent.start();
				}
			}
			this.ambienceRain.audioEvent.setParameterByName("Rain", this.passedRain, false);
			this.ambienceRain.audioEvent.setParameterByName("CityWetness", SessionData.Instance.cityWetness, false);
		}
		if (this.ambienceWind != null)
		{
			this.passedWind = SessionData.Instance.currentWind;
			if (this.passedWind > 0f)
			{
				this.ambienceWind.UpdatePlayState();
				if (this.ambiencePA != null)
				{
					this.ambiencePA.UpdatePlayState();
				}
				if (this.ambienceWind.state == 2 || this.ambienceWind.state == 4)
				{
					this.ambienceWind.audioEvent.start();
					if (this.ambiencePA != null)
					{
						this.ambiencePA.audioEvent.start();
					}
				}
			}
			this.ambienceWind.audioEvent.setParameterByName("Wind", this.passedWind, false);
			this.ambienceWind.audioEvent.setParameterByName("EdgeDistance", this.edgeDistanceNormalized, false);
			if (this.ambiencePA != null)
			{
				this.ambiencePA.audioEvent.setParameterByName("Wind", this.passedWind, false);
				this.ambiencePA.audioEvent.setParameterByName("EdgeDistance", this.edgeDistanceNormalized, false);
			}
		}
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x000B4C64 File Offset: 0x000B2E64
	public void PassIndoorOutdoor()
	{
		int num = 0;
		if (Player.Instance.currentNode == null || !Player.Instance.currentNode.isOutside || Player.Instance.inAirVent)
		{
			num = 1;
		}
		if (this.ambienceRain != null)
		{
			this.ambienceRain.audioEvent.setParameterByName("Interior", (float)num, false);
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.audioEvent.setParameterByName("Interior", (float)num, false);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.audioEvent.setParameterByName("Interior", (float)num, false);
		}
		foreach (NewBuilding newBuilding in GameplayController.Instance.activeAlarmsBuildings)
		{
			newBuilding.UpdateAlarmPAIntExt((float)num);
		}
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x000B4D4C File Offset: 0x000B2F4C
	public void UpdateVentIndoorOutdoor()
	{
		if (!Player.Instance.inAirVent)
		{
			this.ventOutdoorsIndoors = 1f;
			return;
		}
		if (!(Player.Instance.currentDuct != null) || Player.Instance.currentDuctSection == null)
		{
			this.ventOutdoorsIndoors = 1f;
			return;
		}
		if (Player.Instance.currentDuctSection.ext)
		{
			this.ventOutdoorsIndoors = 0f;
			return;
		}
		this.ventOutdoorsIndoors = 1f;
		int num = 16;
		int num2 = 9999;
		Dictionary<AirDuctGroup.AirDuctSection, int> dictionary = new Dictionary<AirDuctGroup.AirDuctSection, int>();
		dictionary.Add(Player.Instance.currentDuctSection, 0);
		HashSet<AirDuctGroup.AirDuctSection> hashSet = new HashSet<AirDuctGroup.AirDuctSection>();
		while (num > 0 && dictionary.Count > 0)
		{
			int num3 = 99999;
			AirDuctGroup.AirDuctSection airDuctSection = null;
			foreach (KeyValuePair<AirDuctGroup.AirDuctSection, int> keyValuePair in dictionary)
			{
				if (keyValuePair.Value < num3)
				{
					airDuctSection = keyValuePair.Key;
					num3 = keyValuePair.Value;
				}
			}
			if (airDuctSection == null)
			{
				break;
			}
			if (airDuctSection.ext)
			{
				num2 = num3;
				break;
			}
			List<Vector3Int> list;
			List<AirDuctGroup.AirVent> list2;
			List<Vector3Int> list3;
			foreach (AirDuctGroup.AirDuctSection airDuctSection2 in airDuctSection.GetNeighborSections(out list, out list2, out list3))
			{
				if (!hashSet.Contains(airDuctSection2) && !dictionary.ContainsKey(airDuctSection2))
				{
					dictionary.Add(airDuctSection2, num3 + 1);
				}
			}
			hashSet.Add(airDuctSection);
			dictionary.Remove(airDuctSection);
			num--;
		}
		this.ventOutdoorsIndoors = Mathf.Clamp01((float)num2 / 5f);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x000B4F0C File Offset: 0x000B310C
	public void UpdateDistanceToVent()
	{
		this.nearbyVent = 0f;
		if (!Player.Instance.inAirVent)
		{
			this.nearbyVent = 0f;
			return;
		}
		if (Player.Instance.currentDuct != null && Player.Instance.currentDuctSection != null)
		{
			int num = 16;
			int num2 = 9999;
			Dictionary<AirDuctGroup.AirDuctSection, int> dictionary = new Dictionary<AirDuctGroup.AirDuctSection, int>();
			dictionary.Add(Player.Instance.currentDuctSection, 0);
			HashSet<AirDuctGroup.AirDuctSection> hashSet = new HashSet<AirDuctGroup.AirDuctSection>();
			while (num > 0 && dictionary.Count > 0)
			{
				int num3 = 99999;
				AirDuctGroup.AirDuctSection airDuctSection = null;
				foreach (KeyValuePair<AirDuctGroup.AirDuctSection, int> keyValuePair in dictionary)
				{
					if (keyValuePair.Value < num3)
					{
						airDuctSection = keyValuePair.Key;
						num3 = keyValuePair.Value;
					}
				}
				if (airDuctSection == null)
				{
					break;
				}
				List<AirDuctGroup.AirVent> list = new List<AirDuctGroup.AirVent>();
				List<Vector3Int> list2;
				List<Vector3Int> list3;
				List<AirDuctGroup.AirDuctSection> neighborSections = airDuctSection.GetNeighborSections(out list2, out list, out list3);
				if (airDuctSection.peekSection || list.Count > 0)
				{
					num2 = num3;
					break;
				}
				foreach (AirDuctGroup.AirDuctSection airDuctSection2 in neighborSections)
				{
					if (!hashSet.Contains(airDuctSection2) && !dictionary.ContainsKey(airDuctSection2))
					{
						dictionary.Add(airDuctSection2, num3 + 1);
					}
				}
				hashSet.Add(airDuctSection);
				dictionary.Remove(airDuctSection);
				num--;
			}
			this.nearbyVent = Mathf.Clamp01((float)num2 / 5f);
			return;
		}
		this.nearbyVent = 0f;
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x00002265 File Offset: 0x00000465
	public void PassTimeOfDay()
	{
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x000B50C4 File Offset: 0x000B32C4
	public void PassEdgeDistance()
	{
		this.edgeDistanceNormalized = Mathf.Clamp01(this.edgeDistance / this.edgeDistanceMultiplier);
		if (this.ambienceWind != null)
		{
			this.ambienceWind.audioEvent.setParameterByName("EdgeDistance", this.edgeDistanceNormalized, false);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.audioEvent.setParameterByName("EdgeDistance", this.edgeDistanceNormalized, false);
		}
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x000B5134 File Offset: 0x000B3334
	public void UpdateClosestExteriorWall()
	{
		this.extWallDistance = 99999f;
		if (SessionData.Instance.startedGame && Player.Instance.isOnStreet && Player.Instance.currentRoom != null)
		{
			foreach (NewNode newNode in Player.Instance.currentRoom.nodes)
			{
				foreach (NewWall newWall in newNode.walls)
				{
					if (newWall.node.isOutside || newWall.node.room.IsOutside())
					{
						float num = Vector3.Distance(Player.Instance.transform.position, newWall.position);
						if (num < this.extWallDistance)
						{
							this.extWallDistance = num;
						}
					}
				}
			}
			foreach (NewRoom newRoom in Player.Instance.currentRoom.adjacentRooms)
			{
				foreach (NewNode newNode2 in newRoom.nodes)
				{
					foreach (NewWall newWall2 in newNode2.walls)
					{
						if (newWall2.node.isOutside || newWall2.node.room.IsOutside())
						{
							float num2 = Vector3.Distance(Player.Instance.transform.position, newWall2.position);
							if (num2 < this.extWallDistance)
							{
								this.extWallDistance = num2;
							}
						}
					}
				}
			}
		}
		this.PassExteriorWallDistance();
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x000B5368 File Offset: 0x000B3568
	public void PassExteriorWallDistance()
	{
		this.extWallNormalized = Mathf.Clamp01(this.extWallDistance / this.extWallDistanceMultiplier);
		if (this.ambienceRain != null)
		{
			this.ambienceRain.audioEvent.setParameterByName("ExtWallDistance", this.extWallNormalized, false);
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.audioEvent.setParameterByName("ExtWallDistance", this.extWallNormalized, false);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.audioEvent.setParameterByName("ExtWallDistance", this.extWallNormalized, false);
		}
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x000B53FC File Offset: 0x000B35FC
	public bool IsSoundPlaying(AudioController.LoopingSoundInfo sound)
	{
		return sound != null && sound.init && this.IsSoundPlaying(sound.audioEvent);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x000B5418 File Offset: 0x000B3618
	public bool IsSoundPlaying(EventInstance sound)
	{
		PLAYBACK_STATE playback_STATE = 2;
		sound.getPlaybackState(ref playback_STATE);
		return playback_STATE == null || playback_STATE == 1;
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x000B543C File Offset: 0x000B363C
	public void StopSound(AudioController.LoopingSoundInfo loop, AudioController.StopType stop, string debugReason)
	{
		if (loop != null)
		{
			loop.debugStoppedReason = debugReason;
			if (loop.eventPreset != null && loop.eventPreset.debug)
			{
				try
				{
					Game.Log(string.Concat(new string[]
					{
						"Audio: Stopping sound ",
						loop.eventPreset.name,
						" ",
						stop.ToString(),
						", reason: ",
						debugReason
					}), 2);
				}
				catch
				{
				}
			}
			if (loop.eventPreset != null && loop.eventPreset.forceOutlineForLoopIfPlayerTrespassing)
			{
				this.ForceOutlineCheck(loop.eventPreset, loop.interactable, true);
			}
			loop.isActive = false;
			this.StopSound(loop.audioEvent, stop);
			loop.isValid = loop.audioEvent.isValid();
		}
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x000B5524 File Offset: 0x000B3724
	public void StopSound(EventInstance sound, AudioController.StopType stop)
	{
		if (stop == AudioController.StopType.triggerCue)
		{
			sound.keyOff();
			return;
		}
		if (stop == AudioController.StopType.fade)
		{
			sound.stop(0);
			sound.release();
			return;
		}
		sound.stop(1);
		sound.release();
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x000B555C File Offset: 0x000B375C
	public EventInstance Play2DSound(AudioEvent eventPreset, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f)
	{
		if (eventPreset == null || eventPreset.guid.Length <= 0)
		{
			EventInstance result = default(EventInstance);
			result.release();
			Game.Log("Audio: Play2D: No preset", 2);
			return result;
		}
		if (eventPreset.debug)
		{
			Game.Log("Audio: Playing sound 2D sound: " + eventPreset.name, 2);
		}
		EventInstance result2 = RuntimeManager.CreateInstance(GUID.Parse(eventPreset.guid));
		if (parameters != null)
		{
			foreach (AudioController.FMODParam fmodparam in parameters)
			{
				result2.setParameterByName(fmodparam.name, fmodparam.value, false);
			}
		}
		result2.start();
		result2.setVolume(Mathf.Clamp01(eventPreset.masterVolumeScale * volumeOverride));
		result2.release();
		return result2;
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x000B5644 File Offset: 0x000B3844
	public void Play2DSoundDelayed(AudioEvent eventPreset, float delay, List<AudioController.FMODParam> parameters = null, float volumeOverride = 1f)
	{
		if (!SessionData.Instance.startedGame || (CityConstructor.Instance != null && CityConstructor.Instance.preSimActive))
		{
			return;
		}
		this.delayedSound.Add(new AudioController.DelayedSoundInfo(delay, eventPreset, null, null, Vector3.zero, parameters, volumeOverride, null, true, true));
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x000B5698 File Offset: 0x000B3898
	public float GetOcculusion(NewNode listenerLocation, NewNode sourceLocation, AudioEvent audioEvent, float baseVolume, Actor soundMaker, AudioController.SoundMaterialOverride detailedMaterialData, out int penetrationCount, out List<AudioController.ActiveListener> activeListeners, out bool isSuspicious, out List<NewRoom> audibleRooms, out float rangeHearing, List<NewNode> additionalLocations = null, bool forceSuspicious = false)
	{
		float num = baseVolume;
		penetrationCount = 0;
		activeListeners = new List<AudioController.ActiveListener>();
		isSuspicious = false;
		audibleRooms = new List<NewRoom>();
		rangeHearing = 0f;
		if (SessionData.Instance.isFloorEdit || SessionData.Instance.isTestScene)
		{
			return num;
		}
		if (listenerLocation.building != null && sourceLocation.building != null && listenerLocation.building != sourceLocation.building)
		{
			num = 0f;
			if (audioEvent.debug)
			{
				Game.Log("Audio: Sound cannot penetrate multiple buildings, resulting in a 0 occlusion value", 2);
			}
			return num;
		}
		if (Mathf.Abs(listenerLocation.nodeCoord.z - sourceLocation.nodeCoord.z) > 3)
		{
			num = 0f;
			if (audioEvent.debug)
			{
				Game.Log("Audio: Sound cannot penetrate more than 3 floors, resulting in a 0 occlusion value", 2);
			}
			return num;
		}
		float num2 = 0f;
		if (audioEvent.guid.Length > 0)
		{
			GUID guid = GUID.Parse(audioEvent.guid);
			EventDescription eventDescription;
			RuntimeManager.StudioSystem.getEventByID(guid, ref eventDescription);
			float num3;
			eventDescription.getMinMaxDistance(ref num3, ref num2);
		}
		rangeHearing = audioEvent.hearingRange;
		if (soundMaker != null)
		{
			if (soundMaker.isRunning)
			{
				rangeHearing += audioEvent.runModifier;
			}
			if (soundMaker.stealthMode)
			{
				rangeHearing += audioEvent.stealthModeModifier;
			}
		}
		if (audioEvent.modifyBasedOnSurface && detailedMaterialData != null)
		{
			rangeHearing += detailedMaterialData.carpet * audioEvent.carpetHearingRangeModifier;
			rangeHearing += detailedMaterialData.concrete * audioEvent.concreteHearingRangeModifier;
			rangeHearing += detailedMaterialData.fabric * audioEvent.fabricHearingRangeModifier;
			rangeHearing += detailedMaterialData.glass * audioEvent.glassHearingRangeModifier;
			rangeHearing += detailedMaterialData.metal * audioEvent.metalHearingRangeModifier;
			rangeHearing += detailedMaterialData.plaster * audioEvent.plasterHearingRangeModifier;
			rangeHearing += detailedMaterialData.tile * audioEvent.tileHearingRangeModifier;
			rangeHearing += detailedMaterialData.wood * audioEvent.woodHearingRangeModifier;
		}
		num2 = Mathf.Max(num2, rangeHearing);
		if ((soundMaker == Player.Instance && !Game.Instance.inaudiblePlayer) || soundMaker != Player.Instance)
		{
			if (forceSuspicious)
			{
				isSuspicious = true;
			}
			else if (audioEvent.canBeSuspicious)
			{
				if (audioEvent.alwaysSuspicious)
				{
					isSuspicious = true;
				}
				else if (audioEvent.suspiciousIfTresspassing && soundMaker != null && soundMaker.isTrespassing)
				{
					isSuspicious = true;
				}
				if (audioEvent.onlySuspiciousIfEmptyAddress && sourceLocation.gameLocation.currentOccupants.Count > audioEvent.suspiciousIfCitizenCount + 1)
				{
					isSuspicious = false;
				}
				if (soundMaker != null && audioEvent.onlySuspiciousIfNotEnforcer && soundMaker.isEnforcer && soundMaker.isOnDuty)
				{
					isSuspicious = false;
				}
			}
		}
		float num4 = this.occlusionUnitVolumeModifier;
		if (audioEvent.overrideOcclusionModifier)
		{
			num4 = audioEvent.occlusionUnitVolumeModifier;
		}
		if (num4 >= 0f)
		{
			Game.Log("Audio: Positive volume modification per unit detected for " + audioEvent.name + "! Look into this as it will cause occlusion errors!", 2);
		}
		Dictionary<NewRoom, float> dictionary = new Dictionary<NewRoom, float>();
		Dictionary<NewRoom, int> dictionary2 = new Dictionary<NewRoom, int>();
		Dictionary<NewRoom, int> dictionary3 = new Dictionary<NewRoom, int>();
		dictionary.Add(sourceLocation.room, num);
		dictionary2.Add(sourceLocation.room, 0);
		dictionary3.Add(sourceLocation.room, 0);
		if (additionalLocations != null)
		{
			foreach (NewNode newNode in additionalLocations)
			{
				if (!dictionary.ContainsKey(newNode.room))
				{
					dictionary.Add(newNode.room, num);
					dictionary2.Add(newNode.room, 0);
					dictionary3.Add(newNode.room, 0);
				}
			}
		}
		HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
		int num5 = Mathf.Max(sourceLocation.room.entrances.Count + 2, dictionary.Count + this.loopingMaximum);
		if (audioEvent.overrideMaximumLoops)
		{
			num5 = Mathf.Max(num5, audioEvent.overriddenMaxLoops);
		}
		bool flag = false;
		while (dictionary.Count > 0 && num5 > 0)
		{
			NewRoom newRoom = null;
			float num6 = -99999f;
			int num7 = 0;
			int num8 = 0;
			foreach (KeyValuePair<NewRoom, float> keyValuePair in dictionary)
			{
				if (keyValuePair.Value > num6)
				{
					newRoom = keyValuePair.Key;
					num6 = dictionary[keyValuePair.Key];
					num7 = dictionary2[keyValuePair.Key];
					num8 = dictionary3[keyValuePair.Key];
				}
			}
			if (newRoom != null)
			{
				if (audioEvent.debug)
				{
					try
					{
						string[] array = new string[8];
						array[0] = "Audio: Looping ";
						array[1] = newRoom.GetName();
						array[2] = " (";
						array[3] = newRoom.roomID.ToString();
						array[4] = ") on floor ";
						int num9 = 5;
						int z = newRoom.entrances[0].fromNode.nodeCoord.z;
						array[num9] = z.ToString();
						array[6] = ": Vol: ";
						array[7] = num6.ToString();
						Game.Log(string.Concat(array), 2);
					}
					catch
					{
					}
				}
				audibleRooms.Add(newRoom);
				if (!flag && newRoom == listenerLocation.room)
				{
					if (audioEvent.debug)
					{
						try
						{
							Game.Log(string.Concat(new string[]
							{
								"Audio: Found listener at ",
								listenerLocation.room.GetName(),
								" with ",
								num7.ToString(),
								" applied and volume ",
								num6.ToString()
							}), 2);
						}
						catch
						{
						}
					}
					if (!isSuspicious)
					{
						penetrationCount = num7;
						num = num6;
						return num;
					}
					penetrationCount = num7;
					num = num6;
					flag = true;
				}
				if (isSuspicious)
				{
					foreach (Actor actor in newRoom.currentOccupants)
					{
						if (!actor.isDead && !actor.isPlayer && !actor.isStunned && actor.canListen && !(actor == soundMaker) && !(actor.ai == null))
						{
							Game.Log("Audio: ...Listener " + actor.name + " is active...", 2);
							int z;
							string text;
							if (actor.ai != null && !actor.isAsleep && actor.seenIllegalThisCheck.Count <= 0 && !actor.IsTrespassing(sourceLocation.room, out z, out text, true))
							{
								float num10 = Vector3.Distance(actor.currentNode.position, sourceLocation.position);
								if (num10 <= rangeHearing)
								{
									float num11 = Mathf.Clamp01(num10 / rangeHearing);
									float num12 = this.emulationRolloff.Evaluate(num11) * num6 * actor.hearingMultiplier;
									if (num12 > this.aiHearingThreshold)
									{
										if (actor.isAsleep)
										{
											if (Toolbox.Instance.Rand(0f, num12, false) <= audioEvent.awakenChance)
											{
												actor.ai.AwakenPrompt();
											}
										}
										else
										{
											bool flag2 = false;
											if (audioEvent.suspiciousIfTresspassing)
											{
												flag2 = actor.IsTrespassing(sourceLocation.room, out z, out text, true);
											}
											if (flag2 && soundMaker != null && soundMaker.isEnforcer)
											{
												flag2 = false;
											}
											if (!flag2)
											{
												int escalationLevel = 0;
												if (soundMaker != null && soundMaker.isTrespassing)
												{
													Human human = soundMaker as Human;
													if (human != null)
													{
														escalationLevel = human.trespassingEscalation;
													}
												}
												activeListeners.Add(new AudioController.ActiveListener
												{
													listener = actor,
													escalationLevel = escalationLevel,
													soundLevel = num12
												});
											}
										}
									}
								}
							}
						}
					}
				}
				if (num6 > 0.01f)
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom.entrances)
					{
						float num13 = num6;
						int num14 = num7;
						if (Vector3.Distance(sourceLocation.position, nodeAccess.worldAccessPoint) <= num2)
						{
							if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.window)
							{
								int num15 = this.windowOcclusionUnits;
								if (audioEvent.overrideWindowOcclusion)
								{
									num15 = audioEvent.windowOcclusionUnits;
								}
								num14 += num15;
								num13 += num4 * (float)num15;
							}
							else if (nodeAccess.door == null)
							{
								int num16 = this.openDoorOcclusionUnits;
								if (audioEvent.overrideOpenDoorOcclusion)
								{
									num16 = audioEvent.openDoorOcclusionUnits;
								}
								num14 += num16;
								num13 += num4 * (float)num16;
							}
							else if (nodeAccess.door != null)
							{
								if (nodeAccess.door.isClosed)
								{
									int num17 = this.closedDoorOcclusionUnits;
									if (audioEvent.overrideClosedDoorOcclusion)
									{
										num17 = audioEvent.closedDoorOcclusionUnits;
									}
									num14 += num17;
									num13 += num4 * (float)num17;
								}
								else
								{
									int num18 = this.openDoorOcclusionUnits;
									if (audioEvent.overrideOpenDoorOcclusion)
									{
										num18 = audioEvent.openDoorOcclusionUnits;
									}
									num14 += num18;
									num13 += num4 * (float)num18;
								}
							}
							if ((nodeAccess.toNode.room != null && !nodeAccess.toNode.room.IsOutside()) || (nodeAccess.fromNode.room != null && !nodeAccess.fromNode.room.IsOutside()))
							{
								int num19 = Mathf.Abs(nodeAccess.toNode.nodeCoord.z - nodeAccess.fromNode.nodeCoord.z);
								num14 += num19 * this.floorDifferenceOcclusionUnits;
								num13 += num4 * (float)(num19 * this.floorDifferenceOcclusionUnits);
							}
							if (num8 + 1 > this.maxRoomDistance)
							{
								num14++;
								num13 += num4;
							}
							if (num13 > 0.01f && !hashSet.Contains(nodeAccess.toNode.room))
							{
								if (!dictionary2.ContainsKey(nodeAccess.toNode.room))
								{
									dictionary2.Add(nodeAccess.toNode.room, num14);
									dictionary.Add(nodeAccess.toNode.room, num13);
									dictionary3.Add(nodeAccess.toNode.room, num8 + 1);
								}
								else
								{
									dictionary2[nodeAccess.toNode.room] = Mathf.Min(dictionary2[nodeAccess.toNode.room], num14);
									dictionary[nodeAccess.toNode.room] = Mathf.Max(dictionary[nodeAccess.toNode.room], num13);
									dictionary3[nodeAccess.toNode.room] = Mathf.Min(dictionary3[nodeAccess.toNode.room], num8 + 1);
								}
							}
						}
					}
					if (audioEvent.canPenetrateWalls)
					{
						int num20 = this.wallOcclusionUnits;
						if (audioEvent.overrideWallOcclusion)
						{
							num20 = audioEvent.wallOcclusionUnits;
						}
						float num21 = num6 + (float)num20 * num4;
						int num22 = num7 + num20;
						if (num8 + 1 > this.maxRoomDistance)
						{
							num22++;
							num21 += num4;
						}
						if (num21 > num6)
						{
							Game.Log(string.Concat(new string[]
							{
								"Audio: Increased applied volume detected for ",
								audioEvent.name,
								"! This could cause weird occlusion errors: ",
								num6.ToString(),
								" + (",
								num20.ToString(),
								" * ",
								num4.ToString()
							}), 2);
						}
						if (num21 > 0.01f)
						{
							foreach (NewRoom newRoom2 in newRoom.adjacentRooms)
							{
								if (!(newRoom2 == newRoom) && !hashSet.Contains(newRoom2))
								{
									if (!dictionary2.ContainsKey(newRoom2))
									{
										dictionary.Add(newRoom2, num21);
										dictionary2.Add(newRoom2, num22);
										dictionary3.Add(newRoom2, num8 + 1);
									}
									else
									{
										dictionary2[newRoom2] = Mathf.Min(dictionary2[newRoom2], num22);
										dictionary[newRoom2] = Mathf.Max(dictionary[newRoom2], num21);
										dictionary3[newRoom2] = Mathf.Min(dictionary3[newRoom2], num8 + 1);
									}
								}
							}
						}
					}
					if (audioEvent.canPenetrateCeilings)
					{
						int num23 = this.ceilingOcclusionUnits;
						if (audioEvent.overrideCeilingOcclusion)
						{
							num23 = audioEvent.ceilingOcclusionUnits;
						}
						float num24 = num6 + (float)num23 * num4 + (float)this.floorDifferenceOcclusionUnits * num4;
						int num25 = num7 + num23 + 1;
						if (num8 + 1 > this.maxRoomDistance)
						{
							num25++;
							num24 += num4;
						}
						if (num24 > num6)
						{
							Game.Log(string.Concat(new string[]
							{
								"Audio: Increased applied volume detected for ",
								audioEvent.name,
								"! This could cause weird occlusion errors: ",
								num6.ToString(),
								" + (",
								num23.ToString(),
								" * ",
								num4.ToString(),
								" ",
								this.floorDifferenceOcclusionUnits.ToString()
							}), 2);
						}
						if (num24 > 0.01f)
						{
							foreach (NewRoom newRoom3 in newRoom.aboveRooms)
							{
								if (!(newRoom3 == newRoom) && !hashSet.Contains(newRoom3))
								{
									if (!dictionary2.ContainsKey(newRoom3))
									{
										dictionary.Add(newRoom3, num24);
										dictionary2.Add(newRoom3, num25);
										dictionary3.Add(newRoom3, num8 + 1);
									}
									else
									{
										dictionary2[newRoom3] = Mathf.Min(dictionary2[newRoom3], num25);
										dictionary[newRoom3] = Mathf.Max(dictionary[newRoom3], num24);
										dictionary3[newRoom3] = Mathf.Min(dictionary3[newRoom3], num8 + 1);
									}
								}
							}
						}
					}
					if (audioEvent.canPenetrateFloors)
					{
						int num26 = this.floorOcclusionUnits;
						if (audioEvent.overrideFloorOcclusion)
						{
							num26 = audioEvent.floorOcclusionUnits;
						}
						float num27 = num6 + (float)num26 * num4 + (float)this.floorDifferenceOcclusionUnits * num4;
						int num28 = num7 + num26 + 1;
						if (num8 + 1 > this.maxRoomDistance)
						{
							num28++;
							num27 += num4;
						}
						if (num27 > num6)
						{
							Game.Log(string.Concat(new string[]
							{
								"Audio: Increased applied volume detected for ",
								audioEvent.name,
								"! This could cause weird occlusion errors: ",
								num6.ToString(),
								" + (",
								num26.ToString(),
								" * ",
								num4.ToString(),
								" ",
								this.floorDifferenceOcclusionUnits.ToString()
							}), 2);
						}
						if (num27 > 0.01f)
						{
							foreach (NewRoom newRoom4 in newRoom.belowRooms)
							{
								if (!(newRoom4 == newRoom) && !hashSet.Contains(newRoom4))
								{
									if (!dictionary2.ContainsKey(newRoom4))
									{
										dictionary.Add(newRoom4, num27);
										dictionary2.Add(newRoom4, num28);
										dictionary3.Add(newRoom4, num8 + 1);
									}
									else
									{
										dictionary2[newRoom4] = Mathf.Min(dictionary2[newRoom4], num28);
										dictionary[newRoom4] = Mathf.Max(dictionary[newRoom4], num27);
										dictionary3[newRoom4] = Mathf.Min(dictionary3[newRoom4], num8 + 1);
									}
								}
							}
						}
					}
				}
			}
			hashSet.Add(newRoom);
			dictionary2.Remove(newRoom);
			dictionary.Remove(newRoom);
			dictionary3.Remove(newRoom);
			num5--;
		}
		if (!flag)
		{
			if (audioEvent.debug)
			{
				Game.Log("Audio: Unable to find the listener; resulting in 0 occlusion value", 2);
			}
			num = 0f;
		}
		else if (audioEvent.debug)
		{
			Game.Log("Audio: Returning with found listener at " + listenerLocation.room.GetName() + " with occlusion volume " + num.ToString(), 2);
		}
		return num;
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x000B6780 File Offset: 0x000B4980
	public float GetAmbientZoneOcculusion(NewNode listenerLocation, AudioController.AmbientZoneInstance ambientZone, out float distance, out int penetrationCount, out NewRoom audibleRoom)
	{
		float num = 0f;
		penetrationCount = 0;
		distance = 99999f;
		audibleRoom = null;
		if (SessionData.Instance.isFloorEdit || SessionData.Instance.isTestScene || listenerLocation == null || listenerLocation.room == null)
		{
			return num;
		}
		float num2 = this.occlusionUnitVolumeModifier;
		if (ambientZone.preset.overrideOcclusionModifier)
		{
			num2 = ambientZone.preset.occlusionUnitVolumeModifier;
		}
		Dictionary<NewRoom, float> dictionary = new Dictionary<NewRoom, float>();
		Dictionary<NewRoom, int> dictionary2 = new Dictionary<NewRoom, int>();
		Dictionary<NewRoom, float> dictionary3 = new Dictionary<NewRoom, float>();
		foreach (NewRoom newRoom in ambientZone.rooms)
		{
			dictionary.Add(newRoom, 1f);
			dictionary2.Add(newRoom, 0);
			dictionary3.Add(newRoom, 0f);
		}
		HashSet<NewRoom> hashSet = new HashSet<NewRoom>();
		int num3 = this.loopingMaximum;
		while (dictionary.Count > 0 && num3 > 0)
		{
			NewRoom newRoom2 = null;
			float num4 = 0f;
			int num5 = 0;
			int num6 = 0;
			float num7 = 0f;
			foreach (KeyValuePair<NewRoom, float> keyValuePair in dictionary)
			{
				if (keyValuePair.Value > num4)
				{
					newRoom2 = keyValuePair.Key;
					num4 = dictionary[keyValuePair.Key];
					num5 = dictionary2[keyValuePair.Key];
					num7 = dictionary3[keyValuePair.Key];
				}
			}
			if (newRoom2 != null)
			{
				if (newRoom2 == listenerLocation.room)
				{
					if (num4 >= 0.9999f)
					{
						penetrationCount = num5;
						num = num4;
						distance = num7;
						audibleRoom = newRoom2;
						return num4;
					}
					if (num4 > num)
					{
						penetrationCount = num5;
						num = num4;
						distance = num7;
						audibleRoom = newRoom2;
					}
				}
				float num8 = this.ambientFalloff.Evaluate(num7 / ambientZone.preset.maxRange);
				if (num4 > 0.01f)
				{
					foreach (NewNode.NodeAccess nodeAccess in newRoom2.entrances)
					{
						if (!(nodeAccess.toNode.room == null))
						{
							float num9 = num4;
							int num10 = num5;
							float num11 = num7;
							float num12 = Vector3.Distance(listenerLocation.position, nodeAccess.worldAccessPoint);
							num11 += num12;
							if (num11 <= ambientZone.preset.maxRange)
							{
								float num13 = this.ambientFalloff.Evaluate(num12 / ambientZone.preset.maxRange);
								num9 += num13 - num8;
								if (num9 >= 0.01f)
								{
									if (nodeAccess.accessType == NewNode.NodeAccess.AccessType.window)
									{
										num10 += this.windowOcclusionUnits;
										num9 += num2 * (float)this.windowOcclusionUnits;
									}
									else if (nodeAccess.door == null)
									{
										num10 += this.openDoorOcclusionUnits;
										num9 += num2 * (float)this.openDoorOcclusionUnits;
									}
									else if (nodeAccess.door != null)
									{
										if (nodeAccess.door.isClosed)
										{
											if (!ambientZone.preset.canPenetrateClosedDoors)
											{
												continue;
											}
											num10 += this.closedDoorOcclusionUnits;
											num9 += num2 * (float)this.closedDoorOcclusionUnits;
										}
										else
										{
											num10 += this.openDoorOcclusionUnits;
											num9 += num2 * (float)this.openDoorOcclusionUnits;
										}
									}
									int num14 = Mathf.Abs(nodeAccess.toNode.nodeCoord.z - nodeAccess.fromNode.nodeCoord.z);
									num10 += num14 * this.floorDifferenceOcclusionUnits;
									num9 += num2 * (float)(num14 * this.floorDifferenceOcclusionUnits);
									if (num6 + 1 > this.maxRoomDistance)
									{
										num10++;
										num9 += num2;
									}
									if (num9 > 0.01f && !hashSet.Contains(nodeAccess.toNode.room))
									{
										if (!dictionary2.ContainsKey(nodeAccess.toNode.room))
										{
											dictionary2.Add(nodeAccess.toNode.room, num10);
											dictionary.Add(nodeAccess.toNode.room, num9);
											dictionary3.Add(nodeAccess.toNode.room, num11);
										}
										else
										{
											dictionary2[nodeAccess.toNode.room] = Mathf.Min(dictionary2[nodeAccess.toNode.room], num10);
											dictionary[nodeAccess.toNode.room] = Mathf.Max(dictionary[nodeAccess.toNode.room], num9);
											dictionary3[nodeAccess.toNode.room] = Mathf.Min(dictionary3[nodeAccess.toNode.room], num11);
										}
									}
								}
							}
						}
					}
				}
				hashSet.Add(newRoom2);
				dictionary2.Remove(newRoom2);
				dictionary.Remove(newRoom2);
				dictionary3.Remove(newRoom2);
			}
			num3--;
		}
		return num;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x000B6CB8 File Offset: 0x000B4EB8
	public void ForceOutlineCheck(AudioEvent audioEvent, Interactable inter, bool forceOff = false)
	{
		if (inter == null)
		{
			Game.LogError("Trying to set forced outline for " + audioEvent.name + " but interactable isn't passed...", 2);
			return;
		}
		if (inter.controller == null)
		{
			return;
		}
		Transform[] componentsInChildren = inter.controller.transform.GetComponentsInChildren<Transform>();
		Transform[] array;
		if (!Player.Instance.illegalStatus || forceOff)
		{
			inter.controller.transform.gameObject.layer = 0;
			array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.layer = 0;
			}
			return;
		}
		inter.controller.transform.gameObject.layer = 30;
		array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = 30;
		}
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x000B6D80 File Offset: 0x000B4F80
	public float GetPlayersSoundLevel(NewNode sourceLocation, AudioEvent audioEvent, float occludedVolume, AudioController.SoundMaterialOverride detailedMaterialData)
	{
		float result = 0f;
		float num = audioEvent.hearingRange;
		if (audioEvent.modifyBasedOnSurface && detailedMaterialData != null)
		{
			num += detailedMaterialData.carpet * audioEvent.carpetHearingRangeModifier;
			num += detailedMaterialData.concrete * audioEvent.concreteHearingRangeModifier;
			num += detailedMaterialData.fabric * audioEvent.fabricHearingRangeModifier;
			num += detailedMaterialData.glass * audioEvent.glassHearingRangeModifier;
			num += detailedMaterialData.metal * audioEvent.metalHearingRangeModifier;
			num += detailedMaterialData.plaster * audioEvent.plasterHearingRangeModifier;
			num += detailedMaterialData.tile * audioEvent.tileHearingRangeModifier;
			num += detailedMaterialData.wood * audioEvent.woodHearingRangeModifier;
		}
		float num2 = Vector3.Distance(Player.Instance.transform.position, sourceLocation.position);
		if (num2 <= num)
		{
			float num3 = num2 / num;
			result = this.emulationRolloff.Evaluate(num3) * occludedVolume * Player.Instance.hearingMultiplier;
		}
		return result;
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x000B6E72 File Offset: 0x000B5072
	public void UpdateAmbientZonesOnEndOfFrame()
	{
		if (Toolbox.Instance != null)
		{
			Toolbox.Instance.InvokeEndOfFrame(this.updateAmbientZonesAction, "Update ambient zones");
		}
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x000B6E98 File Offset: 0x000B5098
	public void UpdateAmbientZones()
	{
		this.PassIndoorOutdoor();
		this.UpdateDistanceFromEdge();
		Player.Instance.UpdatePlayerAmbientState();
		foreach (AudioController.AmbientZoneInstance ambientZoneInstance in this.ambientZones)
		{
			ambientZoneInstance.isActive = false;
			ambientZoneInstance.desiredVolume = 0f;
			ambientZoneInstance.playerDistance = 0f;
			ambientZoneInstance.penetrationCount = 0;
			ambientZoneInstance.audibleRoom = null;
			if (ambientZoneInstance.preset.isAirDuctAmbience && Player.Instance.inAirVent)
			{
				ambientZoneInstance.isActive = true;
				ambientZoneInstance.desiredVolume = 1f;
				ambientZoneInstance.audibleRoom = Player.Instance.currentRoom;
			}
			if (ambientZoneInstance.rooms.Contains(Player.Instance.currentRoom))
			{
				ambientZoneInstance.isActive = true;
				ambientZoneInstance.desiredVolume = 1f;
				ambientZoneInstance.audibleRoom = Player.Instance.currentRoom;
			}
			else if (ambientZoneInstance.preset.useOcclusion)
			{
				ambientZoneInstance.desiredVolume = Mathf.Clamp01(this.GetAmbientZoneOcculusion(Player.Instance.currentNode, ambientZoneInstance, out ambientZoneInstance.playerDistance, out ambientZoneInstance.penetrationCount, out ambientZoneInstance.audibleRoom));
				if (ambientZoneInstance.desiredVolume > 0.01f)
				{
					ambientZoneInstance.isActive = true;
				}
			}
		}
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000B6FFC File Offset: 0x000B51FC
	public void ResetThis()
	{
		while (this.loopingSounds.Count > 0)
		{
			this.StopSound(this.loopingSounds[0], AudioController.StopType.immediate, "Reset audio controller");
		}
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x000B7028 File Offset: 0x000B5228
	public void SetVCALevel(string vcaName, float value)
	{
		try
		{
			VCA vca = RuntimeManager.GetVCA("vca:/" + vcaName);
			Game.Log("Audio: Setting " + vcaName + " VCA to " + value.ToString(), 2);
			vca.setVolume(value);
		}
		catch
		{
			Game.LogError("Unable to find VCA: " + vcaName, 2);
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x000B7094 File Offset: 0x000B5294
	[Button(null, 0)]
	public void UpdateAmbientPlaybackState()
	{
		if (this.ambienceRain != null)
		{
			this.ambienceRain.UpdatePlayState();
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.UpdatePlayState();
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.UpdatePlayState();
		}
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x000B70D4 File Offset: 0x000B52D4
	public void StopAllSounds()
	{
		foreach (AudioController.LoopingSoundInfo loop in this.loopingSounds)
		{
			this.StopSound(loop, AudioController.StopType.immediate, "stop all");
		}
		foreach (AudioController.AmbientZoneInstance ambientZoneInstance in this.ambientZones)
		{
			this.StopSound(ambientZoneInstance.eventData, AudioController.StopType.immediate, "stop all");
		}
		this.StopSound(this.ambienceWind, AudioController.StopType.immediate, "stop all");
		this.StopSound(this.ambienceRain, AudioController.StopType.immediate, "stop all");
		this.StopSound(this.ambiencePA, AudioController.StopType.immediate, "stop all");
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x000B71B0 File Offset: 0x000B53B0
	public void UpdateVolumeChanging()
	{
		List<AudioController.LoopingSoundInfo> list = new List<AudioController.LoopingSoundInfo>();
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo in this.volumeChangingSounds)
		{
			float num = 0f;
			loopingSoundInfo.audioEvent.getVolume(ref num);
			if (loopingSoundInfo.fadeToVolume != num)
			{
				float num2 = 0.5f;
				if (loopingSoundInfo.eventPreset.forceVolumeLevelFadeTime)
				{
					num2 = loopingSoundInfo.eventPreset.volumeLevelFadeTime;
				}
				if (num < loopingSoundInfo.fadeToVolume)
				{
					num += Time.deltaTime / num2;
					num = Mathf.Min(num, loopingSoundInfo.fadeToVolume);
				}
				else if (num > loopingSoundInfo.fadeToVolume)
				{
					num -= Time.deltaTime / num2;
					num = Mathf.Max(num, loopingSoundInfo.fadeToVolume);
				}
				if (loopingSoundInfo.eventPreset.debug)
				{
					Game.Log(string.Concat(new string[]
					{
						"Audio: ",
						loopingSoundInfo.eventPreset.name,
						" Actively fading to new audio level: ",
						num.ToString(),
						"/",
						loopingSoundInfo.fadeToVolume.ToString()
					}), 2);
				}
				loopingSoundInfo.audioEvent.setVolume(num);
				if (num == loopingSoundInfo.fadeToVolume)
				{
					list.Add(loopingSoundInfo);
				}
			}
			else
			{
				list.Add(loopingSoundInfo);
			}
		}
		foreach (AudioController.LoopingSoundInfo loopingSoundInfo2 in list)
		{
			this.volumeChangingSounds.Remove(loopingSoundInfo2);
		}
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x000B7370 File Offset: 0x000B5570
	[Button(null, 0)]
	public void DebugWeatherLoopDisplay()
	{
		if (this.ambienceRain != null)
		{
			this.ambienceRain.UpdatePlayState();
			Game.Log("Rain: " + this.ambienceRain.state.ToString(), 2);
		}
		if (this.ambienceWind != null)
		{
			this.ambienceWind.UpdatePlayState();
			Game.Log("Wind: " + this.ambienceWind.state.ToString(), 2);
		}
		if (this.ambiencePA != null)
		{
			this.ambiencePA.UpdatePlayState();
			Game.Log("PA: " + this.ambiencePA.state.ToString(), 2);
		}
	}

	// Token: 0x04000E5A RID: 3674
	public StudioListener playerListener;

	// Token: 0x04000E5B RID: 3675
	[Header("Misc. Settings")]
	[Tooltip("Speed of sound in unity (m) per (in-game) second")]
	public float speedOfSound = 343f;

	// Token: 0x04000E5C RID: 3676
	[Header("Occlusion: Modifiers")]
	[Space(7f)]
	[Tooltip("Each occlusion unit will decrease volume by this amount...")]
	public float occlusionUnitVolumeModifier = -0.1f;

	// Token: 0x04000E5D RID: 3677
	[Range(0f, 10f)]
	public int openDoorOcclusionUnits;

	// Token: 0x04000E5E RID: 3678
	[Range(0f, 10f)]
	public int closedDoorOcclusionUnits = 5;

	// Token: 0x04000E5F RID: 3679
	[Range(0f, 10f)]
	public int windowOcclusionUnits = 4;

	// Token: 0x04000E60 RID: 3680
	[Range(0f, 10f)]
	public int wallOcclusionUnits = 7;

	// Token: 0x04000E61 RID: 3681
	[Range(0f, 10f)]
	public int ceilingOcclusionUnits = 8;

	// Token: 0x04000E62 RID: 3682
	[Range(0f, 10f)]
	public int floorOcclusionUnits = 8;

	// Token: 0x04000E63 RID: 3683
	[Range(0f, 10f)]
	public int floorDifferenceOcclusionUnits = 2;

	// Token: 0x04000E64 RID: 3684
	[Tooltip("Loop through this many rooms as a maximum...")]
	[Space(5f)]
	public int loopingMaximum = 32;

	// Token: 0x04000E65 RID: 3685
	[Tooltip("Sounds can travel this many rooms away from the source. After that they gain +1 occlusion unit per additional room.")]
	public int maxRoomDistance = 4;

	// Token: 0x04000E66 RID: 3686
	[Tooltip("The emulated rolloff of the sound")]
	public AnimationCurve emulationRolloff;

	// Token: 0x04000E67 RID: 3687
	[Tooltip("Sound needs to be playing at at least this volume for the AI to register it")]
	public float aiHearingThreshold = 0.1f;

	// Token: 0x04000E68 RID: 3688
	[Tooltip("Sound needs to be playing at at least this volume for the player to register it")]
	public float playerHearingThreshold = 0.1f;

	// Token: 0x04000E69 RID: 3689
	[Tooltip("A sound icon represents this much simulated range")]
	public float soundIconRangeUnit = 5f;

	// Token: 0x04000E6A RID: 3690
	[Header("Ambient Sound Properties")]
	[ReadOnly]
	public int updateClosestWindowTicker;

	// Token: 0x04000E6B RID: 3691
	[ReadOnly]
	public int updateMixingTicker;

	// Token: 0x04000E6C RID: 3692
	[ReadOnly]
	public float updateAmbientZonesTimer;

	// Token: 0x04000E6D RID: 3693
	[Tooltip("Update closest windows and open ext door every X frames...")]
	public int updateClosestWindow = 8;

	// Token: 0x04000E6E RID: 3694
	[Tooltip("Update closest windows and open ext door every X frames...")]
	public int updateMixing = 2;

	// Token: 0x04000E6F RID: 3695
	[Tooltip("Current closest window position.")]
	[ReadOnly]
	public Vector3 windowAudioPosition;

	// Token: 0x04000E70 RID: 3696
	[Tooltip("Distance from player to above.")]
	[ReadOnly]
	public float closestWindowDistance;

	// Token: 0x04000E71 RID: 3697
	[ReadOnly]
	[Tooltip("Normalized version of above")]
	public float closestWindowDistanceNormalized;

	// Token: 0x04000E72 RID: 3698
	[Tooltip("Window distance multiplier (used to create normalised variable)")]
	public float closestWindowDistanceMultiplier = 16f;

	// Token: 0x04000E73 RID: 3699
	[Tooltip("Curve used as a multiplier for the above, and based on how open the door is on X (0 = closed, 1 = open)")]
	public AnimationCurve openMultiplierCurve;

	// Token: 0x04000E74 RID: 3700
	[ReadOnly]
	[Tooltip("Interpolated outdoors/indoors transition")]
	[Space(5f)]
	public float ventOutdoorsIndoors;

	// Token: 0x04000E75 RID: 3701
	[Tooltip("Distance to the nearest vent")]
	[ReadOnly]
	public float nearbyVent;

	// Token: 0x04000E76 RID: 3702
	[ReadOnly]
	[Tooltip("Current closest open external door position.")]
	[Space(5f)]
	public Vector3 doorAudioPosition;

	// Token: 0x04000E77 RID: 3703
	[Tooltip("Distance from player to above.")]
	[ReadOnly]
	public float closestDoorDistance;

	// Token: 0x04000E78 RID: 3704
	[Tooltip("Normalized version of above")]
	[ReadOnly]
	public float closestDoorDistanceNormalized;

	// Token: 0x04000E79 RID: 3705
	[Tooltip("Window distance multiplier (used to create normalised variable)")]
	public float closestDoorDistanceMultiplier = 16f;

	// Token: 0x04000E7A RID: 3706
	[Tooltip("Distance from an edge tile")]
	[ReadOnly]
	[Space(5f)]
	public float edgeDistance;

	// Token: 0x04000E7B RID: 3707
	[ReadOnly]
	[Tooltip("Normalized version of above")]
	public float edgeDistanceNormalized;

	// Token: 0x04000E7C RID: 3708
	[Tooltip("Edge distance multiplier (used to create normalised variable)")]
	public float edgeDistanceMultiplier = 30f;

	// Token: 0x04000E7D RID: 3709
	[ReadOnly]
	[Space(5f)]
	[Tooltip("Distance from an exterior wall")]
	public float extWallDistance;

	// Token: 0x04000E7E RID: 3710
	[Tooltip("Normalized version of above")]
	[ReadOnly]
	public float extWallNormalized;

	// Token: 0x04000E7F RID: 3711
	[Tooltip("Edge distance multiplier (used to create normalised variable)")]
	public float extWallDistanceMultiplier = 16f;

	// Token: 0x04000E80 RID: 3712
	[Space(7f)]
	[ReadOnly]
	public float passedWind;

	// Token: 0x04000E81 RID: 3713
	[ReadOnly]
	public float passedRain;

	// Token: 0x04000E82 RID: 3714
	[ReadOnly]
	public float passedCity;

	// Token: 0x04000E83 RID: 3715
	[BoxGroup("Ambient Zones")]
	public List<AudioController.AmbientZoneInstance> ambientZones = new List<AudioController.AmbientZoneInstance>();

	// Token: 0x04000E84 RID: 3716
	public Dictionary<AmbientZone, AudioController.AmbientZoneInstance> ambientZoneReference = new Dictionary<AmbientZone, AudioController.AmbientZoneInstance>();

	// Token: 0x04000E85 RID: 3717
	[Tooltip("As this is a 2D sound we need to apply volume falloff manually")]
	public AnimationCurve ambientFalloff;

	// Token: 0x04000E86 RID: 3718
	public AudioController.LoopingSoundInfo ambienceWind;

	// Token: 0x04000E87 RID: 3719
	public AudioController.LoopingSoundInfo ambienceRain;

	// Token: 0x04000E88 RID: 3720
	public AudioController.LoopingSoundInfo ambiencePA;

	// Token: 0x04000E89 RID: 3721
	[NonSerialized]
	public AudioController.LoopingSoundInfo threatLoop;

	// Token: 0x04000E8A RID: 3722
	public List<AudioController.LoopingSoundInfo> loopingSounds = new List<AudioController.LoopingSoundInfo>();

	// Token: 0x04000E8B RID: 3723
	public HashSet<AudioController.LoopingSoundInfo> volumeChangingSounds = new HashSet<AudioController.LoopingSoundInfo>();

	// Token: 0x04000E8C RID: 3724
	public List<AudioController.DelayedSoundInfo> delayedSound = new List<AudioController.DelayedSoundInfo>();

	// Token: 0x04000E8D RID: 3725
	public int footstepLayerMask;

	// Token: 0x04000E8E RID: 3726
	private Action updateAmbientZonesAction;

	// Token: 0x04000E8F RID: 3727
	private static AudioController _instance;

	// Token: 0x02000232 RID: 562
	[Serializable]
	public class AmbientZoneInstance
	{
		// Token: 0x06000CD8 RID: 3288 RVA: 0x000B7520 File Offset: 0x000B5720
		public AmbientZoneInstance(AmbientZone newPreset)
		{
			this.preset = newPreset;
			AudioController.Instance.ambientZoneReference.Add(this.preset, this);
		}

		// Token: 0x04000E90 RID: 3728
		public AmbientZone preset;

		// Token: 0x04000E91 RID: 3729
		public float playerDistance;

		// Token: 0x04000E92 RID: 3730
		public int penetrationCount;

		// Token: 0x04000E93 RID: 3731
		public NewRoom audibleRoom;

		// Token: 0x04000E94 RID: 3732
		public bool isActive;

		// Token: 0x04000E95 RID: 3733
		[Space(5f)]
		public float desiredVolume;

		// Token: 0x04000E96 RID: 3734
		public float actualVolume;

		// Token: 0x04000E97 RID: 3735
		[Space(5f)]
		public float desiredWalla;

		// Token: 0x04000E98 RID: 3736
		public float actualWalla;

		// Token: 0x04000E99 RID: 3737
		[NonSerialized]
		public AudioController.LoopingSoundInfo eventData;

		// Token: 0x04000E9A RID: 3738
		public HashSet<NewRoom> rooms = new HashSet<NewRoom>();
	}

	// Token: 0x02000233 RID: 563
	[Serializable]
	public class LoopingSoundInfo
	{
		// Token: 0x06000CD9 RID: 3289 RVA: 0x000B7550 File Offset: 0x000B5750
		public PLAYBACK_STATE UpdatePlayState()
		{
			this.audioEvent.getPlaybackState(ref this.state);
			return this.state;
		}

		// Token: 0x06000CDA RID: 3290 RVA: 0x000B756C File Offset: 0x000B576C
		public void UpdateWorldPosition(Vector3 newWorldPos, NewNode newNodePos)
		{
			this.worldPos = newWorldPos;
			if (newNodePos == null)
			{
				Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(this.worldPos);
				if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNodePos))
				{
					newNodePos = this.sourceLocation;
				}
			}
			this.sourceLocation = newNodePos;
			if (this.eventPreset.debug)
			{
				string[] array = new string[6];
				array[0] = "Audio: Updating sound position of ";
				array[1] = this.eventPreset.name;
				array[2] = " to ";
				int num = 3;
				Vector3 vector = newWorldPos;
				array[num] = vector.ToString();
				array[4] = " and sourcelocation node ";
				int num2 = 5;
				vector = this.sourceLocation.position;
				array[num2] = vector.ToString();
				Game.Log(string.Concat(array), 2);
			}
			this.UpdateOcclusion(true);
		}

		// Token: 0x06000CDB RID: 3291 RVA: 0x000B7638 File Offset: 0x000B5838
		public void UpdateOcclusion(bool ignoreLastUpdateTime = false)
		{
			if (this.sourceLocation == null)
			{
				return;
			}
			if (SessionData.Instance.isFloorEdit)
			{
				return;
			}
			if (!SessionData.Instance.startedGame || CityConstructor.Instance.preSimActive)
			{
				return;
			}
			if (!ignoreLastUpdateTime && this.lastUpdated >= SessionData.Instance.gameTime - 0.0005f)
			{
				if (this.eventPreset.debug)
				{
					Game.Log("Audio: Cancelling occlusion update for " + this.eventPreset.name + " has it has been updated recently...", 2);
				}
				return;
			}
			if (this.interactable != null)
			{
				this.worldPos = this.interactable.GetWorldPosition(true);
			}
			this.occlusionVolume = this.volumeOverride;
			int num = 0;
			List<AudioController.ActiveListener> list = null;
			List<NewRoom> list2 = null;
			bool flag = this.forceSuspicious;
			if (Vector3.Distance(Player.Instance.transform.position, this.sourceLocation.position) <= 30f)
			{
				if (this.eventPreset.disableOcclusion)
				{
					this.occlusionVolume = 1f;
				}
				else
				{
					float num2;
					this.occlusionVolume = AudioController.Instance.GetOcculusion(Player.Instance.currentNode, this.sourceLocation, this.eventPreset, this.occlusionVolume, this.who, null, out num, out list, out flag, out list2, out num2, null, this.forceSuspicious);
					if (this.eventPreset.debug)
					{
						Game.Log("Audio: Updated occlusion for sound " + this.eventPreset.name + " with volume " + this.occlusionVolume.ToString(), 2);
					}
					if (flag)
					{
						if (list2 != null)
						{
							for (int i = 0; i < this.audibleRooms.Count; i++)
							{
								NewRoom newRoom = this.audibleRooms[i];
								if (list2.Contains(newRoom))
								{
									list2.Remove(newRoom);
								}
								else
								{
									newRoom.audibleLoopingSounds.Remove(this);
									this.audibleRooms.RemoveAt(i);
									i--;
								}
							}
							while (list2.Count > 0)
							{
								list2[0].audibleLoopingSounds.Add(this);
								this.audibleRooms.Add(list2[0]);
								list2.RemoveAt(0);
							}
						}
						if (this.who != null)
						{
							using (List<AudioController.ActiveListener>.Enumerator enumerator = list.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									AudioController.ActiveListener activeListener = enumerator.Current;
									if (activeListener.listener != this.who)
									{
										activeListener.listener.HearIllegal(this.eventPreset, this.sourceLocation, this.sourceLocation.position, null, activeListener.escalationLevel);
									}
								}
								goto IL_2A2;
							}
							goto IL_28A;
						}
					}
				}
				IL_2A2:
				float num3 = Mathf.Clamp01(this.occlusionVolume * this.eventPreset.masterVolumeScale);
				this.UpdatePlayState();
				this.isValid = this.audioEvent.isValid();
				if (this.isActive && num3 > 0.01f && (this.state == 2 || !this.isValid))
				{
					if (!this.isValid)
					{
						if (this.eventPreset.debug)
						{
							string[] array = new string[8];
							array[0] = "Audio: Starting/restarting looping sound ";
							array[1] = this.eventPreset.name;
							array[2] = " at ";
							int num4 = 3;
							Vector3 vector = this.worldPos;
							array[num4] = vector.ToString();
							array[4] = " because volume is now above minimum threshold. Prev state: ";
							array[5] = this.state.ToString();
							array[6] = ", vol: ";
							array[7] = num3.ToString();
							Game.Log(string.Concat(array), 2);
						}
						GUID guid = GUID.Parse(this.eventPreset.guid);
						RuntimeManager.StudioSystem.getEventByID(guid, ref this.description);
						try
						{
							this.audioEvent = RuntimeManager.CreateInstance(guid);
							goto IL_47A;
						}
						catch
						{
							Game.Log("Audio: Cannot find audio event " + this.eventPreset.guid + " skipping...", 2);
							goto IL_47A;
						}
					}
					if (this.state == 2 && this.eventPreset.debug)
					{
						string[] array2 = new string[8];
						array2[0] = "Audio: Resuming a previously stopped looping sound ";
						array2[1] = this.eventPreset.name;
						array2[2] = " at ";
						int num5 = 3;
						Vector3 vector = this.worldPos;
						array2[num5] = vector.ToString();
						array2[4] = " because volume is now above minimum threshold. Prev state: ";
						array2[5] = this.state.ToString();
						array2[6] = ", vol: ";
						array2[7] = num3.ToString();
						Game.Log(string.Concat(array2), 2);
					}
					IL_47A:
					if (this.isBroadcast != null && this.isBroadcast.currentBroadcastSchedule != null)
					{
						this.audioEvent.setTimelinePosition(Mathf.RoundToInt(this.isBroadcast.currentShowProgressSeconds * 1000f));
					}
					this.audioEvent.set3DAttributes(RuntimeUtils.To3DAttributes(this.worldPos));
					if (this.eventPreset.debug)
					{
						Game.Log("Audio: Start event: " + this.eventPreset.name, 2);
					}
					this.audioEvent.start();
					this.SetVolumeFadeTo(num3);
				}
				else if (!this.isActive || num3 <= 0.01f)
				{
					if (this.eventPreset.debug)
					{
						string[] array3 = new string[5];
						array3[0] = "Audio: Stopping looping sound ";
						array3[1] = this.eventPreset.name;
						array3[2] = " at ";
						int num6 = 3;
						Vector3 vector = this.worldPos;
						array3[num6] = vector.ToString();
						array3[4] = " because volume is now below minimum threshold";
						Game.Log(string.Concat(array3), 2);
					}
					this.audioEvent.stop(1);
					this.audioEvent.release();
				}
				else
				{
					if (this.who != null)
					{
						if (this.who.isRunning)
						{
							this.audioEvent.setParameterByName("Stealth", -1f, false);
						}
						else if (!this.who.stealthMode)
						{
							this.audioEvent.setParameterByName("Stealth", 0f, false);
						}
						else
						{
							this.audioEvent.setParameterByName("Stealth", 1f, false);
						}
					}
					this.audioEvent.set3DAttributes(RuntimeUtils.To3DAttributes(this.worldPos));
					this.audioEvent.setParameterByName("Occlusion", (float)num, false);
					if (this.parameters != null)
					{
						foreach (AudioController.FMODParam fmodparam in this.parameters)
						{
							this.audioEvent.setParameterByName(fmodparam.name, fmodparam.value, false);
						}
					}
					this.SetVolumeFadeTo(num3);
				}
				this.lastUpdated = SessionData.Instance.gameTime;
				this.UpdatePlayState();
				this.isValid = this.audioEvent.isValid();
				this.currentOcclusion = num;
				if ((this.who == null || !this.who.isPlayer) && this.eventPreset.forceOutlineForLoopIfPlayerTrespassing)
				{
					if (this.state == 2)
					{
						AudioController.Instance.ForceOutlineCheck(this.eventPreset, this.interactable, true);
						return;
					}
					AudioController.Instance.ForceOutlineCheck(this.eventPreset, this.interactable, false);
				}
				return;
			}
			IL_28A:
			this.occlusionVolume = 0f;
			list = new List<AudioController.ActiveListener>();
			list2 = new List<NewRoom>();
		}

		// Token: 0x06000CDC RID: 3292 RVA: 0x000B7D90 File Offset: 0x000B5F90
		public void SetVolumeImmediate(float vol)
		{
			Game.Log("Audio: Setting new volume to sound " + this.eventPreset.name + " (immediate) of " + vol.ToString(), 2);
			this.fadeToVolume = vol;
			this.audioEvent.setVolume(vol);
			AudioController.Instance.volumeChangingSounds.Remove(this);
		}

		// Token: 0x06000CDD RID: 3293 RVA: 0x000B7DE9 File Offset: 0x000B5FE9
		public void SetVolumeFadeTo(float vol)
		{
			this.fadeToVolume = vol;
			if (!AudioController.Instance.volumeChangingSounds.Contains(this))
			{
				AudioController.Instance.volumeChangingSounds.Add(this);
			}
		}

		// Token: 0x04000E9B RID: 3739
		public string name;

		// Token: 0x04000E9C RID: 3740
		public bool init;

		// Token: 0x04000E9D RID: 3741
		public EventInstance audioEvent;

		// Token: 0x04000E9E RID: 3742
		public bool isValid;

		// Token: 0x04000E9F RID: 3743
		public EventDescription description;

		// Token: 0x04000EA0 RID: 3744
		public float volumeOverride = 1f;

		// Token: 0x04000EA1 RID: 3745
		public AudioEvent eventPreset;

		// Token: 0x04000EA2 RID: 3746
		public NewNode sourceLocation;

		// Token: 0x04000EA3 RID: 3747
		public Actor who;

		// Token: 0x04000EA4 RID: 3748
		[NonSerialized]
		public Interactable interactable;

		// Token: 0x04000EA5 RID: 3749
		public bool forceSuspicious;

		// Token: 0x04000EA6 RID: 3750
		public List<AudioController.FMODParam> parameters;

		// Token: 0x04000EA7 RID: 3751
		public float lastUpdated;

		// Token: 0x04000EA8 RID: 3752
		public int currentOcclusion;

		// Token: 0x04000EA9 RID: 3753
		public Vector3 worldPos;

		// Token: 0x04000EAA RID: 3754
		public PLAYBACK_STATE state;

		// Token: 0x04000EAB RID: 3755
		public List<NewRoom> audibleRooms = new List<NewRoom>();

		// Token: 0x04000EAC RID: 3756
		public SessionData.TelevisionChannel isBroadcast;

		// Token: 0x04000EAD RID: 3757
		public float occlusionVolume;

		// Token: 0x04000EAE RID: 3758
		public float fadeToVolume;

		// Token: 0x04000EAF RID: 3759
		public bool isActive = true;

		// Token: 0x04000EB0 RID: 3760
		public string debugStoppedReason;

		// Token: 0x04000EB1 RID: 3761
		public InteractablePreset.IfSwitchStateSFX interactableLoopInfo;
	}

	// Token: 0x02000234 RID: 564
	public class ActiveListener
	{
		// Token: 0x04000EB2 RID: 3762
		public Actor listener;

		// Token: 0x04000EB3 RID: 3763
		public float soundLevel;

		// Token: 0x04000EB4 RID: 3764
		public int escalationLevel;
	}

	// Token: 0x02000235 RID: 565
	[Serializable]
	public class DelayedSoundInfo
	{
		// Token: 0x06000CE0 RID: 3296 RVA: 0x000B7E3C File Offset: 0x000B603C
		public DelayedSoundInfo(float newDelay, AudioEvent newEventPreset, Actor newWho, NewNode newLocation, Vector3 newWorldPosition, List<AudioController.FMODParam> newParameters = null, float newVolumeOverride = 1f, List<NewNode> newAdditionalSources = null, bool newForceIgnoreOcclusion = false, bool newIs2D = false)
		{
			this.delay = newDelay;
			this.eventPreset = newEventPreset;
			this.who = newWho;
			this.location = newLocation;
			this.worldPosition = newWorldPosition;
			this.parameters = newParameters;
			this.volumeOverride = newVolumeOverride;
			this.additionalSources = newAdditionalSources;
			this.forceIgnoreOcclusion = newForceIgnoreOcclusion;
			this.is2D = newIs2D;
		}

		// Token: 0x04000EB5 RID: 3765
		public float delay;

		// Token: 0x04000EB6 RID: 3766
		public AudioEvent eventPreset;

		// Token: 0x04000EB7 RID: 3767
		public Actor who;

		// Token: 0x04000EB8 RID: 3768
		public NewNode location;

		// Token: 0x04000EB9 RID: 3769
		public Vector3 worldPosition;

		// Token: 0x04000EBA RID: 3770
		public List<AudioController.FMODParam> parameters;

		// Token: 0x04000EBB RID: 3771
		public float volumeOverride = 1f;

		// Token: 0x04000EBC RID: 3772
		public List<NewNode> additionalSources;

		// Token: 0x04000EBD RID: 3773
		public bool forceIgnoreOcclusion;

		// Token: 0x04000EBE RID: 3774
		public bool is2D;
	}

	// Token: 0x02000236 RID: 566
	[Serializable]
	public class SoundMaterialOverride
	{
		// Token: 0x06000CE1 RID: 3297 RVA: 0x000B7EA8 File Offset: 0x000B60A8
		public SoundMaterialOverride(float newConcrete, float newWood, float newCarpet, float newTile, float newPlaster, float newFabric, float newMetal, float newGlass)
		{
			this.concrete = newConcrete;
			this.wood = newWood;
			this.carpet = newCarpet;
			this.tile = newTile;
			this.plaster = newPlaster;
			this.fabric = newFabric;
			this.metal = newMetal;
			this.glass = newGlass;
		}

		// Token: 0x04000EBF RID: 3775
		public float concrete;

		// Token: 0x04000EC0 RID: 3776
		public float wood;

		// Token: 0x04000EC1 RID: 3777
		public float carpet;

		// Token: 0x04000EC2 RID: 3778
		public float tile;

		// Token: 0x04000EC3 RID: 3779
		public float plaster;

		// Token: 0x04000EC4 RID: 3780
		public float fabric;

		// Token: 0x04000EC5 RID: 3781
		public float metal;

		// Token: 0x04000EC6 RID: 3782
		public float glass;
	}

	// Token: 0x02000237 RID: 567
	public enum CitizenReaction
	{
		// Token: 0x04000EC8 RID: 3784
		investigate,
		// Token: 0x04000EC9 RID: 3785
		immediatePersue,
		// Token: 0x04000ECA RID: 3786
		alarm
	}

	// Token: 0x02000238 RID: 568
	public enum SurfaceType
	{
		// Token: 0x04000ECC RID: 3788
		concrete,
		// Token: 0x04000ECD RID: 3789
		woodenFloor,
		// Token: 0x04000ECE RID: 3790
		tile,
		// Token: 0x04000ECF RID: 3791
		carpet
	}

	// Token: 0x02000239 RID: 569
	public enum StopType
	{
		// Token: 0x04000ED1 RID: 3793
		immediate,
		// Token: 0x04000ED2 RID: 3794
		fade,
		// Token: 0x04000ED3 RID: 3795
		triggerCue
	}

	// Token: 0x0200023A RID: 570
	public struct FMODParam
	{
		// Token: 0x04000ED4 RID: 3796
		public string name;

		// Token: 0x04000ED5 RID: 3797
		public float value;
	}
}
