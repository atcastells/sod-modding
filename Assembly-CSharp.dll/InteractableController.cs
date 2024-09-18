using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

// Token: 0x020003FC RID: 1020
public class InteractableController : Controller
{
	// Token: 0x06001744 RID: 5956 RVA: 0x0015D560 File Offset: 0x0015B760
	public void Setup(Interactable newInteractable)
	{
		this.interactable = newInteractable;
		this.interactable.controller = this;
		if (base.gameObject.layer == 0 && !this.interactable.preset.showWorldObjectInSceneCapture)
		{
			base.gameObject.layer = 7;
		}
		if (Game.Instance.collectDebugData)
		{
			this.debugInteractable.Clear();
			this.debugInteractable.Add(this.interactable);
		}
		base.gameObject.tag = "Interactable";
		if (this.altColl != null)
		{
			this.altColl.gameObject.tag = "Interactable";
		}
		if (this.rb == null)
		{
			this.rb = base.gameObject.GetComponent<Rigidbody>();
		}
		if (this.coll == null)
		{
			this.coll = base.gameObject.GetComponent<Collider>();
		}
		if (this.doorMovement != null)
		{
			this.doorMovement.Setup(this.interactable, true);
		}
		if (this.secondaryDoorMovement != null)
		{
			this.secondaryDoorMovement.Setup(this.interactable, true);
		}
		if (this.thirdDoorMovement != null)
		{
			this.thirdDoorMovement.Setup(this.interactable, true);
		}
		if (this.securitySystem != null)
		{
			this.securitySystem.Setup(this.interactable, true);
		}
		if (this.steam != null)
		{
			this.steam.Setup(this.interactable.node.room);
		}
		if (this.computer != null)
		{
			this.computer.Setup(this);
		}
		if (this.fileSystem != null)
		{
			this.fileSystem.Setup(this);
		}
		if (this.echelonsScreen != null)
		{
			this.echelonsScreen.Setup(this);
		}
		if (this.partSystem != null)
		{
			if (this.useSmokeMaterial)
			{
				try
				{
					this.partSystem.gameObject.GetComponent<ParticleSystemRenderer>().sharedMaterial = CityControls.Instance.smokeMaterial;
				}
				catch
				{
				}
			}
			SessionData.Instance.particleSystems.Add(this);
			this.UpdateParticleSystemDistance();
		}
		this.UpdateSwitchSync();
		if (this.isPhone)
		{
			this.interactable.OnState1Change += this.State1Change;
		}
		this.SetPhysics(this.interactable.preset.forcePhysicsAlwaysOn, null);
		base.enabled = false;
		this.belongsTo = newInteractable.belongsTo;
		if (this.decalProjector != null && this.interactable.preset.isDecal)
		{
			ArtPreset artPreset = this.interactable.objectRef as ArtPreset;
			Interactable.Passed dynamic = null;
			if (!this.interactable.preset.showWorldObjectInSceneCapture)
			{
				this.decalProjector.gameObject.layer = 7;
			}
			if (artPreset == null)
			{
				dynamic = this.interactable.pv.Find((Interactable.Passed item) => item.varType == Interactable.PassedVarType.decalDynamicText);
			}
			this.SetupDecal(artPreset, dynamic, true);
		}
		if (this.pages != null && this.pages.Count > 0)
		{
			EvidenceMultiPage evidenceMultiPage = this.interactable.evidence as EvidenceMultiPage;
			if (evidenceMultiPage != null)
			{
				evidenceMultiPage.OnPageChanged += this.OnPageChange;
				for (int i = 0; i < this.pages.Count; i++)
				{
					DoorMovementController component = this.pages[i].gameObject.GetComponent<DoorMovementController>();
					if (component != null)
					{
						if (!component.isSetup)
						{
							component.Setup(this.interactable, false);
						}
						if (i < evidenceMultiPage.page && !component.isOpen)
						{
							component.SetOpen(1f, null, true);
						}
						else if (i >= evidenceMultiPage.page && component.isOpen)
						{
							component.SetOpen(0f, null, true);
						}
					}
				}
			}
		}
		if (this.coll != null)
		{
			this.colliderExtents = this.coll.bounds.extents;
			if (this.interactable.ft)
			{
				this.coll.isTrigger = true;
			}
		}
		if (this.altColl != null)
		{
			if (this.coll == null)
			{
				this.colliderExtents = this.altColl.bounds.extents;
			}
			if (this.interactable.ft)
			{
				this.altColl.isTrigger = true;
			}
		}
		if (Game.Instance.collectDebugData)
		{
			if (this.interactable.furnitureParent != null)
			{
				this.debugAngle = this.interactable.furnitureParent.angle;
				this.debugFurnitureAnchorNodePos = this.interactable.furnitureParent.anchorNode.position;
			}
			this.debugLocalEuler = this.interactable.lEuler;
			this.debugLocalPos = this.interactable.lPos;
			this.debugWorldPos = base.transform.position;
			this.debugInteractablePredictedWorldPos = this.interactable.wPos;
			if (this.interactable.node != null)
			{
				this.debugNodeCoord = this.interactable.node.nodeCoord;
				this.debugRoom = this.interactable.node.room;
			}
			else
			{
				this.debugNodeCoord = Vector3.zero;
			}
			this.debugUsagePoint = this.interactable.usagePoint;
			this.debugOwnedBy = this.interactable.belongsTo;
			this.debugWrittenBy = this.interactable.writer;
			this.debugReceivedBy = this.interactable.reciever;
			this.debugSwitchState = this.interactable.sw0;
			this.debugState1 = this.interactable.sw1;
			this.debugPasswordSource = this.interactable.passwordSource;
			if (this.interactable.furnitureParent != null)
			{
				this.debugFurnitureOwnedBy = this.interactable.furnitureParent.debugOwners;
			}
		}
	}

	// Token: 0x06001745 RID: 5957 RVA: 0x0015DB64 File Offset: 0x0015BD64
	public void SetupDecal(ArtPreset foundArt, Interactable.Passed dynamic, bool doGraffitiChecks = true)
	{
		if (foundArt != null)
		{
			Material material = foundArt.material;
			if (foundArt.useDynamicText && !GameplayController.Instance.dynamicTextImages.TryGetValue(foundArt, ref material))
			{
				material = foundArt.material;
			}
			this.decalProjector.material = material;
			Texture texture = this.decalProjector.material.GetTexture("_BaseColorMap");
			this.decalProjector.size = new Vector3((float)texture.width * foundArt.pixelScaleMultiplier, (float)texture.height * foundArt.pixelScaleMultiplier, 0.14f);
			return;
		}
		if (dynamic != null)
		{
			Material material2 = null;
			if (GameplayController.Instance.graffitiCache.TryGetValue(dynamic.str, ref material2))
			{
				this.decalProjector.material = material2;
			}
			else
			{
				string[] parsed = dynamic.str.Split('|', 0);
				if (parsed.Length >= 4)
				{
					Game.Log("Parsing custom graffiti: " + dynamic.str, 2);
					Material material3 = Object.Instantiate<Material>(this.decalProjector.material);
					TextToImageController.TextToImageSettings textToImageSettings = new TextToImageController.TextToImageSettings();
					textToImageSettings.textString = parsed[0];
					textToImageSettings.font = DDSControls.Instance.fonts.Find((TMP_FontAsset item) => item.name == parsed[1]);
					if (textToImageSettings.font == null)
					{
						Game.Log("Unable to find font " + parsed[1], 2);
					}
					if (!float.TryParse(parsed[2], ref textToImageSettings.textSize))
					{
						Game.Log("Unable to parse float " + parsed[2], 2);
					}
					if (!ColorUtility.TryParseHtmlString("#" + parsed[3], ref textToImageSettings.color))
					{
						Game.Log("Unable to parse colour " + parsed[3], 2);
					}
					textToImageSettings.useAlpha = true;
					textToImageSettings.enableProcessing = true;
					Texture2D texture2D = TextToImageController.Instance.CaptureTextToImage(textToImageSettings);
					material3.SetTexture("_BaseColorMap", texture2D);
					this.decalProjector.material = material3;
					this.decalProjector.size = new Vector3((float)texture2D.width * 0.02f, (float)texture2D.height * 0.02f, 0.14f);
					GameplayController.Instance.AddToGraffitiCache(dynamic.str, material3);
				}
			}
			if (doGraffitiChecks)
			{
				ResizeTrigger componentInChildren = base.gameObject.GetComponentInChildren<ResizeTrigger>(true);
				if (componentInChildren != null)
				{
					componentInChildren.TriggerGraffitiChecks();
				}
			}
		}
	}

