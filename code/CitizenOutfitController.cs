using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000273 RID: 627
public class CitizenOutfitController : MonoBehaviour
{
	// Token: 0x06000E64 RID: 3684 RVA: 0x000CF250 File Offset: 0x000CD450
	private void Awake()
	{
		foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.anchorConfig)
		{
			this.anchorReference.Add(anchorConfig.anchor, anchorConfig.trans);
		}
		foreach (CitizenOutfitController.ExpressionSetup expressionSetup in this.expressions)
		{
			this.expressionReference.Add(expressionSetup.expression, expressionSetup);
		}
	}

	// Token: 0x06000E65 RID: 3685 RVA: 0x000CF300 File Offset: 0x000CD500
	public void GenerateOutfits(bool forceSpecificDebugOutfit = false)
	{
		this.outfits.Clear();
		this.outfitDebug.Clear();
		List<ClothesPreset.OutfitCategory> list = null;
		List<ClothesPreset> list2 = null;
		Dictionary<string, ClothesPreset> dictionary = null;
		OccupationPreset occupationPreset = null;
		Human.Gender gender = this.human.birthGender;
		Descriptors.BuildType build = this.human.descriptors.build;
		Descriptors.HairStyle hairType = this.human.descriptors.hairType;
		Descriptors.EyeColour eyeColour = this.human.descriptors.eyeColour;
		Human.ShoeType footwear = this.human.descriptors.footwear;
		Color skinColour = this.human.descriptors.skinColour;
		Color hairColour = this.human.descriptors.hairColour;
		new Color(0.43137255f, 0.3137255f, 0.3137255f, 1f);
		bool flag = false;
		if (Toolbox.Instance == null)
		{
			list = Enumerable.ToList<ClothesPreset.OutfitCategory>(Enumerable.Cast<ClothesPreset.OutfitCategory>(Enum.GetValues(typeof(ClothesPreset.OutfitCategory))));
			list2 = AssetLoader.Instance.GetAllClothes();
			dictionary = new Dictionary<string, ClothesPreset>();
			foreach (ClothesPreset clothesPreset in list2)
			{
				dictionary.Add(clothesPreset.name, clothesPreset);
			}
			flag = true;
			if (this.debugOverride)
			{
				occupationPreset = this.debugOverrideJob;
				gender = this.debugOverrideGender;
				hairType = this.debugOverrideHair;
				build = this.debugOverrideBuild;
				skinColour = this.debugOverrideSkinColour;
				eyeColour = this.debugOverrideEyeColour;
				hairColour = this.debugOverrideHairColour;
				Human.ShoeType shoeType = this.debugOverrideShoeType;
				Color.Lerp(new Color(0.43137255f, 0.3137255f, 0.3137255f, 1f), new Color(0.8627451f, 0f, 0f, 1f), this.debugOverrideLipstick);
				this.human.humility = Random.Range(0f, 1f);
				this.human.extraversion = Random.Range(0f, 1f);
				this.human.emotionality = Random.Range(0f, 1f);
				this.human.agreeableness = Random.Range(0f, 1f);
				this.human.conscientiousness = Random.Range(0f, 1f);
				this.human.creativity = Random.Range(0f, 1f);
				if (gender == Human.Gender.male)
				{
					this.human.genderScale = Random.Range(0.5f, 1f);
				}
				else
				{
					this.human.genderScale = Random.Range(0f, 0.5f);
				}
			}
		}
		else
		{
			list = Toolbox.Instance.allOutfitCategories;
			list2 = Toolbox.Instance.allClothes;
			dictionary = Toolbox.Instance.clothesDictionary;
			if (this.human.job != null)
			{
				occupationPreset = this.human.job.preset;
			}
			gender = this.human.gender;
			build = this.human.descriptors.build;
			hairType = this.human.descriptors.hairType;
			eyeColour = this.human.descriptors.eyeColour;
			skinColour = this.human.descriptors.skinColour;
			hairColour = this.human.descriptors.hairColour;
			Human.ShoeType footwear2 = this.human.descriptors.footwear;
			if (gender == Human.Gender.nonBinary)
			{
				if (this.human.genderScale < 0.5f)
				{
					gender = Human.Gender.female;
				}
				else
				{
					gender = Human.Gender.male;
				}
			}
		}
		foreach (MeshRenderer meshRenderer in this.eyeRenderers)
		{
			if (eyeColour == Descriptors.EyeColour.blueEyes)
			{
				meshRenderer.sharedMaterial = this.bluePupil;
			}
			else if (eyeColour == Descriptors.EyeColour.greenEyes)
			{
				meshRenderer.sharedMaterial = this.greenPupil;
			}
			else if (eyeColour == Descriptors.EyeColour.brownEyes)
			{
				meshRenderer.sharedMaterial = this.brownPupil;
			}
			else if (eyeColour == Descriptors.EyeColour.greyEyes)
			{
				meshRenderer.sharedMaterial = this.greyPupil;
			}
		}
		if (this.eyebrowRenderers.Count > 0)
		{
			Color color = Color.Lerp(hairColour, skinColour, 0.4f);
			Material material = Object.Instantiate<Material>(this.eyebrowRenderers[0].sharedMaterial);
			material.SetColor("_BaseColor", color);
			foreach (MeshRenderer meshRenderer2 in this.eyebrowRenderers)
			{
				meshRenderer2.sharedMaterial = material;
			}
		}
		Dictionary<ClothesPreset.OutfitCategory, List<CitizenOutfitController.CharacterAnchor>> dictionary2 = new Dictionary<ClothesPreset.OutfitCategory, List<CitizenOutfitController.CharacterAnchor>>();
		foreach (ClothesPreset.OutfitCategory outfitCategory in list)
		{
			if (!forceSpecificDebugOutfit || outfitCategory == ClothesPreset.OutfitCategory.undressed)
			{
				if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
				{
					this.outfitDebug.Add("> Category " + outfitCategory.ToString());
				}
				List<CitizenOutfitController.CharacterAnchor> list3 = Enumerable.ToList<CitizenOutfitController.CharacterAnchor>(Enumerable.Cast<CitizenOutfitController.CharacterAnchor>(Enum.GetValues(typeof(CitizenOutfitController.CharacterAnchor))));
				List<ClothesPreset> list4 = new List<ClothesPreset>();
				List<ClothesPreset> list5 = list2;
				bool flag2 = true;
				if (outfitCategory == ClothesPreset.OutfitCategory.work && occupationPreset != null && occupationPreset.workOutfit.Count > 0)
				{
					list5 = occupationPreset.workOutfit;
					flag2 = false;
				}
				foreach (ClothesPreset clothesPreset2 in list5)
				{
					if (clothesPreset2 == null)
					{
						Debug.Log("Null clothes preset, probably from job " + occupationPreset.name);
					}
					if ((!flag2 || clothesPreset2.includeInPersonalityMatching) && clothesPreset2.suitableForGenders.Contains(gender) && clothesPreset2.outfitCategories.Contains(outfitCategory) && clothesPreset2.suitableForBuilds.Contains(build) && (!clothesPreset2.useWealthValues || (this.human.societalClass >= clothesPreset2.minimumWealth && this.human.societalClass <= clothesPreset2.maximumWealth)) && (!clothesPreset2.enableFacialFeatureSetup || clothesPreset2.suitableForHairstyle.Contains(hairType)))
					{
						list4.Add(clothesPreset2);
					}
				}
				CitizenOutfitController.Outfit outfit = new CitizenOutfitController.Outfit();
				outfit.category = outfitCategory;
				List<CitizenOutfitController.CharacterAnchor> list6 = new List<CitizenOutfitController.CharacterAnchor>();
				for (int i = 0; i < list3.Count; i++)
				{
					CitizenOutfitController.CharacterAnchor req = list3[i];
					if (!list6.Contains(req))
					{
						if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
						{
							this.outfitDebug.Add("Searching for " + req.ToString() + "...");
						}
						List<ClothesPreset> list7 = list4.FindAll((ClothesPreset item) => item.covers.Contains(req));
						List<ClothesPreset> list8 = new List<ClothesPreset>();
						foreach (ClothesPreset clothesPreset3 in list7)
						{
							bool flag3 = false;
							foreach (CitizenOutfitController.CharacterAnchor characterAnchor in clothesPreset3.covers)
							{
								if (!list3.Contains(characterAnchor) || list6.Contains(characterAnchor))
								{
									flag3 = false;
									break;
								}
								flag3 = true;
							}
							if (flag3)
							{
								if (clothesPreset3.incompatibility.Count > 0)
								{
									bool flag4 = true;
									using (List<ClothesPreset.IncompatibilitySetting>.Enumerator enumerator5 = clothesPreset3.incompatibility.GetEnumerator())
									{
										while (enumerator5.MoveNext())
										{
											ClothesPreset.IncompatibilitySetting inc = enumerator5.Current;
											if (inc.incompatibleIf == ClothesPreset.Incompatibility.inThisCategory)
											{
												if (inc.tags.Count > 0)
												{
													using (List<ClothesPreset.ClothesTags>.Enumerator enumerator6 = inc.tags.GetEnumerator())
													{
														while (enumerator6.MoveNext())
														{
															ClothesPreset.ClothesTags t = enumerator6.Current;
															if (outfit.clothes.Exists((CitizenOutfitController.OutfitClothes item) => item.tags.Contains(t)))
															{
																flag4 = false;
																break;
															}
														}
													}
													if (!flag4)
													{
														break;
													}
												}
												else if (inc.featured != null && outfit.clothes.Exists((CitizenOutfitController.OutfitClothes item) => item.clothes == inc.featured.name))
												{
													flag4 = false;
													break;
												}
											}
											else
											{
												Predicate<CitizenOutfitController.OutfitClothes> <>9__4;
												foreach (CitizenOutfitController.Outfit outfit2 in this.outfits)
												{
													if (inc.tags.Count > 0)
													{
														using (List<ClothesPreset.ClothesTags>.Enumerator enumerator6 = inc.tags.GetEnumerator())
														{
															while (enumerator6.MoveNext())
															{
																ClothesPreset.ClothesTags t = enumerator6.Current;
																if (outfit.clothes.Exists((CitizenOutfitController.OutfitClothes item) => item.tags.Contains(t)))
																{
																	flag4 = false;
																	break;
																}
															}
														}
														if (!flag4)
														{
															break;
														}
													}
													else if (inc.featured != null)
													{
														List<CitizenOutfitController.OutfitClothes> clothes = outfit2.clothes;
														Predicate<CitizenOutfitController.OutfitClothes> predicate;
														if ((predicate = <>9__4) == null)
														{
															predicate = (<>9__4 = ((CitizenOutfitController.OutfitClothes item) => item.clothes == inc.featured.name));
														}
														if (clothes.Exists(predicate))
														{
															flag4 = false;
															break;
														}
													}
												}
											}
											if (!flag4)
											{
												break;
											}
										}
									}
									if (!flag4)
									{
										continue;
									}
								}
								int num = 0;
								int num2 = 0;
								if (clothesPreset3.useTraits && clothesPreset3.characterTraits.Count > 0)
								{
									this.GetChance(this.human, ref clothesPreset3.characterTraits, out num);
								}
								if (clothesPreset3.useHEXACO)
								{
									num2 = this.human.GetHexacoScore(ref clothesPreset3.hexaco);
								}
								int num3 = clothesPreset3.baseChance + num + num2;
								for (int j = 0; j < num3; j++)
								{
									list8.Add(clothesPreset3);
								}
							}
						}
						if (list8.Count > 0)
						{
							int num4;
							if (!flag)
							{
								num4 = Toolbox.Instance.GetPsuedoRandomNumberContained(0, list8.Count, this.human.seed, out this.human.seed);
							}
							else
							{
								num4 = Random.Range(0, list8.Count);
							}
							ClothesPreset clothesPreset4 = list8[num4];
							CitizenOutfitController.OutfitClothes outfitClothes = new CitizenOutfitController.OutfitClothes();
							outfitClothes.clothes = clothesPreset4.name;
							outfitClothes.tags = new List<ClothesPreset.ClothesTags>(clothesPreset4.tags);
							outfitClothes.rank = clothesPreset4.priority;
							outfitClothes.borrowed = false;
							if (clothesPreset4.baseColourSource != ClothesPreset.ClothingColourSource.none)
							{
								if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.underneathColour1 || clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.underneathColour2 || clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.underneathColour3)
								{
									outfitClothes.baseColor = this.GetColourFromUnderneath(clothesPreset4, outfitCategory, clothesPreset4.baseColourSource, ref dictionary);
								}
								else if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.white)
								{
									outfitClothes.baseColor = Color.white;
								}
								else if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.garment)
								{
									if (clothesPreset4.colourBase.Count <= 0)
									{
										Game.LogError("No palettes for garment " + clothesPreset4.name + " colourBase", 2);
									}
									outfitClothes.baseColor = this.PickColourFromPalette(ref clothesPreset4.colourBase, clothesPreset4.name);
								}
								else if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.skin)
								{
									outfitClothes.baseColor = skinColour;
								}
								else if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.hair)
								{
									outfitClothes.baseColor = hairColour;
								}
								else if (clothesPreset4.baseColourSource == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
								{
									outfitClothes.baseColor = this.human.job.employer.uniformColour;
								}
							}
							if (clothesPreset4.colour1Source != ClothesPreset.ClothingColourSource.none)
							{
								if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.underneathColour1 || clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.underneathColour2 || clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.underneathColour3)
								{
									outfitClothes.color1 = this.GetColourFromUnderneath(clothesPreset4, outfitCategory, clothesPreset4.colour1Source, ref dictionary);
								}
								else if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.white)
								{
									outfitClothes.color1 = Color.white;
								}
								else if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.garment)
								{
									if (clothesPreset4.colour1.Count <= 0)
									{
										Game.LogError("No palettes for garment " + clothesPreset4.name + " colour1", 2);
									}
									outfitClothes.color1 = this.PickColourFromPalette(ref clothesPreset4.colour1, clothesPreset4.name);
								}
								else if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.skin)
								{
									outfitClothes.color1 = skinColour;
								}
								else if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.hair)
								{
									outfitClothes.color1 = hairColour;
								}
								else if (clothesPreset4.colour1Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
								{
									outfitClothes.color1 = this.human.job.employer.uniformColour;
								}
							}
							if (clothesPreset4.colour2Source != ClothesPreset.ClothingColourSource.none)
							{
								if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.underneathColour1 || clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.underneathColour2 || clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.underneathColour3)
								{
									outfitClothes.color2 = this.GetColourFromUnderneath(clothesPreset4, outfitCategory, clothesPreset4.colour2Source, ref dictionary);
								}
								else if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.white)
								{
									outfitClothes.color2 = Color.white;
								}
								else if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.garment)
								{
									if (clothesPreset4.colour2.Count <= 0)
									{
										Game.LogError("No palettes for garment " + clothesPreset4.name + " colour2", 2);
									}
									outfitClothes.color2 = this.PickColourFromPalette(ref clothesPreset4.colour2, clothesPreset4.name);
								}
								else if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.skin)
								{
									outfitClothes.color2 = skinColour;
								}
								else if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.hair)
								{
									outfitClothes.color2 = hairColour;
								}
								else if (clothesPreset4.colour2Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
								{
									outfitClothes.color2 = this.human.job.employer.uniformColour;
								}
							}
							if (clothesPreset4.colour3Source != ClothesPreset.ClothingColourSource.none)
							{
								if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.underneathColour1 || clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.underneathColour2 || clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.underneathColour3)
								{
									outfitClothes.color3 = this.GetColourFromUnderneath(clothesPreset4, outfitCategory, clothesPreset4.colour3Source, ref dictionary);
								}
								if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.white)
								{
									outfitClothes.color3 = Color.white;
								}
								else if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.garment)
								{
									if (clothesPreset4.colour3.Count <= 0)
									{
										Game.LogError("No palettes for garment " + clothesPreset4.name + " colour3", 2);
									}
									outfitClothes.color3 = this.PickColourFromPalette(ref clothesPreset4.colour3, clothesPreset4.name);
								}
								else if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.skin)
								{
									outfitClothes.color3 = skinColour;
								}
								else if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.hair)
								{
									outfitClothes.color3 = hairColour;
								}
								else if (clothesPreset4.colour3Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
								{
									outfitClothes.color3 = this.human.job.employer.uniformColour;
								}
							}
							outfit.clothes.Add(outfitClothes);
							if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
							{
								this.outfitDebug.Add("... Adding " + clothesPreset4.name);
							}
							list6.AddRange(clothesPreset4.covers);
						}
					}
				}
				foreach (CitizenOutfitController.CharacterAnchor characterAnchor2 in list6)
				{
					list3.Remove(characterAnchor2);
				}
				outfit.clothes.Sort((CitizenOutfitController.OutfitClothes p2, CitizenOutfitController.OutfitClothes p1) => p1.rank.CompareTo(p2.rank));
				this.outfits.Add(outfit);
				foreach (CitizenOutfitController.CharacterAnchor characterAnchor3 in list3)
				{
					if (!dictionary2.ContainsKey(outfitCategory))
					{
						dictionary2.Add(outfitCategory, new List<CitizenOutfitController.CharacterAnchor>());
					}
					dictionary2[outfitCategory].Add(characterAnchor3);
				}
			}
		}
		if (forceSpecificDebugOutfit)
		{
			foreach (ClothesPreset.OutfitCategory outfitCategory2 in list)
			{
				if (outfitCategory2 != ClothesPreset.OutfitCategory.undressed)
				{
					List<CitizenOutfitController.CharacterAnchor> list9 = Enumerable.ToList<CitizenOutfitController.CharacterAnchor>(Enumerable.Cast<CitizenOutfitController.CharacterAnchor>(Enum.GetValues(typeof(CitizenOutfitController.CharacterAnchor))));
					if (outfitCategory2 != ClothesPreset.OutfitCategory.outdoorsCasual && outfitCategory2 != ClothesPreset.OutfitCategory.outdoorsSmart && outfitCategory2 != ClothesPreset.OutfitCategory.outdoorsWork)
					{
						list9.Remove(CitizenOutfitController.CharacterAnchor.Hat);
					}
					CitizenOutfitController.Outfit outfit3 = new CitizenOutfitController.Outfit();
					outfit3.category = outfitCategory2;
					List<CitizenOutfitController.CharacterAnchor> list10 = new List<CitizenOutfitController.CharacterAnchor>();
					foreach (ClothesPreset clothesPreset5 in this.outfitToLoad)
					{
						CitizenOutfitController.OutfitClothes outfitClothes2 = new CitizenOutfitController.OutfitClothes();
						outfitClothes2.clothes = clothesPreset5.name;
						outfitClothes2.tags = new List<ClothesPreset.ClothesTags>(clothesPreset5.tags);
						outfitClothes2.rank = clothesPreset5.priority;
						outfitClothes2.borrowed = false;
						if (clothesPreset5.baseColourSource != ClothesPreset.ClothingColourSource.none)
						{
							if (clothesPreset5.baseColourSource == ClothesPreset.ClothingColourSource.white)
							{
								outfitClothes2.baseColor = Color.white;
							}
							else if (clothesPreset5.baseColourSource == ClothesPreset.ClothingColourSource.garment)
							{
								if (clothesPreset5.colourBase.Count <= 0)
								{
									Game.LogError("No palettes for garment " + clothesPreset5.name + " colourBase", 2);
								}
								outfitClothes2.baseColor = this.PickColourFromPalette(ref clothesPreset5.colourBase, clothesPreset5.name);
							}
							else if (clothesPreset5.baseColourSource == ClothesPreset.ClothingColourSource.skin)
							{
								outfitClothes2.baseColor = skinColour;
							}
							else if (clothesPreset5.baseColourSource == ClothesPreset.ClothingColourSource.hair)
							{
								outfitClothes2.baseColor = hairColour;
							}
							else if (clothesPreset5.baseColourSource == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
							{
								outfitClothes2.baseColor = this.human.job.employer.uniformColour;
							}
						}
						if (clothesPreset5.colour1Source != ClothesPreset.ClothingColourSource.none)
						{
							if (clothesPreset5.colour1Source == ClothesPreset.ClothingColourSource.white)
							{
								outfitClothes2.color1 = Color.white;
							}
							else if (clothesPreset5.colour1Source == ClothesPreset.ClothingColourSource.garment)
							{
								if (clothesPreset5.colour1.Count <= 0)
								{
									Game.LogError("No palettes for garment " + clothesPreset5.name + " colour1", 2);
								}
								outfitClothes2.color1 = this.PickColourFromPalette(ref clothesPreset5.colour1, clothesPreset5.name);
							}
							else if (clothesPreset5.colour1Source == ClothesPreset.ClothingColourSource.skin)
							{
								outfitClothes2.color1 = skinColour;
							}
							else if (clothesPreset5.colour1Source == ClothesPreset.ClothingColourSource.hair)
							{
								outfitClothes2.color1 = hairColour;
							}
							else if (clothesPreset5.colour1Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
							{
								outfitClothes2.color1 = this.human.job.employer.uniformColour;
							}
						}
						if (clothesPreset5.colour2Source != ClothesPreset.ClothingColourSource.none)
						{
							if (clothesPreset5.colour2Source == ClothesPreset.ClothingColourSource.white)
							{
								outfitClothes2.color2 = Color.white;
							}
							else if (clothesPreset5.colour2Source == ClothesPreset.ClothingColourSource.garment)
							{
								if (clothesPreset5.colour2.Count <= 0)
								{
									Game.LogError("No palettes for garment " + clothesPreset5.name + " colour2", 2);
								}
								outfitClothes2.color2 = this.PickColourFromPalette(ref clothesPreset5.colour2, clothesPreset5.name);
							}
							else if (clothesPreset5.colour2Source == ClothesPreset.ClothingColourSource.skin)
							{
								outfitClothes2.color2 = skinColour;
							}
							else if (clothesPreset5.colour2Source == ClothesPreset.ClothingColourSource.hair)
							{
								outfitClothes2.color2 = hairColour;
							}
							else if (clothesPreset5.colour2Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
							{
								outfitClothes2.color2 = this.human.job.employer.uniformColour;
							}
						}
						if (clothesPreset5.colour3Source != ClothesPreset.ClothingColourSource.none)
						{
							if (clothesPreset5.colour3Source == ClothesPreset.ClothingColourSource.white)
							{
								outfitClothes2.color3 = Color.white;
							}
							else if (clothesPreset5.colour3Source == ClothesPreset.ClothingColourSource.garment)
							{
								if (clothesPreset5.colour3.Count <= 0)
								{
									Game.LogError("No palettes for garment " + clothesPreset5.name + " colour3", 2);
								}
								outfitClothes2.color3 = this.PickColourFromPalette(ref clothesPreset5.colour3, clothesPreset5.name);
							}
							else if (clothesPreset5.colour3Source == ClothesPreset.ClothingColourSource.skin)
							{
								outfitClothes2.color3 = skinColour;
							}
							else if (clothesPreset5.colour3Source == ClothesPreset.ClothingColourSource.hair)
							{
								outfitClothes2.color3 = hairColour;
							}
							else if (clothesPreset5.colour3Source == ClothesPreset.ClothingColourSource.workUniformColour && this.human.job != null && this.human.job.employer != null)
							{
								outfitClothes2.color3 = this.human.job.employer.uniformColour;
							}
						}
						outfit3.clothes.Add(outfitClothes2);
						list10.AddRange(clothesPreset5.covers);
					}
					foreach (CitizenOutfitController.CharacterAnchor characterAnchor4 in list10)
					{
						list9.Remove(characterAnchor4);
					}
					this.outfits.Add(outfit3);
					foreach (CitizenOutfitController.CharacterAnchor characterAnchor5 in list9)
					{
						if (!dictionary2.ContainsKey(outfitCategory2))
						{
							dictionary2.Add(outfitCategory2, new List<CitizenOutfitController.CharacterAnchor>());
						}
						dictionary2[outfitCategory2].Add(characterAnchor5);
					}
				}
			}
		}
		using (Dictionary<ClothesPreset.OutfitCategory, List<CitizenOutfitController.CharacterAnchor>>.Enumerator enumerator8 = dictionary2.GetEnumerator())
		{
			while (enumerator8.MoveNext())
			{
				KeyValuePair<ClothesPreset.OutfitCategory, List<CitizenOutfitController.CharacterAnchor>> pair = enumerator8.Current;
				if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
				{
					this.outfitDebug.Add(string.Concat(new string[]
					{
						" -> Outfit ",
						pair.Key.ToString(),
						" is missing ",
						pair.Value.Count.ToString(),
						" components <-"
					}));
				}
				List<ClothesPreset.OutfitCategory> list11 = new List<ClothesPreset.OutfitCategory>();
				CitizenOutfitController.Outfit outfit4 = this.outfits.Find((CitizenOutfitController.Outfit item) => item.category == pair.Key);
				if (pair.Key == ClothesPreset.OutfitCategory.bed || pair.Key == ClothesPreset.OutfitCategory.underwear)
				{
					list11.Add(ClothesPreset.OutfitCategory.undressed);
				}
				else if (pair.Key == ClothesPreset.OutfitCategory.casual)
				{
					list11.Add(ClothesPreset.OutfitCategory.underwear);
					list11.Add(ClothesPreset.OutfitCategory.undressed);
				}
				else if (pair.Key == ClothesPreset.OutfitCategory.work || pair.Key == ClothesPreset.OutfitCategory.smart || pair.Key == ClothesPreset.OutfitCategory.outdoorsCasual)
				{
					list11.Add(ClothesPreset.OutfitCategory.casual);
					list11.Add(ClothesPreset.OutfitCategory.underwear);
					list11.Add(ClothesPreset.OutfitCategory.undressed);
				}
				else if (pair.Key == ClothesPreset.OutfitCategory.outdoorsSmart)
				{
					list11.Add(ClothesPreset.OutfitCategory.outdoorsCasual);
					list11.Add(ClothesPreset.OutfitCategory.smart);
					list11.Add(ClothesPreset.OutfitCategory.casual);
					list11.Add(ClothesPreset.OutfitCategory.underwear);
					list11.Add(ClothesPreset.OutfitCategory.undressed);
				}
				else if (pair.Key == ClothesPreset.OutfitCategory.outdoorsWork)
				{
					list11.Add(ClothesPreset.OutfitCategory.outdoorsCasual);
					list11.Add(ClothesPreset.OutfitCategory.work);
					list11.Add(ClothesPreset.OutfitCategory.casual);
					list11.Add(ClothesPreset.OutfitCategory.underwear);
					list11.Add(ClothesPreset.OutfitCategory.undressed);
				}
				for (int k = 0; k < list11.Count; k++)
				{
					ClothesPreset.OutfitCategory backupCat = list11[k];
					CitizenOutfitController.Outfit outfit5 = this.outfits.Find((CitizenOutfitController.Outfit item) => item.category == backupCat);
					if (outfit5 != null)
					{
						List<CitizenOutfitController.CharacterAnchor> list12 = new List<CitizenOutfitController.CharacterAnchor>();
						for (int l = 0; l < pair.Value.Count; l++)
						{
							CitizenOutfitController.CharacterAnchor characterAnchor6 = pair.Value[l];
							if (!list12.Contains(characterAnchor6))
							{
								if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
								{
									this.outfitDebug.Add(string.Concat(new string[]
									{
										"Searching for ",
										characterAnchor6.ToString(),
										" in ",
										backupCat.ToString(),
										"..."
									}));
								}
								bool flag5 = false;
								foreach (CitizenOutfitController.OutfitClothes outfitClothes3 in outfit5.clothes)
								{
									if (!outfitClothes3.borrowed)
									{
										ClothesPreset clothesPreset6 = dictionary[outfitClothes3.clothes];
										bool flag6 = false;
										if (clothesPreset6.covers.Contains(characterAnchor6))
										{
											foreach (CitizenOutfitController.CharacterAnchor characterAnchor7 in clothesPreset6.covers)
											{
												if (pair.Value.Contains(characterAnchor7) && !list12.Contains(characterAnchor7))
												{
													flag6 = true;
												}
												else if (clothesPreset6.onlyChooseIfAllModelPartsAreAvailable)
												{
													flag6 = false;
													break;
												}
											}
										}
										if (flag6)
										{
											if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
											{
												List<string> list13 = new List<string>();
												foreach (CitizenOutfitController.CharacterAnchor characterAnchor8 in clothesPreset6.covers)
												{
													list13.Add(" " + characterAnchor8.ToString());
												}
												List<string> list14 = this.outfitDebug;
												string[] array = new string[6];
												array[0] = "> ";
												array[1] = outfitClothes3.clothes;
												array[2] = " borrowed from outfit ";
												array[3] = backupCat.ToString();
												array[4] = " covering: ";
												int num5 = 5;
												List<string> list15 = list13;
												array[num5] = ((list15 != null) ? list15.ToString() : null);
												list14.Add(string.Concat(array));
											}
											CitizenOutfitController.OutfitClothes outfitClothes4 = new CitizenOutfitController.OutfitClothes();
											outfitClothes4.clothes = clothesPreset6.name;
											outfitClothes4.tags = new List<ClothesPreset.ClothesTags>(clothesPreset6.tags);
											outfitClothes4.rank = clothesPreset6.priority;
											outfitClothes4.baseColor = outfitClothes3.baseColor;
											outfitClothes4.color1 = outfitClothes3.color1;
											outfitClothes4.color2 = outfitClothes3.color2;
											outfitClothes4.color3 = outfitClothes3.color3;
											outfitClothes4.borrowed = true;
											outfit4.clothes.Add(outfitClothes4);
											outfit4.clothes.Sort((CitizenOutfitController.OutfitClothes p2, CitizenOutfitController.OutfitClothes p1) => p1.rank.CompareTo(p2.rank));
											list12.AddRange(clothesPreset6.covers);
											flag5 = true;
											break;
										}
									}
								}
								if (!flag5 && (Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
								{
									this.outfitDebug.Add("Unable to find " + characterAnchor6.ToString() + " in " + backupCat.ToString());
								}
							}
						}
						foreach (CitizenOutfitController.CharacterAnchor characterAnchor9 in list12)
						{
							pair.Value.Remove(characterAnchor9);
						}
					}
					if (pair.Value.Count <= 0)
					{
						break;
					}
				}
			}
		}
		if (flag)
		{
			this.human.genderScale = 0.5f;
			this.human.humility = 0f;
			this.human.extraversion = 0f;
			this.human.emotionality = 0f;
			this.human.agreeableness = 0f;
			this.human.conscientiousness = 0f;
			this.human.creativity = 0f;
		}
	}

	// Token: 0x06000E66 RID: 3686 RVA: 0x000D11D0 File Offset: 0x000CF3D0
	public Transform GetBodyAnchor(CitizenOutfitController.CharacterAnchor anchor)
	{
		Transform result = null;
		if (!this.anchorReference.TryGetValue(anchor, ref result))
		{
			try
			{
				Debug.LogError("Unable to get citizen body anchor " + anchor.ToString() + ": " + this.human.GetCitizenName());
			}
			catch
			{
			}
		}
		return result;
	}

	// Token: 0x06000E67 RID: 3687 RVA: 0x000D1234 File Offset: 0x000CF434
	public void MakeClothed()
	{
		bool flag = false;
		if (this.human.ai != null && this.human.ai.currentAction != null && this.human.ai.currentAction.preset.outdoorClothingCheck && this.human.ai.currentAction.node != null && (this.human.ai.currentAction.node.building != this.human.currentBuilding || this.human.ai.currentAction.node.building == null || this.human.ai.currentAction.node.isOutside || this.human.ai.currentAction.node.room.gameLocation.IsOutside() || this.human.ai.currentAction.node.room.IsOutside()))
		{
			flag = true;
		}
		if (this.currentOutfit != ClothesPreset.OutfitCategory.bed && this.currentOutfit != ClothesPreset.OutfitCategory.underwear && this.currentOutfit != ClothesPreset.OutfitCategory.undressed)
		{
			if (flag)
			{
				if (this.currentOutfit == ClothesPreset.OutfitCategory.work)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsWork, false, false, true);
					return;
				}
				if (this.currentOutfit == ClothesPreset.OutfitCategory.casual)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsCasual, false, false, true);
					return;
				}
				if (this.currentOutfit == ClothesPreset.OutfitCategory.smart)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsSmart, false, false, true);
					return;
				}
			}
			else
			{
				if (this.currentOutfit == ClothesPreset.OutfitCategory.outdoorsWork)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.work, false, false, true);
					return;
				}
				if (this.currentOutfit == ClothesPreset.OutfitCategory.outdoorsCasual)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.casual, false, false, true);
					return;
				}
				if (this.currentOutfit == ClothesPreset.OutfitCategory.outdoorsSmart)
				{
					this.SetCurrentOutfit(ClothesPreset.OutfitCategory.smart, false, false, true);
				}
			}
			return;
		}
		if (this.human.isAtWork)
		{
			if (flag)
			{
				this.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsWork, false, false, true);
				return;
			}
			this.SetCurrentOutfit(ClothesPreset.OutfitCategory.work, false, false, true);
			return;
		}
		else
		{
			if (flag)
			{
				this.SetCurrentOutfit(ClothesPreset.OutfitCategory.outdoorsCasual, false, false, true);
				return;
			}
			this.SetCurrentOutfit(ClothesPreset.OutfitCategory.casual, false, false, true);
			return;
		}
	}

	// Token: 0x06000E68 RID: 3688 RVA: 0x000D1430 File Offset: 0x000CF630
	public void SetCurrentOutfit(ClothesPreset.OutfitCategory category, bool forceLoad = false, bool forceReload = false, bool ignoreIfDead = true)
	{
		if (Toolbox.Instance == null)
		{
			forceReload = true;
		}
		if (this.human.ai != null && ignoreIfDead && (this.human.isDead || this.human.isStunned || this.human.ai.inCombat || this.human.ai.ko || this.human.ai.restrained))
		{
			return;
		}
		if (category == ClothesPreset.OutfitCategory.undressed)
		{
			category = ClothesPreset.OutfitCategory.underwear;
		}
		if (this.human != null && this.human.ai != null && !this.isPoser)
		{
			if (this.human.ai.isRagdoll)
			{
				return;
			}
			if (Game.Instance.collectDebugData)
			{
				this.human.SelectedDebug("AI: Set current outfit: " + category.ToString(), Actor.HumanDebug.misc);
			}
		}
		if (Toolbox.Instance != null)
		{
			if (category == ClothesPreset.OutfitCategory.work)
			{
				if (this.human.job == null || this.human.job.preset == null || this.human.job.preset.workOutfit.Count <= 0)
				{
					category = ClothesPreset.OutfitCategory.casual;
				}
			}
			else if (category == ClothesPreset.OutfitCategory.outdoorsWork && (this.human.job == null || this.human.job.preset == null || this.human.job.preset.workOutfit.Count <= 0))
			{
				category = ClothesPreset.OutfitCategory.outdoorsCasual;
			}
		}
		if (this.currentOutfit != category || forceReload)
		{
			this.currentOutfit = category;
			if (this.human.visible || forceLoad || this.isPoser)
			{
				this.LoadCurrentOutfit(forceLoad, forceReload);
			}
		}
	}

	// Token: 0x06000E69 RID: 3689 RVA: 0x000D1600 File Offset: 0x000CF800
	public void LoadCurrentOutfit(bool forceLoad = false, bool forceReload = false)
	{
		if (Toolbox.Instance == null)
		{
			forceReload = true;
		}
		if (this.loadedOutfit != this.currentOutfit || forceLoad)
		{
			if (forceReload)
			{
				this.RemoveCurrentOutfit();
			}
			if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
			{
				this.outfitDebug.Add("> Loading outfit: " + this.currentOutfit.ToString());
			}
			this.RemoveDebugRenderers();
			foreach (CitizenOutfitController.OutfitClothes outfitClothes in this.currentlyLoadedClothes)
			{
				outfitClothes.loadedThisCycle = false;
			}
			CitizenOutfitController.Outfit outfit = this.outfits.Find((CitizenOutfitController.Outfit item) => item.category == this.currentOutfit);
			if (outfit != null)
			{
				Dictionary<string, ClothesPreset> dictionary = null;
				if (Toolbox.Instance != null)
				{
					dictionary = Toolbox.Instance.clothesDictionary;
				}
				else
				{
					dictionary = new Dictionary<string, ClothesPreset>();
					foreach (ClothesPreset clothesPreset in AssetLoader.Instance.GetAllClothes())
					{
						dictionary.Add(clothesPreset.name, clothesPreset);
					}
				}
				List<CitizenOutfitController.OutfitClothes> list = new List<CitizenOutfitController.OutfitClothes>(outfit.clothes);
				this.coveredAnchors = new Dictionary<CitizenOutfitController.CharacterAnchor, int>();
				HashSet<CitizenOutfitController.OutfitClothes> hashSet = new HashSet<CitizenOutfitController.OutfitClothes>();
				for (int i = 0; i < list.Count; i++)
				{
					CitizenOutfitController.OutfitClothes req = list[i];
					if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
					{
						this.outfitDebug.Add("Loading clothing: " + req.clothes + "...");
					}
					CitizenOutfitController.OutfitClothes outfitClothes2 = this.currentlyLoadedClothes.Find((CitizenOutfitController.OutfitClothes item) => !item.incomplete && item.clothes == req.clothes && item.baseColor == req.baseColor && item.color1 == req.color1 && item.color2 == req.color2 && item.color3 == req.color3);
					if (outfitClothes2 != null)
					{
						ClothesPreset clothesPreset2 = null;
						if (!dictionary.TryGetValue(req.clothes, ref clothesPreset2))
						{
							Game.LogError("Unable to find " + req.clothes + " in clothes dictionary...", 2);
						}
						else
						{
							foreach (ClothesPreset.ModelSettings modelSettings in clothesPreset2.models)
							{
								if (modelSettings.exclusiveAnchorModel)
								{
									int num = -1;
									if (this.coveredAnchors.TryGetValue(modelSettings.anchor, ref num))
									{
										if (num > clothesPreset2.priority)
										{
											if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
											{
												this.outfitDebug.Add(string.Concat(new string[]
												{
													"Removing model ",
													modelSettings.prefab.name,
													" because anchor ",
													modelSettings.anchor.ToString(),
													" is already covered and is of higher priority (",
													num.ToString(),
													">",
													clothesPreset2.priority.ToString(),
													")"
												}));
											}
											this.RemoveSpecificModel(outfitClothes2, modelSettings.anchor);
											outfitClothes2.incomplete = true;
										}
									}
									else
									{
										this.coveredAnchors.Add(modelSettings.anchor, clothesPreset2.priority);
									}
								}
							}
							if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
							{
								this.outfitDebug.Add("Found existing item " + req.clothes);
							}
							hashSet.Add(outfitClothes2);
							outfitClothes2.loadedThisCycle = true;
						}
					}
					else
					{
						ClothesPreset cp = null;
						if (!dictionary.TryGetValue(req.clothes, ref cp))
						{
							Game.LogError("Unable to find " + req.clothes + " in clothes dictionary...", 2);
						}
						else
						{
							this.SpawnClothingElement(req, cp);
							hashSet.Add(req);
						}
					}
				}
				for (int j = 0; j < this.currentlyLoadedClothes.Count; j++)
				{
					CitizenOutfitController.OutfitClothes outfitClothes3 = this.currentlyLoadedClothes[j];
					if (!hashSet.Contains(outfitClothes3))
					{
						if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
						{
							this.outfitDebug.Add("Unrequired item " + outfitClothes3.clothes + " (removing)");
						}
						this.RemoveClothingComponent(outfitClothes3);
						j--;
					}
				}
			}
			else if (!this.isPoser)
			{
				Debug.Log("Cannot find " + this.currentOutfit.ToString() + " outfit for " + this.human.GetCitizenName());
			}
			this.loadedOutfit = this.currentOutfit;
			for (int k = 0; k < this.allCurrentMeshes.Count; k++)
			{
				MeshRenderer meshRenderer = this.allCurrentMeshes[k];
				if (meshRenderer == null)
				{
					this.allCurrentMeshes.RemoveAt(k);
					this.allCurrentMeshFilters.RemoveAt(k);
					k--;
				}
				else if (meshRenderer.sharedMaterial == null)
				{
					if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
					{
						this.outfitDebug.Add("Cleaning object " + meshRenderer.gameObject.name);
					}
					this.SafeDestroy<GameObject>(meshRenderer.gameObject);
					this.allCurrentMeshes.RemoveAt(k);
					this.allCurrentMeshFilters.RemoveAt(k);
					k--;
				}
			}
			if (this.human != null && !this.isPoser)
			{
				this.human.updateMeshList = true;
			}
			this.HairHatCompatibilityCheck();
		}
	}

	// Token: 0x06000E6A RID: 3690 RVA: 0x000D1C94 File Offset: 0x000CFE94
	private void SpawnClothingElement(CitizenOutfitController.OutfitClothes cl, ClothesPreset cp)
	{
		if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
		{
			this.outfitDebug.Add("Spawning required item " + cl.clothes);
		}
		cl.spawned = new Dictionary<CitizenOutfitController.CharacterAnchor, List<MeshRenderer>>();
		cl.loadedThisCycle = true;
		if (this.anchorReference.Count <= 0)
		{
			foreach (CitizenOutfitController.AnchorConfig anchorConfig in this.anchorConfig)
			{
				this.anchorReference.Add(anchorConfig.anchor, anchorConfig.trans);
			}
		}
		if (cp.setFootwear && Application.isPlaying && !this.isPoser)
		{
			this.human.SetFootwear(cp.footwear);
		}
		if (cp.isHead)
		{
			this.pupilParent.localPosition = cp.pupilsOffset;
			this.pupilParentOffset = cp.pupilsOffset;
			this.eyebrowParent.localPosition = cp.eyebrowsOffset;
			this.mouth.localPosition = cp.mouthOffset;
		}
		foreach (ClothesPreset.ModelSettings modelSettings in cp.models)
		{
			if (modelSettings == null)
			{
				Game.LogError("Missing model setting on " + cl.clothes, 2);
			}
			else if (modelSettings.prefab == null)
			{
				Game.LogError("Missing prefab on model " + cl.clothes, 2);
			}
			else if (!this.anchorReference.ContainsKey(modelSettings.anchor))
			{
				Game.LogError("Missing anchor reference " + modelSettings.anchor.ToString(), 2);
			}
			else
			{
				if (modelSettings.exclusiveAnchorModel)
				{
					foreach (CitizenOutfitController.OutfitClothes outfitClothes in this.currentlyLoadedClothes)
					{
						if (!outfitClothes.loadedThisCycle && outfitClothes.spawned.ContainsKey(modelSettings.anchor))
						{
							this.RemoveSpecificModel(outfitClothes, modelSettings.anchor);
							outfitClothes.incomplete = true;
						}
					}
					int num = -1;
					if (this.coveredAnchors.TryGetValue(modelSettings.anchor, ref num))
					{
						if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
						{
							this.outfitDebug.Add(string.Concat(new string[]
							{
								"Skipping model ",
								modelSettings.prefab.name,
								" because anchor ",
								modelSettings.anchor.ToString(),
								" is already covered..."
							}));
						}
						cl.incomplete = true;
						continue;
					}
					this.coveredAnchors.Add(modelSettings.anchor, cp.priority);
				}
				GameObject gameObject = Object.Instantiate<GameObject>(modelSettings.prefab, this.anchorReference[modelSettings.anchor]);
				gameObject.transform.localPosition = modelSettings.offsetPosition;
				gameObject.transform.localEulerAngles = modelSettings.offsetEuler;
				gameObject.transform.tag = "CitizenModelDebug";
				MeshRenderer component = gameObject.GetComponent<MeshRenderer>();
				float num2;
				if (Application.isPlaying)
				{
					num2 = Mathf.Clamp01(1f - this.human.conscientiousness) * 0.25f;
					if (this.human.isHomeless)
					{
						num2 += 0.75f;
					}
					num2 = Mathf.Clamp(0.1f, num2, 1f);
				}
				else
				{
					num2 = this.debugOverrideGrub;
				}
				Toolbox.MaterialKey materialKey = new Toolbox.MaterialKey();
				materialKey = new Toolbox.MaterialKey();
				materialKey.baseMaterial = component.sharedMaterial;
				materialKey.mainColour = cl.baseColor;
				materialKey.colour1 = cl.color1;
				materialKey.colour2 = cl.color2;
				materialKey.colour3 = cl.color3;
				materialKey.grubiness = num2;
				Material materialFromKey = MaterialsController.Instance.GetMaterialFromKey(materialKey);
				LODGroup component2 = gameObject.GetComponent<LODGroup>();
				if (component2 != null)
				{
					LOD[] lods = component2.GetLODs();
					if (lods.Length != 0)
					{
						foreach (MeshRenderer rend in lods[0].renderers)
						{
							this.AddMeshRenderer(rend, ref materialFromKey, false, ref cl, modelSettings);
						}
					}
					if (lods.Length > 1)
					{
						for (int j = 0; j < lods[1].renderers.Length; j++)
						{
							if (this.isPoser)
							{
								this.SafeDestroy<GameObject>(lods[1].renderers[j].gameObject);
							}
							else
							{
								this.AddMeshRenderer(lods[1].renderers[j] as MeshRenderer, ref materialFromKey, true, ref cl, modelSettings);
							}
						}
					}
					component2.SetLODs(new LOD[0]);
					this.SafeDestroy<LODGroup>(component2);
				}
				else
				{
					this.AddMeshRenderer(component, ref materialFromKey, false, ref cl, modelSettings);
					if (!this.isPoser)
					{
						this.human.AddMesh(component, true, false, true, false);
					}
				}
				if (!this.isPoser)
				{
					if (this.eyebrowRenderers != null)
					{
						this.human.AddMeshes(this.eyebrowRenderers, true, false);
					}
					if (this.eyeRenderers != null)
					{
						this.human.AddMeshes(this.eyeRenderers, true, false);
					}
					if (this.mouthRenderer != null)
					{
						this.human.AddMesh(this.mouthRenderer, true, false, false, false);
					}
				}
				if (!cl.spawned.ContainsKey(modelSettings.anchor))
				{
					cl.spawned.Add(modelSettings.anchor, new List<MeshRenderer>());
				}
				cl.spawned[modelSettings.anchor].Add(component);
				if (modelSettings.anchor == CitizenOutfitController.CharacterAnchor.Hair)
				{
					this.currentHair = cp;
					this.currentHairRend = component;
				}
				else if (modelSettings.anchor == CitizenOutfitController.CharacterAnchor.Hat)
				{
					this.currentHat = cp;
					this.currentHatRend = component;
				}
			}
		}
		this.currentlyLoadedClothes.Add(cl);
	}

	// Token: 0x06000E6B RID: 3691 RVA: 0x000D22F0 File Offset: 0x000D04F0
	private void AddMeshRenderer(MeshRenderer rend, ref Material applyMat, bool isLOD, ref CitizenOutfitController.OutfitClothes clothesOutfit, ClothesPreset.ModelSettings model)
	{
		this.allCurrentMeshes.Add(rend);
		MeshFilter component = rend.gameObject.GetComponent<MeshFilter>();
		this.allCurrentMeshFilters.Add(component);
		if (Toolbox.Instance != null)
		{
			if (this.isPoser && this.poser.node != null)
			{
				Toolbox.Instance.SetLightLayer(rend, this.poser.node.building, false);
			}
			else
			{
				Toolbox.Instance.SetLightLayer(rend, this.human.currentBuilding, false);
			}
		}
		if (rend.gameObject.transform.tag != "NoMatColour")
		{
			rend.sharedMaterial = applyMat;
		}
		rend.gameObject.transform.tag = "CitizenModelDebug";
		if (!this.isPoser)
		{
			rend.gameObject.layer = 24;
		}
		if (!this.isPoser)
		{
			this.human.AddMesh(rend, true, false, isLOD, false);
		}
		if (isLOD && !Game.Instance.shadowsOnCitizenLOD)
		{
			rend.shadowCastingMode = 0;
		}
		if (!clothesOutfit.spawned.ContainsKey(model.anchor))
		{
			clothesOutfit.spawned.Add(model.anchor, new List<MeshRenderer>());
		}
		clothesOutfit.spawned[model.anchor].Add(rend);
	}

	// Token: 0x06000E6C RID: 3692 RVA: 0x000D2440 File Offset: 0x000D0640
	private void RemoveSpecificModel(CitizenOutfitController.OutfitClothes cl, CitizenOutfitController.CharacterAnchor a)
	{
		List<MeshRenderer> list = null;
		if (cl.spawned.TryGetValue(a, ref list))
		{
			while (list.Count > 0)
			{
				try
				{
					if (list[0] != null)
					{
						if (this.human != null && !this.isPoser)
						{
							this.human.RemoveMesh(list[0], true, false);
						}
						this.SafeDestroy<GameObject>(list[0].gameObject);
					}
				}
				catch
				{
					string text = "Unable to remove ";
					MeshRenderer meshRenderer = list[0];
					Debug.LogError(text + ((meshRenderer != null) ? meshRenderer.ToString() : null));
				}
				list.RemoveAt(0);
			}
		}
	}

	// Token: 0x06000E6D RID: 3693 RVA: 0x000D24FC File Offset: 0x000D06FC
	public void HairHatCompatibilityCheck()
	{
		if (this.currentHatRend != null)
		{
			if (this.human.ai.isRagdoll)
			{
				if (this.currentHairRend != null)
				{
					this.currentHairRend.enabled = true;
					return;
				}
			}
			else if (this.currentHat.hairRenderMode == ClothesPreset.HairRenderSetting.renderAllHair)
			{
				if (this.currentHairRend != null)
				{
					this.currentHairRend.enabled = true;
					return;
				}
			}
			else if (this.currentHat.hairRenderMode == ClothesPreset.HairRenderSetting.renderHatCompatibleHair && this.currentHair != null && this.currentHair.hatRenderCompatible && !this.currentHair.excludeHats.Contains(this.currentHat))
			{
				if (this.currentHairRend != null)
				{
					this.currentHairRend.enabled = true;
					return;
				}
			}
			else if (this.currentHairRend != null)
			{
				this.currentHairRend.enabled = false;
			}
		}
	}

	// Token: 0x06000E6E RID: 3694 RVA: 0x000D25EC File Offset: 0x000D07EC
	private void RemoveClothingComponent(CitizenOutfitController.OutfitClothes cl)
	{
		foreach (KeyValuePair<CitizenOutfitController.CharacterAnchor, List<MeshRenderer>> keyValuePair in cl.spawned)
		{
			while (keyValuePair.Value.Count > 0)
			{
				if (keyValuePair.Value[0] != null)
				{
					if ((Game.Instance == null || (Game.Instance.devMode && Game.Instance.collectDebugData)) && this.enableDebugLog)
					{
						List<string> list = this.outfitDebug;
						string text = "Removing model ";
						GameObject gameObject = keyValuePair.Value[0].gameObject;
						list.Add(text + ((gameObject != null) ? gameObject.ToString() : null));
					}
					try
					{
						if (this.human != null && !this.isPoser)
						{
							this.human.RemoveMesh(keyValuePair.Value[0], true, false);
						}
						this.SafeDestroy<GameObject>(keyValuePair.Value[0].gameObject);
					}
					catch
					{
						string text2 = "Unable to remove ";
						MeshRenderer meshRenderer = keyValuePair.Value[0];
						Debug.LogError(text2 + ((meshRenderer != null) ? meshRenderer.ToString() : null));
					}
				}
				keyValuePair.Value.RemoveAt(0);
			}
		}
		cl.spawned.Clear();
		this.currentlyLoadedClothes.Remove(cl);
	}

	// Token: 0x06000E6F RID: 3695 RVA: 0x000D2790 File Offset: 0x000D0990
	private void RemoveDebugRenderers()
	{
		while (this.debugRenderers.Count > 0)
		{
			try
			{
				this.SafeDestroy<MeshRenderer>(this.debugRenderers[0]);
			}
			catch
			{
				string text = "Unable to remove ";
				MeshRenderer meshRenderer = this.debugRenderers[0];
				Debug.LogError(text + ((meshRenderer != null) ? meshRenderer.ToString() : null));
			}
			this.debugRenderers.RemoveAt(0);
		}
	}

	// Token: 0x06000E70 RID: 3696 RVA: 0x000D280C File Offset: 0x000D0A0C
	[Button(null, 0)]
	public void RemoveCurrentOutfit()
	{
		this.RemoveDebugRenderers();
		while (this.currentlyLoadedClothes.Count > 0)
		{
			CitizenOutfitController.OutfitClothes outfitClothes = this.currentlyLoadedClothes[0];
			foreach (KeyValuePair<CitizenOutfitController.CharacterAnchor, List<MeshRenderer>> keyValuePair in outfitClothes.spawned)
			{
				while (keyValuePair.Value.Count > 0)
				{
					if (keyValuePair.Value[0] != null)
					{
						try
						{
							this.SafeDestroy<GameObject>(keyValuePair.Value[0].gameObject);
						}
						catch
						{
							string text = "Unable to remove ";
							MeshRenderer meshRenderer = keyValuePair.Value[0];
							Debug.LogError(text + ((meshRenderer != null) ? meshRenderer.ToString() : null));
						}
					}
					keyValuePair.Value.RemoveAt(0);
				}
			}
			outfitClothes.spawned.Clear();
			this.currentlyLoadedClothes.RemoveAt(0);
		}
		this.currentlyLoadedClothes.Clear();
		List<MeshRenderer> list = Enumerable.ToList<MeshRenderer>(base.transform.GetComponentsInChildren<MeshRenderer>(true)).FindAll((MeshRenderer item) => item.transform.CompareTag("CitizenModelDebug"));
		while (list.Count > 0)
		{
			try
			{
				this.SafeDestroy<GameObject>(list[0].gameObject);
			}
			catch
			{
				string text2 = "Unable to remove ";
				MeshRenderer meshRenderer2 = list[0];
				Debug.LogError(text2 + ((meshRenderer2 != null) ? meshRenderer2.ToString() : null));
				this.SafeDestroy<MeshRenderer>(list[0]);
			}
			list.RemoveAt(0);
		}
		if (this.human != null && !this.isPoser)
		{
			this.human.UpdateMeshList();
		}
	}

	// Token: 0x06000E71 RID: 3697 RVA: 0x000D29EC File Offset: 0x000D0BEC
	[Button(null, 0)]
	public void LoadSpecificOutfit()
	{
		this.RemoveCurrentOutfit();
		this.GenerateOutfits(true);
		this.CycleOutfits();
	}

	// Token: 0x06000E72 RID: 3698 RVA: 0x000D2A01 File Offset: 0x000D0C01
	[Button(null, 0)]
	public void SelectRandomOutfits()
	{
		this.RemoveCurrentOutfit();
		this.GenerateOutfits(false);
		this.CycleOutfits();
	}

	// Token: 0x06000E73 RID: 3699 RVA: 0x000D2A18 File Offset: 0x000D0C18
	[Button(null, 0)]
	public void CycleOutfits()
	{
		if (this.outfits.Count <= 0)
		{
			Debug.LogError("No outfits assigned to citizen! If in editor, make sure you SelectRandomOutfits first...");
			return;
		}
		int num = (int)this.currentOutfit;
		num++;
		List<ClothesPreset.OutfitCategory> list = Enumerable.ToList<ClothesPreset.OutfitCategory>(Enumerable.Cast<ClothesPreset.OutfitCategory>(Enum.GetValues(typeof(ClothesPreset.OutfitCategory))));
		if (num >= list.Count)
		{
			num = 0;
		}
		this.SetCurrentOutfit((ClothesPreset.OutfitCategory)num, true, false, true);
		Debug.Log("Loaded " + this.currentOutfit.ToString() + " outfit");
	}

	// Token: 0x06000E74 RID: 3700 RVA: 0x000D2A9D File Offset: 0x000D0C9D
	[Button(null, 0)]
	public void ResetAllOutfits()
	{
		this.RemoveCurrentOutfit();
		this.outfits.Clear();
		this.outfitDebug.Clear();
		this.currentOutfit = ClothesPreset.OutfitCategory.underwear;
		this.previousOutfit = ClothesPreset.OutfitCategory.underwear;
		this.loadedOutfit = ClothesPreset.OutfitCategory.underwear;
	}

	// Token: 0x06000E75 RID: 3701 RVA: 0x00002265 File Offset: 0x00000465
	[Button(null, 0)]
	public void CreateNewClothingPreset()
	{
	}

	// Token: 0x06000E76 RID: 3702 RVA: 0x000D2AD0 File Offset: 0x000D0CD0
	[Button(null, 0)]
	public void LoadExpression()
	{
		CitizenOutfitController.ExpressionSetup expressionSetup = this.expressions.Find((CitizenOutfitController.ExpressionSetup item) => item.expression == this.debugOverrideExpression);
		if (expressionSetup != null)
		{
			this.rightEyebrow.localEulerAngles = expressionSetup.eyebrowsEuler;
			this.leftEyebrow.localEulerAngles = -expressionSetup.eyebrowsEuler;
			this.rightEyebrow.localPosition = new Vector3(0.045f, expressionSetup.eyebrowsRaise, 0f);
			this.leftEyebrow.localPosition = new Vector3(-0.045f, expressionSetup.eyebrowsRaise, 0f);
			this.rightPupil.localScale = new Vector3(0.02f, 0.02f * expressionSetup.eyeHeightMultiplier, 0.02f);
			this.leftPupil.localScale = new Vector3(0.02f, 0.02f * expressionSetup.eyeHeightMultiplier, 0.02f);
		}
	}

	// Token: 0x06000E77 RID: 3703 RVA: 0x000D2BB0 File Offset: 0x000D0DB0
	public T SafeDestroyGameObject<T>(T component) where T : Component
	{
		if (component != null)
		{
			this.SafeDestroy<GameObject>(component.gameObject);
		}
		return default(T);
	}

	// Token: 0x06000E78 RID: 3704 RVA: 0x000D2BE8 File Offset: 0x000D0DE8
	public T SafeDestroy<T>(T obj) where T : Object
	{
		if ((Application.isEditor && !Application.isPlaying) || this.isPoser)
		{
			Object.DestroyImmediate(obj);
		}
		else
		{
			Object.Destroy(obj);
		}
		return default(T);
	}

	// Token: 0x06000E79 RID: 3705 RVA: 0x000D2C2C File Offset: 0x000D0E2C
	private Color PickColourFromPalette(ref List<ColourPalettePreset> palettes, string debug = "")
	{
		Color result = Color.black;
		if (palettes.Count <= 0)
		{
			Debug.LogError("No palettes selected...");
			return result;
		}
		List<Color> list = new List<Color>();
		foreach (ColourPalettePreset colourPalettePreset in palettes)
		{
			float num = (float)this.human.GetHexacoScore(ref colourPalettePreset.hexaco);
			foreach (ColourPalettePreset.MaterialSettings materialSettings in colourPalettePreset.colours)
			{
				int num2 = Mathf.RoundToInt(num * (float)materialSettings.weighting * 0.5f);
				for (int i = 0; i < num2; i++)
				{
					list.Add(materialSettings.colour);
				}
			}
		}
		if (list.Count <= 0)
		{
			Debug.Log("No possible colours: " + debug);
			return result;
		}
		int num3;
		if (!Application.isEditor)
		{
			num3 = Toolbox.Instance.GetPsuedoRandomNumberContained(0, list.Count, this.human.seed, out this.human.seed);
		}
		else
		{
			num3 = Random.Range(0, list.Count);
		}
		result = list[num3];
		return result;
	}

	// Token: 0x06000E7A RID: 3706 RVA: 0x000D2D8C File Offset: 0x000D0F8C
	private Color GetColourFromUnderneath(ClothesPreset thisPreset, ClothesPreset.OutfitCategory category, ClothesPreset.ClothingColourSource source, ref Dictionary<string, ClothesPreset> clothesDictionary)
	{
		Color black = Color.black;
		ClothesPreset.OutfitCategory lookInCategory = ClothesPreset.OutfitCategory.underwear;
		if (category == ClothesPreset.OutfitCategory.outdoorsCasual)
		{
			lookInCategory = ClothesPreset.OutfitCategory.casual;
		}
		else if (category == ClothesPreset.OutfitCategory.outdoorsSmart)
		{
			lookInCategory = ClothesPreset.OutfitCategory.smart;
		}
		else if (category == ClothesPreset.OutfitCategory.outdoorsWork)
		{
			lookInCategory = ClothesPreset.OutfitCategory.work;
		}
		CitizenOutfitController.Outfit outfit = this.outfits.Find((CitizenOutfitController.Outfit item) => item.category == lookInCategory);
		if (outfit != null)
		{
			CitizenOutfitController.OutfitClothes outfitClothes = null;
			float num = -99999f;
			foreach (CitizenOutfitController.OutfitClothes outfitClothes2 in outfit.clothes)
			{
				ClothesPreset clothesPreset = clothesDictionary[outfitClothes2.clothes];
				int num2 = 0;
				foreach (CitizenOutfitController.CharacterAnchor characterAnchor in clothesPreset.covers)
				{
					if (thisPreset.covers.Contains(characterAnchor))
					{
						num2++;
					}
				}
				if ((float)num2 > num)
				{
					outfitClothes = outfitClothes2;
					num = (float)num2;
				}
			}
			if (outfitClothes != null)
			{
				if (source == ClothesPreset.ClothingColourSource.underneathColour1)
				{
					return outfitClothes.color1;
				}
				if (source == ClothesPreset.ClothingColourSource.underneathColour2)
				{
					return outfitClothes.color2;
				}
				if (source == ClothesPreset.ClothingColourSource.underneathColour3)
				{
					return outfitClothes.color3;
				}
			}
		}
		return black;
	}

	// Token: 0x06000E7B RID: 3707 RVA: 0x000D2ED0 File Offset: 0x000D10D0
	public bool GetChance(Human human, ref List<ClothesPreset.TraitPickRule> pickRules, out int addChance)
	{
		bool flag = true;
		addChance = 0;
		foreach (ClothesPreset.TraitPickRule traitPickRule in pickRules)
		{
			bool flag2 = false;
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = true;
							break;
						}
					}
					goto IL_1D4;
				}
				goto IL_85;
			}
			goto IL_85;
			IL_1D4:
			if (flag2)
			{
				addChance += traitPickRule.addChance;
				continue;
			}
			if (traitPickRule.mustPassForApplication)
			{
				flag = false;
				continue;
			}
			continue;
			IL_85:
			if (traitPickRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_1D4;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag2 = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (human.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = false;
							break;
						}
					}
					goto IL_1D4;
				}
			}
			if (traitPickRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese && human.partner != null)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = traitPickRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (human.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag2 = true;
							break;
						}
					}
				}
				goto IL_1D4;
			}
			goto IL_1D4;
		}
		if (!flag)
		{
			addChance = 0;
			return false;
		}
		return true;
	}

	// Token: 0x0400112C RID: 4396
	[Header("Components/Anchors")]
	public Human human;

	// Token: 0x0400112D RID: 4397
	public LODGroup lod;

	// Token: 0x0400112E RID: 4398
	public MeshRenderer distantLOD;

	// Token: 0x0400112F RID: 4399
	public bool isPoser;

	// Token: 0x04001130 RID: 4400
	public ScenePoserController poser;

	// Token: 0x04001131 RID: 4401
	public List<CitizenOutfitController.AnchorConfig> anchorConfig = new List<CitizenOutfitController.AnchorConfig>();

	// Token: 0x04001132 RID: 4402
	public Dictionary<CitizenOutfitController.CharacterAnchor, Transform> anchorReference = new Dictionary<CitizenOutfitController.CharacterAnchor, Transform>();

	// Token: 0x04001133 RID: 4403
	public Transform pupilParent;

	// Token: 0x04001134 RID: 4404
	public Transform leftPupil;

	// Token: 0x04001135 RID: 4405
	public Transform rightPupil;

	// Token: 0x04001136 RID: 4406
	public Transform eyebrowParent;

	// Token: 0x04001137 RID: 4407
	public Transform rightEyebrow;

	// Token: 0x04001138 RID: 4408
	public Transform leftEyebrow;

	// Token: 0x04001139 RID: 4409
	public Transform mouth;

	// Token: 0x0400113A RID: 4410
	public List<MeshRenderer> eyeRenderers = new List<MeshRenderer>();

	// Token: 0x0400113B RID: 4411
	public List<MeshRenderer> eyebrowRenderers = new List<MeshRenderer>();

	// Token: 0x0400113C RID: 4412
	public MeshRenderer mouthRenderer;

	// Token: 0x0400113D RID: 4413
	[ReadOnly]
	public Vector3 pupilParentOffset;

	// Token: 0x0400113E RID: 4414
	public List<CitizenOutfitController.ExpressionSetup> expressions = new List<CitizenOutfitController.ExpressionSetup>();

	// Token: 0x0400113F RID: 4415
	public Dictionary<CitizenOutfitController.Expression, CitizenOutfitController.ExpressionSetup> expressionReference = new Dictionary<CitizenOutfitController.Expression, CitizenOutfitController.ExpressionSetup>();

	// Token: 0x04001140 RID: 4416
	[Space(5f)]
	public Material bluePupil;

	// Token: 0x04001141 RID: 4417
	public Material greenPupil;

	// Token: 0x04001142 RID: 4418
	public Material brownPupil;

	// Token: 0x04001143 RID: 4419
	public Material greyPupil;

	// Token: 0x04001144 RID: 4420
	[Header("Outfits")]
	public ClothesPreset.OutfitCategory loadedOutfit;

	// Token: 0x04001145 RID: 4421
	public ClothesPreset.OutfitCategory currentOutfit;

	// Token: 0x04001146 RID: 4422
	public ClothesPreset.OutfitCategory previousOutfit;

	// Token: 0x04001147 RID: 4423
	[NonSerialized]
	public List<CitizenOutfitController.OutfitClothes> currentlyLoadedClothes = new List<CitizenOutfitController.OutfitClothes>();

	// Token: 0x04001148 RID: 4424
	public List<MeshRenderer> allCurrentMeshes = new List<MeshRenderer>();

	// Token: 0x04001149 RID: 4425
	public List<MeshFilter> allCurrentMeshFilters = new List<MeshFilter>();

	// Token: 0x0400114A RID: 4426
	private ClothesPreset currentHair;

	// Token: 0x0400114B RID: 4427
	private MeshRenderer currentHairRend;

	// Token: 0x0400114C RID: 4428
	private ClothesPreset currentHat;

	// Token: 0x0400114D RID: 4429
	private MeshRenderer currentHatRend;

	// Token: 0x0400114E RID: 4430
	public List<CitizenOutfitController.Outfit> outfits = new List<CitizenOutfitController.Outfit>();

	// Token: 0x0400114F RID: 4431
	[Header("Debug")]
	public List<MeshRenderer> debugRenderers = new List<MeshRenderer>();

	// Token: 0x04001150 RID: 4432
	public bool debugOverride;

	// Token: 0x04001151 RID: 4433
	[EnableIf("debugOverride")]
	public OccupationPreset debugOverrideJob;

	// Token: 0x04001152 RID: 4434
	[EnableIf("debugOverride")]
	public Human.Gender debugOverrideGender = Human.Gender.female;

	// Token: 0x04001153 RID: 4435
	[EnableIf("debugOverride")]
	public Descriptors.BuildType debugOverrideBuild = Descriptors.BuildType.average;

	// Token: 0x04001154 RID: 4436
	[EnableIf("debugOverride")]
	public Descriptors.HairStyle debugOverrideHair = Descriptors.HairStyle.shortHair;

	// Token: 0x04001155 RID: 4437
	[EnableIf("debugOverride")]
	public Descriptors.EyeColour debugOverrideEyeColour;

	// Token: 0x04001156 RID: 4438
	[EnableIf("debugOverride")]
	public Human.ShoeType debugOverrideShoeType;

	// Token: 0x04001157 RID: 4439
	[Range(0f, 1f)]
	[EnableIf("debugOverride")]
	public float debugOverrideLipstick;

	// Token: 0x04001158 RID: 4440
	[EnableIf("debugOverride")]
	public Color debugOverrideSkinColour = Color.white;

	// Token: 0x04001159 RID: 4441
	[EnableIf("debugOverride")]
	public Color debugOverrideHairColour = Color.black;

	// Token: 0x0400115A RID: 4442
	[EnableIf("debugOverride")]
	public CitizenOutfitController.Expression debugOverrideExpression;

	// Token: 0x0400115B RID: 4443
	[EnableIf("debugOverride")]
	[Range(0f, 1f)]
	public float debugOverrideGrub = 0.1f;

	// Token: 0x0400115C RID: 4444
	[Space(5f)]
	public bool enableDebugLog;

	// Token: 0x0400115D RID: 4445
	public List<string> outfitDebug = new List<string>();

	// Token: 0x0400115E RID: 4446
	[Header("Load Specific")]
	public List<ClothesPreset> outfitToLoad = new List<ClothesPreset>();

	// Token: 0x0400115F RID: 4447
	[Header("Create New")]
	public string newClothingName;

	// Token: 0x04001160 RID: 4448
	public CitizenOutfitController.ClothingCreatorDirectory directory;

	// Token: 0x04001161 RID: 4449
	public List<GameObject> newClothingComponents = new List<GameObject>();

	// Token: 0x04001162 RID: 4450
	[Tooltip("If this is true, you only need the right side arms and/or legs present as models. The opposite side will be created with flipping.")]
	public bool CreateFlippedArmsAndLegsFromRightSide = true;

	// Token: 0x04001163 RID: 4451
	private Dictionary<CitizenOutfitController.CharacterAnchor, int> coveredAnchors = new Dictionary<CitizenOutfitController.CharacterAnchor, int>();

	// Token: 0x02000274 RID: 628
	public enum CharacterAnchor
	{
		// Token: 0x04001165 RID: 4453
		lowerTorso,
		// Token: 0x04001166 RID: 4454
		upperTorso,
		// Token: 0x04001167 RID: 4455
		Head,
		// Token: 0x04001168 RID: 4456
		Hat,
		// Token: 0x04001169 RID: 4457
		UpperArmRight,
		// Token: 0x0400116A RID: 4458
		UpperArmLeft,
		// Token: 0x0400116B RID: 4459
		LowerArmRight,
		// Token: 0x0400116C RID: 4460
		LowerArmLeft,
		// Token: 0x0400116D RID: 4461
		HandRight,
		// Token: 0x0400116E RID: 4462
		HandLeft,
		// Token: 0x0400116F RID: 4463
		UpperLegRight,
		// Token: 0x04001170 RID: 4464
		UpperLegLeft,
		// Token: 0x04001171 RID: 4465
		LowerLegRight,
		// Token: 0x04001172 RID: 4466
		LowerLegLeft,
		// Token: 0x04001173 RID: 4467
		Midriff,
		// Token: 0x04001174 RID: 4468
		RightFoot,
		// Token: 0x04001175 RID: 4469
		LeftFoot,
		// Token: 0x04001176 RID: 4470
		Hair,
		// Token: 0x04001177 RID: 4471
		Glasses,
		// Token: 0x04001178 RID: 4472
		ArmsParent,
		// Token: 0x04001179 RID: 4473
		beard
	}

	// Token: 0x02000275 RID: 629
	[Serializable]
	public class AnchorConfig
	{
		// Token: 0x0400117A RID: 4474
		public CitizenOutfitController.CharacterAnchor anchor;

		// Token: 0x0400117B RID: 4475
		public Transform trans;

		// Token: 0x0400117C RID: 4476
		public bool outline;

		// Token: 0x0400117D RID: 4477
		public bool captureInSurveillance = true;

		// Token: 0x0400117E RID: 4478
		public float weight = 2f;
	}

	// Token: 0x02000276 RID: 630
	[Serializable]
	public class Outfit
	{
		// Token: 0x0400117F RID: 4479
		public ClothesPreset.OutfitCategory category;

		// Token: 0x04001180 RID: 4480
		public List<CitizenOutfitController.OutfitClothes> clothes = new List<CitizenOutfitController.OutfitClothes>();
	}

	// Token: 0x02000277 RID: 631
	[Serializable]
	public class OutfitClothes
	{
		// Token: 0x04001181 RID: 4481
		public string clothes;

		// Token: 0x04001182 RID: 4482
		public List<ClothesPreset.ClothesTags> tags = new List<ClothesPreset.ClothesTags>();

		// Token: 0x04001183 RID: 4483
		public Color baseColor = Color.clear;

		// Token: 0x04001184 RID: 4484
		public Color color1 = Color.clear;

		// Token: 0x04001185 RID: 4485
		public Color color2 = Color.clear;

		// Token: 0x04001186 RID: 4486
		public Color color3 = Color.clear;

		// Token: 0x04001187 RID: 4487
		public bool borrowed;

		// Token: 0x04001188 RID: 4488
		[NonSerialized]
		public Dictionary<CitizenOutfitController.CharacterAnchor, List<MeshRenderer>> spawned = new Dictionary<CitizenOutfitController.CharacterAnchor, List<MeshRenderer>>();

		// Token: 0x04001189 RID: 4489
		[NonSerialized]
		public int rank = 2;

		// Token: 0x0400118A RID: 4490
		[NonSerialized]
		public bool incomplete;

		// Token: 0x0400118B RID: 4491
		[NonSerialized]
		public bool loadedThisCycle;
	}

	// Token: 0x02000278 RID: 632
	public struct BackupCovering
	{
		// Token: 0x0400118C RID: 4492
		public CitizenOutfitController.Outfit outfit;

		// Token: 0x0400118D RID: 4493
		public ClothesPreset preset;
	}

	// Token: 0x02000279 RID: 633
	public class NewClothingCreation
	{
		// Token: 0x0400118E RID: 4494
		public GameObject newPrefab;

		// Token: 0x0400118F RID: 4495
		public Vector3 offset;

		// Token: 0x04001190 RID: 4496
		public Vector3 euler;
	}

	// Token: 0x0200027A RID: 634
	public enum ClothingCreatorDirectory
	{
		// Token: 0x04001192 RID: 4498
		Tops,
		// Token: 0x04001193 RID: 4499
		Bottoms,
		// Token: 0x04001194 RID: 4500
		Hats,
		// Token: 0x04001195 RID: 4501
		Heads,
		// Token: 0x04001196 RID: 4502
		Shoes,
		// Token: 0x04001197 RID: 4503
		Underwear,
		// Token: 0x04001198 RID: 4504
		Undressed
	}

	// Token: 0x0200027B RID: 635
	public enum Expression
	{
		// Token: 0x0400119A RID: 4506
		neutral,
		// Token: 0x0400119B RID: 4507
		angry,
		// Token: 0x0400119C RID: 4508
		sad,
		// Token: 0x0400119D RID: 4509
		surprised,
		// Token: 0x0400119E RID: 4510
		happy,
		// Token: 0x0400119F RID: 4511
		asleep
	}

	// Token: 0x0200027C RID: 636
	[Serializable]
	public class ExpressionSetup
	{
		// Token: 0x040011A0 RID: 4512
		public CitizenOutfitController.Expression expression;

		// Token: 0x040011A1 RID: 4513
		public Vector3 eyebrowsEuler;

		// Token: 0x040011A2 RID: 4514
		public float eyebrowsRaise;

		// Token: 0x040011A3 RID: 4515
		public float eyeHeightMultiplier;

		// Token: 0x040011A4 RID: 4516
		public bool allowBlinking;
	}
}
