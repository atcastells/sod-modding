using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020002D5 RID: 725
public class GroupsController : MonoBehaviour
{
	// Token: 0x17000082 RID: 130
	// (get) Token: 0x06001089 RID: 4233 RVA: 0x000E9E0D File Offset: 0x000E800D
	public static GroupsController Instance
	{
		get
		{
			return GroupsController._instance;
		}
	}

	// Token: 0x0600108A RID: 4234 RVA: 0x000E9E14 File Offset: 0x000E8014
	private void Awake()
	{
		if (GroupsController._instance != null && GroupsController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		GroupsController._instance = this;
	}

	// Token: 0x0600108B RID: 4235 RVA: 0x000E9E44 File Offset: 0x000E8044
	public void CreateGroups()
	{
		using (List<GroupPreset>.Enumerator enumerator = Toolbox.Instance.allGroups.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				GroupPreset g = enumerator.Current;
				int num = 1;
				List<Citizen> list = new List<Citizen>();
				if (g.groupType == GroupPreset.GroupType.couples)
				{
					foreach (Citizen citizen in CityData.Instance.homedDirectory)
					{
						if (citizen.partner != null && !list.Contains(citizen.partner))
						{
							list.Add(citizen);
						}
					}
					num = list.Count;
				}
				else if (g.groupType == GroupPreset.GroupType.cheaters)
				{
					foreach (Citizen citizen2 in CityData.Instance.homedDirectory)
					{
						if (citizen2.paramour != null && !list.Contains(citizen2.paramour))
						{
							list.Add(citizen2);
						}
					}
					num = list.Count;
				}
				else if (g.groupType == GroupPreset.GroupType.work)
				{
					foreach (Company company in CityData.Instance.companyDirectory)
					{
						if (!company.preset.isSelfEmployed)
						{
							Human human = null;
							foreach (Occupation occupation in company.companyRoster)
							{
								if (occupation.employee != null && occupation.employee.extraversion > 0.5f && (human == null || occupation.employee.extraversion > human.extraversion))
								{
									human = occupation.employee;
								}
							}
							if (human != null)
							{
								list.Add(human as Citizen);
							}
						}
					}
					num = list.Count;
				}
				Predicate<Company> <>9__0;
				for (int i = 0; i < num; i++)
				{
					if (Toolbox.Instance.SeedRand(0f, 1f) <= g.chance)
					{
						bool flag = false;
						List<Citizen> list2 = new List<Citizen>();
						List<SessionData.WeekDay> list3 = new List<SessionData.WeekDay>();
						float startingDecimalHour = Mathf.Round(Toolbox.Instance.SeedRand(g.timeRange.x, g.timeRange.y) / 0.5f) * 0.5f;
						List<Company> list4 = new List<Company>();
						if (g.enableMeetUps)
						{
							List<Company> companyDirectory = CityData.Instance.companyDirectory;
							Predicate<Company> predicate;
							if ((predicate = <>9__0) == null)
							{
								predicate = (<>9__0 = ((Company item) => item.preset != null && !item.preset.isSelfEmployed && item.address != null && g.meetUpLocations.Contains(item.preset)));
							}
							list4 = companyDirectory.FindAll(predicate);
							if (list4.Count > 0)
							{
								bool flag2 = true;
								if (g.groupType == GroupPreset.GroupType.couples)
								{
									list2.Add(list[i]);
									list2.Add(list[i].partner);
									flag2 = this.DecimalTimeFinder(g, list2, out startingDecimalHour, out list3);
								}
								else if (g.groupType == GroupPreset.GroupType.cheaters)
								{
									list2.Add(list[i]);
									list2.Add(list[i].paramour);
									flag2 = this.DecimalTimeFinder(g, list2, out startingDecimalHour, out list3);
								}
								else if (g.groupType == GroupPreset.GroupType.work)
								{
									Citizen citizen3 = list[i];
									list2.Add(citizen3);
									foreach (Occupation occupation2 in citizen3.job.employer.companyRoster)
									{
										if (occupation2.employee != null && !(occupation2.employee == citizen3))
										{
											Acquaintance acquaintance = null;
											if (citizen3.FindAcquaintanceExists(occupation2.employee, out acquaintance) && acquaintance.like > 0.4f)
											{
												if (list2.Count < g.maxMembers)
												{
													list2.Add(occupation2.employee as Citizen);
												}
												else
												{
													for (int j = 0; j < list2.Count; j++)
													{
														Citizen citizen4 = list2[j];
														if (!(citizen4 == citizen3))
														{
															Acquaintance acquaintance2 = null;
															if (citizen3.FindAcquaintanceExists(citizen4, out acquaintance2) && acquaintance2.like < acquaintance.like)
															{
																list2.RemoveAt(j);
																list2.Add(occupation2.employee as Citizen);
																break;
															}
														}
													}
												}
											}
										}
									}
									flag2 = this.DecimalTimeFinder(g, list2, out startingDecimalHour, out list3);
								}
								else
								{
									for (int k = 0; k < g.daysPerWeek; k++)
									{
										int num2 = Toolbox.Instance.SeedRand(0, 7);
										while (list3.Contains((SessionData.WeekDay)num2))
										{
											num2 += 2;
											if (num2 >= 7)
											{
												num2 -= 7;
											}
										}
										list3.Add((SessionData.WeekDay)num2);
									}
								}
								using (List<SessionData.WeekDay>.Enumerator enumerator5 = list3.GetEnumerator())
								{
									while (enumerator5.MoveNext())
									{
										SessionData.WeekDay d = enumerator5.Current;
										list4.RemoveAll((Company item) => !item.publicFacing || !item.IsOpenAtDecimalTime(d, startingDecimalHour) || !item.IsOpenAtDecimalTime(d, startingDecimalHour + g.meetUpLength));
									}
								}
								if (flag2 && list4.Count > 0)
								{
									flag = true;
								}
							}
						}
						else
						{
							flag = true;
						}
						if (flag)
						{
							if (g.groupType == GroupPreset.GroupType.interestGroup)
							{
								List<Citizen> list5 = new List<Citizen>(CityData.Instance.homedDirectory);
								while (list5.Count > 0 && list2.Count < g.maxMembers)
								{
									int num3 = Toolbox.Instance.SeedRand(0, list5.Count);
									Citizen citizen5 = list5[num3];
									if (!citizen5.isPlayer && citizen5.extraversion >= g.minimumExtraversion)
									{
										bool flag3 = true;
										if (g.requiredTraits.Count > 0)
										{
											using (List<CharacterTrait>.Enumerator enumerator6 = g.requiredTraits.GetEnumerator())
											{
												while (enumerator6.MoveNext())
												{
													CharacterTrait t = enumerator6.Current;
													if (!citizen5.characterTraits.Exists((Human.Trait item) => item.trait == t))
													{
														flag3 = false;
														break;
													}
												}
											}
										}
										if (g.enableMeetUps && flag3 && citizen5.job != null && citizen5.job.employer != null)
										{
											bool flag4 = false;
											foreach (SessionData.WeekDay weekDay in citizen5.job.workDaysList)
											{
												if (!list3.Contains(weekDay))
												{
													flag4 = true;
													break;
												}
											}
											if ((!flag4 && startingDecimalHour >= citizen5.job.endTimeDecialHour) || startingDecimalHour + 1f <= citizen5.job.startTimeDecimalHour)
											{
												flag3 = true;
											}
											else if (!flag4)
											{
												flag3 = false;
											}
										}
										if (flag3)
										{
											list2.Add(citizen5);
										}
									}
									list5.RemoveAt(num3);
								}
								foreach (Citizen citizen6 in CityData.Instance.homedDirectory)
								{
									if (citizen6.job != null && citizen6.job.preset.joinGroups.Contains(g) && !list2.Contains(citizen6))
									{
										Game.Log("CityGen: Adding citizen to group through job requirement: " + g.name, 2);
										list2.Add(citizen6);
									}
								}
							}
							if (list2.Count >= g.minMembers)
							{
								NewAddress newAddress = null;
								if (g.enableMeetUps)
								{
									newAddress = list4[Toolbox.Instance.SeedRand(0, list4.Count)].address;
								}
								string[] array = new string[8];
								array[0] = "CityGen: Group ";
								array[1] = g.name;
								array[2] = " created with ";
								array[3] = list2.Count.ToString();
								array[4] = " members, meeting at ";
								int num4 = 5;
								NewAddress newAddress2 = newAddress;
								array[num4] = ((newAddress2 != null) ? newAddress2.ToString() : null);
								array[6] = " load state key: ";
								array[7] = Toolbox.Instance.lastRandomNumberKey;
								Game.Log(string.Concat(array), 2);
								GroupsController.SocialGroup socialGroup = new GroupsController.SocialGroup();
								socialGroup.decimalStartTime = startingDecimalHour;
								socialGroup.preset = g.name;
								socialGroup.members = new List<int>();
								socialGroup.weekDays = new List<SessionData.WeekDay>(list3);
								if (newAddress != null)
								{
									socialGroup.meetingPlace = newAddress.id;
								}
								socialGroup.id = GroupsController.assignID;
								GroupsController.assignID++;
								foreach (Citizen citizen7 in list2)
								{
									socialGroup.members.Add(citizen7.humanID);
									citizen7.groups.Add(socialGroup);
									foreach (Citizen citizen8 in list2)
									{
										if (!(citizen7 == citizen8))
										{
											citizen7.AddAcquaintance(citizen8, Toolbox.Instance.VectorToRandomSeedContained(SocialControls.Instance.knowGroupRange, citizen7.seed, out citizen7.seed), Acquaintance.ConnectionType.groupMember, true, false, Acquaintance.ConnectionType.friend, socialGroup);
										}
									}
								}
								this.groups.Add(socialGroup);
								foreach (GroupPreset.MeetUpVmailThread meetUpVmailThread in g.vmails)
								{
									List<Human> list6 = null;
									List<Human> otherParticipiants = new List<Human>();
									if (this.GetVmailGroupParticiapnts(socialGroup, meetUpVmailThread.sender, out list6) && this.GetVmailGroupParticiapnts(socialGroup, meetUpVmailThread.recevier, out otherParticipiants))
									{
										DDSSaveClasses.DDSTreeSave ddstreeSave = null;
										if (!Toolbox.Instance.allDDSTrees.TryGetValue(meetUpVmailThread.treeID, ref ddstreeSave))
										{
											Game.LogError("Cannot find vmail tree " + meetUpVmailThread.treeID, 2);
											return;
										}
										Toolbox.Instance.NewVmailThread(list6[0], otherParticipiants, meetUpVmailThread.treeID, SessionData.Instance.gameTime + Toolbox.Instance.SeedRand(-48f, -12f), Toolbox.Instance.SeedRand(0, ddstreeSave.messageRef.Count + 1), StateSaveData.CustomDataSource.groupID, socialGroup.id);
									}
								}
								using (List<GroupPreset.ClubClue>.Enumerator enumerator9 = g.clues.GetEnumerator())
								{
									while (enumerator9.MoveNext())
									{
										GroupPreset.ClubClue clubClue = enumerator9.Current;
										List<Interactable.Passed> list7 = new List<Interactable.Passed>();
										list7.Add(new Interactable.Passed(Interactable.PassedVarType.groupID, (float)socialGroup.id, null));
										if (clubClue.spawnAt == GroupPreset.SpawnAt.meetingPlace)
										{
											if (newAddress != null)
											{
												FurnitureLocation furnitureLocation;
												newAddress.PlaceObject(clubClue.preset, list2[0], list2[0], null, out furnitureLocation, list7, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
											}
										}
										else if (clubClue.spawnAt == GroupPreset.SpawnAt.leadersApartment)
										{
											if (list2[0].home != null)
											{
												FurnitureLocation furnitureLocation;
												list2[0].home.PlaceObject(clubClue.preset, list2[0], list2[0], null, out furnitureLocation, list7, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
											}
										}
										else if (clubClue.spawnAt == GroupPreset.SpawnAt.entireGroupsApartments)
										{
											foreach (Human human2 in list2)
											{
												if (human2.home != null)
												{
													FurnitureLocation furnitureLocation;
													human2.home.PlaceObject(clubClue.preset, list2[0], list2[0], null, out furnitureLocation, list7, false, 0, InteractablePreset.OwnedPlacementRule.nonOwnedOnly, 0, null, false, null, null, null, "", false);
												}
											}
										}
									}
									goto IL_D86;
								}
							}
							Game.Log("CityGen: Not enough members (" + list2.Count.ToString() + ") to create " + g.name, 2);
						}
					}
					IL_D86:;
				}
			}
		}
	}

	// Token: 0x0600108C RID: 4236 RVA: 0x000EAD70 File Offset: 0x000E8F70
	public bool GetVmailGroupParticiapnts(GroupsController.SocialGroup group, GroupPreset.MeetUpVmailSender setting, out List<Human> particiapnts)
	{
		particiapnts = new List<Human>();
		if (setting == GroupPreset.MeetUpVmailSender.groupLeader && group.members.Count > 0)
		{
			Human human = null;
			if (!CityData.Instance.GetHuman(group.members[0], out human, true))
			{
				return false;
			}
			particiapnts.Add(human);
		}
		else
		{
			if (setting == GroupPreset.MeetUpVmailSender.entireGroup)
			{
				using (List<int>.Enumerator enumerator = group.members.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						int id = enumerator.Current;
						Human human2 = null;
						if (CityData.Instance.GetHuman(id, out human2, true))
						{
							particiapnts.Add(human2);
						}
					}
					return true;
				}
			}
			if (setting == GroupPreset.MeetUpVmailSender.groupRandom && group.members.Count > 0)
			{
				Human human3 = null;
				if (CityData.Instance.GetHuman(group.members[Toolbox.Instance.SeedRand(0, group.members.Count)], out human3, true))
				{
					particiapnts.Add(human3);
				}
			}
			else if (setting == GroupPreset.MeetUpVmailSender.meetupPlace && group.meetingPlace > -1)
			{
				NewAddress newAddress = null;
				if (!CityData.Instance.addressDictionary.TryGetValue(group.meetingPlace, ref newAddress))
				{
					return false;
				}
				if (newAddress.owners.Count <= 0)
				{
					return false;
				}
				particiapnts.Add(newAddress.owners[0]);
			}
			else if (setting == GroupPreset.MeetUpVmailSender.prioritiseFaithful)
			{
				foreach (int id2 in group.members)
				{
					Human human4 = null;
					if (CityData.Instance.GetHuman(id2, out human4, true) && human4.paramour == null)
					{
						particiapnts.Add(human4);
						return true;
					}
					Human human5 = null;
					if (CityData.Instance.GetHuman(group.members[Toolbox.Instance.SeedRand(0, group.members.Count)], out human5, true))
					{
						particiapnts.Add(human5);
					}
				}
				return true;
			}
		}
		return true;
	}

	// Token: 0x0600108D RID: 4237 RVA: 0x000EAF80 File Offset: 0x000E9180
	public void LoadGroups()
	{
		if (CityConstructor.Instance.currentData != null)
		{
			this.groups = new List<GroupsController.SocialGroup>(CityConstructor.Instance.currentData.groups);
			foreach (GroupsController.SocialGroup socialGroup in this.groups)
			{
				foreach (int num in socialGroup.members)
				{
					Human human = null;
					if (CityData.Instance.GetHuman(num, out human, true))
					{
						human.groups.Add(socialGroup);
						foreach (int num2 in socialGroup.members)
						{
							if (num2 != num)
							{
								Acquaintance acquaintance = null;
								Human findC = null;
								if (CityData.Instance.GetHuman(num2, out findC, true) && human.FindAcquaintanceExists(findC, out acquaintance))
								{
									acquaintance.group = socialGroup;
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x0600108E RID: 4238 RVA: 0x000EB0CC File Offset: 0x000E92CC
	public bool DecimalTimeFinder(GroupPreset g, List<Citizen> people, out float appropriateTime, out List<SessionData.WeekDay> availableDays)
	{
		bool flag = false;
		availableDays = new List<SessionData.WeekDay>();
		appropriateTime = Mathf.Round(Toolbox.Instance.SeedRand(g.timeRange.x, g.timeRange.y) / 0.5f) * 0.5f;
		Vector2 vector;
		vector..ctor(appropriateTime, appropriateTime + g.meetUpLength);
		if (vector.y > 24f)
		{
			vector.y -= 24f;
		}
		if (vector.y < 0f)
		{
			vector.y += 24f;
		}
		float num = 0f;
		while (!flag && num <= 24f)
		{
			flag = true;
			foreach (Citizen citizen in people)
			{
				if (citizen.job != null && citizen.job.employer != null)
				{
					float num2 = citizen.job.startTimeDecimalHour - 1f;
					float num3 = citizen.job.endTimeDecialHour + 1f;
					if (num2 > 24f)
					{
						num2 -= 24f;
					}
					if (num2 < 0f)
					{
						num2 += 24f;
					}
					if (num3 > 24f)
					{
						num3 -= 24f;
					}
					if (num3 < 0f)
					{
						num3 += 24f;
					}
					if (!Toolbox.Instance.DecimalTimeRangeOverlap(new Vector2(num2, num3), vector, true))
					{
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				appropriateTime += 0.5f;
				num += 0.5f;
				if (appropriateTime >= 24f)
				{
					appropriateTime -= 24f;
				}
			}
		}
		if (flag)
		{
			for (int i = 0; i < g.daysPerWeek; i++)
			{
				int num4 = Toolbox.Instance.SeedRand(0, 7);
				while (availableDays.Contains((SessionData.WeekDay)num4))
				{
					num4 += 2;
					if (num4 >= 7)
					{
						num4 -= 7;
					}
				}
				availableDays.Add((SessionData.WeekDay)num4);
			}
		}
		else
		{
			appropriateTime = Mathf.Round(Toolbox.Instance.SeedRand(g.timeRange.x, g.timeRange.y) / 0.5f) * 0.5f;
			List<SessionData.WeekDay> list = new List<SessionData.WeekDay>();
			for (int j = 0; j < 7; j++)
			{
				bool flag2 = true;
				foreach (Citizen citizen2 in people)
				{
					if (citizen2.job != null && citizen2.job.employer != null && citizen2.job.workDaysList.Contains((SessionData.WeekDay)j))
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					list.Add((SessionData.WeekDay)j);
				}
			}
			for (int k = 0; k < g.daysPerWeek; k++)
			{
				if (list.Count > 0)
				{
					int num5 = Toolbox.Instance.SeedRand(0, list.Count);
					availableDays.Add(list[num5]);
					list.RemoveAt(num5);
				}
			}
			if (availableDays.Count > 0)
			{
				flag = true;
			}
		}
		return flag;
	}

	// Token: 0x040014A2 RID: 5282
	[Header("Groups")]
	public List<GroupsController.SocialGroup> groups = new List<GroupsController.SocialGroup>();

	// Token: 0x040014A3 RID: 5283
	public static int assignID = 1;

	// Token: 0x040014A4 RID: 5284
	private static GroupsController _instance;

	// Token: 0x020002D6 RID: 726
	[Serializable]
	public class SocialGroup
	{
		// Token: 0x06001091 RID: 4241 RVA: 0x000EB420 File Offset: 0x000E9620
		public NewAddress GetMeetingPlace()
		{
			NewAddress result = null;
			if (this.meetingPlace > -1)
			{
				CityData.Instance.addressDictionary.TryGetValue(this.meetingPlace, ref result);
			}
			return result;
		}

		// Token: 0x06001092 RID: 4242 RVA: 0x000EB454 File Offset: 0x000E9654
		public float GetNextMeetingTime()
		{
			float meetUpLength = Toolbox.Instance.groupsDictionary[this.preset].meetUpLength;
			float num = this.decimalStartTime + meetUpLength;
			if (num >= 24f)
			{
				num -= 24f;
			}
			return SessionData.Instance.GetNextOrPreviousGameTimeForThisHour(this.weekDays, this.decimalStartTime, num);
		}

		// Token: 0x06001093 RID: 4243 RVA: 0x000EB4AC File Offset: 0x000E96AC
		public GroupPreset GetPreset()
		{
			return Toolbox.Instance.groupsDictionary[this.preset];
		}

		// Token: 0x040014A5 RID: 5285
		public string preset;

		// Token: 0x040014A6 RID: 5286
		public int id;

		// Token: 0x040014A7 RID: 5287
		public float decimalStartTime;

		// Token: 0x040014A8 RID: 5288
		public List<SessionData.WeekDay> weekDays;

		// Token: 0x040014A9 RID: 5289
		public List<int> members;

		// Token: 0x040014AA RID: 5290
		public int meetingPlace;

		// Token: 0x040014AB RID: 5291
		[NonSerialized]
		public List<Interactable> reserved = new List<Interactable>();
	}
}
