using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200032D RID: 813
public class MaterialsController : MonoBehaviour
{
	// Token: 0x1700008F RID: 143
	// (get) Token: 0x06001248 RID: 4680 RVA: 0x00103DB8 File Offset: 0x00101FB8
	public static MaterialsController Instance
	{
		get
		{
			return MaterialsController._instance;
		}
	}

	// Token: 0x06001249 RID: 4681 RVA: 0x00103DBF File Offset: 0x00101FBF
	private void Awake()
	{
		if (MaterialsController._instance != null && MaterialsController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		MaterialsController._instance = this;
	}

	// Token: 0x0600124A RID: 4682 RVA: 0x00103DF0 File Offset: 0x00101FF0
	public Material SetMaterialGroup(GameObject model, MaterialGroupPreset preset, Toolbox.MaterialKey key, bool forceUniqueInstance = false, MeshRenderer renderer = null)
	{
		if (model == null || preset == null)
		{
			return null;
		}
		if (key == null)
		{
			key = new Toolbox.MaterialKey();
		}
		Material material = preset.material;
		bool flag = false;
		Color color = material.GetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY);
		if (key.mainColour != Color.clear && key.mainColour != color)
		{
			color = key.mainColour;
			flag = true;
		}
		Color color2 = Color.clear;
		Color color3 = Color.clear;
		Color color4 = Color.clear;
		bool flag2 = material.HasProperty(MaterialsController.MATERIAL_COLOR1_KEY);
		if (flag2)
		{
			color2 = material.GetColor(MaterialsController.MATERIAL_COLOR1_KEY);
			if (key.colour1 != Color.clear)
			{
				color2 = key.colour1;
				flag = true;
			}
			color3 = material.GetColor(MaterialsController.MATERIAL_COLOR2_KEY);
			if (key.colour2 != Color.clear)
			{
				color3 = key.colour2;
				flag = true;
			}
			color4 = material.GetColor(MaterialsController.MATERIAL_COLOR3_KEY);
			if (key.colour3 != Color.clear)
			{
				color4 = key.colour3;
				flag = true;
			}
		}
		if (!flag && !forceUniqueInstance)
		{
			if (renderer != null)
			{
				renderer.sharedMaterial = preset.material;
			}
			else
			{
				try
				{
					model.GetComponent<MeshRenderer>().sharedMaterial = preset.material;
				}
				catch
				{
				}
			}
			this.useOfBaseMaterials++;
			return preset.material;
		}
		Toolbox.MaterialKey materialKey = new Toolbox.MaterialKey();
		materialKey.baseMaterial = preset.material;
		materialKey.mainColour = key.mainColour;
		materialKey.colour1 = key.colour1;
		materialKey.colour2 = key.colour2;
		materialKey.colour3 = key.colour3;
		materialKey.grubiness = Toolbox.Instance.RoundToPlaces(key.grubiness, 1);
		Material material2 = null;
		if (!forceUniqueInstance && this.commonMaterialsLibrary.TryGetValue(materialKey, ref material2))
		{
			if (renderer != null)
			{
				renderer.sharedMaterial = material2;
			}
			else
			{
				MeshRenderer component = model.GetComponent<MeshRenderer>();
				if (component != null)
				{
					component.sharedMaterial = material2;
				}
			}
			this.materialInstancesAvertedByCommonDictionary++;
			return material2;
		}
		Material material3 = Object.Instantiate<Material>(preset.material);
		material3.name = preset.material.name + " [" + materialKey.GetHashCode().ToString() + "]";
		material3.SetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY, color);
		if (flag2)
		{
			material3.SetColor(MaterialsController.MATERIAL_COLOR1_KEY, color2);
			material3.SetColor(MaterialsController.MATERIAL_COLOR2_KEY, color3);
			material3.SetColor(MaterialsController.MATERIAL_COLOR3_KEY, color4);
			material3.SetFloat(MaterialsController.MATERIAL_GRUB_AMOUNT_KEY, key.grubiness);
		}
		if (forceUniqueInstance)
		{
			if (Game.Instance.devMode && Game.Instance.collectDebugData)
			{
				if (!this.uniqueMaterialsLibrary.ContainsKey(materialKey))
				{
					this.uniqueMaterialsLibrary.Add(materialKey, new List<Material>());
				}
				this.uniqueMaterialsLibrary[materialKey].Add(material3);
			}
		}
		else if (!this.commonMaterialsLibrary.ContainsKey(materialKey))
		{
			this.commonMaterialsLibrary.Add(materialKey, material3);
		}
		this.materialCount++;
		if (renderer != null)
		{
			renderer.sharedMaterial = material3;
		}
		else
		{
			MeshRenderer component2 = model.GetComponent<MeshRenderer>();
			if (component2 != null)
			{
				component2.sharedMaterial = material3;
			}
		}
		return material3;
	}

	// Token: 0x0600124B RID: 4683 RVA: 0x0010415C File Offset: 0x0010235C
	public Material ApplyMaterialKey(GameObject model, Toolbox.MaterialKey key)
	{
		if (model == null)
		{
			return null;
		}
		if (model.CompareTag(MaterialsController.MATERIAL_NO_MAT_COLOUR_KEY))
		{
			return null;
		}
		MeshRenderer component = model.GetComponent<MeshRenderer>();
		return this.ApplyMaterialKey(component, key);
	}

	// Token: 0x0600124C RID: 4684 RVA: 0x00104194 File Offset: 0x00102394
	public Material ApplyMaterialKey(MeshRenderer renderer, Toolbox.MaterialKey key)
	{
		if (renderer == null)
		{
			return null;
		}
		if (renderer.gameObject.CompareTag(MaterialsController.MATERIAL_NO_MAT_COLOUR_KEY))
		{
			return null;
		}
		if (renderer.gameObject.CompareTag(MaterialsController.MATERIAL_RAIN_WINDOW_GLASS_KEY))
		{
			return null;
		}
		if (key == null)
		{
			key = new Toolbox.MaterialKey();
		}
		Material sharedMaterial = renderer.sharedMaterial;
		if (sharedMaterial == null)
		{
			return null;
		}
		if (!sharedMaterial.HasProperty(MaterialsController.MATERIAL_BASE_COLOR_KEY))
		{
			return null;
		}
		bool flag = false;
		Color color = Color.clear;
		color = sharedMaterial.GetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY);
		if (key.mainColour != Color.clear && key.mainColour != color)
		{
			color = key.mainColour;
			flag = true;
		}
		Color color2 = Color.clear;
		Color color3 = Color.clear;
		Color color4 = Color.clear;
		bool flag2 = sharedMaterial.HasProperty(MaterialsController.MATERIAL_COLOR1_KEY);
		if (flag2)
		{
			color2 = sharedMaterial.GetColor(MaterialsController.MATERIAL_COLOR1_KEY);
			if (key.colour1 != Color.clear)
			{
				color2 = key.colour1;
				flag = true;
			}
			color3 = sharedMaterial.GetColor(MaterialsController.MATERIAL_COLOR2_KEY);
			if (key.colour2 != Color.clear)
			{
				color3 = key.colour2;
				flag = true;
			}
			color4 = sharedMaterial.GetColor(MaterialsController.MATERIAL_COLOR3_KEY);
			if (key.colour3 != Color.clear)
			{
				color4 = key.colour3;
				flag = true;
			}
			if (key.grubiness != 0f)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			renderer.sharedMaterial = sharedMaterial;
			this.useOfBaseMaterials++;
			return sharedMaterial;
		}
		Toolbox.MaterialKey materialKey = new Toolbox.MaterialKey();
		materialKey.baseMaterial = sharedMaterial;
		materialKey.mainColour = key.mainColour;
		materialKey.colour1 = key.colour1;
		materialKey.colour2 = key.colour2;
		materialKey.colour3 = key.colour3;
		materialKey.grubiness = Toolbox.Instance.RoundToPlaces(key.grubiness, 1);
		Material material = null;
		if (this.commonMaterialsLibrary.TryGetValue(materialKey, ref material))
		{
			renderer.sharedMaterial = material;
			this.materialInstancesAvertedByCommonDictionary++;
			return material;
		}
		Material material2 = Object.Instantiate<Material>(sharedMaterial);
		material2.name = sharedMaterial.name + " [" + materialKey.GetHashCode().ToString() + "]";
		material2.SetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY, color);
		if (flag2)
		{
			material2.SetColor(MaterialsController.MATERIAL_COLOR1_KEY, color2);
			material2.SetColor(MaterialsController.MATERIAL_COLOR2_KEY, color3);
			material2.SetColor(MaterialsController.MATERIAL_COLOR3_KEY, color4);
			material2.SetFloat(MaterialsController.MATERIAL_GRUB_AMOUNT_KEY, key.grubiness);
		}
		this.commonMaterialsLibrary.Add(materialKey, material2);
		this.materialCount++;
		renderer.sharedMaterial = material2;
		return material2;
	}

	// Token: 0x0600124D RID: 4685 RVA: 0x00104448 File Offset: 0x00102648
	public Material GetMaterialFromKey(Toolbox.MaterialKey key)
	{
		Material baseMaterial = key.baseMaterial;
		if (baseMaterial == null)
		{
			return null;
		}
		if (!baseMaterial.HasProperty(MaterialsController.MATERIAL_BASE_COLOR_KEY))
		{
			return null;
		}
		if (key == null)
		{
			key = new Toolbox.MaterialKey();
		}
		bool flag = false;
		Color color = Color.clear;
		color = baseMaterial.GetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY);
		if (key.mainColour != Color.clear && key.mainColour != color)
		{
			color = key.mainColour;
			flag = true;
		}
		Color color2 = Color.clear;
		Color color3 = Color.clear;
		Color color4 = Color.clear;
		bool flag2 = baseMaterial.HasProperty(MaterialsController.MATERIAL_COLOR1_KEY);
		if (flag2)
		{
			color2 = baseMaterial.GetColor(MaterialsController.MATERIAL_COLOR1_KEY);
			if (key.colour1 != Color.clear)
			{
				color2 = key.colour1;
				flag = true;
			}
			color3 = baseMaterial.GetColor(MaterialsController.MATERIAL_COLOR2_KEY);
			if (key.colour2 != Color.clear)
			{
				color3 = key.colour2;
				flag = true;
			}
			color4 = baseMaterial.GetColor(MaterialsController.MATERIAL_COLOR3_KEY);
			if (key.colour3 != Color.clear)
			{
				color4 = key.colour3;
				flag = true;
			}
			if (key.grubiness != 0f)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			this.useOfBaseMaterials++;
			return baseMaterial;
		}
		Material result = null;
		if (this.commonMaterialsLibrary.TryGetValue(key, ref result))
		{
			this.materialInstancesAvertedByCommonDictionary++;
			return result;
		}
		Material material = Object.Instantiate<Material>(baseMaterial);
		material.name = baseMaterial.name + " [" + key.GetHashCode().ToString() + "]";
		material.SetColor(MaterialsController.MATERIAL_BASE_COLOR_KEY, color);
		if (flag2)
		{
			material.SetColor(MaterialsController.MATERIAL_COLOR1_KEY, color2);
			material.SetColor(MaterialsController.MATERIAL_COLOR2_KEY, color3);
			material.SetColor(MaterialsController.MATERIAL_COLOR3_KEY, color4);
			material.SetFloat(MaterialsController.MATERIAL_GRUB_AMOUNT_KEY, key.grubiness);
		}
		this.commonMaterialsLibrary.Add(key, material);
		this.materialCount++;
		return material;
	}

	// Token: 0x0600124E RID: 4686 RVA: 0x00104654 File Offset: 0x00102854
	public Toolbox.MaterialKey GenerateMaterialKey(MaterialGroupPreset.MaterialVariation variation, ColourSchemePreset scheme, NewRoom room, bool useGrubiness, NewBuilding building = null)
	{
		Toolbox.MaterialKey materialKey = new Toolbox.MaterialKey();
		materialKey.baseMaterial = null;
		materialKey.mainColour = this.GetColourFromScheme(scheme, variation.main, room, building);
		materialKey.colour1 = this.GetColourFromScheme(scheme, variation.colour1, room, building);
		materialKey.colour2 = this.GetColourFromScheme(scheme, variation.colour2, room, building);
		materialKey.colour3 = this.GetColourFromScheme(scheme, variation.colour3, room, building);
		if (building == null && room != null)
		{
			building = room.building;
		}
		if (useGrubiness)
		{
			if (building != null && building.preset != null && building.preset.overrideGrubiness)
			{
				materialKey.grubiness = building.preset.grubinessOverride;
			}
			else if (room != null && !room.gameLocation.isOutside)
			{
				if (room.gameLocation.thisAsAddress != null)
				{
					if (SessionData.Instance.isFloorEdit)
					{
						materialKey.grubiness = 0f;
					}
					else if (room.gameLocation.thisAsAddress.residence != null && room.gameLocation.thisAsAddress.inhabitants.Count > 0)
					{
						if (Player.Instance.home == room.gameLocation)
						{
							materialKey.grubiness = 0f;
						}
						else
						{
							materialKey.grubiness = (float)Mathf.RoundToInt(Mathf.Min(1f - room.gameLocation.thisAsAddress.maxConscientiousness, 1f - room.gameLocation.thisAsAddress.normalizedLandValue) * 10f) / 10f;
						}
					}
					else if (room.gameLocation.isLobby)
					{
						materialKey.grubiness = (float)Mathf.RoundToInt(1f - (float)building.cityTile.landValue * 0.25f * 10f) / 10f;
					}
					else
					{
						materialKey.grubiness = (float)Mathf.RoundToInt((1f - room.gameLocation.thisAsAddress.normalizedLandValue) * 10f) / 10f;
					}
				}
				else
				{
					materialKey.grubiness = 0f;
				}
			}
			else if (room != null && room.gameLocation.isOutside && room.gameLocation.thisAsStreet != null)
			{
				materialKey.grubiness = (float)Mathf.RoundToInt((1f - room.gameLocation.thisAsStreet.normalizedFootfall) * 10f) / 10f;
			}
			else if (building != null && building.cityTile != null)
			{
				materialKey.grubiness = (float)building.cityTile.landValue * 2.5f / 10f;
			}
			else
			{
				materialKey.grubiness = 0f;
			}
			if (room != null && room.preset != null)
			{
				materialKey.grubiness = Mathf.Clamp(materialKey.grubiness, room.preset.minimumGrubiness, room.preset.maximumGrubiness);
			}
			if (room != null && room.building != null && room.building.preset != null && Game.Instance.allowEchelons && room.building.preset.buildingFeaturesEchelonFloors && room.floor != null && room.floor.floor >= room.building.preset.echelonFloorStart)
			{
				materialKey.grubiness = 0f;
			}
		}
		else
		{
			materialKey.grubiness = 0f;
		}
		return materialKey;
	}

	// Token: 0x0600124F RID: 4687 RVA: 0x00104A10 File Offset: 0x00102C10
	public void ApplyMaterial(GameObject model, Material mat)
	{
		if (model == null)
		{
			return;
		}
		MeshRenderer component = model.GetComponent<MeshRenderer>();
		this.ApplyMaterial(component, mat);
	}

	// Token: 0x06001250 RID: 4688 RVA: 0x00104A36 File Offset: 0x00102C36
	public void ApplyMaterial(MeshRenderer renderer, Material mat)
	{
		if (renderer != null)
		{
			renderer.sharedMaterial = mat;
		}
	}

	// Token: 0x06001251 RID: 4689 RVA: 0x00104A48 File Offset: 0x00102C48
	public Color GetColourFromScheme(ColourSchemePreset scheme, MaterialGroupPreset.MaterialColour colourType, NewRoom room, NewBuilding building = null)
	{
		if (SessionData.Instance.isTestScene)
		{
			return Color.white;
		}
		if (colourType == MaterialGroupPreset.MaterialColour.none)
		{
			return Color.clear;
		}
		List<Color> list = new List<Color>();
		if (scheme == null && colourType != MaterialGroupPreset.MaterialColour.wood)
		{
			string[] array = new string[6];
			array[0] = "Trying to get colour from null scheme at ";
			array[1] = room.name;
			array[2] = "( ";
			int num = 3;
			RoomConfiguration preset = room.preset;
			array[num] = ((preset != null) ? preset.ToString() : null);
			array[4] = ") returning white...";
			int num2 = 5;
			ColourSchemePreset colourScheme = room.colourScheme;
			array[num2] = ((colourScheme != null) ? colourScheme.ToString() : null);
			Game.LogError(string.Concat(array), 2);
			return Color.white;
		}
		if (colourType == MaterialGroupPreset.MaterialColour.any)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.primary2);
			list.Add(scheme.secondary1);
			list.Add(scheme.secondary2);
			list.Add(scheme.neutral);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.anyPrimary)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.primary2);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.anyPrimaryOrSecondary)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.primary2);
			list.Add(scheme.secondary1);
			list.Add(scheme.secondary2);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.anyPrimaryOrNeutral)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.primary2);
			list.Add(scheme.neutral);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.anySecondary)
		{
			list.Add(scheme.secondary1);
			list.Add(scheme.secondary2);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.anySecondaryOrNeutral)
		{
			list.Add(scheme.secondary1);
			list.Add(scheme.secondary2);
			list.Add(scheme.neutral);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.any1)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.secondary1);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.any2)
		{
			list.Add(scheme.primary2);
			list.Add(scheme.secondary2);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.any1OrNeutral)
		{
			list.Add(scheme.primary1);
			list.Add(scheme.secondary1);
			list.Add(scheme.neutral);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.any2OrNeutral)
		{
			list.Add(scheme.primary2);
			list.Add(scheme.secondary2);
			list.Add(scheme.neutral);
		}
		else if (colourType == MaterialGroupPreset.MaterialColour.neutral)
		{
			list.Add(scheme.neutral);
		}
		else
		{
			if (colourType == MaterialGroupPreset.MaterialColour.primary1)
			{
				return scheme.primary1;
			}
			if (colourType == MaterialGroupPreset.MaterialColour.primary2)
			{
				return scheme.primary2;
			}
			if (colourType == MaterialGroupPreset.MaterialColour.secondary1)
			{
				return scheme.secondary1;
			}
			if (colourType == MaterialGroupPreset.MaterialColour.secondary2)
			{
				return scheme.secondary2;
			}
			if (colourType == MaterialGroupPreset.MaterialColour.neutral)
			{
				return scheme.neutral;
			}
			if (colourType == MaterialGroupPreset.MaterialColour.wood)
			{
				if (room != null && room.gameLocation != null && room.gameLocation.thisAsAddress != null)
				{
					return room.gameLocation.thisAsAddress.wood;
				}
				if (building != null)
				{
					return building.wood;
				}
				return Color.cyan;
			}
		}
		if (list.Count <= 0)
		{
			return Color.clear;
		}
		if (room != null)
		{
			return list[Toolbox.Instance.GetPsuedoRandomNumber(0, list.Count, (room.roomID * list.Count).ToString(), false)];
		}
		string seedInput = string.Empty;
		if (building != null)
		{
			seedInput = (building.buildingID * building.buildingID).ToString() + building.cityTile.cityCoord.ToString() + CityData.Instance.seed + (list.Count * list.Count).ToString();
		}
		else
		{
			seedInput = CityData.Instance.seed + (list.Count * list.Count).ToString();
		}
		string text;
		return list[Toolbox.Instance.RandContained(0, list.Count, seedInput, out text)];
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x00104E38 File Offset: 0x00103038
	public Material GetFootprintMaterial(FootprintController fc)
	{
		if (fc == null)
		{
			return null;
		}
		MaterialsController.FootprintMaterialKey footprintMaterialKey = default(MaterialsController.FootprintMaterialKey);
		if (fc.human != null)
		{
			footprintMaterialKey.type = (int)fc.human.descriptors.footwear;
			footprintMaterialKey.blood = fc.footprint.bl;
			footprintMaterialKey.strength = fc.footprint.str;
			if (this.footprintMaterialLibrary.ContainsKey(footprintMaterialKey))
			{
				this.footprintInstancesAvertedByDictionary++;
				return this.footprintMaterialLibrary[footprintMaterialKey];
			}
			Material material = null;
			if (fc.human.descriptors.footwear == Human.ShoeType.normal)
			{
				material = Object.Instantiate<Material>(this.footprintMaterialShoe);
			}
			else if (fc.human.descriptors.footwear == Human.ShoeType.boots)
			{
				material = Object.Instantiate<Material>(this.footprintMaterialBoot);
			}
			else if (fc.human.descriptors.footwear == Human.ShoeType.heel)
			{
				material = Object.Instantiate<Material>(this.footprintMaterialHeel);
			}
			if (material != null)
			{
				Color color = Color.Lerp(this.dirtColour, this.bloodColour, fc.footprint.bl);
				color.a = Mathf.Max(fc.footprint.str, fc.footprint.bl);
				if (material != null)
				{
					material.SetColor("_BaseColor", color);
				}
				this.footprintMaterialLibrary.Add(footprintMaterialKey, material);
				this.footprintMaterials++;
				return material;
			}
		}
		return null;
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x00104FAC File Offset: 0x001031AC
	[Button(null, 0)]
	public void PopulateDebugData()
	{
		this.commonMaterialsDebug.Clear();
		foreach (KeyValuePair<Toolbox.MaterialKey, Material> keyValuePair in this.commonMaterialsLibrary)
		{
			MaterialsController.MaterialDebug materialDebug = new MaterialsController.MaterialDebug
			{
				name = keyValuePair.Value.name,
				key = keyValuePair.Key,
				mat = keyValuePair.Value
			};
			this.commonMaterialsDebug.Add(materialDebug);
		}
		this.commonMaterialsDebug.Sort((MaterialsController.MaterialDebug p1, MaterialsController.MaterialDebug p2) => p1.name.CompareTo(p2.name));
		this.uniqueMaterialsDebug.Clear();
		foreach (KeyValuePair<Toolbox.MaterialKey, List<Material>> keyValuePair2 in this.uniqueMaterialsLibrary)
		{
			foreach (Material material in keyValuePair2.Value)
			{
				if (!(material == null))
				{
					MaterialsController.MaterialDebug materialDebug2 = new MaterialsController.MaterialDebug
					{
						name = material.name,
						key = keyValuePair2.Key,
						mat = material
					};
					this.uniqueMaterialsDebug.Add(materialDebug2);
				}
			}
		}
		this.uniqueMaterialsDebug.Sort((MaterialsController.MaterialDebug p1, MaterialsController.MaterialDebug p2) => p1.name.CompareTo(p2.name));
	}

	// Token: 0x04001678 RID: 5752
	[Header("Materials Library")]
	public Dictionary<Toolbox.MaterialKey, Material> commonMaterialsLibrary = new Dictionary<Toolbox.MaterialKey, Material>();

	// Token: 0x04001679 RID: 5753
	public Dictionary<Toolbox.MaterialKey, List<Material>> uniqueMaterialsLibrary = new Dictionary<Toolbox.MaterialKey, List<Material>>();

	// Token: 0x0400167A RID: 5754
	public Dictionary<MaterialsController.FootprintMaterialKey, Material> footprintMaterialLibrary = new Dictionary<MaterialsController.FootprintMaterialKey, Material>();

	// Token: 0x0400167B RID: 5755
	[Space(5f)]
	[ReadOnly]
	[InfoBox("How many total materials have been created through this material library system", 0)]
	public int materialCount;

	// Token: 0x0400167C RID: 5756
	[Space(5f)]
	[ReadOnly]
	[InfoBox("How many material instances have been 'saved' from new instantiation afresh because of the dictionary", 0)]
	public int materialInstancesAvertedByCommonDictionary;

	// Token: 0x0400167D RID: 5757
	[Space(5f)]
	[ReadOnly]
	[InfoBox("How many non-instances can be used (ie direct use of the base material)", 0)]
	public int useOfBaseMaterials;

	// Token: 0x0400167E RID: 5758
	[Space(5f)]
	[ReadOnly]
	[InfoBox("How many material instances are created by lights? (On/off instanced of materials assigned to lights)", 0)]
	public int lightMaterialInstances;

	// Token: 0x0400167F RID: 5759
	[Space(5f)]
	[ReadOnly]
	public int footprintMaterials;

	// Token: 0x04001680 RID: 5760
	[ReadOnly]
	[InfoBox("How many material instances have been 'saved' from new instantiation afresh because of the dictionary", 0)]
	[Space(5f)]
	public int footprintInstancesAvertedByDictionary;

	// Token: 0x04001681 RID: 5761
	[Header("Footprint Settings")]
	public Material footprintMaterialShoe;

	// Token: 0x04001682 RID: 5762
	public Material footprintMaterialBoot;

	// Token: 0x04001683 RID: 5763
	public Material footprintMaterialHeel;

	// Token: 0x04001684 RID: 5764
	public Color dirtColour = Color.black;

	// Token: 0x04001685 RID: 5765
	public Color bloodColour = Color.red;

	// Token: 0x04001686 RID: 5766
	[Space(7f)]
	public List<MaterialsController.MaterialDebug> commonMaterialsDebug = new List<MaterialsController.MaterialDebug>();

	// Token: 0x04001687 RID: 5767
	public List<MaterialsController.MaterialDebug> uniqueMaterialsDebug = new List<MaterialsController.MaterialDebug>();

	// Token: 0x04001688 RID: 5768
	private static MaterialsController _instance;

	// Token: 0x04001689 RID: 5769
	private static readonly string MATERIAL_NO_MAT_COLOUR_KEY = "NoMatColour";

	// Token: 0x0400168A RID: 5770
	private static readonly string MATERIAL_RAIN_WINDOW_GLASS_KEY = "RainWindowGlass";

	// Token: 0x0400168B RID: 5771
	private static readonly string MATERIAL_BASE_COLOR_KEY = "_BaseColor";

	// Token: 0x0400168C RID: 5772
	private static readonly string MATERIAL_COLOR1_KEY = "_Color1";

	// Token: 0x0400168D RID: 5773
	private static readonly string MATERIAL_COLOR2_KEY = "_Color2";

	// Token: 0x0400168E RID: 5774
	private static readonly string MATERIAL_COLOR3_KEY = "_Color3";

	// Token: 0x0400168F RID: 5775
	private static readonly string MATERIAL_GRUB_AMOUNT_KEY = "_GrubAmount";

	// Token: 0x0200032E RID: 814
	[Serializable]
	public class MaterialDebug
	{
		// Token: 0x04001690 RID: 5776
		public string name;

		// Token: 0x04001691 RID: 5777
		public Toolbox.MaterialKey key;

		// Token: 0x04001692 RID: 5778
		public Material mat;
	}

	// Token: 0x0200032F RID: 815
	[Serializable]
	public struct FootprintMaterialKey
	{
		// Token: 0x06001257 RID: 4695 RVA: 0x00105213 File Offset: 0x00103413
		public bool Equals(MaterialsController.FootprintMaterialKey other)
		{
			return object.Equals(other, this);
		}

		// Token: 0x06001258 RID: 4696 RVA: 0x0010522C File Offset: 0x0010342C
		public override bool Equals(object obj)
		{
			if (obj == null || base.GetType() != obj.GetType())
			{
				return false;
			}
			MaterialsController.FootprintMaterialKey footprintMaterialKey = (MaterialsController.FootprintMaterialKey)obj;
			return footprintMaterialKey.type == this.type && Mathf.Approximately(footprintMaterialKey.strength, this.strength) && Mathf.Approximately(footprintMaterialKey.blood, this.blood);
		}

		// Token: 0x06001259 RID: 4697 RVA: 0x00105298 File Offset: 0x00103498
		public override int GetHashCode()
		{
			HashCode hashCode = default(HashCode);
			hashCode.Add<int>(this.type);
			hashCode.Add<float>(this.strength);
			hashCode.Add<float>(this.blood);
			return hashCode.ToHashCode();
		}

		// Token: 0x0600125A RID: 4698 RVA: 0x001052DB File Offset: 0x001034DB
		public static bool operator ==(MaterialsController.FootprintMaterialKey c1, MaterialsController.FootprintMaterialKey c2)
		{
			return c1.Equals(c2);
		}

		// Token: 0x0600125B RID: 4699 RVA: 0x001052E5 File Offset: 0x001034E5
		public static bool operator !=(MaterialsController.FootprintMaterialKey c1, MaterialsController.FootprintMaterialKey c2)
		{
			return !c1.Equals(c2);
		}

		// Token: 0x04001693 RID: 5779
		public int type;

		// Token: 0x04001694 RID: 5780
		public float strength;

		// Token: 0x04001695 RID: 5781
		public float blood;
	}
}
