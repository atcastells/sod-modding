using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

// Token: 0x02000403 RID: 1027
public class InteractableCreator : MonoBehaviour
{
	// Token: 0x170000A5 RID: 165
	// (get) Token: 0x0600177B RID: 6011 RVA: 0x0016128B File Offset: 0x0015F48B
	public static InteractableCreator Instance
	{
		get
		{
			return InteractableCreator._instance;
		}
	}

	// Token: 0x0600177C RID: 6012 RVA: 0x00161292 File Offset: 0x0015F492
	private void Awake()
	{
		if (InteractableCreator._instance != null && InteractableCreator._instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		InteractableCreator._instance = this;
	}

	// Token: 0x0600177D RID: 6013 RVA: 0x001612C0 File Offset: 0x0015F4C0
	public Interactable CreateCitizenInteractable(InteractablePreset preset, Human citizen, Transform trans, Evidence evidence)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = false;
		interactable.parentTransform = trans;
		interactable.evidence = evidence;
		interactable.SetSpawnPositionRelevent(false);
		interactable.SetPolymorphicReference(citizen);
		interactable.isActor = citizen;
		interactable.node = citizen.currentNode;
		if (citizen.speechController != null && preset == CitizenControls.Instance.citizenInteractable)
		{
			interactable.speechController = citizen.speechController;
			interactable.speechController.interactable = interactable;
			interactable.speechController.actor = citizen;
		}
		interactable.SetOwner(citizen, true);
		interactable.SetWriter(citizen);
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x0600177E RID: 6014 RVA: 0x0016136C File Offset: 0x0015F56C
	public Interactable CreateTransformInteractable(InteractablePreset preset, Transform trans, Human belongsTo, Evidence evidence, Vector3 localPos, Vector3 localEuler, List<Interactable.Passed> passedVars)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = false;
		interactable.parentTransform = trans;
		interactable.evidence = evidence;
		interactable.SetSpawnPositionRelevent(false);
		interactable.lPos = localPos;
		interactable.lEuler = localEuler;
		interactable.pv = passedVars;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x0600177F RID: 6015 RVA: 0x001613D0 File Offset: 0x0015F5D0
	public Interactable CreateFurnitureIntegratedInteractable(InteractablePreset preset, NewRoom room, FurnitureLocation furniture, Human belongsTo, Human writer, Human recevier, Vector3 localPos, Vector3 localEuler, InteractableController.InteractableID pairTo, FurniturePreset.SubObjectOwnership pairToOwner, LightingPreset isLight, List<Interactable.Passed> passedVars)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = false;
		interactable.AssignFurnitureBasedID(furniture);
		interactable.furnitureParent = furniture;
		interactable.fp = furniture.id;
		interactable.SetSpawnPositionRelevent(false);
		interactable.lPos = localPos;
		interactable.lEuler = localEuler;
		interactable.pv = passedVars;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(writer);
		interactable.SetReciever(recevier);
		interactable.pt = (int)pairTo;
		interactable.pto = (int)pairToOwner;
		interactable.MainSetupStart();
		interactable.OnCreate();
		if (isLight != null)
		{
			interactable.SetAsLight(isLight, -1, preset.isMainLight, null);
			interactable.lp = isLight.name;
			interactable.ml = preset.isMainLight;
		}
		return interactable;
	}

