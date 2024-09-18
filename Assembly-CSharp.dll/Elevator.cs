using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003AA RID: 938
public class Elevator
{
	// Token: 0x0600154E RID: 5454 RVA: 0x00135FD8 File Offset: 0x001341D8
	public Elevator(StairwellPreset newPreset, NewBuilding newBuilding, NewTile newBottom)
	{
		this.preset = newPreset;
		this.bottom = newBottom;
		this.building = newBuilding;
		this.building.stairwells.Add(this.bottom, this);
		this.AddFloor(this.bottom);
		this.currentFloor = this.bottom.globalTileCoord.z;
		this.currentDestination = this.bottom.globalTileCoord.z;
		this.ultimateDesitnation = this.bottom.globalTileCoord.z;
		if (this.preset.featuresElevator)
		{
			this.spawnedObject = Toolbox.Instance.SpawnObject(this.preset.elevatorObject, newBottom.parent).transform;
			ElevatorKillBox componentInChildren = this.spawnedObject.GetComponentInChildren<ElevatorKillBox>();
			if (componentInChildren != null)
			{
				componentInChildren.elevator = this;
			}
			foreach (NewNode newNode in newBottom.nodes)
			{
				if (newNode.room != null && !newNode.room.isNullRoom)
				{
					this.spawnedObject.SetParent(newNode.room.transform, true);
					break;
				}
			}
			this.spawnedObject.position = newBottom.position + new Vector3(0f, 0.01f, 0f);
			InteractableController componentInChildren2 = this.spawnedObject.gameObject.GetComponentInChildren<InteractableController>();
			this.controls = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.elevatorControls, this.spawnedObject, null, null, componentInChildren2.gameObject.transform.localPosition, componentInChildren2.gameObject.transform.localEulerAngles, null);
			componentInChildren2.Setup(this.controls);
			this.controls.SetPolymorphicReference(this);
			this.cable1 = this.spawnedObject.transform.Find("Cable1");
			this.cable2 = this.spawnedObject.transform.Find("Cable2");
			this.UpdateCables();
		}
	}

	// Token: 0x0600154F RID: 5455 RVA: 0x0013621C File Offset: 0x0013441C
	public void LoadElevatorSaveData(StateSaveData.ElevatorStateSave data)
	{
		if (data == null)
		{
			return;
		}
		if (this.spawnedObject != null)
		{
			this.spawnedObject.transform.position = new Vector3(this.spawnedObject.transform.position.x, data.yPos, this.spawnedObject.transform.position.z);
		}
		this.currentFloor = data.floor;
	}

	// Token: 0x06001550 RID: 5456 RVA: 0x0013628C File Offset: 0x0013448C
	public void AddFloor(NewTile newTile)
	{
		if (this.elevatorFloors.ContainsKey(newTile.globalTileCoord.z))
		{
			return;
		}
		if (this.top == null || newTile.globalTileCoord.z > this.top.globalTileCoord.z)
		{
			this.top = newTile;
		}
		if (this.bottom == null || newTile.globalTileCoord.z < this.bottom.globalTileCoord.z)
		{
			this.bottom = newTile;
		}
		if (this.preset.featuresElevator)
		{
			Elevator.ElevatorFloor elevatorFloor = new Elevator.ElevatorFloor();
			elevatorFloor.elevatorRoom = newTile.nodes[0].room;
			elevatorFloor.floor = newTile.globalTileCoord.z;
			elevatorFloor.elevatorTile = newTile;
			if (newTile.stairwell != null)
			{
				this.OnSpawnStairwell(newTile);
			}
			this.elevatorFloors.Add(newTile.globalTileCoord.z, elevatorFloor);
			this.UpdateCables();
		}
	}

	// Token: 0x06001551 RID: 5457 RVA: 0x00136380 File Offset: 0x00134580
	public void OnSpawnStairwell(NewTile tile)
	{
		Elevator.ElevatorFloor elevatorFloor = null;
		if (!this.elevatorFloors.TryGetValue(tile.globalTileCoord.z, ref elevatorFloor))
		{
			Game.LogError("Elevator at " + tile.building.name + " cannot find floor entry for floor " + tile.globalTileCoord.z.ToString(), 2);
			return;
		}
		elevatorFloor.spawned = tile.stairwell;
		foreach (InteractableController interactableController in elevatorFloor.spawned.GetComponentsInChildren<InteractableController>())
		{
			if (interactableController.id == InteractableController.InteractableID.A)
			{
				elevatorFloor.upButton = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.elevatorUpButton, interactableController.transform.parent, null, null, interactableController.transform.localPosition, interactableController.transform.localEulerAngles, null);
				elevatorFloor.upButton.SetPolymorphicReference(this);
				interactableController.Setup(elevatorFloor.upButton);
			}
			else if (interactableController.id == InteractableController.InteractableID.B)
			{
				elevatorFloor.downButton = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.elevatorDownButton, interactableController.transform.parent, null, null, interactableController.transform.localPosition, interactableController.transform.localEulerAngles, null);
				elevatorFloor.downButton.SetPolymorphicReference(this);
				interactableController.Setup(elevatorFloor.downButton);
			}
			else if (interactableController.id == InteractableController.InteractableID.C)
			{
				elevatorFloor.door = InteractableCreator.Instance.CreateTransformInteractable(InteriorControls.Instance.doorC, interactableController.transform.parent, null, null, interactableController.transform.localPosition, interactableController.transform.localEulerAngles, null);
				elevatorFloor.door.SetPolymorphicReference(this);
				interactableController.Setup(elevatorFloor.door);
			}
		}
		this.spawnedObject.eulerAngles = tile.stairwell.transform.eulerAngles;
	}

	// Token: 0x06001552 RID: 5458 RVA: 0x0013654C File Offset: 0x0013474C
	public void CallElevator(int newFloor, bool upButton)
	{
		newFloor = Mathf.Clamp(newFloor, this.bottom.globalTileCoord.z, this.top.globalTileCoord.z);
		if (!this.calls.ContainsKey(newFloor))
		{
			this.calls.Add(newFloor, new List<Elevator.ElevatorCall>());
			this.calls[newFloor].Add(new Elevator.ElevatorCall(this.elevatorFloors[newFloor], upButton, SessionData.Instance.gameTime));
		}
		else if (!this.calls[newFloor].Exists((Elevator.ElevatorCall item) => item.callUp == upButton))
		{
			this.calls[newFloor].Add(new Elevator.ElevatorCall(this.elevatorFloors[newFloor], upButton, SessionData.Instance.gameTime));
		}
		Game.Log("Call elevator " + newFloor.ToString() + " up: " + upButton.ToString(), 2);
		if (!this.isActive)
		{
			this.isActive = true;
			SessionData.Instance.activeElevators.Add(this);
			this.ElevatorUpdate();
		}
	}

	// Token: 0x06001553 RID: 5459 RVA: 0x00136680 File Offset: 0x00134880
	public void ElevatorUpdate()
	{
		try
		{
			if (!this.inTransit && this.calls.Count > 0)
			{
				float num = float.PositiveInfinity;
				int key = this.currentFloor;
				foreach (KeyValuePair<int, List<Elevator.ElevatorCall>> keyValuePair in this.calls)
				{
					foreach (Elevator.ElevatorCall elevatorCall in keyValuePair.Value)
					{
						if (elevatorCall.registered < num)
						{
							num = elevatorCall.registered;
							key = keyValuePair.Key;
						}
					}
				}
				if (!this.inTransit)
				{
					this.liftTimer = this.preset.movementDelay;
					this.SetInTransit(true);
					if (this.elevatorFloors != null && this.elevatorFloors.ContainsKey(this.currentFloor) && this.elevatorFloors[this.currentFloor].door != null)
					{
						try
						{
							this.elevatorFloors[this.currentFloor].door.OnInteraction(this.elevatorFloors[this.currentFloor].door.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.interactionName == "Close"), null, true, 0f);
						}
						catch
						{
							Game.Log("Unable to close elevator door!", 2);
						}
					}
				}
				this.isGoingUp = false;
				if (key > this.currentFloor)
				{
					this.isGoingUp = true;
				}
				this.UpdateDestination();
			}
		}
		catch
		{
			Game.LogError("Elevator call from idle error!", 2);
		}
		try
		{
			if (this.inTransit)
			{
				if (this.movementAudio != null)
				{
					if (this.spawnedObject != null)
					{
						this.movementAudio.UpdateWorldPosition(this.spawnedObject.transform.position, null);
					}
					this.movementAudio.audioEvent.setParameterByName("Speed", this.currentSpeed / this.preset.elevatorMaxSpeed, false);
					if (Player.Instance.currentNode != null && Player.Instance.currentNode.floor != null)
					{
						float num2 = Player.Instance.currentNode.position.y + (float)Player.Instance.currentNode.floor.defaultCeilingHeight * 0.1f - (float)Player.Instance.currentNode.floorHeight * 0.1f;
						float y = Player.Instance.currentNode.position.y;
						float num3 = Mathf.Lerp(y, num2, 0.5f);
						float y2 = CameraController.Instance.cam.transform.position.y;
						float num4;
						if (y2 < y || y2 > num2)
						{
							num4 = 1f;
						}
						else if (y2 <= num3)
						{
							num4 = 1f - Mathf.InverseLerp(y, num3, y2);
						}
						else
						{
							num4 = Mathf.InverseLerp(num3, num2, y2);
						}
						this.movementAudio.audioEvent.setParameterByName("FloorPulse", num4, false);
					}
				}
				float num5 = Mathf.Abs(this.desiredY - this.spawnedObject.transform.position.y);
				if (this.liftTimer > 0f)
				{
					this.liftTimer -= Time.deltaTime;
					this.liftTimer = Mathf.Max(this.liftTimer, 0f);
				}
				else
				{
					if (num5 > 0.005f)
					{
						this.isMoving = true;
						if (num5 >= this.preset.accelerateWhileThisFarAway)
						{
							if (this.currentSpeed < this.preset.elevatorMaxSpeed)
							{
								this.currentSpeed += this.preset.elevatorAcceleration * Time.deltaTime;
								this.currentSpeed = Mathf.Clamp(this.currentSpeed, 0f, this.preset.elevatorMaxSpeed);
								this.reachedSpeed = this.currentSpeed;
							}
						}
						else
						{
							this.currentSpeed = Mathf.Lerp(this.reachedSpeed, 0.15f, 1f - num5 / this.preset.accelerateWhileThisFarAway);
						}
						if (this.spawnedObject != null)
						{
							this.spawnedObject.transform.position = Vector3.MoveTowards(this.spawnedObject.transform.position, new Vector3(this.spawnedObject.transform.position.x, this.desiredY, this.spawnedObject.transform.position.z), this.currentSpeed * Time.deltaTime);
						}
						if (Player.Instance.currentVehicle == this.spawnedObject.transform || Vector3.Distance(Player.Instance.transform.position, this.spawnedObject.transform.position) <= 4f)
						{
							SessionData.Instance.ExecuteSyncPhysics(SessionData.PhysicsSyncType.onPlayerMovement);
						}
						this.UpdateCables();
						Vector3Int vector3Int = CityData.Instance.RealPosToPathmapIncludingZ(this.spawnedObject.transform.position);
						int num6 = Mathf.FloorToInt((float)vector3Int.z);
						if (num6 == this.currentFloor)
						{
							goto IL_73D;
						}
						this.currentFloor = num6;
						using (List<NewNode>.Enumerator enumerator3 = PathFinder.Instance.tileMap[vector3Int].nodes.GetEnumerator())
						{
							while (enumerator3.MoveNext())
							{
								NewNode newNode = enumerator3.Current;
								if (newNode.room != null)
								{
									if (this.spawnedObject != null)
									{
										this.spawnedObject.SetParent(newNode.room.transform, true);
										break;
									}
									break;
								}
							}
							goto IL_73D;
						}
					}
					this.currentSpeed = 0f;
					this.reachedSpeed = 0f;
					if (this.calls.ContainsKey(this.currentFloor))
					{
						this.calls.Remove(this.currentFloor);
						if (this.isMoving)
						{
							AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.elevatorDing, Player.Instance, this.elevatorFloors[this.currentFloor].door.node, this.elevatorFloors[this.currentFloor].door.controller.transform.position, null, null, 1f, null, false, null, false);
							this.isMoving = false;
						}
						if (this.elevatorFloors != null && this.elevatorFloors.ContainsKey(this.currentFloor) && this.elevatorFloors[this.currentFloor].door != null)
						{
							try
							{
								this.elevatorFloors[this.currentFloor].door.OnInteraction(this.elevatorFloors[this.currentFloor].door.preset.GetActions(0).Find((InteractablePreset.InteractionAction item) => item.interactionName == "Open"), null, true, 0f);
							}
							catch
							{
								Game.Log("Unable to open elevator door!", 2);
							}
						}
						if (this.calls.Count > 0)
						{
							this.liftTimer = this.preset.liftDelay;
						}
					}
					else
					{
						this.SetInTransit(false);
					}
				}
			}
			IL_73D:;
		}
		catch (Exception ex)
		{
			Game.LogError("Elevator inTransit error: " + ex.ToString(), 2);
		}
		if (this.calls != null && this.calls.Count <= 0)
		{
			this.EndMovement();
		}
	}

	// Token: 0x06001554 RID: 5460 RVA: 0x00136EB0 File Offset: 0x001350B0
	private void UpdateCables()
	{
		if (this.top != null && this.cable1 != null)
		{
			float num = this.top.position.y + PathFinder.Instance.tileSize.z - this.cable1.localPosition.y;
			this.cable1.localScale = new Vector3(1f, num, 1f);
			this.cable2.localScale = new Vector3(1f, num, 1f);
		}
	}

	// Token: 0x06001555 RID: 5461 RVA: 0x00136F3C File Offset: 0x0013513C
	private void EndMovement()
	{
		this.isActive = false;
		this.SetInTransit(false);
		this.currentSpeed = 0f;
		this.reachedSpeed = 0f;
		this.liftTimer = 0f;
		SessionData.Instance.activeElevators.Remove(this);
	}

	// Token: 0x06001556 RID: 5462 RVA: 0x00136F8C File Offset: 0x0013518C
	public void SetInTransit(bool val)
	{
		if (val != this.inTransit)
		{
			Game.Log("Set elevator in transit: " + val.ToString(), 2);
			this.inTransit = val;
			if (this.controls != null)
			{
				this.controls.SetSwitchState(this.inTransit, null, true, false, false);
			}
			else
			{
				Game.Log("Elevator controls is null!", 2);
			}
			if (this.inTransit)
			{
				if (this.movementAudio != null)
				{
					AudioController.Instance.StopSound(this.movementAudio, AudioController.StopType.fade, "Elevator stopped");
					Game.Log("Stop elevator sound", 2);
				}
				this.movementAudio = AudioController.Instance.PlayWorldLooping(AudioControls.Instance.elevatorMovement, null, null, this.spawnedObject.transform.position, null, null, 1f, false, true, null, null);
				Game.Log("Play elevator sound loop", 2);
				return;
			}
			if (this.movementAudio != null)
			{
				this.movementAudio.audioEvent.setParameterByName("Speed", 0f, false);
				AudioController.Instance.StopSound(this.movementAudio, AudioController.StopType.triggerCue, "Elevator stop");
			}
		}
	}

	// Token: 0x06001557 RID: 5463 RVA: 0x0013709C File Offset: 0x0013529C
	public void UpdateDestination()
	{
		if (this.calls.Count <= 0)
		{
			this.EndMovement();
			return;
		}
		if (this.isGoingUp)
		{
			int key = this.currentFloor;
			int num = 99999;
			foreach (KeyValuePair<int, List<Elevator.ElevatorCall>> keyValuePair in this.calls)
			{
				if (keyValuePair.Key > key)
				{
					key = keyValuePair.Key;
				}
				if (keyValuePair.Key > this.currentFloor && keyValuePair.Key < num)
				{
					num = keyValuePair.Key;
				}
			}
			this.ultimateDesitnation = key;
			this.currentDestination = Mathf.Min(num, key);
		}
		else
		{
			int key2 = this.currentFloor;
			int num2 = -99999;
			foreach (KeyValuePair<int, List<Elevator.ElevatorCall>> keyValuePair2 in this.calls)
			{
				if (keyValuePair2.Key < key2)
				{
					key2 = keyValuePair2.Key;
				}
				if (keyValuePair2.Key < this.currentFloor && keyValuePair2.Key > num2)
				{
					num2 = keyValuePair2.Key;
				}
			}
			this.ultimateDesitnation = key2;
			this.currentDestination = Mathf.Max(num2, key2);
		}
		Game.Log("New current destination " + this.currentDestination.ToString(), 2);
		this.desiredY = this.elevatorFloors[this.currentDestination].elevatorTile.position.y + 0.01f;
	}

	// Token: 0x04001A52 RID: 6738
	[Tooltip("Setup")]
	public NewBuilding building;

	// Token: 0x04001A53 RID: 6739
	public Transform spawnedObject;

	// Token: 0x04001A54 RID: 6740
	public StairwellPreset preset;

	// Token: 0x04001A55 RID: 6741
	[NonSerialized]
	public Interactable controls;

	// Token: 0x04001A56 RID: 6742
	public Collider vehicleDetector;

	// Token: 0x04001A57 RID: 6743
	public Transform cable1;

	// Token: 0x04001A58 RID: 6744
	public Transform cable2;

	// Token: 0x04001A59 RID: 6745
	public AudioController.LoopingSoundInfo movementAudio;

	// Token: 0x04001A5A RID: 6746
	[Tooltip("List of floors that the elevator can travel too using the passed elevator rooms")]
	public Dictionary<int, Elevator.ElevatorFloor> elevatorFloors = new Dictionary<int, Elevator.ElevatorFloor>();

	// Token: 0x04001A5B RID: 6747
	[Tooltip("The bottom tile (start")]
	[Header("Game State")]
	public NewTile bottom;

	// Token: 0x04001A5C RID: 6748
	[Tooltip("The top")]
	public NewTile top;

	// Token: 0x04001A5D RID: 6749
	private float reachedSpeed;

	// Token: 0x04001A5E RID: 6750
	public float currentSpeed;

	// Token: 0x04001A5F RID: 6751
	public float desiredY;

	// Token: 0x04001A60 RID: 6752
	public float liftTimer;

	// Token: 0x04001A61 RID: 6753
	[Tooltip("The elevator's current floor position")]
	public int currentFloor;

	// Token: 0x04001A62 RID: 6754
	[Tooltip("Is the elevator currently moving?")]
	public bool inTransit;

	// Token: 0x04001A63 RID: 6755
	[Tooltip("The elevator's current direction")]
	public bool isGoingUp = true;

	// Token: 0x04001A64 RID: 6756
	[Tooltip("The elevator's current destination: The next floor this will stop at")]
	public int currentDestination;

	// Token: 0x04001A65 RID: 6757
	[Tooltip("The elevator's current destination: The furthest floor this will stop at")]
	public int ultimateDesitnation;

	// Token: 0x04001A66 RID: 6758
	public bool isActive;

	// Token: 0x04001A67 RID: 6759
	public bool isMoving;

	// Token: 0x04001A68 RID: 6760
	[Tooltip("Used for elevator AI: Call on floors on the way to destination")]
	public Dictionary<int, List<Elevator.ElevatorCall>> calls = new Dictionary<int, List<Elevator.ElevatorCall>>();

	// Token: 0x020003AB RID: 939
	[Serializable]
	public class ElevatorFloor
	{
		// Token: 0x04001A69 RID: 6761
		public int floor;

		// Token: 0x04001A6A RID: 6762
		public NewTile elevatorTile;

		// Token: 0x04001A6B RID: 6763
		public NewRoom elevatorRoom;

		// Token: 0x04001A6C RID: 6764
		public GameObject spawned;

		// Token: 0x04001A6D RID: 6765
		[NonSerialized]
		public Interactable upButton;

		// Token: 0x04001A6E RID: 6766
		[NonSerialized]
		public Interactable downButton;

		// Token: 0x04001A6F RID: 6767
		[NonSerialized]
		public Interactable door;
	}

	// Token: 0x020003AC RID: 940
	[Serializable]
	public class ElevatorCall
	{
		// Token: 0x06001559 RID: 5465 RVA: 0x00137244 File Offset: 0x00135444
		public ElevatorCall(Elevator.ElevatorFloor newFloor, bool newUp, float newRegistered)
		{
			this.floor = newFloor;
			this.callUp = newUp;
			this.registered = newRegistered;
		}

		// Token: 0x04001A70 RID: 6768
		public Elevator.ElevatorFloor floor;

		// Token: 0x04001A71 RID: 6769
		public bool callUp;

		// Token: 0x04001A72 RID: 6770
		public float registered;
	}
}
