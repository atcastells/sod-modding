using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x0200028C RID: 652
public class CleanupController : MonoBehaviour
{
	// Token: 0x17000073 RID: 115
	// (get) Token: 0x06000EA6 RID: 3750 RVA: 0x000D3F40 File Offset: 0x000D2140
	public static CleanupController Instance
	{
		get
		{
			return CleanupController._instance;
		}
	}

	// Token: 0x06000EA7 RID: 3751 RVA: 0x000D3F47 File Offset: 0x000D2147
	private void Awake()
	{
		if (CleanupController._instance != null && CleanupController._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		CleanupController._instance = this;
	}

	// Token: 0x06000EA8 RID: 3752 RVA: 0x000D3F78 File Offset: 0x000D2178
	public void TrashUpdate()
	{
		if (this.trash.Count > GameplayControls.Instance.globalCreatedTrashLimit)
		{
			List<Interactable> list = new List<Interactable>();
			for (int i = 0; i < this.trash.Count; i++)
			{
				Interactable interactable = this.trash[i];
				if (interactable.IsSafeToDelete() && !PlayerApartmentController.Instance.itemStorage.Contains(interactable) && (interactable.node == null || interactable.node.gameLocation == null || interactable.node.gameLocation.thisAsAddress == null || (!Player.Instance.apartmentsOwned.Contains(interactable.node.gameLocation.thisAsAddress) && interactable.node.gameLocation != Player.Instance.home)))
				{
					list.Add(interactable);
				}
				if (this.trash.Count - list.Count <= GameplayControls.Instance.globalCreatedTrashLimit)
				{
					break;
				}
			}
			this.trashRemovedLastUpdate = list.Count;
			foreach (Interactable interactable2 in list)
			{
				interactable2.Delete();
			}
			this.currentTrash = this.trash.Count;
		}
	}

	// Token: 0x06000EA9 RID: 3753 RVA: 0x000D40DC File Offset: 0x000D22DC
	[Button(null, 0)]
	public void UpdateData()
	{
		this.totalInteractables = 0;
		this.savableCount = 0;
		this.trashCount = 0;
		this.trashThreshold = GameplayControls.Instance.globalCreatedTrashLimit;
		this.removedCityDataInteractables = this.removedCityDataItems.Count;
		Dictionary<InteractablePreset, List<Interactable>> dictionary = new Dictionary<InteractablePreset, List<Interactable>>();
		Dictionary<InteractablePreset, List<Interactable>> dictionary2 = new Dictionary<InteractablePreset, List<Interactable>>();
		Dictionary<InteractablePreset, List<Interactable>> dictionary3 = new Dictionary<InteractablePreset, List<Interactable>>();
		foreach (Interactable interactable in CityData.Instance.interactableDirectory)
		{
			if (interactable.IsSaveStateEligable())
			{
				if (!dictionary.ContainsKey(interactable.preset))
				{
					dictionary.Add(interactable.preset, new List<Interactable>());
				}
				dictionary[interactable.preset].Add(interactable);
				this.savableCount++;
			}
			else
			{
				if (!dictionary2.ContainsKey(interactable.preset))
				{
					dictionary2.Add(interactable.preset, new List<Interactable>());
				}
				dictionary2[interactable.preset].Add(interactable);
			}
			if (this.trash.Contains(interactable))
			{
				if (!dictionary3.ContainsKey(interactable.preset))
				{
					dictionary3.Add(interactable.preset, new List<Interactable>());
				}
				dictionary3[interactable.preset].Add(interactable);
				this.trashCount++;
			}
			this.totalInteractables++;
		}
		this.savablePercent = Mathf.RoundToInt((float)this.savableCount / (float)this.totalInteractables * 100f);
		this.trashPercent = Mathf.RoundToInt((float)this.trashCount / (float)this.totalInteractables * 100f);
		this.trashThresholdPercent = Mathf.RoundToInt((float)this.trashCount / (float)this.trashThreshold * 100f);
		this.breakdownSavable.Clear();
		this.breakdownNonSavable.Clear();
		this.breakdownTrash.Clear();
		foreach (InteractablePreset interactablePreset in new List<InteractablePreset>(dictionary.Keys))
		{
			CleanupController.DebugInteractable debugInteractable = new CleanupController.DebugInteractable();
			debugInteractable.count = dictionary[interactablePreset].Count;
			debugInteractable.name = interactablePreset.name + " (" + debugInteractable.count.ToString() + ")";
			int num = 0;
			int num2 = 0;
			foreach (Interactable interactable2 in dictionary[interactablePreset])
			{
				if (interactable2.IsSaveStateEligable())
				{
					num++;
					string reason = interactable2.GetReasonForSaveStateEligable();
					CleanupController.SaveableBecause saveableBecause = debugInteractable.savableDetails.Find((CleanupController.SaveableBecause item) => item.reason == reason);
					if (saveableBecause != null)
					{
						saveableBecause.count++;
					}
					else
					{
						debugInteractable.savableDetails.Add(new CleanupController.SaveableBecause
						{
							reason = reason,
							count = 1
						});
					}
				}
				if (this.trash.Contains(interactable2))
				{
					num2++;
				}
			}
			debugInteractable.savableDetails.Sort((CleanupController.SaveableBecause p2, CleanupController.SaveableBecause p1) => p1.count.CompareTo(p2.count));
			debugInteractable.savablePercent = Mathf.RoundToInt((float)num / (float)debugInteractable.count * 100f);
			debugInteractable.trashPercent = Mathf.RoundToInt((float)num2 / (float)debugInteractable.count * 100f);
			this.breakdownSavable.Add(debugInteractable);
		}
		this.breakdownSavable.Sort((CleanupController.DebugInteractable p2, CleanupController.DebugInteractable p1) => p1.count.CompareTo(p2.count));
		foreach (InteractablePreset interactablePreset2 in new List<InteractablePreset>(dictionary2.Keys))
		{
			CleanupController.DebugInteractable debugInteractable2 = new CleanupController.DebugInteractable();
			debugInteractable2.count = dictionary2[interactablePreset2].Count;
			debugInteractable2.name = interactablePreset2.name + " (" + debugInteractable2.count.ToString() + ")";
			int num3 = 0;
			int num4 = 0;
			foreach (Interactable interactable3 in dictionary2[interactablePreset2])
			{
				if (interactable3.IsSaveStateEligable())
				{
					num3++;
				}
				if (this.trash.Contains(interactable3))
				{
					num4++;
				}
			}
			debugInteractable2.savablePercent = Mathf.RoundToInt((float)num3 / (float)debugInteractable2.count * 100f);
			debugInteractable2.trashPercent = Mathf.RoundToInt((float)num4 / (float)debugInteractable2.count * 100f);
			this.breakdownNonSavable.Add(debugInteractable2);
		}
		this.breakdownNonSavable.Sort((CleanupController.DebugInteractable p2, CleanupController.DebugInteractable p1) => p1.count.CompareTo(p2.count));
		foreach (InteractablePreset interactablePreset3 in new List<InteractablePreset>(dictionary3.Keys))
		{
			CleanupController.DebugInteractable debugInteractable3 = new CleanupController.DebugInteractable();
			debugInteractable3.count = dictionary3[interactablePreset3].Count;
			debugInteractable3.name = interactablePreset3.name + " (" + debugInteractable3.count.ToString() + ")";
			int num5 = 0;
			int num6 = 0;
			foreach (Interactable interactable4 in dictionary3[interactablePreset3])
			{
				if (interactable4.IsSaveStateEligable())
				{
					num5++;
				}
				if (this.trash.Contains(interactable4))
				{
					num6++;
				}
			}
			debugInteractable3.savablePercent = Mathf.RoundToInt((float)num5 / (float)debugInteractable3.count * 100f);
			debugInteractable3.trashPercent = Mathf.RoundToInt((float)num6 / (float)debugInteractable3.count * 100f);
			this.breakdownTrash.Add(debugInteractable3);
		}
		this.breakdownTrash.Sort((CleanupController.DebugInteractable p2, CleanupController.DebugInteractable p1) => p1.count.CompareTo(p2.count));
	}

	// Token: 0x040011BE RID: 4542
	[ReadOnly]
	[Header("Interactables")]
	[InfoBox("Breakdown of active interactables", 0)]
	public int totalInteractables;

	// Token: 0x040011BF RID: 4543
	[ReadOnly]
	public int removedCityDataInteractables;

	// Token: 0x040011C0 RID: 4544
	[NonSerialized]
	public List<int> removedCityDataItems = new List<int>();

	// Token: 0x040011C1 RID: 4545
	[ReadOnly]
	public int savableCount;

	// Token: 0x040011C2 RID: 4546
	[ProgressBar("Savable %", 100f, 8)]
	public int savablePercent;

	// Token: 0x040011C3 RID: 4547
	[ReadOnly]
	public int trashCount;

	// Token: 0x040011C4 RID: 4548
	[ProgressBar("Trash %", 100f, 9)]
	public int trashPercent;

	// Token: 0x040011C5 RID: 4549
	[ReadOnly]
	public int trashThreshold;

	// Token: 0x040011C6 RID: 4550
	[ProgressBar("Trash Threshold %", 100f, 4)]
	public int trashThresholdPercent;

	// Token: 0x040011C7 RID: 4551
	[ReadOnly]
	public int trashRemovedLastUpdate;

	// Token: 0x040011C8 RID: 4552
	[Space(7f)]
	public List<CleanupController.DebugInteractable> breakdownSavable = new List<CleanupController.DebugInteractable>();

	// Token: 0x040011C9 RID: 4553
	public List<CleanupController.DebugInteractable> breakdownNonSavable = new List<CleanupController.DebugInteractable>();

	// Token: 0x040011CA RID: 4554
	public List<CleanupController.DebugInteractable> breakdownTrash = new List<CleanupController.DebugInteractable>();

	// Token: 0x040011CB RID: 4555
	[ReadOnly]
	[Header("Trash")]
	public int currentTrash;

	// Token: 0x040011CC RID: 4556
	[NonSerialized]
	public List<Interactable> trash = new List<Interactable>();

	// Token: 0x040011CD RID: 4557
	[ReadOnly]
	public int binTrash;

	// Token: 0x040011CE RID: 4558
	private static CleanupController _instance;

	// Token: 0x0200028D RID: 653
	[Serializable]
	public class DebugInteractable
	{
		// Token: 0x040011CF RID: 4559
		public string name;

		// Token: 0x040011D0 RID: 4560
		public int count;

		// Token: 0x040011D1 RID: 4561
		[ProgressBar("Savable", 100f, 8)]
		public int savablePercent;

		// Token: 0x040011D2 RID: 4562
		[ProgressBar("Trash", 100f, 9)]
		public int trashPercent;

		// Token: 0x040011D3 RID: 4563
		[Space(7f)]
		public List<CleanupController.SaveableBecause> savableDetails = new List<CleanupController.SaveableBecause>();
	}

	// Token: 0x0200028E RID: 654
	[Serializable]
	public class SaveableBecause
	{
		// Token: 0x040011D4 RID: 4564
		public string reason;

		// Token: 0x040011D5 RID: 4565
		public int count;
	}
}