	// Token: 0x06001780 RID: 6016 RVA: 0x00161490 File Offset: 0x0015F690
	public Interactable CreateFurnitureSpawnedInteractableThreadSafe(InteractablePreset preset, NewRoom room, FurnitureLocation furniture, FurniturePreset.SubObject subObject, Human belongsTo, Human writer, Human recevier, List<Interactable.Passed> passedVars, LightingPreset isLight, object passedObject, string ddsOverride = "")
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignRoomBasedID(room);
		interactable.SetSpawnPositionRelevent(true);
		interactable.furnitureParent = furniture;
		interactable.fp = furniture.id;
		interactable.subObject = subObject;
		interactable.fsoi = furniture.furniture.subObjects.IndexOf(subObject);
		interactable.lPos = subObject.localPos;
		interactable.lEuler = subObject.localRot;
		if (ddsOverride.Length > 0)
		{
			if (passedVars == null)
			{
				passedVars = new List<Interactable.Passed>();
			}
			passedVars.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, ddsOverride));
		}
		interactable.pv = passedVars;
		if (passedObject != null)
		{
			interactable.syncDisk = (passedObject as SyncDiskPreset);
			if (interactable.syncDisk != null)
			{
				interactable.sd = interactable.syncDisk.name;
			}
			interactable.book = (passedObject as BookPreset);
			if (interactable.book != null)
			{
				interactable.bo = interactable.book.name;
			}
		}
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(writer);
		interactable.SetReciever(recevier);
		interactable.MainSetupStart();
		interactable.OnCreate();
		if (isLight != null)
		{
			interactable.SetAsLight(isLight, -1, preset.isMainLight, null);
			interactable.lp = isLight.name;
			interactable.ml = false;
		}
		return interactable;
	}

	// Token: 0x06001781 RID: 6017 RVA: 0x001615E8 File Offset: 0x0015F7E8
	public Interactable CreateFurnitureSpawnedInteractable(InteractablePreset preset, FurnitureLocation furniture, FurniturePreset.SubObject subObject, Human belongsTo, Human writer, Human recevier, List<Interactable.Passed> passedVars, LightingPreset isLight, object passedObject, string ddsOverride = "")
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignIDWorld();
		interactable.SetSpawnPositionRelevent(true);
		interactable.furnitureParent = furniture;
		interactable.fp = furniture.id;
		interactable.subObject = subObject;
		interactable.fsoi = furniture.furniture.subObjects.IndexOf(subObject);
		interactable.lPos = subObject.localPos;
		interactable.lEuler = subObject.localRot;
		if (ddsOverride.Length > 0)
		{
			if (passedVars == null)
			{
				passedVars = new List<Interactable.Passed>();
			}
			passedVars.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, ddsOverride));
		}
		interactable.pv = passedVars;
		if (passedObject != null)
		{
			interactable.syncDisk = (passedObject as SyncDiskPreset);
			if (interactable.syncDisk != null)
			{
				interactable.sd = interactable.syncDisk.name;
			}
			interactable.book = (passedObject as BookPreset);
			if (interactable.book != null)
			{
				interactable.bo = interactable.book.name;
			}
		}
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(writer);
		interactable.SetReciever(recevier);
		interactable.MainSetupStart();
		interactable.OnCreate();
		if (isLight != null)
		{
			interactable.SetAsLight(isLight, -1, preset.isMainLight, null);
			interactable.lp = isLight.name;
			interactable.ml = false;
		}
		return interactable;
	}

	// Token: 0x06001782 RID: 6018 RVA: 0x0016173C File Offset: 0x0015F93C
	public Interactable CreateWorldInteractable(InteractablePreset preset, Human belongsTo, Human writer, Human recevier, Vector3 worldPos, Vector3 worldEuler, List<Interactable.Passed> passedVars, object passedObject, string ddsOverride = "")
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignIDWorld();
		interactable.SetSpawnPositionRelevent(false);
		interactable.wo = true;
		interactable.lPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.wPos = worldPos;
		interactable.lEuler = worldEuler;
		if (ddsOverride.Length > 0)
		{
			if (passedVars == null)
			{
				passedVars = new List<Interactable.Passed>();
			}
			passedVars.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, ddsOverride));
		}
		interactable.pv = passedVars;
		if (passedObject != null)
		{
			interactable.syncDisk = (passedObject as SyncDiskPreset);
			if (interactable.syncDisk != null)
			{
				interactable.sd = interactable.syncDisk.name;
			}
			interactable.book = (passedObject as BookPreset);
			if (interactable.book != null)
			{
				interactable.bo = interactable.book.name;
			}
		}
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(writer);
		interactable.SetReciever(recevier);
		interactable.MainSetupStart();
		interactable.OnCreate();
		if (preset != null && preset.isLight != null)
		{
			Game.Log("Object: Setting world object as light", 2);
			interactable.SetAsLight(preset.isLight, -1, preset.isMainLight, null);
			interactable.lp = preset.isLight.name;
			interactable.ml = preset.isMainLight;
		}
		return interactable;
	}

	// Token: 0x06001783 RID: 6019 RVA: 0x00161890 File Offset: 0x0015FA90
	public Interactable CreateWorldInteractableFromMetaObject(MetaObject meta, InteractablePreset preset, Vector3 worldPos, Vector3 worldEuler)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.id = meta.id;
		interactable.SetSpawnPositionRelevent(false);
		interactable.wo = true;
		interactable.lPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.wPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.pv = new List<Interactable.Passed>(meta.passed);
		if (meta.owner > -1)
		{
			Human newOwner = null;
			if (CityData.Instance.GetHuman(meta.owner, out newOwner, true))
			{
				interactable.SetOwner(newOwner, true);
			}
		}
		if (meta.writer > -1)
		{
			Human writer = null;
			if (CityData.Instance.GetHuman(meta.writer, out writer, true))
			{
				interactable.SetWriter(writer);
			}
		}
		if (meta.reciever > -1)
		{
			Human reciever = null;
			if (CityData.Instance.GetHuman(meta.reciever, out reciever, true))
			{
				interactable.SetReciever(reciever);
			}
		}
		interactable.evidence = meta.GetEvidence(true, CityData.Instance.RealPosToNodeInt(worldPos));
		interactable.MainSetupStart();
		interactable.OnCreate();
		meta.Remove();
		return interactable;
	}

	// Token: 0x06001784 RID: 6020 RVA: 0x00161998 File Offset: 0x0015FB98
	public Interactable CreateDoorParentedInteractable(InteractablePreset preset, NewDoor door, Human belongsTo, Vector3 localPos, Vector3 localEuler, List<Interactable.Passed> passedVars, string ddsOverride = "")
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignIDWorld();
		interactable.SetSpawnPositionRelevent(false);
		interactable.parentTransform = door.transform;
		interactable.dp = door.wall.id;
		interactable.lPos = localPos;
		interactable.lEuler = localEuler;
		if (ddsOverride.Length > 0)
		{
			if (passedVars == null)
			{
				passedVars = new List<Interactable.Passed>();
			}
			passedVars.Add(new Interactable.Passed(Interactable.PassedVarType.ddsOverride, 0f, ddsOverride));
		}
		interactable.pv = passedVars;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x06001785 RID: 6021 RVA: 0x00161A3C File Offset: 0x0015FC3C
	public Interactable CreateMainLightInteractable(InteractablePreset preset, NewRoom room, Vector3 worldPos, Vector3 worldEuler, LightingPreset lightPreset, Interactable.LightConfiguration preconfiguredLight, int lightZoneSize = -1)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignRoomBasedID(room);
		interactable.SetSpawnPositionRelevent(false);
		interactable.wo = true;
		interactable.lPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.wPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.MainSetupStart();
		interactable.OnCreate();
		interactable.SetAsLight(lightPreset, lightZoneSize, true, preconfiguredLight);
		interactable.lp = lightPreset.presetName;
		interactable.ml = true;
		interactable.lzs = lightZoneSize;
		return interactable;
	}

	// Token: 0x06001786 RID: 6022 RVA: 0x00161AC0 File Offset: 0x0015FCC0
	public Interactable CreateBookInteractable(InteractablePreset preset, NewRoom room, FurnitureLocation furniture, Human belongsTo, Vector3 localPos, Vector3 localEuler, BookPreset book)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = true;
		interactable.AssignRoomBasedID(room);
		interactable.SetSpawnPositionRelevent(false);
		interactable.furnitureParent = furniture;
		interactable.fp = furniture.id;
		interactable.lPos = localPos;
		interactable.lEuler = localEuler;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.book = book;
		interactable.bo = book.name;
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x06001787 RID: 6023 RVA: 0x00161B40 File Offset: 0x0015FD40
	public Interactable CreateFingerprintInteractable(Human belongsTo, Vector3 worldPos, Vector3 worldEuler, FingerprintScannerController.Print print)
	{
		Game.Log("Player: Create new fingerprint interactable...", 2);
		Interactable interactable = new Interactable(GameplayControls.Instance.fignerprintPreset);
		interactable.save = false;
		interactable.AssignIDWorld();
		interactable.SetSpawnPositionRelevent(false);
		interactable.print.Clear();
		interactable.print.Add(new Interactable.SavedPrint
		{
			worldPos = print.worldPos
		});
		if (print.interactable != null)
		{
			interactable.print[0].interactableID = print.interactable.id;
		}
		interactable.wo = true;
		interactable.lPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.wPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x06001788 RID: 6024 RVA: 0x00161C0C File Offset: 0x0015FE0C
	public Interactable CreateFootprintInteractable(Human belongsTo, Vector3 worldPos, Vector3 worldEuler, GameplayController.Footprint print)
	{
		Game.Log("Player: Create new fingerprint interactable...", 2);
		Interactable interactable = new Interactable(GameplayControls.Instance.footprintPreset);
		interactable.save = false;
		interactable.AssignIDWorld();
		interactable.SetSpawnPositionRelevent(false);
		interactable.print.Clear();
		interactable.print.Add(new Interactable.SavedPrint
		{
			worldPos = print.wP
		});
		interactable.wo = true;
		interactable.lPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.wPos = worldPos;
		interactable.lEuler = worldEuler;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x06001789 RID: 6025 RVA: 0x00161CB0 File Offset: 0x0015FEB0
	public Interactable CreateInteractableLock(InteractablePreset preset, FurnitureLocation furniture, Human belongsTo, Vector3 localPos, Vector3 localEuler, InteractableController.InteractableID pairTo)
	{
		Interactable interactable = new Interactable(preset);
		interactable.save = false;
		interactable.furnitureParent = furniture;
		interactable.fp = furniture.id;
		interactable.SetSpawnPositionRelevent(false);
		interactable.lPos = localPos;
		interactable.lEuler = localEuler;
		interactable.SetOwner(belongsTo, true);
		interactable.SetWriter(belongsTo);
		interactable.pt = (int)pairTo;
		interactable.MainSetupStart();
		interactable.OnCreate();
		return interactable;
	}

	// Token: 0x0600178A RID: 6026 RVA: 0x00161D18 File Offset: 0x0015FF18
	[Button(null, 0)]
	public void FindInteractable()
	{
		Interactable interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.debugFindID);
		if (interactable != null)
		{
			string[] array = new string[8];
			array[0] = "Found item ";
			array[1] = interactable.id.ToString();
			array[2] = ": ";
			array[3] = interactable.name;
			array[4] = " at ";
			int num = 5;
			Vector3 wPos = interactable.wPos;
			array[num] = wPos.ToString();
			array[6] = " spawned: ";
			int num2 = 7;
			InteractableController controller = interactable.controller;
			array[num2] = ((controller != null) ? controller.ToString() : null);
			Game.Log(string.Concat(array), 2);
			if (interactable.node != null)
			{
				Game.Log("Location: " + interactable.node.room.GetName() + " at " + interactable.node.gameLocation.name, 2);
				return;
			}
		}
		else
		{
			Game.Log("Unable to find item " + this.debugFindID.ToString(), 2);
		}
	}

	// Token: 0x0600178B RID: 6027 RVA: 0x00161E14 File Offset: 0x00160014
	[Button(null, 0)]
	public void ForceSpawnCheck()
	{
		Interactable interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.debugFindID);
		if (interactable != null)
		{
			string[] array = new string[8];
			array[0] = "Found item ";
			array[1] = interactable.id.ToString();
			array[2] = ": ";
			array[3] = interactable.name;
			array[4] = " at ";
			int num = 5;
			Vector3 wPos = interactable.wPos;
			array[num] = wPos.ToString();
			array[6] = " spawned: ";
			int num2 = 7;
			InteractableController controller = interactable.controller;
			array[num2] = ((controller != null) ? controller.ToString() : null);
			Game.Log(string.Concat(array), 2);
			interactable.SpawnCheck();
			return;
		}
		Game.Log("Unable to find item " + this.debugFindID.ToString(), 2);
	}

	// Token: 0x0600178C RID: 6028 RVA: 0x00161ED8 File Offset: 0x001600D8
	[Button(null, 0)]
	public void ListFurnitureParentSpawned()
	{
		Interactable interactable = CityData.Instance.interactableDirectory.Find((Interactable item) => item.id == this.debugFindID);
		if (interactable != null)
		{
			string[] array = new string[8];
			array[0] = "Found item ";
			array[1] = interactable.id.ToString();
			array[2] = ": ";
			array[3] = interactable.name;
			array[4] = " at ";
			int num = 5;
			Vector3 wPos = interactable.wPos;
			array[num] = wPos.ToString();
			array[6] = " spawned: ";
			int num2 = 7;
			InteractableController controller = interactable.controller;
			array[num2] = ((controller != null) ? controller.ToString() : null);
			Game.Log(string.Concat(array), 2);
			if (interactable.furnitureParent == null)
			{
				return;
			}
			Game.Log("Furniture parent is " + interactable.furnitureParent.id.ToString(), 2);
			using (List<Interactable>.Enumerator enumerator = interactable.furnitureParent.spawnedInteractables.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Interactable interactable2 = enumerator.Current;
					string[] array2 = new string[7];
					array2[0] = interactable2.GetName();
					array2[1] = " ";
					array2[2] = interactable2.id.ToString();
					array2[3] = " is a spawned interactable of furniture ";
					array2[4] = interactable.furnitureParent.furniture.name;
					array2[5] = ": ";
					int num3 = 6;
					InteractableController controller2 = interactable.controller;
					array2[num3] = ((controller2 != null) ? controller2.ToString() : null);
					Game.Log(string.Concat(array2), 2);
				}
				return;
			}
		}
		Game.Log("Unable to find item " + this.debugFindID.ToString(), 2);
	}

	// Token: 0x04001CD9 RID: 7385
	[Header("Debug")]
	public int debugFindID;

	// Token: 0x04001CDA RID: 7386
	private static InteractableCreator _instance;
}