	// Token: 0x06001746 RID: 5958 RVA: 0x0015DDE0 File Offset: 0x0015BFE0
	public void UpdateSwitchSync()
	{
		if (this.enableSwitchSync)
		{
			foreach (SwitchSyncBehaviour switchSyncBehaviour in this.switchSyncObjects)
			{
				if (!(switchSyncBehaviour == null) && (!switchSyncBehaviour.onlySyncWhenParentIsOn || this.interactable.sw0))
				{
					switchSyncBehaviour.SetOn(this.interactable.GetSwitchQuery(switchSyncBehaviour.syncWithState));
				}
			}
		}
	}

	// Token: 0x06001747 RID: 5959 RVA: 0x0015DE6C File Offset: 0x0015C06C
	public void OnPageChange(int newPage)
	{
		if (this.fileSystem != null)
		{
			this.fileSystem.SetPage(newPage, false);
		}
		for (int i = 0; i < this.pages.Count; i++)
		{
			Transform transform = this.pages[i];
			if (transform != null)
			{
				DoorMovementController component = transform.gameObject.GetComponent<DoorMovementController>();
				if (component != null)
				{
					if (!component.isSetup)
					{
						component.Setup(this.interactable, false);
					}
					if (i < newPage && !component.isOpen)
					{
						component.SetOpen(1f, null, false);
					}
					else if (i >= newPage && component.isOpen)
					{
						component.SetOpen(0f, null, false);
					}
				}
			}
			else if (Game.Instance.printDebug)
			{
				Game.Log("Calendar page transform missing at index " + i.ToString(), 2);
			}
		}
	}

	// Token: 0x06001748 RID: 5960 RVA: 0x0015DF4C File Offset: 0x0015C14C
	private void OnDestroy()
	{
		if (this.pages != null && this.interactable != null && this.pages.Count > 0)
		{
			(this.interactable.evidence as EvidenceMultiPage).OnPageChanged -= this.OnPageChange;
		}
		if (this.isPhone && this.interactable != null)
		{
			this.interactable.OnState1Change -= this.State1Change;
		}
	}

