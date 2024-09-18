using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02000327 RID: 807
public class LightCullingController : MonoBehaviour
{
	// Token: 0x1700008E RID: 142
	// (get) Token: 0x0600123D RID: 4669 RVA: 0x0010318A File Offset: 0x0010138A
	public static LightCullingController Instance
	{
		get
		{
			return LightCullingController._instance;
		}
	}

	// Token: 0x0600123E RID: 4670 RVA: 0x00103191 File Offset: 0x00101391
	private void Awake()
	{
		if (LightCullingController._instance != null && LightCullingController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		LightCullingController._instance = this;
	}

	// Token: 0x0600123F RID: 4671 RVA: 0x001031C0 File Offset: 0x001013C0
	private void Update()
	{
		if (!SessionData.Instance.play)
		{
			return;
		}
		if (!Game.Instance.enableCustomLightCulling)
		{
			base.enabled = false;
			return;
		}
		if (this.primaryJobsActive && !this.handlePrimary.IsCompleted)
		{
			return;
		}
		if (this.secondaryJobsActive && !this.handleSecondary.IsCompleted)
		{
			return;
		}
		if (this.lightsToCheck.Count > 0 && !this.primaryJobsActive && !this.primaryJobsCompleted && !this.secondaryJobsActive && !this.secondaryJobsCompleted)
		{
			int num = this.lightsToCheckPerFrame;
			this.lightRaycastDataCollection.Clear();
			this.notCulled.Clear();
			this.lightsCheckedThisFrame.Clear();
			this.lightRaycastDataCollectionFromRadius.Clear();
			int num2 = this.lightsToCheck.Count;
			while (num > 0 && num2 > 0)
			{
				if (this.checkingCursor < this.lightsToCheck.Count)
				{
					LightController lightController = this.lightsToCheck[this.checkingCursor];
					if (lightController == null || lightController.lightComponent == null)
					{
						this.lightsToCheck.RemoveAt(this.checkingCursor);
					}
					else
					{
						if (lightController.lightComponent.type != 2)
						{
							lightController.SetCulled(false, false);
						}
						else if (lightController.interactable != null && lightController.interactable.node != null && !lightController.interactable.node.room.isVisible)
						{
							lightController.SetCulled(true, false);
						}
						else if (lightController.interactable != null && lightController.interactable.node != null && Player.Instance.currentRoom == lightController.interactable.node.room)
						{
							lightController.SetCulled(false, false);
						}
						else
						{
							Vector3 position = lightController.lightComponent.transform.position;
							float fadeDistance = lightController.hdrpLightData.fadeDistance;
							if (Vector3.Distance(position, CameraController.Instance.cam.transform.position) > fadeDistance)
							{
								lightController.SetCulled(true, false);
							}
							else
							{
								this.lightsCheckedThisFrame.Add(lightController);
								LightCullingController.LightRaycastData lightRaycastData = new LightCullingController.LightRaycastData(lightController, LightCullingController.LightRaycastData.RayType.lightToCam, position, CameraController.Instance.cam.transform.position - position, fadeDistance);
								this.lightRaycastDataCollection.Add(lightRaycastData);
								int num3 = Mathf.RoundToInt(Mathf.Lerp(this.radiusChecksPerLight.x, this.radiusChecksPerLight.y, lightController.lightComponent.range / 20f));
								for (int i = 0; i < num3; i++)
								{
									Vector3 insideUnitSphere = Random.insideUnitSphere;
									LightCullingController.LightRaycastData lightRaycastData2 = new LightCullingController.LightRaycastData(lightController, LightCullingController.LightRaycastData.RayType.lightToRadiusPoint, position, insideUnitSphere, fadeDistance);
									this.lightRaycastDataCollection.Add(lightRaycastData2);
								}
								num--;
							}
						}
						this.checkingCursor++;
					}
				}
				else
				{
					this.checkingCursor = 0;
				}
				num2--;
			}
		}
		if (this.lightRaycastDataCollection.Count > 0)
		{
			if (!this.primaryJobsActive && !this.primaryJobsCompleted)
			{
				this.resultsPrimary = new NativeArray<RaycastHit>(this.lightRaycastDataCollection.Count, 3, 1);
				this.commandsPrimary = new NativeArray<RaycastCommand>(this.lightRaycastDataCollection.Count, 3, 1);
				this.originsPrimary = new NativeArray<Vector3>(this.lightRaycastDataCollection.Count, 3, 1);
				this.directionsPrimary = new NativeArray<Vector3>(this.lightRaycastDataCollection.Count, 3, 1);
				this.rangePrimary = new NativeArray<float>(this.lightRaycastDataCollection.Count, 3, 1);
				for (int j = 0; j < this.lightRaycastDataCollection.Count; j++)
				{
					LightCullingController.LightRaycastData lightRaycastData3 = this.lightRaycastDataCollection[j];
					this.originsPrimary[j] = lightRaycastData3.originPoint;
					this.directionsPrimary[j] = lightRaycastData3.direction;
					this.rangePrimary[j] = lightRaycastData3.range;
				}
				LightCullingController.SetupCommandJob setupCommandJob = new LightCullingController.SetupCommandJob
				{
					commands = this.commandsPrimary,
					origins = this.originsPrimary,
					directions = this.directionsPrimary,
					ranges = this.rangePrimary,
					mask = Toolbox.Instance.lightCullingMask
				};
				this.handlePrimary = IJobParallelForExtensions.Schedule<LightCullingController.SetupCommandJob>(setupCommandJob, this.commandsPrimary.Length, 1, default(JobHandle));
				this.handlePrimary = RaycastCommand.ScheduleBatch(this.commandsPrimary, this.resultsPrimary, 1, this.handlePrimary);
				this.handlePrimary.Complete();
				this.primaryJobsActive = true;
				this.primaryJobsCompleted = false;
				return;
			}
			if (this.primaryJobsActive && !this.primaryJobsCompleted && this.handlePrimary.IsCompleted)
			{
				this.primaryJobsActive = false;
				this.primaryJobsCompleted = true;
				this.lightRaycastDataCollectionFromRadius.Clear();
				for (int k = 0; k < this.resultsPrimary.Length; k++)
				{
					RaycastHit raycastHit = this.resultsPrimary[k];
					LightCullingController.LightRaycastData lightRaycastData4 = this.lightRaycastDataCollection[k];
					if (!this.notCulled.Contains(lightRaycastData4.lightRef))
					{
						if (lightRaycastData4.rayType == LightCullingController.LightRaycastData.RayType.lightToCam)
						{
							if (raycastHit.collider == null || raycastHit.collider.transform.CompareTag("MainCamera") || raycastHit.collider.transform.CompareTag("Player") || (raycastHit.collider.transform.parent != null && raycastHit.collider.transform.parent.CompareTag("Player")))
							{
								this.notCulled.Add(lightRaycastData4.lightRef);
								lightRaycastData4.lightRef.SetCulled(false, true);
							}
						}
						else if (lightRaycastData4.rayType == LightCullingController.LightRaycastData.RayType.lightToRadiusPoint && raycastHit.collider != null)
						{
							Vector3 position2 = CameraController.Instance.cam.transform.position;
							LightCullingController.LightRaycastData lightRaycastData5 = new LightCullingController.LightRaycastData(lightRaycastData4.lightRef, LightCullingController.LightRaycastData.RayType.radiusPointToCam, raycastHit.point, position2 - raycastHit.point, lightRaycastData4.lightRef.lightComponent.range + lightRaycastData4.lightRef.hdrpLightData.fadeDistance);
							this.lightRaycastDataCollectionFromRadius.Add(lightRaycastData5);
						}
					}
				}
				this.resultsPrimary.Dispose();
				this.commandsPrimary.Dispose();
				this.originsPrimary.Dispose();
				this.directionsPrimary.Dispose();
				this.rangePrimary.Dispose();
				if (this.lightRaycastDataCollectionFromRadius.Count > 0)
				{
					this.resultsSecondary = new NativeArray<RaycastHit>(this.lightRaycastDataCollectionFromRadius.Count, 3, 1);
					this.commandsSecondary = new NativeArray<RaycastCommand>(this.lightRaycastDataCollectionFromRadius.Count, 3, 1);
					this.originsSecondary = new NativeArray<Vector3>(this.lightRaycastDataCollectionFromRadius.Count, 3, 1);
					this.directionsSecondary = new NativeArray<Vector3>(this.lightRaycastDataCollectionFromRadius.Count, 3, 1);
					this.rangeSecondary = new NativeArray<float>(this.lightRaycastDataCollectionFromRadius.Count, 3, 1);
					for (int l = 0; l < this.lightRaycastDataCollectionFromRadius.Count; l++)
					{
						LightCullingController.LightRaycastData lightRaycastData6 = this.lightRaycastDataCollectionFromRadius[l];
						this.originsSecondary[l] = lightRaycastData6.originPoint;
						this.directionsSecondary[l] = lightRaycastData6.direction;
						this.rangeSecondary[l] = lightRaycastData6.range;
					}
					LightCullingController.SetupCommandJob setupCommandJob2 = new LightCullingController.SetupCommandJob
					{
						commands = this.commandsSecondary,
						origins = this.originsSecondary,
						directions = this.directionsSecondary,
						ranges = this.rangeSecondary,
						mask = Toolbox.Instance.lightCullingMask
					};
					this.handleSecondary = IJobParallelForExtensions.Schedule<LightCullingController.SetupCommandJob>(setupCommandJob2, this.commandsSecondary.Length, 1, default(JobHandle));
					this.handleSecondary = RaycastCommand.ScheduleBatch(this.commandsSecondary, this.resultsSecondary, 1, this.handleSecondary);
					this.handleSecondary.Complete();
					this.secondaryJobsActive = true;
					this.secondaryJobsCompleted = false;
					return;
				}
				this.secondaryJobsCompleted = true;
				return;
			}
			else if (this.secondaryJobsActive && !this.secondaryJobsCompleted && this.handleSecondary.IsCompleted)
			{
				this.secondaryJobsActive = false;
				this.secondaryJobsCompleted = true;
				for (int m = 0; m < this.resultsSecondary.Length; m++)
				{
					RaycastHit raycastHit2 = this.resultsSecondary[m];
					LightCullingController.LightRaycastData lightRaycastData7 = this.lightRaycastDataCollectionFromRadius[m];
					if (!this.notCulled.Contains(lightRaycastData7.lightRef) && (raycastHit2.collider == null || raycastHit2.collider.transform.CompareTag("MainCamera") || raycastHit2.collider.transform.CompareTag("Player") || (raycastHit2.collider.transform.parent != null && raycastHit2.collider.transform.parent.CompareTag("Player"))))
					{
						this.notCulled.Add(lightRaycastData7.lightRef);
						lightRaycastData7.lightRef.SetCulled(false, true);
					}
				}
				this.resultsSecondary.Dispose();
				this.commandsSecondary.Dispose();
				this.originsSecondary.Dispose();
				this.directionsSecondary.Dispose();
				this.rangeSecondary.Dispose();
				return;
			}
		}
		else
		{
			this.primaryJobsCompleted = true;
			this.primaryJobsActive = false;
			this.secondaryJobsCompleted = true;
			this.secondaryJobsActive = false;
		}
		if (!this.primaryJobsActive && !this.secondaryJobsActive && this.primaryJobsCompleted && this.secondaryJobsCompleted)
		{
			foreach (LightController lightController2 in this.lightsCheckedThisFrame)
			{
				if (this.notCulled.Contains(lightController2))
				{
					lightController2.SetCulled(false, true);
				}
				else
				{
					lightController2.SetCulled(true, true);
				}
			}
			this.primaryJobsActive = false;
			this.primaryJobsCompleted = false;
			this.secondaryJobsActive = false;
			this.secondaryJobsCompleted = false;
		}
	}

	// Token: 0x0400164A RID: 5706
	[Header("Settings")]
	public int lightsToCheckPerFrame = 10;

	// Token: 0x0400164B RID: 5707
	[Tooltip("This is lerped depending on the range of the light. A range of 20 represents the maximum value")]
	public Vector2 radiusChecksPerLight = new Vector2(1f, 12f);

	// Token: 0x0400164C RID: 5708
	[Tooltip("When NOT culled, lights are active for a minimum time to avoid a flickering effect due to frequent checking. This value is in GAMETIME")]
	public float minimumLightUnculledTime = 0.01f;

	// Token: 0x0400164D RID: 5709
	public List<LightController> lightsToCheck = new List<LightController>();

	// Token: 0x0400164E RID: 5710
	public int checkingCursor;

	// Token: 0x0400164F RID: 5711
	private List<LightController> lightsCheckedThisFrame = new List<LightController>();

	// Token: 0x04001650 RID: 5712
	public List<LightController> culledLights = new List<LightController>();

	// Token: 0x04001651 RID: 5713
	private JobHandle handlePrimary;

	// Token: 0x04001652 RID: 5714
	private NativeArray<RaycastHit> resultsPrimary;

	// Token: 0x04001653 RID: 5715
	private NativeArray<RaycastCommand> commandsPrimary;

	// Token: 0x04001654 RID: 5716
	private NativeArray<Vector3> originsPrimary;

	// Token: 0x04001655 RID: 5717
	private NativeArray<Vector3> directionsPrimary;

	// Token: 0x04001656 RID: 5718
	private NativeArray<float> rangePrimary;

	// Token: 0x04001657 RID: 5719
	private bool primaryJobsActive;

	// Token: 0x04001658 RID: 5720
	private bool primaryJobsCompleted;

	// Token: 0x04001659 RID: 5721
	private JobHandle handleSecondary;

	// Token: 0x0400165A RID: 5722
	private NativeArray<RaycastHit> resultsSecondary;

	// Token: 0x0400165B RID: 5723
	private NativeArray<RaycastCommand> commandsSecondary;

	// Token: 0x0400165C RID: 5724
	private NativeArray<Vector3> originsSecondary;

	// Token: 0x0400165D RID: 5725
	private NativeArray<Vector3> directionsSecondary;

	// Token: 0x0400165E RID: 5726
	private NativeArray<float> rangeSecondary;

	// Token: 0x0400165F RID: 5727
	private bool secondaryJobsActive;

	// Token: 0x04001660 RID: 5728
	private bool secondaryJobsCompleted;

	// Token: 0x04001661 RID: 5729
	private List<LightController> notCulled = new List<LightController>();

	// Token: 0x04001662 RID: 5730
	private List<LightCullingController.LightRaycastData> lightRaycastDataCollectionFromRadius = new List<LightCullingController.LightRaycastData>();

	// Token: 0x04001663 RID: 5731
	private List<LightCullingController.LightRaycastData> lightRaycastDataCollection = new List<LightCullingController.LightRaycastData>();

	// Token: 0x04001664 RID: 5732
	private static LightCullingController _instance;

	// Token: 0x02000328 RID: 808
	private struct LightRaycastData
	{
		// Token: 0x06001241 RID: 4673 RVA: 0x00103C81 File Offset: 0x00101E81
		public LightRaycastData(LightController newLightRef, LightCullingController.LightRaycastData.RayType newRayType, Vector3 newOriginPoint, Vector3 newDir, float newRange)
		{
			this.lightRef = newLightRef;
			this.rayType = newRayType;
			this.originPoint = newOriginPoint;
			this.direction = newDir;
			this.range = newRange;
		}

		// Token: 0x04001665 RID: 5733
		public LightCullingController.LightRaycastData.RayType rayType;

		// Token: 0x04001666 RID: 5734
		public LightController lightRef;

		// Token: 0x04001667 RID: 5735
		public Vector3 originPoint;

		// Token: 0x04001668 RID: 5736
		public Vector3 direction;

		// Token: 0x04001669 RID: 5737
		public float range;

		// Token: 0x02000329 RID: 809
		public enum RayType
		{
			// Token: 0x0400166B RID: 5739
			lightToCam,
			// Token: 0x0400166C RID: 5740
			lightToFeet,
			// Token: 0x0400166D RID: 5741
			lightToRadiusPoint,
			// Token: 0x0400166E RID: 5742
			radiusPointToCam
		}
	}

	// Token: 0x0200032A RID: 810
	[BurstCompile]
	private struct SetupCommandJob : IJobParallelFor
	{
		// Token: 0x06001242 RID: 4674 RVA: 0x00103CA8 File Offset: 0x00101EA8
		public void Execute(int index)
		{
			this.commands[index] = new RaycastCommand(this.origins[index], this.directions[index], this.ranges[index], this.mask, 1);
		}

		// Token: 0x0400166F RID: 5743
		public NativeArray<RaycastCommand> commands;

		// Token: 0x04001670 RID: 5744
		[ReadOnly]
		public NativeArray<Vector3> directions;

		// Token: 0x04001671 RID: 5745
		[ReadOnly]
		public NativeArray<Vector3> origins;

		// Token: 0x04001672 RID: 5746
		[ReadOnly]
		public NativeArray<float> ranges;

		// Token: 0x04001673 RID: 5747
		public int mask;
	}
}
