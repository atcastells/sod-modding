using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x0200010E RID: 270
public class LightController : MonoBehaviour
{
	// Token: 0x060007BF RID: 1983 RVA: 0x00075374 File Offset: 0x00073574
	public void Setup(NewRoom newRoom, Interactable newInteractable, Interactable.LightConfiguration configData, LightingPreset newPreset, int lightZoneSize = -1, Transform newCeilingFan = null)
	{
		this.preset = newPreset;
		this.room = newRoom;
		this.interactable = newInteractable;
		this.ceilingFan = newCeilingFan;
		if (this.lightComponent == null)
		{
			this.lightComponent = base.gameObject.GetComponentInChildren<Light>();
		}
		if (this.hdrpLightData == null)
		{
			this.hdrpLightData = base.gameObject.GetComponentInChildren<HDAdditionalLightData>();
		}
		if (this.rend == null)
		{
			this.rend = base.transform.parent.GetComponent<MeshRenderer>();
		}
		if (this.mat == null && this.rend != null)
		{
			if (this.preset.useInstancedEmissive)
			{
				Material material = Object.Instantiate<Material>(this.rend.sharedMaterial);
				Object @object = material;
				Material sharedMaterial = this.rend.sharedMaterial;
				@object.name = ((sharedMaterial != null) ? sharedMaterial.ToString() : null) + " [Instanced by Light]";
				this.mat = material;
				this.rend.sharedMaterial = material;
				MaterialsController.Instance.lightMaterialInstances++;
			}
			else
			{
				this.mat = this.rend.sharedMaterial;
			}
		}
		string seedInput = string.Empty;
		if (this.interactable != null)
		{
			seedInput = this.interactable.seed;
		}
		else
		{
			seedInput = Toolbox.Instance.SeedRand(0, 9999999).ToString();
		}
		if (configData == null)
		{
			configData = new Interactable.LightConfiguration();
			Color white = Color.white;
			Color color = Color.Lerp(this.preset.coolColours[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.coolColours.Count, seedInput, out seedInput)].colourOne, this.preset.warmColours[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.warmColours.Count, seedInput, out seedInput)].colourOne, Toolbox.Instance.GetPsuedoRandomNumberContained(0.33f, 0.66f, seedInput, out seedInput));
			Color color2 = Color.Lerp(this.preset.coolColours[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.coolColours.Count, seedInput, out seedInput)].colourTwo, this.preset.warmColours[Toolbox.Instance.GetPsuedoRandomNumberContained(0, this.preset.warmColours.Count, seedInput, out seedInput)].colourTwo, Toolbox.Instance.GetPsuedoRandomNumberContained(0.33f, 0.66f, seedInput, out seedInput));
			configData.colour = Color.Lerp(color, color2, 0.5f);
			configData.range = this.preset.defaultRange;
			float num = this.preset.defaultIntensity;
			float num2 = 1f;
			if (this.room != null)
			{
				if (this.room.lowerRoom != null)
				{
					num2 = 1.33f;
				}
				if (lightZoneSize < 0)
				{
					num = (float)this.room.nodes.Count * num2 * 100f - 50f;
				}
				else
				{
					num = (float)lightZoneSize * num2 * 100f - 50f;
				}
				if (this.room.preset.wellLit)
				{
					num *= 1.5f;
					configData.range *= 3f;
				}
			}
			configData.intensity = Mathf.Clamp(num, this.preset.intensityRange.x, this.preset.intensityRange.y);
			configData.flickerColourMultiplier = Toolbox.Instance.GetPsuedoRandomNumberContained(this.preset.flickerMultiplierRange.x, this.preset.flickerMultiplierRange.y, seedInput, out seedInput);
			configData.pulseSpeed = Toolbox.Instance.GetPsuedoRandomNumberContained(this.preset.flickerPulseRange.x, this.preset.flickerPulseRange.y, seedInput, out seedInput);
			configData.intervalTime = Toolbox.Instance.GetPsuedoRandomNumberContained(this.preset.flickerNormalityIntervalRange.x, this.preset.flickerNormalityIntervalRange.y, seedInput, out seedInput);
			float num3 = 0f;
			if (this.room != null && this.room.gameLocation.floor != null && this.room.gameLocation.floor.floor <= CityControls.Instance.lowestFloor)
			{
				num3 = CityControls.Instance.lowestFloorIncreaseFlickerChance;
			}
			if (this.preset.chanceOfFlicker + num3 >= Toolbox.Instance.GetPsuedoRandomNumberContained(0f, 1f, seedInput, out seedInput))
			{
				configData.flicker = true;
			}
			else
			{
				configData.flicker = false;
			}
		}
		if (this.room == null || this.room.building == null)
		{
			this.hdrpLightData.lightlayersMask = 2;
		}
		else if (this.room.building != null)
		{
			if (this.room.building.interiorLightCullingLayer == 0)
			{
				this.hdrpLightData.lightlayersMask = 4;
			}
			else if (this.room.building.interiorLightCullingLayer == 1)
			{
				this.hdrpLightData.lightlayersMask = 8;
			}
			else if (this.room.building.interiorLightCullingLayer == 2)
			{
				this.hdrpLightData.lightlayersMask = 16;
			}
			else if (this.room.building.interiorLightCullingLayer == 3)
			{
				this.hdrpLightData.lightlayersMask = 32;
			}
			if ((this.room.floor != null && this.room.floor.floor == 0) || (this.interactable != null && this.interactable.preset.forceIncludeOnStreetLightLayer) || this.room.IsOutside())
			{
				this.hdrpLightData.lightlayersMask = Toolbox.Instance.CreateLayerMask(Toolbox.LayerMaskMode.onlyCast, new int[]
				{
					this.room.building.interiorLightCullingLayer + 2,
					1
				});
			}
			this.room.building.allInteriorMainLights.Add(this);
		}
		if (base.transform.parent != null && base.transform.parent.gameObject.layer != 23 && base.transform.gameObject.layer != 23 && this.interactable != null)
		{
			Game.Log(string.Concat(new string[]
			{
				"Misc Error: Light fitting ",
				base.transform.parent.name,
				" (",
				this.interactable.name,
				") is not on layer 23, and therefore might not work properly with light level raycasting..."
			}), 2);
		}
		if (SessionData.Instance.isTestScene)
		{
			this.hdrpLightData.lightlayersMask = 255;
		}
		this.SetShadows(this.preset.enableShadows);
		this.SetVolumetrics(this.preset.enableVolumetrics);
		if (this.preset.enableShadows && this.room != null && this.room.preset.baseLightingShadowTint)
		{
			this.hdrpLightData.shadowTint = this.room.GetShadowTint(configData.colour, this.room.preset.baseLightingShadowTintIntensity);
		}
		if (this.room != null && !this.room.IsOutside())
		{
			this.hdrpLightData.penumbraTint = true;
		}
		this.hdrpLightData.fadeDistance = this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier;
		this.isOn = this.lightComponent.enabled;
		this.ceilingFanOn = this.lightComponent.enabled;
		this.SetColour(configData.colour);
		this.SetIntensity(configData.intensity);
		this.lightComponent.range = configData.range;
		this.flickerColourMultiplier = configData.flickerColourMultiplier;
		this.pulseSpeed = configData.pulseSpeed;
		this.intervalTime = configData.intervalTime;
		this.SetFlicker(configData.flicker);
		if (this.preset.onByDefault)
		{
			this.SetOn(true, true);
		}
		else if (this.interactable.preset.lightswitch == InteractablePreset.Switch.switchState)
		{
			this.SetOn(this.interactable.sw0, true);
		}
		else if (this.interactable.preset.lightswitch == InteractablePreset.Switch.custom1)
		{
			this.SetOn(this.interactable.sw1, true);
		}
		else if (this.interactable.preset.lightswitch == InteractablePreset.Switch.custom2)
		{
			this.SetOn(this.interactable.sw2, true);
		}
		else if (this.interactable.preset.lightswitch == InteractablePreset.Switch.custom3)
		{
			this.SetOn(this.interactable.sw3, true);
		}
		if (this.preset.isAtriumLight)
		{
			float num4 = PathFinder.Instance.tileSize.z;
			Vector3Int vector3Int = Vector3Int.zero;
			for (int i = 1; i < 50; i++)
			{
				vector3Int = this.interactable.node.nodeCoord - new Vector3Int(0, 0, i);
				NewNode newNode = null;
				if (!PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) || newNode.floorType == NewNode.FloorTileType.floorOnly || newNode.floorType == NewNode.FloorTileType.floorAndCeiling)
				{
					break;
				}
				num4 += PathFinder.Instance.tileSize.z;
			}
			float num5 = this.preset.heightInterval;
			float num6 = 0f;
			while (num5 < num4)
			{
				if (this.preset.bulbPrefab != null)
				{
					GameObject gameObject = this.preset.bulbPrefab;
					if (num5 + this.preset.heightInterval >= num4 && this.preset.endBulbPrefab != null)
					{
						gameObject = this.preset.endBulbPrefab;
					}
					GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, base.transform.parent);
					gameObject2.transform.position = this.interactable.wPos - new Vector3(0f, num5, 0f);
					Toolbox.Instance.SetLightLayer(gameObject2, this.room.building, false);
					if (this.preset.cablePrefab != null)
					{
						GameObject gameObject3 = Object.Instantiate<GameObject>(this.preset.cablePrefab, gameObject2.transform);
						gameObject3.transform.localEulerAngles = Vector3.zero;
						gameObject3.transform.localScale = new Vector3(gameObject3.transform.localScale.x, this.preset.heightInterval, gameObject3.transform.localScale.z);
						gameObject3.transform.localPosition = new Vector3(0f, this.preset.heightInterval * 0.5f, 0f);
						Toolbox.Instance.SetLightLayer(gameObject3, this.room.building, false);
					}
				}
				num6 = num5;
				num5 += this.preset.heightInterval;
			}
			this.hdrpLightData.shapeWidth = num6;
			base.transform.localPosition = new Vector3(0f, -num6 * 0.5f - 5f, 0f);
			this.hdrpLightData.fadeDistance = Mathf.Max(this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier, this.hdrpLightData.shapeWidth);
		}
		this.isSetup = true;
		if (LightCullingController.Instance != null && Game.Instance.enableCustomLightCulling)
		{
			LightCullingController.Instance.lightsToCheck.Add(this);
		}
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x00075EF0 File Offset: 0x000740F0
	public void UpdateFadeDistances()
	{
		if (this.preset != null && this.hdrpLightData != null)
		{
			this.hdrpLightData.fadeDistance = this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier;
			this.hdrpLightData.volumetricFadeDistance = this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier;
			if (this.preset.isAtriumLight)
			{
				this.hdrpLightData.fadeDistance = Mathf.Max(this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier, this.hdrpLightData.shapeWidth);
			}
			if (this.useShadows)
			{
				this.hdrpLightData.SetShadowFadeDistance(this.preset.shadowFadeDistance * Game.Instance.shadowFadeDistanceMultiplier);
			}
		}
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x00075FC8 File Offset: 0x000741C8
	public void SetColour(Color newCol)
	{
		this.lightColour = newCol;
		if (this.lightComponent != null)
		{
			this.lightComponent.color = this.lightColour;
		}
		this.flickerColour = this.lightColour * this.flickerColourMultiplier;
		this.emissionColour = this.lightColour;
		this.emissionColour.a = this.preset.emissionMultiplier;
		if (this.mat != null && this.preset.useInstancedEmissive)
		{
			this.mat.SetColor("_EmissiveColor", this.emissionColour);
		}
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00076068 File Offset: 0x00074268
	public void SetIntensity(float newInt)
	{
		this.intensity = newInt * CityControls.Instance.weatherSettings.globalLightIntensityMultiplier;
		if (this.room != null && this.room.floor != null && this.room.floor.floor <= CityControls.Instance.lowestFloor)
		{
			this.intensity *= CityControls.Instance.lowestFloorLightMultiplier;
		}
		this.hdrpLightData.intensity = this.intensity;
	}

	// Token: 0x060007C3 RID: 1987 RVA: 0x000760F4 File Offset: 0x000742F4
	public void SetShadows(bool val)
	{
		this.useShadows = val;
		this.hdrpLightData.EnableShadows(this.useShadows);
		if (this.useShadows)
		{
			this.hdrpLightData.linkShadowLayers = true;
			this.hdrpLightData.SetShadowResolutionLevel((int)this.preset.resolution);
			this.hdrpLightData.SetShadowFadeDistance(this.preset.shadowFadeDistance * Game.Instance.shadowFadeDistanceMultiplier);
			if (Game.Instance.overrideLightControllerShadowMode)
			{
				if (Game.Instance.shadowModeOverride == LightingPreset.ShadowMode.everyFrame)
				{
					this.hdrpLightData.SetShadowUpdateMode(0);
					return;
				}
				if (Game.Instance.shadowModeOverride == LightingPreset.ShadowMode.onDemand)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					return;
				}
				if (Game.Instance.shadowModeOverride == LightingPreset.ShadowMode.onEnable)
				{
					this.hdrpLightData.SetShadowUpdateMode(1);
					return;
				}
				if (Game.Instance.shadowModeOverride == LightingPreset.ShadowMode.dynamicSystemStatic)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					return;
				}
				if (Game.Instance.shadowModeOverride == LightingPreset.ShadowMode.dynamicSystemSlowerUpdate)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					if (!CityData.Instance.dynamicShadowSystemLights.Contains(this))
					{
						CityData.Instance.dynamicShadowSystemLights.Add(this);
						return;
					}
				}
			}
			else
			{
				if (this.preset.shadowMode == LightingPreset.ShadowMode.everyFrame)
				{
					this.hdrpLightData.SetShadowUpdateMode(0);
					return;
				}
				if (this.preset.shadowMode == LightingPreset.ShadowMode.onDemand)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					return;
				}
				if (this.preset.shadowMode == LightingPreset.ShadowMode.onEnable)
				{
					this.hdrpLightData.SetShadowUpdateMode(1);
					return;
				}
				if (this.preset.shadowMode == LightingPreset.ShadowMode.dynamicSystemStatic)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					return;
				}
				if (this.preset.shadowMode == LightingPreset.ShadowMode.dynamicSystemSlowerUpdate)
				{
					this.hdrpLightData.SetShadowUpdateMode(2);
					if (!CityData.Instance.dynamicShadowSystemLights.Contains(this))
					{
						CityData.Instance.dynamicShadowSystemLights.Add(this);
					}
				}
			}
		}
	}

	// Token: 0x060007C4 RID: 1988 RVA: 0x000762C8 File Offset: 0x000744C8
	public void SetVolumetrics(bool val)
	{
		this.useVolumetrics = val;
		this.hdrpLightData.volumetricFadeDistance = this.preset.fadeDistance * Game.Instance.lightFadeDistanceMultiplier;
		float num = 1f;
		if (this.room != null)
		{
			num = this.room.preset.baseRoomAtmosphere;
		}
		this.SetVolumentricAtmosphere(num * this.preset.atmosphereMultiplier);
	}

	// Token: 0x060007C5 RID: 1989 RVA: 0x00076335 File Offset: 0x00074535
	public void SetVolumentricAtmosphere(float newVal)
	{
		this.hdrpLightData.volumetricDimmer = Mathf.Clamp01(newVal);
		this.hdrpLightData.volumetricFadeDistance = this.preset.defaultRange;
	}

	// Token: 0x060007C6 RID: 1990 RVA: 0x00076360 File Offset: 0x00074560
	public void SetFlicker(bool val)
	{
		this.flicker = val;
		if (this.flicker)
		{
			if (this.isOn)
			{
				base.gameObject.SetActive(true);
			}
			base.enabled = true;
			return;
		}
		this.flickerState = 1f;
		if ((this.isOn && this.lightState == 1f) || (!this.isOn && this.lightState == 0f))
		{
			base.enabled = false;
			if (!this.isOn && this.lightState == 0f)
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060007C7 RID: 1991 RVA: 0x000763F8 File Offset: 0x000745F8
	private void Update()
	{
		if (this.isOn && this.lightState < 1f)
		{
			this.lightState += Time.deltaTime * SessionData.Instance.currentTimeMultiplier * this.preset.fadeSpeed;
			this.lightState = Mathf.Clamp01(this.lightState);
		}
		else if (!this.isOn && this.lightState > 0f)
		{
			this.lightState -= Time.deltaTime * SessionData.Instance.currentTimeMultiplier * this.preset.fadeSpeed;
			this.lightState = Mathf.Clamp01(this.lightState);
		}
		if (this.ceilingFan != null && SessionData.Instance.play)
		{
			if (this.ceilingFanOn)
			{
				if (this.ceilingFanSpeed < InteriorControls.Instance.ceilingFanSpeed)
				{
					this.ceilingFanSpeed += 1f * Time.deltaTime;
				}
			}
			else if (this.ceilingFanSpeed > 0f)
			{
				this.ceilingFanSpeed -= 1f * Time.deltaTime;
			}
			this.ceilingFanSpeed = Mathf.Clamp(this.ceilingFanSpeed, 0f, InteriorControls.Instance.ceilingFanSpeed);
			this.ceilingFan.transform.localEulerAngles = new Vector3(0f, this.ceilingFan.transform.localEulerAngles.y + this.ceilingFanSpeed, 0f);
		}
		if (this.flicker && this.isOn)
		{
			if (SessionData.Instance.play)
			{
				if (Game.Instance.flickeringLights)
				{
					this.interval += Time.deltaTime * SessionData.Instance.currentTimeMultiplier;
				}
				else
				{
					this.interval = 0f;
				}
				if (!this.flickerInterval && this.interval >= this.intervalTime)
				{
					this.interval = 0f;
					this.flickerInterval = true;
					this.intervalTime = Toolbox.Instance.Rand(this.preset.flickerIntervalRange.x, this.preset.flickerIntervalRange.y, false);
				}
				else if (this.flickerInterval && this.interval >= this.intervalTime)
				{
					this.interval = 0f;
					this.flickerInterval = false;
					this.intervalTime = Toolbox.Instance.Rand(this.preset.flickerNormalityIntervalRange.x, this.preset.flickerNormalityIntervalRange.y, false);
				}
				if (this.flickerState < 1f && this.flickerSwitch)
				{
					this.flickerState += Time.deltaTime * this.pulseSpeed;
				}
				else if (this.flickerState >= 1f && this.flickerSwitch)
				{
					this.flickerSwitch = false;
					this.flickerColourMultiplier = Toolbox.Instance.Rand(this.preset.flickerMultiplierRange.x, this.preset.flickerMultiplierRange.y, false);
					this.pulseSpeed = Toolbox.Instance.Rand(this.preset.flickerPulseRange.x, this.preset.flickerPulseRange.y, false);
				}
				if (this.flickerInterval && this.flickerState > 0f && !this.flickerSwitch)
				{
					this.flickerState -= Time.deltaTime * this.pulseSpeed;
				}
				else if (this.flickerState <= 0f && !this.flickerSwitch)
				{
					this.flickerSwitch = true;
					this.flickerColourMultiplier = Toolbox.Instance.Rand(this.preset.flickerMultiplierRange.x, this.preset.flickerMultiplierRange.y, false);
				}
				this.flickerState = Mathf.Clamp01(this.flickerState);
				this.flickerColour = this.lightColour * this.flickerColourMultiplier;
				this.UpdateLight();
				return;
			}
		}
		else
		{
			this.UpdateLight();
			if (((this.isOn && this.lightState >= 1f) || (!this.isOn && this.lightState <= 0f)) && (this.ceilingFan == null || (!this.ceilingFanOn && this.ceilingFanSpeed == 0f)))
			{
				base.enabled = false;
				if (!this.isOn && this.lightState <= 0f)
				{
					base.gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x060007C8 RID: 1992 RVA: 0x00076868 File Offset: 0x00074A68
	public void SetOn(bool val, bool forceInstant = false)
	{
		this.isOn = val;
		this.ceilingFanOn = this.isOn;
		if (!this.preset.fadeOnOff || forceInstant)
		{
			if (this.isOn)
			{
				this.lightState = 1f;
			}
			else
			{
				this.lightState = 0f;
			}
			this.UpdateLight();
			if (!this.flicker)
			{
				if (this.isOn)
				{
					if (this.ceilingFan == null)
					{
						base.enabled = false;
					}
					if (!base.gameObject.activeSelf)
					{
						base.gameObject.SetActive(true);
					}
				}
				else if (this.ceilingFan == null)
				{
					base.enabled = false;
					base.gameObject.SetActive(false);
				}
			}
			else
			{
				if (!base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(true);
				}
				base.enabled = true;
			}
		}
		if ((this.preset.useOnMaterial != null || this.preset.useBroadcastMaterial) && this.rend != null)
		{
			if (this.isOn)
			{
				if (this.preset.useBroadcastMaterial)
				{
					this.rend.sharedMaterial = SessionData.Instance.televisionChannels[0].broadcastMaterialInstanced;
				}
				else if (this.preset.useOnMaterial != null)
				{
					this.rend.sharedMaterial = this.preset.useOnMaterial;
				}
			}
			else
			{
				this.rend.sharedMaterial = this.mat;
			}
		}
		if ((this.preset.fadeOnOff && !forceInstant) || this.ceilingFan != null)
		{
			base.enabled = true;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x060007C9 RID: 1993 RVA: 0x00076A1B File Offset: 0x00074C1B
	public void SetUnscrewed(bool val, bool forceInstance = false)
	{
		if (this.interactable != null && !this.interactable.preset.allowUnscrewed)
		{
			val = false;
		}
		this.isUnscrewed = val;
		this.UpdateLight();
	}

	// Token: 0x060007CA RID: 1994 RVA: 0x00076A47 File Offset: 0x00074C47
	public void SetClosedBreaker(bool val, bool forceInstance = false)
	{
		this.closedBreaker = val;
		this.UpdateLight();
	}

	// Token: 0x060007CB RID: 1995 RVA: 0x00076A56 File Offset: 0x00074C56
	private void OnEnable()
	{
		if (this.isSetup)
		{
			this.UpdateLight();
		}
	}

	// Token: 0x060007CC RID: 1996 RVA: 0x00076A56 File Offset: 0x00074C56
	private void OnDisable()
	{
		if (this.isSetup)
		{
			this.UpdateLight();
		}
	}

	// Token: 0x060007CD RID: 1997 RVA: 0x00076A68 File Offset: 0x00074C68
	public void UpdateLight()
	{
		if (this.lightComponent == null)
		{
			base.transform.name = Toolbox.Instance.GenerateUniqueID();
			this.lightComponent = base.gameObject.GetComponent<Light>();
			if (this.lightComponent == null)
			{
				Game.Log("Unable to find light component on " + base.transform.name, 2);
			}
			else
			{
				Game.Log("Had to assign Light Component on " + base.transform.name, 2);
			}
		}
		if (this.hdrpLightData == null)
		{
			base.transform.name = Toolbox.Instance.GenerateUniqueID();
			this.hdrpLightData = base.gameObject.GetComponent<HDAdditionalLightData>();
			if (this.hdrpLightData == null)
			{
				Game.Log("Unable to find light component on " + base.transform.name, 2);
			}
			else
			{
				Game.Log("Had to assign HDAdditionalLightData on " + base.transform.name, 2);
			}
		}
		if (this.isUnscrewed || this.closedBreaker)
		{
			this.hdrpLightData.intensity = 0f;
		}
		else
		{
			if (this.flicker && Game.Instance.flickeringLights)
			{
				this.lightComponent.color = Color.Lerp(this.flickerColour, this.lightColour, this.flickerState);
			}
			else
			{
				this.lightComponent.color = this.lightColour;
			}
			this.hdrpLightData.intensity = this.intensity * this.lightState;
			if (this.mat != null && this.preset.useInstancedEmissive)
			{
				Color color = this.lightComponent.color;
				this.mat.SetColor("_EmissiveColor", color * this.lightState);
			}
		}
		if (this.useShadows && this.isOn && this.lightComponent.enabled && this.hdrpLightData != null && (this.preset.shadowMode == LightingPreset.ShadowMode.dynamicSystemStatic || this.preset.shadowMode == LightingPreset.ShadowMode.dynamicSystemSlowerUpdate))
		{
			this.hdrpLightData.RequestShadowMapRendering();
		}
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x00076C84 File Offset: 0x00074E84
	public void SetCulled(bool val, bool respectTimer)
	{
		if (respectTimer && val && SessionData.Instance.gameTime < this.cullingTimer)
		{
			return;
		}
		if (this.isCulled != val)
		{
			this.isCulled = val;
			if (base.gameObject != null)
			{
				base.gameObject.SetActive(!this.isCulled);
			}
		}
		if (!this.isCulled)
		{
			this.cullingTimer = SessionData.Instance.gameTime + LightCullingController.Instance.minimumLightUnculledTime;
			if (Game.Instance.collectDebugData)
			{
				LightCullingController.Instance.culledLights.Remove(this);
				return;
			}
		}
		else if (Game.Instance.collectDebugData && !LightCullingController.Instance.culledLights.Contains(this))
		{
			LightCullingController.Instance.culledLights.Add(this);
		}
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00076D4B File Offset: 0x00074F4B
	[Button(null, 0)]
	public void CullToggle()
	{
		this.SetCulled(!this.isCulled, false);
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x00076D60 File Offset: 0x00074F60
	private void OnDestroy()
	{
		if (LightCullingController.Instance != null && Game.Instance.enableCustomLightCulling)
		{
			LightCullingController.Instance.lightsToCheck.Remove(this);
		}
		if (this.preset.useInstancedEmissive && this.mat != null)
		{
			MaterialsController.Instance.lightMaterialInstances--;
			Object.Destroy(this.mat);
		}
	}

	// Token: 0x040007F1 RID: 2033
	public bool isSetup;

	// Token: 0x040007F2 RID: 2034
	[Header("Location")]
	public NewRoom room;

	// Token: 0x040007F3 RID: 2035
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x040007F4 RID: 2036
	[Header("Profile")]
	public LightingPreset preset;

	// Token: 0x040007F5 RID: 2037
	[Header("Light")]
	public bool isOn = true;

	// Token: 0x040007F6 RID: 2038
	public bool isUnscrewed;

	// Token: 0x040007F7 RID: 2039
	public bool closedBreaker;

	// Token: 0x040007F8 RID: 2040
	public bool isCulled;

	// Token: 0x040007F9 RID: 2041
	public float lightState = 1f;

	// Token: 0x040007FA RID: 2042
	public Light lightComponent;

	// Token: 0x040007FB RID: 2043
	public HDAdditionalLightData hdrpLightData;

	// Token: 0x040007FC RID: 2044
	[Tooltip("Colour of the light")]
	[Space(7f)]
	public Color lightColour;

	// Token: 0x040007FD RID: 2045
	[Tooltip("If flicker is present, flicker to this colour")]
	private Color flickerColour;

	// Token: 0x040007FE RID: 2046
	[Tooltip("The model's emissive colour (instaned materials only)")]
	private Color emissionColour;

	// Token: 0x040007FF RID: 2047
	[Tooltip("Intensity of the light")]
	public float intensity = 1f;

	// Token: 0x04000800 RID: 2048
	[Tooltip("A timer that dictates the minimum amount of time this light can be unculled. Prevents flickering with frequent raycast checks.")]
	public float cullingTimer = 2f;

	// Token: 0x04000801 RID: 2049
	[Header("Model")]
	[Tooltip("Change material on this model")]
	public MeshRenderer rend;

	// Token: 0x04000802 RID: 2050
	[Tooltip("The material of the parent model: For altering emission")]
	public Material mat;

	// Token: 0x04000803 RID: 2051
	[Header("Voumetrics")]
	public bool useVolumetrics;

	// Token: 0x04000804 RID: 2052
	[Header("Shadows")]
	public bool useShadows;

	// Token: 0x04000805 RID: 2053
	[Tooltip("Does this light flicker?")]
	[Header("Flicker")]
	public bool flicker;

	// Token: 0x04000806 RID: 2054
	[Tooltip("When flickering, use this multiplier on the flicker colour to determin the actual colour (basically a darker version of flicker colour)")]
	public float flickerColourMultiplier;

	// Token: 0x04000807 RID: 2055
	public float pulseSpeed = 1f;

	// Token: 0x04000808 RID: 2056
	private float flickerState = 1f;

	// Token: 0x04000809 RID: 2057
	private bool flickerSwitch;

	// Token: 0x0400080A RID: 2058
	private bool flickerInterval;

	// Token: 0x0400080B RID: 2059
	private float interval;

	// Token: 0x0400080C RID: 2060
	private float intervalTime;

	// Token: 0x0400080D RID: 2061
	[Header("Ceiling Fan")]
	public Transform ceilingFan;

	// Token: 0x0400080E RID: 2062
	public bool ceilingFanOn = true;

	// Token: 0x0400080F RID: 2063
	public float ceilingFanSpeed;
}