	// Token: 0x06001749 RID: 5961 RVA: 0x0015DFC0 File Offset: 0x0015C1C0
	public void GetScreenBox(out Vector2 uiMin, out Vector2 uiMax)
	{
		uiMax = Vector2.zero;
		uiMin = Vector2.zero;
		if (this.coll == null)
		{
			return;
		}
		Collider collider = this.coll;
		if (this.altColl != null && InteractionController.Instance.playerCurrentRaycastHit.collider == this.altColl)
		{
			collider = this.altColl;
		}
		Vector3 center = collider.bounds.center;
		List<Vector3> list = new List<Vector3>();
		Vector3 vector = center - collider.bounds.extents;
		Vector3 vector2 = center + collider.bounds.extents;
		list.Add(vector);
		list.Add(vector2);
		list.Add(new Vector3(vector.x, vector.y, vector2.z));
		list.Add(new Vector3(vector.x, vector2.y, vector.z));
		list.Add(new Vector3(vector2.x, vector.y, vector.z));
		list.Add(new Vector3(vector.x, vector2.y, vector2.z));
		list.Add(new Vector3(vector2.x, vector.y, vector2.z));
		list.Add(new Vector3(vector2.x, vector2.y, vector.z));
		Vector2 vector3;
		vector3..ctor(9999999f, 9999999f);
		Vector2 vector4;
		vector4..ctor(-9999999f, -9999999f);
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 vector5 = CameraController.Instance.cam.WorldToScreenPoint(list[i]);
			vector3 = Vector2.Min(vector3, vector5);
			vector4 = Vector2.Max(vector4, vector5);
		}
		RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector3, null, ref uiMin);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(InterfaceController.Instance.firstPersonUI, vector4, null, ref uiMax);
	}

	// Token: 0x0600174A RID: 5962 RVA: 0x00002265 File Offset: 0x00000465
	public void OnExitInteractionMode()
	{
	}

	// Token: 0x0600174B RID: 5963 RVA: 0x0015E1C0 File Offset: 0x0015C3C0
	public void MovablePickUpThis()
	{
		if (InteractionController.Instance.carryingObject != null)
		{
			InteractionController.Instance.carryingObject.DropThis(false);
		}
		Game.Log("Player: Pick up " + base.name, 2);
		if (this.meshes.Count <= 0)
		{
			return;
		}
		this.damageEligable = false;
		InteractionController.Instance.carryingObject = this;
		this.isCarriedByPlayer = true;
		this.SetPhysics(false, null);
		InteractionController.Instance.DisplayInteractionCursor(false, true);
		if (this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.stovetopKettle)
		{
			this.interactable.SetSwitchState(false, null, true, false, false);
		}
		this.minimumPhysicsTime = 0f;
		this.pickupPos = base.transform.position;
		this.pickupRot = base.transform.rotation;
		this.carryProgress = 0f;
		this.rotProgress = 0f;
		foreach (Collider collider in this.additionalPhysicsOnlyColliders)
		{
			Physics.IgnoreCollision(collider, Player.Instance.charController, true);
		}
		Player.Instance.UpdateSkinWidth();
		this.coll.enabled = false;
		if (this.altColl != null)
		{
			this.altColl.enabled = false;
		}
		base.enabled = true;
		this.interactable.SetOriginalPosition(false, true);
	}

	// Token: 0x0600174C RID: 5964 RVA: 0x0015E33C File Offset: 0x0015C53C
	public void RotateHeldObject(float val)
	{
		this.heldEuler.y = this.heldEuler.y + val;
		this.heldEuler = new Vector3((float)Mathf.RoundToInt(this.heldEuler.x), (float)Mathf.RoundToInt(this.heldEuler.y), (float)Mathf.RoundToInt(this.heldEuler.z));
		this.pickupRot = base.transform.rotation;
		this.rotProgress = 0f;
		string text = "Player: Rotate held object: ";
		Vector3 vector = this.heldEuler;
		Game.Log(text + vector.ToString(), 2);
	}

	// Token: 0x0600174D RID: 5965 RVA: 0x0015E3D8 File Offset: 0x0015C5D8
	private void Update()
	{
		if (this.objectParticleCreationDelay > 0f)
		{
			this.objectParticleCreationDelay -= Time.deltaTime;
		}
		if (this.isCarriedByPlayer)
		{
			if (this.rb != null)
			{
				this.rb.useGravity = false;
			}
			if (this.carryProgress < 1f)
			{
				this.carryProgress += Time.deltaTime * 9f;
				this.carryProgress = Mathf.Clamp01(this.carryProgress);
			}
			if (this.rotProgress < 1f)
			{
				this.rotProgress += Time.deltaTime * 8f;
				this.rotProgress = Mathf.Clamp01(this.rotProgress);
			}
			if (this.interactable.usagePoint != null)
			{
				Human human = null;
				this.interactable.usagePoint.TryGetUserAtSlot(Interactable.UsePointSlot.defaultSlot, out human);
				if (human != null && human.ai != null && human.ai.currentAction != null)
				{
					human.speechController.TriggerBark(SpeechController.Bark.fallOffChair);
					Vector3 position = human.outfitController.GetBodyAnchor(CitizenOutfitController.CharacterAnchor.lowerTorso).position;
					Vector3 damageDirection = position - CameraController.Instance.cam.transform.position;
					human.RecieveDamage(0.01f, Player.Instance, position, damageDirection, CriminalControls.Instance.punchSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, false, true, Toolbox.Instance.Rand(0f, 0.05f, false), 1f, false, true, 1f);
					human.ai.currentAction.Remove(human.ai.currentAction.preset.repeatDelayOnActionFail);
				}
			}
			PhysicsProfile physicsProfile = this.interactable.preset.GetPhysicsProfile();
			if (physicsProfile != null && !this.setHeldEuler)
			{
				this.heldEuler = physicsProfile.heldEuler;
				this.setHeldEuler = true;
			}
			float num = 0.25f;
			Vector3 vector = base.transform.position;
			if (this.alternativePhysicsParent != null)
			{
				vector = this.alternativePhysicsParent.transform.position;
			}
			if (this.meshes.Count > 0)
			{
				vector = this.meshes[0].bounds.ClosestPoint(Player.Instance.cam.transform.position);
				num = Vector3.Distance(this.meshes[0].bounds.center, vector);
			}
			Vector3 vector2 = Player.Instance.cam.transform.TransformPoint(Vector3.forward * (GameplayControls.Instance.carryDistance + num));
			Vector3 vector3 = this.ConvertBoundsPositionToTransformPosition(vector2);
			Vector3 vector4 = vector2 - Player.Instance.cam.transform.position;
			Ray ray = new Ray(Player.Instance.cam.transform.position, vector4);
			float num2 = GameplayControls.Instance.carryDistance + num * 2f;
			Quaternion quaternion = Quaternion.identity;
			bool flag = false;
			this.apartmentPlacementIsValid = true;
			if (this.interactable.preset.apartmentPlacementMode != InteractablePreset.ApartmentPlacementMode.physics)
			{
				this.apartmentPlacementIsValid = false;
				num2 += 2.5f;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(ray, ref raycastHit, num2, Toolbox.Instance.heldObjectsObjectsLayerMask))
			{
				if (this.interactable.preset.apartmentPlacementMode == InteractablePreset.ApartmentPlacementMode.vertical)
				{
					if (raycastHit.normal.y == 0f)
					{
						vector2 = raycastHit.point + raycastHit.normal * this.colliderExtents.z;
						vector3 = this.ConvertBoundsPositionToTransformPosition(vector2);
						quaternion = Quaternion.LookRotation(raycastHit.normal);
						flag = true;
						this.apartmentPlacementIsValid = true;
					}
				}
				else if (this.interactable.preset.apartmentPlacementMode == InteractablePreset.ApartmentPlacementMode.ceiling && raycastHit.normal.y == -1f)
				{
					vector2 = raycastHit.point + raycastHit.normal * this.colliderExtents.z;
					vector3 = this.ConvertBoundsPositionToTransformPosition(vector2);
					quaternion = Quaternion.identity;
					flag = true;
					this.apartmentPlacementIsValid = true;
				}
				if (this.apartmentPlacementIsValid && this.interactable.preset.apartmentPlacementMode != InteractablePreset.ApartmentPlacementMode.physics && this.interactable.preset.mustTouchFurniture.Count > 0)
				{
					MeshFilter component = raycastHit.transform.gameObject.GetComponent<MeshFilter>();
					if (component != null)
					{
						FurniturePreset furnitureFromMesh = Toolbox.Instance.GetFurnitureFromMesh(component.sharedMesh);
						if (furnitureFromMesh != null)
						{
							if (this.interactable.preset.mustTouchFurniture.Contains(furnitureFromMesh))
							{
								this.apartmentPlacementIsValid = true;
							}
							else
							{
								this.apartmentPlacementIsValid = false;
							}
						}
						else
						{
							this.apartmentPlacementIsValid = false;
						}
					}
					else
					{
						this.apartmentPlacementIsValid = false;
					}
				}
				if (!flag)
				{
					float num3 = num2 - raycastHit.distance;
					vector2 = Player.Instance.cam.transform.TransformPoint(Vector3.forward * Mathf.Max(GameplayControls.Instance.carryDistance + num - num3, Mathf.Max(GameplayControls.Instance.carryDistance, num + 0.1f)));
					vector3 = this.ConvertBoundsPositionToTransformPosition(vector2);
				}
			}
			if (this.interactable.preset.apartmentPlacementMode != InteractablePreset.ApartmentPlacementMode.physics)
			{
				this.interactable.UpdateCurrentActions();
			}
			if (!flag)
			{
				quaternion = Player.Instance.transform.rotation * Quaternion.Euler(this.heldEuler);
			}
			vector3..ctor(vector3.x, Mathf.Max(vector3.y, Player.Instance.transform.position.y - Player.Instance.charController.height * 0.5f - 0.2f), vector3.z);
			if (this.alternativePhysicsParent != null)
			{
				this.alternativePhysicsParent.transform.SetPositionAndRotation(Vector3.Lerp(this.pickupPos, vector3, this.carryProgress), Quaternion.Slerp(this.pickupRot, quaternion, this.rotProgress));
			}
			else
			{
				base.transform.SetPositionAndRotation(Vector3.Lerp(this.pickupPos, vector3, this.carryProgress), Quaternion.Slerp(this.pickupRot, quaternion, this.rotProgress));
			}
		}
		else if (this.physicsOn)
		{
			if (SessionData.Instance.play && this.rb != null && Mathf.Abs(this.rb.velocity.x) + Mathf.Abs(this.rb.velocity.y) + Mathf.Abs(this.rb.velocity.z) < 0.1f && !this.interactable.preset.forcePhysicsAlwaysOn)
			{
				this.minimumPhysicsTime += Time.deltaTime;
				if (this.minimumPhysicsTime >= GameplayControls.Instance.physicsOffTime)
				{
					this.SetPhysics(false, null);
					base.enabled = false;
					return;
				}
			}
			else
			{
				this.minimumPhysicsTime = 0f;
			}
			if (this.interactable.preset.particleProfile != null && this.rb != null && this.objectParticleCreationDelay <= 0f && this.rb.velocity.magnitude > 0.1f)
			{
				if (this.interactable.preset.particleProfile.spatterTrigger == ParticleEffect.SpatterTrigger.whileInAirOrAnyImpact)
				{
					this.Spatter(base.transform.position);
				}
				if (this.interactable.preset.particleProfile.creationTrigger == ParticleEffect.SpatterTrigger.whileInAirOrAnyImpact)
				{
					this.ParticleObjectCreation();
				}
			}
		}
		this.UpdateLastMovement();
	}

	// Token: 0x0600174E RID: 5966 RVA: 0x0015EB7C File Offset: 0x0015CD7C
	private Vector3 ConvertBoundsPositionToTransformPosition(Vector3 boundsPosition)
	{
		Vector3 result = boundsPosition;
		if (this.meshes.Count > 0)
		{
			Vector3 vector = base.transform.InverseTransformPoint(this.meshes[0].bounds.center);
			result = boundsPosition - vector;
		}
		return result;
	}

	// Token: 0x0600174F RID: 5967 RVA: 0x0015EBC8 File Offset: 0x0015CDC8
	public void DropThis(bool throwThis)
	{
		this.carryProgress = 0f;
		this.rotProgress = 0f;
		this.interactable.SetOriginalPosition(false, true);
		if (this.isCarriedByPlayer)
		{
			InteractionController.Instance.SetLockedInInteractionMode(null, 0, false);
			if (this.interactable.phy)
			{
				this.interactable.SetPhysicsPickupState(false, Player.Instance, true, false);
			}
			InteractionController.Instance.carryingObject = null;
			this.isCarriedByPlayer = false;
		}
		if (!this.physicsOn && this.interactable.preset.apartmentPlacementMode == InteractablePreset.ApartmentPlacementMode.physics)
		{
			this.SetPhysics(true, Player.Instance);
		}
		foreach (Collider collider in this.additionalPhysicsOnlyColliders)
		{
			Physics.IgnoreCollision(collider, Player.Instance.charController, false);
		}
		if (this.rb != null)
		{
			this.rb.useGravity = true;
		}
		this.coll.enabled = true;
		if (this.altColl != null)
		{
			this.altColl.enabled = true;
		}
		if (throwThis && this.rb != null)
		{
			AudioController.Instance.PlayWorldOneShot(AudioControls.Instance.throwObject, this.interactable.controller.thrownBy, null, this.interactable.controller.thrownBy.transform.position, null, null, 1f, null, false, null, false);
			this.damageEligable = true;
			float throwForceMultiplier = this.interactable.preset.GetPhysicsProfile().throwForceMultiplier;
			Vector3 vector = Player.Instance.cam.transform.forward * (GameplayControls.Instance.throwForce * throwForceMultiplier) * (1f + UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.throwPowerModifier));
			this.rb.AddForce(vector, 2);
			string name = base.name;
			string text = " add force of ";
			Vector3 vector2 = vector;
			Game.Log(name + text + vector2.ToString(), 2);
		}
		this.minimumPhysicsTime = 0f;
		Player.Instance.UpdateSkinWidth();
	}

	// Token: 0x06001750 RID: 5968 RVA: 0x0015EDFC File Offset: 0x0015CFFC
	private void OnCollisionEnter(Collision collision)
	{
		if (this.rb == null)
		{
			return;
		}
		PhysicsProfile physicsProfile = this.interactable.preset.GetPhysicsProfile();
		float throwDamageMultiplier = physicsProfile.throwDamageMultiplier;
		float num = collision.relativeVelocity.magnitude * this.rb.mass * 0.015f * throwDamageMultiplier;
		if (num < 0.002f)
		{
			return;
		}
		bool flag = true;
		float volumeOverride = Mathf.Lerp(0.1f, 1f, num * 10f);
		string[] array = new string[8];
		array[0] = "Object: ";
		array[1] = base.name;
		array[2] = " thrown by ";
		int num2 = 3;
		Actor actor = this.thrownBy;
		array[num2] = ((actor != null) ? actor.ToString() : null);
		array[4] = " collides with ";
		array[5] = collision.collider.name;
		array[6] = " with damage: ";
		array[7] = num.ToString();
		Game.Log(string.Concat(array), 2);
		Actor soundMaker = this.thrownBy;
		if (physicsProfile.treatAsCausedByPlayer && soundMaker == null)
		{
			soundMaker = Player.Instance;
		}
		if (!this.damageEligable && soundMaker == Player.Instance)
		{
			Predicate<NewAIController.TrackingTarget> <>9__0;
			foreach (Actor actor2 in this.interactable.node.room.currentOccupants)
			{
				if (!(actor2 == soundMaker) && !(actor2.ai == null) && !(actor2.speechController == null) && !actor2.isDead && !actor2.isStunned && !actor2.isAsleep && actor2.locationsOfAuthority.Contains(this.interactable.node.gameLocation))
				{
					List<NewAIController.TrackingTarget> trackedTargets = actor2.ai.trackedTargets;
					Predicate<NewAIController.TrackingTarget> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = ((NewAIController.TrackingTarget item) => item.actor == soundMaker));
					}
					if (trackedTargets.Exists(predicate) && actor2.speechController.speechQueue.Count <= 0)
					{
						actor2.speechController.TriggerBark(SpeechController.Bark.confrontMessingAround);
						break;
					}
				}
			}
		}
		BreakableWindowController component = collision.collider.gameObject.GetComponent<BreakableWindowController>();
		if (component != null && !component.isBroken)
		{
			component.InteractableCollision(collision, num, soundMaker, this.interactable);
			if (component.isBroken)
			{
				this.rb.AddForce(collision.relativeVelocity * -0.66f, 2);
				flag = false;
			}
		}
		if (this.damageEligable)
		{
			Human hitCit = collision.collider.gameObject.GetComponentInParent<Human>();
			if (hitCit != null && hitCit != this.thrownBy)
			{
				if (num >= hitCit.maximumHealth * 0.1f && !hitCit.isPlayer)
				{
					hitCit.RecieveDamage(num, this.thrownBy, collision.GetContact(0).point, this.rb.velocity, CriminalControls.Instance.punchSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, true, true, Toolbox.Instance.Rand(0.06f, 0.11f, false), 1f, false, true, 1f);
				}
				else
				{
					if (hitCit.isPlayer)
					{
						num *= GameplayControls.Instance.incomingPlayerPhysicsDamageMultiplier;
					}
					hitCit.RecieveDamage(num, this.thrownBy, collision.GetContact(0).point, this.rb.velocity, CriminalControls.Instance.punchSpatter, null, SpatterSimulation.EraseMode.useDespawnTime, false, false, 0f, 1f, false, true, 1f);
				}
				Predicate<Case.ResolveQuestion> <>9__1;
				foreach (Case @case in CasePanelController.Instance.activeCases)
				{
					List<Case.ResolveQuestion> resolveQuestions = @case.resolveQuestions;
					Predicate<Case.ResolveQuestion> predicate2;
					if ((predicate2 = <>9__1) == null)
					{
						predicate2 = (<>9__1 = ((Case.ResolveQuestion item) => item.revengeObjTarget == hitCit.humanID && item.revengeObjective == "PurpThrowFood"));
					}
					foreach (Case.ResolveQuestion resolveQuestion in resolveQuestions.FindAll(predicate2))
					{
						Game.Log("Jobs: Manually complete throw food objective", 2);
						resolveQuestion.completedRevenge = true;
						resolveQuestion.UpdateValid(@case);
						if (hitCit.evidenceEntry.GetTiedKeys(Evidence.DataKey.name).Contains(Evidence.DataKey.photo))
						{
							resolveQuestion.SetProgress(1f, false);
						}
						@case.ValidationCheck();
					}
				}
			}
			this.damageEligable = false;
		}
		AudioController.SoundMaterialOverride soundMaterialOverride = null;
		if (this.interactable != null && physicsProfile.physicsCollisionAudio != null)
		{
			MaterialOverrideController component2 = collision.transform.GetComponent<MaterialOverrideController>();
			if (component2 != null)
			{
				soundMaterialOverride = new AudioController.SoundMaterialOverride(component2.concrete, component2.wood, component2.carpet, component2.tile, component2.plaster, component2.fabric, component2.metal, component2.glass);
			}
			else
			{
				MeshFilter component3 = collision.transform.GetComponent<MeshFilter>();
				if (component3 != null)
				{
					FurniturePreset furnitureFromMesh = Toolbox.Instance.GetFurnitureFromMesh(component3.sharedMesh);
					if (furnitureFromMesh != null)
					{
						soundMaterialOverride = new AudioController.SoundMaterialOverride(furnitureFromMesh.concrete, furnitureFromMesh.wood, furnitureFromMesh.carpet, furnitureFromMesh.tile, furnitureFromMesh.plaster, furnitureFromMesh.fabric, furnitureFromMesh.metal, furnitureFromMesh.glass);
					}
				}
			}
			if (soundMaterialOverride == null && this.interactable.node != null)
			{
				soundMaterialOverride = new AudioController.SoundMaterialOverride(this.interactable.node.room.floorMaterial.concrete, this.interactable.node.room.floorMaterial.wood, this.interactable.node.room.floorMaterial.carpet, this.interactable.node.room.floorMaterial.tile, this.interactable.node.room.floorMaterial.plaster, this.interactable.node.room.floorMaterial.fabric, this.interactable.node.room.floorMaterial.metal, this.interactable.node.room.floorMaterial.glass);
			}
			if (flag)
			{
				AudioController instance = AudioController.Instance;
				AudioEvent physicsCollisionAudio = physicsProfile.physicsCollisionAudio;
				Actor soundMaker2 = soundMaker;
				NewNode node = this.interactable.node;
				Vector3 position = base.transform.position;
				Interactable interactable = this.interactable;
				List<AudioController.FMODParam> parameters = null;
				AudioController.SoundMaterialOverride surfaceData = soundMaterialOverride;
				instance.PlayWorldOneShot(physicsCollisionAudio, soundMaker2, node, position, interactable, parameters, volumeOverride, null, false, surfaceData, false);
			}
		}
		if (collision.collider.gameObject.CompareTag("Garbage"))
		{
			Game.Log("Garbage collision", 2);
			if (CleanupController.Instance.trash.Contains(this.interactable))
			{
				int num3 = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.streetCleaningMoney));
				if (num3 > 0)
				{
					this.interactable.SafeDelete(false);
					GameplayController.Instance.AddMoney(num3, true, "readingforstreetcleaning");
				}
				if (AchievementsController.Instance != null)
				{
					AchievementsController.Instance.AddToStat("Spick-and-Span", "junk_disposed", 1);
				}
			}
			else
			{
				Game.Log("This is not trash", 2);
			}
		}
		if (this.interactable.preset.particleProfile != null)
		{
			foreach (AudioEvent eventPreset in this.interactable.preset.particleProfile.impactEvents)
			{
				AudioController.Instance.PlayWorldOneShot(eventPreset, soundMaker, this.interactable.node, base.transform.position, this.interactable, null, 1f, null, false, soundMaterialOverride, false);
			}
			if (this.interactable.preset.particleProfile.spatterTrigger == ParticleEffect.SpatterTrigger.onAnyImpact || this.interactable.preset.particleProfile.spatterTrigger == ParticleEffect.SpatterTrigger.whileInAirOrAnyImpact)
			{
				this.Spatter(collision.GetContact(0).point);
			}
			if (this.interactable.preset.particleProfile.creationTrigger == ParticleEffect.SpatterTrigger.onAnyImpact || this.interactable.preset.particleProfile.creationTrigger == ParticleEffect.SpatterTrigger.whileInAirOrAnyImpact)
			{
				this.ParticleObjectCreation();
			}
		}
		if (this.interactable.preset.breakable && this.interactable.preset.particleProfile != null && this.interactable.preset.particleProfile.deleteObject && !this.broken && num >= this.interactable.preset.particleProfile.damageBreakPoint)
		{
			foreach (AudioEvent eventPreset2 in this.interactable.preset.particleProfile.breakEvents)
			{
				AudioController.Instance.PlayWorldOneShot(eventPreset2, soundMaker, this.interactable.node, base.transform.position, this.interactable, null, 1f, null, false, soundMaterialOverride, false);
			}
			ContactPoint contact = collision.GetContact(0);
			this.BreakObject(contact.point, contact.normal, collision.relativeVelocity.magnitude, soundMaker);
		}
	}

	// Token: 0x06001751 RID: 5969 RVA: 0x0015F7F8 File Offset: 0x0015D9F8
	public void BreakObject(Vector3 contactPoint, Vector3 normal, float magnitude, Actor breaker)
	{
		if (this.interactable.preset.particleProfile.effectPrefab != null)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.interactable.preset.particleProfile.effectPrefab, PrefabControls.Instance.mapContainer);
			gameObject.transform.position = contactPoint;
			gameObject.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
		}
		this.broken = true;
		if (this.interactable.preset.particleProfile.shatter)
		{
			this.Shatter(contactPoint, magnitude);
		}
		if (this.interactable.preset.particleProfile != null)
		{
			if (this.interactable.preset.particleProfile.spatterTrigger == ParticleEffect.SpatterTrigger.onBreak)
			{
				this.Spatter(contactPoint);
			}
			if (this.interactable.preset.particleProfile.creationTrigger == ParticleEffect.SpatterTrigger.onBreak)
			{
				this.ParticleObjectCreation();
			}
		}
		if (breaker != null && breaker.isPlayer && this.interactable.spawnNode != null && this.interactable.spawnNode.gameLocation != null && this.interactable.spawnNode.gameLocation.thisAsAddress != null && InteractionController.Instance.GetValidPlayerActionIllegal(this.interactable, this.interactable.spawnNode, false, true) && this.interactable.val > 1f)
		{
			this.interactable.spawnNode.gameLocation.thisAsAddress.AddVandalism(this.interactable);
			StatusController.Instance.AddFineRecord(this.interactable.spawnNode.gameLocation.thisAsAddress, this.interactable, StatusController.CrimeType.vandalism, true, Mathf.RoundToInt(this.interactable.val * (float)GameplayControls.Instance.vandalismFineMultiplier), false);
			InteractionController.Instance.SetIllegalActionActive(true);
		}
		this.interactable.MarkAsTrash(true, false, 0f);
		this.interactable.RemoveFromPlacement();
	}

	// Token: 0x06001752 RID: 5970 RVA: 0x0015FA08 File Offset: 0x0015DC08
	public void Spatter(Vector3 target)
	{
		if (this.interactable.preset.particleProfile.spatter != null)
		{
			SpatterPatternPreset spatter = this.interactable.preset.particleProfile.spatter;
			float newSpatterCountMultiplier = this.interactable.preset.particleProfile.countMultiplier;
			bool stickToActors = this.interactable.preset.particleProfile.stickToActors;
			if (this.interactable.preset.overrideSpatterSettings)
			{
				if (this.interactable.preset.spatterSimulation != null)
				{
					spatter = this.interactable.preset.spatterSimulation;
				}
				newSpatterCountMultiplier = this.interactable.preset.spatterCountMultiplier;
			}
			new SpatterSimulation(this.meshes[0].bounds.center, target, spatter, SpatterSimulation.EraseMode.onceExecutedAndOutOfAddressPlusDespawnTime, newSpatterCountMultiplier, stickToActors);
			if (this.interactable.preset.particleProfile.spatterIsVandalism)
			{
				Vector3Int vector3Int = CityData.Instance.RealPosToNodeInt(this.meshes[0].bounds.center);
				NewNode newNode = null;
				if (PathFinder.Instance.nodeMap.TryGetValue(vector3Int, ref newNode) && newNode.gameLocation != null && newNode.gameLocation.thisAsAddress != null && InteractionController.Instance.GetValidPlayerActionIllegal(this.interactable, newNode, false, true))
				{
					StatusController.Instance.AddFineRecord(newNode.gameLocation.thisAsAddress, null, StatusController.CrimeType.vandalism, true, this.interactable.preset.particleProfile.vandalismFine, true);
					newNode.gameLocation.thisAsAddress.AddVandalism(this.interactable.preset.particleProfile.vandalismFine);
					InteractionController.Instance.SetIllegalActionActive(true);
				}
			}
		}
	}

	// Token: 0x06001753 RID: 5971 RVA: 0x0015FBE4 File Offset: 0x0015DDE4
	public void ParticleObjectCreation()
	{
		Game.Log("Particle obj creation...", 2);
		if (this.objectParticleCreationDelay > 0f)
		{
			Game.Log("Delay " + this.objectParticleCreationDelay.ToString(), 2);
			return;
		}
		for (int i = 0; i < this.interactable.preset.particleProfile.instances; i++)
		{
			GameObject gameObject = Object.Instantiate<GameObject>(this.interactable.preset.particleProfile.objectPool[Toolbox.Instance.Rand(0, this.interactable.preset.particleProfile.objectPool.Count, false)], PrefabControls.Instance.mapContainer);
			if (this.coll != null)
			{
				Physics.IgnoreCollision(gameObject.GetComponent<Collider>(), this.coll);
			}
			gameObject.transform.position = base.transform.position;
			if (this.interactable.preset.particleProfile.useRandomRotation)
			{
				gameObject.transform.rotation = Quaternion.Euler(new Vector3((float)Toolbox.Instance.Rand(0, 360, false), (float)Toolbox.Instance.Rand(0, 360, false), (float)Toolbox.Instance.Rand(0, 360, false)));
			}
			else
			{
				gameObject.transform.eulerAngles = base.transform.eulerAngles + this.interactable.preset.particleProfile.localEuler;
			}
			Game.Log("Create " + gameObject.name, 2);
		}
		this.objectParticleCreationDelay = 0.25f;
	}

	// Token: 0x06001754 RID: 5972 RVA: 0x0015FD88 File Offset: 0x0015DF88
	private Vector3 UvTo3D(Vector2 uv, Mesh mesh)
	{
		if (mesh == null || !mesh.isReadable)
		{
			Game.LogError("Mesh is not readable! Fingerprints cannot be gathered as the verts aren't readable...", 2);
			return Vector3.zero;
		}
		int[] triangles = mesh.triangles;
		Vector2[] uv2 = mesh.uv;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < triangles.Length; i += 3)
		{
			Vector2 vector = uv2[triangles[i]];
			Vector2 vector2 = uv2[triangles[i + 1]];
			Vector2 vector3 = uv2[triangles[i + 2]];
			float num = this.Area(vector, vector2, vector3);
			if (num != 0f)
			{
				float num2 = this.Area(vector2, vector3, uv) / num;
				if (num2 >= 0f)
				{
					float num3 = this.Area(vector3, vector, uv) / num;
					if (num3 >= 0f)
					{
						float num4 = this.Area(vector, vector2, uv) / num;
						if (num4 >= 0f)
						{
							return num2 * vertices[triangles[i]] + num3 * vertices[triangles[i + 1]] + num4 * vertices[triangles[i + 2]];
						}
					}
				}
			}
		}
		return Vector3.zero;
	}

	// Token: 0x06001755 RID: 5973 RVA: 0x0015FEB0 File Offset: 0x0015E0B0
	private float Area(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		Vector2 vector = p1 - p3;
		Vector2 vector2 = p2 - p3;
		return (vector.x * vector2.y - vector.y * vector2.x) / 2f;
	}

	// Token: 0x06001756 RID: 5974 RVA: 0x0015FEF0 File Offset: 0x0015E0F0
	public void Shatter(Vector3 contact, float force)
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		Game.Log(string.Concat(new string[]
		{
			"Shattering ",
			base.name,
			" with ",
			this.meshes.Count.ToString(),
			" meshes..."
		}), 2);
		if (this.meshes.Count > 0)
		{
			foreach (MeshRenderer meshRenderer in this.meshes)
			{
				MeshFilter component = meshRenderer.gameObject.GetComponent<MeshFilter>();
				if (component == null)
				{
					Game.Log("Misc Error: Cannot find mesh filter for " + meshRenderer.name, 2);
				}
				else
				{
					Texture2D texture2D = meshRenderer.material.GetTexture("_BaseColorMap") as Texture2D;
					if (texture2D == null)
					{
						Game.Log("Misc Error: Cannot find base Texture for " + meshRenderer.material.name, 2);
					}
					else
					{
						if (!texture2D.isReadable)
						{
							Game.LogError("Texture for " + base.name + " is not readable! Set this if you want to shatter this object...", 2);
						}
						Vector3 shardSize = this.interactable.preset.particleProfile.shardSize;
						int num = 0;
						int shardEveryXPixels = this.interactable.preset.particleProfile.shardEveryXPixels;
						int num2 = 0;
						int num3 = 0;
						if (this.interactable.preset.overrideShatterSettings)
						{
							shardSize = this.interactable.preset.shardSize;
							shardEveryXPixels = this.interactable.preset.shardEveryXPixels;
						}
						GameObject gameObject = PrefabControls.Instance.shatterShard;
						if (this.interactable.preset.particleProfile.isGlass)
						{
							gameObject = PrefabControls.Instance.glassShard;
						}
						for (int i = 0; i < texture2D.width; i++)
						{
							for (int j = 0; j < texture2D.height; j++)
							{
								Color pixel = texture2D.GetPixel(i, j);
								if (pixel != Color.black && pixel != Color.clear)
								{
									if (num2 <= 0)
									{
										Vector2 uv;
										uv..ctor((float)i / (float)texture2D.width, (float)j / (float)texture2D.height);
										GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, this.interactable.node.room.transform);
										gameObject2.transform.position = component.gameObject.transform.TransformPoint(this.UvTo3D(uv, component.mesh));
										gameObject2.transform.localScale = shardSize;
										if (this.interactable.preset.particleProfile.isGlass)
										{
											pixel.a = 0.5f;
										}
										gameObject2.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", pixel);
										num++;
										num2 = shardEveryXPixels;
										gameObject2.GetComponent<Rigidbody>().AddExplosionForce(force * this.interactable.preset.particleProfile.shatterForceMultiplier * Toolbox.Instance.Rand(0.8f, 1.5f, false), contact, component.mesh.bounds.size.magnitude);
									}
									else
									{
										num2--;
									}
									num3++;
								}
							}
						}
						Game.Log("...Created " + num.ToString() + " shards.", 2);
					}
				}
			}
		}
	}

	// Token: 0x06001757 RID: 5975 RVA: 0x0016028C File Offset: 0x0015E48C
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Garbage"))
		{
			Game.Log("Garbage collision...", 2);
			if (CleanupController.Instance.trash.Contains(this.interactable))
			{
				this.interactable.SafeDelete(false);
				int num = Mathf.RoundToInt(UpgradeEffectController.Instance.GetUpgradeEffect(SyncDiskPreset.Effect.streetCleaningMoney));
				if (num > 0)
				{
					GameplayController.Instance.AddMoney(num, true, "readingforstreetcleaning");
				}
				if (AchievementsController.Instance != null)
				{
					AchievementsController.Instance.AddToStat("Spick-and-Span", "junk_disposed", 1);
					return;
				}
			}
			else if (base.gameObject.layer == 24)
			{
				Citizen cit = base.gameObject.GetComponentInParent<Citizen>();
				if (cit != null)
				{
					if (cit.isDead)
					{
						using (List<Case>.Enumerator enumerator = CasePanelController.Instance.activeCases.GetEnumerator())
						{
							Predicate<Objective.ObjectiveTrigger> <>9__0;
							while (enumerator.MoveNext())
							{
								Case @case = enumerator.Current;
								foreach (Objective objective in @case.currentActiveObjectives)
								{
									List<Objective.ObjectiveTrigger> triggers = objective.queueElement.triggers;
									Predicate<Objective.ObjectiveTrigger> predicate;
									if ((predicate = <>9__0) == null)
									{
										predicate = (<>9__0 = ((Objective.ObjectiveTrigger item) => item.triggerType == Objective.ObjectiveTriggerType.disposeOfBody && item.evidenceID == cit.evidenceEntry.evID));
									}
									Objective.ObjectiveTrigger objectiveTrigger = triggers.Find(predicate);
									if (objectiveTrigger != null && !objectiveTrigger.triggered && !objective.isCancelled)
									{
										Game.Log("Murder: Trash trigger: Successful disposal of body in dumpster", 2);
										objectiveTrigger.Trigger(false);
									}
								}
							}
							return;
						}
					}
					if (Game.Instance.printDebug)
					{
						Game.Log("This is not trash: " + base.name, 2);
						return;
					}
				}
				else if (Game.Instance.printDebug)
				{
					Game.Log("This is not trash: " + base.name, 2);
					return;
				}
			}
			else if (Game.Instance.printDebug)
			{
				Game.Log("This is not trash: " + base.name, 2);
			}
		}
	}

	// Token: 0x06001758 RID: 5976 RVA: 0x001604BC File Offset: 0x0015E6BC
	public void SetPhysics(bool val, Actor setThrownBy = null)
	{
		this.physicsOn = val;
		if (this.physicsOn)
		{
			base.enabled = true;
			this.thrownBy = setThrownBy;
			if (this.coll != null)
			{
				this.wasTrigger = this.coll.isTrigger;
			}
			this.coll.isTrigger = false;
			if (this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.stovetopKettle)
			{
				this.interactable.SetSwitchState(false, null, true, false, false);
			}
			if (this.rb == null)
			{
				if (this.alternativePhysicsParent != null)
				{
					this.rb = this.alternativePhysicsParent.gameObject.AddComponent<Rigidbody>();
				}
				else
				{
					this.rb = base.gameObject.AddComponent<Rigidbody>();
				}
				PhysicsProfile physicsProfile = this.interactable.preset.GetPhysicsProfile();
				this.rb.mass = physicsProfile.mass;
				this.rb.drag = physicsProfile.drag;
				this.rb.angularDrag = physicsProfile.angularDrag;
				this.rb.collisionDetectionMode = physicsProfile.collisionMode;
				if (this.interactable.preset.overrideMass)
				{
					this.rb.mass = this.interactable.preset.mass;
				}
				this.rb.interpolation = GameplayControls.Instance.interpolation;
			}
			if (!GameplayController.Instance.activePhysics.Contains(this.interactable))
			{
				GameplayController.Instance.activePhysics.Add(this.interactable);
				Player.Instance.UpdateCulling();
				return;
			}
		}
		else
		{
			if (GameplayController.Instance.activePhysics.Contains(this.interactable))
			{
				GameplayController.Instance.activePhysics.Remove(this.interactable);
				Player.Instance.UpdateCullingOnEndOfFrame();
			}
			this.objectParticleCreationDelay = 0f;
			this.minimumPhysicsTime = 0f;
			this.damageEligable = false;
			if (this.wasTrigger && this.coll != null)
			{
				this.coll.isTrigger = true;
			}
			if (this.rb != null)
			{
				Object.Destroy(this.rb);
			}
			if (this.interactable.preset.specialCaseFlag == InteractablePreset.SpecialCase.stovetopKettle && this.interactable.node != null)
			{
				FurniturePreset.SubObject subObject = null;
				FurnitureLocation furnitureLocation = null;
				foreach (FurnitureLocation furnitureLocation2 in this.interactable.node.individualFurniture)
				{
					subObject = furnitureLocation2.furniture.subObjects.Find((FurniturePreset.SubObject item) => item.preset.name == "HobBoilPoint");
					if (subObject != null)
					{
						Interactable interactable = furnitureLocation2.integratedInteractables.Find((Interactable item) => item.preset.actionsPreset.Exists((InteractableActionsPreset item) => item.name == "Cooker"));
						if (interactable != null && interactable.sw0)
						{
							furnitureLocation = furnitureLocation2;
							break;
						}
					}
				}
				if (subObject != null && furnitureLocation != null && Vector3.Distance(Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(new Vector3(0f, (float)(furnitureLocation.angle + furnitureLocation.diagonalAngle), 0f)), Vector3.one).MultiplyPoint3x4(subObject.localPos) + furnitureLocation.anchorNode.position, this.interactable.GetWorldPosition(true)) < 0.3f)
				{
					this.interactable.SetSwitchState(true, null, true, false, false);
				}
			}
			this.thrownBy = null;
			base.enabled = false;
		}
	}

	// Token: 0x06001759 RID: 5977 RVA: 0x00160858 File Offset: 0x0015EA58
	public void SetVisible(bool val, bool forceUpdate = false)
	{
		if (val != this.isVisible || forceUpdate)
		{
			this.isVisible = val;
			if (this.interactable.printDebug)
			{
				Game.Log("Interactable " + this.interactable.name + " set visible: " + this.isVisible.ToString(), 2);
			}
			for (int i = 0; i < this.meshes.Count; i++)
			{
				if (this.meshes[i] != null)
				{
					this.meshes[i].enabled = this.isVisible;
				}
				else
				{
					this.meshes.RemoveAt(i);
					i--;
				}
			}
		}
	}

	// Token: 0x0600175A RID: 5978 RVA: 0x0016090C File Offset: 0x0015EB0C
	private void UpdateLastMovement()
	{
		if (this.interactable != null)
		{
			this.interactable.lma = SessionData.Instance.gameTime;
			if (this.coll == null)
			{
				this.coll = base.transform.gameObject.GetComponent<Collider>();
			}
			this.interactable.MoveInteractable(base.transform.position, base.transform.eulerAngles, false);
		}
	}

	// Token: 0x0600175B RID: 5979 RVA: 0x0016097C File Offset: 0x0015EB7C
	public void UpdateParticleSystemDistance()
	{
		if (Vector3.Distance(base.transform.position, Player.Instance.transform.position) <= this.particleSystemDistance)
		{
			if (!this.partSystem.gameObject.activeSelf)
			{
				this.partSystem.gameObject.SetActive(true);
				this.partSystem.Play();
				return;
			}
		}
		else if (this.partSystem.gameObject.activeSelf)
		{
			this.partSystem.Pause();
			this.partSystem.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600175C RID: 5980 RVA: 0x00160A10 File Offset: 0x0015EC10
	public void State1Change()
	{
		if (this.interactable.sw1)
		{
			if (this.phoneReciever != null)
			{
				this.phoneReciever.SetActive(false);
				return;
			}
		}
		else if (this.phoneReciever != null)
		{
			this.phoneReciever.SetActive(true);
		}
	}

	// Token: 0x0600175D RID: 5981 RVA: 0x00160A60 File Offset: 0x0015EC60
	[Button(null, 0)]
	public void DisplayCCTVVectors()
	{
		Vector3 vector = Quaternion.Euler(this.interactable.cve) * Vector3.forward;
		Debug.DrawRay(this.interactable.cvp, vector, Color.red, 6f);
	}

	// Token: 0x0600175E RID: 5982 RVA: 0x00160AA3 File Offset: 0x0015ECA3
	[Button(null, 0)]
	public void RefreshCCTVCoveredArea()
	{
		if (this.interactable.sceneRecorder != null)
		{
			this.interactable.sceneRecorder.RefreshCoveredArea();
			this.DisplayCCTVViewNodes();
		}
	}

	// Token: 0x0600175F RID: 5983 RVA: 0x00160AC8 File Offset: 0x0015ECC8
	[Button(null, 0)]
	public void DisplayCCTVViewNodes()
	{
		if (this.interactable.sceneRecorder != null)
		{
			Game.Log("Camera covers " + this.interactable.sceneRecorder.coveredNodes.Count.ToString() + " nodes", 2);
			foreach (KeyValuePair<NewNode, List<int>> keyValuePair in this.interactable.sceneRecorder.coveredNodes)
			{
				Debug.DrawRay(this.interactable.cvp, keyValuePair.Key.position - this.interactable.cvp, Color.blue, 6f);
			}
		}
	}

	// Token: 0x06001760 RID: 5984 RVA: 0x00160B98 File Offset: 0x0015ED98
	[Button(null, 0)]
	public void UpdateSaveFlags()
	{
		this.willBeSavedWithCity = this.interactable.save;
		this.willBeSavedWithState = this.interactable.IsSaveStateEligable();
		this.isMainLight = this.interactable.ml;
	}

	// Token: 0x06001761 RID: 5985 RVA: 0x00160BD0 File Offset: 0x0015EDD0
	[Button(null, 0)]
	public void GetSaveStateEligable()
	{
		Game.Log(this.interactable.IsSaveStateEligable().ToString() + " (moved: " + this.interactable.mov.ToString() + ")", 2);
	}

	// Token: 0x06001762 RID: 5986 RVA: 0x00160C15 File Offset: 0x0015EE15
	[Button(null, 0)]
	public void WasThisLoadedFromSaveGameData()
	{
		Game.Log(this.interactable.wasLoadedFromSave, 2);
	}

	// Token: 0x06001763 RID: 5987 RVA: 0x00160C30 File Offset: 0x0015EE30
	[Button(null, 0)]
	public void SetupInteractable()
	{
		if (this.meshes == null || this.meshes.Count <= 0)
		{
			this.meshes = Enumerable.ToList<MeshRenderer>(base.gameObject.GetComponentsInChildren<MeshRenderer>());
		}
		if (this.rb == null)
		{
			this.rb = base.gameObject.GetComponent<Rigidbody>();
		}
		if (this.coll == null)
		{
			this.coll = base.gameObject.GetComponent<Collider>();
		}
		if (this.coll != null)
		{
			MeshCollider meshCollider = this.coll as MeshCollider;
			if (meshCollider != null)
			{
				meshCollider.convex = true;
			}
		}
		if (this.coll != null && this.lightController != null)
		{
			this.coll.gameObject.layer = 23;
		}
		if (this.lod == null)
		{
			this.lod = base.gameObject.GetComponentInChildren<LODGroup>();
		}
		if (this.lod == null && this.meshes != null && this.meshes.Count > 0)
		{
			this.lod = base.gameObject.AddComponent<LODGroup>();
			LOD lod = default(LOD);
			Renderer[] renderers = this.meshes.ToArray();
			lod.renderers = renderers;
			lod.screenRelativeTransitionHeight = 0.01f;
			this.lod.SetLODs(new LOD[]
			{
				lod
			});
			this.lod.RecalculateBounds();
		}
	}

	// Token: 0x06001764 RID: 5988 RVA: 0x00160DA0 File Offset: 0x0015EFA0
	[Button(null, 0)]
	public void IsOnPoolList()
	{
		string text = "Interactable controller reference: ";
		InteractableController controller = this.interactable.controller;
		Debug.Log(text + ((controller != null) ? controller.ToString() : null));
		Debug.Log("Is on range checking (load) list: " + ObjectPoolingController.Instance.interactableRangeToLoadList.Contains(this.interactable).ToString());
		Debug.Log("Is on range checking (enable/disable) list: " + ObjectPoolingController.Instance.interactableRangeToEnableDisableList.Contains(this.interactable).ToString());
		float num = 0f;
		Debug.Log("Current visibility range status: " + ObjectPoolingController.Instance.SpawnRangeCheck(this.interactable, out num).ToString() + " distance: " + num.ToString());
		Debug.Log("Geometry loaded: " + this.interactable.loadedGeometry.ToString());
	}

	// Token: 0x06001765 RID: 5989 RVA: 0x00160E88 File Offset: 0x0015F088
	[Button("Load Furniture's Nodespace Area", 0)]
	public void LoadWalkable()
	{
		if (this.interactable.furnitureParent != null)
		{
			foreach (NewNode newNode in this.interactable.furnitureParent.coversNodes)
			{
				foreach (KeyValuePair<Vector3, NewNode.NodeSpace> keyValuePair in newNode.walkableNodeSpace)
				{
					GameObject gameObject = GameObject.CreatePrimitive(0);
					gameObject.transform.SetParent(base.gameObject.transform);
					gameObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
					gameObject.transform.position = keyValuePair.Value.position;
				}
			}
		}
	}

	// Token: 0x06001766 RID: 5990 RVA: 0x00160F80 File Offset: 0x0015F180
	[Button("List Users", 0)]
	public void ListUsers()
	{
		foreach (KeyValuePair<Interactable.UsePointSlot, Human> keyValuePair in this.interactable.usagePoint.users)
		{
			string text = "Usage slot ";
			string text2 = keyValuePair.Key.ToString();
			string text3 = ": ";
			Human value = keyValuePair.Value;
			Game.Log(text + text2 + text3 + ((value != null) ? value.ToString() : null), 2);
		}
		string text4 = "Reserved: ";
		GroupsController.SocialGroup reserved = this.interactable.usagePoint.reserved;
		Game.Log(text4 + ((reserved != null) ? reserved.ToString() : null), 2);
	}

	// Token: 0x06001767 RID: 5991 RVA: 0x00161040 File Offset: 0x0015F240
	[Button(null, 0)]
	public void CalculateLocalFurniturePostion()
	{
		if (this.interactable.furnitureParent != null)
		{
			Game.Log(this.interactable.furnitureParent.GetSubObjectLocalPosition(this.interactable.subObject), 2);
		}
	}

	// Token: 0x06001768 RID: 5992 RVA: 0x00161078 File Offset: 0x0015F278
	[Button(null, 0)]
	public void TogglePrintDebug()
	{
		if (this.interactable != null)
		{
			this.interactable.printDebug = !this.interactable.printDebug;
			Game.Log("Print debug for " + this.interactable.name + ": " + this.interactable.printDebug.ToString(), 2);
		}
	}

	// Token: 0x06001769 RID: 5993 RVA: 0x001610D8 File Offset: 0x0015F2D8
	[Button(null, 0)]
	public void Explode()
	{
		this.BreakObject(this.meshes[0].bounds.center, Vector3.zero, 8f, null);
	}

	// Token: 0x0600176A RID: 5994 RVA: 0x00161110 File Offset: 0x0015F310
	[Button(null, 0)]
	public void GetLocalizedSnapshot()
	{
		Interactable localizedSnapshot = Toolbox.Instance.GetLocalizedSnapshot(this.interactable);
		if (localizedSnapshot != null)
		{
			ActionController.Instance.Inspect(localizedSnapshot, Player.Instance.currentNode, Player.Instance);
		}
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x0016114B File Offset: 0x0015F34B
	[Button(null, 0)]
	public void UpdateName()
	{
		this.interactable.UpdateName(false, Evidence.DataKey.name);
	}

	// Token: 0x04001C63 RID: 7267
	[NonSerialized]
	public Interactable interactable;

	// Token: 0x04001C64 RID: 7268
	[Tooltip("In-editor, set the ID here. This will be used by the preset to identify pairing for interactables.")]
	[Header("Editor Setup")]
	public InteractableController.InteractableID id;

	// Token: 0x04001C65 RID: 7269
	[Header("Components")]
	public List<MeshRenderer> meshes = new List<MeshRenderer>();

	// Token: 0x04001C66 RID: 7270
	public LODGroup lod;

	// Token: 0x04001C67 RID: 7271
	public Rigidbody rb;

	// Token: 0x04001C68 RID: 7272
	public Collider coll;

	// Token: 0x04001C69 RID: 7273
	public Collider altColl;

	// Token: 0x04001C6A RID: 7274
	public List<Collider> additionalPhysicsOnlyColliders = new List<Collider>();

	// Token: 0x04001C6B RID: 7275
	public Transform alternativePhysicsParent;

	// Token: 0x04001C6C RID: 7276
	public WorldFlashController flash;

	// Token: 0x04001C6D RID: 7277
	public DoorMovementController doorMovement;

	// Token: 0x04001C6E RID: 7278
	public DoorMovementController secondaryDoorMovement;

	// Token: 0x04001C6F RID: 7279
	public DoorMovementController thirdDoorMovement;

	// Token: 0x04001C70 RID: 7280
	public LightController lightController;

	// Token: 0x04001C71 RID: 7281
	public SteamController steam;

	// Token: 0x04001C72 RID: 7282
	public ComputerController computer;

	// Token: 0x04001C73 RID: 7283
	public SecuritySystem securitySystem;

	// Token: 0x04001C74 RID: 7284
	public FileSystemController fileSystem;

	// Token: 0x04001C75 RID: 7285
	public DecalProjector decalProjector;

	// Token: 0x04001C76 RID: 7286
	public List<Transform> pages;

	// Token: 0x04001C77 RID: 7287
	public ParticleSystem partSystem;

	// Token: 0x04001C78 RID: 7288
	public EchelonsLaserScreenController echelonsScreen;

	// Token: 0x04001C79 RID: 7289
	public bool useSmokeMaterial;

	// Token: 0x04001C7A RID: 7290
	public Transform lockParentOverride;

	// Token: 0x04001C7B RID: 7291
	public Vector3 lockedInTransformOffset = Vector3.zero;

	// Token: 0x04001C7C RID: 7292
	[Space(5f)]
	[Tooltip("Automatically sync these on/off depending on switch state.")]
	public bool enableSwitchSync;

	// Token: 0x04001C7D RID: 7293
	public List<SwitchSyncBehaviour> switchSyncObjects = new List<SwitchSyncBehaviour>();

	// Token: 0x04001C7E RID: 7294
	[Header("State")]
	[Tooltip("True if currently being carried by the player")]
	public bool isVisible = true;

	// Token: 0x04001C7F RID: 7295
	public bool isCarriedByPlayer;

	// Token: 0x04001C80 RID: 7296
	private float carryProgress;

	// Token: 0x04001C81 RID: 7297
	private float rotProgress;

	// Token: 0x04001C82 RID: 7298
	private Vector3 pickupPos;

	// Token: 0x04001C83 RID: 7299
	private Quaternion pickupRot;

	// Token: 0x04001C84 RID: 7300
	private Vector3 heldEuler;

	// Token: 0x04001C85 RID: 7301
	private bool setHeldEuler;

	// Token: 0x04001C86 RID: 7302
	[Tooltip("True if the physics are currently active")]
	public bool physicsOn;

	// Token: 0x04001C87 RID: 7303
	[Tooltip("For measuring time after physics movement")]
	public float minimumPhysicsTime;

	// Token: 0x04001C88 RID: 7304
	public bool damageEligable;

	// Token: 0x04001C89 RID: 7305
	public bool wasTrigger;

	// Token: 0x04001C8A RID: 7306
	public Actor thrownBy;

	// Token: 0x04001C8B RID: 7307
	private float objectParticleCreationDelay;

	// Token: 0x04001C8C RID: 7308
	private Vector3 colliderExtents;

	// Token: 0x04001C8D RID: 7309
	public bool apartmentPlacementIsValid;

	// Token: 0x04001C8E RID: 7310
	[Header("Interactions")]
	[Tooltip("Look at when interacting (if null then use centre)")]
	public Transform lookAtTarget;

	// Token: 0x04001C8F RID: 7311
	[Tooltip("The interaction window for this object")]
	public InfoWindow interactionWindow;

	// Token: 0x04001C90 RID: 7312
	[Header("Special Cases")]
	[Tooltip("Use this flag for quickly checking if they player is looking at a door")]
	public NewDoor isDoor;

	// Token: 0x04001C91 RID: 7313
	public Actor isActor;

	// Token: 0x04001C92 RID: 7314
	public Human belongsTo;

	// Token: 0x04001C93 RID: 7315
	public bool isPhone;

	// Token: 0x04001C94 RID: 7316
	public GameObject phoneReciever;

	// Token: 0x04001C95 RID: 7317
	public float particleSystemDistance = 20f;

	// Token: 0x04001C96 RID: 7318
	public bool willBeSavedWithCity;

	// Token: 0x04001C97 RID: 7319
	public bool willBeSavedWithState;

	// Token: 0x04001C98 RID: 7320
	public bool isMainLight;

	// Token: 0x04001C99 RID: 7321
	private bool broken;

	// Token: 0x04001C9A RID: 7322
	[Header("Debug")]
	public List<Interactable> debugInteractable = new List<Interactable>();

	// Token: 0x04001C9B RID: 7323
	[Tooltip("Angle of furniture parent")]
	public int debugAngle;

	// Token: 0x04001C9C RID: 7324
	public Vector3 debugFurnitureAnchorNodePos;

	// Token: 0x04001C9D RID: 7325
	[Tooltip("Local position of this, should match the transform")]
	public Vector3 debugLocalPos;

	// Token: 0x04001C9E RID: 7326
	[Tooltip("Local euler of this, should match the transform")]
	public Vector3 debugLocalEuler;

	// Token: 0x04001C9F RID: 7327
	public Vector3 debugWorldPos;

	// Token: 0x04001CA0 RID: 7328
	public Vector3 debugInteractablePredictedWorldPos;

	// Token: 0x04001CA1 RID: 7329
	[Tooltip("Interactable node")]
	public Vector3 debugNodeCoord;

	// Token: 0x04001CA2 RID: 7330
	[Tooltip("The usage point")]
	public Interactable.UsagePoint debugUsagePoint;

	// Token: 0x04001CA3 RID: 7331
	public Human debugOwnedBy;

	// Token: 0x04001CA4 RID: 7332
	public Human debugWrittenBy;

	// Token: 0x04001CA5 RID: 7333
	public Human debugReceivedBy;

	// Token: 0x04001CA6 RID: 7334
	public object debugPasswordSource;

	// Token: 0x04001CA7 RID: 7335
	public List<MonoBehaviour> debugFurnitureOwnedBy;

	// Token: 0x04001CA8 RID: 7336
	public bool debugSwitchState;

	// Token: 0x04001CA9 RID: 7337
	public bool debugState1;

	// Token: 0x04001CAA RID: 7338
	public NewRoom debugRoom;

	// Token: 0x04001CAB RID: 7339
	public AirDuctGroup.AirVent debugVent;

	// Token: 0x020003FD RID: 1021
	public enum InteractableID
	{
		// Token: 0x04001CAD RID: 7341
		A,
		// Token: 0x04001CAE RID: 7342
		B,
		// Token: 0x04001CAF RID: 7343
		C,
		// Token: 0x04001CB0 RID: 7344
		D,
		// Token: 0x04001CB1 RID: 7345
		E,
		// Token: 0x04001CB2 RID: 7346
		F,
		// Token: 0x04001CB3 RID: 7347
		G,
		// Token: 0x04001CB4 RID: 7348
		H,
		// Token: 0x04001CB5 RID: 7349
		I,
		// Token: 0x04001CB6 RID: 7350
		J,
		// Token: 0x04001CB7 RID: 7351
		hidingPlace,
		// Token: 0x04001CB8 RID: 7352
		none,
		// Token: 0x04001CB9 RID: 7353
		K,
		// Token: 0x04001CBA RID: 7354
		L,
		// Token: 0x04001CBB RID: 7355
		M,
		// Token: 0x04001CBC RID: 7356
		N,
		// Token: 0x04001CBD RID: 7357
		O,
		// Token: 0x04001CBE RID: 7358
		P,
		// Token: 0x04001CBF RID: 7359
		Q,
		// Token: 0x04001CC0 RID: 7360
		R,
		// Token: 0x04001CC1 RID: 7361
		S,
		// Token: 0x04001CC2 RID: 7362
		T,
		// Token: 0x04001CC3 RID: 7363
		U,
		// Token: 0x04001CC4 RID: 7364
		V,
		// Token: 0x04001CC5 RID: 7365
		W,
		// Token: 0x04001CC6 RID: 7366
		X,
		// Token: 0x04001CC7 RID: 7367
		Y,
		// Token: 0x04001CC8 RID: 7368
		Z,
		// Token: 0x04001CC9 RID: 7369
		AA,
		// Token: 0x04001CCA RID: 7370
		BB,
		// Token: 0x04001CCB RID: 7371
		CC,
		// Token: 0x04001CCC RID: 7372
		DD
	}
}
