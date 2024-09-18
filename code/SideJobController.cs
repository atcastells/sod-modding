using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000312 RID: 786
public class SideJobController : MonoBehaviour
{
	// Token: 0x1700008C RID: 140
	// (get) Token: 0x060011EB RID: 4587 RVA: 0x000FFD8F File Offset: 0x000FDF8F
	public static SideJobController Instance
	{
		get
		{
			return SideJobController._instance;
		}
	}

	// Token: 0x060011EC RID: 4588 RVA: 0x000FFD96 File Offset: 0x000FDF96
	private void Awake()
	{
		if (SideJobController._instance != null && SideJobController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		SideJobController._instance = this;
	}

	// Token: 0x060011ED RID: 4589 RVA: 0x000FFDC4 File Offset: 0x000FDFC4
	private void Start()
	{
		using (List<JobPreset>.Enumerator enumerator = Toolbox.Instance.allSideJobs.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				JobPreset p = enumerator.Current;
				if (this.jobTracking.Find((SideJobController.JobTracking item) => item.preset == p && !item.preset.disabled) == null)
				{
					SideJobController.JobTracking jobTracking = new SideJobController.JobTracking();
					jobTracking.name = p.name;
					jobTracking.preset = p;
					this.jobTracking.Add(jobTracking);
					foreach (KeyValuePair<int, SideJob> keyValuePair in this.allJobsDictionary)
					{
						if (keyValuePair.Value.preset == p)
						{
							if (keyValuePair.Value.state == SideJob.JobState.ended)
							{
								if (!jobTracking.endedJobs.Contains(keyValuePair.Value))
								{
									jobTracking.endedJobs.Add(keyValuePair.Value);
								}
							}
							else if (!jobTracking.activeJobs.Contains(keyValuePair.Value))
							{
								jobTracking.activeJobs.Add(keyValuePair.Value);
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x000FFF40 File Offset: 0x000FE140
	public void JobCreationCheck()
	{
		if (!this.enableJobs)
		{
			return;
		}
		if (!SessionData.Instance.startedGame)
		{
			return;
		}
		foreach (SideJobController.JobTracking jobTracking in this.jobTracking)
		{
			JobPreset preset = jobTracking.preset;
			if (!preset.disabled)
			{
				jobTracking.desiredActiveInstances = preset.GetFrequencyForSocialCreditLevel();
				if (jobTracking.activeJobs.Count < jobTracking.desiredActiveInstances)
				{
					Game.Log(string.Concat(new string[]
					{
						"Jobs: ",
						preset.name,
						" has lower active jobs than desired (",
						jobTracking.activeJobs.Count.ToString(),
						"/",
						jobTracking.desiredActiveInstances.ToString(),
						")"
					}), 2);
					List<SideJobController.JobPickData> list = new List<SideJobController.JobPickData>();
					foreach (Citizen citizen in CityData.Instance.citizenDirectory)
					{
						if (!this.exemptFromPurps.Contains(citizen) && !citizen.isDead && !citizen.isPlayer)
						{
							foreach (MotivePreset motivePreset in preset.purpetratorMotives)
							{
								if ((!citizen.isHomeless || motivePreset.allowHomelessPurps) && ((citizen.job != null && citizen.job.employer != null) || motivePreset.allowJoblessPurps))
								{
									int num = 0;
									if (this.MotivePass(ref motivePreset.purpTraitModifiers, citizen, out num) && (!motivePreset.usePurpJobs || motivePreset.purpJobs.Count <= 0 || motivePreset.purpJobs.Contains(citizen.job.preset)))
									{
										if (motivePreset.usePosterConnections)
										{
											using (List<Acquaintance>.Enumerator enumerator4 = citizen.acquaintances.GetEnumerator())
											{
												while (enumerator4.MoveNext())
												{
													Acquaintance acquaintance = enumerator4.Current;
													if (!(acquaintance.with as Citizen == null) && !this.exemptFromPosters.Contains(acquaintance.with as Citizen) && (!acquaintance.with.isHomeless || motivePreset.allowHomelessPosters) && ((acquaintance.with.job != null && acquaintance.with.job.employer != null) || motivePreset.allowJoblessPosters))
													{
														bool flag = false;
														foreach (Acquaintance.ConnectionType connectionType in acquaintance.connections)
														{
															if (motivePreset.acceptableConnections.Contains(connectionType))
															{
																flag = true;
																break;
															}
															if (motivePreset.acceptableConnections.Contains(Acquaintance.ConnectionType.anyAcquaintance))
															{
																flag = true;
																break;
															}
															if (motivePreset.acceptableConnections.Contains(Acquaintance.ConnectionType.friendOrWork) && (acquaintance.connections.Contains(Acquaintance.ConnectionType.friend) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workTeam) || acquaintance.connections.Contains(Acquaintance.ConnectionType.workOther) || acquaintance.connections.Contains(Acquaintance.ConnectionType.familiarWork)))
															{
																flag = true;
																break;
															}
															if (motivePreset.acceptableConnections.Contains(Acquaintance.ConnectionType.knowsName) && acquaintance.dataKeys.Contains(Evidence.DataKey.name))
															{
																flag = true;
																break;
															}
														}
														if (!flag && motivePreset.acceptableConnections.Contains(acquaintance.secretConnection))
														{
															flag = true;
														}
														if (flag)
														{
															int num2 = 0;
															if ((!motivePreset.usePosterTraits || this.MotivePass(ref motivePreset.posterTraitModifiers, acquaintance.with as Citizen, out num2)) && (motivePreset.allowEnforcers || !citizen.isEnforcer))
															{
																if (motivePreset.purpMustLiveAtDifferentAddressToPoster)
																{
																	if (citizen.home == acquaintance.with.home)
																	{
																		continue;
																	}
																}
																else
																{
																	num2 -= 2;
																}
																SideJobController.JobPickData jobPickData = new SideJobController.JobPickData();
																jobPickData.motive = motivePreset;
																jobPickData.purp = citizen;
																jobPickData.poster = (acquaintance.with as Citizen);
																jobPickData.score = (float)(num + num2) + Toolbox.Instance.Rand(0f, 0.25f, false);
																if (citizen.home != null && acquaintance.with.home != null && citizen.home.building == acquaintance.with.home.building)
																{
																	jobPickData.score -= (float)preset.penaltyForPurpAndPosterSameBuilding;
																}
																list.Add(jobPickData);
															}
														}
													}
												}
												continue;
											}
										}
										foreach (Citizen citizen2 in CityData.Instance.citizenDirectory)
										{
											if (!(citizen2 == citizen) && (!citizen2.isHomeless || motivePreset.allowHomelessPosters) && ((citizen2.job != null && citizen2.job.employer != null) || motivePreset.allowJoblessPosters) && !this.exemptFromPosters.Contains(citizen2))
											{
												int num3 = 0;
												if ((!motivePreset.usePosterTraits || this.MotivePass(ref motivePreset.posterTraitModifiers, citizen2, out num3)) && (motivePreset.allowEnforcers || !citizen.isEnforcer))
												{
													if (motivePreset.purpMustLiveAtDifferentAddressToPoster)
													{
														if (citizen.home == citizen2.home)
														{
															continue;
														}
													}
													else
													{
														num3 -= 2;
													}
													SideJobController.JobPickData jobPickData2 = new SideJobController.JobPickData();
													jobPickData2.motive = motivePreset;
													jobPickData2.purp = citizen;
													jobPickData2.poster = citizen2;
													jobPickData2.score = (float)(num + num3) + Toolbox.Instance.Rand(0f, 0.25f, false);
													if (citizen.home != null && citizen2.home != null && citizen.home.building == citizen2.home.building)
													{
														jobPickData2.score -= (float)preset.penaltyForPurpAndPosterSameBuilding;
													}
													list.Add(jobPickData2);
												}
											}
										}
									}
								}
							}
						}
					}
					list.Sort((SideJobController.JobPickData p1, SideJobController.JobPickData p2) => p2.score.CompareTo(p1.score));
					if (list.Count > 0)
					{
						SideJobController.JobPickData jobPickData3 = list[0];
						bool flag2 = false;
						if (jobTracking.activeJobs.Count < preset.immediatePostCountThreshold)
						{
							flag2 = true;
						}
						string text = "SideJob";
						if (preset.subClass.Length > 0)
						{
							text = "SideJob" + preset.subClass;
						}
						object[] array = new object[]
						{
							preset,
							jobPickData3,
							flag2
						};
						Game.Log(string.Concat(new string[]
						{
							"Jobs: Creating new job: ",
							text,
							" (",
							jobPickData3.motive.name,
							")"
						}), 2);
						SideJob sideJob = Activator.CreateInstance(Type.GetType(text), array) as SideJob;
						if (sideJob != null && !jobTracking.activeJobs.Contains(sideJob))
						{
							jobTracking.activeJobs.Add(sideJob);
						}
					}
					else
					{
						Game.Log("Jobs: No compatible job matches for " + preset.presetName, 2);
					}
				}
				else if (jobTracking.activeJobs.Count > jobTracking.desiredActiveInstances && SessionData.Instance.startedGame)
				{
					for (int i = 0; i < jobTracking.activeJobs.Count; i++)
					{
						SideJob sideJob2 = jobTracking.activeJobs[i];
						if (!sideJob2.accepted && sideJob2.post != null && !sideJob2.post.rPl && sideJob2.post.node != null && Player.Instance.currentGameLocation != null && sideJob2.post.node.gameLocation != Player.Instance.currentGameLocation && sideJob2.post.node.tile.cityTile != Player.Instance.currentCityTile)
						{
							Game.Log(string.Concat(new string[]
							{
								"Jobs: Removing job ",
								sideJob2.preset.name,
								" as there are too many jobs (",
								jobTracking.activeJobs.Count.ToString(),
								") for this social credit level (",
								jobTracking.desiredActiveInstances.ToString(),
								")"
							}), 2);
							sideJob2.End();
							if (jobTracking.activeJobs.Count <= jobTracking.desiredActiveInstances)
							{
								break;
							}
						}
					}
				}
				for (int j = 0; j < jobTracking.activeJobs.Count; j++)
				{
					jobTracking.activeJobs[j].GameWorldLoop();
				}
			}
		}
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0010092C File Offset: 0x000FEB2C
	public void AddExemptFromPostersJob(Human cit, SideJob job)
	{
		if (job != null)
		{
			if (!this.exemptFromPostersJobs.ContainsKey(cit))
			{
				this.exemptFromPostersJobs.Add(cit, new List<SideJob>());
			}
			if (!this.exemptFromPostersJobs[cit].Contains(job))
			{
				this.exemptFromPostersJobs[cit].Add(job);
			}
		}
		if (!this.exemptFromPosters.Contains(cit))
		{
			this.exemptFromPosters.Add(cit);
		}
	}

	// Token: 0x060011F0 RID: 4592 RVA: 0x0010099C File Offset: 0x000FEB9C
	public void AddExemptFromPurpJob(Human cit, SideJob job)
	{
		if (cit == null)
		{
			return;
		}
		if (job != null)
		{
			if (!this.exemptFromPurpsJobs.ContainsKey(cit))
			{
				this.exemptFromPurpsJobs.Add(cit, new List<SideJob>());
			}
			if (!this.exemptFromPurpsJobs[cit].Contains(job))
			{
				this.exemptFromPurpsJobs[cit].Add(job);
			}
		}
		if (!this.exemptFromPurps.Contains(cit))
		{
			this.exemptFromPurps.Add(cit);
		}
	}

	// Token: 0x060011F1 RID: 4593 RVA: 0x00100A18 File Offset: 0x000FEC18
	public void RemoveExemptFromPosters(Human cit, SideJob job)
	{
		if (cit == null)
		{
			return;
		}
		if (this.exemptFromPostersJobs.ContainsKey(cit))
		{
			this.exemptFromPostersJobs[cit].Remove(job);
			if (this.exemptFromPostersJobs[cit].Count <= 0)
			{
				this.exemptFromPosters.Remove(cit);
				return;
			}
		}
		else
		{
			this.exemptFromPosters.Remove(cit);
		}
	}

	// Token: 0x060011F2 RID: 4594 RVA: 0x00100A80 File Offset: 0x000FEC80
	public void RemoveExemptFromPurps(Human cit, SideJob job)
	{
		if (cit == null)
		{
			return;
		}
		if (this.exemptFromPurpsJobs.ContainsKey(cit))
		{
			this.exemptFromPurpsJobs[cit].Remove(job);
			if (this.exemptFromPurpsJobs[cit].Count <= 0)
			{
				this.exemptFromPurpsJobs.Remove(cit);
				return;
			}
		}
		else
		{
			this.exemptFromPurps.Remove(cit);
		}
	}

	// Token: 0x060011F3 RID: 4595 RVA: 0x00100AE8 File Offset: 0x000FECE8
	private bool MotivePass(ref List<MotivePreset.ModifierRule> rules, Citizen cit, out int score)
	{
		score = 0;
		bool result = true;
		foreach (MotivePreset.ModifierRule modifierRule in rules)
		{
			bool flag = false;
			if (modifierRule.rule == CharacterTrait.RuleType.ifAnyOfThese)
			{
				using (List<CharacterTrait>.Enumerator enumerator2 = modifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = true;
							break;
						}
					}
					goto IL_1D6;
				}
				goto IL_85;
			}
			goto IL_85;
			IL_1D6:
			if (flag)
			{
				score += modifierRule.score;
				continue;
			}
			if (modifierRule.mustPassForApplication)
			{
				score = 0;
				result = false;
				continue;
			}
			continue;
			IL_85:
			if (modifierRule.rule == CharacterTrait.RuleType.ifAllOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = modifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (!cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1D6;
				}
			}
			if (modifierRule.rule == CharacterTrait.RuleType.ifNoneOfThese)
			{
				flag = true;
				using (List<CharacterTrait>.Enumerator enumerator2 = modifierRule.traitList.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						CharacterTrait searchTrait = enumerator2.Current;
						if (cit.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
						{
							flag = false;
							break;
						}
					}
					goto IL_1D6;
				}
			}
			if (modifierRule.rule == CharacterTrait.RuleType.ifPartnerAnyOfThese)
			{
				if (cit.partner != null)
				{
					using (List<CharacterTrait>.Enumerator enumerator2 = modifierRule.traitList.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							CharacterTrait searchTrait = enumerator2.Current;
							if (cit.partner.characterTraits.Exists((Human.Trait item) => item.trait == searchTrait))
							{
								flag = true;
								break;
							}
						}
						goto IL_1D6;
					}
				}
				flag = false;
				goto IL_1D6;
			}
			goto IL_1D6;
		}
		return result;
	}

	// Token: 0x060011F4 RID: 4596 RVA: 0x00100D84 File Offset: 0x000FEF84
	public void SideJobObjectiveComplete(SideJob job, Objective objective)
	{
		this.invokedSideJob = job;
		this.invokedObjective = objective;
		Game.Log("Invoking side job method: " + objective.queueElement.entryRef, 2);
		try
		{
			base.Invoke(objective.queueElement.entryRef, 0f);
		}
		catch
		{
			Game.LogError("Could not invoke Side Job method: " + objective.queueElement.entryRef, 2);
		}
	}

	// Token: 0x060011F5 RID: 4597 RVA: 0x00100E00 File Offset: 0x000FF000
	public void CallPoster()
	{
		Game.Log("Jobs: CallPoster", 2);
	}

	// Token: 0x060011F6 RID: 4598 RVA: 0x00100E0D File Offset: 0x000FF00D
	public void CallFake()
	{
		Game.Log("Jobs: CallFake", 2);
	}

	// Token: 0x060011F7 RID: 4599 RVA: 0x00100E1A File Offset: 0x000FF01A
	public void SabotageRecoverInfo()
	{
		Game.Log("Jobs: SabotageRecoverInfo", 2);
	}

	// Token: 0x060011F8 RID: 4600 RVA: 0x00100E28 File Offset: 0x000FF028
	[Button(null, 0)]
	public void ListSpawnedItemsForJob()
	{
		SideJob sideJob = null;
		if (this.allJobsDictionary.TryGetValue(this.debugJobID, ref sideJob))
		{
			foreach (KeyValuePair<JobPreset.JobTag, Interactable> keyValuePair in sideJob.activeJobItems)
			{
				Game.Log(string.Concat(new string[]
				{
					"Job item ",
					keyValuePair.Value.GetName(),
					" (",
					keyValuePair.Value.id.ToString(),
					") exists with tag ",
					keyValuePair.Key.ToString(),
					" for job ",
					sideJob.jobID.ToString(),
					" at ",
					keyValuePair.Value.GetWorldPosition(true).ToString()
				}), 2);
			}
			if (sideJob.activeJobItems.Count <= 0)
			{
				Game.Log("No active job items have been spawned for job " + this.debugJobID.ToString(), 2);
				return;
			}
		}
		else
		{
			Game.Log("Unable to find job " + this.debugJobID.ToString(), 2);
		}
	}

	// Token: 0x040015FF RID: 5631
	public bool enableJobs = true;

	// Token: 0x04001600 RID: 5632
	[Header("Job Tracking")]
	public List<SideJobController.JobTracking> jobTracking = new List<SideJobController.JobTracking>();

	// Token: 0x04001601 RID: 5633
	[Header("Exempt Citizens")]
	public List<Human> exemptFromPosters = new List<Human>();

	// Token: 0x04001602 RID: 5634
	public List<Human> exemptFromPurps = new List<Human>();

	// Token: 0x04001603 RID: 5635
	public Dictionary<Human, List<SideJob>> exemptFromPostersJobs = new Dictionary<Human, List<SideJob>>();

	// Token: 0x04001604 RID: 5636
	public Dictionary<Human, List<SideJob>> exemptFromPurpsJobs = new Dictionary<Human, List<SideJob>>();

	// Token: 0x04001605 RID: 5637
	public Dictionary<int, SideJob> allJobsDictionary = new Dictionary<int, SideJob>();

	// Token: 0x04001606 RID: 5638
	[NonSerialized]
	public SideJob invokedSideJob;

	// Token: 0x04001607 RID: 5639
	[NonSerialized]
	public Objective invokedObjective;

	// Token: 0x04001608 RID: 5640
	[Header("Debug")]
	public int debugJobID;

	// Token: 0x04001609 RID: 5641
	private static SideJobController _instance;

	// Token: 0x02000313 RID: 787
	[Serializable]
	public class JobTracking
	{
		// Token: 0x0400160A RID: 5642
		public string name;

		// Token: 0x0400160B RID: 5643
		public JobPreset preset;

		// Token: 0x0400160C RID: 5644
		public List<SideJob> activeJobs = new List<SideJob>();

		// Token: 0x0400160D RID: 5645
		public List<SideJob> endedJobs = new List<SideJob>();

		// Token: 0x0400160E RID: 5646
		public int desiredActiveInstances;
	}

	// Token: 0x02000314 RID: 788
	[Serializable]
	public class JobPickData
	{
		// Token: 0x0400160F RID: 5647
		public MotivePreset motive;

		// Token: 0x04001610 RID: 5648
		public Citizen poster;

		// Token: 0x04001611 RID: 5649
		public Citizen purp;

		// Token: 0x04001612 RID: 5650
		public float score;
	}
}
